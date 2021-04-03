using System.Text.Json;
using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{

	public class SubscribeEventsMessage : HomeAssistantMessage
	{
		public const string MessageType = "subscribe_events";
		public override string TypeId => MessageType;

		public string EventType { get; set; }

		public int CommandId { get; set; }
	}

	public class SubscribeEventsMessageConverter : IAlgebraicTypeConverter<string>
	{
		private const string EventTypePropertyName = "event_type";
		private const string IdPropertyName = "id";
		public bool CanConvert(string typeId)
		{
			return typeId == SubscribeEventsMessage.MessageType;
		}

		public IAlgebraicType<string> Read(ref Utf8JsonReader reader, string typeToConvert, JsonSerializerOptions options)
		{

			using (JsonDocument document = JsonDocument.ParseValue(ref reader))
			{
				JsonElement root = document.RootElement;
				int id = root.GetProperty(IdPropertyName).GetInt32();
				SubscribeEventsMessage message = new SubscribeEventsMessage() { CommandId = id };

				if (root.TryGetProperty(EventTypePropertyName, out JsonElement eventType))
				{
					message.EventType = eventType.GetString();
				}

				return message;
			}
		}

		public void Write(Utf8JsonWriter writer, IAlgebraicType<string> value, JsonSerializerOptions options)
		{
			SubscribeEventsMessage message = (SubscribeEventsMessage)value;

			writer.WriteStartObject();
			writer.WriteString(HomeAssistantMessage.PropertyTypeJsonName, SubscribeEventsMessage.MessageType);
			writer.WriteNumber(IdPropertyName, message.CommandId);
			if (!string.IsNullOrEmpty(message.EventType))
			{
				writer.WriteString(EventTypePropertyName, message.EventType);
			}
			writer.WriteEndObject();
		}
	}


}