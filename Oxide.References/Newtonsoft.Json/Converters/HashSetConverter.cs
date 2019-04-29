using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Newtonsoft.Json.Converters
{
	public class HashSetConverter : JsonConverter
	{
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public HashSetConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			if (!objectType.IsGenericType())
			{
				return false;
			}
			return objectType.GetGenericTypeDefinition() == typeof(HashSet<>);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			bool objectCreationHandling = serializer.ObjectCreationHandling == ObjectCreationHandling.Replace;
			if (reader.TokenType == JsonToken.Null)
			{
				if (!objectCreationHandling)
				{
					return existingValue;
				}
				return null;
			}
			object obj = (objectCreationHandling || existingValue == null ? Activator.CreateInstance(objectType) : existingValue);
			Type genericArguments = objectType.GetGenericArguments()[0];
			MethodInfo method = objectType.GetMethod("Add");
			JArray jArrays = JArray.Load(reader);
			for (int i = 0; i < jArrays.Count; i++)
			{
				object obj1 = serializer.Deserialize(jArrays[i].CreateReader(), genericArguments);
				method.Invoke(obj, new object[] { obj1 });
			}
			return obj;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
		}
	}
}