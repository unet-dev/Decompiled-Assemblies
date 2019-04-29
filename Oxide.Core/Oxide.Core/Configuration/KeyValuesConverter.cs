using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Configuration
{
	public class KeyValuesConverter : JsonConverter
	{
		public KeyValuesConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			if (objectType == typeof(Dictionary<string, object>))
			{
				return true;
			}
			return objectType == typeof(List<object>);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			int num;
			int num1;
			if (objectType != typeof(Dictionary<string, object>))
			{
				if (objectType != typeof(List<object>))
				{
					return existingValue;
				}
				List<object> objs = existingValue as List<object> ?? new List<object>();
				while (reader.Read() && reader.TokenType != JsonToken.EndArray)
				{
					switch (reader.TokenType)
					{
						case JsonToken.StartObject:
						{
							objs.Add(serializer.Deserialize<Dictionary<string, object>>(reader));
							continue;
						}
						case JsonToken.StartArray:
						{
							objs.Add(serializer.Deserialize<List<object>>(reader));
							continue;
						}
						case JsonToken.StartConstructor:
						case JsonToken.PropertyName:
						case JsonToken.Comment:
						case JsonToken.Raw:
						case JsonToken.Undefined:
						case JsonToken.EndObject:
						case JsonToken.EndArray:
						case JsonToken.EndConstructor:
						{
							this.Throw(string.Concat("Unexpected token: ", reader.TokenType));
							continue;
						}
						case JsonToken.Integer:
						{
							string str = reader.Value.ToString();
							if (!int.TryParse(str, out num1))
							{
								objs.Add(str);
								continue;
							}
							else
							{
								objs.Add(num1);
								continue;
							}
						}
						case JsonToken.Float:
						case JsonToken.String:
						case JsonToken.Boolean:
						case JsonToken.Null:
						case JsonToken.Date:
						case JsonToken.Bytes:
						{
							objs.Add(reader.Value);
							continue;
						}
						default:
						{
							goto case JsonToken.EndConstructor;
						}
					}
				}
				return objs;
			}
			Dictionary<string, object> value = existingValue as Dictionary<string, object> ?? new Dictionary<string, object>();
			if (reader.TokenType == JsonToken.StartArray)
			{
				return value;
			}
			while (reader.Read() && reader.TokenType != JsonToken.EndObject)
			{
				if (reader.TokenType != JsonToken.PropertyName)
				{
					this.Throw(string.Concat("Unexpected token: ", reader.TokenType));
				}
				string value1 = reader.Value as string;
				if (!reader.Read())
				{
					this.Throw("Unexpected end of json");
				}
				switch (reader.TokenType)
				{
					case JsonToken.StartObject:
					{
						value[value1] = serializer.Deserialize<Dictionary<string, object>>(reader);
						continue;
					}
					case JsonToken.StartArray:
					{
						value[value1] = serializer.Deserialize<List<object>>(reader);
						continue;
					}
					case JsonToken.StartConstructor:
					case JsonToken.PropertyName:
					case JsonToken.Comment:
					case JsonToken.Raw:
					case JsonToken.Undefined:
					case JsonToken.EndObject:
					case JsonToken.EndArray:
					case JsonToken.EndConstructor:
					{
						this.Throw(string.Concat("Unexpected token: ", reader.TokenType));
						continue;
					}
					case JsonToken.Integer:
					{
						string str1 = reader.Value.ToString();
						if (!int.TryParse(str1, out num))
						{
							value[value1] = str1;
							continue;
						}
						else
						{
							value[value1] = num;
							continue;
						}
					}
					case JsonToken.Float:
					case JsonToken.String:
					case JsonToken.Boolean:
					case JsonToken.Null:
					case JsonToken.Date:
					case JsonToken.Bytes:
					{
						value[value1] = reader.Value;
						continue;
					}
					default:
					{
						goto case JsonToken.EndConstructor;
					}
				}
			}
			return value;
		}

		private void Throw(string message)
		{
			throw new Exception(message);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (!(value is Dictionary<string, object>))
			{
				if (value is List<object>)
				{
					List<object> objs = (List<object>)value;
					writer.WriteStartArray();
					foreach (object obj in objs)
					{
						serializer.Serialize(writer, obj);
					}
					writer.WriteEndArray();
				}
				return;
			}
			Dictionary<string, object> strs = (Dictionary<string, object>)value;
			writer.WriteStartObject();
			foreach (KeyValuePair<string, object> keyValuePair in 
				from i in strs
				orderby i.Key
				select i)
			{
				writer.WritePropertyName(keyValuePair.Key, true);
				serializer.Serialize(writer, keyValuePair.Value);
			}
			writer.WriteEndObject();
		}
	}
}