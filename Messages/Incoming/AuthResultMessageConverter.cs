using System;
using System.Text.Json;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	internal class AuthResultMessageConverter : IncomingMessageBaseConverter<AuthRequiredMessage>
	{
		public override bool CanConvert(string typeId)
		{
			switch (typeId)
			{
				case AuthResultMessage.AuthOkType:
				case AuthResultMessage.AuthInvalidType:
					return true;
				default:
					return false;
			}
		}

		public override IncomingMessageBase Read(ref Utf8JsonReader reader, string typeToConvert, JsonSerializerOptions options)
		{
			using (JsonDocument document = JsonDocument.ParseValue(ref reader))
			{
				switch (typeToConvert)
				{
					case AuthResultMessage.AuthOkType:
						return new AuthResultMessage() { Success = true };
					case AuthResultMessage.AuthInvalidType:
						string failReason;

						JsonElement messageElement = document.RootElement.GetProperty("message");
						failReason = messageElement.GetString();

						return new AuthResultMessage() { Success = false, Message = failReason };
					default:
						throw new JsonException();
				}
			}
		}

		public override void Write(Utf8JsonWriter writer, IncomingMessageBase value, string typeToConvert, JsonSerializerOptions options)
		{
			AuthResultMessage message = (AuthResultMessage)value;

			if (message.Success)
			{
				writer.WriteStartObject();
				writer.WriteString(IncomingMessageBase.PropertyTypeJsonName, AuthResultMessage.AuthOkType);
				writer.WriteEndObject();
			}
			else
			{

				writer.WriteStartObject();
				writer.WriteString(IncomingMessageBase.PropertyTypeJsonName, AuthResultMessage.AuthInvalidType);
				writer.WriteString("mesage", message.Message);
				writer.WriteEndObject();
			}

		}
	}
}
