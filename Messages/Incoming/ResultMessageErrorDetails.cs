using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class ResultMessageErrorDetails
	{
		//TODO - Code is broken, a string is getting sent back, ignore for now
		[JsonIgnore]
		[JsonPropertyName("code")]
		public int Code { get; set; }

		[JsonPropertyName("message")]
		public string Message { get; set; }
	}
}