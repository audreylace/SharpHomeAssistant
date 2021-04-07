using System;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AudreysCloud.Community.SharpHomeAssistant.Exceptions;
using AudreysCloud.Community.SharpHomeAssistant.Messages;

namespace AudreysCloud.Community.SharpHomeAssistant
{
	public sealed class SharpHomeAssistantConnection
	{
		public const long SafeCommandCounterStartPos = 100;

		public SharpHomeAssistantConnectionState State { get; private set; }

		public string AccessToken { get; set; }

		public int MaxMessageSize { get; set; }

		#region Private Properties
		private ClientWebSocket WebSocket { get; set; }

		private SemaphoreSlim SendSemaphore { get; set; }

		private SemaphoreSlim ReceiveSemaphore { get; set; }

		private SemaphoreSlim CounterSemphaphore { get; set; }

		private JsonSerializerOptions jsonSerializerOptions { get; set; }

		private CancellationTokenSource ShutdownTokenSource { get; set; }

		private long CommandIdCounter { get; set; }

		private bool ContinueReceive { get; set; }

		private MemoryStream PreviousReceiveStream { get; set; }
		#endregion Private Properties

		public SharpHomeAssistantConnection()
		{
			State = SharpHomeAssistantConnectionState.NotConnected;
			MaxMessageSize = 1024 * 1024; // 1 Megabyte
			jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
			jsonSerializerOptions.Converters.Add(new IncomingMessageConverter());
			ShutdownTokenSource = new CancellationTokenSource();
			ShutdownTokenSource.Cancel();
			CommandIdCounter = 1;
			SendSemaphore = new SemaphoreSlim(1, 1);
			ReceiveSemaphore = new SemaphoreSlim(1, 1);
			CounterSemphaphore = new SemaphoreSlim(1, 1);
		}

		#region Public Methods

		public async Task<ConnectResult> ConnectAsync(Uri serverUri, CancellationToken cancellationToken)
		{
			CheckAndThrowIfNotInState(SharpHomeAssistantConnectionState.NotConnected, nameof(ConnectAsync));

			WebSocket = new ClientWebSocket();
			try
			{
				State = SharpHomeAssistantConnectionState.Connecting;
				await WebSocket.ConnectAsync(serverUri, cancellationToken);
				ConnectResult result = await NegotiateConnection(cancellationToken);

				if (result.Success)
				{
					await CounterSemphaphore.WaitAsync(cancellationToken);
					CommandIdCounter = Math.Max(CommandIdCounter, SafeCommandCounterStartPos);
					CounterSemphaphore.Release();

					State = SharpHomeAssistantConnectionState.Connected;
				}

				ShutdownTokenSource = new CancellationTokenSource();
				return result;
			}
			catch (WebSocketException ex)
			{
				return new ConnectResult() { Success = false, Exception = ex };
			}
			catch (Exception)
			{
				State = SharpHomeAssistantConnectionState.Aborted;
				throw;
			}
		}

		public async Task<ConnectResult> ConnectUsingSocketAsync(ClientWebSocket webSocket, CancellationToken cancellationToken)
		{

			CheckAndThrowIfNotInState(SharpHomeAssistantConnectionState.NotConnected, nameof(ConnectUsingSocketAsync));
			CheckAndThrowIfWebsocketNotOpen(webSocket, nameof(ConnectUsingSocketAsync));


			try
			{
				State = SharpHomeAssistantConnectionState.Connecting;
				WebSocket = webSocket;
				ConnectResult result = await NegotiateConnection(cancellationToken);

				if (result.Success)
				{
					State = SharpHomeAssistantConnectionState.Connected;
				}

				ShutdownTokenSource = new CancellationTokenSource();

				return result;
			}
			catch (Exception)
			{
				State = SharpHomeAssistantConnectionState.Aborted;
				throw;
			}
		}


		/// <Summary>
		/// Closes the underlying websocket connection and transitiones the class into the not connected state.
		/// </Summary>
		public async Task CloseAsync(CancellationToken token)
		{
			if (State != SharpHomeAssistantConnectionState.Closing && State != SharpHomeAssistantConnectionState.Connected)
			{
				throw new InvalidOperationException(String.Format("{0} can only be called in a Closing state or Connected state. The class is current in the {1} state.", nameof(CloseAsync), State));
			}

			State = SharpHomeAssistantConnectionState.Closing;

			ShutdownTokenSource.Cancel();

			if (WebSocket.State == WebSocketState.Open || WebSocket.State == WebSocketState.CloseReceived || WebSocket.State == WebSocketState.CloseSent)
			{
				await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, default(string), token);
			}

			WebSocket.Dispose();
			WebSocket = null;
			State = SharpHomeAssistantConnectionState.NotConnected;
		}

		public void Abort()
		{
			State = SharpHomeAssistantConnectionState.Aborted;
			ShutdownTokenSource.Cancel();
			WebSocket.Abort();
			WebSocket.Dispose();
			WebSocket = null;
		}

		public async Task SendMessageAsync<MessageType>(MessageType messageBase, CancellationToken cancellationToken) where MessageType : OutgoingMessageBase
		{

			CheckAndThrowIfNotInState(SharpHomeAssistantConnectionState.Connected, nameof(SendMessageAsync));
			CheckAndThrowIfWebsocketNotOpen(WebSocket, nameof(SendMessageAsync));

			CancellationToken shutdownToken = ShutdownTokenSource.Token;

			CancellationTokenSource joinedSource = CancellationTokenSource.CreateLinkedTokenSource(shutdownToken, cancellationToken);

			await SendWebSocketMessageAsync<MessageType>(messageBase, joinedSource.Token);
		}

		public async Task<ReceiveMessageAsyncResult> ReceiveMessageAsync(CancellationToken cancellationToken)
		{
			CheckAndThrowIfNotInState(SharpHomeAssistantConnectionState.Connected, nameof(ReceiveMessageAsync));

			// 
			// Assign in local context in case a new shutsown token source is 
			// created later because of another async operation.
			//
			CancellationToken shutdownToken = ShutdownTokenSource.Token;

			CancellationTokenSource joinedSource = CancellationTokenSource.CreateLinkedTokenSource(shutdownToken, cancellationToken);
			using (ReceiveOperationResult result = await ReceiveWebsocketMessageAsync(joinedSource.Token))
			{
				if (!result.Success)
				{
					if (result.OperationCancelled)
					{
						cancellationToken.ThrowIfCancellationRequested();
						shutdownToken.ThrowIfCancellationRequested();
						//
						// Throw this if for some reason the other tokens were not cancelled.
						//
						throw new Exception("Operation was cancelled but neither cancellationToken nor shutdownToken is cancelled. This is unexpected and likely indicates a programming error.");
					}

					if (result.GotCloseMessage)
					{
						State = SharpHomeAssistantConnectionState.Closing;
						return new ReceiveMessageAsyncResult()
						{
							Status = ReceiveMessageAsyncStatus.CloseMessageReceived
						};
					}

					if (result.MessageOverflow)
					{
						return new ReceiveMessageAsyncResult()
						{
							Status = ReceiveMessageAsyncStatus.MessageOverflow
						};
					}

				}

				if (result.GotBinaryMessage)
				{
					return new ReceiveMessageAsyncResult()
					{
						Status = ReceiveMessageAsyncStatus.GotBinaryMessage,
						BinaryMessage = result.Stream.ToArray()
					};
				}

				//
				// Only used joined source for receive operations. 
				// After the message is written to the memory stream
				// we should just deserialize it and return it.
				//
				IncomingMessageBase message = await JsonSerializer.DeserializeAsync<IncomingMessageBase>(result.Stream, jsonSerializerOptions, cancellationToken);
				return new ReceiveMessageAsyncResult() { Message = message, Status = ReceiveMessageAsyncStatus.MessageReceived };
			}
		}

		public long GetNextCommandId()
		{
			try
			{
				CounterSemphaphore.Wait();
				return CommandIdCounter++;
			}
			finally
			{
				CounterSemphaphore.Release();
			}

		}

		#endregion Public Methods

		#region Private Methods

		private async Task<ConnectResult> NegotiateConnection(CancellationToken cancellationToken)
		{
			CheckAndThrowIfWebsocketNotOpen(WebSocket, nameof(NegotiateConnection));
			try
			{
				using (ReceiveOperationResult result = await ReceiveWebsocketMessageAsync(cancellationToken))
				{
					result.ThrowIfFailed();
					result.ThrowIfBinary();

					IncomingMessageBase message = await JsonSerializer.DeserializeAsync<IncomingMessageBase>(result.Stream, jsonSerializerOptions, cancellationToken);
					if (message.TypeId != AuthRequiredMessage.MessageType)
					{
						throw new SharpHomeAssistantProtocolException(String.Format("Expected message with type {0} but instead got {1}", AuthRequiredMessage.MessageType, message.TypeId));
					}
				}

				AuthMessage authMessage = new AuthMessage() { AccessToken = AccessToken };

				await SendWebSocketMessageAsync<AuthMessage>(authMessage, cancellationToken);

				using (ReceiveOperationResult result = await ReceiveWebsocketMessageAsync(cancellationToken))
				{
					result.ThrowIfFailed();
					result.ThrowIfBinary();

					IncomingMessageBase message = await JsonSerializer.DeserializeAsync<IncomingMessageBase>(result.Stream, jsonSerializerOptions, cancellationToken);
					if (message.TypeId != AuthResultMessage.MessageType)
					{
						throw new SharpHomeAssistantProtocolException(String.Format("Expected message with type {0} but instead got {1}", AuthResultMessage.MessageType, message.TypeId));
					}

					AuthResultMessage resultMessage = (AuthResultMessage)message;

					if (!resultMessage.Success)
					{
						await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, resultMessage.Message, cancellationToken);
						State = SharpHomeAssistantConnectionState.NotConnected;
						return new ConnectResult() { Success = false, Message = resultMessage.Message };
					}
				}

				return new ConnectResult() { Success = true };
			}
			catch (JsonException ex)
			{
				await WebSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Internal Server error", cancellationToken);
				WebSocket.Dispose();
				WebSocket = null;
				return new ConnectResult() { Success = false, Exception = ex };

			}
			catch (WebSocketException ex)
			{
				WebSocket.Abort();
				WebSocket.Dispose();
				WebSocket = null;

				return new ConnectResult() { Success = false, Exception = ex };
			}

		}

		private void CheckAndThrowIfNotInState(SharpHomeAssistantConnectionState expectedState, string methodName)
		{
			if (State != expectedState)
			{
				throw new InvalidOperationException(String.Format("{0} can only be called in the {1} state. The class is current in the {2} state.", methodName, expectedState, State));
			}
		}

		private void CheckAndThrowIfWebsocketNotOpen(ClientWebSocket socket, string methodName)
		{
			if (WebSocket.State != WebSocketState.Open)
			{
				throw new InvalidOperationException(
						String.Format("The supplied ClientWebSocket must have a state of open. The supplied ClientWebSocket had a state of {0} in method {1}.", socket.State, methodName)
						);
			}
		}

		private async Task SendWebSocketMessageAsync<T>(T message, CancellationToken cancellationToken)
		{
			CheckAndThrowIfWebsocketNotOpen(WebSocket, nameof(SendWebSocketMessageAsync));

			await SendSemaphore.WaitAsync(cancellationToken);
			try
			{
				byte[] messageBytes = JsonSerializer.SerializeToUtf8Bytes(message, typeof(T), jsonSerializerOptions);

				await WebSocket.SendAsync(messageBytes,
					  WebSocketMessageType.Text,
					  true,
					  cancellationToken);
			}
			finally
			{
				SendSemaphore.Release();
			}
		}

		private async Task<ReceiveOperationResult> ReceiveWebsocketMessageAsync(CancellationToken cancellationToken)
		{
			CheckAndThrowIfWebsocketNotOpen(WebSocket, nameof(ReceiveWebsocketMessageAsync));

			try
			{
				await ReceiveSemaphore.WaitAsync(cancellationToken);
			}
			catch (OperationCanceledException)
			{
				return new ReceiveOperationResult() { Success = false, OperationCancelled = true };
			}

			CheckAndThrowIfWebsocketNotOpen(WebSocket, nameof(ReceiveWebsocketMessageAsync));

			byte[] buffer = new byte[128];
			System.IO.MemoryStream stream;

			if (ContinueReceive && PreviousReceiveStream != null)
			{
				stream = PreviousReceiveStream;
			}
			else
			{
				stream = new MemoryStream();
			}

			PreviousReceiveStream = null;

			bool receiveIncomplete = false;
			try
			{
				WebSocketReceiveResult result;

				do
				{
					result = await WebSocket.ReceiveAsync(buffer, cancellationToken);

					if (receiveIncomplete && result.EndOfMessage)
					{
						receiveIncomplete = false;
					}

					if (result.MessageType == WebSocketMessageType.Close)
					{
						continue; //Drain the close message
					}

					receiveIncomplete = true;
					//
					// Don't pass in cancellation token here. Write the message to the stream to ensure
					// that a cancellation does not result in an inconsistent state.
					//
					await stream.WriteAsync(buffer, 0, result.Count);

					if (MaxMessageSize > 0 && stream.Length > MaxMessageSize)
					{
						return new ReceiveOperationResult { Success = false, MessageOverflow = true };
					}

				} while (!result.EndOfMessage);

				stream.Seek(0, System.IO.SeekOrigin.Begin);


				if (result.MessageType == WebSocketMessageType.Text)
				{
					return new ReceiveOperationResult()
					{
						Stream = stream,
						Success = true
					};
				}
				else if (result.MessageType == WebSocketMessageType.Binary)
				{

					return new ReceiveOperationResult()
					{
						Stream = stream,
						Success = true,
						GotBinaryMessage = true
					};

				}
				else if (result.MessageType == WebSocketMessageType.Close)
				{
					return new ReceiveOperationResult()
					{
						Success = false,
						GotCloseMessage = true
					};
				}
				else
				{
					throw new NotImplementedException();
				}

			}
			catch (OperationCanceledException)
			{
				if (receiveIncomplete)
				{
					ContinueReceive = true;
					PreviousReceiveStream = stream;
				}
				return new ReceiveOperationResult() { Success = false, OperationCancelled = true };
			}
			finally
			{
				ReceiveSemaphore.Release();
			}
		}

		#endregion Private Methods


	}
}