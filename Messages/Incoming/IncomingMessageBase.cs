using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public abstract class IncomingMessageBase : HomeAssistantMessage
	{
		public static bool TryConvert<TOutType>(IncomingMessageBase messageBase, out TOutType outMessage) where TOutType : IncomingMessageBase
		{
			outMessage = null;

			outMessage = messageBase as TOutType;
			if (outMessage != null)
			{
				return true;
			}

			return false;
		}
	}
}