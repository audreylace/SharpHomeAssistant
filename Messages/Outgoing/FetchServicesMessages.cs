namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// This will get a dump of the current services in Home Assistant.
	/// </summary>
	public class FetchServicesMessage : CommandMessageBase
	{
#pragma warning disable CS1591
		protected override string GetMessageType() => "get_services";
#pragma warning restore CS1591
	}
}