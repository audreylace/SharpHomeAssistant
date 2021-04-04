using AudreysCloud.Community.SharpHomeAssistant.Messages;

namespace AudreysCloud.Community.SharpHomeAssistant
{

	public class ReceiveMessageAsyncResult
	{

		public IncomingMessageBase Message { get; set; }

		public ReceiveMessageAsyncStatus Status { get; set; }

		public byte[] BinaryMessage { get; set; }

	}
}