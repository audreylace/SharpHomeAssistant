using System;
using System.Text.Json;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class AuthResultMessage : IncomingMessageBase
	{
		public const string AuthOkType = "auth_ok";
		public const string AuthInvalidType = "auth_invalid";

		public const string MessageType = "auth_result";
		public override string TypeId => MessageType;

		public bool Success { get; set; }
		public string Message { get; set; }
	}

	public class AuthResultMessageConverter : IAlgebraicTypeConverter<string>
	{
		public bool CanConvert(string typeId)
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

		public IAlgebraicType<string> Read(ref Utf8JsonReader reader, string typeToConvert, JsonSerializerOptions options)
		{
			switch (typeToConvert)
			{
				case AuthResultMessage.AuthOkType:
					return new AuthResultMessage() { Success = true };
				case AuthResultMessage.AuthInvalidType:
					string failReason;
					using (JsonDocument document = JsonDocument.ParseValue(ref reader))
					{
						JsonElement messageElement = document.RootElement.GetProperty("message");
						failReason = messageElement.GetString();
					}
					return new AuthResultMessage() { Success = false, Message = failReason };
				default:
					throw new JsonException();
			}
		}

		public void Write(Utf8JsonWriter writer, IAlgebraicType<string> value, JsonSerializerOptions options)
		{
			AuthResultMessage message = (AuthResultMessage)value;

			switch (message.TypeId)
			{
				case AuthResultMessage.AuthOkType:
					writer.WriteStartObject();
					writer.WriteString("type", AuthResultMessage.AuthOkType);
					writer.WriteEndObject();
					break;
				case AuthResultMessage.AuthInvalidType:
					writer.WriteStartObject();
					writer.WriteString(IncomingMessageBase.PropertyTypeJsonName, AuthResultMessage.AuthInvalidType);
					writer.WriteString("mesage", message.Message);
					writer.WriteEndObject();
					break;
				default:
					throw new NotImplementedException();

			}
		}
	}



}