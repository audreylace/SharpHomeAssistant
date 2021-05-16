using System.Text.Json;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Base class of all incoming message converters. Contains common functionlaity used by all of them.
	/// </summary>
	/// <typeparam name="Type">The type of the converted object.</typeparam>
	public abstract class IncomingMessageBaseConverter<Type> : IAlgebraicTypeConverter<IncomingMessageBase, string> where Type : IncomingMessageBase
	{
		/// <summary>
		/// Checks if this converter can convert the provided algebraic type.
		/// </summary>
		/// <param name="typeId">The value of the descriminator to check.</param>
		/// <returns>Returns true if and only if this converter can convert this type.</returns>
		public virtual bool CanConvert(string typeId)
		{
			return MessageType == typeId;
		}

		/// <summary>
		/// Returns the message type of this converter.
		/// </summary>
		protected virtual string MessageType => MessageTypeAttribute.GetMessageTypeString(typeof(Type));

		/// <summary>
		/// Helper function to write the type attribute of the message.
		/// </summary>
		/// <param name="writer">JSON writer that will be used to write the key/value pair.`</param>
		protected void WriteTypeAttribute(Utf8JsonWriter writer)
		{
			writer.WriteString(IncomingMessageBase.PropertyTypeJsonName, MessageType);
		}

		/// <summary>
		/// Converts a JSON object into its corresponding C# form.
		/// </summary>
		/// <param name="reader">The Json reader to use.</param>
		/// <param name="typeToConvert">The value of the descriminator type.</param>
		/// <param name="options">JSON converter options.</param>
		/// <returns>The converted C# object.</returns>
		public abstract IncomingMessageBase Read(ref Utf8JsonReader reader, string typeToConvert, JsonSerializerOptions options);

		/// <summary>
		/// Writes the JSON output of the C# object.
		/// </summary>
		/// <param name="writer">JSON writer used to create the JSON stream.</param>
		/// <param name="value">The C# object to convert.</param>
		/// <param name="typeToConvert">The descriminator type.</param>
		/// <param name="options">JSON converter options.</param>
		public abstract void Write(Utf8JsonWriter writer, IncomingMessageBase value, string typeToConvert, JsonSerializerOptions options);
	}

}