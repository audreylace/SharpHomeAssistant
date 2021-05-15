
namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class PingMessage : CommandMessageBase
	{
		protected override string GetMessageType() => "ping";

	}
}