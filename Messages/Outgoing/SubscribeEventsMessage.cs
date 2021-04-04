using System.Text.Json;
using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{


	public class SubscribeEventsMessage : CommandMessageBase
	{
		public const string MessageType = "subscribe_events";

		[JsonPropertyName("event_type")]
		public string EventType { get; set; }

		public override string GetTypeId()
		{
			return MessageType;
		}
	}

	// public class SubscribeEventsMessageConverter : CommandMessageBaseConverter<SubscribeEventsMessage>
	// {
	// 	private const string EventTypePropertyName = "event_type";
	// 	public override bool CanConvert(string typeId)
	// 	{
	// 		return typeId == SubscribeEventsMessage.MessageType;
	// 	}

	// 	protected override void OnWriteMessageProperties(Utf8JsonWriter writer, SubscribeEventsMessage message, JsonSerializerOptions options)
	// 	{
	// 		if (!string.IsNullOrEmpty(message.EventType))
	// 		{
	// 			writer.WriteString(EventTypePropertyName, message.EventType);
	// 		}
	// 	}

	// 	protected override string GetMessageType(SubscribeEventsMessage message)
	// 	{
	// 		return SubscribeEventsMessage.MessageType;
	// 	}

	// 	protected override SubscribeEventsMessage OnReadMessageProperties(JsonDocument document, string typeToConvert, JsonSerializerOptions options)
	// 	{
	// 		SubscribeEventsMessage message = new SubscribeEventsMessage();
	// 		JsonElement root = document.RootElement;
	// 		if (root.TryGetProperty(EventTypePropertyName, out JsonElement eventType))
	// 		{
	// 			message.EventType = eventType.GetString();
	// 		}

	// 		return message;
	// 	}
	// }


}