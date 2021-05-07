namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{

	internal class MessageTypeAttribute : System.Attribute
	{
		public MessageTypeAttribute(string messageType)
		{
			MessageType = messageType;
		}
		public string MessageType { get; set; }
	}

}