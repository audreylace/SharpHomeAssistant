using System;
using System.Text.Json;
using AudreysCloud.Community.SharpHomeAssistant.Utils;

namespace AudreysCloud.Community.SharpHomeAssistant.Messages
{
	public class IncomingMessageConverter : AlgebraicTypeConverter<string>
	{
		public override bool CanConvert(Type typeToConvert)
		{
			return typeToConvert.IsAssignableTo(typeof(IncomingMessageBase));
		}
		public IncomingMessageConverter() : base()
		{
			Converters.Add(new AuthRequiredMessageConverter());
			Converters.Add(new AuthResultMessageConverter());
			Converters.Add(new PongMessageConverter());
			Converters.Add(new ResultMessageConverter());
			Converters.Add(new EventMessageConverter());
		}
		protected override string GetTypeId(ref Utf8JsonReader reader, JsonSerializerOptions options)
		{
			using (JsonDocument document = JsonDocument.ParseValue(ref reader))
			{
				JsonElement root = document.RootElement;

				if (root.TryGetProperty("type", out JsonElement typeElement))
				{
					return typeElement.GetString();
				}
				else
				{
					throw new JsonException();
				}
			}
		}

		protected override IAlgebraicType<string> OnReadConverterNotFound(ref Utf8JsonReader reader, string typeToConvert, JsonSerializerOptions options)
		{
			using (JsonDocument document = JsonDocument.ParseValue(ref reader))
			{
				UnknownMessage unknownMessage = new UnknownMessage();

				unknownMessage.Message = document.RootElement.Clone();
				unknownMessage.UnknownMessageType = typeToConvert;

				return unknownMessage;
			}
		}
	}
}