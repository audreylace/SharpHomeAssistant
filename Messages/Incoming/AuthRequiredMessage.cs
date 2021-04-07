namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	internal class AuthRequiredMessage : IncomingMessageBase
	{
		public const string MessageType = "auth_required";
		public override string TypeId => MessageType;
	}

}