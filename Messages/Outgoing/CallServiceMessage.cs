using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Message sent to invoke a service action.
	/// </summary>
	/// <typeparam name="ServiceDataType">Type of the ServiceData field.</typeparam>
	[MessageType("call_service")]
	public class CallServiceMessage<ServiceDataType> : CommandMessageBase
	{
		/// <summary>
		/// The domain of the service. Aka light in "light.turn_on."
		/// </summary>
		[JsonPropertyName("domain")]
		public string Domain { get; set; }

		/// <summary>
		/// The service name, aka, turn_on in "light.turn_on."
		/// </summary>
		[JsonPropertyName("service")]
		public string Service { get; set; }


		/// <summary>
		/// Arguments to send to the service.
		/// </summary>
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("service_data")]

		public ServiceDataType ServiceData { get; set; }

	}

}