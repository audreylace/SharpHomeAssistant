using System;
using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public abstract class IncomingMessageBase
	{
		public const string PropertyTypeJsonName = "type";

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

		public static bool TryConvert<TOutType>(IncomingMessageBase messageBase, out TOutType outMessage) where TOutType : IncomingMessageBase
		{
			outMessage = null;

			outMessage = messageBase as TOutType;
			if (outMessage != null)
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