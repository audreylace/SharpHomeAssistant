namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class FetchStatesMessage : CommandMessageBase
	{
		public override string GetMessageType() => "get_states";

	}
}