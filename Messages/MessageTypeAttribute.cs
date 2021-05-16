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
	}

}