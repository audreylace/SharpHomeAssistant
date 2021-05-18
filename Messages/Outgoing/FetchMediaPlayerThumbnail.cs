using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Fetch a base64 encoded thumbnail picture for a media player.
	/// </summary>
	public class FetchMediaPlayerThumbnailMessage : CommandMessageBase
	{
#pragma warning disable CS1591
		protected override string GetMessageType() => "media_player_thumbnail";
#pragma warning restore CS1591

		/// <summary>
		/// The media player to fetch the thumbnail from.
		/// </summary>
		[JsonPropertyName("entity_id")]
		public string EntityId { get; set; }
	}
}