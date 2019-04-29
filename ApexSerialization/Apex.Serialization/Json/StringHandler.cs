using System;
using System.Text;

namespace Apex.Serialization.Json
{
	internal static class StringHandler
	{
		internal static void EscapeString(string s, StringBuilder b)
		{
			b.EnsureCapacity(s.Length);
			int num = 0;
			string str = null;
			char[] chrArray = null;
			bool flag = false;
			int length = s.Length;
			for (int i = 0; i < length; i++)
			{
				char chr = s[i];
				if (chr < ' ')
				{
					if (chr == 0)
					{
						str = "\\0";
					}
					else
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
								flag = true;
								break;
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
								goto case '\v';
							}
						}
					}
				}
				else if (chr > '~')
				{
					flag = true;
				}
				else if (chr == '\"')
				{
					str = "\\\"";
				}
				else if (chr == '\\')
				{
					str = "\\\\";
				}
				if (str != null | flag)
				{
					if (i > num)
					{
						b.Append(s, num, i - num);
					}
					num = i + 1;
					if (!flag)
					{
						b.Append(str);
						str = null;
					}
					else
					{
						flag = false;
						if (chrArray == null)
						{
							chrArray = new char[6];
						}
						StringHandler.ToCharAsUnicode(chr, chrArray);
						b.Append(chrArray);
					}
				}
			}
			if (num < length)
			{
				b.Append(s, num, length - num);
			}
		}

		private static char IntToHex(int n)
		{
			if (n <= 9)
			{
				return (char)(n + 48);
			}
			return (char)(n - 10 + 97);
		}

		private static void ToCharAsUnicode(char c, char[] buffer)
		{
			buffer[0] = '\\';
			buffer[1] = 'u';
			buffer[2] = StringHandler.IntToHex(c >> '\f' & 15);
			buffer[3] = StringHandler.IntToHex(c >> '\b' & 15);
			buffer[4] = StringHandler.IntToHex(c >> '\u0004' & 15);
			buffer[5] = StringHandler.IntToHex(c & '\u000F');
		}
	}
}