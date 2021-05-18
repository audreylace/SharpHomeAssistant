using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpHomeAssistant.Messages;


namespace AudreysCloud.Community.SharpHomeAssistant.Components.InputBoolean
{
	/// <summary>
	/// This command is used to update a input_boolean.
	/// </summary>
	[MessageType("input_boolean/update")]
	public class UpdateInputBooleanCommand : CommandMessageBase
	{

		/// <summary>
		/// The id of the input_boolean being updated.
		/// </summary>
		[JsonPropertyName("input_boolean_id")]
		public string EntityId { get; set; }


#nullable enable

		/// <summary>
		/// Set to change the name of the input boolean.
		/// </summary>
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string? Name { get; set; }

		/// <summary>
		/// The new value for this Input Boolean.
		/// </summary>
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public bool? Initial { get; set; }

		/// <summary>
		/// The icon this variable will have in the UI.
		/// </summary>
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string? Icon { get; set; }

#nullable restore

	}
}