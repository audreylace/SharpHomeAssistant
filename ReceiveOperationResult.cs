using System;
using System.IO;

namespace AudreysCloud.Community.SharpHomeAssistant
{
	internal class ReceiveOperationResult : IDisposable
	{
		public bool Success { get; set; }
		public MemoryStream Stream { get; set; }
		public bool OperationCancelled { get; set; }

		public bool GotCloseMessage { get; set; }

		public bool GotBinaryMessage { get; set; }
		public bool MessageOverflow { get; set; }

		public void Dispose()
		{
			if (Stream != null)
			{
				Stream.Dispose();
			}
		}

		public void ThrowIfBinary()
		{
			if (GotBinaryMessage)
			{
				throw new Exception("Got Binary message when expecting a text based message.");
			}
		}

		public void ThrowIfFailed()
		{
			if (!Success)
			{
				if (GotCloseMessage)
				{
					throw new Exception("Receive operation failed because the close message was received on the web socket.");
				}

				if (OperationCancelled)
				{
					throw new Exception("Receive operation failed because the operation was cancelled.");
				}

				if (MessageOverflow)
				{
					throw new Exception("Message exceeded the max message size.");
				}

				throw new Exception("Receive operation failed.");
			}
		}
	}
}