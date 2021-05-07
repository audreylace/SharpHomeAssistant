
namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class PingMessage : CommandMessageBase
	{
		public override string GetMessageType() => "ping";

	}
}