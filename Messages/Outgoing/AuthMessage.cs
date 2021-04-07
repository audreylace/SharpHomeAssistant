using System.Text.Json;
using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	internal class AuthMessage : OutgoingMessageBase
	{
		public const string MessageType = "auth";
		public override string GetTypeId()
		{
			return MessageType;
		}

		[JsonPropertyName("access_token")]
		public string AccessToken { get; set; }
	}

	// public class AuthMessageConverter : IAlgebraicTypeConverter<string>
	// {
	// 	private const string AccessTokenPropertyName = "access_token";
	// 	public bool CanConvert(string typeId)
	// 	{
	// 		return AuthMessage.MessageType == typeId;
	// 	}

	// 	public IAlgebraicType<string> Read(ref Utf8JsonReader reader, string typeToConvert, JsonSerializerOptions options)
	// 	{
	// 		using (JsonDocument document = JsonDocument.ParseValue(ref reader))
	// 		{
	// 			JsonElement accessTokenProperty = document.RootElement.GetProperty(AccessTokenPropertyName);
	// 			string token = accessTokenProperty.GetString();
	// 			return new AuthMessage() { AccessToken = token };
	// 		}
	// 	}

	// 	public void Write(Utf8JsonWriter writer, IAlgebraicType<string> value, JsonSerializerOptions options)
	// 	{
	// 		AuthMessage message = (AuthMessage)value;

	// 		writer.WriteStartObject();
	// 		writer.WriteString(HomeAssistantMessage.PropertyTypeJsonName, AuthMessage.MessageType);
	// 		writer.WriteString(AccessTokenPropertyName, message.AccessToken);
	// 		writer.WriteEndObject();
	// 	}
	// }
}