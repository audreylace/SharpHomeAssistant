using System.Text.Json;

namespace AudreysCloud.Community.SharpHomeAssistant.Utils
{
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
}
