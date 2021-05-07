
using System.Text.Json;
using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public abstract class CommandMessageBase : OutgoingMessageBase
	{
		[JsonPropertyName("id")]
		public long CommandId { get; set; }
	}

}