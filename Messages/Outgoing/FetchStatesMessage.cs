namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class FetchStatesMessage : CommandMessageBase
	{
		protected override string GetMessageType() => "get_states";

	}
}