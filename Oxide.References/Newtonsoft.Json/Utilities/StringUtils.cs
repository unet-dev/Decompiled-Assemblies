using Newtonsoft.Json.Shims;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal static class StringUtils
	{
		public const string CarriageReturnLineFeed = "\r\n";

		public const string Empty = "";

		public const char CarriageReturn = '\r';

		public const char LineFeed = '\n';

		public const char Tab = '\t';

		public static StringWriter CreateStringWriter(int capacity)
		{
			return new StringWriter(new StringBuilder(capacity), CultureInfo.InvariantCulture);
		}

		public static bool EndsWith(this string source, char value)
		{
			if (source.Length <= 0)
			{
				return false;
			}
			return source[source.Length - 1] == value;
		}

		public static TSource ForgivingCaseSensitiveFind<TSource>(this IEnumerable<TSource> source, Func<TSource, string> valueSelector, string testValue)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (valueSelector == null)
			{
				throw new ArgumentNullException("valueSelector");
			}
			IEnumerable<TSource> tSources = 
				from  in source
				where string.Equals(valueSelector(s), testValue, StringComparison.OrdinalIgnoreCase)
				select ;
			if (tSources.Count<TSource>() <= 1)
			{
				return tSources.SingleOrDefault<TSource>();
			}
			return (
				from  in source
				where string.Equals(valueSelector(s), testValue, StringComparison.Ordinal)
				select ).SingleOrDefault<TSource>();
		}

		public static string FormatWith(this string format, IFormatProvider provider, object arg0)
		{
			return format.FormatWith(provider, new object[] { arg0 });
		}

		public static string FormatWith(this string format, IFormatProvider provider, object arg0, object arg1)
		{
			return format.FormatWith(provider, new object[] { arg0, arg1 });
		}

		public static string FormatWith(this string format, IFormatProvider provider, object arg0, object arg1, object arg2)
		{
			return format.FormatWith(provider, new object[] { arg0, arg1, arg2 });
		}

		public static string FormatWith(this string format, IFormatProvider provider, object arg0, object arg1, object arg2, object arg3)
		{
			return format.FormatWith(provider, new object[] { arg0, arg1, arg2, arg3 });
		}

		private static string FormatWith(this string format, IFormatProvider provider, params object[] args)
		{
			ValidationUtils.ArgumentNotNull(format, "format");
			return string.Format(provider, format, args);
		}

		public static int? GetLength(string value)
		{
			if (value == null)
			{
				return null;
			}
			return new int?(value.Length);
		}

		public static bool IsHighSurrogate(char c)
		{
			return char.IsHighSurrogate(c);
		}

		public static bool IsLowSurrogate(char c)
		{
			return char.IsLowSurrogate(c);
		}

		public static bool IsWhiteSpace(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (s.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < s.Length; i++)
			{
				if (!char.IsWhiteSpace(s[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static string NullEmptyString(string s)
		{
			if (!string.IsNullOrEmpty(s))
			{
				return s;
			}
			return null;
		}

		public static bool StartsWith(this string source, char value)
		{
			if (source.Length <= 0)
			{
				return false;
			}
			return source[0] == value;
		}

		public static string ToCamelCase(string s)
		{
			if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
			{
				return s;
			}
			char[] charArray = s.ToCharArray();
			for (int i = 0; i < (int)charArray.Length && (i != 1 || char.IsUpper(charArray[i])) && (!(i > 0 & i + 1 < (int)charArray.Length) || char.IsUpper(charArray[i + 1])); i++)
			{
				charArray[i] = char.ToLower(charArray[i], CultureInfo.InvariantCulture);
			}
			return new string(charArray);
		}

		public static void ToCharAsUnicode(char c, char[] buffer)
		{
			buffer[0] = '\\';
			buffer[1] = 'u';
			buffer[2] = MathUtils.IntToHex(c >> '\f' & 15);
			buffer[3] = MathUtils.IntToHex(c >> '\b' & 15);
			buffer[4] = MathUtils.IntToHex(c >> '\u0004' & 15);
			buffer[5] = MathUtils.IntToHex(c & '\u000F');
		}
	}
}