
using System.Text.Json;
using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public abstract class CommandMessageBase : OutgoingMessageBase
	{
		[JsonPropertyName("id")]
		public int CommandId { get; set; }
	}

	// public abstract class CommandMessageBaseConverter<MessageType> : IAlgebraicTypeConverter<string> where MessageType : CommandMessageBase
	// {
	// 	protected const string IdPropertyName = "id";

	// 	public abstract bool CanConvert(string typeId);

	// 	protected abstract string GetMessageType(MessageType message);

	// 	protected int GetId(JsonElement root)
	// 	{
	// 		return root.GetProperty(IdPropertyName).GetInt32();
	// 	}

	// 	protected abstract MessageType OnReadMessageProperties(JsonDocument document, string typeToConvert, JsonSerializerOptions options);
	// 	public virtual IAlgebraicType<string> Read(ref Utf8JsonReader reader, string typeToConvert, JsonSerializerOptions options)
	// 	{
	// 		using (JsonDocument document = JsonDocument.ParseValue(ref reader))
	// 		{
	// 			JsonElement root = document.RootElement;
	// 			int id = GetId(root);

	// 			MessageType message = OnReadMessageProperties(document, typeToConvert, options);
	// 			message.CommandId = id;

	// 			return message;
	// 		}
	// 	}
	// 	protected abstract void OnWriteMessageProperties(Utf8JsonWriter writer, MessageType value, JsonSerializerOptions options);

	// 	protected void WriteCommandId(Utf8JsonWriter writer, MessageType value, JsonSerializerOptions options)
	// 	{
	// 		writer.WriteNumber(IdPropertyName, value.CommandId);
	// 	}
	// 	protected void WriteCommandMessage(Utf8JsonWriter writer, IAlgebraicType<string> value, JsonSerializerOptions options)
	// 	{
	// 		MessageType message = (MessageType)value;

	// 		writer.WriteStartObject();

	// 		writer.WriteString(HomeAssistantMessage.PropertyTypeJsonName, GetMessageType(message));
	// 		WriteCommandId(writer, message, options);
	// 		OnWriteMessageProperties(writer, message, options);

	// 		writer.WriteEndObject();
	// 	}

	// 	public virtual void Write(Utf8JsonWriter writer, IAlgebraicType<string> value, JsonSerializerOptions options)
	// 	{
	// 		WriteCommandMessage(writer, value, options);
	// 	}
	// }

}