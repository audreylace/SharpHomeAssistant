using System;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	[MessageType("result")]
	public class ResultMessage : IncomingMessageBase
	{

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
}