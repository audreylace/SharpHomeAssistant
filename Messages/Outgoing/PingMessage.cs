
namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Connection ping message. Used to keep the websocket connection open in periods of low activity. Note, you must manualy send it. The connection will not do it for you automatically.
	/// </summary>
	public class PingMessage : CommandMessageBase
	{
#pragma warning disable CS1591
		protected override string GetMessageType() => "ping";
#pragma warning restore CS1591

	}
}