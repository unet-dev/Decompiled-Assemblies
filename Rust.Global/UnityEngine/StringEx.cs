using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace UnityEngine
{
	public static class StringEx
	{
		public static bool Contains(this string haystack, string needle, CompareOptions options)
		{
			return CultureInfo.InvariantCulture.CompareInfo.IndexOf(haystack, needle, options) >= 0;
		}

		public static string EscapeRichText(this string str)
		{
			if (str.Contains("<"))
			{
				str = str.Replace("<", "<​");
			}
			if (str.Contains(">"))
			{
				str = str.Replace(">", "​>");
			}
			return str;
		}

		public static bool IsLower(this string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				if (char.IsUpper(str[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static uint ManifestHash(this string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return (uint)0;
			}
			if (!str.IsLower())
			{
				str = str.ToLower();
			}
			return BitConverter.ToUInt32((new MD5CryptoServiceProvider()).ComputeHash(Encoding.UTF8.GetBytes(str)), 0);
		}

		public static string Replace(this string originalString, string oldValue, string newValue, StringComparison comparisonType)
		{
			int length = 0;
			while (true)
			{
				length = originalString.IndexOf(oldValue, length, comparisonType);
				if (length == -1)
				{
					break;
				}
				originalString = string.Concat(originalString.Substring(0, length), newValue, originalString.Substring(length + oldValue.Length));
				length += newValue.Length;
			}
			return originalString;
		}

		public static IEnumerable<string> SplitToChunks(string str, int chunkLength)
		{
			if (string.IsNullOrEmpty(str))
			{
				throw new ArgumentException("string cannot be null");
			}
			if (chunkLength < 1)
			{
				throw new ArgumentException("chunk length needs to be more than 0");
			}
			for (int i = 0; i < str.Length; i += chunkLength)
			{
				if (chunkLength + i >= str.Length)
				{
					chunkLength = str.Length - i;
				}
				yield return str.Substring(i, chunkLength);
			}
		}

		public static IEnumerable<string> SplitToLines(string input)
		{
			if (input != null)
			{
				using (StringReader stringReader = new StringReader(input))
				{
					while (true)
					{
						string str = stringReader.ReadLine();
						string str1 = str;
						if (str == null)
						{
							break;
						}
						yield return str1;
					}
				}
				stringReader = null;
			}
			else
			{
			}
		}

		public static string ToPrintable(this string str, int maxLength = 2147483647)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(str))
			{
				int num = Mathf.Min(str.Length, maxLength);
				for (int i = 0; i < num; i++)
				{
					char chr = str[i];
					if (!char.IsControl(chr))
					{
						stringBuilder.Append(chr);
					}
				}
			}
			return stringBuilder.ToString();
		}
	}
}