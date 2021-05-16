
namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Connection ping message. Used to keep the websocket connection open in periods of low activity. Note, you must manualy send it. The connection will not do it for you automatically.
	/// </summary>
	[MessageType("ping")]
	public class PingMessage : CommandMessageBase
	{
	}
}