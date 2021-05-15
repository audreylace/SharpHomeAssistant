using System;

namespace AudreysCloud.Community.SharpHomeAssistant.Exceptions
{

	/// <summary>
	/// General exception class thrown by SharpHomeAssistant.
	/// </summary>
	public class SharpHomeAssistantException : Exception
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public SharpHomeAssistantException() : base() { }

		/// <summary>
		/// Creates an exception with a message.
		/// </summary>
		/// <param name="message">The message describing the exception.</param>
		public SharpHomeAssistantException(string message) : base(message) { }

		/// <summary>
		/// Creates an exception with a message and a linked inner exception.
		/// </summary>
		/// <param name="message">Message describing the exception.</param>
		/// <param name="innerException">Exception being wrapped that caused this exception.</param>
		public SharpHomeAssistantException(string message, Exception innerException) : base(message, innerException) { }
	}
}