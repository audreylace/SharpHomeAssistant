using System.Text.Json;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="Type"></typeparam>
	public abstract class IncomingMessageBaseConverter<Type> : IAlgebraicTypeConverter<IncomingMessageBase, string> where Type : IncomingMessageBase
	{
		public virtual bool CanConvert(string typeId)
		{
			return MessageType == typeId;
		}
		protected string MessageType => MessageTypeAttribute.GetMessageTypeString(typeof(Type));

		protected void WriteTypeAttribute(Utf8JsonWriter writer)
		{
			writer.WriteString(IncomingMessageBase.PropertyTypeJsonName, MessageType);
		}
		public abstract IncomingMessageBase Read(ref Utf8JsonReader reader, string typeToConvert, JsonSerializerOptions options);
		public abstract void Write(Utf8JsonWriter writer, IncomingMessageBase value, string typeToConvert, JsonSerializerOptions options);
	}

}