using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Facepunch.Extend
{
	public static class StringExtensions
	{
		private static char[] FilenameDelim;

		private readonly static char[] _badCharacters;

		static StringExtensions()
		{
			Facepunch.Extend.StringExtensions.FilenameDelim = new char[] { '/', '\\' };
			Facepunch.Extend.StringExtensions._badCharacters = new char[] { '\0', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', '\u0006', '\a', '\b', '\t', '\v', '\f', '\r', '\u000E', '\u000F', '\u0010', '\u0012', '\u0013', '\u0014', '\u0016', '\u0017', '\u0018', '\u0019', '\u001A', '\u001B', '\u001C', '\u001D', '\u001E', '\u001F', '\u00A0', '­', '\u2000', '\u2001', '\u2002', '\u2003', '\u2004', '\u2005', '\u2006', '\u2007', '\u2008', '\u2009', '\u200A', '\u200B', '\u200C', '\u200D', '\u200E', '\u200F', '‐', '‑', '‒', '–', '—', '―', '‖', '‗', '‘', '’', '‚', '‛', '“', '”', '„', '‟', '\u2028', '\u2029', '\u202F', '\u205F', '\u2060', '\u2420', '\u2422', '\u2423', '\u3000', '\uFEFF' };
		}

		public static string Base64Decode(this string base64EncodedData)
		{
			byte[] numArray = Convert.FromBase64String(base64EncodedData);
			return Encoding.UTF8.GetString(numArray);
		}

		public static string Base64Encode(this string plainText)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
		}

		public static bool Contains(this string source, string toCheck, StringComparison comp)
		{
			return source.IndexOf(toCheck, comp) >= 0;
		}

		public static string QuoteSafe(this string str)
		{
			str = str.Replace("\"", "\\\"").TrimEnd(new char[] { '\\' });
			return string.Concat("\"", str, "\"");
		}

		public static string RemoveBadCharacters(this string str)
		{
			str = new string((
				from x in str
				where !Facepunch.Extend.StringExtensions._badCharacters.Contains<char>(x)
				select x).ToArray<char>());
			return str;
		}

		public static string Snippet(this string source, string find, int padding)
		{
			if (string.IsNullOrEmpty(find))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < source.Length; i += find.Length)
			{
				i = source.IndexOf(find, i, StringComparison.InvariantCultureIgnoreCase);
				if (i == -1)
				{
					break;
				}
				int num = (i - padding).Clamp<int>(0, source.Length);
				int num1 = (num + find.Length + padding * 2).Clamp<int>(0, source.Length);
				i = num1;
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" ... ");
				}
				stringBuilder.Append(source.Substring(num, num1 - num));
			}
			return stringBuilder.ToString();
		}

		public static string[] SplitQuotesStrings(this string input)
		{
			input = input.Replace("\\\"", "&qute;");
			MatchCollection matchCollections = (new Regex("\"([^\"]+)\"|'([^']+)'|\\S+")).Matches(input);
			string[] strArrays = new string[matchCollections.Count];
			for (int i = 0; i < matchCollections.Count; i++)
			{
				strArrays[i] = matchCollections[i].Groups[0].Value.Trim(new char[] { ' ', '\"' });
				strArrays[i] = strArrays[i].Replace("&qute;", "\"");
			}
			return strArrays;
		}

		public static bool ToBool(this string str)
		{
			if (str == null)
			{
				return false;
			}
			if (str == "1")
			{
				return true;
			}
			str = str.Trim();
			str = str.ToLower();
			if (str == "true")
			{
				return true;
			}
			if (str == "t")
			{
				return true;
			}
			if (str == "yes")
			{
				return true;
			}
			if (str == "y")
			{
				return true;
			}
			return false;
		}

		public static decimal ToDecimal(this string str, [DecimalConstant(0, 0, 0, 0, 0)] decimal Default = default(decimal))
		{
			decimal @default = Default;
			decimal.TryParse(str, out @default);
			return @default;
		}

		public static float ToFloat(this string str, float Default = 0f)
		{
			return (float)((float)str.ToDecimal((decimal)Default));
		}

		public static int ToInt(this string str, int Default = 0)
		{
			decimal num = str.ToDecimal(Default);
			if (num <= new decimal(-2147483648))
			{
				return -2147483648;
			}
			if (num >= new decimal(2147483647))
			{
				return 2147483647;
			}
			return (int)num;
		}

		public static string Truncate(this string str, int maxLength, string appendage = null)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}
			if (str.Length <= maxLength)
			{
				return str;
			}
			if (appendage != null)
			{
				maxLength -= appendage.Length;
			}
			str = str.Substring(0, maxLength);
			if (appendage == null)
			{
				return str;
			}
			return string.Concat(str, appendage);
		}

		public static string TruncateFilename(this string str, int maxLength, string appendage = null)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}
			if (str.Length <= maxLength)
			{
				return str;
			}
			maxLength -= 3;
			string str1 = str;
			int num = 0;
			do
			{
				int num1 = num;
				num = num1 + 1;
				if (num1 >= 100)
				{
					return str.Split(Facepunch.Extend.StringExtensions.FilenameDelim).ToList<string>().Last<string>();
				}
				List<string> list = str.Split(Facepunch.Extend.StringExtensions.FilenameDelim).ToList<string>();
				list.RemoveRange(list.Count - 1 - num, num);
				if (list.Count == 1)
				{
					return list.Last<string>();
				}
				list.Insert(list.Count - 1, "...");
				str1 = string.Join("/", list.ToArray());
			}
			while (str1.Length >= maxLength);
			return str1;
		}
	}
}