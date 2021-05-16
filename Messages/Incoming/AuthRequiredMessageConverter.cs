using System.Text.Json;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	internal class AuthRequiredMessageConverter : IncomingMessageBaseConverter<AuthRequiredMessage>
	{

		public override IncomingMessageBase Read(ref Utf8JsonReader reader, string typeToConvert, JsonSerializerOptions options)
		{
			using (JsonDocument document = JsonDocument.ParseValue(ref reader)) //Parse the value to advance the reader
			{ }

			return new AuthRequiredMessage();
		}

		public override void Write(Utf8JsonWriter writer, IncomingMessageBase value, string typeToConvert, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			WriteTypeAttribute(writer);
			writer.WriteEndObject();
		}
	}

}