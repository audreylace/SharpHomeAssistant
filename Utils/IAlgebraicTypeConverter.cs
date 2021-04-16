using System.Text.Json;

#nullable enable

namespace AudreysCloud.Community.SharpHomeAssistant.Utils
{
	public interface IAlgebraicTypeConverter<TypeIdType>
	{
		bool CanConvert(TypeIdType typeId);
		IAlgebraicType<TypeIdType>? Read(ref Utf8JsonReader reader, TypeIdType typeToConvert, JsonSerializerOptions options);
		void Write(Utf8JsonWriter writer, IAlgebraicType<TypeIdType> value, JsonSerializerOptions options);

	}
}
