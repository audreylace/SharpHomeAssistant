using System.Text.Json;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class AuthRequiredMessageConverter : IAlgebraicTypeConverter<string>
	{
		public bool CanConvert(string typeId)
		{
			return typeId == AuthRequiredMessage.MessageType;
		}

		public IAlgebraicType<string> Read(ref Utf8JsonReader reader, string typeToConvert, JsonSerializerOptions options)
		{
			using (JsonDocument document = JsonDocument.ParseValue(ref reader)) //Parse the value to advance the reader
			{ }

			return new AuthRequiredMessage();
		}

		public void Write(Utf8JsonWriter writer, IAlgebraicType<string> value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteString(IncomingMessageBase.PropertyTypeJsonName, AuthRequiredMessage.MessageType);
			writer.WriteEndObject();
		}
	}

}