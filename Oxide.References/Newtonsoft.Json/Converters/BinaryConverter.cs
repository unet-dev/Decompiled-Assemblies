using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Newtonsoft.Json.Converters
{
	[Preserve]
	public class BinaryConverter : JsonConverter
	{
		private const string BinaryTypeName = "System.Data.Linq.Binary";

		private const string BinaryToArrayName = "ToArray";

		private ReflectionObject _reflectionObject;

		public BinaryConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			if (objectType.AssignableToTypeName("System.Data.Linq.Binary"))
			{
				return true;
			}
			return false;
		}

		private void EnsureReflectionObject(Type t)
		{
			if (this._reflectionObject == null)
			{
				this._reflectionObject = ReflectionObject.Create(t, t.GetConstructor(new Type[] { typeof(byte[]) }), new string[] { "ToArray" });
			}
		}

		private byte[] GetByteArray(object value)
		{
			if (!value.GetType().AssignableToTypeName("System.Data.Linq.Binary"))
			{
				throw new JsonSerializationException("Unexpected value type when writing binary: {0}".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
			}
			this.EnsureReflectionObject(value.GetType());
			return (byte[])this._reflectionObject.GetValue(value, "ToArray");
		}

		private byte[] ReadByteArray(JsonReader reader)
		{
			List<byte> nums = new List<byte>();
			while (reader.Read())
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType == JsonToken.Comment)
				{
					continue;
				}
				if (tokenType != JsonToken.Integer)
				{
					if (tokenType != JsonToken.EndArray)
					{
						throw JsonSerializationException.Create(reader, "Unexpected token when reading bytes: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
					}
					return nums.ToArray();
				}
				nums.Add(Convert.ToByte(reader.Value, CultureInfo.InvariantCulture));
			}
			throw JsonSerializationException.Create(reader, "Unexpected end when reading bytes.");
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			byte[] numArray;
			if (reader.TokenType == JsonToken.Null)
			{
				if (!ReflectionUtils.IsNullable(objectType))
				{
					throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
				}
				return null;
			}
			if (reader.TokenType != JsonToken.StartArray)
			{
				if (reader.TokenType != JsonToken.String)
				{
					throw JsonSerializationException.Create(reader, "Unexpected token parsing binary. Expected String or StartArray, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
				}
				numArray = Convert.FromBase64String(reader.Value.ToString());
			}
			else
			{
				numArray = this.ReadByteArray(reader);
			}
			Type type = (ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType);
			if (!type.AssignableToTypeName("System.Data.Linq.Binary"))
			{
				throw JsonSerializationException.Create(reader, "Unexpected object type when writing binary: {0}".FormatWith(CultureInfo.InvariantCulture, objectType));
			}
			this.EnsureReflectionObject(type);
			return this._reflectionObject.Creator(new object[] { numArray });
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			writer.WriteValue(this.GetByteArray(value));
		}
	}
}