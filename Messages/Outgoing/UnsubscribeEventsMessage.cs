
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Message sent to stop receiving the events initially subscribed to via the SubscribeEventsMessage.
	/// </summary>
	/// <see cref="SubscribeEventsMessage" />
	[MessageType("unsubscribe_events")]
	public class UnsubscribeEventsMessage : CommandMessageBase
	{
		/// <summary>
		/// The command id of the initial SubscribeEventsMessage.
		/// </summary>

		[JsonPropertyName("subscription")]
		public int Subscription { get; set; }
	}
}