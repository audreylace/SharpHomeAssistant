using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class ResultMessageErrorDetails
	{
		[JsonPropertyName("code")]
		public int Code { get; set; }

		[JsonPropertyName("message")]
		public string Message { get; set; }
	}
	public class ResultMessage : IncomingMessageBase
	{
		public const string MessageType = "result";

		[JsonPropertyName("type")]
		public override string TypeId => MessageType;

		[JsonPropertyName("success")]
		public bool Success { get; set; }

		[JsonPropertyName("result")]
		public object Result { get; set; }

		[JsonPropertyName("id")]
		public int CommandId { get; set; }


		[JsonPropertyName("error")]
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public ResultMessageErrorDetails ErrorDetails { get; set; }
	}

	public class ResultMessageConverter : IAlgebraicTypeConverter<string>
	{
		public bool CanConvert(string typeId)
		{
			return typeId == ResultMessage.MessageType;
		}

		public IAlgebraicType<string> Read(ref Utf8JsonReader reader, string typeToConvert, JsonSerializerOptions options)
		{
			return JsonSerializer.Deserialize<ResultMessage>(ref reader, options);
		}

		public void Write(Utf8JsonWriter writer, IAlgebraicType<string> value, JsonSerializerOptions options)
		{
			ResultMessage message = (ResultMessage)value;
			JsonSerializer.Serialize<ResultMessage>(writer, message, options);
		}
	}
}