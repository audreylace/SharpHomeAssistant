using System.Text.Json;
using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{

	/// <summary>
	/// Subscribes to events on the event bus of the Home Assistant Instance.
	/// </summary>
	public class SubscribeEventsMessage : CommandMessageBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns>A string with the value of "subscribe_events."</returns>
		protected override string GetMessageType() => "subscribe_events";

		/// <summary>
		/// The name of the event to subscribe too. If left empty, then all events are subscribed to.
		/// </summary>
		[JsonPropertyName("event_type")]
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public string EventName { get; set; }

	}
}