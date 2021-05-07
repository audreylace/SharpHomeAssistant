using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace AudreysCloud.Community.SharpHomeAssistant.Utils
{
	public abstract class AlgebraicTypeConverter<TypeToConvert, DiscriminatorType> : JsonConverter<TypeToConvert>
	{

		protected List<IAlgebraicTypeConverter<TypeToConvert, DiscriminatorType>> Converters { get; set; }
		protected abstract DiscriminatorType GetDiscriminatorTypeFromJson(ref Utf8JsonReader reader, JsonSerializerOptions options);
		protected abstract DiscriminatorType GetDiscriminatorTypeFromValue(TypeToConvert value);

		public AlgebraicTypeConverter()
		{
			Converters = new List<IAlgebraicTypeConverter<TypeToConvert, DiscriminatorType>>();
		}

		public override bool CanConvert(Type t)
		{
			return t.IsAssignableTo(typeof(TypeToConvert));
		}

		protected virtual TypeToConvert? OnReadConverterNotFound(ref Utf8JsonReader reader, DiscriminatorType discriminatorType, JsonSerializerOptions options)
		{
			throw new JsonException();
		}

		protected virtual TypeToConvert? OnWriteConverterNotFound(Utf8JsonWriter writer, TypeToConvert value, JsonSerializerOptions options)
		{
			throw new JsonException();
		}
		public override TypeToConvert? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			Utf8JsonReader getTypeIdReader = reader;

			JsonSerializerOptions optionClone = new JsonSerializerOptions(options);
			optionClone.Converters.Remove(this);

			DiscriminatorType elementType = GetDiscriminatorTypeFromJson(ref getTypeIdReader, optionClone);

			int index = Converters.FindIndex((c) => c.CanConvert(elementType));

			if (index != -1)
			{
				return Converters[index].Read(ref reader, elementType, optionClone);
			}

			return OnReadConverterNotFound(ref reader, elementType, optionClone);

		}
		public override void Write(Utf8JsonWriter writer, TypeToConvert value, JsonSerializerOptions options)
		{
			JsonSerializerOptions optionClone = new JsonSerializerOptions(options);
			optionClone.Converters.Remove(this);
			int index = Converters.FindIndex((c) => c.CanConvert(GetDiscriminatorTypeFromValue(value)));

			if (index == -1)
			{

				OnWriteConverterNotFound(writer, value, optionClone);
			}
			else
			{
				Converters[index].Write(writer, value, optionClone);
			}
		}

	}
}
