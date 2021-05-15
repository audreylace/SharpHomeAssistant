namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Message to send to get all of the states of the home assistant instance.
	/// </summary>
	public class FetchStatesMessage : CommandMessageBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns>A string with the value of "get_states."</returns>
		protected override string GetMessageType() => "get_states";

	}
}