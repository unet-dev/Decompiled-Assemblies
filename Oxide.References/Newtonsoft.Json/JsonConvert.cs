using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Newtonsoft.Json
{
	[Preserve]
	public static class JsonConvert
	{
		public readonly static string True;

		public readonly static string False;

		public readonly static string Null;

		public readonly static string Undefined;

		public readonly static string PositiveInfinity;

		public readonly static string NegativeInfinity;

		public readonly static string NaN;

		private readonly static JsonSerializerSettings InitialSerializerSettings;

		public static Func<JsonSerializerSettings> DefaultSettings
		{
			get;
			set;
		}

		static JsonConvert()
		{
			JsonConvert.True = "true";
			JsonConvert.False = "false";
			JsonConvert.Null = "null";
			JsonConvert.Undefined = "undefined";
			JsonConvert.PositiveInfinity = "Infinity";
			JsonConvert.NegativeInfinity = "-Infinity";
			JsonConvert.NaN = "NaN";
			JsonConvert.InitialSerializerSettings = new JsonSerializerSettings();
			JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(JsonConvert.GetDefaultSettings);
		}

		public static T DeserializeAnonymousType<T>(string value, T anonymousTypeObject)
		{
			return JsonConvert.DeserializeObject<T>(value);
		}

		public static T DeserializeAnonymousType<T>(string value, T anonymousTypeObject, JsonSerializerSettings settings)
		{
			return JsonConvert.DeserializeObject<T>(value, settings);
		}

		public static object DeserializeObject(string value)
		{
			return JsonConvert.DeserializeObject(value, null, (JsonSerializerSettings)null);
		}

		public static object DeserializeObject(string value, JsonSerializerSettings settings)
		{
			return JsonConvert.DeserializeObject(value, null, settings);
		}

		public static object DeserializeObject(string value, Type type)
		{
			return JsonConvert.DeserializeObject(value, type, (JsonSerializerSettings)null);
		}

		public static T DeserializeObject<T>(string value)
		{
			return JsonConvert.DeserializeObject<T>(value, (JsonSerializerSettings)null);
		}

		public static T DeserializeObject<T>(string value, params JsonConverter[] converters)
		{
			return (T)JsonConvert.DeserializeObject(value, typeof(T), converters);
		}

		public static T DeserializeObject<T>(string value, JsonSerializerSettings settings)
		{
			return (T)JsonConvert.DeserializeObject(value, typeof(T), settings);
		}

		public static object DeserializeObject(string value, Type type, params JsonConverter[] converters)
		{
			object jsonSerializerSetting;
			if (converters == null || converters.Length == 0)
			{
				jsonSerializerSetting = null;
			}
			else
			{
				jsonSerializerSetting = new JsonSerializerSettings();
				((JsonSerializerSettings)jsonSerializerSetting).Converters = converters;
			}
			return JsonConvert.DeserializeObject(value, type, (JsonSerializerSettings)jsonSerializerSetting);
		}

		public static object DeserializeObject(string value, Type type, JsonSerializerSettings settings)
		{
			object obj;
			ValidationUtils.ArgumentNotNull(value, "value");
			JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);
			if (!jsonSerializer.IsCheckAdditionalContentSet())
			{
				jsonSerializer.CheckAdditionalContent = true;
			}
			using (JsonTextReader jsonTextReader = new JsonTextReader(new StringReader(value)))
			{
				obj = jsonSerializer.Deserialize(jsonTextReader, type);
			}
			return obj;
		}

		public static XmlDocument DeserializeXmlNode(string value)
		{
			return JsonConvert.DeserializeXmlNode(value, null);
		}

		public static XmlDocument DeserializeXmlNode(string value, string deserializeRootElementName)
		{
			return JsonConvert.DeserializeXmlNode(value, deserializeRootElementName, false);
		}

		public static XmlDocument DeserializeXmlNode(string value, string deserializeRootElementName, bool writeArrayAttribute)
		{
			XmlNodeConverter xmlNodeConverter = new XmlNodeConverter()
			{
				DeserializeRootElementName = deserializeRootElementName,
				WriteArrayAttribute = writeArrayAttribute
			};
			return (XmlDocument)JsonConvert.DeserializeObject(value, typeof(XmlDocument), new JsonConverter[] { xmlNodeConverter });
		}

		public static XDocument DeserializeXNode(string value)
		{
			return JsonConvert.DeserializeXNode(value, null);
		}

		public static XDocument DeserializeXNode(string value, string deserializeRootElementName)
		{
			return JsonConvert.DeserializeXNode(value, deserializeRootElementName, false);
		}

		public static XDocument DeserializeXNode(string value, string deserializeRootElementName, bool writeArrayAttribute)
		{
			XmlNodeConverter xmlNodeConverter = new XmlNodeConverter()
			{
				DeserializeRootElementName = deserializeRootElementName,
				WriteArrayAttribute = writeArrayAttribute
			};
			return (XDocument)JsonConvert.DeserializeObject(value, typeof(XDocument), new JsonConverter[] { xmlNodeConverter });
		}

		private static string EnsureDecimalPlace(double value, string text)
		{
			if (double.IsNaN(value) || double.IsInfinity(value) || text.IndexOf('.') != -1 || text.IndexOf('E') != -1 || text.IndexOf('e') != -1)
			{
				return text;
			}
			return string.Concat(text, ".0");
		}

		private static string EnsureDecimalPlace(string text)
		{
			if (text.IndexOf('.') != -1)
			{
				return text;
			}
			return string.Concat(text, ".0");
		}

		private static string EnsureFloatFormat(double value, string text, FloatFormatHandling floatFormatHandling, char quoteChar, bool nullable)
		{
			if (floatFormatHandling == FloatFormatHandling.Symbol || !double.IsInfinity(value) && !double.IsNaN(value))
			{
				return text;
			}
			if (floatFormatHandling == FloatFormatHandling.DefaultValue)
			{
				if (nullable)
				{
					return JsonConvert.Null;
				}
				return "0.0";
			}
			return string.Concat(quoteChar.ToString(), text, quoteChar.ToString());
		}

		internal static JsonSerializerSettings GetDefaultSettings()
		{
			return JsonConvert.InitialSerializerSettings;
		}

		public static void PopulateObject(string value, object target)
		{
			JsonConvert.PopulateObject(value, target, null);
		}

		public static void PopulateObject(string value, object target, JsonSerializerSettings settings)
		{
			JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);
			using (JsonReader jsonTextReader = new JsonTextReader(new StringReader(value)))
			{
				jsonSerializer.Populate(jsonTextReader, target);
				if (jsonTextReader.Read() && jsonTextReader.TokenType != JsonToken.Comment)
				{
					throw new JsonSerializationException("Additional text found in JSON string after finishing deserializing object.");
				}
			}
		}

		public static string SerializeObject(object value)
		{
			return JsonConvert.SerializeObject(value, null, null);
		}

		public static string SerializeObject(object value, Newtonsoft.Json.Formatting formatting)
		{
			return JsonConvert.SerializeObject(value, formatting, (JsonSerializerSettings)null);
		}

		public static string SerializeObject(object value, params JsonConverter[] converters)
		{
			object jsonSerializerSetting;
			if (converters == null || converters.Length == 0)
			{
				jsonSerializerSetting = null;
			}
			else
			{
				jsonSerializerSetting = new JsonSerializerSettings();
				((JsonSerializerSettings)jsonSerializerSetting).Converters = converters;
			}
			return JsonConvert.SerializeObject(value, null, (JsonSerializerSettings)jsonSerializerSetting);
		}

		public static string SerializeObject(object value, Newtonsoft.Json.Formatting formatting, params JsonConverter[] converters)
		{
			object jsonSerializerSetting;
			if (converters == null || converters.Length == 0)
			{
				jsonSerializerSetting = null;
			}
			else
			{
				jsonSerializerSetting = new JsonSerializerSettings();
				((JsonSerializerSettings)jsonSerializerSetting).Converters = converters;
			}
			return JsonConvert.SerializeObject(value, null, formatting, (JsonSerializerSettings)jsonSerializerSetting);
		}

		public static string SerializeObject(object value, JsonSerializerSettings settings)
		{
			return JsonConvert.SerializeObject(value, null, settings);
		}

		public static string SerializeObject(object value, Type type, JsonSerializerSettings settings)
		{
			return JsonConvert.SerializeObjectInternal(value, type, JsonSerializer.CreateDefault(settings));
		}

		public static string SerializeObject(object value, Newtonsoft.Json.Formatting formatting, JsonSerializerSettings settings)
		{
			return JsonConvert.SerializeObject(value, null, formatting, settings);
		}

		public static string SerializeObject(object value, Type type, Newtonsoft.Json.Formatting formatting, JsonSerializerSettings settings)
		{
			JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);
			jsonSerializer.Formatting = formatting;
			return JsonConvert.SerializeObjectInternal(value, type, jsonSerializer);
		}

		private static string SerializeObjectInternal(object value, Type type, JsonSerializer jsonSerializer)
		{
			StringWriter stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
			using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
			{
				jsonTextWriter.Formatting = jsonSerializer.Formatting;
				jsonSerializer.Serialize(jsonTextWriter, value, type);
			}
			return stringWriter.ToString();
		}

		public static string SerializeXmlNode(XmlNode node)
		{
			return JsonConvert.SerializeXmlNode(node, Newtonsoft.Json.Formatting.None);
		}

		public static string SerializeXmlNode(XmlNode node, Newtonsoft.Json.Formatting formatting)
		{
			XmlNodeConverter xmlNodeConverter = new XmlNodeConverter();
			return JsonConvert.SerializeObject(node, formatting, new JsonConverter[] { xmlNodeConverter });
		}

		public static string SerializeXmlNode(XmlNode node, Newtonsoft.Json.Formatting formatting, bool omitRootObject)
		{
			XmlNodeConverter xmlNodeConverter = new XmlNodeConverter()
			{
				OmitRootObject = omitRootObject
			};
			return JsonConvert.SerializeObject(node, formatting, new JsonConverter[] { xmlNodeConverter });
		}

		public static string SerializeXNode(XObject node)
		{
			return JsonConvert.SerializeXNode(node, Newtonsoft.Json.Formatting.None);
		}

		public static string SerializeXNode(XObject node, Newtonsoft.Json.Formatting formatting)
		{
			return JsonConvert.SerializeXNode(node, formatting, false);
		}

		public static string SerializeXNode(XObject node, Newtonsoft.Json.Formatting formatting, bool omitRootObject)
		{
			XmlNodeConverter xmlNodeConverter = new XmlNodeConverter()
			{
				OmitRootObject = omitRootObject
			};
			return JsonConvert.SerializeObject(node, formatting, new JsonConverter[] { xmlNodeConverter });
		}

		public static string ToString(DateTime value)
		{
			return JsonConvert.ToString(value, DateFormatHandling.IsoDateFormat, DateTimeZoneHandling.RoundtripKind);
		}

		public static string ToString(DateTime value, DateFormatHandling format, DateTimeZoneHandling timeZoneHandling)
		{
			string str;
			DateTime dateTime = DateTimeUtils.EnsureDateTime(value, timeZoneHandling);
			using (StringWriter stringWriter = StringUtils.CreateStringWriter(64))
			{
				stringWriter.Write('\"');
				DateTimeUtils.WriteDateTimeString(stringWriter, dateTime, format, null, CultureInfo.InvariantCulture);
				stringWriter.Write('\"');
				str = stringWriter.ToString();
			}
			return str;
		}

		public static string ToString(DateTimeOffset value)
		{
			return JsonConvert.ToString(value, DateFormatHandling.IsoDateFormat);
		}

		public static string ToString(DateTimeOffset value, DateFormatHandling format)
		{
			string str;
			using (StringWriter stringWriter = StringUtils.CreateStringWriter(64))
			{
				stringWriter.Write('\"');
				DateTimeUtils.WriteDateTimeOffsetString(stringWriter, value, format, null, CultureInfo.InvariantCulture);
				stringWriter.Write('\"');
				str = stringWriter.ToString();
			}
			return str;
		}

		public static string ToString(bool value)
		{
			if (!value)
			{
				return JsonConvert.False;
			}
			return JsonConvert.True;
		}

		public static string ToString(char value)
		{
			return JsonConvert.ToString(char.ToString(value));
		}

		public static string ToString(Enum value)
		{
			return value.ToString("D");
		}

		public static string ToString(int value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(short value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		[CLSCompliant(false)]
		public static string ToString(ushort value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		[CLSCompliant(false)]
		public static string ToString(uint value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(long value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		[CLSCompliant(false)]
		public static string ToString(ulong value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(float value)
		{
			return JsonConvert.EnsureDecimalPlace((double)value, value.ToString("R", CultureInfo.InvariantCulture));
		}

		internal static string ToString(float value, FloatFormatHandling floatFormatHandling, char quoteChar, bool nullable)
		{
			return JsonConvert.EnsureFloatFormat((double)value, JsonConvert.EnsureDecimalPlace((double)value, value.ToString("R", CultureInfo.InvariantCulture)), floatFormatHandling, quoteChar, nullable);
		}

		public static string ToString(double value)
		{
			return JsonConvert.EnsureDecimalPlace(value, value.ToString("R", CultureInfo.InvariantCulture));
		}

		internal static string ToString(double value, FloatFormatHandling floatFormatHandling, char quoteChar, bool nullable)
		{
			return JsonConvert.EnsureFloatFormat(value, JsonConvert.EnsureDecimalPlace(value, value.ToString("R", CultureInfo.InvariantCulture)), floatFormatHandling, quoteChar, nullable);
		}

		public static string ToString(byte value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		[CLSCompliant(false)]
		public static string ToString(sbyte value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(decimal value)
		{
			return JsonConvert.EnsureDecimalPlace(value.ToString(null, CultureInfo.InvariantCulture));
		}

		public static string ToString(Guid value)
		{
			return JsonConvert.ToString(value, '\"');
		}

		internal static string ToString(Guid value, char quoteChar)
		{
			string str = value.ToString("D", CultureInfo.InvariantCulture);
			string str1 = quoteChar.ToString(CultureInfo.InvariantCulture);
			return string.Concat(str1, str, str1);
		}

		public static string ToString(TimeSpan value)
		{
			return JsonConvert.ToString(value, '\"');
		}

		internal static string ToString(TimeSpan value, char quoteChar)
		{
			return JsonConvert.ToString(value.ToString(), quoteChar);
		}

		public static string ToString(Uri value)
		{
			if (value == null)
			{
				return JsonConvert.Null;
			}
			return JsonConvert.ToString(value, '\"');
		}

		internal static string ToString(Uri value, char quoteChar)
		{
			return JsonConvert.ToString(value.OriginalString, quoteChar);
		}

		public static string ToString(string value)
		{
			return JsonConvert.ToString(value, '\"');
		}

		public static string ToString(string value, char delimiter)
		{
			return JsonConvert.ToString(value, delimiter, StringEscapeHandling.Default);
		}

		public static string ToString(string value, char delimiter, StringEscapeHandling stringEscapeHandling)
		{
			if (delimiter != '\"' && delimiter != '\'')
			{
				throw new ArgumentException("Delimiter must be a single or double quote.", "delimiter");
			}
			return JavaScriptUtils.ToEscapedJavaScriptString(value, delimiter, true, stringEscapeHandling);
		}

		public static string ToString(object value)
		{
			if (value == null)
			{
				return JsonConvert.Null;
			}
			PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(value.GetType());
			switch (typeCode)
			{
				case PrimitiveTypeCode.Char:
				{
					return JsonConvert.ToString((char)value);
				}
				case PrimitiveTypeCode.CharNullable:
				case PrimitiveTypeCode.BooleanNullable:
				case PrimitiveTypeCode.SByteNullable:
				case PrimitiveTypeCode.Int16Nullable:
				case PrimitiveTypeCode.UInt16Nullable:
				case PrimitiveTypeCode.Int32Nullable:
				case PrimitiveTypeCode.ByteNullable:
				case PrimitiveTypeCode.UInt32Nullable:
				case PrimitiveTypeCode.Int64Nullable:
				case PrimitiveTypeCode.UInt64Nullable:
				case PrimitiveTypeCode.SingleNullable:
				case PrimitiveTypeCode.DoubleNullable:
				case PrimitiveTypeCode.DateTimeNullable:
				case PrimitiveTypeCode.DateTimeOffsetNullable:
				case PrimitiveTypeCode.DecimalNullable:
				case PrimitiveTypeCode.GuidNullable:
				{
					throw new ArgumentException("Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
				}
				case PrimitiveTypeCode.Boolean:
				{
					return JsonConvert.ToString((bool)value);
				}
				case PrimitiveTypeCode.SByte:
				{
					return JsonConvert.ToString((sbyte)value);
				}
				case PrimitiveTypeCode.Int16:
				{
					return JsonConvert.ToString((short)value);
				}
				case PrimitiveTypeCode.UInt16:
				{
					return JsonConvert.ToString((ushort)value);
				}
				case PrimitiveTypeCode.Int32:
				{
					return JsonConvert.ToString((int)value);
				}
				case PrimitiveTypeCode.Byte:
				{
					return JsonConvert.ToString((byte)value);
				}
				case PrimitiveTypeCode.UInt32:
				{
					return JsonConvert.ToString((uint)value);
				}
				case PrimitiveTypeCode.Int64:
				{
					return JsonConvert.ToString((long)value);
				}
				case PrimitiveTypeCode.UInt64:
				{
					return JsonConvert.ToString((ulong)value);
				}
				case PrimitiveTypeCode.Single:
				{
					return JsonConvert.ToString((float)value);
				}
				case PrimitiveTypeCode.Double:
				{
					return JsonConvert.ToString((double)value);
				}
				case PrimitiveTypeCode.DateTime:
				{
					return JsonConvert.ToString((DateTime)value);
				}
				case PrimitiveTypeCode.DateTimeOffset:
				{
					return JsonConvert.ToString((DateTimeOffset)value);
				}
				case PrimitiveTypeCode.Decimal:
				{
					return JsonConvert.ToString((decimal)value);
				}
				case PrimitiveTypeCode.Guid:
				{
					return JsonConvert.ToString((Guid)value);
				}
				case PrimitiveTypeCode.TimeSpan:
				{
					return JsonConvert.ToString((TimeSpan)value);
				}
				default:
				{
					switch (typeCode)
					{
						case PrimitiveTypeCode.Uri:
						{
							return JsonConvert.ToString((Uri)value);
						}
						case PrimitiveTypeCode.String:
						{
							return JsonConvert.ToString((string)value);
						}
						case PrimitiveTypeCode.Bytes:
						{
							throw new ArgumentException("Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
						}
						case PrimitiveTypeCode.DBNull:
						{
							return JsonConvert.Null;
						}
						default:
						{
							throw new ArgumentException("Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
						}
					}
					break;
				}
			}
		}
	}
}