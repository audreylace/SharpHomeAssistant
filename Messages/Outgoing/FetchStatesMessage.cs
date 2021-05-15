namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Message to send to get all of the states of the home assistant instance.
	/// </summary>
	public class FetchStatesMessage : CommandMessageBase
	{
#pragma warning disable CS1591
		protected override string GetMessageType() => "get_states";
#pragma warning restore CS1591
	}
}