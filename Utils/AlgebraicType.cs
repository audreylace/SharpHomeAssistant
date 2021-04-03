using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpHomeAssistant.Utils
{
	public interface IAlgebraicType<TypeIdType>
	{
		TypeIdType TypeId { get; }
	}

	public interface IAlgebraicTypeConverter<TypeIdType>
	{
		bool CanConvert(TypeIdType typeId);
		IAlgebraicType<TypeIdType>? Read(ref Utf8JsonReader reader, TypeIdType typeToConvert, JsonSerializerOptions options);
		void Write(Utf8JsonWriter writer, IAlgebraicType<TypeIdType> value, JsonSerializerOptions options);

	}

	public class UnknownType<TypeIdType> : IAlgebraicType<TypeIdType>
	{
		public UnknownType(TypeIdType type)
		{
			TypeId = type;
		}

		public UnknownType(TypeIdType type, ref Utf8JsonReader reader, JsonSerializerOptions options) : this(type)
		{
			Value = (JsonElement)JsonSerializer.Deserialize<object>(ref reader, options);
		}
		public UnknownType(TypeIdType type, JsonElement element) : this(type)
		{
			Value = element.Clone();
		}

		public TypeIdType TypeId { get; set; }

		public JsonElement Value { get; set; }

	}

	public abstract class AlgebraicTypeConverter<TypeIdType> : JsonConverter<IAlgebraicType<TypeIdType>>
	{
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

			TypeIdType elementType = GetTypeId(ref getTypeIdReader, options);

			int index = Converters.FindIndex((c) => c.CanConvert(elementType));

			if (index != -1)
			{
				return Converters[index].Read(ref reader, elementType, options);
			}

			return OnReadConverterNotFound(ref reader, elementType, options);

		}
		public override void Write(Utf8JsonWriter writer, IAlgebraicType<TypeIdType> value, JsonSerializerOptions options)
		{
			int index = Converters.FindIndex((c) => c.CanConvert(value.TypeId));

			if (index == -1)
			{

				OnWriteConverterNotFound(writer, value, options);
			}
			else
			{
				Converters[index].Write(writer, value, options);
			}
		}

	}
}
