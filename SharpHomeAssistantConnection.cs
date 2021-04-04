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

	internal class ReceiveOperationResult : IDisposable
	{
		public bool Success { get; set; }
		public MemoryStream Stream { get; set; }
		public bool OperationCancelled { get; set; }

		public bool MessageReceiveStarted { get; set; }

		public bool GotCloseMessage { get; set; }

		public void Dispose()
		{
			Stream.Dispose();
		}

		public void ThrowIfFailed()
		{
			if (!Success)
			{
				if (GotCloseMessage)
				{
					throw new Exception("Receive operation failed because the close message was received on the web socket.");
				}

				if (OperationCancelled)
				{
					throw new Exception("Receive operation failed because the operation was cancelled.");
				}

				throw new Exception("Receive operation failed.");
			}
		}
	}

	public class ReceiveMessageAsyncResult
	{
		public bool CloseMessageReceived { get; set; }
		public IncomingMessageBase Message { get; set; }
	}
	public sealed class SharpHomeAssistantConnection
	{
		private ClientWebSocket WebSocket { get; set; }
		private SemaphoreSlim SendSemaphore { get; set; }
		private SemaphoreSlim ReceiveSemaphore { get; set; }
		private JsonSerializerOptions jsonSerializerOptions { get; set; }

		public SharpHomeAssistantConnectionState State { get; private set; }
		public string AccessToken { get; set; }
		public int MaxMessageSize { get; set; }

		private CancellationTokenSource ShutdownTokenSource { get; set; }

		private int CommandIdCounter { get; set; }

		private bool ContinueReceive { get; set; }

		private MemoryStream PreviousReceiveStream { get; set; }


		public SharpHomeAssistantConnection()
		{
			State = SharpHomeAssistantConnectionState.NotConnected;
			MaxMessageSize = 1024 * 1024; // 1 Megabyte
			jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
			jsonSerializerOptions.Converters.Add(new IncomingMessageConverter());
			ShutdownTokenSource = new CancellationTokenSource();
			ShutdownTokenSource.Cancel();
		}

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
		public async Task<ConnectResult> ConnectUsingSocketAsync(ClientWebSocket webSocket, CancellationToken cancellationToken)
		{
			if (webSocket.State != WebSocketState.Open)
			{
				throw new ArgumentException(
					String.Format("The supplied ClientWebSocket must have a state of open. The supplied ClientWebSocket had a state of {0}.", webSocket.State),
					"webSocket"
					);
			}

			CheckAndThrowIfNotInState(SharpHomeAssistantConnectionState.NotConnected, nameof(ConnectUsingSocketAsync));

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

		private async Task<ConnectResult> NegotiateConnection(CancellationToken cancellationToken)
		{
			try
			{
				using (ReceiveOperationResult result = await ReceiveWebsocketMessageAsync(cancellationToken))
				{
					result.ThrowIfFailed();

					IncomingMessageBase message = await JsonSerializer.DeserializeAsync<IncomingMessageBase>(result.Stream, jsonSerializerOptions, cancellationToken);
					if (message.TypeId != AuthRequiredMessage.MessageType)
					{
						throw new SharpHomeAssistantProtocolException(String.Format("Expected message with type {0} but instead got {1}", AuthRequiredMessage.MessageType, message.TypeId));
					}
				}

				AuthMessage authMessage = new AuthMessage() { AccessToken = AccessToken };

				await WebSocket.SendAsync(JsonSerializer.SerializeToUtf8Bytes(authMessage, jsonSerializerOptions),
					  WebSocketMessageType.Text,
					  true,
					  cancellationToken);

				using (ReceiveOperationResult result = await ReceiveWebsocketMessageAsync(cancellationToken))
				{
					result.ThrowIfFailed();

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
				await WebSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, ex.Message, cancellationToken);
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

		/// <summary>Closes the underlying websocket connection and transitiones the class into the not connected state.</summary>
		public async Task CloseAsync(CancellationToken token)
		{
			CheckAndThrowIfNotInState(SharpHomeAssistantConnectionState.Connected, nameof(CloseAsync));

			State = SharpHomeAssistantConnectionState.Closing;

			ShutdownTokenSource.Cancel();

			await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, default(string), token);
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

		public async Task<ReceiveMessageAsyncResult> ReceiveMessageAsync(CancellationToken cancellationToken)
		{
			CheckAndThrowIfNotInState(SharpHomeAssistantConnectionState.Connected, nameof(ReceiveMessageAsync));
			CancellationTokenSource joinedSource = CancellationTokenSource.CreateLinkedTokenSource(ShutdownTokenSource.Token);
			using (ReceiveOperationResult result = await ReceiveWebsocketMessageAsync(joinedSource.Token))
			{
				if (!result.Success)
				{
					if (result.OperationCancelled)
					{
						cancellationToken.ThrowIfCancellationRequested();
						ShutdownTokenSource.Token.ThrowIfCancellationRequested();
						//
						// Throw this if for some reason the other tokens were not cancelled.
						// How would this be possible? Uncertain until it happens. :/
						// This feels a little cargo culty...
						//
						throw new OperationCanceledException();
					}

					if (result.GotCloseMessage)
					{
						return new ReceiveMessageAsyncResult() { CloseMessageReceived = true };
					}
				}
				//
				// Only used joined source for receive operations. 
				// After the message is written to the memory stream
				// we should just deserialize it and return it.
				//
				IncomingMessageBase message = await JsonSerializer.DeserializeAsync<IncomingMessageBase>(result.Stream, jsonSerializerOptions, cancellationToken);
				return new ReceiveMessageAsyncResult() { Message = message };
			}
		}

		private void CheckAndThrowIfNotInState(SharpHomeAssistantConnectionState expectedState, string methodName)
		{
			if (State != expectedState)
			{
				throw new InvalidOperationException(String.Format("{0} can only be called in the {1}} state. The class is current in the {2} state.", methodName, expectedState, State));
			}
		}

		private async Task<ReceiveOperationResult> ReceiveWebsocketMessageAsync(CancellationToken cancellationToken)
		{

			byte[] buffer = new byte[128];
			System.IO.MemoryStream stream;

			await ReceiveSemaphore.WaitAsync(cancellationToken);

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
						return new ReceiveOperationResult() { Stream = stream, Success = false, GotCloseMessage = true };
					}
					else if (result.MessageType == WebSocketMessageType.Binary)
					{
						throw new SharpHomeAssistantProtocolException("Got binary message from websocket but expected message to be in text format.");
					}

					receiveIncomplete = true;
					//
					// Don't pass in cancellation token here. Write the message to the stream to ensure
					// the cancellation does not result in an inconsistent state.
					//
					await stream.WriteAsync(buffer, 0, result.Count);

					if (MaxMessageSize > 0 && stream.Length > MaxMessageSize)
					{
						// Just completely abort in this case.
						Abort();

						throw new SharpHomeAssistantProtocolException(String.Format("Websocket message exceeds max size of {0} bytes", MaxMessageSize));
					}

				} while (!result.EndOfMessage);

				stream.Seek(0, System.IO.SeekOrigin.Begin);

				return new ReceiveOperationResult() { Stream = stream, Success = true };

			}
			catch (OperationCanceledException)
			{
				if (receiveIncomplete)
				{
					ContinueReceive = true;
					PreviousReceiveStream = stream;
				}
				return new ReceiveOperationResult() { Success = false, MessageReceiveStarted = receiveIncomplete };
			}
			finally
			{
				ReceiveSemaphore.Release();
			}
		}
	}
}