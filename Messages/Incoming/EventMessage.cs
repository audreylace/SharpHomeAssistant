using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Message sent by the remote instance as part of an active event subscription.
	/// </summary>
	/// <see cref="SubscribeEventsMessage" />
	/// <see cref="UnsubscribeEventsMessage" />
	[MessageType("event")]
	public class EventMessage : IncomingMessageBase
	{
		/// <summary>
		/// The ID of this event subscription. This is the ID of the initial SubscribeEventsMessage command.
		/// </summary>

		[JsonPropertyName("id")]
		public int CommandId { get; set; }

		/// <summary>
		/// The Event data. If present, this will be a JsonElement per the specs of System.Text.Json.
		/// </summary>
		/// <see cref="JsonElement" />
		[JsonPropertyName("event")]
		public object Event { get; set; }

	}
}