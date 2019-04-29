using Apex.Serialization;
using System;
using System.Globalization;

namespace Apex.Serialization.Json
{
	internal sealed class JsonParser : IJsonParser
	{
		private string _s;

		private int _idx;

		private int _length;

		private int _valStart;

		private int _valEnd;

		private StringBuffer _b;

		private char[] _hexBuffer;

		private StageContainer _curRoot;

		internal JsonParser()
		{
			this._b = new StringBuffer(1024);
			this._hexBuffer = new char[4];
		}

		public StageElement Parse(string json)
		{
			this._s = json;
			this._length = json.Length;
			this._b.position = 0;
			this._curRoot = null;
			this._idx = 0;
			while (this._idx < this._length)
			{
				if (this._s[this._idx] == '{')
				{
					this._idx++;
					this.ParseElement(null);
				}
				this._idx++;
			}
			return this._curRoot as StageElement;
		}

		private void ParseElement(string name)
		{
			StageElement stageElement = new StageElement(name);
			if (this._curRoot != null)
			{
				this._curRoot.Add(stageElement);
			}
			this._curRoot = stageElement;
			while (this._idx < this._length)
			{
				char chr = this._s[this._idx];
				if (chr > ' ')
				{
					if (chr == ',')
					{
						goto Label0;
					}
					if (chr == '}')
					{
						this._curRoot = this._curRoot.parent ?? this._curRoot;
						return;
					}
				}
				else
				{
					switch (chr)
					{
						case '\t':
						case '\n':
						case '\f':
						case '\r':
						{
							goto Label0;
						}
						case '\v':
						{
							break;
						}
						default:
						{
							if (chr == ' ')
							{
								goto Label0;
							}
							break;
						}
					}
				}
				this.ParseItem();
			Label0:
				this._idx++;
			}
		}

		private void ParseItem()
		{
			bool flag = false;
			while (this._idx < this._length)
			{
				char chr = this._s[this._idx];
				if (chr <= ' ')
				{
					switch (chr)
					{
						case '\t':
						case '\n':
						case '\v':
						case '\f':
						case '\r':
						{
							break;
						}
						default:
						{
							if (chr == ' ')
							{
								break;
							}
							break;
						}
					}
				}
				else if (chr == '\"')
				{
					this._idx++;
					if (this._s[this._idx] == '@')
					{
						flag = true;
						this._idx++;
					}
					this.ParseString();
				}
				else if (chr == ':')
				{
					this._idx++;
					this.ParseItemType(this._b.Flush(), flag);
					return;
				}
				this._idx++;
			}
		}

		private void ParseItemType(string name, bool isAttribute)
		{
			StageValue stageAttribute;
			while (true)
			{
				if (this._idx >= this._length)
				{
					return;
				}
				char chr = this._s[this._idx];
				if (chr > ' ')
				{
					if (chr == '\"')
					{
						this._idx++;
						this.ParseString();
						if (isAttribute)
						{
							stageAttribute = new StageAttribute(name, this._b.Flush(), true);
						}
						else
						{
							stageAttribute = new StageValue(name, this._b.Flush(), true);
						}
						this._curRoot.Add(stageAttribute);
						return;
					}
					if (chr == '[')
					{
						this._idx++;
						this.ParseList(name);
						return;
					}
					if (chr != '{')
					{
						break;
					}
					this._idx++;
					this.ParseElement(name);
					return;
				}
				else
				{
					switch (chr)
					{
						case '\t':
						case '\n':
						case '\f':
						case '\r':
						{
							this._idx++;
							continue;
						}
						case '\v':
						{
							break;
						}
						default:
						{
							if (chr == ' ')
							{
								goto case '\r';
							}
							break;
						}
					}
				}
			}
			this.ParseValue(name, isAttribute);
		}

		private void ParseList(string name)
		{
			StageList stageList = new StageList(name);
			this._curRoot.Add(stageList);
			this._curRoot = stageList;
			while (this._idx < this._length)
			{
				char chr = this._s[this._idx];
				if (chr > ' ')
				{
					if (chr == ',')
					{
						goto Label0;
					}
					if (chr == ']')
					{
						this._curRoot = this._curRoot.parent ?? this._curRoot;
						return;
					}
				}
				else
				{
					switch (chr)
					{
						case '\t':
						case '\n':
						case '\f':
						case '\r':
						{
							goto Label0;
						}
						case '\v':
						{
							break;
						}
						default:
						{
							if (chr == ' ')
							{
								goto Label0;
							}
							break;
						}
					}
				}
				this.ParseItemType(null, false);
			Label0:
				this._idx++;
			}
		}

		private void ParseString()
		{
			this._valStart = this._idx;
			this._valEnd = -1;
			while (this._idx < this._length)
			{
				char chr = this._s[this._idx];
				if (chr == '\"')
				{
					if (this._valEnd >= 0)
					{
						this._b.Append(this._s, this._valStart, this._valEnd - this._valStart + 1);
					}
					return;
				}
				if (chr == '\\')
				{
					if (this._valEnd >= 0)
					{
						this._b.Append(this._s, this._valStart, this._valEnd - this._valStart + 1);
						this._valEnd = -1;
					}
					this._idx++;
					this.UnescapeChar();
					this._valStart = this._idx + 1;
				}
				else
				{
					this._valEnd = this._idx;
				}
				this._idx++;
			}
		}

		private void ParseValue(string name, bool isAttribute)
		{
			StageValue stageAttribute;
			this._valStart = this._idx;
			this._valEnd = -1;
			while (this._idx < this._length)
			{
				char chr = this._s[this._idx];
				if (chr <= ' ')
				{
					switch (chr)
					{
						case '\t':
						case '\n':
						case '\f':
						case '\r':
						{
							if (this._valEnd != -1)
							{
								goto Label0;
							}
							this._valStart++;
							goto Label0;
						}
						case '\v':
						{
							break;
						}
						default:
						{
							if (chr == ' ')
							{
								goto case '\r';
							}
							break;
						}
					}
				}
				else if (chr == ',' || chr == ']' || chr == '}')
				{
					if (this._valEnd >= 0)
					{
						this._b.Append(this._s, this._valStart, this._valEnd - this._valStart + 1);
					}
					if (isAttribute)
					{
						stageAttribute = new StageAttribute(name, this._b.Flush(), false);
					}
					else
					{
						stageAttribute = new StageValue(name, this._b.Flush(), false);
					}
					this._curRoot.Add(stageAttribute);
					this._idx--;
					return;
				}
				this._valEnd = this._idx;
			Label0:
				this._idx++;
			}
		}

		private void UnescapeChar()
		{
			char chr = this._s[this._idx];
			if (chr <= '\\')
			{
				if (chr != '\"')
				{
					if (chr == '0')
					{
						this._b.Append('\0');
						return;
					}
					if (chr != '\\')
					{
						return;
					}
				}
				this._b.Append(chr);
				return;
			}
			if (chr <= 'f')
			{
				if (chr == 'b')
				{
					this._b.Append('\b');
					return;
				}
				if (chr != 'f')
				{
					return;
				}
				this._b.Append('\f');
				return;
			}
			if (chr == 'n')
			{
				this._b.Append('\n');
				return;
			}
			switch (chr)
			{
				case 'r':
				{
					this._b.Append('\r');
					return;
				}
				case 's':
				{
					return;
				}
				case 't':
				{
					this._b.Append('\t');
					return;
				}
				case 'u':
				{
					this._s.CopyTo(this._idx + 1, this._hexBuffer, 0, 4);
					char chr1 = Convert.ToChar(int.Parse(new string(this._hexBuffer), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo));
					this._b.Append(chr1);
					this._idx += 4;
					return;
				}
				default:
				{
					return;
				}
			}
		}
	}
}