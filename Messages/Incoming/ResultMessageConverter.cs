using System.Text.Json;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class ResultMessageConverter : IncomingMessageBaseConverter<ResultMessage>
	{
		public override IncomingMessageBase Read(ref Utf8JsonReader reader, string typeToConvert, JsonSerializerOptions options)
		{
			return JsonSerializer.Deserialize<ResultMessage>(ref reader, options);
		}

		public override void Write(Utf8JsonWriter writer, IncomingMessageBase value, JsonSerializerOptions options)
		{
			ResultMessage message = (ResultMessage)value;
			JsonSerializer.Serialize<ResultMessage>(writer, message, options);
		}
	}
}