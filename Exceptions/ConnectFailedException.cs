using System;

namespace AudreysCloud.Community.SharpHomeAssistant.Exceptions
{

	public class ConnectFailedException : SharpHomeAssistantException
	{
		internal const string ExceptionMessage = "Sharp Home Assistant failed to connect to the home assistant server.";
		public ConnectFailedException() : base(ExceptionMessage) { }
		public ConnectFailedException(string message) : base(message) { }
		public ConnectFailedException(string message, Exception innerException) : base(message, innerException) { }
		public ConnectFailedException(Exception ex) : base(ex.Message, ex) { }
	}
}