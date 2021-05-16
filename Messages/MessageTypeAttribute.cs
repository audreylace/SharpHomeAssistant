using System;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{

	/// <summary>
	/// Attribute used to specify the type of message.
	/// </summary>
	public class MessageTypeAttribute : System.Attribute
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="messageType">A string describing the value of the type field.</param>
		public MessageTypeAttribute(string messageType)
		{
			MessageType = messageType;
		}

		/// <summary>
		/// The specified message type.
		/// </summary>
		public string MessageType { get; set; }

		/// <summary>
		/// Returns the string value of message type.
		/// </summary>
		/// <param name="messageType">The C# type to extract the message string from.</param>
		/// <see cref="MessageTypeAttribute" />
		/// <returns>String value of the message type as specified in MessageTypeAttribute.</returns>
		public static string GetMessageTypeString(Type messageType)
		{
			Attribute[] attributes = Attribute.GetCustomAttributes(messageType);
			foreach (Attribute attribute in attributes)
			{
				MessageTypeAttribute messageTypeAttribute = attribute as MessageTypeAttribute;

				if (messageTypeAttribute != null)
				{
					return messageTypeAttribute.MessageType;
				}
			}

			throw new InvalidOperationException(String.Format("Passed in type {0} did not have a MessageTypeAttribute", messageType));
		}

	}

}