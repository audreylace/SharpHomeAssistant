using System.Text.Json;
using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	[MessageType("auth")]
	internal class AuthMessage : OutgoingMessageBase
	{
		[JsonPropertyName("access_token")]
		public string AccessToken { get; set; }
	}
}