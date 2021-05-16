using System.Text.Json;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// Converter for a message of unknown type.
	/// </summary>
	public class UnknownMessageConverter : IncomingMessageBaseConverter<UnknownMessage>
	{
		/// <summary>
		/// Reads the message from the JSON stream.
		/// </summary>
		/// <param name="reader">JSON stream reader.</param>
		/// <param name="typeToConvert">Value of the descriminator field.</param>
		/// <param name="options">JSON conversion options.</param>
		/// <returns>The parsed message stored in UnknownMessage C# type.</returns>
		public override IncomingMessageBase Read(ref Utf8JsonReader reader, string typeToConvert, JsonSerializerOptions options)
		{
			UnknownMessage message = new UnknownMessage() { UnknownMessageType = typeToConvert };
			using (JsonDocument document = JsonDocument.ParseValue(ref reader))
			{
				message.Message = document.RootElement.Clone();
			}

			return message;
		}

		/// <summary>
		/// Writes the unknown message type to a JSON stream.
		/// </summary>
		/// <param name="writer">The JSON stream writer.</param>
		/// <param name="value">The message to convert.</param>
		/// <param name="typeToConvert">Value of the descriminator union.</param>
		/// <param name="options">Json conversion options.</param>
		public override void Write(Utf8JsonWriter writer, IncomingMessageBase value, string typeToConvert, JsonSerializerOptions options)
		{

			UnknownMessage message = (UnknownMessage)value;
			JsonSerializer.Serialize(writer, message.Message, options);
		}
	}
}