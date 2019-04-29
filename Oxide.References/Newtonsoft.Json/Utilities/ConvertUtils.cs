using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Shims;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal static class ConvertUtils
	{
		private readonly static Dictionary<Type, PrimitiveTypeCode> TypeCodeMap;

		private readonly static TypeInformation[] PrimitiveTypeCodes;

		private readonly static ThreadSafeStore<ConvertUtils.TypeConvertKey, Func<object, object>> CastConverters;

		static ConvertUtils()
		{
			ConvertUtils.TypeCodeMap = new Dictionary<Type, PrimitiveTypeCode>()
			{
				{ typeof(char), PrimitiveTypeCode.Char },
				{ typeof(char?), PrimitiveTypeCode.CharNullable },
				{ typeof(bool), PrimitiveTypeCode.Boolean },
				{ typeof(bool?), PrimitiveTypeCode.BooleanNullable },
				{ typeof(sbyte), PrimitiveTypeCode.SByte },
				{ typeof(sbyte?), PrimitiveTypeCode.SByteNullable },
				{ typeof(short), PrimitiveTypeCode.Int16 },
				{ typeof(short?), PrimitiveTypeCode.Int16Nullable },
				{ typeof(ushort), PrimitiveTypeCode.UInt16 },
				{ typeof(ushort?), PrimitiveTypeCode.UInt16Nullable },
				{ typeof(int), PrimitiveTypeCode.Int32 },
				{ typeof(int?), PrimitiveTypeCode.Int32Nullable },
				{ typeof(byte), PrimitiveTypeCode.Byte },
				{ typeof(byte?), PrimitiveTypeCode.ByteNullable },
				{ typeof(uint), PrimitiveTypeCode.UInt32 },
				{ typeof(uint?), PrimitiveTypeCode.UInt32Nullable },
				{ typeof(long), PrimitiveTypeCode.Int64 },
				{ typeof(long?), PrimitiveTypeCode.Int64Nullable },
				{ typeof(ulong), PrimitiveTypeCode.UInt64 },
				{ typeof(ulong?), PrimitiveTypeCode.UInt64Nullable },
				{ typeof(float), PrimitiveTypeCode.Single },
				{ typeof(float?), PrimitiveTypeCode.SingleNullable },
				{ typeof(double), PrimitiveTypeCode.Double },
				{ typeof(double?), PrimitiveTypeCode.DoubleNullable },
				{ typeof(DateTime), PrimitiveTypeCode.DateTime },
				{ typeof(DateTime?), PrimitiveTypeCode.DateTimeNullable },
				{ typeof(DateTimeOffset), PrimitiveTypeCode.DateTimeOffset },
				{ typeof(DateTimeOffset?), PrimitiveTypeCode.DateTimeOffsetNullable },
				{ typeof(decimal), PrimitiveTypeCode.Decimal },
				{ typeof(decimal?), PrimitiveTypeCode.DecimalNullable },
				{ typeof(Guid), PrimitiveTypeCode.Guid },
				{ typeof(Guid?), PrimitiveTypeCode.GuidNullable },
				{ typeof(TimeSpan), PrimitiveTypeCode.TimeSpan },
				{ typeof(TimeSpan?), PrimitiveTypeCode.TimeSpanNullable },
				{ typeof(Uri), PrimitiveTypeCode.Uri },
				{ typeof(string), PrimitiveTypeCode.String },
				{ typeof(byte[]), PrimitiveTypeCode.Bytes },
				{ typeof(DBNull), PrimitiveTypeCode.DBNull }
			};
			ConvertUtils.PrimitiveTypeCodes = new TypeInformation[] { new TypeInformation()
			{
				Type = typeof(object),
				TypeCode = PrimitiveTypeCode.Empty
			}, new TypeInformation()
			{
				Type = typeof(object),
				TypeCode = PrimitiveTypeCode.Object
			}, new TypeInformation()
			{
				Type = typeof(object),
				TypeCode = PrimitiveTypeCode.DBNull
			}, new TypeInformation()
			{
				Type = typeof(bool),
				TypeCode = PrimitiveTypeCode.Boolean
			}, new TypeInformation()
			{
				Type = typeof(char),
				TypeCode = PrimitiveTypeCode.Char
			}, new TypeInformation()
			{
				Type = typeof(sbyte),
				TypeCode = PrimitiveTypeCode.SByte
			}, new TypeInformation()
			{
				Type = typeof(byte),
				TypeCode = PrimitiveTypeCode.Byte
			}, new TypeInformation()
			{
				Type = typeof(short),
				TypeCode = PrimitiveTypeCode.Int16
			}, new TypeInformation()
			{
				Type = typeof(ushort),
				TypeCode = PrimitiveTypeCode.UInt16
			}, new TypeInformation()
			{
				Type = typeof(int),
				TypeCode = PrimitiveTypeCode.Int32
			}, new TypeInformation()
			{
				Type = typeof(uint),
				TypeCode = PrimitiveTypeCode.UInt32
			}, new TypeInformation()
			{
				Type = typeof(long),
				TypeCode = PrimitiveTypeCode.Int64
			}, new TypeInformation()
			{
				Type = typeof(ulong),
				TypeCode = PrimitiveTypeCode.UInt64
			}, new TypeInformation()
			{
				Type = typeof(float),
				TypeCode = PrimitiveTypeCode.Single
			}, new TypeInformation()
			{
				Type = typeof(double),
				TypeCode = PrimitiveTypeCode.Double
			}, new TypeInformation()
			{
				Type = typeof(decimal),
				TypeCode = PrimitiveTypeCode.Decimal
			}, new TypeInformation()
			{
				Type = typeof(DateTime),
				TypeCode = PrimitiveTypeCode.DateTime
			}, new TypeInformation()
			{
				Type = typeof(object),
				TypeCode = PrimitiveTypeCode.Empty
			}, new TypeInformation()
			{
				Type = typeof(string),
				TypeCode = PrimitiveTypeCode.String
			} };
			ConvertUtils.CastConverters = new ThreadSafeStore<ConvertUtils.TypeConvertKey, Func<object, object>>(new Func<ConvertUtils.TypeConvertKey, Func<object, object>>(ConvertUtils.CreateCastConverter));
		}

		public static object Convert(object initialValue, CultureInfo culture, Type targetType)
		{
			object obj;
			switch (ConvertUtils.TryConvertInternal(initialValue, culture, targetType, out obj))
			{
				case ConvertUtils.ConvertResult.Success:
				{
					return obj;
				}
				case ConvertUtils.ConvertResult.CannotConvertNull:
				{
					throw new Exception("Can not convert null {0} into non-nullable {1}.".FormatWith(CultureInfo.InvariantCulture, initialValue.GetType(), targetType));
				}
				case ConvertUtils.ConvertResult.NotInstantiableType:
				{
					throw new ArgumentException("Target type {0} is not a value type or a non-abstract class.".FormatWith(CultureInfo.InvariantCulture, targetType), "targetType");
				}
				case ConvertUtils.ConvertResult.NoValidConversion:
				{
					throw new InvalidOperationException("Can not convert from {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, initialValue.GetType(), targetType));
				}
			}
			throw new InvalidOperationException("Unexpected conversion result.");
		}

		public static object ConvertOrCast(object initialValue, CultureInfo culture, Type targetType)
		{
			object obj;
			if (targetType == typeof(object))
			{
				return initialValue;
			}
			if (initialValue == null && ReflectionUtils.IsNullable(targetType))
			{
				return null;
			}
			if (ConvertUtils.TryConvert(initialValue, culture, targetType, out obj))
			{
				return obj;
			}
			return ConvertUtils.EnsureTypeAssignable(initialValue, ReflectionUtils.GetObjectType(initialValue), targetType);
		}

		private static Func<object, object> CreateCastConverter(ConvertUtils.TypeConvertKey t)
		{
			MethodInfo method = t.TargetType.GetMethod("op_Implicit", new Type[] { t.InitialType }) ?? t.TargetType.GetMethod("op_Explicit", new Type[] { t.InitialType });
			if (method == null)
			{
				return null;
			}
			MethodCall<object, object> methodCall = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
			return (object o) => methodCall(null, new object[] { o });
		}

		private static object EnsureTypeAssignable(object value, Type initialType, Type targetType)
		{
			Type type;
			if (value != null)
			{
				type = value.GetType();
			}
			else
			{
				type = null;
			}
			Type type1 = type;
			if (value != null)
			{
				if (targetType.IsAssignableFrom(type1))
				{
					return value;
				}
				Func<object, object> func = ConvertUtils.CastConverters.Get(new ConvertUtils.TypeConvertKey(type1, targetType));
				if (func != null)
				{
					return func(value);
				}
			}
			else if (ReflectionUtils.IsNullable(targetType))
			{
				return null;
			}
			throw new ArgumentException("Could not cast or convert from {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, (initialType != null ? initialType.ToString() : "{null}"), targetType));
		}

		internal static TypeConverter GetConverter(Type t)
		{
			return JsonTypeReflector.GetTypeConverter(t);
		}

		public static PrimitiveTypeCode GetTypeCode(Type t)
		{
			bool flag;
			return ConvertUtils.GetTypeCode(t, out flag);
		}

		public static PrimitiveTypeCode GetTypeCode(Type t, out bool isEnum)
		{
			PrimitiveTypeCode primitiveTypeCode;
			if (ConvertUtils.TypeCodeMap.TryGetValue(t, out primitiveTypeCode))
			{
				isEnum = false;
				return primitiveTypeCode;
			}
			if (t.IsEnum())
			{
				isEnum = true;
				return ConvertUtils.GetTypeCode(Enum.GetUnderlyingType(t));
			}
			if (ReflectionUtils.IsNullableType(t))
			{
				Type underlyingType = Nullable.GetUnderlyingType(t);
				if (underlyingType.IsEnum())
				{
					Type type = typeof(Nullable<>);
					Type[] typeArray = new Type[] { Enum.GetUnderlyingType(underlyingType) };
					isEnum = true;
					return ConvertUtils.GetTypeCode(type.MakeGenericType(typeArray));
				}
			}
			isEnum = false;
			return PrimitiveTypeCode.Object;
		}

		public static TypeInformation GetTypeInformation(IConvertible convertable)
		{
			return ConvertUtils.PrimitiveTypeCodes[(int)convertable.GetTypeCode()];
		}

		private static int HexCharToInt(char ch)
		{
			if (ch <= '9' && ch >= '0')
			{
				return ch - 48;
			}
			if (ch <= 'F' && ch >= 'A')
			{
				return ch - 55;
			}
			if (ch > 'f' || ch < 'a')
			{
				throw new FormatException(string.Concat("Invalid hex character: ", ch.ToString()));
			}
			return ch - 87;
		}

		public static int HexTextToInt(char[] text, int start, int end)
		{
			int num = 0;
			for (int i = start; i < end; i++)
			{
				num = num + (ConvertUtils.HexCharToInt(text[i]) << ((end - 1 - i) * 4 & 31));
			}
			return num;
		}

		public static ParseResult Int32TryParse(char[] chars, int start, int length, out int value)
		{
			value = 0;
			if (length == 0)
			{
				return ParseResult.Invalid;
			}
			bool flag = chars[start] == '-';
			if (flag)
			{
				if (length == 1)
				{
					return ParseResult.Invalid;
				}
				start++;
				length--;
			}
			int num = start + length;
			if (length > 10 || length == 10 && chars[start] - 48 > '\u0002')
			{
				for (int i = start; i < num; i++)
				{
					int num1 = chars[i] - 48;
					if (num1 < 0 || num1 > 9)
					{
						return ParseResult.Invalid;
					}
				}
				return ParseResult.Overflow;
			}
			for (int j = start; j < num; j++)
			{
				int num2 = chars[j] - 48;
				if (num2 < 0 || num2 > 9)
				{
					return ParseResult.Invalid;
				}
				int num3 = 10 * value - num2;
				if (num3 > value)
				{
					for (j++; j < num; j++)
					{
						num2 = chars[j] - 48;
						if (num2 < 0 || num2 > 9)
						{
							return ParseResult.Invalid;
						}
					}
					return ParseResult.Overflow;
				}
				value = num3;
			}
			if (!flag)
			{
				if (value == -2147483648)
				{
					return ParseResult.Overflow;
				}
				value = -value;
			}
			return ParseResult.Success;
		}

		public static ParseResult Int64TryParse(char[] chars, int start, int length, out long value)
		{
			value = (long)0;
			if (length == 0)
			{
				return ParseResult.Invalid;
			}
			bool flag = chars[start] == '-';
			if (flag)
			{
				if (length == 1)
				{
					return ParseResult.Invalid;
				}
				start++;
				length--;
			}
			int num = start + length;
			if (length > 19)
			{
				for (int i = start; i < num; i++)
				{
					int num1 = chars[i] - 48;
					if (num1 < 0 || num1 > 9)
					{
						return ParseResult.Invalid;
					}
				}
				return ParseResult.Overflow;
			}
			for (int j = start; j < num; j++)
			{
				int num2 = chars[j] - 48;
				if (num2 < 0 || num2 > 9)
				{
					return ParseResult.Invalid;
				}
				long num3 = (long)10 * value - (long)num2;
				if (num3 > value)
				{
					for (j++; j < num; j++)
					{
						num2 = chars[j] - 48;
						if (num2 < 0 || num2 > 9)
						{
							return ParseResult.Invalid;
						}
					}
					return ParseResult.Overflow;
				}
				value = num3;
			}
			if (!flag)
			{
				if (value == -9223372036854775808L)
				{
					return ParseResult.Overflow;
				}
				value = -value;
			}
			return ParseResult.Success;
		}

		public static bool IsConvertible(Type t)
		{
			return typeof(IConvertible).IsAssignableFrom(t);
		}

		public static bool IsInteger(object value)
		{
			switch (ConvertUtils.GetTypeCode(value.GetType()))
			{
				case PrimitiveTypeCode.SByte:
				case PrimitiveTypeCode.Int16:
				case PrimitiveTypeCode.UInt16:
				case PrimitiveTypeCode.Int32:
				case PrimitiveTypeCode.Byte:
				case PrimitiveTypeCode.UInt32:
				case PrimitiveTypeCode.Int64:
				case PrimitiveTypeCode.UInt64:
				{
					return true;
				}
				case PrimitiveTypeCode.SByteNullable:
				case PrimitiveTypeCode.Int16Nullable:
				case PrimitiveTypeCode.UInt16Nullable:
				case PrimitiveTypeCode.Int32Nullable:
				case PrimitiveTypeCode.ByteNullable:
				case PrimitiveTypeCode.UInt32Nullable:
				case PrimitiveTypeCode.Int64Nullable:
				{
					return false;
				}
				default:
				{
					return false;
				}
			}
		}

		public static TimeSpan ParseTimeSpan(string input)
		{
			return TimeSpan.Parse(input);
		}

		private static bool TryConvert(object initialValue, CultureInfo culture, Type targetType, out object value)
		{
			bool flag;
			try
			{
				if (ConvertUtils.TryConvertInternal(initialValue, culture, targetType, out value) != ConvertUtils.ConvertResult.Success)
				{
					value = null;
					flag = false;
				}
				else
				{
					flag = true;
				}
			}
			catch
			{
				value = null;
				flag = false;
			}
			return flag;
		}

		public static bool TryConvertGuid(string s, out Guid g)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (!(new Regex("^[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}$")).Match(s).Success)
			{
				g = Guid.Empty;
				return false;
			}
			g = new Guid(s);
			return true;
		}

		private static ConvertUtils.ConvertResult TryConvertInternal(object initialValue, CultureInfo culture, Type targetType, out object value)
		{
			Version version;
			if (initialValue == null)
			{
				throw new ArgumentNullException("initialValue");
			}
			if (ReflectionUtils.IsNullableType(targetType))
			{
				targetType = Nullable.GetUnderlyingType(targetType);
			}
			Type type = initialValue.GetType();
			if (targetType == type)
			{
				value = initialValue;
				return ConvertUtils.ConvertResult.Success;
			}
			if (ConvertUtils.IsConvertible(initialValue.GetType()) && ConvertUtils.IsConvertible(targetType))
			{
				if (targetType.IsEnum())
				{
					if (initialValue is string)
					{
						value = Enum.Parse(targetType, initialValue.ToString(), true);
						return ConvertUtils.ConvertResult.Success;
					}
					if (ConvertUtils.IsInteger(initialValue))
					{
						value = Enum.ToObject(targetType, initialValue);
						return ConvertUtils.ConvertResult.Success;
					}
				}
				value = Convert.ChangeType(initialValue, targetType, culture);
				return ConvertUtils.ConvertResult.Success;
			}
			if (initialValue is DateTime && targetType == typeof(DateTimeOffset))
			{
				value = new DateTimeOffset((DateTime)initialValue);
				return ConvertUtils.ConvertResult.Success;
			}
			if (initialValue is byte[] && targetType == typeof(Guid))
			{
				value = new Guid((byte[])initialValue);
				return ConvertUtils.ConvertResult.Success;
			}
			if (initialValue is Guid && targetType == typeof(byte[]))
			{
				value = ((Guid)initialValue).ToByteArray();
				return ConvertUtils.ConvertResult.Success;
			}
			string str = initialValue as string;
			if (str != null)
			{
				if (targetType == typeof(Guid))
				{
					value = new Guid(str);
					return ConvertUtils.ConvertResult.Success;
				}
				if (targetType == typeof(Uri))
				{
					value = new Uri(str, UriKind.RelativeOrAbsolute);
					return ConvertUtils.ConvertResult.Success;
				}
				if (targetType == typeof(TimeSpan))
				{
					value = ConvertUtils.ParseTimeSpan(str);
					return ConvertUtils.ConvertResult.Success;
				}
				if (targetType == typeof(byte[]))
				{
					value = Convert.FromBase64String(str);
					return ConvertUtils.ConvertResult.Success;
				}
				if (targetType == typeof(Version))
				{
					if (ConvertUtils.VersionTryParse(str, out version))
					{
						value = version;
						return ConvertUtils.ConvertResult.Success;
					}
					value = null;
					return ConvertUtils.ConvertResult.NoValidConversion;
				}
				if (typeof(Type).IsAssignableFrom(targetType))
				{
					value = Type.GetType(str, true);
					return ConvertUtils.ConvertResult.Success;
				}
			}
			TypeConverter converter = ConvertUtils.GetConverter(type);
			if (converter != null && converter.CanConvertTo(targetType))
			{
				value = converter.ConvertTo(null, culture, initialValue, targetType);
				return ConvertUtils.ConvertResult.Success;
			}
			TypeConverter typeConverter = ConvertUtils.GetConverter(targetType);
			if (typeConverter != null && typeConverter.CanConvertFrom(type))
			{
				value = typeConverter.ConvertFrom(null, culture, initialValue);
				return ConvertUtils.ConvertResult.Success;
			}
			if (initialValue == DBNull.Value)
			{
				if (!ReflectionUtils.IsNullable(targetType))
				{
					value = null;
					return ConvertUtils.ConvertResult.CannotConvertNull;
				}
				value = ConvertUtils.EnsureTypeAssignable(null, type, targetType);
				return ConvertUtils.ConvertResult.Success;
			}
			if (!targetType.IsInterface() && !targetType.IsGenericTypeDefinition() && !targetType.IsAbstract())
			{
				value = null;
				return ConvertUtils.ConvertResult.NoValidConversion;
			}
			value = null;
			return ConvertUtils.ConvertResult.NotInstantiableType;
		}

		public static bool VersionTryParse(string input, out Version result)
		{
			bool flag;
			try
			{
				result = new Version(input);
				flag = true;
			}
			catch
			{
				result = null;
				flag = false;
			}
			return flag;
		}

		internal enum ConvertResult
		{
			Success,
			CannotConvertNull,
			NotInstantiableType,
			NoValidConversion
		}

		internal struct TypeConvertKey
		{
			private readonly Type _initialType;

			private readonly Type _targetType;

			public Type InitialType
			{
				get
				{
					return this._initialType;
				}
			}

			public Type TargetType
			{
				get
				{
					return this._targetType;
				}
			}

			public TypeConvertKey(Type initialType, Type targetType)
			{
				this._initialType = initialType;
				this._targetType = targetType;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is ConvertUtils.TypeConvertKey))
				{
					return false;
				}
				return this.Equals((ConvertUtils.TypeConvertKey)obj);
			}

			public bool Equals(ConvertUtils.TypeConvertKey other)
			{
				if (this._initialType != other._initialType)
				{
					return false;
				}
				return this._targetType == other._targetType;
			}

			public override int GetHashCode()
			{
				return this._initialType.GetHashCode() ^ this._targetType.GetHashCode();
			}
		}
	}
}