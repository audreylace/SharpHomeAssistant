using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class ResultMessageErrorDetails
	{

		[JsonPropertyName("code")]
		public string Code { get; set; }

		[JsonPropertyName("message")]
		public string Message { get; set; }
	}
}