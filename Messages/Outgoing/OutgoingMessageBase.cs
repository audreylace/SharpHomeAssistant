using AudreysCloud.Community.SharpHomeAssistant.Utils;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Base class of all outgoing messages.
	/// </summary>
	public abstract class OutgoingMessageBase
	{

		/// <summary>
		/// Derived classes override this method to declare the message type. This will be used to populate the type entry of the outging message.
		/// </summary>
		/// <see cref="MessageType" />
		/// <returns>String identifying the message type.</returns>
		protected abstract string GetMessageType();

		/// <summary>
		/// Type field of the message used by the Home Assistant instance to know what type of message this is.
		/// </summary>
		[JsonPropertyName("type")]
		public string MessageType { get => GetMessageType(); }
	}


}