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
		public string Code { get; set; }

		/// <summary>
		/// A human readable message describing the error.
		/// </summary>
		public string Message { get; set; }
	}
}