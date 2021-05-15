using System;

namespace AudreysCloud.Community.SharpHomeAssistant.Exceptions
{

	/// <summary>
	/// Exception thrown when a protocl error occurs.
	/// </summary>
	public class SharpHomeAssistantProtocolException : SharpHomeAssistantException
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public SharpHomeAssistantProtocolException() : base("Sharp Home Assistant Protocol Exception") { }

		/// <summary>
		/// Creates an exception with a message.
		/// </summary>
		/// <param name="message">The message describing the exception.</param>
		public SharpHomeAssistantProtocolException(string message) : base(message) { }

		/// <summary>
		/// Creates an exception with a message and a linked inner exception.
		/// </summary>
		/// <param name="message">Message describing the exception.</param>
		/// <param name="innerException">Exception being wrapped that caused this exception.</param>
		public SharpHomeAssistantProtocolException(string message, Exception innerException) : base(message, innerException) { }
	}

}