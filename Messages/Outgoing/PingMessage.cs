
namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Connection ping message. Used to keep the websocket connection open
	/// in periods of low activity.
	/// </summary>
	public class PingMessage : CommandMessageBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns>A string with a value of "ping."</returns>
		protected override string GetMessageType() => "ping";

	}
}