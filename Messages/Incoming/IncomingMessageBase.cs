using System;
using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public abstract class IncomingMessageBase
	{
		public const string PropertyTypeJsonName = "type";

		/// <summary>
		/// Returns the string value of message type.
		/// 
		/// Application code should use TryConvert instead of this method. This method's main purpose
		/// is to support json conversions and other internal operations.
		/// </summary>
		/// <param name="messageType">The C# type to extract the message string from.</param>
		/// <see cref="TryConvert" />
		/// <see cref="MessageTypeAttribute" />
		/// <returns>String value of the message type as specified in IncomingMessageTypeAttribute.</returns>
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

		/// <summary>
		/// Attempts to convert a IncomingBaseMessage into the desired message type.
		/// </summary>
		/// <typeparam name="TOutType">The type to convert the message base into.</typeparam>
		/// <param name="messageToConvert">The message to convert</param>
		/// <param name="convertedMessage">The converted message. This will be null if the conversion fails.</param>
		/// <returns>True if the message conversion was a success.</returns>
		public static bool TryConvert<TOutType>(IncomingMessageBase messageToConvert, out TOutType convertedMessage) where TOutType : IncomingMessageBase
		{
			convertedMessage = null;

			convertedMessage = messageToConvert as TOutType;
			if (convertedMessage != null)
			{
				return true;
			}

			return false;
		}


		[JsonPropertyName(IncomingMessageBase.PropertyTypeJsonName)]
		public string MessageType
		{
			get
			{
				Type myType = GetType();
				return IncomingMessageBase.GetMessageTypeString(myType);
			}
		}

	}
}