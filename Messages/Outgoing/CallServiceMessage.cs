using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class CallServiceMessage<ServiceDataType> : CommandMessageBase
	{
		public const string MessageType = "call_service";

		[JsonPropertyName("domain")]
		public string Domain { get; set; }

		[JsonPropertyName("service")]
		public string Service { get; set; }


		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("service_data")]

		public ServiceDataType ServiceData { get; set; }

		public override string GetTypeId()
		{
			return MessageType;
		}
	}

}