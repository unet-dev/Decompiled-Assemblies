using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Converters
{
	[Preserve]
	public class StringEnumConverter : JsonConverter
	{
		public bool AllowIntegerValues
		{
			get;
			set;
		}

		public bool CamelCaseText
		{
			get;
			set;
		}

		public StringEnumConverter()
		{
			this.AllowIntegerValues = true;
		}

		public StringEnumConverter(bool camelCaseText) : this()
		{
			this.CamelCaseText = camelCaseText;
		}

		public override bool CanConvert(Type objectType)
		{
			return ((ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType)).IsEnum();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			object obj;
			if (reader.TokenType == JsonToken.Null)
			{
				if (!ReflectionUtils.IsNullableType(objectType))
				{
					throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
				}
				return null;
			}
			bool flag = ReflectionUtils.IsNullableType(objectType);
			Type type = (flag ? Nullable.GetUnderlyingType(objectType) : objectType);
			try
			{
				if (reader.TokenType == JsonToken.String)
				{
					obj = EnumUtils.ParseEnumName(reader.Value.ToString(), flag, type);
				}
				else if (reader.TokenType != JsonToken.Integer)
				{
					throw JsonSerializationException.Create(reader, "Unexpected token {0} when parsing enum.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
				}
				else
				{
					if (!this.AllowIntegerValues)
					{
						throw JsonSerializationException.Create(reader, "Integer value {0} is not allowed.".FormatWith(CultureInfo.InvariantCulture, reader.Value));
					}
					obj = ConvertUtils.ConvertOrCast(reader.Value, CultureInfo.InvariantCulture, type);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				throw JsonSerializationException.Create(reader, "Error converting value {0} to type '{1}'.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.FormatValueForPrint(reader.Value), objectType), exception);
			}
			return obj;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			Enum @enum = (Enum)value;
			string str = @enum.ToString("G");
			if (char.IsNumber(str[0]) || str[0] == '-')
			{
				writer.WriteValue(value);
				return;
			}
			string enumName = EnumUtils.ToEnumName(@enum.GetType(), str, this.CamelCaseText);
			writer.WriteValue(enumName);
		}
	}
}