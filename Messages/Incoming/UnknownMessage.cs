using System.Text.Json;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	[MessageType("unknown")]
	public class UnknownMessage : IncomingMessageBase
	{
		public JsonElement Message { get; set; }
		public string UnknownMessageType { get; set; }
	}
}