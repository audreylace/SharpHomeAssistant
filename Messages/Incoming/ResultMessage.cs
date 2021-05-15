using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Result message representing the results of a command message.
	/// </summary>
	[MessageType("result")]
	public class ResultMessage : IncomingMessageBase
	{

		/// <summary>
		/// True when the action was a success.
		/// </summary>
		[JsonPropertyName("success")]
		public bool Success { get; set; }

		/// <summary>
		/// Data sent back as a result of the command. This is a JsonElement.
		/// </summary>
		/// <see cref="JsonElement" />
		[JsonPropertyName("result")]
		public object Result { get; set; }

		/// <summary>
		/// The ID of the initial command sent out.
		/// </summary>
		[JsonPropertyName("id")]
		public int CommandId { get; set; }

		/// <summary>
		/// If success is false then this will have details about why the command did not execute correctly.
		/// </summary>
		[JsonPropertyName("error")]
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public ResultMessageErrorDetails ErrorDetails { get; set; }
	}
}