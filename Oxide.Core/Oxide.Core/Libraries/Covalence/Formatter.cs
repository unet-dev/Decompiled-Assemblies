using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Oxide.Core.Libraries.Covalence
{
	public class Formatter
	{
		private readonly static Dictionary<string, string> colorNames;

		private readonly static Dictionary<ElementType, Formatter.TokenType?> closeTags;

		static Formatter()
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			strs["aqua"] = "00ffff";
			strs["black"] = "000000";
			strs["blue"] = "0000ff";
			strs["brown"] = "a52a2a";
			strs["cyan"] = "00ffff";
			strs["darkblue"] = "0000a0";
			strs["fuchsia"] = "ff00ff";
			strs["green"] = "008000";
			strs["grey"] = "808080";
			strs["lightblue"] = "add8e6";
			strs["lime"] = "00ff00";
			strs["magenta"] = "ff00ff";
			strs["maroon"] = "800000";
			strs["navy"] = "000080";
			strs["olive"] = "808000";
			strs["orange"] = "ffa500";
			strs["purple"] = "800080";
			strs["red"] = "ff0000";
			strs["silver"] = "c0c0c0";
			strs["teal"] = "008080";
			strs["white"] = "ffffff";
			strs["yellow"] = "ffff00";
			Formatter.colorNames = strs;
			Dictionary<ElementType, Formatter.TokenType?> elementTypes = new Dictionary<ElementType, Formatter.TokenType?>();
			elementTypes[ElementType.String] = null;
			elementTypes[ElementType.Bold] = new Formatter.TokenType?(Formatter.TokenType.CloseBold);
			elementTypes[ElementType.Italic] = new Formatter.TokenType?(Formatter.TokenType.CloseItalic);
			elementTypes[ElementType.Color] = new Formatter.TokenType?(Formatter.TokenType.CloseColor);
			elementTypes[ElementType.Size] = new Formatter.TokenType?(Formatter.TokenType.CloseSize);
			Formatter.closeTags = elementTypes;
		}

		public Formatter()
		{
		}

		private static List<Element> Parse(List<Formatter.Token> tokens)
		{
			int num = 0;
			Stack<Formatter.Entry> entries = new Stack<Formatter.Entry>();
			entries.Push(new Formatter.Entry(null, Element.Tag(ElementType.String)));
			while (num < tokens.Count)
			{
				int num1 = num;
				num = num1 + 1;
				Formatter.Token item = tokens[num1];
				Action<Element> action = (Element el) => entries.Push(new Formatter.Entry(item.Pattern, el));
				Element element = entries.Peek().Element;
				Formatter.TokenType type = item.Type;
				Formatter.TokenType? nullable = Formatter.closeTags[element.Type];
				if (!(type == nullable.GetValueOrDefault() & nullable.HasValue))
				{
					switch (item.Type)
					{
						case Formatter.TokenType.String:
						{
							element.Body.Add(Element.String(item.Val));
							continue;
						}
						case Formatter.TokenType.Bold:
						{
							action(Element.Tag(ElementType.Bold));
							continue;
						}
						case Formatter.TokenType.Italic:
						{
							action(Element.Tag(ElementType.Italic));
							continue;
						}
						case Formatter.TokenType.Color:
						{
							action(Element.ParamTag(ElementType.Color, item.Val));
							continue;
						}
						case Formatter.TokenType.Size:
						{
							action(Element.ParamTag(ElementType.Size, item.Val));
							continue;
						}
					}
					element.Body.Add(Element.String(item.Pattern));
				}
				else
				{
					entries.Pop();
					entries.Peek().Element.Body.Add(element);
				}
			}
			while (entries.Count > 1)
			{
				Formatter.Entry entry = entries.Pop();
				List<Element> body = entries.Peek().Element.Body;
				body.Add(Element.String(entry.Pattern));
				body.AddRange(entry.Element.Body);
			}
			return entries.Pop().Element.Body;
		}

		public static List<Element> Parse(string text)
		{
			return Formatter.Parse(Formatter.Lexer.Lex(text));
		}

		private static string RGBAtoRGB(object rgba)
		{
			return rgba.ToString().Substring(0, 6);
		}

		public static string ToPlaintext(string text)
		{
			return Formatter.ToTreeFormat(text, new Dictionary<ElementType, Func<object, Formatter.Tag>>());
		}

		public static string ToRoKAnd7DTD(string text)
		{
			string str = text;
			Dictionary<ElementType, Func<object, Formatter.Tag>> elementTypes = new Dictionary<ElementType, Func<object, Formatter.Tag>>();
			elementTypes[ElementType.Color] = (object c) => new Formatter.Tag(string.Concat("[", Formatter.RGBAtoRGB(c), "]"), "[e7e7e7]");
			return Formatter.ToTreeFormat(str, elementTypes);
		}

		public static string ToRustLegacy(string text)
		{
			string str = text;
			Dictionary<ElementType, Func<object, Formatter.Tag>> elementTypes = new Dictionary<ElementType, Func<object, Formatter.Tag>>();
			elementTypes[ElementType.Color] = (object c) => new Formatter.Tag(string.Concat("[color #", Formatter.RGBAtoRGB(c), "]"), "[color #ffffff]");
			return Formatter.ToTreeFormat(str, elementTypes);
		}

		public static string ToTerraria(string text)
		{
			string str = text;
			Dictionary<ElementType, Func<object, Formatter.Tag>> elementTypes = new Dictionary<ElementType, Func<object, Formatter.Tag>>();
			elementTypes[ElementType.Color] = (object c) => new Formatter.Tag(string.Concat("[c/", Formatter.RGBAtoRGB(c), ":"), "]");
			return Formatter.ToTreeFormat(str, elementTypes);
		}

		private static string ToTreeFormat(List<Element> tree, Dictionary<ElementType, Func<object, Formatter.Tag>> translations)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Element element in tree)
			{
				if (element.Type != ElementType.String)
				{
					Formatter.Tag tag = Formatter.Translation(element, translations);
					stringBuilder.Append(tag.Open);
					stringBuilder.Append(Formatter.ToTreeFormat(element.Body, translations));
					stringBuilder.Append(tag.Close);
				}
				else
				{
					stringBuilder.Append(element.Val);
				}
			}
			return stringBuilder.ToString();
		}

		private static string ToTreeFormat(string text, Dictionary<ElementType, Func<object, Formatter.Tag>> translations)
		{
			return Formatter.ToTreeFormat(Formatter.Parse(text), translations);
		}

		public static string ToUnity(string text)
		{
			string str = text;
			Dictionary<ElementType, Func<object, Formatter.Tag>> elementTypes = new Dictionary<ElementType, Func<object, Formatter.Tag>>();
			elementTypes[ElementType.Bold] = (object _) => new Formatter.Tag("<b>", "</b>");
			elementTypes[ElementType.Italic] = (object _) => new Formatter.Tag("<i>", "</i>");
			elementTypes[ElementType.Color] = (object c) => new Formatter.Tag(string.Format("<color=#{0}>", c), "</color>");
			elementTypes[ElementType.Size] = (object s) => new Formatter.Tag(string.Format("<size={0}>", s), "</size>");
			return Formatter.ToTreeFormat(str, elementTypes);
		}

		private static Formatter.Tag Translation(Element e, Dictionary<ElementType, Func<object, Formatter.Tag>> translations)
		{
			Func<object, Formatter.Tag> func;
			if (!translations.TryGetValue(e.Type, out func))
			{
				return new Formatter.Tag("", "");
			}
			return func(e.Val);
		}

		private class Entry
		{
			public string Pattern;

			public Element Element;

			public Entry(string pattern, Element e)
			{
				this.Pattern = pattern;
				this.Element = e;
			}
		}

		private class Lexer
		{
			private string text;

			private int patternStart;

			private int tokenStart;

			private int position;

			private List<Formatter.Token> tokens;

			public Lexer()
			{
			}

			private void Add(Formatter.TokenType type, object val = null)
			{
				Formatter.Token token = new Formatter.Token()
				{
					Type = type,
					Val = val,
					Pattern = this.text.Substring(this.patternStart, this.position - this.patternStart)
				};
				this.tokens.Add(token);
			}

			private Formatter.Lexer.State CloseTag()
			{
				char chr = this.Current();
				if (chr > '+')
				{
					if (chr == 'b')
					{
						return this.EndTag(Formatter.TokenType.CloseBold);
					}
					if (chr == 'i')
					{
						return this.EndTag(Formatter.TokenType.CloseItalic);
					}
				}
				else
				{
					if (chr == '#')
					{
						return this.EndTag(Formatter.TokenType.CloseColor);
					}
					if (chr == '+')
					{
						return this.EndTag(Formatter.TokenType.CloseSize);
					}
				}
				this.Reset();
				return new Formatter.Lexer.State(this.Str);
			}

			private char Current()
			{
				return this.text[this.position];
			}

			private Formatter.Lexer.State EndTag(Formatter.TokenType t)
			{
				this.Next();
				return () => {
					if (this.Current() != ']')
					{
						this.Reset();
						return new Formatter.Lexer.State(this.Str);
					}
					this.Next();
					this.Add(t, null);
					this.StartNewPattern();
					return new Formatter.Lexer.State(this.Str);
				};
			}

			private static bool IsValidColorCode(string val)
			{
				if (val.Length != 6 && val.Length != 8)
				{
					return false;
				}
				return val.All<char>((char c) => {
					if (c >= '0' && c <= '9' || c >= 'a' && c <= 'f')
					{
						return true;
					}
					if (c < 'A')
					{
						return false;
					}
					return c <= 'F';
				});
			}

			public static List<Formatter.Token> Lex(string text)
			{
				Formatter.Lexer lexer = new Formatter.Lexer()
				{
					text = text
				};
				Formatter.Lexer.State state = new Formatter.Lexer.State(lexer.Str);
				while (lexer.position < lexer.text.Length)
				{
					state = state();
				}
				lexer.WritePatternString();
				return lexer.tokens;
			}

			private void Next()
			{
				this.position++;
			}

			private Formatter.Lexer.State ParamTag(Formatter.TokenType t, Func<string, object> parse)
			{
				this.Next();
				this.StartNewToken();
				Formatter.Lexer.State state = null;
				state = () => {
					if (this.Current() != ']')
					{
						this.Next();
						return state;
					}
					object obj = parse(this.Token());
					if (obj == null)
					{
						this.Reset();
						return new Formatter.Lexer.State(this.Str);
					}
					this.Next();
					this.Add(t, obj);
					this.StartNewPattern();
					return new Formatter.Lexer.State(this.Str);
				};
				return state;
			}

			private static object ParseColor(string val)
			{
				string str;
				if (!Formatter.colorNames.TryGetValue(val.ToLower(), out str) && !Formatter.Lexer.IsValidColorCode(val))
				{
					return null;
				}
				str = str ?? val;
				if (str.Length == 6)
				{
					str = string.Concat(str, "ff");
				}
				return str;
			}

			private static object ParseSize(string val)
			{
				int num;
				if (!int.TryParse(val, out num))
				{
					return null;
				}
				return num;
			}

			private void Reset()
			{
				this.tokenStart = this.patternStart;
			}

			private void StartNewPattern()
			{
				this.patternStart = this.position;
				this.StartNewToken();
			}

			private void StartNewToken()
			{
				this.tokenStart = this.position;
			}

			private Formatter.Lexer.State Str()
			{
				if (this.Current() != '[')
				{
					this.Next();
					return new Formatter.Lexer.State(this.Str);
				}
				this.WritePatternString();
				this.StartNewPattern();
				this.Next();
				return new Formatter.Lexer.State(this.Tag);
			}

			private Formatter.Lexer.State Tag()
			{
				char chr = this.Current();
				if (chr > '+')
				{
					if (chr == '/')
					{
						this.Next();
						return new Formatter.Lexer.State(this.CloseTag);
					}
					if (chr == 'b')
					{
						return this.EndTag(Formatter.TokenType.Bold);
					}
					if (chr == 'i')
					{
						return this.EndTag(Formatter.TokenType.Italic);
					}
				}
				else
				{
					if (chr == '#')
					{
						return this.ParamTag(Formatter.TokenType.Color, new Func<string, object>(Formatter.Lexer.ParseColor));
					}
					if (chr == '+')
					{
						return this.ParamTag(Formatter.TokenType.Size, new Func<string, object>(Formatter.Lexer.ParseSize));
					}
				}
				this.Reset();
				return new Formatter.Lexer.State(this.Str);
			}

			private string Token()
			{
				return this.text.Substring(this.tokenStart, this.position - this.tokenStart);
			}

			private void WritePatternString()
			{
				if (this.patternStart >= this.position)
				{
					return;
				}
				int num = this.tokenStart;
				this.tokenStart = this.patternStart;
				this.Add(Formatter.TokenType.String, this.Token());
				this.tokenStart = num;
			}

			private delegate Formatter.Lexer.State State();
		}

		private class Tag
		{
			public string Open;

			public string Close;

			public Tag(string open, string close)
			{
				this.Open = open;
				this.Close = close;
			}
		}

		private class Token
		{
			public Formatter.TokenType Type;

			public object Val;

			public string Pattern;

			public Token()
			{
			}
		}

		private enum TokenType
		{
			String,
			Bold,
			Italic,
			Color,
			Size,
			CloseBold,
			CloseItalic,
			CloseColor,
			CloseSize
		}
	}
}