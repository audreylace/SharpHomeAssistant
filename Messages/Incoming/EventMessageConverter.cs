using System.Text.Json;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class EventMessageConverter : IncomingMessageBaseConverter<EventMessage>
	{
		public override IncomingMessageBase Read(ref Utf8JsonReader reader, string typeToConvert, JsonSerializerOptions options)
		{
			return JsonSerializer.Deserialize<EventMessage>(ref reader, options);
		}

		public override void Write(Utf8JsonWriter writer, IncomingMessageBase value, JsonSerializerOptions options)
		{
			EventMessage message = (EventMessage)value;
			JsonSerializer.Serialize<EventMessage>(writer, message, options);
		}
	}
}