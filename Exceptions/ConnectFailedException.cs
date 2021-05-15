using System;

namespace AudreysCloud.Community.SharpHomeAssistant.Exceptions
{

	/// <summary>
	/// Exception thrown when the intial connect operation fails.
	/// </summary>
	public class ConnectFailedException : SharpHomeAssistantException
	{
		internal const string ExceptionMessage = "Sharp Home Assistant failed to connect to the home assistant server.";

		/// <summary>
		/// Default cosntructor.
		/// </summary>
		public ConnectFailedException() : base(ExceptionMessage) { }

		/// <summary>
		/// Creates an exception with a message.
		/// </summary>
		/// <param name="message">The message describing the exception.</param>
		public ConnectFailedException(string message) : base(message) { }


		/// <summary>
		/// Creates an exception with a message and a linked inner exception.
		/// </summary>
		/// <param name="message">Message describing the exception.</param>
		/// <param name="innerException">Exception being wrapped that caused this exception.</param>
		public ConnectFailedException(string message, Exception innerException) : base(message, innerException) { }

		/// <summary>
		/// Wraps an exception inside a connect failed exception.
		/// </summary>
		/// <param name="innerException">The exception to wrap.</param>
		public ConnectFailedException(Exception innerException) : base(innerException.Message, innerException) { }
	}
}