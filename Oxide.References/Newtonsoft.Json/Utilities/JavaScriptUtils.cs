using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal static class JavaScriptUtils
	{
		internal readonly static bool[] SingleQuoteCharEscapeFlags;

		internal readonly static bool[] DoubleQuoteCharEscapeFlags;

		internal readonly static bool[] HtmlCharEscapeFlags;

		private const int UnicodeTextLength = 6;

		private const string EscapedUnicodeText = "!";

		static JavaScriptUtils()
		{
			JavaScriptUtils.SingleQuoteCharEscapeFlags = new bool[128];
			JavaScriptUtils.DoubleQuoteCharEscapeFlags = new bool[128];
			JavaScriptUtils.HtmlCharEscapeFlags = new bool[128];
			IList<char> chrs = new List<char>()
			{
				'\n',
				'\r',
				'\t',
				'\\',
				'\f',
				'\b'
			};
			for (int i = 0; i < 32; i++)
			{
				chrs.Add((char)i);
			}
			foreach (char chr in chrs.Union<char>((IEnumerable<char>)(new char[] { '\'' })))
			{
				JavaScriptUtils.SingleQuoteCharEscapeFlags[chr] = true;
			}
			foreach (char chr1 in chrs.Union<char>((IEnumerable<char>)(new char[] { '\"' })))
			{
				JavaScriptUtils.DoubleQuoteCharEscapeFlags[chr1] = true;
			}
			foreach (char chr2 in chrs.Union<char>((IEnumerable<char>)(new char[] { '\"', '\'', '<', '>', '&' })))
			{
				JavaScriptUtils.HtmlCharEscapeFlags[chr2] = true;
			}
		}

		public static bool[] GetCharEscapeFlags(StringEscapeHandling stringEscapeHandling, char quoteChar)
		{
			if (stringEscapeHandling == StringEscapeHandling.EscapeHtml)
			{
				return JavaScriptUtils.HtmlCharEscapeFlags;
			}
			if (quoteChar == '\"')
			{
				return JavaScriptUtils.DoubleQuoteCharEscapeFlags;
			}
			return JavaScriptUtils.SingleQuoteCharEscapeFlags;
		}

		public static bool ShouldEscapeJavaScriptString(string s, bool[] charEscapeFlags)
		{
			if (s == null)
			{
				return false;
			}
			string str = s;
			for (int i = 0; i < str.Length; i++)
			{
				char chr = str[i];
				if (chr >= (char)((int)charEscapeFlags.Length) || charEscapeFlags[chr])
				{
					return true;
				}
			}
			return false;
		}

		public static string ToEscapedJavaScriptString(string value, char delimiter, bool appendDelimiters, StringEscapeHandling stringEscapeHandling)
		{
			string str;
			bool[] charEscapeFlags = JavaScriptUtils.GetCharEscapeFlags(stringEscapeHandling, delimiter);
			int? length = StringUtils.GetLength(value);
			using (StringWriter stringWriter = StringUtils.CreateStringWriter((length.HasValue ? length.GetValueOrDefault() : 16)))
			{
				char[] chrArray = null;
				JavaScriptUtils.WriteEscapedJavaScriptString(stringWriter, value, delimiter, appendDelimiters, charEscapeFlags, stringEscapeHandling, null, ref chrArray);
				str = stringWriter.ToString();
			}
			return str;
		}

		public static void WriteEscapedJavaScriptString(TextWriter writer, string s, char delimiter, bool appendDelimiters, bool[] charEscapeFlags, StringEscapeHandling stringEscapeHandling, IArrayPool<char> bufferPool, ref char[] writeBuffer)
		{
			char chr;
			string str;
			if (appendDelimiters)
			{
				writer.Write(delimiter);
			}
			if (s != null)
			{
				int num = 0;
				for (int i = 0; i < s.Length; i++)
				{
					chr = s[i];
					if (chr >= (char)((int)charEscapeFlags.Length) || charEscapeFlags[chr])
					{
						if (chr <= '\\')
						{
							switch (chr)
							{
								case '\b':
								{
									str = "\\b";
									break;
								}
								case '\t':
								{
									str = "\\t";
									break;
								}
								case '\n':
								{
									str = "\\n";
									break;
								}
								case '\v':
								{
									goto Label0;
								}
								case '\f':
								{
									str = "\\f";
									break;
								}
								case '\r':
								{
									str = "\\r";
									break;
								}
								default:
								{
									if (chr == '\\')
									{
										str = "\\\\";
										break;
									}
									else
									{
										goto Label0;
									}
								}
							}
						}
						else if (chr == '\u0085')
						{
							str = "\\u0085";
						}
						else if (chr == '\u2028')
						{
							str = "\\u2028";
						}
						else
						{
							if (chr != '\u2029')
							{
								goto Label0;
							}
							str = "\\u2029";
						}
					Label2:
						if (str != null)
						{
							bool flag = string.Equals(str, "!");
							if (i > num)
							{
								int num1 = i - num + (flag ? 6 : 0);
								int num2 = (flag ? 6 : 0);
								if (writeBuffer == null || (int)writeBuffer.Length < num1)
								{
									char[] chrArray = BufferUtils.RentBuffer(bufferPool, num1);
									if (flag)
									{
										Array.Copy(writeBuffer, chrArray, 6);
									}
									BufferUtils.ReturnBuffer(bufferPool, writeBuffer);
									writeBuffer = chrArray;
								}
								s.CopyTo(num, writeBuffer, num2, num1 - num2);
								writer.Write(writeBuffer, num2, num1 - num2);
							}
							num = i + 1;
							if (flag)
							{
								writer.Write(writeBuffer, 0, 6);
							}
							else
							{
								writer.Write(str);
							}
						}
					}
				}
				if (num != 0)
				{
					int length = s.Length - num;
					if (writeBuffer == null || (int)writeBuffer.Length < length)
					{
						writeBuffer = BufferUtils.EnsureBufferSize(bufferPool, length, writeBuffer);
					}
					s.CopyTo(num, writeBuffer, 0, length);
					writer.Write(writeBuffer, 0, length);
				}
				else
				{
					writer.Write(s);
				}
			}
			if (appendDelimiters)
			{
				writer.Write(delimiter);
			}
			return;
			if (chr >= (char)((int)charEscapeFlags.Length) && stringEscapeHandling != StringEscapeHandling.EscapeNonAscii)
			{
				str = null;
				goto Label2;
			}
			else if (chr == '\'' && stringEscapeHandling != StringEscapeHandling.EscapeHtml)
			{
				str = "\\'";
				goto Label2;
			}
			else if (chr != '\"' || stringEscapeHandling == StringEscapeHandling.EscapeHtml)
			{
				if (writeBuffer == null || (int)writeBuffer.Length < 6)
				{
					writeBuffer = BufferUtils.EnsureBufferSize(bufferPool, 6, writeBuffer);
				}
				StringUtils.ToCharAsUnicode(chr, writeBuffer);
				str = "!";
				goto Label2;
			}
			else
			{
				str = "\\\"";
				goto Label2;
			}
		}
	}
}