using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace ProtoBuf
{
	internal sealed class Helpers
	{
		public readonly static Type[] EmptyTypes;

		static Helpers()
		{
			Helpers.EmptyTypes = Type.EmptyTypes;
		}

		private Helpers()
		{
		}

		public static StringBuilder AppendLine(StringBuilder builder)
		{
			return builder.AppendLine();
		}

		public static void BlockCopy(byte[] from, int fromIndex, byte[] to, int toIndex, int count)
		{
			Buffer.BlockCopy(from, fromIndex, to, toIndex, count);
		}

		[Conditional("DEBUG")]
		public static void DebugAssert(bool condition, string message)
		{
		}

		[Conditional("DEBUG")]
		public static void DebugAssert(bool condition, string message, params object[] args)
		{
		}

		[Conditional("DEBUG")]
		public static void DebugAssert(bool condition)
		{
		}

		[Conditional("DEBUG")]
		public static void DebugWriteLine(string message, object obj)
		{
		}

		[Conditional("DEBUG")]
		public static void DebugWriteLine(string message)
		{
		}

		internal static ConstructorInfo GetConstructor(Type type, Type[] parameterTypes, bool nonPublic)
		{
			return type.GetConstructor((nonPublic ? BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic : BindingFlags.Instance | BindingFlags.Public), null, parameterTypes, null);
		}

		internal static ConstructorInfo[] GetConstructors(Type type, bool nonPublic)
		{
			return type.GetConstructors((nonPublic ? BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic : BindingFlags.Instance | BindingFlags.Public));
		}

		internal static MethodInfo GetGetMethod(PropertyInfo property, bool nonPublic, bool allowInternal)
		{
			if (property == null)
			{
				return null;
			}
			MethodInfo getMethod = property.GetGetMethod(nonPublic);
			if (getMethod == null && !nonPublic && allowInternal)
			{
				getMethod = property.GetGetMethod(true);
				if (getMethod == null && !getMethod.IsAssembly && !getMethod.IsFamilyOrAssembly)
				{
					getMethod = null;
				}
			}
			return getMethod;
		}

		internal static MemberInfo[] GetInstanceFieldsAndProperties(Type type, bool publicOnly)
		{
			BindingFlags bindingFlag = (publicOnly ? BindingFlags.Instance | BindingFlags.Public : BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			PropertyInfo[] properties = type.GetProperties(bindingFlag);
			FieldInfo[] fields = type.GetFields(bindingFlag);
			MemberInfo[] memberInfoArray = new MemberInfo[(int)fields.Length + (int)properties.Length];
			properties.CopyTo(memberInfoArray, 0);
			fields.CopyTo(memberInfoArray, (int)properties.Length);
			return memberInfoArray;
		}

		internal static MethodInfo GetInstanceMethod(Type declaringType, string name)
		{
			return declaringType.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		internal static MethodInfo GetInstanceMethod(Type declaringType, string name, Type[] types)
		{
			if (types == null)
			{
				types = Helpers.EmptyTypes;
			}
			return declaringType.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, types, null);
		}

		internal static Type GetMemberType(MemberInfo member)
		{
			MemberTypes memberType = member.MemberType;
			if (memberType == MemberTypes.Field)
			{
				return ((FieldInfo)member).FieldType;
			}
			if (memberType != MemberTypes.Property)
			{
				return null;
			}
			return ((PropertyInfo)member).PropertyType;
		}

		internal static PropertyInfo GetProperty(Type type, string name, bool nonPublic)
		{
			return type.GetProperty(name, (nonPublic ? BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic : BindingFlags.Instance | BindingFlags.Public));
		}

		internal static MethodInfo GetSetMethod(PropertyInfo property, bool nonPublic, bool allowInternal)
		{
			if (property == null)
			{
				return null;
			}
			MethodInfo setMethod = property.GetSetMethod(nonPublic);
			if (setMethod == null && !nonPublic && allowInternal)
			{
				setMethod = property.GetGetMethod(true);
				if (setMethod == null && !setMethod.IsAssembly && !setMethod.IsFamilyOrAssembly)
				{
					setMethod = null;
				}
			}
			return setMethod;
		}

		internal static MethodInfo GetStaticMethod(Type declaringType, string name)
		{
			return declaringType.GetMethod(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public static ProtoTypeCode GetTypeCode(Type type)
		{
			TypeCode typeCode = Type.GetTypeCode(type);
			switch (typeCode)
			{
				case TypeCode.Empty:
				case TypeCode.Boolean:
				case TypeCode.Char:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				case TypeCode.DateTime:
				case TypeCode.String:
				{
					return (ProtoTypeCode)typeCode;
				}
				case TypeCode.Object:
				case TypeCode.DBNull:
				case TypeCode.Object | TypeCode.DateTime:
				{
					if (type == typeof(TimeSpan))
					{
						return ProtoTypeCode.TimeSpan;
					}
					if (type == typeof(Guid))
					{
						return ProtoTypeCode.Guid;
					}
					if (type == typeof(Uri))
					{
						return ProtoTypeCode.Uri;
					}
					if (type == typeof(byte[]))
					{
						return ProtoTypeCode.ByteArray;
					}
					if (type == typeof(Type))
					{
						return ProtoTypeCode.Type;
					}
					return ProtoTypeCode.Unknown;
				}
				default:
				{
					if (type == typeof(TimeSpan))
					{
						return ProtoTypeCode.TimeSpan;
					}
					if (type == typeof(Guid))
					{
						return ProtoTypeCode.Guid;
					}
					if (type == typeof(Uri))
					{
						return ProtoTypeCode.Uri;
					}
					if (type == typeof(byte[]))
					{
						return ProtoTypeCode.ByteArray;
					}
					if (type == typeof(Type))
					{
						return ProtoTypeCode.Type;
					}
					return ProtoTypeCode.Unknown;
				}
			}
		}

		internal static Type GetUnderlyingType(Type type)
		{
			return Nullable.GetUnderlyingType(type);
		}

		internal static bool IsAssignableFrom(Type target, Type type)
		{
			return target.IsAssignableFrom(type);
		}

		internal static bool IsEnum(Type type)
		{
			return type.IsEnum;
		}

		public static bool IsInfinity(float value)
		{
			return float.IsInfinity(value);
		}

		public static bool IsInfinity(double value)
		{
			return double.IsInfinity(value);
		}

		public static bool IsNullOrEmpty(string value)
		{
			if (value == null)
			{
				return true;
			}
			return value.Length == 0;
		}

		internal static bool IsSubclassOf(Type type, Type baseClass)
		{
			return type.IsSubclassOf(baseClass);
		}

		internal static bool IsValueType(Type type)
		{
			return type.IsValueType;
		}

		internal static object ParseEnum(Type type, string value)
		{
			return Enum.Parse(type, value, true);
		}

		public static void Sort(int[] keys, object[] values)
		{
			bool flag;
			do
			{
				flag = false;
				for (int i = 1; i < (int)keys.Length; i++)
				{
					if (keys[i - 1] > keys[i])
					{
						int num = keys[i];
						keys[i] = keys[i - 1];
						keys[i - 1] = num;
						object obj = values[i];
						values[i] = values[i - 1];
						values[i - 1] = obj;
						flag = true;
					}
				}
			}
			while (flag);
		}

		[Conditional("TRACE")]
		public static void TraceWriteLine(string message)
		{
			Trace.WriteLine(message);
		}
	}
}