using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace Newtonsoft.Json.Converters
{
	public class ResolutionConverter : JsonConverter
	{
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public ResolutionConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Resolution);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jObjects = JObject.Load(reader);
			Resolution resolution = new Resolution()
			{
				height = (int)jObjects["height"],
				width = (int)jObjects["width"],
				refreshRate = (int)jObjects["refreshRate"]
			};
			return resolution;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			Resolution resolution = (Resolution)value;
			writer.WriteStartObject();
			writer.WritePropertyName("height");
			writer.WriteValue(resolution.height);
			writer.WritePropertyName("width");
			writer.WriteValue(resolution.width);
			writer.WritePropertyName("refreshRate");
			writer.WriteValue(resolution.refreshRate);
			writer.WriteEndObject();
		}
	}
}