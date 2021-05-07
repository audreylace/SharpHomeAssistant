
using System.Text.Json;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	internal class PongMessageConverter : IncomingMessageBaseConverter<PongMessage>
	{
		public override IncomingMessageBase Read(ref Utf8JsonReader reader, string typeToConvert, JsonSerializerOptions options)
		{
			return JsonSerializer.Deserialize<PongMessage>(ref reader, options);
		}

		public override void Write(Utf8JsonWriter writer, IncomingMessageBase value, JsonSerializerOptions options)
		{
			PongMessage message = (PongMessage)value;
			JsonSerializer.Serialize<PongMessage>(writer, message, options);
		}
	}
}