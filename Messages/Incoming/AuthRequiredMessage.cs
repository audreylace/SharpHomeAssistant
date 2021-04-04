namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class AuthRequiredMessage : IncomingMessageBase
	{
		public const string MessageType = "auth_required";
		public override string TypeId => MessageType;
	}

}