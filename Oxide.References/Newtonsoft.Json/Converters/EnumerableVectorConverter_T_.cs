using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Newtonsoft.Json.Converters
{
	public class EnumerableVectorConverter<T> : JsonConverter
	{
		private readonly static Newtonsoft.Json.Converters.VectorConverter VectorConverter;

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		static EnumerableVectorConverter()
		{
			EnumerableVectorConverter<T>.VectorConverter = new Newtonsoft.Json.Converters.VectorConverter();
		}

		public EnumerableVectorConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			if (typeof(IEnumerable<Vector2>).IsAssignableFrom(objectType) || typeof(IEnumerable<Vector3>).IsAssignableFrom(objectType))
			{
				return true;
			}
			return typeof(IEnumerable<Vector4>).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			List<T> ts = new List<T>();
			JObject jObjects = JObject.Load(reader);
			for (int i = 0; i < jObjects.Count; i++)
			{
				ts.Add(JsonConvert.DeserializeObject<T>(jObjects[i].ToString()));
			}
			return ts;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			T[] array;
			if (value == null)
			{
				writer.WriteNull();
			}
			IEnumerable<T> ts = value as IEnumerable<T>;
			if (ts != null)
			{
				array = ts.ToArray<T>();
			}
			else
			{
				array = null;
			}
			T[] tArray = array;
			if (tArray == null)
			{
				writer.WriteNull();
				return;
			}
			writer.WriteStartArray();
			for (int i = 0; i < (int)tArray.Length; i++)
			{
				EnumerableVectorConverter<T>.VectorConverter.WriteJson(writer, tArray[i], serializer);
			}
			writer.WriteEndArray();
		}
	}
}