using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameAnalyticsSDK.Utilities
{
	public class GA_MiniJSON
	{
		public GA_MiniJSON()
		{
		}

		public static object Deserialize(string json)
		{
			if (json == null)
			{
				return null;
			}
			return GA_MiniJSON.Parser.Parse(json);
		}

		public static string Serialize(object obj)
		{
			return GA_MiniJSON.Serializer.Serialize(obj);
		}

		private sealed class Parser : IDisposable
		{
			private const string WORD_BREAK = "{}[],:\"";

			private StringReader json;

			private char NextChar
			{
				get
				{
					return Convert.ToChar(this.json.Read());
				}
			}

			private GA_MiniJSON.Parser.TOKEN NextToken
			{
				get
				{
					this.EatWhitespace();
					if (this.json.Peek() == -1)
					{
						return GA_MiniJSON.Parser.TOKEN.NONE;
					}
					char peekChar = this.PeekChar;
					if (peekChar > '[')
					{
						if (peekChar == ']')
						{
							this.json.Read();
							return GA_MiniJSON.Parser.TOKEN.SQUARED_CLOSE;
						}
						if (peekChar == '{')
						{
							return GA_MiniJSON.Parser.TOKEN.CURLY_OPEN;
						}
						if (peekChar == '}')
						{
							this.json.Read();
							return GA_MiniJSON.Parser.TOKEN.CURLY_CLOSE;
						}
					}
					else
					{
						switch (peekChar)
						{
							case '\"':
							{
								return GA_MiniJSON.Parser.TOKEN.STRING;
							}
							case '#':
							case '$':
							case '%':
							case '&':
							case '\'':
							case '(':
							case ')':
							case '*':
							case '+':
							case '.':
							case '/':
							{
								break;
							}
							case ',':
							{
								this.json.Read();
								return GA_MiniJSON.Parser.TOKEN.COMMA;
							}
							case '-':
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
								return GA_MiniJSON.Parser.TOKEN.NUMBER;
							}
							case ':':
							{
								return GA_MiniJSON.Parser.TOKEN.COLON;
							}
							default:
							{
								if (peekChar == '[')
								{
									return GA_MiniJSON.Parser.TOKEN.SQUARED_OPEN;
								}
								break;
							}
						}
					}
					string nextWord = this.NextWord;
					if (nextWord == "false")
					{
						return GA_MiniJSON.Parser.TOKEN.FALSE;
					}
					if (nextWord == "true")
					{
						return GA_MiniJSON.Parser.TOKEN.TRUE;
					}
					if (nextWord == "null")
					{
						return GA_MiniJSON.Parser.TOKEN.NULL;
					}
					return GA_MiniJSON.Parser.TOKEN.NONE;
				}
			}

			private string NextWord
			{
				get
				{
					StringBuilder stringBuilder = new StringBuilder();
					do
					{
						if (GA_MiniJSON.Parser.IsWordBreak(this.PeekChar))
						{
							break;
						}
						stringBuilder.Append(this.NextChar);
					}
					while (this.json.Peek() != -1);
					return stringBuilder.ToString();
				}
			}

			private char PeekChar
			{
				get
				{
					return Convert.ToChar(this.json.Peek());
				}
			}

			private Parser(string jsonString)
			{
				this.json = new StringReader(jsonString);
			}

			public void Dispose()
			{
				this.json.Dispose();
				this.json = null;
			}

			private void EatWhitespace()
			{
				do
				{
					if (!char.IsWhiteSpace(this.PeekChar))
					{
						break;
					}
					this.json.Read();
				}
				while (this.json.Peek() != -1);
			}

			public static bool IsWordBreak(char c)
			{
				if (char.IsWhiteSpace(c))
				{
					return true;
				}
				return "{}[],:\"".IndexOf(c) != -1;
			}

			public static object Parse(string jsonString)
			{
				object obj;
				using (GA_MiniJSON.Parser parser = new GA_MiniJSON.Parser(jsonString))
				{
					obj = parser.ParseValue();
				}
				return obj;
			}

			private List<object> ParseArray()
			{
				List<object> objs = new List<object>();
				this.json.Read();
				bool flag = true;
				while (flag)
				{
					GA_MiniJSON.Parser.TOKEN nextToken = this.NextToken;
					if (nextToken == GA_MiniJSON.Parser.TOKEN.NONE)
					{
						return null;
					}
					if (nextToken == GA_MiniJSON.Parser.TOKEN.SQUARED_CLOSE)
					{
						flag = false;
					}
					else
					{
						if (nextToken == GA_MiniJSON.Parser.TOKEN.COMMA)
						{
							continue;
						}
						objs.Add(this.ParseByToken(nextToken));
					}
				}
				return objs;
			}

			private object ParseByToken(GA_MiniJSON.Parser.TOKEN token)
			{
				switch (token)
				{
					case GA_MiniJSON.Parser.TOKEN.CURLY_OPEN:
					{
						return this.ParseObject();
					}
					case GA_MiniJSON.Parser.TOKEN.CURLY_CLOSE:
					case GA_MiniJSON.Parser.TOKEN.SQUARED_CLOSE:
					case GA_MiniJSON.Parser.TOKEN.COLON:
					case GA_MiniJSON.Parser.TOKEN.COMMA:
					{
						return null;
					}
					case GA_MiniJSON.Parser.TOKEN.SQUARED_OPEN:
					{
						return this.ParseArray();
					}
					case GA_MiniJSON.Parser.TOKEN.STRING:
					{
						return this.ParseString();
					}
					case GA_MiniJSON.Parser.TOKEN.NUMBER:
					{
						return this.ParseNumber();
					}
					case GA_MiniJSON.Parser.TOKEN.TRUE:
					{
						return true;
					}
					case GA_MiniJSON.Parser.TOKEN.FALSE:
					{
						return false;
					}
					case GA_MiniJSON.Parser.TOKEN.NULL:
					{
						return null;
					}
					default:
					{
						return null;
					}
				}
			}

			private object ParseNumber()
			{
				double num;
				long num1;
				string nextWord = this.NextWord;
				if (nextWord.IndexOf('.') == -1)
				{
					long.TryParse(nextWord, out num1);
					return num1;
				}
				double.TryParse(nextWord, out num);
				return num;
			}

			private Dictionary<string, object> ParseObject()
			{
				Dictionary<string, object> strs = new Dictionary<string, object>();
				this.json.Read();
				while (true)
				{
					GA_MiniJSON.Parser.TOKEN nextToken = this.NextToken;
					if (nextToken == GA_MiniJSON.Parser.TOKEN.NONE)
					{
						return null;
					}
					if (nextToken == GA_MiniJSON.Parser.TOKEN.CURLY_CLOSE)
					{
						return strs;
					}
					if (nextToken != GA_MiniJSON.Parser.TOKEN.COMMA)
					{
						string str = this.ParseString();
						if (str == null)
						{
							return null;
						}
						if (this.NextToken != GA_MiniJSON.Parser.TOKEN.COLON)
						{
							break;
						}
						this.json.Read();
						strs[str] = this.ParseValue();
					}
				}
				return null;
			}

			private string ParseString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				this.json.Read();
				bool flag = true;
				while (flag)
				{
					if (this.json.Peek() != -1)
					{
						char nextChar = this.NextChar;
						if (nextChar == '\"')
						{
							flag = false;
						}
						else if (nextChar != '\\')
						{
							stringBuilder.Append(nextChar);
						}
						else if (this.json.Peek() != -1)
						{
							nextChar = this.NextChar;
							if (nextChar <= '\\')
							{
								if (nextChar == '\"' || nextChar == '/' || nextChar == '\\')
								{
									stringBuilder.Append(nextChar);
								}
							}
							else if (nextChar > 'f')
							{
								if (nextChar == 'n')
								{
									stringBuilder.Append('\n');
								}
								else
								{
									switch (nextChar)
									{
										case 'r':
										{
											stringBuilder.Append('\r');
											continue;
										}
										case 't':
										{
											stringBuilder.Append('\t');
											continue;
										}
										case 'u':
										{
											char[] chrArray = new char[4];
											for (int i = 0; i < 4; i++)
											{
												chrArray[i] = this.NextChar;
											}
											stringBuilder.Append((char)Convert.ToInt32(new string(chrArray), 16));
											continue;
										}
										default:
										{
											continue;
										}
									}
								}
							}
							else if (nextChar == 'b')
							{
								stringBuilder.Append('\b');
							}
							else if (nextChar == 'f')
							{
								stringBuilder.Append('\f');
							}
						}
						else
						{
							flag = false;
						}
					}
					else
					{
						flag = false;
						break;
					}
				}
				return stringBuilder.ToString();
			}

			private object ParseValue()
			{
				return this.ParseByToken(this.NextToken);
			}

			private enum TOKEN
			{
				NONE,
				CURLY_OPEN,
				CURLY_CLOSE,
				SQUARED_OPEN,
				SQUARED_CLOSE,
				COLON,
				COMMA,
				STRING,
				NUMBER,
				TRUE,
				FALSE,
				NULL
			}
		}

		private sealed class Serializer
		{
			private StringBuilder builder;

			private Serializer()
			{
				this.builder = new StringBuilder();
			}

			public static string Serialize(object obj)
			{
				GA_MiniJSON.Serializer serializer = new GA_MiniJSON.Serializer();
				serializer.SerializeValue(obj);
				return serializer.builder.ToString();
			}

			private void SerializeArray(IList anArray)
			{
				this.builder.Append('[');
				bool flag = true;
				foreach (object obj in anArray)
				{
					if (!flag)
					{
						this.builder.Append(',');
					}
					this.SerializeValue(obj);
					flag = false;
				}
				this.builder.Append(']');
			}

			private void SerializeObject(IDictionary obj)
			{
				bool flag = true;
				this.builder.Append('{');
				foreach (object key in obj.Keys)
				{
					if (!flag)
					{
						this.builder.Append(',');
					}
					this.SerializeString(key.ToString());
					this.builder.Append(':');
					this.SerializeValue(obj[key]);
					flag = false;
				}
				this.builder.Append('}');
			}

			private void SerializeOther(object value)
			{
				if (value is float)
				{
					StringBuilder stringBuilder = this.builder;
					float single = (float)value;
					stringBuilder.Append(single.ToString("R"));
					return;
				}
				if (value is int || value is uint || value is long || value is sbyte || value is byte || value is short || value is ushort || value is ulong)
				{
					this.builder.Append(value);
					return;
				}
				if (!(value is double) && !(value is decimal))
				{
					this.SerializeString(value.ToString());
					return;
				}
				StringBuilder stringBuilder1 = this.builder;
				double num = Convert.ToDouble(value);
				stringBuilder1.Append(num.ToString("R"));
			}

			private void SerializeString(string str)
			{
				this.builder.Append('\"');
				char[] charArray = str.ToCharArray();
				for (int i = 0; i < (int)charArray.Length; i++)
				{
					char chr = charArray[i];
					switch (chr)
					{
						case '\b':
						{
							this.builder.Append("\\b");
							break;
						}
						case '\t':
						{
							this.builder.Append("\\t");
							break;
						}
						case '\n':
						{
							this.builder.Append("\\n");
							break;
						}
						case '\v':
						{
							int num = Convert.ToInt32(chr);
							if (num < 32 || num > 126)
							{
								this.builder.Append("\\u");
								this.builder.Append(num.ToString("x4"));
								break;
							}
							else
							{
								this.builder.Append(chr);
								break;
							}
						}
						case '\f':
						{
							this.builder.Append("\\f");
							break;
						}
						case '\r':
						{
							this.builder.Append("\\r");
							break;
						}
						default:
						{
							if (chr == '\"')
							{
								this.builder.Append("\\\"");
								break;
							}
							else if (chr == '\\')
							{
								this.builder.Append("\\\\");
								break;
							}
							else
							{
								goto case '\v';
							}
						}
					}
				}
				this.builder.Append('\"');
			}

			private void SerializeValue(object value)
			{
				if (value == null)
				{
					this.builder.Append("null");
					return;
				}
				string str = value as string;
				string str1 = str;
				if (str != null)
				{
					this.SerializeString(str1);
					return;
				}
				if (value is bool)
				{
					this.builder.Append(((bool)value ? "true" : "false"));
					return;
				}
				IList lists = value as IList;
				IList lists1 = lists;
				if (lists != null)
				{
					this.SerializeArray(lists1);
					return;
				}
				IDictionary dictionaries = value as IDictionary;
				IDictionary dictionaries1 = dictionaries;
				if (dictionaries != null)
				{
					this.SerializeObject(dictionaries1);
					return;
				}
				if (!(value is char))
				{
					this.SerializeOther(value);
					return;
				}
				this.SerializeString(new string((char)value, 1));
			}
		}
	}
}