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
	public class ResultMessageBase<ResultType> : HomeAssistantMessage
	{
		public const string MessageType = "result";


		[JsonPropertyName("type")]
		public override string TypeId => MessageType;

		[JsonPropertyName("success")]
		public bool Success { get; set; }

		[JsonPropertyName("result")]
		public ResultType Result { get; set; }

		[JsonPropertyName("id")]
		public int CommandId { get; set; }

		[JsonPropertyName("error")]
		public ResultMessageErrorDetails ErrorDetails { get; set; }
	}
}