using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Details about why the command failed.
	/// </summary>
	public class ResultMessageErrorDetails
	{

		/// <summary>
		/// A code identifying the type of error.
		/// </summary>
		[JsonPropertyName("code")]
		public string Code { get; set; }

		/// <summary>
		/// A human readable message describing the error.
		/// </summary>
		[JsonPropertyName("message")]
		public string Message { get; set; }
	}
}