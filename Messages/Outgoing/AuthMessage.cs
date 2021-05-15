using System.Text.Json;
using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	internal class AuthMessage : OutgoingMessageBase
	{
		protected override string GetMessageType() => "auth";

		[JsonPropertyName("access_token")]
		public string AccessToken { get; set; }
	}
}