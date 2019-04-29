using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Converters
{
	[Preserve]
	public class KeyValuePairConverter : JsonConverter
	{
		private const string KeyName = "Key";

		private const string ValueName = "Value";

		private readonly static ThreadSafeStore<Type, ReflectionObject> ReflectionObjectPerType;

		static KeyValuePairConverter()
		{
			KeyValuePairConverter.ReflectionObjectPerType = new ThreadSafeStore<Type, ReflectionObject>(new Func<Type, ReflectionObject>(KeyValuePairConverter.InitializeReflectionObject));
		}

		public KeyValuePairConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			Type type = (ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType);
			if (!type.IsValueType() || !type.IsGenericType())
			{
				return false;
			}
			return type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
		}

		private static ReflectionObject InitializeReflectionObject(Type t)
		{
			Type[] genericArguments = t.GetGenericArguments();
			Type item = ((IList<Type>)genericArguments)[0];
			Type type = ((IList<Type>)genericArguments)[1];
			return ReflectionObject.Create(t, t.GetConstructor(new Type[] { item, type }), new string[] { "Key", "Value" });
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				if (!ReflectionUtils.IsNullableType(objectType))
				{
					throw JsonSerializationException.Create(reader, "Cannot convert null value to KeyValuePair.");
				}
				return null;
			}
			object obj = null;
			object obj1 = null;
			reader.ReadAndAssert();
			ReflectionObject reflectionObject = KeyValuePairConverter.ReflectionObjectPerType.Get((ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType));
			while (reader.TokenType == JsonToken.PropertyName)
			{
				string str = reader.Value.ToString();
				if (string.Equals(str, "Key", StringComparison.OrdinalIgnoreCase))
				{
					reader.ReadAndAssert();
					obj = serializer.Deserialize(reader, reflectionObject.GetType("Key"));
				}
				else if (!string.Equals(str, "Value", StringComparison.OrdinalIgnoreCase))
				{
					reader.Skip();
				}
				else
				{
					reader.ReadAndAssert();
					obj1 = serializer.Deserialize(reader, reflectionObject.GetType("Value"));
				}
				reader.ReadAndAssert();
			}
			return reflectionObject.Creator(new object[] { obj, obj1 });
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			ReflectionObject reflectionObject = KeyValuePairConverter.ReflectionObjectPerType.Get(value.GetType());
			DefaultContractResolver contractResolver = serializer.ContractResolver as DefaultContractResolver;
			writer.WriteStartObject();
			writer.WritePropertyName((contractResolver != null ? contractResolver.GetResolvedPropertyName("Key") : "Key"));
			serializer.Serialize(writer, reflectionObject.GetValue(value, "Key"), reflectionObject.GetType("Key"));
			writer.WritePropertyName((contractResolver != null ? contractResolver.GetResolvedPropertyName("Value") : "Value"));
			serializer.Serialize(writer, reflectionObject.GetValue(value, "Value"), reflectionObject.GetType("Value"));
			writer.WriteEndObject();
		}
	}
}