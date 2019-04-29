using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;
using System.IO;

namespace Newtonsoft.Json
{
	[Preserve]
	public class JsonTextReader : JsonReader, IJsonLineInfo
	{
		private const char UnicodeReplacementChar = '\uFFFD';

		private const int MaximumJavascriptIntegerCharacterLength = 380;

		private readonly TextReader _reader;

		private char[] _chars;

		private int _charsUsed;

		private int _charPos;

		private int _lineStartPos;

		private int _lineNumber;

		private bool _isEndOfFile;

		private StringBuffer _stringBuffer;

		private StringReference _stringReference;

		private IArrayPool<char> _arrayPool;

		internal PropertyNameTable NameTable;

		public IArrayPool<char> ArrayPool
		{
			get
			{
				return this._arrayPool;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this._arrayPool = value;
			}
		}

		public int LineNumber
		{
			get
			{
				if (base.CurrentState == JsonReader.State.Start && this.LinePosition == 0 && this.TokenType != JsonToken.Comment)
				{
					return 0;
				}
				return this._lineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return this._charPos - this._lineStartPos;
			}
		}

		public JsonTextReader(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this._reader = reader;
			this._lineNumber = 1;
		}

		private static void BlockCopyChars(char[] src, int srcOffset, char[] dst, int dstOffset, int count)
		{
			Buffer.BlockCopy(src, srcOffset * 2, dst, dstOffset * 2, count * 2);
		}

		private void ClearRecentString()
		{
			this._stringBuffer.Position = 0;
			this._stringReference = new StringReference();
		}

		public override void Close()
		{
			base.Close();
			if (this._chars != null)
			{
				BufferUtils.ReturnBuffer(this._arrayPool, this._chars);
				this._chars = null;
			}
			if (base.CloseInput && this._reader != null)
			{
				this._reader.Close();
			}
			this._stringBuffer.Clear(this._arrayPool);
		}

		private JsonReaderException CreateUnexpectedCharacterException(char c)
		{
			return JsonReaderException.Create(this, "Unexpected character encountered while parsing value: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
		}

		private bool EatWhitespace(bool oneOrMore)
		{
			bool flag = false;
			bool flag1 = false;
			while (!flag)
			{
				char chr = this._chars[this._charPos];
				if (chr == 0)
				{
					if (this._charsUsed != this._charPos)
					{
						this._charPos++;
					}
					else
					{
						if (this.ReadData(false) != 0)
						{
							continue;
						}
						flag = true;
					}
				}
				else if (chr == '\n')
				{
					this.ProcessLineFeed();
				}
				else if (chr == '\r')
				{
					this.ProcessCarriageReturn(false);
				}
				else if (chr == ' ' || char.IsWhiteSpace(chr))
				{
					flag1 = true;
					this._charPos++;
				}
				else
				{
					flag = true;
				}
			}
			return !oneOrMore | flag1;
		}

		private void EndComment(bool setToken, int initialPosition, int endPosition)
		{
			if (setToken)
			{
				base.SetToken(JsonToken.Comment, new string(this._chars, initialPosition, endPosition - initialPosition));
			}
		}

		private void EnsureBuffer()
		{
			if (this._chars == null)
			{
				this._chars = BufferUtils.RentBuffer(this._arrayPool, 1024);
				this._chars[0] = '\0';
			}
		}

		private void EnsureBufferNotEmpty()
		{
			if (this._stringBuffer.IsEmpty)
			{
				this._stringBuffer = new StringBuffer(this._arrayPool, 1024);
			}
		}

		private bool EnsureChars(int relativePosition, bool append)
		{
			if (this._charPos + relativePosition < this._charsUsed)
			{
				return true;
			}
			return this.ReadChars(relativePosition, append);
		}

		private void HandleNull()
		{
			if (!this.EnsureChars(1, true))
			{
				this._charPos = this._charsUsed;
				throw base.CreateUnexpectedEndException();
			}
			if (this._chars[this._charPos + 1] != 'u')
			{
				this._charPos += 2;
				throw this.CreateUnexpectedCharacterException(this._chars[this._charPos - 1]);
			}
			this.ParseNull();
		}

		public bool HasLineInfo()
		{
			return true;
		}

		private bool IsSeparator(char c)
		{
			if (c > ')')
			{
				if (c <= '/')
				{
					if (c == ',')
					{
						return true;
					}
					if (c == '/')
					{
						if (!this.EnsureChars(1, false))
						{
							return false;
						}
						char chr = this._chars[this._charPos + 1];
						if (chr == '*')
						{
							return true;
						}
						return chr == '/';
					}
					if (char.IsWhiteSpace(c))
					{
						return true;
					}
					return false;
				}
				else if (c != ']' && c != '}')
				{
					if (char.IsWhiteSpace(c))
					{
						return true;
					}
					return false;
				}
				return true;
			}
			else
			{
				switch (c)
				{
					case '\t':
					case '\n':
					case '\r':
					{
						return true;
					}
					case '\v':
					case '\f':
					{
						break;
					}
					default:
					{
						if (c == ' ')
						{
							return true;
						}
						if (c == ')')
						{
							if (base.CurrentState != JsonReader.State.Constructor && base.CurrentState != JsonReader.State.ConstructorStart)
							{
								return false;
							}
							return true;
						}
						else
						{
							break;
						}
					}
				}
			}
			if (char.IsWhiteSpace(c))
			{
				return true;
			}
			return false;
		}

		private bool MatchValue(string value)
		{
			if (!this.EnsureChars(value.Length - 1, true))
			{
				this._charPos = this._charsUsed;
				throw base.CreateUnexpectedEndException();
			}
			for (int i = 0; i < value.Length; i++)
			{
				if (this._chars[this._charPos + i] != value[i])
				{
					this._charPos += i;
					return false;
				}
			}
			this._charPos += value.Length;
			return true;
		}

		private bool MatchValueWithTrailingSeparator(string value)
		{
			if (!this.MatchValue(value))
			{
				return false;
			}
			if (!this.EnsureChars(0, false))
			{
				return true;
			}
			if (this.IsSeparator(this._chars[this._charPos]))
			{
				return true;
			}
			return this._chars[this._charPos] == '\0';
		}

		private void OnNewLine(int pos)
		{
			this._lineNumber++;
			this._lineStartPos = pos;
		}

		private void ParseComment(bool setToken)
		{
			bool flag;
			this._charPos++;
			if (!this.EnsureChars(1, false))
			{
				throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
			}
			if (this._chars[this._charPos] != '*')
			{
				if (this._chars[this._charPos] != '/')
				{
					throw JsonReaderException.Create(this, "Error parsing comment. Expected: *, got {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
				}
				flag = true;
			}
			else
			{
				flag = false;
			}
			this._charPos++;
			int num = this._charPos;
			do
			{
			Label0:
				char chr = this._chars[this._charPos];
				if (chr > '\n')
				{
					if (chr == '\r')
					{
						if (flag)
						{
							this.EndComment(setToken, num, this._charPos);
							return;
						}
						this.ProcessCarriageReturn(true);
						goto Label0;
					}
					else if (chr == '*')
					{
						this._charPos++;
						continue;
					}
				}
				else if (chr != 0)
				{
					if (chr == '\n')
					{
						if (flag)
						{
							this.EndComment(setToken, num, this._charPos);
							return;
						}
						this.ProcessLineFeed();
						goto Label0;
					}
				}
				else if (this._charsUsed != this._charPos)
				{
					this._charPos++;
					goto Label0;
				}
				else if (this.ReadData(true) == 0)
				{
					if (!flag)
					{
						throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
					}
					this.EndComment(setToken, num, this._charPos);
					return;
				}
				else
				{
					goto Label0;
				}
				this._charPos++;
				goto Label0;
			}
			while (flag || !this.EnsureChars(0, true) || this._chars[this._charPos] != '/');
			this.EndComment(setToken, num, this._charPos - 1);
			this._charPos++;
		}

		private void ParseConstructor()
		{
			int num;
			if (!this.MatchValueWithTrailingSeparator("new"))
			{
				throw JsonReaderException.Create(this, "Unexpected content while parsing JSON.");
			}
			this.EatWhitespace(false);
			int num1 = this._charPos;
			while (true)
			{
				char chr = this._chars[this._charPos];
				if (chr == 0)
				{
					if (this._charsUsed != this._charPos)
					{
						num = this._charPos;
						this._charPos++;
						break;
					}
					else if (this.ReadData(true) == 0)
					{
						throw JsonReaderException.Create(this, "Unexpected end while parsing constructor.");
					}
				}
				else if (char.IsLetterOrDigit(chr))
				{
					this._charPos++;
				}
				else if (chr == '\r')
				{
					num = this._charPos;
					this.ProcessCarriageReturn(true);
					break;
				}
				else if (chr == '\n')
				{
					num = this._charPos;
					this.ProcessLineFeed();
					break;
				}
				else if (!char.IsWhiteSpace(chr))
				{
					if (chr != '(')
					{
						throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, chr));
					}
					num = this._charPos;
					break;
				}
				else
				{
					num = this._charPos;
					this._charPos++;
					break;
				}
			}
			this._stringReference = new StringReference(this._chars, num1, num - num1);
			string str = this._stringReference.ToString();
			this.EatWhitespace(false);
			if (this._chars[this._charPos] != '(')
			{
				throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
			}
			this._charPos++;
			this.ClearRecentString();
			base.SetToken(JsonToken.StartConstructor, str);
		}

		private void ParseFalse()
		{
			if (!this.MatchValueWithTrailingSeparator(JsonConvert.False))
			{
				throw JsonReaderException.Create(this, "Error parsing boolean value.");
			}
			base.SetToken(JsonToken.Boolean, false);
		}

		private void ParseNull()
		{
			if (!this.MatchValueWithTrailingSeparator(JsonConvert.Null))
			{
				throw JsonReaderException.Create(this, "Error parsing null value.");
			}
			base.SetToken(JsonToken.Null);
		}

		private void ParseNumber(ReadType readType)
		{
			object num;
			JsonToken jsonToken;
			double num1;
			int num2;
			decimal num3;
			double num4;
			long num5;
			decimal num6;
			double num7;
			this.ShiftBufferIfNeeded();
			char chr = this._chars[this._charPos];
			int num8 = this._charPos;
			this.ReadNumberIntoBuffer();
			base.SetPostValueState(true);
			this._stringReference = new StringReference(this._chars, num8, this._charPos - num8);
			bool flag = (!char.IsDigit(chr) ? false : this._stringReference.Length == 1);
			bool flag1 = (chr != '0' || this._stringReference.Length <= 1 || this._stringReference.Chars[this._stringReference.StartIndex + 1] == '.' || this._stringReference.Chars[this._stringReference.StartIndex + 1] == 'e' ? false : this._stringReference.Chars[this._stringReference.StartIndex + 1] != 'E');
			if (readType == ReadType.ReadAsString)
			{
				string str = this._stringReference.ToString();
				if (flag1)
				{
					try
					{
						if (!str.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
						{
							Convert.ToInt64(str, 8);
						}
						else
						{
							Convert.ToInt64(str, 16);
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, str), exception);
					}
				}
				else if (!double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out num1))
				{
					throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
				}
				jsonToken = JsonToken.String;
				num = str;
			}
			else if (readType == ReadType.ReadAsInt32)
			{
				if (flag)
				{
					num = chr - 48;
				}
				else if (!flag1)
				{
					ParseResult parseResult = ConvertUtils.Int32TryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out num2);
					if (parseResult != ParseResult.Success)
					{
						if (parseResult != ParseResult.Overflow)
						{
							throw JsonReaderException.Create(this, "Input string '{0}' is not a valid integer.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
						}
						throw JsonReaderException.Create(this, "JSON integer {0} is too large or small for an Int32.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
					}
					num = num2;
				}
				else
				{
					string str1 = this._stringReference.ToString();
					try
					{
						num = (str1.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt32(str1, 16) : Convert.ToInt32(str1, 8));
					}
					catch (Exception exception3)
					{
						Exception exception2 = exception3;
						throw JsonReaderException.Create(this, "Input string '{0}' is not a valid integer.".FormatWith(CultureInfo.InvariantCulture, str1), exception2);
					}
				}
				jsonToken = JsonToken.Integer;
			}
			else if (readType == ReadType.ReadAsDecimal)
			{
				if (flag)
				{
					num = chr - new decimal(48);
				}
				else if (!flag1)
				{
					if (!decimal.TryParse(this._stringReference.ToString(), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent | NumberStyles.Integer | NumberStyles.Number | NumberStyles.Float, CultureInfo.InvariantCulture, out num3))
					{
						throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
					}
					num = num3;
				}
				else
				{
					string str2 = this._stringReference.ToString();
					try
					{
						num = Convert.ToDecimal((str2.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(str2, 16) : Convert.ToInt64(str2, 8)));
					}
					catch (Exception exception5)
					{
						Exception exception4 = exception5;
						throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, str2), exception4);
					}
				}
				jsonToken = JsonToken.Float;
			}
			else if (readType == ReadType.ReadAsDouble)
			{
				if (flag)
				{
					num = (double)chr - 48;
				}
				else if (!flag1)
				{
					if (!double.TryParse(this._stringReference.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out num4))
					{
						throw JsonReaderException.Create(this, "Input string '{0}' is not a valid double.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
					}
					num = num4;
				}
				else
				{
					string str3 = this._stringReference.ToString();
					try
					{
						num = Convert.ToDouble((str3.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(str3, 16) : Convert.ToInt64(str3, 8)));
					}
					catch (Exception exception7)
					{
						Exception exception6 = exception7;
						throw JsonReaderException.Create(this, "Input string '{0}' is not a valid double.".FormatWith(CultureInfo.InvariantCulture, str3), exception6);
					}
				}
				jsonToken = JsonToken.Float;
			}
			else if (flag)
			{
				num = (long)((ulong)chr - (long)48);
				jsonToken = JsonToken.Integer;
			}
			else if (!flag1)
			{
				ParseResult parseResult1 = ConvertUtils.Int64TryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out num5);
				if (parseResult1 != ParseResult.Success)
				{
					if (parseResult1 == ParseResult.Overflow)
					{
						throw JsonReaderException.Create(this, "JSON integer {0} is too large or small for an Int64.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
					}
					string str4 = this._stringReference.ToString();
					if (this._floatParseHandling != Newtonsoft.Json.FloatParseHandling.Decimal)
					{
						if (!double.TryParse(str4, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent | NumberStyles.Integer | NumberStyles.Float, CultureInfo.InvariantCulture, out num7))
						{
							throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, str4));
						}
						num = num7;
					}
					else
					{
						if (!decimal.TryParse(str4, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent | NumberStyles.Integer | NumberStyles.Number | NumberStyles.Float, CultureInfo.InvariantCulture, out num6))
						{
							throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, str4));
						}
						num = num6;
					}
					jsonToken = JsonToken.Float;
				}
				else
				{
					num = num5;
					jsonToken = JsonToken.Integer;
				}
			}
			else
			{
				string str5 = this._stringReference.ToString();
				try
				{
					num = (str5.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(str5, 16) : Convert.ToInt64(str5, 8));
				}
				catch (Exception exception9)
				{
					Exception exception8 = exception9;
					throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, str5), exception8);
				}
				jsonToken = JsonToken.Integer;
			}
			this.ClearRecentString();
			base.SetToken(jsonToken, num, false);
		}

		private object ParseNumberNaN(ReadType readType)
		{
			if (!this.MatchValueWithTrailingSeparator(JsonConvert.NaN))
			{
				throw JsonReaderException.Create(this, "Error parsing NaN value.");
			}
			if (readType != ReadType.Read)
			{
				if (readType == ReadType.ReadAsString)
				{
					base.SetToken(JsonToken.String, JsonConvert.NaN);
					return JsonConvert.NaN;
				}
				if (readType != ReadType.ReadAsDouble)
				{
					throw JsonReaderException.Create(this, "Cannot read NaN value.");
				}
			}
			if (this._floatParseHandling == Newtonsoft.Json.FloatParseHandling.Double)
			{
				base.SetToken(JsonToken.Float, Double.NaN);
				return Double.NaN;
			}
			throw JsonReaderException.Create(this, "Cannot read NaN value.");
		}

		private object ParseNumberNegativeInfinity(ReadType readType)
		{
			if (!this.MatchValueWithTrailingSeparator(JsonConvert.NegativeInfinity))
			{
				throw JsonReaderException.Create(this, "Error parsing -Infinity value.");
			}
			if (readType != ReadType.Read)
			{
				if (readType == ReadType.ReadAsString)
				{
					base.SetToken(JsonToken.String, JsonConvert.NegativeInfinity);
					return JsonConvert.NegativeInfinity;
				}
				if (readType != ReadType.ReadAsDouble)
				{
					throw JsonReaderException.Create(this, "Cannot read -Infinity value.");
				}
			}
			if (this._floatParseHandling == Newtonsoft.Json.FloatParseHandling.Double)
			{
				base.SetToken(JsonToken.Float, Double.NegativeInfinity);
				return Double.NegativeInfinity;
			}
			throw JsonReaderException.Create(this, "Cannot read -Infinity value.");
		}

		private object ParseNumberPositiveInfinity(ReadType readType)
		{
			if (!this.MatchValueWithTrailingSeparator(JsonConvert.PositiveInfinity))
			{
				throw JsonReaderException.Create(this, "Error parsing Infinity value.");
			}
			if (readType != ReadType.Read)
			{
				if (readType == ReadType.ReadAsString)
				{
					base.SetToken(JsonToken.String, JsonConvert.PositiveInfinity);
					return JsonConvert.PositiveInfinity;
				}
				if (readType != ReadType.ReadAsDouble)
				{
					throw JsonReaderException.Create(this, "Cannot read Infinity value.");
				}
			}
			if (this._floatParseHandling == Newtonsoft.Json.FloatParseHandling.Double)
			{
				base.SetToken(JsonToken.Float, Double.PositiveInfinity);
				return Double.PositiveInfinity;
			}
			throw JsonReaderException.Create(this, "Cannot read Infinity value.");
		}

		private bool ParseObject()
		{
			do
			{
			Label1:
				char chr = this._chars[this._charPos];
				if (chr > '\r')
				{
					if (chr == ' ')
					{
						goto Label0;
					}
					if (chr == '/')
					{
						this.ParseComment(true);
						return true;
					}
					if (chr == '}')
					{
						base.SetToken(JsonToken.EndObject);
						this._charPos++;
						return true;
					}
				}
				else if (chr == 0)
				{
					if (this._charsUsed == this._charPos)
					{
						continue;
					}
					this._charPos++;
					goto Label1;
				}
				else
				{
					switch (chr)
					{
						case '\t':
						{
							goto Label0;
						}
						case '\n':
						{
							this.ProcessLineFeed();
							goto Label1;
						}
						case '\r':
						{
							this.ProcessCarriageReturn(false);
							goto Label1;
						}
					}
				}
				if (!char.IsWhiteSpace(chr))
				{
					return this.ParseProperty();
				}
				this._charPos++;
				goto Label1;
			}
			while (this.ReadData(false) != 0);
			return false;
		Label0:
			this._charPos++;
			goto Label1;
		}

		private bool ParsePostValue()
		{
			do
			{
			Label0:
				char chr = this._chars[this._charPos];
				if (chr <= ')')
				{
					if (chr > '\r')
					{
						if (chr == ' ')
						{
							goto Label2;
						}
						if (chr == ')')
						{
							this._charPos++;
							base.SetToken(JsonToken.EndConstructor);
							return true;
						}
						goto Label1;
					}
					else if (chr == 0)
					{
						if (this._charsUsed == this._charPos)
						{
							continue;
						}
						this._charPos++;
						goto Label0;
					}
					else
					{
						switch (chr)
						{
							case '\t':
							{
								break;
							}
							case '\n':
							{
								this.ProcessLineFeed();
								goto Label0;
							}
							case '\r':
							{
								this.ProcessCarriageReturn(false);
								goto Label0;
							}
							default:
							{
								goto Label1;
							}
						}
					}
				Label2:
					this._charPos++;
					goto Label0;
				}
				else if (chr > '/')
				{
					if (chr == ']')
					{
						this._charPos++;
						base.SetToken(JsonToken.EndArray);
						return true;
					}
					if (chr == '}')
					{
						this._charPos++;
						base.SetToken(JsonToken.EndObject);
						return true;
					}
				}
				else
				{
					if (chr == ',')
					{
						this._charPos++;
						base.SetStateBasedOnCurrent();
						return false;
					}
					if (chr == '/')
					{
						this.ParseComment(true);
						return true;
					}
				}
			Label1:
				if (!char.IsWhiteSpace(chr))
				{
					throw JsonReaderException.Create(this, "After parsing a value an unexpected character was encountered: {0}.".FormatWith(CultureInfo.InvariantCulture, chr));
				}
				this._charPos++;
				goto Label0;
			}
			while (this.ReadData(false) != 0);
			this._currentState = JsonReader.State.Finished;
			return false;
		}

		private bool ParseProperty()
		{
			char chr;
			string str;
			char chr1 = this._chars[this._charPos];
			if (chr1 == '\"' || chr1 == '\'')
			{
				this._charPos++;
				chr = chr1;
				this.ShiftBufferIfNeeded();
				this.ReadStringIntoBuffer(chr);
			}
			else
			{
				if (!this.ValidIdentifierChar(chr1))
				{
					throw JsonReaderException.Create(this, "Invalid property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
				}
				chr = '\0';
				this.ShiftBufferIfNeeded();
				this.ParseUnquotedProperty();
			}
			str = (this.NameTable == null ? this._stringReference.ToString() : this.NameTable.Get(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length) ?? this._stringReference.ToString());
			this.EatWhitespace(false);
			if (this._chars[this._charPos] != ':')
			{
				throw JsonReaderException.Create(this, "Invalid character after parsing property name. Expected ':' but got: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
			}
			this._charPos++;
			base.SetToken(JsonToken.PropertyName, str);
			this._quoteChar = chr;
			this.ClearRecentString();
			return true;
		}

		private void ParseString(char quote, ReadType readType)
		{
			Guid guid;
			byte[] numArray;
			Newtonsoft.Json.DateParseHandling dateParseHandling;
			DateTime dateTime;
			DateTimeOffset dateTimeOffset;
			this._charPos++;
			this.ShiftBufferIfNeeded();
			this.ReadStringIntoBuffer(quote);
			base.SetPostValueState(true);
			switch (readType)
			{
				case ReadType.ReadAsInt32:
				case ReadType.ReadAsDecimal:
				case ReadType.ReadAsBoolean:
				{
					return;
				}
				case ReadType.ReadAsBytes:
				{
					if (this._stringReference.Length != 0)
					{
						numArray = (this._stringReference.Length != 36 || !ConvertUtils.TryConvertGuid(this._stringReference.ToString(), out guid) ? Convert.FromBase64CharArray(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length) : guid.ToByteArray());
					}
					else
					{
						numArray = new byte[0];
					}
					base.SetToken(JsonToken.Bytes, numArray, false);
					return;
				}
				case ReadType.ReadAsString:
				{
					base.SetToken(JsonToken.String, this._stringReference.ToString(), false);
					this._quoteChar = quote;
					return;
				}
				case ReadType.ReadAsDateTime:
				case ReadType.ReadAsDateTimeOffset:
				case ReadType.ReadAsDouble:
				{
					if (this._dateParseHandling != Newtonsoft.Json.DateParseHandling.None)
					{
						if (readType != ReadType.ReadAsDateTime)
						{
							dateParseHandling = (readType != ReadType.ReadAsDateTimeOffset ? this._dateParseHandling : Newtonsoft.Json.DateParseHandling.DateTimeOffset);
						}
						else
						{
							dateParseHandling = Newtonsoft.Json.DateParseHandling.DateTime;
						}
						if (dateParseHandling == Newtonsoft.Json.DateParseHandling.DateTime)
						{
							if (DateTimeUtils.TryParseDateTime(this._stringReference, base.DateTimeZoneHandling, base.DateFormatString, base.Culture, out dateTime))
							{
								base.SetToken(JsonToken.Date, dateTime, false);
								return;
							}
						}
						else if (DateTimeUtils.TryParseDateTimeOffset(this._stringReference, base.DateFormatString, base.Culture, out dateTimeOffset))
						{
							base.SetToken(JsonToken.Date, dateTimeOffset, false);
							return;
						}
					}
					base.SetToken(JsonToken.String, this._stringReference.ToString(), false);
					this._quoteChar = quote;
					return;
				}
				default:
				{
					goto case ReadType.ReadAsDouble;
				}
			}
		}

		private void ParseTrue()
		{
			if (!this.MatchValueWithTrailingSeparator(JsonConvert.True))
			{
				throw JsonReaderException.Create(this, "Error parsing boolean value.");
			}
			base.SetToken(JsonToken.Boolean, true);
		}

		private void ParseUndefined()
		{
			if (!this.MatchValueWithTrailingSeparator(JsonConvert.Undefined))
			{
				throw JsonReaderException.Create(this, "Error parsing undefined value.");
			}
			base.SetToken(JsonToken.Undefined);
		}

		private char ParseUnicode()
		{
			if (!this.EnsureChars(4, true))
			{
				throw JsonReaderException.Create(this, "Unexpected end while parsing unicode character.");
			}
			char chr = Convert.ToChar(ConvertUtils.HexTextToInt(this._chars, this._charPos, this._charPos + 4));
			this._charPos += 4;
			return chr;
		}

		private void ParseUnquotedProperty()
		{
			int num = this._charPos;
			do
			{
			Label0:
				if (this._chars[this._charPos] != 0)
				{
					char chr = this._chars[this._charPos];
					if (!this.ValidIdentifierChar(chr))
					{
						if (!char.IsWhiteSpace(chr) && chr != ':')
						{
							throw JsonReaderException.Create(this, "Invalid JavaScript property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, chr));
						}
						this._stringReference = new StringReference(this._chars, num, this._charPos - num);
						return;
					}
					this._charPos++;
					goto Label0;
				}
				else
				{
					if (this._charsUsed == this._charPos)
					{
						continue;
					}
					this._stringReference = new StringReference(this._chars, num, this._charPos - num);
					return;
				}
			}
			while (this.ReadData(true) != 0);
			throw JsonReaderException.Create(this, "Unexpected end while parsing unquoted property name.");
		}

		private bool ParseValue()
		{
			char chr;
			while (true)
			{
				chr = this._chars[this._charPos];
				if (chr > 'N')
				{
					if (chr <= 'f')
					{
						if (chr == '[')
						{
							this._charPos++;
							base.SetToken(JsonToken.StartArray);
							return true;
						}
						if (chr == ']')
						{
							this._charPos++;
							base.SetToken(JsonToken.EndArray);
							return true;
						}
						if (chr == 'f')
						{
							this.ParseFalse();
							return true;
						}
					}
					else if (chr > 't')
					{
						if (chr == 'u')
						{
							this.ParseUndefined();
							return true;
						}
						if (chr == '{')
						{
							this._charPos++;
							base.SetToken(JsonToken.StartObject);
							return true;
						}
					}
					else
					{
						if (chr == 'n')
						{
							if (!this.EnsureChars(1, true))
							{
								this._charPos++;
								throw base.CreateUnexpectedEndException();
							}
							char chr1 = this._chars[this._charPos + 1];
							if (chr1 != 'u')
							{
								if (chr1 != 'e')
								{
									throw this.CreateUnexpectedCharacterException(this._chars[this._charPos]);
								}
								this.ParseConstructor();
							}
							else
							{
								this.ParseNull();
							}
							return true;
						}
						if (chr == 't')
						{
							this.ParseTrue();
							return true;
						}
					}
				}
				else if (chr > ' ')
				{
					if (chr > '/')
					{
						if (chr == 'I')
						{
							this.ParseNumberPositiveInfinity(ReadType.Read);
							return true;
						}
						if (chr == 'N')
						{
							this.ParseNumberNaN(ReadType.Read);
							return true;
						}
					}
					else
					{
						if (chr == '\"')
						{
							break;
						}
						switch (chr)
						{
							case '\'':
							{
								this.ParseString(chr, ReadType.Read);
								return true;
							}
							case ')':
							{
								this._charPos++;
								base.SetToken(JsonToken.EndConstructor);
								return true;
							}
							case ',':
							{
								base.SetToken(JsonToken.Undefined);
								return true;
							}
							case '-':
							{
								if (!this.EnsureChars(1, true) || this._chars[this._charPos + 1] != 'I')
								{
									this.ParseNumber(ReadType.Read);
								}
								else
								{
									this.ParseNumberNegativeInfinity(ReadType.Read);
								}
								return true;
							}
							case '/':
							{
								this.ParseComment(true);
								return true;
							}
						}
					}
				}
				else if (chr != 0)
				{
					switch (chr)
					{
						case '\t':
						{
							this._charPos++;
							continue;
						}
						case '\n':
						{
							this.ProcessLineFeed();
							continue;
						}
						case '\v':
						case '\f':
						{
							break;
						}
						case '\r':
						{
							this.ProcessCarriageReturn(false);
							continue;
						}
						default:
						{
							if (chr == ' ')
							{
								goto case '\t';
							}
							break;
						}
					}
				}
				else if (this._charsUsed != this._charPos)
				{
					this._charPos++;
					continue;
				}
				else if (this.ReadData(false) == 0)
				{
					return false;
				}
				if (!char.IsWhiteSpace(chr))
				{
					if (!char.IsNumber(chr) && chr != '-' && chr != '.')
					{
						throw this.CreateUnexpectedCharacterException(chr);
					}
					this.ParseNumber(ReadType.Read);
					return true;
				}
				this._charPos++;
			}
			this.ParseString(chr, ReadType.Read);
			return true;
		}

		private void ProcessCarriageReturn(bool append)
		{
			this._charPos++;
			if (this.EnsureChars(1, append) && this._chars[this._charPos] == '\n')
			{
				this._charPos++;
			}
			this.OnNewLine(this._charPos);
		}

		private void ProcessLineFeed()
		{
			this._charPos++;
			this.OnNewLine(this._charPos);
		}

		private void ProcessValueComma()
		{
			this._charPos++;
			if (this._currentState != JsonReader.State.PostValue)
			{
				base.SetToken(JsonToken.Undefined);
				throw this.CreateUnexpectedCharacterException(',');
			}
			base.SetStateBasedOnCurrent();
		}

		public override bool Read()
		{
			this.EnsureBuffer();
			do
			{
				switch (this._currentState)
				{
					case JsonReader.State.Start:
					case JsonReader.State.Property:
					case JsonReader.State.ArrayStart:
					case JsonReader.State.Array:
					case JsonReader.State.ConstructorStart:
					case JsonReader.State.Constructor:
					{
						return this.ParseValue();
					}
					case JsonReader.State.Complete:
					case JsonReader.State.Closed:
					case JsonReader.State.Error:
					{
						throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
					}
					case JsonReader.State.ObjectStart:
					case JsonReader.State.Object:
					{
						return this.ParseObject();
					}
					case JsonReader.State.PostValue:
					{
						continue;
					}
					case JsonReader.State.Finished:
					{
						if (!this.EnsureChars(0, false))
						{
							base.SetToken(JsonToken.None);
							return false;
						}
						this.EatWhitespace(false);
						if (this._isEndOfFile)
						{
							base.SetToken(JsonToken.None);
							return false;
						}
						if (this._chars[this._charPos] != '/')
						{
							throw JsonReaderException.Create(this, "Additional text encountered after finished reading JSON content: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
						}
						this.ParseComment(true);
						return true;
					}
					default:
					{
						throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
					}
				}
			}
			while (!this.ParsePostValue());
			return true;
		}

		public override bool? ReadAsBoolean()
		{
			char chr;
			bool? nullable;
			this.EnsureBuffer();
			switch (this._currentState)
			{
				case JsonReader.State.Start:
				case JsonReader.State.Property:
				case JsonReader.State.ArrayStart:
				case JsonReader.State.Array:
				case JsonReader.State.PostValue:
				case JsonReader.State.ConstructorStart:
				case JsonReader.State.Constructor:
				{
					while (true)
					{
						chr = this._chars[this._charPos];
						if (chr > '9')
						{
							if (chr > 'f')
							{
								if (chr == 'n')
								{
									this.HandleNull();
									nullable = null;
									return nullable;
								}
								if (chr == 't')
								{
									break;
								}
							}
							else
							{
								if (chr == ']')
								{
									this._charPos++;
									if (this._currentState != JsonReader.State.Array && this._currentState != JsonReader.State.ArrayStart && this._currentState != JsonReader.State.PostValue)
									{
										throw this.CreateUnexpectedCharacterException(chr);
									}
									base.SetToken(JsonToken.EndArray);
									nullable = null;
									return nullable;
								}
								if (chr == 'f')
								{
									break;
								}
							}
						}
						else if (chr != 0)
						{
							switch (chr)
							{
								case '\t':
								{
								Label0:
									this._charPos++;
									continue;
								}
								case '\n':
								{
									this.ProcessLineFeed();
									continue;
								}
								case '\v':
								case '\f':
								{
									break;
								}
								case '\r':
								{
									this.ProcessCarriageReturn(false);
									continue;
								}
								default:
								{
									switch (chr)
									{
										case ' ':
										{
											goto Label0;
										}
										case '\"':
										case '\'':
										{
											this.ParseString(chr, ReadType.Read);
											return base.ReadBooleanString(this._stringReference.ToString());
										}
										case ',':
										{
											this.ProcessValueComma();
											continue;
										}
										case '-':
										case '.':
										case '0':
										case '1':
										case '2':
										case '3':
										case '4':
										case '5':
										case '6':
										case '7':
										case '8':
										case '9':
										{
											this.ParseNumber(ReadType.Read);
											bool flag = Convert.ToBoolean(this.Value, CultureInfo.InvariantCulture);
											base.SetToken(JsonToken.Boolean, flag, false);
											return new bool?(flag);
										}
										case '/':
										{
											this.ParseComment(false);
											continue;
										}
									}
									break;
								}
							}
						}
						else if (this.ReadNullChar())
						{
							base.SetToken(JsonToken.None, null, false);
							nullable = null;
							return nullable;
						}
						this._charPos++;
						if (!char.IsWhiteSpace(chr))
						{
							throw this.CreateUnexpectedCharacterException(chr);
						}
					}
					bool flag1 = chr == 't';
					if (!this.MatchValueWithTrailingSeparator((flag1 ? JsonConvert.True : JsonConvert.False)))
					{
						throw this.CreateUnexpectedCharacterException(this._chars[this._charPos]);
					}
					base.SetToken(JsonToken.Boolean, flag1);
					return new bool?(flag1);
				}
				case JsonReader.State.Complete:
				case JsonReader.State.ObjectStart:
				case JsonReader.State.Object:
				case JsonReader.State.Closed:
				case JsonReader.State.Error:
				{
					throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
				}
				case JsonReader.State.Finished:
				{
					this.ReadFinished();
					nullable = null;
					return nullable;
				}
				default:
				{
					throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
				}
			}
		}

		public override byte[] ReadAsBytes()
		{
			this.EnsureBuffer();
			bool flag = false;
			switch (this._currentState)
			{
				case JsonReader.State.Start:
				case JsonReader.State.Property:
				case JsonReader.State.ArrayStart:
				case JsonReader.State.Array:
				case JsonReader.State.PostValue:
				case JsonReader.State.ConstructorStart:
				case JsonReader.State.Constructor:
				{
					do
					{
					Label0:
						char chr = this._chars[this._charPos];
						if (chr <= '\'')
						{
							if (chr > '\r')
							{
								if (chr == ' ')
								{
									goto Label2;
								}
								if (chr == '\"' || chr == '\'')
								{
									this.ParseString(chr, ReadType.ReadAsBytes);
									byte[] value = (byte[])this.Value;
									if (flag)
									{
										base.ReaderReadAndAssert();
										if (this.TokenType != JsonToken.EndObject)
										{
											throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, this.TokenType));
										}
										base.SetToken(JsonToken.Bytes, value, false);
									}
									return value;
								}
								goto Label1;
							}
							else
							{
								if (chr == 0)
								{
									continue;
								}
								switch (chr)
								{
									case '\t':
									{
										break;
									}
									case '\n':
									{
										this.ProcessLineFeed();
										goto Label0;
									}
									case '\r':
									{
										this.ProcessCarriageReturn(false);
										goto Label0;
									}
									default:
									{
										goto Label1;
									}
								}
							}
						Label2:
							this._charPos++;
							goto Label0;
						}
						else if (chr > '[')
						{
							if (chr == ']')
							{
								this._charPos++;
								if (this._currentState != JsonReader.State.Array && this._currentState != JsonReader.State.ArrayStart && this._currentState != JsonReader.State.PostValue)
								{
									throw this.CreateUnexpectedCharacterException(chr);
								}
								base.SetToken(JsonToken.EndArray);
								return null;
							}
							if (chr == 'n')
							{
								this.HandleNull();
								return null;
							}
							if (chr == '{')
							{
								this._charPos++;
								base.SetToken(JsonToken.StartObject);
								base.ReadIntoWrappedTypeObject();
								flag = true;
								goto Label0;
							}
						}
						else if (chr == ',')
						{
							this.ProcessValueComma();
							goto Label0;
						}
						else if (chr == '/')
						{
							this.ParseComment(false);
							goto Label0;
						}
						else if (chr == '[')
						{
							this._charPos++;
							base.SetToken(JsonToken.StartArray);
							return base.ReadArrayIntoByteArray();
						}
					Label1:
						this._charPos++;
						if (!char.IsWhiteSpace(chr))
						{
							throw this.CreateUnexpectedCharacterException(chr);
						}
						else
						{
							goto Label0;
						}
					}
					while (!this.ReadNullChar());
					base.SetToken(JsonToken.None, null, false);
					return null;
				}
				case JsonReader.State.Complete:
				case JsonReader.State.ObjectStart:
				case JsonReader.State.Object:
				case JsonReader.State.Closed:
				case JsonReader.State.Error:
				{
					throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
				}
				case JsonReader.State.Finished:
				{
					this.ReadFinished();
					return null;
				}
				default:
				{
					throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
				}
			}
		}

		public override DateTime? ReadAsDateTime()
		{
			return (DateTime?)this.ReadStringValue(ReadType.ReadAsDateTime);
		}

		public override DateTimeOffset? ReadAsDateTimeOffset()
		{
			return (DateTimeOffset?)this.ReadStringValue(ReadType.ReadAsDateTimeOffset);
		}

		public override decimal? ReadAsDecimal()
		{
			return (decimal?)this.ReadNumberValue(ReadType.ReadAsDecimal);
		}

		public override double? ReadAsDouble()
		{
			return (double?)this.ReadNumberValue(ReadType.ReadAsDouble);
		}

		public override int? ReadAsInt32()
		{
			return (int?)this.ReadNumberValue(ReadType.ReadAsInt32);
		}

		public override string ReadAsString()
		{
			return (string)this.ReadStringValue(ReadType.ReadAsString);
		}

		private bool ReadChars(int relativePosition, bool append)
		{
			if (this._isEndOfFile)
			{
				return false;
			}
			int num = this._charPos + relativePosition - this._charsUsed + 1;
			int num1 = 0;
			do
			{
				int num2 = this.ReadData(append, num - num1);
				if (num2 == 0)
				{
					break;
				}
				num1 += num2;
			}
			while (num1 < num);
			if (num1 < num)
			{
				return false;
			}
			return true;
		}

		private int ReadData(bool append)
		{
			return this.ReadData(append, 0);
		}

		private int ReadData(bool append, int charsRequired)
		{
			if (this._isEndOfFile)
			{
				return 0;
			}
			if (this._charsUsed + charsRequired >= (int)this._chars.Length - 1)
			{
				if (!append)
				{
					int num = this._charsUsed - this._charPos;
					if (num + charsRequired + 1 >= (int)this._chars.Length)
					{
						char[] chrArray = BufferUtils.RentBuffer(this._arrayPool, num + charsRequired + 1);
						if (num > 0)
						{
							JsonTextReader.BlockCopyChars(this._chars, this._charPos, chrArray, 0, num);
						}
						BufferUtils.ReturnBuffer(this._arrayPool, this._chars);
						this._chars = chrArray;
					}
					else if (num > 0)
					{
						JsonTextReader.BlockCopyChars(this._chars, this._charPos, this._chars, 0, num);
					}
					this._lineStartPos -= this._charPos;
					this._charPos = 0;
					this._charsUsed = num;
				}
				else
				{
					int num1 = Math.Max((int)this._chars.Length * 2, this._charsUsed + charsRequired + 1);
					char[] chrArray1 = BufferUtils.RentBuffer(this._arrayPool, num1);
					JsonTextReader.BlockCopyChars(this._chars, 0, chrArray1, 0, (int)this._chars.Length);
					BufferUtils.ReturnBuffer(this._arrayPool, this._chars);
					this._chars = chrArray1;
				}
			}
			int length = (int)this._chars.Length - this._charsUsed - 1;
			int num2 = this._reader.Read(this._chars, this._charsUsed, length);
			this._charsUsed += num2;
			if (num2 == 0)
			{
				this._isEndOfFile = true;
			}
			this._chars[this._charsUsed] = '\0';
			return num2;
		}

		private void ReadFinished()
		{
			if (this.EnsureChars(0, false))
			{
				this.EatWhitespace(false);
				if (this._isEndOfFile)
				{
					return;
				}
				if (this._chars[this._charPos] != '/')
				{
					throw JsonReaderException.Create(this, "Additional text encountered after finished reading JSON content: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
				}
				this.ParseComment(false);
			}
			base.SetToken(JsonToken.None);
		}

		private bool ReadNullChar()
		{
			if (this._charsUsed != this._charPos)
			{
				this._charPos++;
			}
			else if (this.ReadData(false) == 0)
			{
				this._isEndOfFile = true;
				return true;
			}
			return false;
		}

		private void ReadNumberIntoBuffer()
		{
			char chr;
			int num = this._charPos;
			while (true)
			{
				char chr1 = this._chars[num];
				if (chr1 <= 'F')
				{
					if (chr1 == 0)
					{
						this._charPos = num;
						if (this._charsUsed != num)
						{
							return;
						}
						if (this.ReadData(true) == 0)
						{
							return;
						}
					}
					else
					{
						switch (chr1)
						{
							case '+':
							case '-':
							case '.':
							case '0':
							case '1':
							case '2':
							case '3':
							case '4':
							case '5':
							case '6':
							case '7':
							case '8':
							case '9':
							case 'A':
							case 'B':
							case 'C':
							case 'D':
							case 'E':
							case 'F':
							{
								break;
							}
							case ',':
							case '/':
							case ':':
							case ';':
							case '<':
							case '=':
							case '>':
							case '?':
							case '@':
							{
								this._charPos = num;
								chr = this._chars[this._charPos];
								if (!char.IsWhiteSpace(chr) && chr != ',' && chr != '}' && chr != ']' && chr != ')' && chr != '/')
								{
									throw JsonReaderException.Create(this, "Unexpected character encountered while parsing number: {0}.".FormatWith(CultureInfo.InvariantCulture, chr));
								}
								return;
							}
							default:
							{
								this._charPos = num;
								chr = this._chars[this._charPos];
								if (!char.IsWhiteSpace(chr) && chr != ',' && chr != '}' && chr != ']' && chr != ')' && chr != '/')
								{
									throw JsonReaderException.Create(this, "Unexpected character encountered while parsing number: {0}.".FormatWith(CultureInfo.InvariantCulture, chr));
								}
								return;
							}
						}
					}
				}
				else if (chr1 != 'X')
				{
					switch (chr1)
					{
						case 'a':
						case 'b':
						case 'c':
						case 'd':
						case 'e':
						case 'f':
						{
							break;
						}
						default:
						{
							if (chr1 == 'x')
							{
								break;
							}
							this._charPos = num;
							chr = this._chars[this._charPos];
							if (!char.IsWhiteSpace(chr) && chr != ',' && chr != '}' && chr != ']' && chr != ')' && chr != '/')
							{
								throw JsonReaderException.Create(this, "Unexpected character encountered while parsing number: {0}.".FormatWith(CultureInfo.InvariantCulture, chr));
							}
							return;
						}
					}
				}
				num++;
			}
			this._charPos = num;
			chr = this._chars[this._charPos];
			if (!char.IsWhiteSpace(chr) && chr != ',' && chr != '}' && chr != ']' && chr != ')' && chr != '/')
			{
				throw JsonReaderException.Create(this, "Unexpected character encountered while parsing number: {0}.".FormatWith(CultureInfo.InvariantCulture, chr));
			}
		}

		private object ReadNumberValue(ReadType readType)
		{
			this.EnsureBuffer();
			switch (this._currentState)
			{
				case JsonReader.State.Start:
				case JsonReader.State.Property:
				case JsonReader.State.ArrayStart:
				case JsonReader.State.Array:
				case JsonReader.State.PostValue:
				case JsonReader.State.ConstructorStart:
				case JsonReader.State.Constructor:
				{
					do
					{
					Label0:
						char chr = this._chars[this._charPos];
						if (chr <= '9')
						{
							if (chr == 0)
							{
								continue;
							}
							switch (chr)
							{
								case '\t':
								{
								Label1:
									this._charPos++;
									goto Label0;
								}
								case '\n':
								{
									this.ProcessLineFeed();
									goto Label0;
								}
								case '\v':
								case '\f':
								{
									break;
								}
								case '\r':
								{
									this.ProcessCarriageReturn(false);
									goto Label0;
								}
								default:
								{
									switch (chr)
									{
										case ' ':
										{
											goto Label1;
										}
										case '\"':
										case '\'':
										{
											this.ParseString(chr, readType);
											if (readType == ReadType.ReadAsInt32)
											{
												return base.ReadInt32String(this._stringReference.ToString());
											}
											if (readType == ReadType.ReadAsDecimal)
											{
												return base.ReadDecimalString(this._stringReference.ToString());
											}
											if (readType != ReadType.ReadAsDouble)
											{
												throw new ArgumentOutOfRangeException("readType");
											}
											return base.ReadDoubleString(this._stringReference.ToString());
										}
										case ',':
										{
											this.ProcessValueComma();
											goto Label0;
										}
										case '-':
										{
											if (this.EnsureChars(1, true) && this._chars[this._charPos + 1] == 'I')
											{
												return this.ParseNumberNegativeInfinity(readType);
											}
											this.ParseNumber(readType);
											return this.Value;
										}
										case '.':
										case '0':
										case '1':
										case '2':
										case '3':
										case '4':
										case '5':
										case '6':
										case '7':
										case '8':
										case '9':
										{
											this.ParseNumber(readType);
											return this.Value;
										}
										case '/':
										{
											this.ParseComment(false);
											goto Label0;
										}
									}
									break;
								}
							}
						}
						else if (chr > 'N')
						{
							if (chr == ']')
							{
								this._charPos++;
								if (this._currentState != JsonReader.State.Array && this._currentState != JsonReader.State.ArrayStart && this._currentState != JsonReader.State.PostValue)
								{
									throw this.CreateUnexpectedCharacterException(chr);
								}
								base.SetToken(JsonToken.EndArray);
								return null;
							}
							if (chr == 'n')
							{
								this.HandleNull();
								return null;
							}
						}
						else
						{
							if (chr == 'I')
							{
								return this.ParseNumberPositiveInfinity(readType);
							}
							if (chr == 'N')
							{
								return this.ParseNumberNaN(readType);
							}
						}
						this._charPos++;
						if (!char.IsWhiteSpace(chr))
						{
							throw this.CreateUnexpectedCharacterException(chr);
						}
						else
						{
							goto Label0;
						}
					}
					while (!this.ReadNullChar());
					base.SetToken(JsonToken.None, null, false);
					return null;
				}
				case JsonReader.State.Complete:
				case JsonReader.State.ObjectStart:
				case JsonReader.State.Object:
				case JsonReader.State.Closed:
				case JsonReader.State.Error:
				{
					throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
				}
				case JsonReader.State.Finished:
				{
					this.ReadFinished();
					return null;
				}
				default:
				{
					throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
				}
			}
		}

		private void ReadStringIntoBuffer(char quote)
		{
			char chr;
			char chr1;
			bool flag;
			int num = this._charPos;
			int num1 = this._charPos;
			int num2 = this._charPos;
			this._stringBuffer.Position = 0;
			while (true)
			{
				int num3 = num;
				num = num3 + 1;
				char chr2 = this._chars[num3];
				if (chr2 > '\r')
				{
					if (chr2 != '\"' && chr2 != '\'')
					{
						if (chr2 == '\\')
						{
							this._charPos = num;
							if (!this.EnsureChars(0, true))
							{
								throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
							}
							int num4 = num - 1;
							chr = this._chars[num];
							num++;
							if (chr <= '\\')
							{
								if (chr > '\'')
								{
									if (chr == '/')
									{
										goto Label2;
									}
									if (chr == '\\')
									{
										chr1 = '\\';
										goto Label1;
									}
									else
									{
										break;
									}
								}
								else if (chr != '\"' && chr != '\'')
								{
									break;
								}
							Label2:
								chr1 = chr;
							}
							else if (chr > 'f')
							{
								if (chr == 'n')
								{
									chr1 = '\n';
								}
								else
								{
									switch (chr)
									{
										case 'r':
										{
											chr1 = '\r';
											break;
										}
										case 's':
										{
											this._charPos = num;
											throw JsonReaderException.Create(this, "Bad JSON escape sequence: {0}.".FormatWith(CultureInfo.InvariantCulture, string.Concat("\\", chr.ToString())));
										}
										case 't':
										{
											chr1 = '\t';
											break;
										}
										case 'u':
										{
											this._charPos = num;
											chr1 = this.ParseUnicode();
											if (StringUtils.IsLowSurrogate(chr1))
											{
												chr1 = '\uFFFD';
											}
											else if (StringUtils.IsHighSurrogate(chr1))
											{
												do
												{
													flag = false;
													if (!this.EnsureChars(2, true) || this._chars[this._charPos] != '\\' || this._chars[this._charPos + 1] != 'u')
													{
														chr1 = '\uFFFD';
													}
													else
													{
														char chr3 = chr1;
														this._charPos += 2;
														chr1 = this.ParseUnicode();
														if (!StringUtils.IsLowSurrogate(chr1))
														{
															if (!StringUtils.IsHighSurrogate(chr1))
															{
																chr3 = '\uFFFD';
															}
															else
															{
																chr3 = '\uFFFD';
																flag = true;
															}
														}
														this.EnsureBufferNotEmpty();
														this.WriteCharToBuffer(chr3, num2, num4);
														num2 = this._charPos;
													}
												}
												while (flag);
											}
											num = this._charPos;
											break;
										}
										default:
										{
											this._charPos = num;
											throw JsonReaderException.Create(this, "Bad JSON escape sequence: {0}.".FormatWith(CultureInfo.InvariantCulture, string.Concat("\\", chr.ToString())));
										}
									}
								}
							}
							else if (chr == 'b')
							{
								chr1 = '\b';
							}
							else if (chr == 'f')
							{
								chr1 = '\f';
							}
							else
							{
								break;
							}
						Label1:
							this.EnsureBufferNotEmpty();
							this.WriteCharToBuffer(chr1, num2, num4);
							num2 = num;
						}
					}
					else if (this._chars[num - 1] == quote)
					{
						num--;
						if (num1 != num2)
						{
							this.EnsureBufferNotEmpty();
							if (num > num2)
							{
								this._stringBuffer.Append(this._arrayPool, this._chars, num2, num - num2);
							}
							this._stringReference = new StringReference(this._stringBuffer.InternalBuffer, 0, this._stringBuffer.Position);
						}
						else
						{
							this._stringReference = new StringReference(this._chars, num1, num - num1);
						}
						num++;
						this._charPos = num;
						return;
					}
				}
				else if (chr2 == 0)
				{
					if (this._charsUsed == num - 1)
					{
						num--;
						if (this.ReadData(true) == 0)
						{
							this._charPos = num;
							throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
						}
					}
				}
				else if (chr2 == '\n')
				{
					this._charPos = num - 1;
					this.ProcessLineFeed();
					num = this._charPos;
				}
				else if (chr2 == '\r')
				{
					this._charPos = num - 1;
					this.ProcessCarriageReturn(true);
					num = this._charPos;
				}
			}
			this._charPos = num;
			throw JsonReaderException.Create(this, "Bad JSON escape sequence: {0}.".FormatWith(CultureInfo.InvariantCulture, string.Concat("\\", chr.ToString())));
		}

		private object ReadStringValue(ReadType readType)
		{
			char chr;
			this.EnsureBuffer();
			switch (this._currentState)
			{
				case JsonReader.State.Start:
				case JsonReader.State.Property:
				case JsonReader.State.ArrayStart:
				case JsonReader.State.Array:
				case JsonReader.State.PostValue:
				case JsonReader.State.ConstructorStart:
				case JsonReader.State.Constructor:
				{
					while (true)
					{
						chr = this._chars[this._charPos];
						if (chr <= 'I')
						{
							if (chr > '\r')
							{
								switch (chr)
								{
									case ' ':
									{
										break;
									}
									case '!':
									case '#':
									case '$':
									case '%':
									case '&':
									case '(':
									case ')':
									case '*':
									case '+':
									{
										goto Label0;
									}
									case '\"':
									case '\'':
									{
										this.ParseString(chr, readType);
										switch (readType)
										{
											case ReadType.ReadAsBytes:
											{
												return this.Value;
											}
											case ReadType.ReadAsString:
											{
												return this.Value;
											}
											case ReadType.ReadAsDecimal:
											{
												throw new ArgumentOutOfRangeException("readType");
											}
											case ReadType.ReadAsDateTime:
											{
												if (this.Value is DateTime)
												{
													return (DateTime)this.Value;
												}
												return base.ReadDateTimeString((string)this.Value);
											}
											case ReadType.ReadAsDateTimeOffset:
											{
												if (this.Value is DateTimeOffset)
												{
													return (DateTimeOffset)this.Value;
												}
												return base.ReadDateTimeOffsetString((string)this.Value);
											}
											default:
											{
												throw new ArgumentOutOfRangeException("readType");
											}
										}
										break;
									}
									case ',':
									{
										this.ProcessValueComma();
										continue;
									}
									case '-':
									{
										if (this.EnsureChars(1, true) && this._chars[this._charPos + 1] == 'I')
										{
											return this.ParseNumberNegativeInfinity(readType);
										}
										this.ParseNumber(readType);
										return this.Value;
									}
									case '.':
									case '0':
									case '1':
									case '2':
									case '3':
									case '4':
									case '5':
									case '6':
									case '7':
									case '8':
									case '9':
									{
										if (readType != ReadType.ReadAsString)
										{
											this._charPos++;
											throw this.CreateUnexpectedCharacterException(chr);
										}
										this.ParseNumber(ReadType.ReadAsString);
										return this.Value;
									}
									case '/':
									{
										this.ParseComment(false);
										continue;
									}
									default:
									{
										if (chr == 'I')
										{
											return this.ParseNumberPositiveInfinity(readType);
										}
										goto Label0;
									}
								}
							}
							else if (chr != 0)
							{
								switch (chr)
								{
									case '\t':
									{
										break;
									}
									case '\n':
									{
										this.ProcessLineFeed();
										continue;
									}
									case '\r':
									{
										this.ProcessCarriageReturn(false);
										continue;
									}
									default:
									{
										goto Label0;
									}
								}
							}
							else if (this.ReadNullChar())
							{
								base.SetToken(JsonToken.None, null, false);
								return null;
							}
							this._charPos++;
							continue;
						}
						else if (chr > ']')
						{
							if (chr == 'f')
							{
								break;
							}
							if (chr == 'n')
							{
								this.HandleNull();
								return null;
							}
							if (chr == 't')
							{
								break;
							}
						}
						else
						{
							if (chr == 'N')
							{
								return this.ParseNumberNaN(readType);
							}
							if (chr == ']')
							{
								this._charPos++;
								if (this._currentState != JsonReader.State.Array && this._currentState != JsonReader.State.ArrayStart && this._currentState != JsonReader.State.PostValue)
								{
									throw this.CreateUnexpectedCharacterException(chr);
								}
								base.SetToken(JsonToken.EndArray);
								return null;
							}
						}
					Label0:
						this._charPos++;
						if (!char.IsWhiteSpace(chr))
						{
							throw this.CreateUnexpectedCharacterException(chr);
						}
					}
					if (readType != ReadType.ReadAsString)
					{
						this._charPos++;
						throw this.CreateUnexpectedCharacterException(chr);
					}
					string str = (chr == 't' ? JsonConvert.True : JsonConvert.False);
					if (!this.MatchValueWithTrailingSeparator(str))
					{
						throw this.CreateUnexpectedCharacterException(this._chars[this._charPos]);
					}
					base.SetToken(JsonToken.String, str);
					return str;
				}
				case JsonReader.State.Complete:
				case JsonReader.State.ObjectStart:
				case JsonReader.State.Object:
				case JsonReader.State.Closed:
				case JsonReader.State.Error:
				{
					throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
				}
				case JsonReader.State.Finished:
				{
					this.ReadFinished();
					return null;
				}
				default:
				{
					throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
				}
			}
		}

		private void ShiftBufferIfNeeded()
		{
			int length = (int)this._chars.Length;
			if ((double)(length - this._charPos) <= (double)length * 0.1)
			{
				int num = this._charsUsed - this._charPos;
				if (num > 0)
				{
					JsonTextReader.BlockCopyChars(this._chars, this._charPos, this._chars, 0, num);
				}
				this._lineStartPos -= this._charPos;
				this._charPos = 0;
				this._charsUsed = num;
				this._chars[this._charsUsed] = '\0';
			}
		}

		private bool ValidIdentifierChar(char value)
		{
			if (char.IsLetterOrDigit(value) || value == '\u005F')
			{
				return true;
			}
			return value == '$';
		}

		private void WriteCharToBuffer(char writeChar, int lastWritePosition, int writeToPosition)
		{
			if (writeToPosition > lastWritePosition)
			{
				this._stringBuffer.Append(this._arrayPool, this._chars, lastWritePosition, writeToPosition - lastWritePosition);
			}
			this._stringBuffer.Append(this._arrayPool, writeChar);
		}
	}
}