using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Converters
{
	[Preserve]
	public abstract class CustomCreationConverter<T> : JsonConverter
	{
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		protected CustomCreationConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(T).IsAssignableFrom(objectType);
		}

		public abstract T Create(Type objectType);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			T t = this.Create(objectType);
			if (t == null)
			{
				throw new JsonSerializationException("No object created.");
			}
			serializer.Populate(reader, t);
			return t;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotSupportedException("CustomCreationConverter should only be used while deserializing.");
		}
	}
}