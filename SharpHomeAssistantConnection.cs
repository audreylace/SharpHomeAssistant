
using System;
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
	/// <summary>
	/// Class used to consume the Home Assistant WebSocket API.
	/// https://developers.home-assistant.io/docs/api/websocket/
	/// </summary>
	public sealed class SharpHomeAssistantConnection
	{

		#region Private Fields

		// The websocket that is used to connect to the Home Assistant instance.
		// This will be null until the connection has been established.
		private WebSocket _socket;

		// 
		// Channel used to pass messages read from the websocket connection to receivers with backpressure.
		// This will be null until the connection has been established.
		private Channel<MemoryStream> _receiveChannel;


		// Channel used to pass messages to write from consumers to the websocket connection with backpressure.
		// 	This will be null until the connection has been established 	
		private Channel<byte[]> _sendChannel;


		// Cancellation source used to stop all async operations in the class. All async operations
		// should subscribe to token source. This source is cancelled when abort is called. 
		// Code is not expected to gracefully recover from this as once this cancellation source
		// is cancelled the class is put into the abort state.
		private CancellationTokenSource _forceShutdown;

		// Json options used to control Json serialization/unserialization.
		private JsonSerializerOptions _jsonSerializerOptions { get; set; }

		// Running send task. Consumes messages off of the write channel and sends them to the remote HA instance over the websocket.
		// Messages are sent over this channel via the DoSendAsync method. 
		private Task _sendTask;


		// Running receive task. Places messages on the write channel sent from the remote HA instance over the websocket.
		// Messages are sent over this channel via the DoReceiveAync method. 
		private Task _receiveTask;


		#endregion Private Fields

		#region Public Members

		#region Public Properties

		/// <summary>
		/// State of the connection.
		/// </summary>
		public SharpHomeAssistantConnectionState State { get; private set; }

		/// <summary>
		/// The access token used to authenticate this connection with the remote Home Assistant instance.
		/// </summary>
		public string AccessToken { get; set; }

		/// <summary>
		/// The max size in bytes of a single web socket message. 
		/// </summary>
		/// <remarks>
		/// This is set to 1 megabyte by default. Setting it to 0 will disable size limiting and allow messages of any sizes. Setting it to 0 is not recommended in a production environment since it opens up the client to abuse via memory exhaustion.
		/// </remarks>
		public int MaxMessageSize { get; set; }

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Default constructor for the class.
		/// </summary>
		public SharpHomeAssistantConnection()
		{
			State = SharpHomeAssistantConnectionState.NotConnected;
			MaxMessageSize = 1024 * 1024; // 1 Megabyte

			_jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
			_jsonSerializerOptions.Converters.Add(new IncomingMessageConverter());

			_forceShutdown = new CancellationTokenSource();
		}

		/// <summary>
		/// This method should be used if you already have an open active websocket connection to a Home Assistant instance but have not yet completed the authentication phase.
		/// 
		/// https://developers.home-assistant.io/docs/api/websocket/#authentication-phase
		/// 
		/// </summary>
		/// <param name="socket">The socket to use. This socket should be in the Open state. Once passed in this socket should no longer be used by consuming code.</param>
		/// <param name="cancellationToken">Token used to cancel the async operation.</param>
		/// <exception cref="InvalidOperationException">
		/// Throws an invalid operation exception if the web socket is not in the open state.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// Throws an invalid operation exception if this class is not in the NotConnected state.
		/// </exception>
		/// <exception cref="ConnectFailedException">
		/// Thrown when the remote server rejects the authentication message.
		/// </exception>
		/// <exception cref="SharpHomeAssistantProtocolException">
		/// Thrown if some sort of internal error occurs during the authentication handshake.
		/// </exception>
		/// <exception cref="OperationCanceledException">
		/// If the source of the cancellation token is cancelled.
		/// </exception>
		/// <remarks>
		/// Cancelling this operation can put this class in the aborted state. Check before trying to use this class again if a connect operation is cancelled.
		/// </remarks>
		/// <see cref="SharpHomeAssistantConnectionState" />
		/// <returns>The task representing the async operation.</returns>
		public async Task ConnectUsingSocketAsync(WebSocket socket, CancellationToken cancellationToken)
		{
			CheckAndThrowIfNotInState(SharpHomeAssistantConnectionState.NotConnected, nameof(ConnectUsingSocketAsync));
			await PerformConnectionHandshakeAsync(socket, cancellationToken);
		}

		/// <summary>
		/// This method opens a websocket connection to the Home Assistant server at the provided uri and does the authentication handshake.
		/// </summary>
		/// <param name="serverUri">URI of the server to connect to.</param>
		/// <param name="cancellationToken"></param>
		/// <exception cref="InvalidOperationException">
		/// Throws an invalid operation exception if this class is not in the NotConnected state.
		/// </exception>
		/// <exception cref="ConnectFailedException">
		/// Thrown when the remote server rejects the authentication message or a websocket failure is encountered during the connect attempt.
		/// </exception>
		/// <exception cref="SharpHomeAssistantProtocolException">
		/// Thrown if some sort of internal error occurs during the authentication handshake.
		/// </exception>
		/// <exception cref="OperationCanceledException">
		/// If the source of the cancellation token is cancelled.
		/// </exception>
		/// <see cref="SharpHomeAssistantConnectionState" />
		/// <returns>The task representing the async operation.</returns>
		public async Task ConnectAsync(Uri serverUri, CancellationToken cancellationToken)
		{
			CheckAndThrowIfNotInState(SharpHomeAssistantConnectionState.NotConnected, nameof(ConnectAsync));

			ClientWebSocket newSocket = new ClientWebSocket();

			State = SharpHomeAssistantConnectionState.Connecting;
			try
			{
				await newSocket.ConnectAsync(serverUri, cancellationToken);
			}
			catch (WebSocketException ex)
			{
				State = SharpHomeAssistantConnectionState.NotConnected;
				throw new ConnectFailedException(ex);
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
			await PerformConnectionHandshakeAsync(newSocket, cancellationToken);

		}

		/// <summary>
		/// Closes down the connection gracefully.
		/// </summary>
		/// <param name="cancellationToken">Used to cancel the graceful operation. This is equivalent to calling Abort.</param>
		/// <exception cref="InvalidOperationException">
		/// Throws an invalid operation exception if this class is not in the Connected state.
		/// </exception>
		/// <remarks>
		/// Exceptions that occured in any internal task will bubble up through this method since all background operations are joined here.
		/// </remarks>
		/// <returns>Task representing the asyncronous operation.</returns>
		public async Task CloseAsync(CancellationToken cancellationToken)
		{
			if (State != SharpHomeAssistantConnectionState.Closing && State != SharpHomeAssistantConnectionState.Connected)
			{
				throw new InvalidOperationException(String.Format("{0} expects the connection to either be Open or Closing. The state of the connection was {1}.", nameof(CloseAsync), State));
			}

			try
			{
				await CloseConnectionAsync(cancellationToken);
			}
			catch
			{
				Abort();
				throw;
			}
		}



		/// <summary>
		/// Aborts the websocket connection as well as all internal operations. This class can not be re-used after calling this.
		/// </summary>
		public void Abort()
		{

			_forceShutdown?.Cancel();

			_socket?.Abort();
			_socket?.Dispose();
			_socket = null;

			State = SharpHomeAssistantConnectionState.Aborted;
		}

		/// <summary>
		/// Sends a message to the Home Assistant server.
		/// </summary>
		/// <typeparam name="TMessageType">Type of the message being passed in. This must derive from OutgoingMessageBase.</typeparam>
		/// <param name="message">Message to send.</param>
		/// <param name="cancellationToken">Token used to abort this send.</param>
		/// <exception cref="InvalidOperationException">
		/// Throws an invalid operation exception if this class is not in the Connected state.
		/// </exception>
		/// <exception cref="OperationCanceledException">
		/// If this send operation was cancelled by the  passed in cancellation token then the exception's token will equal that. If the token does not match the passed in one, that means that any of the internal cancellation souces have been cancelled. This can happen for many reasons from consuming API calling CloseAsync to the remote connection sending a request close message.
		/// </exception>
		/// <exception cref="SharpHomeAssistantProtocolException">
		/// Thrown if for some reason the message can not be serialized into JSON
		/// </exception>
		/// <returns>The task representing the async send operation. True if the message was queued to be sent, false otherwise.</returns>
		public async Task<bool> SendMessageAsync<TMessageType>(TMessageType message, CancellationToken cancellationToken) where TMessageType : OutgoingMessageBase
		{
			CheckAndThrowIfNotInState(SharpHomeAssistantConnectionState.Connected, nameof(SendMessageAsync));
			CheckAndThrowIfWebsocketNotOpen(_socket, nameof(SendMessageAsync));

			try
			{
				return await WaitAndThenQueueMessageToSendAsync(message, cancellationToken);
			}
			catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				//Simplify the exception to just the calling codes if they cancelled. 
				cancellationToken.ThrowIfCancellationRequested();

				//Just in case and to satisfy the compilier.
				throw new Exception();
			}
		}

		/// <summary>
		/// Receives a message from the remote Home Assistant server.
		/// </summary>
		/// <param name="cancellationToken">Token used to abort waiting for a message.</param>
		/// <exception cref="OperationCanceledException">
		/// If this receive operation was cancelled by the passed in cancellation token then the exception's token will equal that. If the token does not match the passed in one, that means that any of the internal cancellation souces have been cancelled. This can happen for many reasons from consuming API calling CloseAsync to the remote connection sending a request close message.
		/// </exception>
		/// <returns>A task which will be populated with an incoming message once one is received from the websocket. If for some reason the receive failed, null will be returned.</returns>
		public async Task<IncomingMessageBase> ReceiveMessageAsync(CancellationToken cancellationToken)
		{
			CheckAndThrowIfNotInState(SharpHomeAssistantConnectionState.Connected, nameof(SendMessageAsync));
			CheckAndThrowIfWebsocketNotOpen(_socket, nameof(SendMessageAsync));

			try
			{
				return await WaitAndThenReceiveMessageAsync(cancellationToken);
			}
			catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				//Simplify the exception to just the calling codes if they cancelled. 
				cancellationToken.ThrowIfCancellationRequested();

				//Just in case and to satisfy the compilier.
				throw new Exception();
			}
		}

		#endregion Public Methods

		#endregion Public Members

		#region Private Methods

		private async Task CloseConnectionAsync(CancellationToken cancellationToken)
		{
			if (_socket == null || (_socket.State != WebSocketState.Open && _socket.State != WebSocketState.CloseReceived && _socket.State != WebSocketState.CloseSent))
			{
				if (_socket != null)
				{
					throw new InvalidOperationException(String.Format("The websocket is in an invalid state. Expected a state of open, close received or close sent. Instead the socket was in {0}", _socket.State));
				}

				throw new InvalidOperationException("_socket is null. This is not permitted.");
			}


			CancellationTokenSource cancelAndAbort = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _forceShutdown.Token);

			State = SharpHomeAssistantConnectionState.Closing;

			//
			// Drain all pending messages to send.
			//
			_sendChannel.Writer.TryComplete();
			await _sendTask;

			//
			// No more messages to send so now we close the output.
			//
			await _socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", cancelAndAbort.Token);


			// 
			// Complete the receive channel to avoid the receive operations hitting a deadlock 
			// waiting for something to read the messages.
			//
			_receiveChannel.Writer.TryComplete();

			//
			// Wait for and join the receive task.
			//
			await _receiveTask;


			//
			// Transition to the closed state, if we are not there already.
			//
			if (_socket.State == WebSocketState.CloseReceived || _socket.State == WebSocketState.CloseSent)
			{
				await _socket.CloseAsync(_socket.CloseStatus ?? WebSocketCloseStatus.NormalClosure, _socket.CloseStatusDescription ?? "Closing Connection", cancelAndAbort.Token);
			}
			else if (_socket.State != WebSocketState.Closed)
			{
				throw new Exception(String.Format("The websocket should have closed, but instead it has a state of {0}", _socket.State));
			}

			State = SharpHomeAssistantConnectionState.NotConnected;

			_socket.Dispose();
			_socket = null;
		}

		private async Task PerformConnectionHandshakeAsync(WebSocket socket, CancellationToken cancellationToken)
		{

			CheckAndThrowIfWebsocketNotOpen(socket, nameof(ConnectUsingSocketAsync));

			try
			{
				State = SharpHomeAssistantConnectionState.Connecting;

				_socket = socket;

				//
				// Create the send/receive channels every time to clear out stale data.
				//
				_sendChannel = Channel.CreateBounded<byte[]>(1);
				_receiveChannel = Channel.CreateBounded<MemoryStream>(1);

				_sendTask = SendMessagesPlacedOnQueueAsync();
				_receiveTask = ReceiveMessagesAndPlaceOnQueueAsync();

				try
				{
					IncomingMessageBase message = await WaitAndThenReceiveMessageAsync(cancellationToken);

					if (message == null)
					{
						throw new Exception("Receive loop died, aborting connection.", _receiveChannel.Reader.Completion.Exception);
					}

					ThrowIfWrongMessageTypeOrConvert<AuthRequiredMessage>(message);

					AuthMessage authMessage = new AuthMessage() { AccessToken = AccessToken };
					if (!await WaitAndThenQueueMessageToSendAsync(authMessage, cancellationToken))
					{
						throw new Exception("Send loop died, aborting connection.", _sendChannel.Reader.Completion.Exception);
					}

					message = await WaitAndThenReceiveMessageAsync(cancellationToken);
					if (message == null)
					{
						throw new Exception("Receive loop died, aborting connection.", _receiveChannel.Reader.Completion.Exception);
					}

					AuthResultMessage resultMessage = ThrowIfWrongMessageTypeOrConvert<AuthResultMessage>(message);

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
					await CloseConnectionAsync(cancellationToken);
					throw;
				}

				State = SharpHomeAssistantConnectionState.Connected;
			}
			catch (Exception ex) when (
					!(ex is ConnectFailedException)
					&& !(ex is SharpHomeAssistantProtocolException))
			{

				Abort();
				throw;
			}
		}

		private WantedMessageType ThrowIfWrongMessageTypeOrConvert<WantedMessageType>(IncomingMessageBase incomingMessage) where WantedMessageType : IncomingMessageBase
		{
			WantedMessageType outMessage;
			if (!IncomingMessageBase.TryConvert<WantedMessageType>(incomingMessage, out outMessage))
			{
				throw new SharpHomeAssistantProtocolException(String.Format("Expected message with type {0} but instead got {1}.",
					IncomingMessageBase.GetMessageTypeString(typeof(WantedMessageType)), incomingMessage.MessageType));
			}

			return outMessage;
		}

		private void CheckAndThrowIfWebsocketNotOpen(WebSocket socket, string methodName)
		{
			if (socket.State != WebSocketState.Open)
			{
				throw new InvalidOperationException(
						String.Format("The supplied WebSocket must have a state of open. The supplied WebSocket had a state of {0} in method {1}.", socket.State, methodName)
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

		private async Task<IncomingMessageBase> WaitAndThenReceiveMessageAsync(CancellationToken cancellationToken)
		{
			CheckAndThrowIfWebsocketNotOpen(_socket, nameof(WaitAndThenReceiveMessageAsync));

			CancellationTokenSource joinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
				cancellationToken,
				_forceShutdown.Token);

			while (await _receiveChannel.Reader.WaitToReadAsync(joinedTokenSource.Token))
			{
				MemoryStream stream;
				if (_receiveChannel.Reader.TryRead(out stream))
				{
					try
					{
						//
						// Don't pass in a token to the json serializer. At this point we have the message from the channel so if we abort we could loose it!.
						//
						IncomingMessageBase message = await JsonSerializer.DeserializeAsync<IncomingMessageBase>(stream, _jsonSerializerOptions);
						return message;
					}
					catch (Exception ex) when (ex is JsonException || ex is NotSupportedException)
					{
						throw new SharpHomeAssistantProtocolException("Could not deserialize received message.", ex);
					}

				}
			}

			return null;
		}

		private async Task<bool> WaitAndThenQueueMessageToSendAsync<T>(T message, CancellationToken cancellationToken) where T : OutgoingMessageBase
		{
			CheckAndThrowIfWebsocketNotOpen(_socket, nameof(WaitAndThenReceiveMessageAsync));
			CancellationTokenSource joinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
				cancellationToken,
				_forceShutdown.Token);

			joinedTokenSource.Token.ThrowIfCancellationRequested();


			byte[] messageToSend;

			try
			{
				messageToSend = JsonSerializer.SerializeToUtf8Bytes(message, typeof(T), _jsonSerializerOptions);
				if (messageToSend.Length < 2)
				{
					throw new SharpHomeAssistantProtocolException("Serialized message is invalid.");
				}
			}
			catch (Exception ex) when (ex is JsonException || ex is NotSupportedException)
			{
				throw new SharpHomeAssistantProtocolException("Could not handle serialize message to send.", ex);
			}

			while (await _sendChannel.Writer.WaitToWriteAsync(joinedTokenSource.Token))
			{
				if (_sendChannel.Writer.TryWrite(messageToSend))
				{
					return true;
				}
			}

			return false;
		}

		private async Task ReceiveMessagesAndPlaceOnQueueAsync()
		{
			WebSocketReceiveResult result;
			try
			{
				while (_socket.State == WebSocketState.Open || _socket.State == WebSocketState.CloseSent)
				{
					MemoryStream currentStream = new MemoryStream();

					byte[] buffer = new byte[128];
					do
					{
						result = await _socket.ReceiveAsync(buffer, _forceShutdown.Token);

						if (result.MessageType == WebSocketMessageType.Binary)
						{
							continue;
						}

						if (result.MessageType == WebSocketMessageType.Close)
						{
							break;
						}

						await currentStream.WriteAsync(buffer, 0, result.Count);

						if (MaxMessageSize > 0 && currentStream.Length > MaxMessageSize)
						{
							throw new Exception("Received message is too big.");
						}

					} while (!result.EndOfMessage);

					if (result.MessageType == WebSocketMessageType.Text)
					{
						currentStream.Seek(0, System.IO.SeekOrigin.Begin);

						while (await _receiveChannel.Writer.WaitToWriteAsync(_forceShutdown.Token))
						{
							if (_receiveChannel.Writer.TryWrite(currentStream))
							{
								break;
							}
						}
					}
					else if (result.MessageType == WebSocketMessageType.Close)
					{
						break;
					}
				}

				_receiveChannel.Writer.TryComplete();
			}
			catch (Exception ex)
			{
				_receiveChannel.Writer.TryComplete(ex);
				throw;
			}

		}

		private async Task SendMessagesPlacedOnQueueAsync()
		{
			try
			{
				while ((_socket.State == WebSocketState.Open || _socket.State == WebSocketState.CloseReceived) && await _sendChannel.Reader.WaitToReadAsync(_forceShutdown.Token))
				{
					byte[] message;
					if (!_sendChannel.Reader.TryRead(out message))
					{
						continue;
					}


					await _socket.SendAsync(message, WebSocketMessageType.Text, true, _forceShutdown.Token);

				}

				_sendChannel.Writer.TryComplete();
			}
			catch (Exception ex)
			{
				_sendChannel.Writer.TryComplete(ex);
				throw;
			}
		}

		#endregion Private Methods

	}
}