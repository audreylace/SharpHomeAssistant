
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
		/// <summary>
		/// Used by OutgoingMessageBase to populate the message type field. Message type field is used
		/// by the remote home assistant instance to know what type of message this is.
		/// </summary>
		/// <returns>String identifying this message type.</returns>
		protected override string GetMessageType() => "unsubscribe_events";


		/// <summary>
		/// The command id of the initial SubscribeEventsMessage.
		/// </summary>

		[JsonPropertyName("subscription")]
		public int Subscription { get; set; }
	}
}