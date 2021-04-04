using System;

namespace AudreysCloud.Community.SharpHomeAssistant.Exceptions
{

	public class SharpHomeAssistantException : Exception
	{
		public SharpHomeAssistantException() : base() { }

		public SharpHomeAssistantException(string message) : base(message) { }

		public SharpHomeAssistantException(string message, Exception innerException) : base(message, innerException) { }
	}
}