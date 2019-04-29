using Newtonsoft.Json;
using System;

namespace Newtonsoft.Json.Converters
{
	public class UriConverter : JsonConverter
	{
		public UriConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Uri);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JsonToken tokenType = reader.TokenType;
			if (tokenType == JsonToken.String)
			{
				return new Uri((string)reader.Value);
			}
			if (tokenType != JsonToken.Null)
			{
				throw new InvalidOperationException("Unhandled case for UriConverter. Check to see if this converter has been applied to the wrong serialization type.");
			}
			return null;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			Uri uri = value as Uri;
			if (uri == null)
			{
				throw new InvalidOperationException("Unhandled case for UriConverter. Check to see if this converter has been applied to the wrong serialization type.");
			}
			writer.WriteValue(uri.OriginalString);
		}
	}
}