using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class EventMessage : IncomingMessageBase
	{

		public const string MessageType = "event";

		public override string TypeId => MessageType;

		[JsonPropertyName("id")]
		public int CommandId { get; set; }

		[JsonPropertyName("event")]
		public object Event { get; set; }

	}
}