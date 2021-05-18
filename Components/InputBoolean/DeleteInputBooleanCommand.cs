using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpHomeAssistant.Messages;

namespace AudreysCloud.Community.SharpHomeAssistant.Components.InputBoolean
{
	/// <summary>
	/// This command deletes an input_boolean.
	/// </summary>
	[MessageType("input_boolean/create")]
	public class DeleteInputBooleanCommand : CommandMessageBase
	{
		/// <summary>
		/// The id of the input_boolean being deleted.
		/// </summary>
		[JsonPropertyName("input_boolean_id")]
		public string EntityId { get; set; }
	}
}