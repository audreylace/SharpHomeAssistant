using System.Text.Json;
using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{


	public class SubscribeEventsMessage : CommandMessageBase
	{
		public override string GetMessageType() => "subscribe_events";

		[JsonPropertyName("event_type")]
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public string EventName { get; set; }

	}
}