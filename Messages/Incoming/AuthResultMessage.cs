namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{

	[MessageType("auth_result")]
	internal class AuthResultMessage : IncomingMessageBase
	{
		public const string AuthOkType = "auth_ok";
		public const string AuthInvalidType = "auth_invalid";

		public bool Success { get; set; }
		public string Message { get; set; }
	}



}