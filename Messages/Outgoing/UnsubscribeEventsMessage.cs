
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class UnsubscribeEventsMessage : CommandMessageBase
	{
		public const string MessageType = "unsubscribe_events";

		[JsonPropertyName("subscription")]
		public int Subscription { get; set; }

		public override string GetTypeId()
		{
			return MessageType;
		}
	}

	// public class UnsubscribeEventsMessageConverter : CommandMessageBaseConverter<UnsubscribeEventsMessage>
	// {
	// 	private const string SubscriptionPropertyName = "subscription";
	// 	public override bool CanConvert(string typeId)
	// 	{
	// 		return UnsubscribeEventsMessage.MessageType == typeId;
	// 	}

	// 	protected override string GetMessageType(UnsubscribeEventsMessage message)
	// 	{
	// 		return UnsubscribeEventsMessage.MessageType;
	// 	}

	// 	protected override UnsubscribeEventsMessage OnReadMessageProperties(JsonDocument document, string typeToConvert, JsonSerializerOptions options)
	// 	{
	// 		UnsubscribeEventsMessage message = new UnsubscribeEventsMessage();
	// 		JsonElement element = document.RootElement;

	// 		message.Subscription = element.GetProperty(SubscriptionPropertyName).GetInt32();

	// 		return message;
	// 	}

	// 	protected override void OnWriteMessageProperties(Utf8JsonWriter writer, UnsubscribeEventsMessage value, JsonSerializerOptions options)
	// 	{
	// 		writer.WriteNumber(SubscriptionPropertyName, value.Subscription);
	// 	}
	// }
}