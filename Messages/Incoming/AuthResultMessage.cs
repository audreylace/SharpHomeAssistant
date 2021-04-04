namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class AuthResultMessage : IncomingMessageBase
	{
		public const string AuthOkType = "auth_ok";
		public const string AuthInvalidType = "auth_invalid";

		public const string MessageType = "auth_result";
		public override string TypeId => MessageType;

		public bool Success { get; set; }
		public string Message { get; set; }
	}



}