using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpHomeAssistant.Messages;



namespace AudreysCloud.Community.SharpHomeAssistant.Components.InputBoolean
{
	/// <summary>
	/// This command creates an input_boolean.
	/// </summary>
	[MessageType("input_boolean/create")]
	public class CreateInputBooleanCommand : CommandMessageBase
	{
		/// <summary>
		/// The name of the input boolean being created.
		/// </summary>
		public string Name { get; set; }

#nullable enable
		/// <summary>
		/// The initial value for this Input Boolean.
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