using System.Text.Json;

#nullable enable

namespace AudreysCloud.Community.SharpHomeAssistant.Utils
{
	/// <summary>
	/// A JSON converter that handles a specific type in a algebraic data type.
	/// 
	/// See https://en.wikipedia.org/wiki/Algebraic_data_type for more details.
	/// </summary>
	/// <typeparam name="ToConvertType">The type of the converted C# object.</typeparam>
	/// <typeparam name="DescriminatorType">The type of the C# value used to descriminate amongs various members of the composite type.</typeparam>
	/// 
	public interface IAlgebraicTypeConverter<ToConvertType, DescriminatorType>
	{
		/// <summary>
		/// Checks if this converter can convert this descriminator type.
		/// </summary>
		/// <param name="typeId">The type to check for conversion.</param>
		/// <returns>True if and only if the type can be converted.</returns>
		bool CanConvert(DescriminatorType typeId);

		/// <summary>
		/// Reads the supplied JSON and creates the corresponding C# object.
		/// </summary>
		/// <param name="reader">Reader used to read the JSON stream.</param>
		/// <param name="typeToConvert">The value of the descriminator for the type being converted.</param>
		/// <param name="options">The JSON serializer options in use.</param>
		/// <returns>The C# representation of the object.</returns>
		ToConvertType? Read(ref Utf8JsonReader reader, DescriminatorType typeToConvert, JsonSerializerOptions options);

		/// <summary>
		/// Converts the C# object back into its JSON form.
		/// </summary>
		/// <param name="writer">The JSON writer to use to serialize the object.</param>
		/// <param name="value">The object being converted.</param>
		/// <param name="typeToConvert">The descriminator value of this type encountered by the AlgebraicTypeConverter.</param>
		/// <param name="options">Serialization options.</param>
		void Write(Utf8JsonWriter writer, ToConvertType value, DescriminatorType typeToConvert, JsonSerializerOptions options);
	}
}
