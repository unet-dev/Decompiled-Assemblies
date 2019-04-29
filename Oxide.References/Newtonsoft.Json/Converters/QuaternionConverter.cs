using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Newtonsoft.Json.Converters
{
	public class QuaternionConverter : JsonConverter
	{
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public QuaternionConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Quaternion);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jObjects = JObject.Load(reader);
			List<JProperty> list = jObjects.Properties().ToList<JProperty>();
			Quaternion item = new Quaternion();
			if (list.Any<JProperty>((JProperty p) => p.Name == "w"))
			{
				item.w = (float)((float)jObjects["w"]);
			}
			if (list.Any<JProperty>((JProperty p) => p.Name == "x"))
			{
				item.x = (float)((float)jObjects["x"]);
			}
			if (list.Any<JProperty>((JProperty p) => p.Name == "y"))
			{
				item.y = (float)((float)jObjects["y"]);
			}
			if (list.Any<JProperty>((JProperty p) => p.Name == "z"))
			{
				item.z = (float)((float)jObjects["z"]);
			}
			if (list.Any<JProperty>((JProperty p) => p.Name == "eulerAngles"))
			{
				JToken jTokens = jObjects["eulerAngles"];
				Vector3 vector3 = new Vector3()
				{
					x = (float)((float)jTokens["x"]),
					y = (float)((float)jTokens["y"]),
					z = (float)((float)jTokens["z"])
				};
				item.eulerAngles = vector3;
			}
			return item;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			Quaternion quaternion = (Quaternion)value;
			writer.WriteStartObject();
			writer.WritePropertyName("w");
			writer.WriteValue(quaternion.w);
			writer.WritePropertyName("x");
			writer.WriteValue(quaternion.x);
			writer.WritePropertyName("y");
			writer.WriteValue(quaternion.y);
			writer.WritePropertyName("z");
			writer.WriteValue(quaternion.z);
			writer.WritePropertyName("eulerAngles");
			writer.WriteStartObject();
			writer.WritePropertyName("x");
			writer.WriteValue(quaternion.eulerAngles.x);
			writer.WritePropertyName("y");
			writer.WriteValue(quaternion.eulerAngles.y);
			writer.WritePropertyName("z");
			writer.WriteValue(quaternion.eulerAngles.z);
			writer.WriteEndObject();
			writer.WriteEndObject();
		}
	}
}