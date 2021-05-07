namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{

	public class MessageTypeAttribute : System.Attribute
	{
		public MessageTypeAttribute(string messageType)
		{
			MessageType = messageType;
		}
		public string MessageType { get; set; }
	}

}