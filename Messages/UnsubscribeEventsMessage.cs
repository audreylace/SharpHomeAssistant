
using System.Text.Json;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class UnsubscribeEventsMessage : CommandMessageBase
	{
		public const string MessageType = "unsubscribe_events";
		public override string TypeId => MessageType;
		public int Subscription { get; set; }
	}

	public class UnsubscribeEventsMessageConverter : CommandMessageBaseConverter<UnsubscribeEventsMessage>
	{
		private const string SubscriptionPropertyName = "subscription";
		public override bool CanConvert(string typeId)
		{
			return UnsubscribeEventsMessage.MessageType == typeId;
		}

		protected override string GetMessageType(UnsubscribeEventsMessage message)
		{
			return UnsubscribeEventsMessage.MessageType;
		}

		protected override UnsubscribeEventsMessage OnReadMessageProperties(JsonDocument document, string typeToConvert, JsonSerializerOptions options)
		{
			UnsubscribeEventsMessage message = new UnsubscribeEventsMessage();
			JsonElement element = document.RootElement;

			message.Subscription = element.GetProperty(SubscriptionPropertyName).GetInt32();

			return message;
		}

		protected override void OnWriteMessageProperties(Utf8JsonWriter writer, UnsubscribeEventsMessage value, JsonSerializerOptions options)
		{
			writer.WriteNumber(SubscriptionPropertyName, value.Subscription);
		}
	}
}