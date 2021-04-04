using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Utils
{
	public abstract class AlgebraicTypeConverter<TypeIdType> : JsonConverter<IAlgebraicType<TypeIdType>>
	{

		public AlgebraicTypeConverter()
		{
			Converters = new List<IAlgebraicTypeConverter<TypeIdType>>();
		}
		protected List<IAlgebraicTypeConverter<TypeIdType>> Converters { get; set; }

		protected abstract TypeIdType GetTypeId(ref Utf8JsonReader reader, JsonSerializerOptions options);
		public override bool CanConvert(Type typeToConvert)
		{
			return typeToConvert.IsAssignableTo(typeof(IAlgebraicType<TypeIdType>));
		}

		protected virtual IAlgebraicType<TypeIdType>? OnReadConverterNotFound(ref Utf8JsonReader reader, TypeIdType typeToConvert, JsonSerializerOptions options)
		{
			throw new JsonException();
		}

		protected virtual IAlgebraicType<TypeIdType>? OnWriteConverterNotFound(Utf8JsonWriter writer, IAlgebraicType<TypeIdType> value, JsonSerializerOptions options)
		{
			throw new JsonException();
		}
		public override IAlgebraicType<TypeIdType>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			Utf8JsonReader getTypeIdReader = reader;

			JsonSerializerOptions optionClone = new JsonSerializerOptions(options);
			optionClone.Converters.Remove(this);

			TypeIdType elementType = GetTypeId(ref getTypeIdReader, optionClone);

			int index = Converters.FindIndex((c) => c.CanConvert(elementType));

			if (index != -1)
			{
				return Converters[index].Read(ref reader, elementType, optionClone);
			}

			return OnReadConverterNotFound(ref reader, elementType, optionClone);

		}
		public override void Write(Utf8JsonWriter writer, IAlgebraicType<TypeIdType> value, JsonSerializerOptions options)
		{
			JsonSerializerOptions optionClone = new JsonSerializerOptions(options);
			optionClone.Converters.Remove(this);
			int index = Converters.FindIndex((c) => c.CanConvert(value.TypeId));

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
