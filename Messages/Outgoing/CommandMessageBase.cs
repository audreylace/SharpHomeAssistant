
using System.Text.Json;
using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Base class of all outgoing command messages.
	/// </summary>
	public abstract class CommandMessageBase : OutgoingMessageBase
	{
		/// <summary>
		/// The ID of this command. The response will reference this ID.
		/// </summary>
		[JsonPropertyName("id")]
		public long CommandId { get; set; }
	}

}