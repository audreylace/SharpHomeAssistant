
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class UnsubscribeEventsMessage : CommandMessageBase
	{
		public override string GetMessageType() => "unsubscribe_events";

		[JsonPropertyName("subscription")]
		public int Subscription { get; set; }
	}
}