using Oxide.Core.Libraries.Covalence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Oxide.Core
{
	public static class ExtensionMethods
	{
		public static string Basename(this string text, string extension = null)
		{
			if (extension != null)
			{
				if (!extension.Equals("*.*"))
				{
					if (extension[0] == '*')
					{
						extension = extension.Substring(1);
					}
					return Regex.Match(text, string.Concat("([^\\\\/]+)\\", extension, "+$")).Groups[1].Value;
				}
				Match match = Regex.Match(text, "([^\\\\/]+)\\.[^\\.]+$");
				if (match.Success)
				{
					return match.Groups[1].Value;
				}
			}
			return Regex.Match(text, "[^\\\\/]+$").Groups[0].Value;
		}

		public static bool Contains<T>(this T[] array, T value)
		{
			T[] tArray = array;
			for (int i = 0; i < (int)tArray.Length; i++)
			{
				if (tArray[i].Equals(value))
				{
					return true;
				}
			}
			return false;
		}

		public static string Dirname(this string text)
		{
			return Regex.Match(text, "(.+)[\\/][^\\/]+$").Groups[1].Value;
		}

		public static string Humanize(this string name)
		{
			return Regex.Replace(name, "(\\B[A-Z])", " $1");
		}

		public static bool IsSteamId(this string id)
		{
			ulong num;
			if (!ulong.TryParse(id, out num))
			{
				return false;
			}
			return num > 76561197960265728L;
		}

		public static bool IsSteamId(this ulong id)
		{
			return id > 76561197960265728L;
		}

		public static string Plaintext(this string text)
		{
			return Formatter.ToPlaintext(text);
		}

		public static string Quote(this string text)
		{
			return text.QuoteSafe();
		}

		public static string QuoteSafe(this string text)
		{
			return string.Concat("\"", text.Replace("\"", "\\\"").TrimEnd(new char[] { '\\' }), "\"");
		}

		public static T Sample<T>(this T[] array)
		{
			return array[Oxide.Core.Random.Range(0, (int)array.Length)];
		}

		public static string Sanitize(this string text)
		{
			return text.Replace("{", "{{").Replace("}", "}}");
		}

		public static string SentenceCase(this string text)
		{
			return (new Regex("(^[a-z])|\\.\\s+(.)", RegexOptions.ExplicitCapture)).Replace(text.ToLower(), (Match s) => s.Value.ToUpper());
		}

		public static string TitleCase(this string text)
		{
			return CultureInfo.InstalledUICulture.TextInfo.ToTitleCase((text.Contains<char>('\u005F') ? text.Replace('\u005F', ' ') : text));
		}

		public static string Titleize(this string text)
		{
			return text.TitleCase();
		}

		public static string ToSentence<T>(this IEnumerable<T> items)
		{
			string str;
			IEnumerator<T> enumerator = items.GetEnumerator();
			if (!enumerator.MoveNext())
			{
				return string.Empty;
			}
			T current = enumerator.Current;
			if (!enumerator.MoveNext())
			{
				if (current == null)
				{
					return null;
				}
				return current.ToString();
			}
			if (current != null)
			{
				str = current.ToString();
			}
			else
			{
				str = null;
			}
			StringBuilder stringBuilder = new StringBuilder(str);
			bool flag = true;
			while (flag)
			{
				T t = enumerator.Current;
				flag = enumerator.MoveNext();
				stringBuilder.Append((flag ? ", " : " and "));
				stringBuilder.Append(t);
			}
			return stringBuilder.ToString();
		}

		public static string Truncate(this string text, int max)
		{
			if (text.Length <= max)
			{
				return text;
			}
			return string.Concat(text.Substring(0, max), " ...");
		}
	}
}