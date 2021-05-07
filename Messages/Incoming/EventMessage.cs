using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	[MessageType("event")]
	public class EventMessage : IncomingMessageBase
	{

		[JsonPropertyName("id")]
		public int CommandId { get; set; }

		[JsonPropertyName("event")]
		public object Event { get; set; }

	}
}