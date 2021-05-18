using AudreysCloud.Community.SharpHomeAssistant.Utils;
using System;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Base class of all outgoing messages.
	/// </summary>
	public abstract class OutgoingMessageBase
	{
		/// <summary>
		/// Name of the JSON type field used to identify the type of message this is.
		/// </summary>
		public const string PropertyTypeJsonName = "type";

		/// <summary>
		/// Derived classes override this method to declare the message type. This will be used to populate the type entry of the outging message.
		/// </summary>
		/// <see cref="MessageType" />
		/// <returns>String identifying the message type.</returns>
		protected virtual string GetMessageType()
		{
			Type myType = GetType();
			return MessageTypeAttribute.GetMessageTypeString(myType);

		}

		/// <summary>
		/// Type field of the message used by the Home Assistant instance to know what type of message this is.
		/// </summary>
		[JsonPropertyName(OutgoingMessageBase.PropertyTypeJsonName)]
		public string MessageType { get => GetMessageType(); }
	}


}