using Newtonsoft.Json.Shims;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal static class EnumUtils
	{
		private readonly static ThreadSafeStore<Type, BidirectionalDictionary<string, string>> EnumMemberNamesPerType;

		static EnumUtils()
		{
			EnumUtils.EnumMemberNamesPerType = new ThreadSafeStore<Type, BidirectionalDictionary<string, string>>(new Func<Type, BidirectionalDictionary<string, string>>(EnumUtils.InitializeEnumType));
		}

		public static IList<T> GetFlagsValues<T>(T value)
		where T : struct
		{
			Type type = typeof(T);
			if (!type.IsDefined(typeof(FlagsAttribute), false))
			{
				throw new ArgumentException("Enum type {0} is not a set of flags.".FormatWith(CultureInfo.InvariantCulture, type));
			}
			Type underlyingType = Enum.GetUnderlyingType(value.GetType());
			ulong num = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
			IList<EnumValue<ulong>> namesAndValues = EnumUtils.GetNamesAndValues<T>();
			IList<T> ts = new List<T>();
			foreach (EnumValue<ulong> namesAndValue in namesAndValues)
			{
				if ((num & namesAndValue.Value) != namesAndValue.Value || namesAndValue.Value == 0)
				{
					continue;
				}
				ts.Add((T)Convert.ChangeType(namesAndValue.Value, underlyingType, CultureInfo.CurrentCulture));
			}
			if (ts.Count == 0)
			{
				if (namesAndValues.SingleOrDefault<EnumValue<ulong>>((EnumValue<ulong> v) => v.Value == (long)0) != null)
				{
					ts.Add(default(T));
				}
			}
			return ts;
		}

		public static IList<string> GetNames(Type enumType)
		{
			if (!enumType.IsEnum())
			{
				throw new ArgumentException(string.Concat("Type '", enumType.Name, "' is not an enum."));
			}
			List<string> strs = new List<string>();
			foreach (FieldInfo fieldInfo in 
				from f in (IEnumerable<FieldInfo>)enumType.GetFields()
				where f.IsLiteral
				select f)
			{
				strs.Add(fieldInfo.Name);
			}
			return strs;
		}

		public static IList<EnumValue<ulong>> GetNamesAndValues<T>()
		where T : struct
		{
			return EnumUtils.GetNamesAndValues<ulong>(typeof(T));
		}

		public static IList<EnumValue<TUnderlyingType>> GetNamesAndValues<TUnderlyingType>(Type enumType)
		where TUnderlyingType : struct
		{
			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}
			if (!enumType.IsEnum())
			{
				throw new ArgumentException("Type {0} is not an Enum.".FormatWith(CultureInfo.InvariantCulture, enumType), "enumType");
			}
			IList<object> values = EnumUtils.GetValues(enumType);
			IList<string> names = EnumUtils.GetNames(enumType);
			IList<EnumValue<TUnderlyingType>> enumValues = new List<EnumValue<TUnderlyingType>>();
			for (int i = 0; i < values.Count; i++)
			{
				try
				{
					enumValues.Add(new EnumValue<TUnderlyingType>(names[i], (TUnderlyingType)Convert.ChangeType(values[i], typeof(TUnderlyingType), CultureInfo.CurrentCulture)));
				}
				catch (OverflowException overflowException1)
				{
					OverflowException overflowException = overflowException1;
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Value from enum with the underlying type of {0} cannot be added to dictionary with a value type of {1}. Value was too large: {2}", new object[] { Enum.GetUnderlyingType(enumType), typeof(TUnderlyingType), Convert.ToUInt64(values[i], CultureInfo.InvariantCulture) }), overflowException);
				}
			}
			return enumValues;
		}

		public static IList<object> GetValues(Type enumType)
		{
			if (!enumType.IsEnum())
			{
				throw new ArgumentException(string.Concat("Type '", enumType.Name, "' is not an enum."));
			}
			List<object> objs = new List<object>();
			foreach (FieldInfo fieldInfo in 
				from f in (IEnumerable<FieldInfo>)enumType.GetFields()
				where f.IsLiteral
				select f)
			{
				objs.Add(fieldInfo.GetValue(enumType));
			}
			return objs;
		}

		private static BidirectionalDictionary<string, string> InitializeEnumType(Type type)
		{
			string str;
			BidirectionalDictionary<string, string> bidirectionalDictionary = new BidirectionalDictionary<string, string>(StringComparer.OrdinalIgnoreCase, StringComparer.OrdinalIgnoreCase);
			FieldInfo[] fields = type.GetFields();
			for (int i = 0; i < (int)fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				string name = fieldInfo.Name;
				string str1 = (
					from EnumMemberAttribute a in fieldInfo.GetCustomAttributes(typeof(EnumMemberAttribute), true)
					select a.Value).SingleOrDefault<string>() ?? fieldInfo.Name;
				if (bidirectionalDictionary.TryGetBySecond(str1, out str))
				{
					throw new InvalidOperationException("Enum name '{0}' already exists on enum '{1}'.".FormatWith(CultureInfo.InvariantCulture, str1, type.Name));
				}
				bidirectionalDictionary.Set(name, str1);
			}
			return bidirectionalDictionary;
		}

		public static object ParseEnumName(string enumText, bool isNullable, Type t)
		{
			string str;
			if ((enumText == string.Empty) & isNullable)
			{
				return null;
			}
			BidirectionalDictionary<string, string> bidirectionalDictionary = EnumUtils.EnumMemberNamesPerType.Get(t);
			if (enumText.IndexOf(',') == -1)
			{
				str = EnumUtils.ResolvedEnumName(bidirectionalDictionary, enumText);
			}
			else
			{
				string[] strArrays = enumText.Split(new char[] { ',' });
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					string str1 = strArrays[i].Trim();
					strArrays[i] = EnumUtils.ResolvedEnumName(bidirectionalDictionary, str1);
				}
				str = string.Join(", ", strArrays);
			}
			return Enum.Parse(t, str, true);
		}

		private static string ResolvedEnumName(BidirectionalDictionary<string, string> map, string enumText)
		{
			string str;
			map.TryGetBySecond(enumText, out str);
			str = str ?? enumText;
			return str;
		}

		public static string ToEnumName(Type enumType, string enumText, bool camelCaseText)
		{
			string camelCase;
			BidirectionalDictionary<string, string> bidirectionalDictionary = EnumUtils.EnumMemberNamesPerType.Get(enumType);
			string[] strArrays = enumText.Split(new char[] { ',' });
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i].Trim();
				bidirectionalDictionary.TryGetByFirst(str, out camelCase);
				camelCase = camelCase ?? str;
				if (camelCaseText)
				{
					camelCase = StringUtils.ToCamelCase(camelCase);
				}
				strArrays[i] = camelCase;
			}
			return string.Join(", ", strArrays);
		}
	}
}