
using System.Text.Json;
using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class PongMessage : IncomingMessageBase
	{
		public const string MessageType = "pong";

		[JsonPropertyName("type")]
		public override string TypeId => MessageType;

		[JsonPropertyName("id")]
		public int CommandId { get; set; }

	}

	public class PongMessageConverter : IAlgebraicTypeConverter<string>
	{
		public bool CanConvert(string typeId)
		{
			return typeId == PongMessage.MessageType;
		}

		public IAlgebraicType<string> Read(ref Utf8JsonReader reader, string typeToConvert, JsonSerializerOptions options)
		{
			return JsonSerializer.Deserialize<PongMessage>(ref reader, options);
		}

		public void Write(Utf8JsonWriter writer, IAlgebraicType<string> value, JsonSerializerOptions options)
		{
			PongMessage message = (PongMessage)value;
			JsonSerializer.Serialize<PongMessage>(writer, message, options);
		}
	}
}