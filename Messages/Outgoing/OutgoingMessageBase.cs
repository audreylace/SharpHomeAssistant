using AudreysCloud.Community.SharpHomeAssistant.Utils;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public abstract class OutgoingMessageBase : HomeAssistantMessage
	{
		public abstract string GetTypeId();

		[JsonPropertyName("type")]
		public override string TypeId => GetTypeId();
	}


}