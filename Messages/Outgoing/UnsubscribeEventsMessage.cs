
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Message sent to stop receiving the events initially subscribed to via the SubscribeEventsMessage.
	/// </summary>
	/// <see cref="SubscribeEventsMessage" />
	public class UnsubscribeEventsMessage : CommandMessageBase
	{

#pragma warning disable CS1591
		protected override string GetMessageType() => "unsubscribe_events";
#pragma warning restore CS1591

		/// <summary>
		/// The command id of the initial SubscribeEventsMessage.
		/// </summary>

		[JsonPropertyName("subscription")]
		public int Subscription { get; set; }
	}
}