using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpHomeAssistant.Messages;

namespace AudreysCloud.Community.SharpHomeAssistant.Components.InputBoolean
{
	/// <summary>
	/// Gets the list of input booleans.
	/// </summary>
	[MessageType("input_boolean/list")]
	public class ListInputBooleanCommand : CommandMessageBase
	{

	}
}