using System;

namespace AudreysCloud.Community.SharpHomeAssistant.Exceptions
{

	public class SharpHomeAssistantProtocolException : SharpHomeAssistantException
	{
		public SharpHomeAssistantProtocolException() : base("Sharp Home Assistant Protocol Exception") { }
		public SharpHomeAssistantProtocolException(string message) : base(message) { }
		public SharpHomeAssistantProtocolException(string message, Exception innerException) : base(message, innerException) { }
	}

}