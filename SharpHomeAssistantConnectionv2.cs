
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AudreysCloud.Community.SharpHomeAssistant.Exceptions;
using AudreysCloud.Community.SharpHomeAssistant.Messages;

namespace AudreysCloud.Community.SharpHomeAssistant
{
	public sealed class SharpHomeAssistantConnectionV2
	{
		private ClientWebSocket _socket;


		private CancellationTokenSource _readChannelCancellationSource;
		private CancellationTokenSource _writeChannelCancellationSource;
		private CancellationTokenSource _disconnectingCancellationSource;

		private Channel<MemoryStream> _readChannel;
		private Channel<MemoryStream> _writeChannel;


		private CancellationTokenSource _forceShutdown;
		private JsonSerializerOptions _jsonSerializerOptions { get; set; }

		private Task _sendTask;
		private Task _receiveTask;

		public SharpHomeAssistantConnectionState State { get; private set; }
		public string AccessToken { get; set; }
		public int MaxMessageSize { get; set; }

		public SharpHomeAssistantConnectionV2()
		{
			State = SharpHomeAssistantConnectionState.NotConnected;
			MaxMessageSize = 1024 * 1024; // 1 Megabyte

			_jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
			_jsonSerializerOptions.Converters.Add(new IncomingMessageConverter());

			_forceShutdown = new CancellationTokenSource();
		}

		public async Task ConnectUsingSocketAsync(ClientWebSocket socket, CancellationToken cancellationToken)
		{
			CheckAndThrowIfNotInState(SharpHomeAssistantConnectionState.NotConnected, nameof(ConnectUsingSocketAsync));
			await DoConnectAsync(socket, cancellationToken);
		}
		public async Task ConnectAsync(Uri serverUri, CancellationToken cancellationToken)
		{
			CheckAndThrowIfNotInState(SharpHomeAssistantConnectionState.NotConnected, nameof(ConnectAsync));

			ClientWebSocket newSocket = new ClientWebSocket();

			State = SharpHomeAssistantConnectionState.Connecting;
			try
			{
				await newSocket.ConnectAsync(serverUri, cancellationToken);
			}
			catch (Exception)
			{
				//
				// We can safely mark the class as not connected at this point 
				// since no methods that have side effects have been invoked.
				//
				State = SharpHomeAssistantConnectionState.NotConnected;

				throw;
			}

			CheckAndThrowIfWebsocketNotOpen(newSocket, nameof(ConnectAsync));
			await DoConnectAsync(newSocket, cancellationToken);

		}

		public async Task CloseAsync(CancellationToken cancellationToken)
		{
			CheckAndThrowIfNotInState(SharpHomeAssistantConnectionState.Connected, nameof(CloseAsync));
			if (_socket.State != WebSocketState.Open && _socket.State != WebSocketState.CloseReceived && _socket.State != WebSocketState.CloseSent)
			{
				throw new InvalidOperationException(String.Format("Can not call {0} on {1} when in state {2}.", nameof(CloseAsync), nameof(SharpHomeAssistantConnectionV2), _socket.State));
			}

			CancellationTokenSource cancelAndAbort = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _forceShutdown.Token);

			try
			{
				//
				// End all actions on the API.
				//
				_disconnectingCancellationSource.Cancel();

				State = SharpHomeAssistantConnectionState.Closing;

				_writeChannelCancellationSource.Cancel();
				_readChannelCancellationSource.Cancel();

				await _sendTask;
				await _receiveTask;

				await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing websocket connection", cancelAndAbort.Token);

				State = SharpHomeAssistantConnectionState.NotConnected;

				_socket.Dispose();
				_socket = null;
			}
			catch
			{
				Abort();
				throw;
			}
		}

		public void Abort()
		{
			_readChannelCancellationSource?.Cancel();
			_writeChannelCancellationSource?.Cancel();
			_disconnectingCancellationSource?.Cancel();
			_forceShutdown?.Cancel();

			if (_socket != null)
			{
				_socket.Abort();
				_socket.Dispose();
				_socket = null;
			}

			State = SharpHomeAssistantConnectionState.Aborted;
		}

		public async Task SendMessageAsync<MessageType>(MessageType messageBase, CancellationToken cancellationToken) where MessageType : OutgoingMessageBase
		{
			CheckAndThrowIfNotInState(SharpHomeAssistantConnectionState.Connected, nameof(SendMessageAsync));
			CheckAndThrowIfWebsocketNotOpen(_socket, nameof(SendMessageAsync));

			CancellationTokenSource joinedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disconnectingCancellationSource.Token);

			try
			{
				await DoSendAsync(messageBase, joinedSource.Token);
			}
			catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				//Simplify the exception to just the calling codes if they cancelled. 
				cancellationToken.ThrowIfCancellationRequested();


				//Just in case and to satisfy the compilier.
				throw new Exception();
			}
		}

		public async Task<IncomingMessageBase> ReceiveMessageAsync(CancellationToken cancellationToken)
		{
			CheckAndThrowIfNotInState(SharpHomeAssistantConnectionState.Connected, nameof(SendMessageAsync));
			CheckAndThrowIfWebsocketNotOpen(_socket, nameof(SendMessageAsync));

			CancellationTokenSource joinedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disconnectingCancellationSource.Token);

			try
			{
				return await DoReceiveAsync(joinedSource.Token);
			}
			catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				//Simplify the exception to just the calling codes if they cancelled. 
				cancellationToken.ThrowIfCancellationRequested();

				//Just in case and to satisfy the compilier.
				throw new Exception();
			}
		}

		private async Task DoConnectAsync(ClientWebSocket socket, CancellationToken cancellationToken)
		{

			CheckAndThrowIfWebsocketNotOpen(socket, nameof(ConnectUsingSocketAsync));

			try
			{
				State = SharpHomeAssistantConnectionState.Connecting;

				_socket = socket;

				//
				// Create the send/receive channels, every time to clear out stale data.
				//
				_writeChannel = Channel.CreateBounded<MemoryStream>(1);
				_readChannel = Channel.CreateBounded<MemoryStream>(1);

				_readChannelCancellationSource = new CancellationTokenSource();
				_writeChannelCancellationSource = new CancellationTokenSource();
				_disconnectingCancellationSource = new CancellationTokenSource();

				_sendTask = SendLoopAsync();
				_receiveTask = ReceiveLoopAsync();

				try
				{
					IncomingMessageBase message = await DoReceiveAsync(cancellationToken);

					ThrowIfWrongMessageType(AuthRequiredMessage.MessageType, message.TypeId);

					AuthMessage authMessage = new AuthMessage() { AccessToken = AccessToken };
					await DoSendAsync(authMessage, cancellationToken);

					message = await DoReceiveAsync(cancellationToken);

					ThrowIfWrongMessageType(AuthRequiredMessage.MessageType, message.TypeId);

					AuthResultMessage resultMessage = (AuthResultMessage)message;

					if (!resultMessage.Success)
					{
						throw new ConnectFailedException(resultMessage.Message);
					}
				}
				catch (Exception ex) when (
					ex is ConnectFailedException
					|| ex is SharpHomeAssistantProtocolException
					)
				{
					State = SharpHomeAssistantConnectionState.Closing;

					_writeChannelCancellationSource.Cancel();
					_readChannelCancellationSource.Cancel();
					await _sendTask;
					await _receiveTask;

					await _socket.CloseAsync(WebSocketCloseStatus.InternalServerError, ex.Message, cancellationToken);
					if (_socket.State != WebSocketState.Closed)
					{
						throw new Exception(String.Format("Expected WebSocket to be closed after returning from CloseAsync. The WebSocket had as state of {0}", _socket.State));
					}

					State = SharpHomeAssistantConnectionState.NotConnected;
					throw;
				}

				State = SharpHomeAssistantConnectionState.Connected;
			}
			catch (Exception ex) when (
					!(ex is ConnectFailedException)
					&& !(ex is SharpHomeAssistantProtocolException))
			{

				if (_socket != null)
				{
					_socket.Abort();
				}

				State = SharpHomeAssistantConnectionState.Aborted;
				_forceShutdown.Cancel();

				throw;
			}
		}
		private void ThrowIfWrongMessageType(string wantedType, string gotType)
		{
			if (wantedType != gotType)
			{
				throw new SharpHomeAssistantProtocolException(String.Format("Expected message with type {0} but instead got {1}.", wantedType, gotType));
			}

		}

		private void CheckAndThrowIfWebsocketNotOpen(ClientWebSocket socket, string methodName)
		{
			if (socket.State != WebSocketState.Open)
			{
				throw new InvalidOperationException(
						String.Format("The supplied ClientWebSocket must have a state of open. The supplied ClientWebSocket had a state of {0} in method {1}.", socket.State, methodName)
						);
			}
		}

		private void CheckAndThrowIfNotInState(SharpHomeAssistantConnectionState expectedState, string methodName)
		{
			if (State != expectedState)
			{
				throw new InvalidOperationException(String.Format("{0} can only be called in the {1} state. The class is current in the {2} state.", methodName, expectedState, State));
			}
		}

		private async Task<IncomingMessageBase> DoReceiveAsync(CancellationToken cancellationToken)
		{
			CheckAndThrowIfWebsocketNotOpen(_socket, nameof(DoReceiveAsync));

			CancellationTokenSource joinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
				cancellationToken,
				_forceShutdown.Token,
				_readChannelCancellationSource.Token);

			joinedTokenSource.Token.ThrowIfCancellationRequested();

			using (MemoryStream stream = await _readChannel.Reader.ReadAsync(joinedTokenSource.Token))
			{
				try
				{
					IncomingMessageBase message = await JsonSerializer.DeserializeAsync<IncomingMessageBase>(stream, _jsonSerializerOptions, joinedTokenSource.Token);
					return message;
				}
				catch (Exception ex) when (ex is JsonException || ex is NotSupportedException)
				{
					throw new SharpHomeAssistantProtocolException("Could not deserialize received message.", ex);
				}

			}
		}

		private async Task DoSendAsync<T>(T message, CancellationToken cancellationToken) where T : OutgoingMessageBase
		{
			CheckAndThrowIfWebsocketNotOpen(_socket, nameof(DoReceiveAsync));
			CancellationTokenSource joinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
				cancellationToken,
				_forceShutdown.Token,
				_writeChannelCancellationSource.Token);

			joinedTokenSource.Token.ThrowIfCancellationRequested();


			MemoryStream stream = new MemoryStream();

			try
			{
				await JsonSerializer.SerializeAsync(stream, message, typeof(T), _jsonSerializerOptions, joinedTokenSource.Token);
			}
			catch (Exception ex) when (ex is JsonException || ex is NotSupportedException)
			{
				throw new SharpHomeAssistantProtocolException("Could not handle serialize message to send.", ex);
			}

			await _writeChannel.Writer.WriteAsync(stream, joinedTokenSource.Token);

		}

		private async Task<KeyValuePair<WebSocketCloseStatus, string>> ReceiveLoopAsync()
		{
			WebSocketReceiveResult result;
			CancellationTokenSource joinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_forceShutdown.Token, _readChannelCancellationSource.Token);
			try
			{
				do
				{
					MemoryStream currentStream = new MemoryStream();

					byte[] buffer = new byte[128];
					do
					{

						result = await _socket.ReceiveAsync(buffer, _forceShutdown.Token);
						await currentStream.WriteAsync(buffer, 0, result.Count);

						if (result.MessageType == WebSocketMessageType.Binary)
						{
							return new KeyValuePair<WebSocketCloseStatus, string>(WebSocketCloseStatus.InvalidMessageType, "Binary messages are not supported");
						}

						if (result.MessageType == WebSocketMessageType.Close)
						{
							return new KeyValuePair<WebSocketCloseStatus, string>(WebSocketCloseStatus.NormalClosure, "Close message received, closing connection");

						}

						if (MaxMessageSize > 0 && currentStream.Length > MaxMessageSize)
						{
							return new KeyValuePair<WebSocketCloseStatus, string>(WebSocketCloseStatus.MessageTooBig, "Message sent exceeded max message size");
						}

					} while (!result.EndOfMessage);

					currentStream.Seek(0, System.IO.SeekOrigin.Begin);
					await _readChannel.Writer.WriteAsync(currentStream, joinedTokenSource.Token);

				} while (_socket.State == WebSocketState.Open || _socket.State == WebSocketState.CloseSent);

				throw new Exception("WebSocket is in an invalid transition state. The close message was not received by the receive method. This likely means something went wrong with the WebSocket.");
			}
			catch (OperationCanceledException) when (_readChannelCancellationSource.IsCancellationRequested)
			{
				return new KeyValuePair<WebSocketCloseStatus, string>(WebSocketCloseStatus.NormalClosure, "Connection is being closed.");
			}
			finally
			{
				_readChannelCancellationSource.Cancel();
			}

		}
		private async Task SendLoopAsync()
		{

			CancellationTokenSource joinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_forceShutdown.Token, _writeChannelCancellationSource.Token);
			try
			{
				do
				{
					// Only use the joined source when waiting on the _writeChannel. 
					// For all other stuff just let it finish and then exit.
					MemoryStream stream = await _writeChannel.Reader.ReadAsync(joinedTokenSource.Token);
					byte[] buffer = new byte[128];
					do
					{
						int count = await stream.ReadAsync(buffer, 0, 128, _forceShutdown.Token);
						if (count < 128)
						{
							await _socket.SendAsync(new ArraySegment<byte>(buffer, 0, count), WebSocketMessageType.Text, true, _forceShutdown.Token);
							break;
						}
						else
						{
							await _socket.SendAsync(new ArraySegment<byte>(buffer, 0, 128), WebSocketMessageType.Text, false, _forceShutdown.Token);
						}

					} while (true);

					await stream.DisposeAsync();

				} while (_socket.State == WebSocketState.Open);
			}
			catch (OperationCanceledException) when (_writeChannelCancellationSource.IsCancellationRequested)
			{
				//  Do nothing for this case
			}
			finally
			{
				_writeChannelCancellationSource.Cancel();
			}
		}
	}
}