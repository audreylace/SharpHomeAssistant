namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// This will get a dump of the current registered panels in Home Assistant.
	/// </summary>
	public class FetchPanelsMessage : CommandMessageBase
	{
#pragma warning disable CS1591
		protected override string GetMessageType() => "get_panels";
#pragma warning restore CS1591
	}
}