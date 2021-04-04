namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class FetchStatesMessage : CommandMessageBase
	{
		public const string MessageType = "get_states";
		public override string GetTypeId()
		{
			return MessageType;
		}
	}
}