using System;
using System.Collections.Generic;
using System.Text.Json;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	/// <summary>
	/// JSON converter used to convert incoming Home Assistant messages into their C# form.
	/// </summary>
	public class IncomingMessageConverter : AlgebraicTypeConverter<IncomingMessageBase, string>
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public IncomingMessageConverter() : base()
		{
			Converters.Add(new AuthRequiredMessageConverter());
			Converters.Add(new AuthResultMessageConverter());
			Converters.Add(new PongMessageConverter());
			Converters.Add(new ResultMessageConverter());
			Converters.Add(new EventMessageConverter());
		}


		/// <summary>
		/// The list of converters used to convert JSON messages between their C# form and JSON form.
		/// </summary>
		public List<IAlgebraicTypeConverter<IncomingMessageBase, string>> ConverterList
		{
			get
			{
				return Converters;
			}
		}

		/// <summary>
		/// If a message converter is not found, this method is used to convert the incoming message into its C# form.
		/// </summary>
		/// <param name="reader">JSON reader used to read the input JSON stream.</param>
		/// <param name="typeToConvert">Descriminator value of this type.</param>
		/// <param name="options">JSON convertion options.</param>
		/// <returns>A C# object of type UnknownMessage.</returns>
		/// <see cref="UnknownMessage" />
		protected override IncomingMessageBase OnReadConverterNotFound(ref Utf8JsonReader reader, string typeToConvert, JsonSerializerOptions options)
		{
			using (JsonDocument document = JsonDocument.ParseValue(ref reader))
			{
				UnknownMessage unknownMessage = new UnknownMessage();

				unknownMessage.Message = document.RootElement.Clone();
				unknownMessage.UnknownMessageType = typeToConvert;

				return unknownMessage;
			}
		}

		/// <summary>
		/// Reads the input JSON to get the descriminator value of the type.
		/// </summary>
		/// <param name="reader">Reader being used to read the input JSON</param>
		/// <param name="options">JSON conversion options that are active.</param>
		/// <returns></returns>
		protected override string GetDiscriminatorTypeFromJson(ref Utf8JsonReader reader, JsonSerializerOptions options)
		{
			using (JsonDocument document = JsonDocument.ParseValue(ref reader))
			{
				JsonElement root = document.RootElement;

				if (root.TryGetProperty(IncomingMessageBase.PropertyTypeJsonName, out JsonElement typeElement))
				{
					return typeElement.GetString();
				}
				else
				{
					throw new JsonException();
				}
			}
		}

		/// <summary>
		/// Given a type that derives from IncomingMessageBase, this method will return that types descriminator value.
		/// </summary>
		/// <param name="message">The C# object to extract the descriminator value from.</param>
		/// <returns>Descriminator value as a string.</returns>
		protected override string GetDiscriminatorTypeFromValue(IncomingMessageBase message)
		{
			return MessageTypeAttribute.GetMessageTypeString(message.GetType());
		}

		/// <summary>
		/// Writer to handle converting a object with not corresponding handler. It is not implemented in this class.
		/// </summary>
		/// <exception cref="NotImplementedException">Thrown when this method is invoked.</exception>
		/// <param name="writer"></param>
		/// <param name="value"></param>
		/// <param name="typeValue"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		protected override void OnWriteConverterNotFound(Utf8JsonWriter writer, IncomingMessageBase value, string typeValue, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}
	}
}