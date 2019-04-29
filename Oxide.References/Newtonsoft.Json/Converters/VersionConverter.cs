using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;

namespace Newtonsoft.Json.Converters
{
	[Preserve]
	public class VersionConverter : JsonConverter
	{
		public VersionConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Version);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			object version;
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			if (reader.TokenType != JsonToken.String)
			{
				throw JsonSerializationException.Create(reader, "Unexpected token or value when parsing version. Token: {0}, Value: {1}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType, reader.Value));
			}
			try
			{
				version = new Version((string)reader.Value);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				throw JsonSerializationException.Create(reader, "Error parsing version string: {0}".FormatWith(CultureInfo.InvariantCulture, reader.Value), exception);
			}
			return version;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			if (!(value is Version))
			{
				throw new JsonSerializationException("Expected Version object value");
			}
			writer.WriteValue(value.ToString());
		}
	}
}