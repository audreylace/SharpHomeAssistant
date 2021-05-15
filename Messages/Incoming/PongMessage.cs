using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Pong message sent back in response to a ping message.
	/// </summary>
	/// <see cref="PingMessage" />
	[MessageType("pong")]
	public class PongMessage : IncomingMessageBase
	{
		/// <summary>
		/// ID of the ping message this pong message is in response to.
		/// </summary>
		[JsonPropertyName("id")]
		public int CommandId { get; set; }
	}


}