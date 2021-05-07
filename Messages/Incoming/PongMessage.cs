using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	[MessageType("pong")]
	public class PongMessage : IncomingMessageBase
	{
		[JsonPropertyName("id")]
		public int CommandId { get; set; }
	}


}