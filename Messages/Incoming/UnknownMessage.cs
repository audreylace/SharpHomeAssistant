using System.Text.Json;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Represents a message with no defined C# class to represent it.
	/// </summary>
	[MessageType("unknown")]
	public class UnknownMessage : IncomingMessageBase
	{
		/// <summary>
		/// The raw receieved message.
		/// </summary>
		public JsonElement Message { get; set; }

		/// <summary>
		/// The value of the message "type" field.
		/// </summary>
		public string UnknownMessageType { get; set; }
	}
}