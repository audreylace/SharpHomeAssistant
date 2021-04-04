using AudreysCloud.Community.SharpHomeAssistant.Messages;

namespace AudreysCloud.Community.SharpHomeAssistant
{
	public class ReceiveMessageAsyncResult
	{
		public bool CloseMessageReceived { get; set; }
		public IncomingMessageBase Message { get; set; }
	}
}