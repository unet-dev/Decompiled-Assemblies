using System;
using System.Text;

namespace Mono.Unix
{
	[Serializable]
	public class UnixEncoding : Encoding
	{
		public readonly static Encoding Instance;

		public readonly static char EscapeByte;

		static UnixEncoding()
		{
			UnixEncoding.Instance = new UnixEncoding();
			UnixEncoding.EscapeByte = '\0';
		}

		public UnixEncoding()
		{
		}

		private static string _(string arg)
		{
			return arg;
		}

		private static void CopyRaw(byte[] raw, ref int next_raw, char[] chars, ref int posn, int length)
		{
			if (posn + next_raw * 2 > length)
			{
				throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "chars");
			}
			for (int i = 0; i < next_raw; i++)
			{
				int num = posn;
				int num1 = num;
				posn = num + 1;
				chars[num1] = UnixEncoding.EscapeByte;
				int num2 = posn;
				num1 = num2;
				posn = num2 + 1;
				chars[num1] = (char)raw[i];
			}
			next_raw = 0;
		}

		public override bool Equals(object value)
		{
			if (value is UnixEncoding)
			{
				return true;
			}
			return false;
		}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			return UnixEncoding.InternalGetByteCount(chars, index, count, 0, true);
		}

		public override int GetByteCount(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			int num = 0;
			int length = s.Length;
			int num1 = 0;
			while (length > 0)
			{
				int num2 = num;
				num = num2 + 1;
				char chr = s[num2];
				if (chr == UnixEncoding.EscapeByte && length > 1)
				{
					num1++;
					num++;
					length--;
				}
				else if (chr < '\u0080')
				{
					num1++;
				}
				else if (chr < '\u0800')
				{
					num1 += 2;
				}
				else if (chr < '\uD800' || chr > '\uDBFF' || length <= 1)
				{
					num1 += 3;
				}
				else
				{
					uint num3 = s[num];
					if (num3 < 56320 || num3 > 57343)
					{
						num1 += 3;
					}
					else
					{
						num1 += 4;
						num++;
						length--;
					}
				}
				length--;
			}
			return num1;
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			uint num = 0;
			return UnixEncoding.InternalGetBytes(chars, charIndex, charCount, bytes, byteIndex, ref num, true);
		}

		public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			uint num;
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (charIndex < 0 || charIndex > s.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", UnixEncoding._("ArgRange_StringIndex"));
			}
			if (charCount < 0 || charCount > s.Length - charIndex)
			{
				throw new ArgumentOutOfRangeException("charCount", UnixEncoding._("ArgRange_StringRange"));
			}
			if (byteIndex < 0 || byteIndex > (int)bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", UnixEncoding._("ArgRange_Array"));
			}
			int length = (int)bytes.Length;
			int num1 = byteIndex;
			while (charCount > 0)
			{
				int num2 = charIndex;
				charIndex = num2 + 1;
				char chr = s[num2];
				if (chr >= '\uD800' && chr <= '\uDBFF' && charCount > 1)
				{
					num = s[charIndex];
					if (num < 56320 || num > 57343)
					{
						num = chr;
					}
					else
					{
						num = num - 56320 + (chr - 55296 << '\n') + 65536;
						charIndex++;
						charCount--;
					}
				}
				else if (chr != UnixEncoding.EscapeByte || charCount <= 1)
				{
					num = chr;
				}
				else
				{
					if (num1 >= length)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
					}
					charCount -= 2;
					if (charCount >= 0)
					{
						int num3 = num1;
						num1 = num3 + 1;
						int num4 = charIndex;
						charIndex = num4 + 1;
						bytes[num3] = (byte)s[num4];
					}
					continue;
				}
				charCount--;
				if (num < 128)
				{
					if (num1 >= length)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
					}
					int num5 = num1;
					num1 = num5 + 1;
					bytes[num5] = (byte)num;
				}
				else if (num < 2048)
				{
					if (num1 + 2 > length)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
					}
					int num6 = num1;
					num1 = num6 + 1;
					bytes[num6] = (byte)(192 | num >> 6);
					int num7 = num1;
					num1 = num7 + 1;
					bytes[num7] = (byte)(128 | num & 63);
				}
				else if (num >= 65536)
				{
					if (num1 + 4 > length)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
					}
					int num8 = num1;
					num1 = num8 + 1;
					bytes[num8] = (byte)(240 | num >> 18);
					int num9 = num1;
					num1 = num9 + 1;
					bytes[num9] = (byte)(128 | num >> 12 & 63);
					int num10 = num1;
					num1 = num10 + 1;
					bytes[num10] = (byte)(128 | num >> 6 & 63);
					int num11 = num1;
					num1 = num11 + 1;
					bytes[num11] = (byte)(128 | num & 63);
				}
				else
				{
					if (num1 + 3 > length)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
					}
					int num12 = num1;
					num1 = num12 + 1;
					bytes[num12] = (byte)(224 | num >> 12);
					int num13 = num1;
					num1 = num13 + 1;
					bytes[num13] = (byte)(128 | num >> 6 & 63);
					int num14 = num1;
					num1 = num14 + 1;
					bytes[num14] = (byte)(128 | num & 63);
				}
			}
			return num1 - byteIndex;
		}

		public override byte[] GetBytes(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			byte[] numArray = new byte[this.GetByteCount(s)];
			this.GetBytes(s, 0, s.Length, numArray, 0);
			return numArray;
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			return UnixEncoding.InternalGetCharCount(bytes, index, count, 0, 0, true, true);
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			uint num = 0;
			uint num1 = 0;
			return UnixEncoding.InternalGetChars(bytes, byteIndex, byteCount, chars, charIndex, ref num, ref num1, true, true);
		}

		public override Decoder GetDecoder()
		{
			return new UnixEncoding.UnixDecoder();
		}

		public override Encoder GetEncoder()
		{
			return new UnixEncoding.UnixEncoder();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override int GetMaxByteCount(int charCount)
		{
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount", UnixEncoding._("ArgRange_NonNegative"));
			}
			return charCount * 4;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount", UnixEncoding._("ArgRange_NonNegative"));
			}
			return byteCount;
		}

		public override byte[] GetPreamble()
		{
			return new byte[0];
		}

		private static int InternalGetByteCount(char[] chars, int index, int count, uint leftOver, bool flush)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (index < 0 || index > (int)chars.Length)
			{
				throw new ArgumentOutOfRangeException("index", UnixEncoding._("ArgRange_Array"));
			}
			if (count < 0 || count > (int)chars.Length - index)
			{
				throw new ArgumentOutOfRangeException("count", UnixEncoding._("ArgRange_Array"));
			}
			int num = 0;
			uint num1 = leftOver;
			while (count > 0)
			{
				char chr = chars[index];
				if (num1 != 0)
				{
					if (chr < '\uDC00' || chr > '\uDFFF')
					{
						num += 3;
						num1 = 0;
						continue;
					}
					else
					{
						num += 4;
						num1 = 0;
					}
				}
				else if (chr == UnixEncoding.EscapeByte && count > 1)
				{
					num++;
					index++;
					count--;
				}
				else if (chr < '\u0080')
				{
					num++;
				}
				else if (chr < '\u0800')
				{
					num += 2;
				}
				else if (chr < '\uD800' || chr > '\uDBFF')
				{
					num += 3;
				}
				else
				{
					num1 = chr;
				}
				index++;
				count--;
			}
			if (flush && num1 != 0)
			{
				num += 3;
			}
			return num;
		}

		private static int InternalGetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, ref uint leftOver, bool flush)
		{
			uint num;
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (charIndex < 0 || charIndex > (int)chars.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", UnixEncoding._("ArgRange_Array"));
			}
			if (charCount < 0 || charCount > (int)chars.Length - charIndex)
			{
				throw new ArgumentOutOfRangeException("charCount", UnixEncoding._("ArgRange_Array"));
			}
			if (byteIndex < 0 || byteIndex > (int)bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", UnixEncoding._("ArgRange_Array"));
			}
			int length = (int)bytes.Length;
			uint num1 = leftOver;
			int num2 = byteIndex;
			while (charCount > 0)
			{
				int num3 = charIndex;
				charIndex = num3 + 1;
				char chr = chars[num3];
				charCount--;
				if (num1 != 0)
				{
					if (chr < '\uDC00' || chr > '\uDFFF')
					{
						num = num1;
						num1 = 0;
						charIndex--;
						charCount++;
					}
					else
					{
						num = (num1 - 55296 << 10) + (chr - 56320) + 65536;
						num1 = 0;
					}
				}
				else if (chr >= '\uD800' && chr <= '\uDBFF')
				{
					num1 = chr;
					continue;
				}
				else if (chr != UnixEncoding.EscapeByte)
				{
					num = chr;
				}
				else
				{
					if (num2 >= length)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
					}
					int num4 = charCount - 1;
					charCount = num4;
					if (num4 >= 0)
					{
						int num5 = num2;
						num2 = num5 + 1;
						int num6 = charIndex;
						charIndex = num6 + 1;
						bytes[num5] = (byte)chars[num6];
					}
					continue;
				}
				if (num < 128)
				{
					if (num2 >= length)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
					}
					int num7 = num2;
					num2 = num7 + 1;
					bytes[num7] = (byte)num;
				}
				else if (num < 2048)
				{
					if (num2 + 2 > length)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
					}
					int num8 = num2;
					num2 = num8 + 1;
					bytes[num8] = (byte)(192 | num >> 6);
					int num9 = num2;
					num2 = num9 + 1;
					bytes[num9] = (byte)(128 | num & 63);
				}
				else if (num >= 65536)
				{
					if (num2 + 4 > length)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
					}
					int num10 = num2;
					num2 = num10 + 1;
					bytes[num10] = (byte)(240 | num >> 18);
					int num11 = num2;
					num2 = num11 + 1;
					bytes[num11] = (byte)(128 | num >> 12 & 63);
					int num12 = num2;
					num2 = num12 + 1;
					bytes[num12] = (byte)(128 | num >> 6 & 63);
					int num13 = num2;
					num2 = num13 + 1;
					bytes[num13] = (byte)(128 | num & 63);
				}
				else
				{
					if (num2 + 3 > length)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
					}
					int num14 = num2;
					num2 = num14 + 1;
					bytes[num14] = (byte)(224 | num >> 12);
					int num15 = num2;
					num2 = num15 + 1;
					bytes[num15] = (byte)(128 | num >> 6 & 63);
					int num16 = num2;
					num2 = num16 + 1;
					bytes[num16] = (byte)(128 | num & 63);
				}
			}
			if (flush && num1 != 0)
			{
				if (num2 + 3 > length)
				{
					throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
				}
				int num17 = num2;
				num2 = num17 + 1;
				bytes[num17] = (byte)(224 | num1 >> 12);
				int num18 = num2;
				num2 = num18 + 1;
				bytes[num18] = (byte)(128 | num1 >> 6 & 63);
				int num19 = num2;
				num2 = num19 + 1;
				bytes[num19] = (byte)(128 | num1 & 63);
				num1 = 0;
			}
			leftOver = num1;
			return num2 - byteIndex;
		}

		private static int InternalGetCharCount(byte[] bytes, int index, int count, uint leftOverBits, uint leftOverCount, bool throwOnInvalid, bool flush)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (index < 0 || index > (int)bytes.Length)
			{
				throw new ArgumentOutOfRangeException("index", UnixEncoding._("ArgRange_Array"));
			}
			if (count < 0 || count > (int)bytes.Length - index)
			{
				throw new ArgumentOutOfRangeException("count", UnixEncoding._("ArgRange_Array"));
			}
			int num = 0;
			int num1 = 0;
			uint num2 = leftOverBits;
			uint num3 = leftOverCount & 15;
			uint num4 = leftOverCount >> 4 & 15;
			while (count > 0)
			{
				int num5 = index;
				index = num5 + 1;
				uint num6 = bytes[num5];
				num++;
				count--;
				if (num4 != 0)
				{
					if ((num6 & 192) != 128)
					{
						!throwOnInvalid;
						if (num6 < 128)
						{
							index--;
							count++;
							num--;
						}
						num1 = num1 + num * 2;
						num4 = 0;
						num = 0;
					}
					else
					{
						num2 = num2 << 6 | num6 & 63;
						UInt32 num7 = num3 + 1;
						num3 = num7;
						if (num7 >= num4)
						{
							if (num2 < 65536)
							{
								bool flag = false;
								switch (num4)
								{
									case 2:
									{
										flag = num2 <= 127;
										break;
									}
									case 3:
									{
										flag = num2 <= 2047;
										break;
									}
									case 4:
									{
										flag = num2 <= 65535;
										break;
									}
									case 5:
									{
										flag = num2 <= 2097151;
										break;
									}
									case 6:
									{
										flag = num2 <= 67108863;
										break;
									}
								}
								if (!flag)
								{
									num1++;
								}
								else
								{
									num1 = num1 + num * 2;
								}
							}
							else if (num2 < 1114112)
							{
								num1 += 2;
							}
							else if (throwOnInvalid)
							{
								num1 = num1 + num * 2;
							}
							num4 = 0;
							num = 0;
						}
					}
				}
				else if (num6 < 128)
				{
					num1++;
					num = 0;
				}
				else if ((num6 & 224) == 192)
				{
					num2 = num6 & 31;
					num3 = 1;
					num4 = 2;
				}
				else if ((num6 & 240) == 224)
				{
					num2 = num6 & 15;
					num3 = 1;
					num4 = 3;
				}
				else if ((num6 & 248) == 240)
				{
					num2 = num6 & 7;
					num3 = 1;
					num4 = 4;
				}
				else if ((num6 & 252) == 248)
				{
					num2 = num6 & 3;
					num3 = 1;
					num4 = 5;
				}
				else if ((num6 & 254) != 252)
				{
					!throwOnInvalid;
					num1 = num1 + num * 2;
					num = 0;
				}
				else
				{
					num2 = num6 & 3;
					num3 = 1;
					num4 = 6;
				}
			}
			if (flush && num4 != 0 && throwOnInvalid)
			{
				num1 = num1 + num * 2;
			}
			return num1;
		}

		private static int InternalGetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, ref uint leftOverBits, ref uint leftOverCount, bool throwOnInvalid, bool flush)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (byteIndex < 0 || byteIndex > (int)bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", UnixEncoding._("ArgRange_Array"));
			}
			if (byteCount < 0 || byteCount > (int)bytes.Length - byteIndex)
			{
				throw new ArgumentOutOfRangeException("byteCount", UnixEncoding._("ArgRange_Array"));
			}
			if (charIndex < 0 || charIndex > (int)chars.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", UnixEncoding._("ArgRange_Array"));
			}
			if (charIndex == (int)chars.Length)
			{
				return 0;
			}
			byte[] numArray = new byte[6];
			int num = 0;
			int length = (int)chars.Length;
			int num1 = charIndex;
			uint num2 = leftOverBits;
			uint num3 = leftOverCount & 15;
			uint num4 = leftOverCount >> 4 & 15;
			while (byteCount > 0)
			{
				int num5 = byteIndex;
				byteIndex = num5 + 1;
				uint num6 = bytes[num5];
				int num7 = num;
				num = num7 + 1;
				numArray[num7] = (byte)num6;
				byteCount--;
				if (num4 != 0)
				{
					if ((num6 & 192) != 128)
					{
						!throwOnInvalid;
						if (num6 < 128)
						{
							byteIndex--;
							byteCount++;
							num--;
						}
						UnixEncoding.CopyRaw(numArray, ref num, chars, ref num1, length);
						num4 = 0;
						num = 0;
					}
					else
					{
						num2 = num2 << 6 | num6 & 63;
						UInt32 num8 = num3 + 1;
						num3 = num8;
						if (num8 >= num4)
						{
							if (num2 < 65536)
							{
								bool flag = false;
								switch (num4)
								{
									case 2:
									{
										flag = num2 <= 127;
										break;
									}
									case 3:
									{
										flag = num2 <= 2047;
										break;
									}
									case 4:
									{
										flag = num2 <= 65535;
										break;
									}
									case 5:
									{
										flag = num2 <= 2097151;
										break;
									}
									case 6:
									{
										flag = num2 <= 67108863;
										break;
									}
								}
								if (!flag)
								{
									if (num1 >= length)
									{
										throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "chars");
									}
									int num9 = num1;
									num1 = num9 + 1;
									chars[num9] = (char)num2;
								}
								else
								{
									UnixEncoding.CopyRaw(numArray, ref num, chars, ref num1, length);
								}
							}
							else if (num2 < 1114112)
							{
								if (num1 + 2 > length)
								{
									throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "chars");
								}
								num2 -= 65536;
								int num10 = num1;
								num1 = num10 + 1;
								chars[num10] = (char)((num2 >> 10) + 55296);
								int num11 = num1;
								num1 = num11 + 1;
								chars[num11] = (char)((num2 & 1023) + 56320);
							}
							else if (throwOnInvalid)
							{
								UnixEncoding.CopyRaw(numArray, ref num, chars, ref num1, length);
							}
							num4 = 0;
							num = 0;
						}
					}
				}
				else if (num6 < 128)
				{
					if (num1 >= length)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "chars");
					}
					num = 0;
					int num12 = num1;
					num1 = num12 + 1;
					chars[num12] = (char)num6;
				}
				else if ((num6 & 224) == 192)
				{
					num2 = num6 & 31;
					num3 = 1;
					num4 = 2;
				}
				else if ((num6 & 240) == 224)
				{
					num2 = num6 & 15;
					num3 = 1;
					num4 = 3;
				}
				else if ((num6 & 248) == 240)
				{
					num2 = num6 & 7;
					num3 = 1;
					num4 = 4;
				}
				else if ((num6 & 252) == 248)
				{
					num2 = num6 & 3;
					num3 = 1;
					num4 = 5;
				}
				else if ((num6 & 254) != 252)
				{
					!throwOnInvalid;
					num = 0;
					int num13 = num1;
					num1 = num13 + 1;
					chars[num13] = UnixEncoding.EscapeByte;
					int num14 = num1;
					num1 = num14 + 1;
					chars[num14] = (char)num6;
				}
				else
				{
					num2 = num6 & 3;
					num3 = 1;
					num4 = 6;
				}
			}
			if (flush && num4 != 0 && throwOnInvalid)
			{
				UnixEncoding.CopyRaw(numArray, ref num, chars, ref num1, length);
			}
			leftOverBits = num2;
			leftOverCount = num3 | num4 << 4;
			return num1 - charIndex;
		}

		[Serializable]
		private class UnixDecoder : Decoder
		{
			private uint leftOverBits;

			private uint leftOverCount;

			public UnixDecoder()
			{
				this.leftOverBits = 0;
				this.leftOverCount = 0;
			}

			public override int GetCharCount(byte[] bytes, int index, int count)
			{
				return UnixEncoding.InternalGetCharCount(bytes, index, count, this.leftOverBits, this.leftOverCount, true, false);
			}

			public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
			{
				return UnixEncoding.InternalGetChars(bytes, byteIndex, byteCount, chars, charIndex, ref this.leftOverBits, ref this.leftOverCount, true, false);
			}
		}

		[Serializable]
		private class UnixEncoder : Encoder
		{
			private uint leftOver;

			public UnixEncoder()
			{
				this.leftOver = 0;
			}

			public override int GetByteCount(char[] chars, int index, int count, bool flush)
			{
				return UnixEncoding.InternalGetByteCount(chars, index, count, this.leftOver, flush);
			}

			public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteCount, bool flush)
			{
				int num = UnixEncoding.InternalGetBytes(chars, charIndex, charCount, bytes, byteCount, ref this.leftOver, flush);
				return num;
			}
		}
	}
}