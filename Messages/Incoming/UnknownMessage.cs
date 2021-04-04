using System.Text.Json;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class UnknownMessage : IncomingMessageBase
	{
		public const string MessageType = "unknown";
		public override string TypeId => MessageType;
		public JsonElement Message { get; set; }
		public string UnknownMessageType { get; set; }
	}
}