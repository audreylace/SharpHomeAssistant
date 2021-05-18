namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// This will get a dump of the current config in Home Assistant.
	/// </summary>
	public class FetchConfigMessage : CommandMessageBase
	{
#pragma warning disable CS1591
		protected override string GetMessageType() => "get_config";
#pragma warning restore CS1591
	}
}