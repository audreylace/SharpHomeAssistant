namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Message to send to get all of the states of the home assistant instance.
	/// </summary>
	[MessageType("get_states")]
	public class FetchStatesMessage : CommandMessageBase
	{
	}
}