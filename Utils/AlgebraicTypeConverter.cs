using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace AudreysCloud.Community.SharpHomeAssistant.Utils
{
	/// <summary>
	/// JSON converter class used to convert algebraic data type.
	/// </summary>
	/// <typeparam name="TypeToConvert">The C# type to convert.</typeparam>
	/// <typeparam name="DiscriminatorType">The C# type of the descriminator value.</typeparam>
	public abstract class AlgebraicTypeConverter<TypeToConvert, DiscriminatorType> : JsonConverter<TypeToConvert>
	{

		/// <summary>
		/// List of converters used to handle various members of this data union.
		/// </summary>
		protected List<IAlgebraicTypeConverter<TypeToConvert, DiscriminatorType>> Converters { get; set; }

		/// <summary>
		/// Reads the input JSON and returns the value of the descriminator field.
		/// </summary>
		/// <param name="reader">JSON reader used to read the JSON input stream.</param>
		/// <param name="options">JSON converter options.</param>
		/// <returns>The value of the descriminator field.</returns>
		protected abstract DiscriminatorType GetDiscriminatorTypeFromJson(ref Utf8JsonReader reader, JsonSerializerOptions options);

		/// <summary>
		/// Given a C# object in the union, returns the descriminator value of that object.
		/// </summary>
		/// <param name="value">The C# object to extract the descriminator value from.</param>
		/// <returns>The value of the field.</returns>
		protected abstract DiscriminatorType GetDiscriminatorTypeFromValue(TypeToConvert value);

		/// <summary>
		/// Default Constructor
		/// </summary>
		public AlgebraicTypeConverter()
		{
			Converters = new List<IAlgebraicTypeConverter<TypeToConvert, DiscriminatorType>>();
		}

		/// <summary>
		/// Use by the JSON converter infrastructure to see if this class can handle the given type.
		/// </summary>
		/// <param name="t">The type to check.</param>
		/// <returns>True if this class can convert it.</returns>
		public override bool CanConvert(Type t)
		{
			return t.IsAssignableTo(typeof(TypeToConvert));
		}

		/// <summary>
		/// Handles the case when a valid converter is not found in the converter list as part of a read operation.
		/// </summary>
		/// <param name="reader">JSON stream reader.</param>
		/// <param name="discriminatorType">The value of the descriminator field.</param>
		/// <param name="options">JSON converter options.</param>
		/// <returns>The C# representation of the converted object.</returns>
		protected abstract TypeToConvert? OnReadConverterNotFound(ref Utf8JsonReader reader, DiscriminatorType discriminatorType, JsonSerializerOptions options);

		/// <summary>
		/// Handles the case when a valid converter is not found in the converter list as part of a write operation.
		/// </summary>
		/// <param name="writer">Json stream writer.</param>
		/// <param name="value">The C# object to convert to JSON.</param>
		/// <param name="typeValue">The value of the descriminator field.</param>
		/// <param name="options">JSON convertion options.</param>
		protected abstract void OnWriteConverterNotFound(Utf8JsonWriter writer, TypeToConvert value, DiscriminatorType typeValue, JsonSerializerOptions options);

		/// <summary>
		/// Read method invoked by the json converter. Creates the C# object from the input JSON.
		/// </summary>
		/// <param name="reader">The JSON reader being used to consume the input stream.</param>
		/// <param name="typeToConvert">The converted C# object.</param>
		/// <param name="options">The converter options in use.</param>
		/// <returns>C# representation constructed from the input JSON.</returns>
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

		/// <summary>
		/// Write method invoked by the JSON converter. Creates JSON from the input object.
		/// </summary>
		/// <param name="writer">JSON writer being used to create the output JSON.</param>
		/// <param name="value">The input object to convert.</param>
		/// <param name="options">The convertion options in use.</param>
		public override void Write(Utf8JsonWriter writer, TypeToConvert value, JsonSerializerOptions options)
		{
			JsonSerializerOptions optionClone = new JsonSerializerOptions(options);
			optionClone.Converters.Remove(this);
			DiscriminatorType typeName = GetDiscriminatorTypeFromValue(value);
			int index = Converters.FindIndex((c) => c.CanConvert(typeName));

			if (index == -1)
			{

				OnWriteConverterNotFound(writer, value, typeName, optionClone);
			}
			else
			{
				Converters[index].Write(writer, value, typeName, optionClone);
			}
		}

	}
}
