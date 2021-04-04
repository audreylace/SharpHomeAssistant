using System;
using AudreysCloud.Community.SharpHomeAssistant.Exceptions;

namespace AudreysCloud.Community.SharpHomeAssistant
{
	public class ConnectResult
	{
		public bool Success { get; set; }
		public string Message { get; set; }

		public Exception Exception { get; set; }
		public void ThrowIfFailed()
		{
			if (!Success)
			{
				if (Exception != null)
				{
					throw new ConnectFailedException(ConnectFailedException.ExceptionMessage, Exception);
				}

				if (!String.IsNullOrEmpty(Message))
				{
					throw new ConnectFailedException(String.Format("Failed to connect to remote Home Assistant server. Server sent the following error message \"{0}.\"", Message));
				}

				throw new ConnectFailedException();
			}
		}
	}
}