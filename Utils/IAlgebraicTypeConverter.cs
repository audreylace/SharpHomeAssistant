using System.Text.Json;

#nullable enable

namespace AudreysCloud.Community.SharpHomeAssistant.Utils
{
	internal interface IAlgebraicTypeConverter<ToConvertType, DescriminatorType>
	{
		bool CanConvert(DescriminatorType typeId);
		ToConvertType? Read(ref Utf8JsonReader reader, DescriminatorType typeToConvert, JsonSerializerOptions options);
		void Write(Utf8JsonWriter writer, ToConvertType value, JsonSerializerOptions options);

	}
}
