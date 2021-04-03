using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public abstract class HomeAssistantMessage : IAlgebraicType<string>
	{
		public const string PropertyTypeJsonName = "type";
		public abstract string TypeId { get; }
	}
}