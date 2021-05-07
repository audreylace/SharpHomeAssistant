using System.Text.Json;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	internal abstract class IncomingMessageBaseConverter<Type> : IAlgebraicTypeConverter<IncomingMessageBase, string> where Type : IncomingMessageBase
	{
		public virtual bool CanConvert(string typeId)
		{
			return MessageType == typeId;
		}
		protected string MessageType => IncomingMessageBase.GetMessageTypeString(typeof(Type));

		protected void WriteTypeAttribute(Utf8JsonWriter writer)
		{
			writer.WriteString(IncomingMessageBase.PropertyTypeJsonName, MessageType);
		}
		public abstract IncomingMessageBase Read(ref Utf8JsonReader reader, string typeToConvert, JsonSerializerOptions options);
		public abstract void Write(Utf8JsonWriter writer, IncomingMessageBase value, JsonSerializerOptions options);
	}

}