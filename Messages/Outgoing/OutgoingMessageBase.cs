using AudreysCloud.Community.SharpHomeAssistant.Utils;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public abstract class OutgoingMessageBase
	{

		protected abstract string GetMessageType();

		[JsonPropertyName("type")]
		public string MessageType { get => GetMessageType(); }
	}


}