
namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	internal class PingMessage : CommandMessageBase
	{
		public const string MessageType = "ping";
		public override string GetTypeId()
		{
			return MessageType;
		}
	}
}