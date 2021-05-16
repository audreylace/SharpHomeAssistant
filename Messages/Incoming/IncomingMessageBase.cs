using System;
using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Base class of all incoming messages.
	/// </summary>
	public abstract class IncomingMessageBase
	{
		/// <summary>
		/// Name of the JSON type field used to identify the type of message this is.
		/// </summary>
		public const string PropertyTypeJsonName = "type";


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

		/// <summary>
		/// String identifying the type of this message.
		/// </summary>
		[JsonPropertyName(IncomingMessageBase.PropertyTypeJsonName)]
		public string MessageType
		{
			get
			{
				Type myType = GetType();
				return MessageTypeAttribute.GetMessageTypeString(myType);
			}
		}

	}
}