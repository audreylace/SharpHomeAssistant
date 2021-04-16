// using System;
// using System.Net.WebSockets;
// using System.Threading;
// using System.Threading.Tasks;

// namespace AudreysCloud.Community.SharpHomeAssistant.Utils
// {

// 	/// <summary>Helper class for working around some of the idiosyncrasies of client web socket.</summary>
// 	internal class WebSocketManager : IDisposable
// 	{

// 		/// <summary>The wrapped websocket. Code can use this to call non-wrapped methods on the websocket. </summary>
// 		public ClientWebSocket WebSocket { get; }

// 		public WebSocketState SocketState
// 		{
// 			get => WebSocket.State;
// 		}

// 		public WebSocketManager() : this(new ClientWebSocket())
// 		{

// 		}
// 		public WebSocketManager(ClientWebSocket socket)
// 		{

// 		}

// 		public void Dispose()
// 		{
// 			WebSocket.Dispose();
// 		}

// 		public async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken) { }
// 		public Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken) { }

// 		public Task CloseAsync(WebSocketCloseStatus closeStatus, string? statusDescription, CancellationToken cancellationToken) { }

// 		public Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string? statusDescription, CancellationToken cancellationToken) { }
// 		public void Abort() { }


// 		private _sendCh

// 	}
// }