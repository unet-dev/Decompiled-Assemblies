using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace Newtonsoft.Json.Converters
{
	public class ColorConverter : JsonConverter
	{
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public ColorConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			if (objectType == typeof(Color))
			{
				return true;
			}
			return objectType == typeof(Color32);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return new Color();
			}
			JObject jObjects = JObject.Load(reader);
			if (objectType == typeof(Color32))
			{
				return new Color32((byte)jObjects["r"], (byte)jObjects["g"], (byte)jObjects["b"], (byte)jObjects["a"]);
			}
			return new Color((float)((float)jObjects["r"]), (float)((float)jObjects["g"]), (float)((float)jObjects["b"]), (float)((float)jObjects["a"]));
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			Color color = (Color)value;
			writer.WriteStartObject();
			writer.WritePropertyName("a");
			writer.WriteValue(color.a);
			writer.WritePropertyName("r");
			writer.WriteValue(color.r);
			writer.WritePropertyName("g");
			writer.WriteValue(color.g);
			writer.WritePropertyName("b");
			writer.WriteValue(color.b);
			writer.WriteEndObject();
		}
	}
}