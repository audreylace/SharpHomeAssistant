using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class PongMessage : IncomingMessageBase
	{
		public const string MessageType = "pong";

		[JsonPropertyName("type")]
		public override string TypeId => MessageType;

		[JsonPropertyName("id")]
		public int CommandId { get; set; }

	}
}