using System.Text.Json;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class AuthRequiredMessage : IncomingMessageBase
	{
		public const string MessageType = "auth_required";
		public override string TypeId => MessageType;
	}

	public class AuthRequiredMessageConverter : IAlgebraicTypeConverter<string>
	{
		public bool CanConvert(string typeId)
		{
			return typeId == AuthRequiredMessage.MessageType;
		}

		public IAlgebraicType<string> Read(ref Utf8JsonReader reader, string typeToConvert, JsonSerializerOptions options)
		{
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