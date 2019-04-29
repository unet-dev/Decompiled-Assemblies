using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Newtonsoft.Json.Linq
{
	[Preserve]
	public class JValue : JToken, IFormattable, IComparable, IConvertible
	{
		private JTokenType _valueType;

		private object _value;

		public override bool HasValues
		{
			get
			{
				return false;
			}
		}

		public override JTokenType Type
		{
			get
			{
				return this._valueType;
			}
		}

		public object Value
		{
			get
			{
				return this._value;
			}
			set
			{
				System.Type type;
				System.Type type1;
				if (this._value != null)
				{
					type1 = this._value.GetType();
				}
				else
				{
					type1 = null;
				}
				if (value != null)
				{
					type = value.GetType();
				}
				else
				{
					type = null;
				}
				if (type1 != type)
				{
					this._valueType = JValue.GetValueType(new JTokenType?(this._valueType), value);
				}
				this._value = value;
			}
		}

		internal JValue(object value, JTokenType type)
		{
			this._value = value;
			this._valueType = type;
		}

		public JValue(JValue other) : this(other.Value, other.Type)
		{
		}

		public JValue(long value) : this(value, JTokenType.Integer)
		{
		}

		public JValue(decimal value) : this(value, JTokenType.Float)
		{
		}

		public JValue(char value) : this(value, JTokenType.String)
		{
		}

		[CLSCompliant(false)]
		public JValue(ulong value) : this(value, JTokenType.Integer)
		{
		}

		public JValue(double value) : this(value, JTokenType.Float)
		{
		}

		public JValue(float value) : this(value, JTokenType.Float)
		{
		}

		public JValue(DateTime value) : this(value, JTokenType.Date)
		{
		}

		public JValue(DateTimeOffset value) : this(value, JTokenType.Date)
		{
		}

		public JValue(bool value) : this(value, JTokenType.Boolean)
		{
		}

		public JValue(string value) : this(value, JTokenType.String)
		{
		}

		public JValue(Guid value) : this(value, JTokenType.Guid)
		{
		}

		public JValue(Uri value) : this(value, (value != null ? JTokenType.Uri : JTokenType.Null))
		{
		}

		public JValue(TimeSpan value) : this(value, JTokenType.TimeSpan)
		{
		}

		public JValue(object value) : this(value, JValue.GetValueType(null, value))
		{
		}

		internal override JToken CloneToken()
		{
			return new JValue(this);
		}

		internal static int Compare(JTokenType valueType, object objA, object objB)
		{
			DateTime dateTime;
			DateTimeOffset dateTimeOffset;
			if (objA == null && objB == null)
			{
				return 0;
			}
			if (objA != null && objB == null)
			{
				return 1;
			}
			if (objA == null && objB != null)
			{
				return -1;
			}
			switch (valueType)
			{
				case JTokenType.Comment:
				case JTokenType.String:
				case JTokenType.Raw:
				{
					string str = Convert.ToString(objA, CultureInfo.InvariantCulture);
					string str1 = Convert.ToString(objB, CultureInfo.InvariantCulture);
					return string.CompareOrdinal(str, str1);
				}
				case JTokenType.Integer:
				{
					if (objA is ulong || objB is ulong || objA is decimal || objB is decimal)
					{
						decimal num = Convert.ToDecimal(objA, CultureInfo.InvariantCulture);
						return num.CompareTo(Convert.ToDecimal(objB, CultureInfo.InvariantCulture));
					}
					if (objA is float || objB is float || objA is double || objB is double)
					{
						return JValue.CompareFloat(objA, objB);
					}
					long num1 = Convert.ToInt64(objA, CultureInfo.InvariantCulture);
					return num1.CompareTo(Convert.ToInt64(objB, CultureInfo.InvariantCulture));
				}
				case JTokenType.Float:
				{
					return JValue.CompareFloat(objA, objB);
				}
				case JTokenType.Boolean:
				{
					bool flag = Convert.ToBoolean(objA, CultureInfo.InvariantCulture);
					bool flag1 = Convert.ToBoolean(objB, CultureInfo.InvariantCulture);
					return flag.CompareTo(flag1);
				}
				case JTokenType.Null:
				case JTokenType.Undefined:
				{
					throw MiscellaneousUtils.CreateArgumentOutOfRangeException("valueType", valueType, "Unexpected value type: {0}".FormatWith(CultureInfo.InvariantCulture, valueType));
				}
				case JTokenType.Date:
				{
					if (!(objA is DateTime))
					{
						DateTimeOffset dateTimeOffset1 = (DateTimeOffset)objA;
						dateTimeOffset = (!(objB is DateTimeOffset) ? new DateTimeOffset(Convert.ToDateTime(objB, CultureInfo.InvariantCulture)) : (DateTimeOffset)objB);
						return dateTimeOffset1.CompareTo(dateTimeOffset);
					}
					DateTime dateTime1 = (DateTime)objA;
					dateTime = (!(objB is DateTimeOffset) ? Convert.ToDateTime(objB, CultureInfo.InvariantCulture) : ((DateTimeOffset)objB).DateTime);
					return dateTime1.CompareTo(dateTime);
				}
				case JTokenType.Bytes:
				{
					if (!(objB is byte[]))
					{
						throw new ArgumentException("Object must be of type byte[].");
					}
					byte[] numArray = objA as byte[];
					byte[] numArray1 = objB as byte[];
					if (numArray == null)
					{
						return -1;
					}
					if (numArray1 == null)
					{
						return 1;
					}
					return MiscellaneousUtils.ByteArrayCompare(numArray, numArray1);
				}
				case JTokenType.Guid:
				{
					if (!(objB is Guid))
					{
						throw new ArgumentException("Object must be of type Guid.");
					}
					return ((Guid)objA).CompareTo((Guid)objB);
				}
				case JTokenType.Uri:
				{
					if (!(objB is Uri))
					{
						throw new ArgumentException("Object must be of type Uri.");
					}
					Uri uri = (Uri)objA;
					Uri uri1 = (Uri)objB;
					return Comparer<string>.Default.Compare(uri.ToString(), uri1.ToString());
				}
				case JTokenType.TimeSpan:
				{
					if (!(objB is TimeSpan))
					{
						throw new ArgumentException("Object must be of type TimeSpan.");
					}
					return ((TimeSpan)objA).CompareTo((TimeSpan)objB);
				}
				default:
				{
					throw MiscellaneousUtils.CreateArgumentOutOfRangeException("valueType", valueType, "Unexpected value type: {0}".FormatWith(CultureInfo.InvariantCulture, valueType));
				}
			}
		}

		private static int CompareFloat(object objA, object objB)
		{
			double num = Convert.ToDouble(objA, CultureInfo.InvariantCulture);
			double num1 = Convert.ToDouble(objB, CultureInfo.InvariantCulture);
			if (MathUtils.ApproxEquals(num, num1))
			{
				return 0;
			}
			return num.CompareTo(num1);
		}

		public int CompareTo(JValue obj)
		{
			if (obj == null)
			{
				return 1;
			}
			return JValue.Compare(this._valueType, this._value, obj._value);
		}

		public static JValue CreateComment(string value)
		{
			return new JValue(value, JTokenType.Comment);
		}

		public static JValue CreateNull()
		{
			return new JValue(null, JTokenType.Null);
		}

		public static JValue CreateString(string value)
		{
			return new JValue(value, JTokenType.String);
		}

		public static JValue CreateUndefined()
		{
			return new JValue(null, JTokenType.Undefined);
		}

		internal override bool DeepEquals(JToken node)
		{
			JValue jValue = node as JValue;
			if (jValue == null)
			{
				return false;
			}
			if (jValue == this)
			{
				return true;
			}
			return JValue.ValuesEquals(this, jValue);
		}

		public bool Equals(JValue other)
		{
			if (other == null)
			{
				return false;
			}
			return JValue.ValuesEquals(this, other);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			JValue jValue = obj as JValue;
			if (jValue != null)
			{
				return this.Equals(jValue);
			}
			return base.Equals(obj);
		}

		internal override int GetDeepHashCode()
		{
			return this._valueType.GetHashCode() ^ (this._value != null ? this._value.GetHashCode() : 0);
		}

		public override int GetHashCode()
		{
			if (this._value == null)
			{
				return 0;
			}
			return this._value.GetHashCode();
		}

		private static JTokenType GetStringValueType(JTokenType? current)
		{
			if (!current.HasValue)
			{
				return JTokenType.String;
			}
			JTokenType valueOrDefault = current.GetValueOrDefault();
			if (valueOrDefault != JTokenType.Comment && valueOrDefault != JTokenType.String && valueOrDefault != JTokenType.Raw)
			{
				return JTokenType.String;
			}
			return current.GetValueOrDefault();
		}

		private static JTokenType GetValueType(JTokenType? current, object value)
		{
			if (value == null)
			{
				return JTokenType.Null;
			}
			if (value == DBNull.Value)
			{
				return JTokenType.Null;
			}
			if (value is string)
			{
				return JValue.GetStringValueType(current);
			}
			if (value is long || value is int || value is short || value is sbyte || value is ulong || value is uint || value is ushort || value is byte)
			{
				return JTokenType.Integer;
			}
			if (value is Enum)
			{
				return JTokenType.Integer;
			}
			if (value is double || value is float || value is decimal)
			{
				return JTokenType.Float;
			}
			if (value is DateTime)
			{
				return JTokenType.Date;
			}
			if (value is DateTimeOffset)
			{
				return JTokenType.Date;
			}
			if (value is byte[])
			{
				return JTokenType.Bytes;
			}
			if (value is bool)
			{
				return JTokenType.Boolean;
			}
			if (value is Guid)
			{
				return JTokenType.Guid;
			}
			if (value is Uri)
			{
				return JTokenType.Uri;
			}
			if (!(value is TimeSpan))
			{
				throw new ArgumentException("Could not determine JSON object type for type {0}.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
			}
			return JTokenType.TimeSpan;
		}

		int System.IComparable.CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			return JValue.Compare(this._valueType, this._value, (obj is JValue ? ((JValue)obj).Value : obj));
		}

		TypeCode System.IConvertible.GetTypeCode()
		{
			if (this._value == null)
			{
				return TypeCode.Empty;
			}
			IConvertible convertible = this._value as IConvertible;
			if (convertible == null)
			{
				return TypeCode.Object;
			}
			return convertible.GetTypeCode();
		}

		bool System.IConvertible.ToBoolean(IFormatProvider provider)
		{
			return (bool)this;
		}

		byte System.IConvertible.ToByte(IFormatProvider provider)
		{
			return (byte)this;
		}

		char System.IConvertible.ToChar(IFormatProvider provider)
		{
			return (char)this;
		}

		DateTime System.IConvertible.ToDateTime(IFormatProvider provider)
		{
			return (DateTime)this;
		}

		decimal System.IConvertible.ToDecimal(IFormatProvider provider)
		{
			return (decimal)this;
		}

		double System.IConvertible.ToDouble(IFormatProvider provider)
		{
			return (double)((double)this);
		}

		short System.IConvertible.ToInt16(IFormatProvider provider)
		{
			return (short)this;
		}

		int System.IConvertible.ToInt32(IFormatProvider provider)
		{
			return (int)this;
		}

		long System.IConvertible.ToInt64(IFormatProvider provider)
		{
			return (long)this;
		}

		sbyte System.IConvertible.ToSByte(IFormatProvider provider)
		{
			return (sbyte)this;
		}

		float System.IConvertible.ToSingle(IFormatProvider provider)
		{
			return (float)((float)this);
		}

		object System.IConvertible.ToType(System.Type conversionType, IFormatProvider provider)
		{
			return base.ToObject(conversionType);
		}

		ushort System.IConvertible.ToUInt16(IFormatProvider provider)
		{
			return (ushort)this;
		}

		uint System.IConvertible.ToUInt32(IFormatProvider provider)
		{
			return (uint)this;
		}

		ulong System.IConvertible.ToUInt64(IFormatProvider provider)
		{
			return (ulong)this;
		}

		public override string ToString()
		{
			if (this._value == null)
			{
				return string.Empty;
			}
			return this._value.ToString();
		}

		public string ToString(string format)
		{
			return this.ToString(format, CultureInfo.CurrentCulture);
		}

		public string ToString(IFormatProvider formatProvider)
		{
			return this.ToString(null, formatProvider);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (this._value == null)
			{
				return string.Empty;
			}
			IFormattable formattable = this._value as IFormattable;
			if (formattable == null)
			{
				return this._value.ToString();
			}
			return formattable.ToString(format, formatProvider);
		}

		private static bool ValuesEquals(JValue v1, JValue v2)
		{
			if (v1 == v2)
			{
				return true;
			}
			if (v1._valueType != v2._valueType)
			{
				return false;
			}
			return JValue.Compare(v1._valueType, v1._value, v2._value) == 0;
		}

		public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
		{
			string str;
			string str1;
			string str2;
			Guid? nullable;
			TimeSpan? nullable1;
			if (converters != null && converters.Length != 0 && this._value != null)
			{
				JsonConverter matchingConverter = JsonSerializer.GetMatchingConverter(converters, this._value.GetType());
				if (matchingConverter != null && matchingConverter.CanWrite)
				{
					matchingConverter.WriteJson(writer, this._value, JsonSerializer.CreateDefault());
					return;
				}
			}
			switch (this._valueType)
			{
				case JTokenType.Comment:
				{
					JsonWriter jsonWriter = writer;
					if (this._value != null)
					{
						str = this._value.ToString();
					}
					else
					{
						str = null;
					}
					jsonWriter.WriteComment(str);
					return;
				}
				case JTokenType.Integer:
				{
					if (this._value is int)
					{
						writer.WriteValue((int)this._value);
						return;
					}
					if (this._value is long)
					{
						writer.WriteValue((long)this._value);
						return;
					}
					if (this._value is ulong)
					{
						writer.WriteValue((ulong)this._value);
						return;
					}
					writer.WriteValue(Convert.ToInt64(this._value, CultureInfo.InvariantCulture));
					return;
				}
				case JTokenType.Float:
				{
					if (this._value is decimal)
					{
						writer.WriteValue((decimal)this._value);
						return;
					}
					if (this._value is double)
					{
						writer.WriteValue((double)this._value);
						return;
					}
					if (this._value is float)
					{
						writer.WriteValue((float)this._value);
						return;
					}
					writer.WriteValue(Convert.ToDouble(this._value, CultureInfo.InvariantCulture));
					return;
				}
				case JTokenType.String:
				{
					JsonWriter jsonWriter1 = writer;
					if (this._value != null)
					{
						str1 = this._value.ToString();
					}
					else
					{
						str1 = null;
					}
					jsonWriter1.WriteValue(str1);
					return;
				}
				case JTokenType.Boolean:
				{
					writer.WriteValue(Convert.ToBoolean(this._value, CultureInfo.InvariantCulture));
					return;
				}
				case JTokenType.Null:
				{
					writer.WriteNull();
					return;
				}
				case JTokenType.Undefined:
				{
					writer.WriteUndefined();
					return;
				}
				case JTokenType.Date:
				{
					if (this._value is DateTimeOffset)
					{
						writer.WriteValue((DateTimeOffset)this._value);
						return;
					}
					writer.WriteValue(Convert.ToDateTime(this._value, CultureInfo.InvariantCulture));
					return;
				}
				case JTokenType.Raw:
				{
					JsonWriter jsonWriter2 = writer;
					if (this._value != null)
					{
						str2 = this._value.ToString();
					}
					else
					{
						str2 = null;
					}
					jsonWriter2.WriteRawValue(str2);
					return;
				}
				case JTokenType.Bytes:
				{
					writer.WriteValue((byte[])this._value);
					return;
				}
				case JTokenType.Guid:
				{
					JsonWriter jsonWriter3 = writer;
					if (this._value != null)
					{
						nullable = (Guid?)this._value;
					}
					else
					{
						nullable = null;
					}
					jsonWriter3.WriteValue(nullable);
					return;
				}
				case JTokenType.Uri:
				{
					writer.WriteValue((Uri)this._value);
					return;
				}
				case JTokenType.TimeSpan:
				{
					JsonWriter jsonWriter4 = writer;
					if (this._value != null)
					{
						nullable1 = (TimeSpan?)this._value;
					}
					else
					{
						nullable1 = null;
					}
					jsonWriter4.WriteValue(nullable1);
					return;
				}
			}
			throw MiscellaneousUtils.CreateArgumentOutOfRangeException("TokenType", this._valueType, "Unexpected token type.");
		}
	}
}