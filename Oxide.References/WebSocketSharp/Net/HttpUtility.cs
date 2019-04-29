using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Threading;
using WebSocketSharp;

namespace WebSocketSharp.Net
{
	internal sealed class HttpUtility
	{
		private static Dictionary<string, char> _entities;

		private static char[] _hexChars;

		private static object _sync;

		static HttpUtility()
		{
			HttpUtility._hexChars = "0123456789abcdef".ToCharArray();
			HttpUtility._sync = new object();
		}

		public HttpUtility()
		{
		}

		internal static Uri CreateRequestUrl(string requestUri, string host, bool websocketRequest, bool secure)
		{
			Uri uri;
			Uri uri1;
			Uri uri2;
			bool flag;
			if ((requestUri == null || requestUri.Length == 0 || host == null ? false : host.Length != 0))
			{
				string str = null;
				string pathAndQuery = null;
				if (requestUri.StartsWith("/"))
				{
					pathAndQuery = requestUri;
				}
				else if (requestUri.MaybeUri())
				{
					if (!Uri.TryCreate(requestUri, UriKind.Absolute, out uri2))
					{
						flag = false;
					}
					else
					{
						string scheme = uri2.Scheme;
						str = scheme;
						flag = (!scheme.StartsWith("http") || websocketRequest ? str.StartsWith("ws") & websocketRequest : true);
					}
					if (!flag)
					{
						uri1 = null;
						return uri1;
					}
					host = uri2.Authority;
					pathAndQuery = uri2.PathAndQuery;
				}
				else if (requestUri != "*")
				{
					host = requestUri;
				}
				if (str == null)
				{
					str = string.Concat((websocketRequest ? "ws" : "http"), (secure ? "s" : string.Empty));
				}
				if (host.IndexOf(':') == -1)
				{
					host = string.Format("{0}:{1}", host, (str == "http" || str == "ws" ? 80 : 443));
				}
				if (Uri.TryCreate(string.Format("{0}://{1}{2}", str, host, pathAndQuery), UriKind.Absolute, out uri))
				{
					uri1 = uri;
				}
				else
				{
					uri1 = null;
				}
			}
			else
			{
				uri1 = null;
			}
			return uri1;
		}

		internal static IPrincipal CreateUser(string response, AuthenticationSchemes scheme, string realm, string method, Func<IIdentity, NetworkCredential> credentialsFinder)
		{
			IPrincipal genericPrincipal;
			if ((response == null ? true : response.Length == 0))
			{
				genericPrincipal = null;
			}
			else if (credentialsFinder == null)
			{
				genericPrincipal = null;
			}
			else if ((scheme == AuthenticationSchemes.Basic ? true : scheme == AuthenticationSchemes.Digest))
			{
				if (scheme == AuthenticationSchemes.Digest)
				{
					if ((realm == null ? true : realm.Length == 0))
					{
						genericPrincipal = null;
						return genericPrincipal;
					}
					else if ((method == null ? true : method.Length == 0))
					{
						genericPrincipal = null;
						return genericPrincipal;
					}
				}
				if (response.StartsWith(scheme.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					AuthenticationResponse authenticationResponse = AuthenticationResponse.Parse(response);
					if (authenticationResponse != null)
					{
						IIdentity identity = authenticationResponse.ToIdentity();
						if (identity != null)
						{
							NetworkCredential networkCredential = null;
							try
							{
								networkCredential = credentialsFinder(identity);
							}
							catch
							{
							}
							if (networkCredential == null)
							{
								genericPrincipal = null;
							}
							else if ((scheme != AuthenticationSchemes.Basic ? false : ((HttpBasicIdentity)identity).Password != networkCredential.Password))
							{
								genericPrincipal = null;
							}
							else if ((scheme != AuthenticationSchemes.Digest ? true : ((HttpDigestIdentity)identity).IsValid(networkCredential.Password, realm, method, null)))
							{
								genericPrincipal = new GenericPrincipal(identity, networkCredential.Roles);
							}
							else
							{
								genericPrincipal = null;
							}
						}
						else
						{
							genericPrincipal = null;
						}
					}
					else
					{
						genericPrincipal = null;
					}
				}
				else
				{
					genericPrincipal = null;
				}
			}
			else
			{
				genericPrincipal = null;
			}
			return genericPrincipal;
		}

		private static int getChar(byte[] bytes, int offset, int length)
		{
			int num;
			int num1 = 0;
			int num2 = length + offset;
			int num3 = offset;
			while (true)
			{
				if (num3 < num2)
				{
					int num4 = HttpUtility.getInt(bytes[num3]);
					if (num4 != -1)
					{
						num1 = (num1 << 4) + num4;
						num3++;
					}
					else
					{
						num = -1;
						break;
					}
				}
				else
				{
					num = num1;
					break;
				}
			}
			return num;
		}

		private static int getChar(string s, int offset, int length)
		{
			int num;
			int num1 = 0;
			int num2 = length + offset;
			int num3 = offset;
			while (true)
			{
				if (num3 < num2)
				{
					char chr = s[num3];
					if (chr <= '\u007F')
					{
						int num4 = HttpUtility.getInt((byte)chr);
						if (num4 != -1)
						{
							num1 = (num1 << 4) + num4;
							num3++;
						}
						else
						{
							num = -1;
							break;
						}
					}
					else
					{
						num = -1;
						break;
					}
				}
				else
				{
					num = num1;
					break;
				}
			}
			return num;
		}

		private static char[] getChars(MemoryStream buffer, Encoding encoding)
		{
			char[] chars = encoding.GetChars(buffer.GetBuffer(), 0, (int)buffer.Length);
			return chars;
		}

		internal static Encoding GetEncoding(string contentType)
		{
			Encoding encoding;
			string[] strArrays = contentType.Split(new char[] { ';' });
			int num = 0;
			while (true)
			{
				if (num < (int)strArrays.Length)
				{
					string str = strArrays[num].Trim();
					if (!str.StartsWith("charset", StringComparison.OrdinalIgnoreCase))
					{
						num++;
					}
					else
					{
						encoding = Encoding.GetEncoding(str.GetValue('=', true));
						break;
					}
				}
				else
				{
					encoding = null;
					break;
				}
			}
			return encoding;
		}

		private static Dictionary<string, char> getEntities()
		{
			Dictionary<string, char> strs;
			object obj = HttpUtility._sync;
			Monitor.Enter(obj);
			try
			{
				if (HttpUtility._entities == null)
				{
					HttpUtility.initEntities();
				}
				strs = HttpUtility._entities;
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return strs;
		}

		private static int getInt(byte b)
		{
			int num;
			char chr = (char)b;
			if (chr >= '0' && chr <= '9')
			{
				num = chr - 48;
			}
			else if (chr >= 'a' && chr <= 'f')
			{
				num = chr - 97 + 10;
			}
			else if (chr < 'A' || chr > 'F')
			{
				num = -1;
			}
			else
			{
				num = chr - 65 + 10;
			}
			return num;
		}

		public static string HtmlAttributeEncode(string s)
		{
			string str;
			string str1;
			if ((s == null || s.Length == 0 ? false : s.Contains(new char[] { '&', '\"', '<', '>' })))
			{
				StringBuilder stringBuilder = new StringBuilder();
				string str2 = s;
				for (int i = 0; i < str2.Length; i++)
				{
					char chr = str2[i];
					StringBuilder stringBuilder1 = stringBuilder;
					if (chr == '&')
					{
						str1 = "&amp;";
					}
					else if (chr == '\"')
					{
						str1 = "&quot;";
					}
					else if (chr == '<')
					{
						str1 = "&lt;";
					}
					else
					{
						str1 = (chr == '>' ? "&gt;" : chr.ToString());
					}
					stringBuilder1.Append(str1);
				}
				str = stringBuilder.ToString();
			}
			else
			{
				str = s;
			}
			return str;
		}

		public static void HtmlAttributeEncode(string s, TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write(HttpUtility.HtmlAttributeEncode(s));
		}

		public static string HtmlDecode(string s)
		{
			string str;
			if ((s == null || s.Length == 0 ? false : s.Contains(new char[] { '&' })))
			{
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder1 = new StringBuilder();
				int num = 0;
				int num1 = 0;
				bool flag = false;
				string str1 = s;
				for (int i = 0; i < str1.Length; i++)
				{
					char chr = str1[i];
					if (num == 0)
					{
						if (chr != '&')
						{
							stringBuilder1.Append(chr);
						}
						else
						{
							stringBuilder.Append(chr);
							num = 1;
						}
					}
					else if (chr == '&')
					{
						num = 1;
						if (flag)
						{
							stringBuilder.Append(num1.ToString(CultureInfo.InvariantCulture));
							flag = false;
						}
						stringBuilder1.Append(stringBuilder.ToString());
						stringBuilder.Length = 0;
						stringBuilder.Append('&');
					}
					else if (num == 1)
					{
						if (chr != ';')
						{
							num1 = 0;
							num = (chr == '#' ? 3 : 2);
							stringBuilder.Append(chr);
						}
						else
						{
							num = 0;
							stringBuilder1.Append(stringBuilder.ToString());
							stringBuilder1.Append(chr);
							stringBuilder.Length = 0;
						}
					}
					else if (num == 2)
					{
						stringBuilder.Append(chr);
						if (chr == ';')
						{
							string str2 = stringBuilder.ToString();
							Dictionary<string, char> entities = HttpUtility.getEntities();
							if ((str2.Length <= 1 ? false : entities.ContainsKey(str2.Substring(1, str2.Length - 2))))
							{
								char item = entities[str2.Substring(1, str2.Length - 2)];
								str2 = item.ToString();
							}
							stringBuilder1.Append(str2);
							num = 0;
							stringBuilder.Length = 0;
						}
					}
					else if (num == 3)
					{
						if (chr == ';')
						{
							if (num1 <= 65535)
							{
								stringBuilder1.Append((char)num1);
							}
							else
							{
								stringBuilder1.Append("&#");
								stringBuilder1.Append(num1.ToString(CultureInfo.InvariantCulture));
								stringBuilder1.Append(";");
							}
							num = 0;
							stringBuilder.Length = 0;
							flag = false;
						}
						else if (!char.IsDigit(chr))
						{
							num = 2;
							if (flag)
							{
								stringBuilder.Append(num1.ToString(CultureInfo.InvariantCulture));
								flag = false;
							}
							stringBuilder.Append(chr);
						}
						else
						{
							num1 = num1 * 10 + (chr - 48);
							flag = true;
						}
					}
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder1.Append(stringBuilder.ToString());
				}
				else if (flag)
				{
					stringBuilder1.Append(num1.ToString(CultureInfo.InvariantCulture));
				}
				str = stringBuilder1.ToString();
			}
			else
			{
				str = s;
			}
			return str;
		}

		public static void HtmlDecode(string s, TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write(HttpUtility.HtmlDecode(s));
		}

		public static string HtmlEncode(string s)
		{
			string str;
			if ((s == null ? false : s.Length != 0))
			{
				bool flag = false;
				string str1 = s;
				int num = 0;
				while (num < str1.Length)
				{
					char chr = str1[num];
					if ((chr == '&' || chr == '\"' || chr == '<' || chr == '>' ? false : chr <= '\u009F'))
					{
						num++;
					}
					else
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					StringBuilder stringBuilder = new StringBuilder();
					string str2 = s;
					for (int i = 0; i < str2.Length; i++)
					{
						char chr1 = str2[i];
						if (chr1 == '&')
						{
							stringBuilder.Append("&amp;");
						}
						else if (chr1 == '\"')
						{
							stringBuilder.Append("&quot;");
						}
						else if (chr1 == '<')
						{
							stringBuilder.Append("&lt;");
						}
						else if (chr1 == '>')
						{
							stringBuilder.Append("&gt;");
						}
						else if (chr1 <= '\u009F')
						{
							stringBuilder.Append(chr1);
						}
						else
						{
							stringBuilder.Append("&#");
							int num1 = chr1;
							stringBuilder.Append(num1.ToString(CultureInfo.InvariantCulture));
							stringBuilder.Append(";");
						}
					}
					str = stringBuilder.ToString();
				}
				else
				{
					str = s;
				}
			}
			else
			{
				str = s;
			}
			return str;
		}

		public static void HtmlEncode(string s, TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write(HttpUtility.HtmlEncode(s));
		}

		private static void initEntities()
		{
			HttpUtility._entities = new Dictionary<string, char>()
			{
				{ "nbsp", '\u00A0' },
				{ "iexcl", '¡' },
				{ "cent", '¢' },
				{ "pound", '£' },
				{ "curren", '¤' },
				{ "yen", '¥' },
				{ "brvbar", '\u00A6' },
				{ "sect", '\u00A7' },
				{ "uml", '\u00A8' },
				{ "copy", '\u00A9' },
				{ "ordf", 'ª' },
				{ "laquo", '«' },
				{ "not", '¬' },
				{ "shy", '­' },
				{ "reg", '\u00AE' },
				{ "macr", '\u00AF' },
				{ "deg", '\u00B0' },
				{ "plusmn", '±' },
				{ "sup2", '\u00B2' },
				{ "sup3", '\u00B3' },
				{ "acute", '\u00B4' },
				{ "micro", 'µ' },
				{ "para", '\u00B6' },
				{ "middot", '·' },
				{ "cedil", '\u00B8' },
				{ "sup1", '\u00B9' },
				{ "ordm", 'º' },
				{ "raquo", '»' },
				{ "frac14", '\u00BC' },
				{ "frac12", '\u00BD' },
				{ "frac34", '\u00BE' },
				{ "iquest", '¿' },
				{ "Agrave", 'À' },
				{ "Aacute", 'Á' },
				{ "Acirc", 'Â' },
				{ "Atilde", 'Ã' },
				{ "Auml", 'Ä' },
				{ "Aring", 'Å' },
				{ "AElig", 'Æ' },
				{ "Ccedil", 'Ç' },
				{ "Egrave", 'È' },
				{ "Eacute", 'É' },
				{ "Ecirc", 'Ê' },
				{ "Euml", 'Ë' },
				{ "Igrave", 'Ì' },
				{ "Iacute", 'Í' },
				{ "Icirc", 'Î' },
				{ "Iuml", 'Ï' },
				{ "ETH", 'Ð' },
				{ "Ntilde", 'Ñ' },
				{ "Ograve", 'Ò' },
				{ "Oacute", 'Ó' },
				{ "Ocirc", 'Ô' },
				{ "Otilde", 'Õ' },
				{ "Ouml", 'Ö' },
				{ "times", '×' },
				{ "Oslash", 'Ø' },
				{ "Ugrave", 'Ù' },
				{ "Uacute", 'Ú' },
				{ "Ucirc", 'Û' },
				{ "Uuml", 'Ü' },
				{ "Yacute", 'Ý' },
				{ "THORN", 'Þ' },
				{ "szlig", 'ß' },
				{ "agrave", 'à' },
				{ "aacute", 'á' },
				{ "acirc", 'â' },
				{ "atilde", 'ã' },
				{ "auml", 'ä' },
				{ "aring", 'å' },
				{ "aelig", 'æ' },
				{ "ccedil", 'ç' },
				{ "egrave", 'è' },
				{ "eacute", 'é' },
				{ "ecirc", 'ê' },
				{ "euml", 'ë' },
				{ "igrave", 'ì' },
				{ "iacute", 'í' },
				{ "icirc", 'î' },
				{ "iuml", 'ï' },
				{ "eth", 'ð' },
				{ "ntilde", 'ñ' },
				{ "ograve", 'ò' },
				{ "oacute", 'ó' },
				{ "ocirc", 'ô' },
				{ "otilde", 'õ' },
				{ "ouml", 'ö' },
				{ "divide", '÷' },
				{ "oslash", 'ø' },
				{ "ugrave", 'ù' },
				{ "uacute", 'ú' },
				{ "ucirc", 'û' },
				{ "uuml", 'ü' },
				{ "yacute", 'ý' },
				{ "thorn", 'þ' },
				{ "yuml", 'ÿ' },
				{ "fnof", 'ƒ' },
				{ "Alpha", 'Α' },
				{ "Beta", 'Β' },
				{ "Gamma", 'Γ' },
				{ "Delta", 'Δ' },
				{ "Epsilon", 'Ε' },
				{ "Zeta", 'Ζ' },
				{ "Eta", 'Η' },
				{ "Theta", 'Θ' },
				{ "Iota", 'Ι' },
				{ "Kappa", 'Κ' },
				{ "Lambda", 'Λ' },
				{ "Mu", 'Μ' },
				{ "Nu", 'Ν' },
				{ "Xi", 'Ξ' },
				{ "Omicron", 'Ο' },
				{ "Pi", 'Π' },
				{ "Rho", 'Ρ' },
				{ "Sigma", 'Σ' },
				{ "Tau", 'Τ' },
				{ "Upsilon", 'Υ' },
				{ "Phi", 'Φ' },
				{ "Chi", 'Χ' },
				{ "Psi", 'Ψ' },
				{ "Omega", 'Ω' },
				{ "alpha", 'α' },
				{ "beta", 'β' },
				{ "gamma", 'γ' },
				{ "delta", 'δ' },
				{ "epsilon", 'ε' },
				{ "zeta", 'ζ' },
				{ "eta", 'η' },
				{ "theta", 'θ' },
				{ "iota", 'ι' },
				{ "kappa", 'κ' },
				{ "lambda", 'λ' },
				{ "mu", 'μ' },
				{ "nu", 'ν' },
				{ "xi", 'ξ' },
				{ "omicron", 'ο' },
				{ "pi", 'π' },
				{ "rho", 'ρ' },
				{ "sigmaf", 'ς' },
				{ "sigma", 'σ' },
				{ "tau", 'τ' },
				{ "upsilon", 'υ' },
				{ "phi", 'φ' },
				{ "chi", 'χ' },
				{ "psi", 'ψ' },
				{ "omega", 'ω' },
				{ "thetasym", 'ϑ' },
				{ "upsih", 'ϒ' },
				{ "piv", 'ϖ' },
				{ "bull", '•' },
				{ "hellip", '…' },
				{ "prime", '′' },
				{ "Prime", '″' },
				{ "oline", '‾' },
				{ "frasl", '⁄' },
				{ "weierp", '℘' },
				{ "image", 'ℑ' },
				{ "real", 'ℜ' },
				{ "trade", '\u2122' },
				{ "alefsym", '\u2135' },
				{ "larr", '←' },
				{ "uarr", '↑' },
				{ "rarr", '→' },
				{ "darr", '↓' },
				{ "harr", '↔' },
				{ "crarr", '\u21B5' },
				{ "lArr", '\u21D0' },
				{ "uArr", '\u21D1' },
				{ "rArr", '⇒' },
				{ "dArr", '\u21D3' },
				{ "hArr", '⇔' },
				{ "forall", '∀' },
				{ "part", '∂' },
				{ "exist", '∃' },
				{ "empty", '∅' },
				{ "nabla", '∇' },
				{ "isin", '∈' },
				{ "notin", '∉' },
				{ "ni", '∋' },
				{ "prod", '∏' },
				{ "sum", '∑' },
				{ "minus", '−' },
				{ "lowast", '∗' },
				{ "radic", '√' },
				{ "prop", '∝' },
				{ "infin", '∞' },
				{ "ang", '∠' },
				{ "and", '∧' },
				{ "or", '∨' },
				{ "cap", '∩' },
				{ "cup", '∪' },
				{ "int", '∫' },
				{ "there4", '∴' },
				{ "sim", '∼' },
				{ "cong", '≅' },
				{ "asymp", '≈' },
				{ "ne", '≠' },
				{ "equiv", '≡' },
				{ "le", '≤' },
				{ "ge", '≥' },
				{ "sub", '⊂' },
				{ "sup", '⊃' },
				{ "nsub", '⊄' },
				{ "sube", '⊆' },
				{ "supe", '⊇' },
				{ "oplus", '⊕' },
				{ "otimes", '⊗' },
				{ "perp", '⊥' },
				{ "sdot", '⋅' },
				{ "lceil", '⌈' },
				{ "rceil", '⌉' },
				{ "lfloor", '⌊' },
				{ "rfloor", '⌋' },
				{ "lang", '〈' },
				{ "rang", '〉' },
				{ "loz", '\u25CA' },
				{ "spades", '\u2660' },
				{ "clubs", '\u2663' },
				{ "hearts", '\u2665' },
				{ "diams", '\u2666' },
				{ "quot", '\"' },
				{ "amp", '&' },
				{ "lt", '<' },
				{ "gt", '>' },
				{ "OElig", 'Œ' },
				{ "oelig", 'œ' },
				{ "Scaron", 'Š' },
				{ "scaron", 'š' },
				{ "Yuml", 'Ÿ' },
				{ "circ", '\u02C6' },
				{ "tilde", '\u02DC' },
				{ "ensp", '\u2002' },
				{ "emsp", '\u2003' },
				{ "thinsp", '\u2009' },
				{ "zwnj", '\u200C' },
				{ "zwj", '\u200D' },
				{ "lrm", '\u200E' },
				{ "rlm", '\u200F' },
				{ "ndash", '–' },
				{ "mdash", '—' },
				{ "lsquo", '‘' },
				{ "rsquo", '’' },
				{ "sbquo", '‚' },
				{ "ldquo", '“' },
				{ "rdquo", '”' },
				{ "bdquo", '„' },
				{ "dagger", '†' },
				{ "Dagger", '‡' },
				{ "permil", '‰' },
				{ "lsaquo", '‹' },
				{ "rsaquo", '›' },
				{ "euro", '€' }
			};
		}

		internal static NameValueCollection InternalParseQueryString(string query, Encoding encoding)
		{
			NameValueCollection nameValueCollection;
			bool flag;
			if (query != null)
			{
				int length = query.Length;
				int num = length;
				if (length == 0)
				{
					goto Label1;
				}
				flag = (num != 1 ? false : query[0] == '?');
				goto Label0;
			}
		Label1:
			flag = true;
		Label0:
			if (!flag)
			{
				if (query[0] == '?')
				{
					query = query.Substring(1);
				}
				QueryStringCollection queryStringCollection = new QueryStringCollection();
				string[] strArrays = query.Split(new char[] { '&' });
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					string str = strArrays[i];
					int num1 = str.IndexOf('=');
					if (num1 <= -1)
					{
						queryStringCollection.Add(null, HttpUtility.UrlDecode(str, encoding));
					}
					else
					{
						string str1 = HttpUtility.UrlDecode(str.Substring(0, num1), encoding);
						queryStringCollection.Add(str1, (str.Length > num1 + 1 ? HttpUtility.UrlDecode(str.Substring(num1 + 1), encoding) : string.Empty));
					}
				}
				nameValueCollection = queryStringCollection;
			}
			else
			{
				nameValueCollection = new NameValueCollection(1);
			}
			return nameValueCollection;
		}

		internal static string InternalUrlDecode(byte[] bytes, int offset, int count, Encoding encoding)
		{
			int i;
			int chr;
			StringBuilder stringBuilder = new StringBuilder();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int num = count + offset;
				for (i = offset; i < num; i++)
				{
					if ((bytes[i] != 37 || i + 2 >= count ? false : bytes[i + 1] != 37))
					{
						if ((bytes[i + 1] != 117 ? true : i + 5 >= num))
						{
							int chr1 = HttpUtility.getChar(bytes, i + 1, 2);
							chr = chr1;
							if (chr1 == -1)
							{
								goto Label1;
							}
							memoryStream.WriteByte((byte)chr);
							i += 2;
							goto Label0;
						}
						else
						{
							if (memoryStream.Length > (long)0)
							{
								stringBuilder.Append(HttpUtility.getChars(memoryStream, encoding));
								memoryStream.SetLength((long)0);
							}
							chr = HttpUtility.getChar(bytes, i + 2, 4);
							if (chr != -1)
							{
								goto Label2;
							}
						}
					Label1:
					}
					if (memoryStream.Length > (long)0)
					{
						stringBuilder.Append(HttpUtility.getChars(memoryStream, encoding));
						memoryStream.SetLength((long)0);
					}
					if (bytes[i] != 43)
					{
						stringBuilder.Append((char)bytes[i]);
					}
					else
					{
						stringBuilder.Append(' ');
					}
				Label0:
				}
				if (memoryStream.Length > (long)0)
				{
					stringBuilder.Append(HttpUtility.getChars(memoryStream, encoding));
				}
			}
			return stringBuilder.ToString();
		Label2:
			stringBuilder.Append((char)chr);
			i += 5;
			goto Label0;
		}

		internal static byte[] InternalUrlDecodeToBytes(byte[] bytes, int offset, int count)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int num = offset + count;
				for (int i = offset; i < num; i++)
				{
					char chr = (char)bytes[i];
					if (chr == '+')
					{
						chr = ' ';
					}
					else if ((chr != '%' ? false : i < num - 2))
					{
						int chr1 = HttpUtility.getChar(bytes, i + 1, 2);
						if (chr1 != -1)
						{
							chr = (char)chr1;
							i += 2;
						}
					}
					memoryStream.WriteByte((byte)chr);
				}
				memoryStream.Close();
				array = memoryStream.ToArray();
			}
			return array;
		}

		internal static byte[] InternalUrlEncodeToBytes(byte[] bytes, int offset, int count)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int num = offset + count;
				for (int i = offset; i < num; i++)
				{
					HttpUtility.urlEncode(bytes[i], memoryStream, false);
				}
				memoryStream.Close();
				array = memoryStream.ToArray();
			}
			return array;
		}

		internal static byte[] InternalUrlEncodeUnicodeToBytes(string s)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				string str = s;
				for (int i = 0; i < str.Length; i++)
				{
					HttpUtility.urlEncode(str[i], memoryStream, true);
				}
				memoryStream.Close();
				array = memoryStream.ToArray();
			}
			return array;
		}

		private static bool notEncoded(char c)
		{
			return (c == '!' || c == '\'' || c == '(' || c == ')' || c == '*' || c == '-' || c == '.' ? true : c == '\u005F');
		}

		public static NameValueCollection ParseQueryString(string query)
		{
			return HttpUtility.ParseQueryString(query, Encoding.UTF8);
		}

		public static NameValueCollection ParseQueryString(string query, Encoding encoding)
		{
			if (query == null)
			{
				throw new ArgumentNullException("query");
			}
			return HttpUtility.InternalParseQueryString(query, encoding ?? Encoding.UTF8);
		}

		public static string UrlDecode(string s)
		{
			return HttpUtility.UrlDecode(s, Encoding.UTF8);
		}

		public static string UrlDecode(string s, Encoding encoding)
		{
			string str;
			int chr;
			if ((s == null || s.Length == 0 ? false : s.Contains(new char[] { '%', '+' })))
			{
				if (encoding == null)
				{
					encoding = Encoding.UTF8;
				}
				List<byte> nums = new List<byte>();
				int length = s.Length;
				for (int i = 0; i < length; i++)
				{
					char chr1 = s[i];
					if ((chr1 != '%' || i + 2 >= length ? false : s[i + 1] != '%'))
					{
						if ((s[i + 1] != 'u' ? true : i + 5 >= length))
						{
							int num = HttpUtility.getChar(s, i + 1, 2);
							chr = num;
							if (num == -1)
							{
								HttpUtility.writeCharBytes('%', nums, encoding);
							}
							else
							{
								HttpUtility.writeCharBytes((char)chr, nums, encoding);
								i += 2;
							}
						}
						else
						{
							chr = HttpUtility.getChar(s, i + 2, 4);
							if (chr == -1)
							{
								HttpUtility.writeCharBytes('%', nums, encoding);
							}
							else
							{
								HttpUtility.writeCharBytes((char)chr, nums, encoding);
								i += 5;
							}
						}
					}
					else if (chr1 != '+')
					{
						HttpUtility.writeCharBytes(chr1, nums, encoding);
					}
					else
					{
						HttpUtility.writeCharBytes(' ', nums, encoding);
					}
				}
				str = encoding.GetString(nums.ToArray());
			}
			else
			{
				str = s;
			}
			return str;
		}

		public static string UrlDecode(byte[] bytes, Encoding encoding)
		{
			string str;
			if (bytes == null)
			{
				str = null;
			}
			else
			{
				int length = (int)bytes.Length;
				int num = length;
				str = (length == 0 ? string.Empty : HttpUtility.InternalUrlDecode(bytes, 0, num, encoding ?? Encoding.UTF8));
			}
			return str;
		}

		public static string UrlDecode(byte[] bytes, int offset, int count, Encoding encoding)
		{
			string empty;
			if (bytes != null)
			{
				int length = (int)bytes.Length;
				if ((length == 0 ? false : count != 0))
				{
					if ((offset < 0 ? true : offset >= length))
					{
						throw new ArgumentOutOfRangeException("offset");
					}
					if ((count < 0 ? true : count > length - offset))
					{
						throw new ArgumentOutOfRangeException("count");
					}
					empty = HttpUtility.InternalUrlDecode(bytes, offset, count, encoding ?? Encoding.UTF8);
				}
				else
				{
					empty = string.Empty;
				}
			}
			else
			{
				empty = null;
			}
			return empty;
		}

		public static byte[] UrlDecodeToBytes(byte[] bytes)
		{
			byte[] numArray;
			byte[] numArray1;
			if (bytes != null)
			{
				int length = (int)bytes.Length;
				int num = length;
				if (length <= 0)
				{
					numArray1 = bytes;
					numArray = numArray1;
					return numArray;
				}
				numArray1 = HttpUtility.InternalUrlDecodeToBytes(bytes, 0, num);
				numArray = numArray1;
				return numArray;
			}
			numArray1 = bytes;
			numArray = numArray1;
			return numArray;
		}

		public static byte[] UrlDecodeToBytes(string s)
		{
			return HttpUtility.UrlDecodeToBytes(s, Encoding.UTF8);
		}

		public static byte[] UrlDecodeToBytes(string s, Encoding encoding)
		{
			byte[] bytes;
			if (s == null)
			{
				bytes = null;
			}
			else if (s.Length != 0)
			{
				byte[] numArray = (encoding ?? Encoding.UTF8).GetBytes(s);
				bytes = HttpUtility.InternalUrlDecodeToBytes(numArray, 0, (int)numArray.Length);
			}
			else
			{
				bytes = new byte[0];
			}
			return bytes;
		}

		public static byte[] UrlDecodeToBytes(byte[] bytes, int offset, int count)
		{
			int num = 0;
			byte[] numArray;
			bool flag;
			if (bytes == null)
			{
				flag = true;
			}
			else
			{
				int length = (int)bytes.Length;
				num = length;
				flag = length == 0;
			}
			if (flag)
			{
				numArray = bytes;
			}
			else if (count != 0)
			{
				if ((offset < 0 ? true : offset >= num))
				{
					throw new ArgumentOutOfRangeException("offset");
				}
				if ((count < 0 ? true : count > num - offset))
				{
					throw new ArgumentOutOfRangeException("count");
				}
				numArray = HttpUtility.InternalUrlDecodeToBytes(bytes, offset, count);
			}
			else
			{
				numArray = new byte[0];
			}
			return numArray;
		}

		private static void urlEncode(char c, Stream result, bool unicode)
		{
			if (c > 'ÿ')
			{
				result.WriteByte(37);
				result.WriteByte(117);
				int num = c;
				result.WriteByte((byte)HttpUtility._hexChars[num >> 12]);
				int num1 = num >> 8 & 15;
				result.WriteByte((byte)HttpUtility._hexChars[num1]);
				num1 = num >> 4 & 15;
				result.WriteByte((byte)HttpUtility._hexChars[num1]);
				result.WriteByte((byte)HttpUtility._hexChars[num & 15]);
			}
			else if ((c <= ' ' ? false : HttpUtility.notEncoded(c)))
			{
				result.WriteByte((byte)c);
			}
			else if (c == ' ')
			{
				result.WriteByte(43);
			}
			else if ((c < '0' || c < 'A' && c > '9' || c > 'Z' && c < 'a' ? false : c <= 'z'))
			{
				result.WriteByte((byte)c);
			}
			else
			{
				if ((!unicode ? true : c <= '\u007F'))
				{
					result.WriteByte(37);
				}
				else
				{
					result.WriteByte(37);
					result.WriteByte(117);
					result.WriteByte(48);
					result.WriteByte(48);
				}
				int num2 = c;
				result.WriteByte((byte)HttpUtility._hexChars[num2 >> 4]);
				result.WriteByte((byte)HttpUtility._hexChars[num2 & 15]);
			}
		}

		public static string UrlEncode(byte[] bytes)
		{
			string str;
			if (bytes == null)
			{
				str = null;
			}
			else
			{
				int length = (int)bytes.Length;
				int num = length;
				str = (length == 0 ? string.Empty : Encoding.ASCII.GetString(HttpUtility.InternalUrlEncodeToBytes(bytes, 0, num)));
			}
			return str;
		}

		public static string UrlEncode(string s)
		{
			return HttpUtility.UrlEncode(s, Encoding.UTF8);
		}

		public static string UrlEncode(string s, Encoding encoding)
		{
			int num = 0;
			string str;
			bool flag;
			if (s == null)
			{
				flag = true;
			}
			else
			{
				int length = s.Length;
				num = length;
				flag = length == 0;
			}
			if (!flag)
			{
				bool flag1 = false;
				string str1 = s;
				for (int i = 0; i < str1.Length; i++)
				{
					char chr = str1[i];
					if ((chr < '0' || chr < 'A' && chr > '9' || chr > 'Z' && chr < 'a' ? true : chr > 'z'))
					{
						if (!HttpUtility.notEncoded(chr))
						{
							flag1 = true;
							break;
						}
					}
				}
				if (flag1)
				{
					if (encoding == null)
					{
						encoding = Encoding.UTF8;
					}
					byte[] numArray = new byte[encoding.GetMaxByteCount(num)];
					int bytes = encoding.GetBytes(s, 0, num, numArray, 0);
					str = Encoding.ASCII.GetString(HttpUtility.InternalUrlEncodeToBytes(numArray, 0, bytes));
				}
				else
				{
					str = s;
				}
			}
			else
			{
				str = s;
			}
			return str;
		}

		public static string UrlEncode(byte[] bytes, int offset, int count)
		{
			string str;
			byte[] numArray = HttpUtility.UrlEncodeToBytes(bytes, offset, count);
			if (numArray == null)
			{
				str = null;
			}
			else
			{
				str = (numArray.Length == 0 ? string.Empty : Encoding.ASCII.GetString(numArray));
			}
			return str;
		}

		public static byte[] UrlEncodeToBytes(byte[] bytes)
		{
			byte[] numArray;
			byte[] numArray1;
			if (bytes != null)
			{
				int length = (int)bytes.Length;
				int num = length;
				if (length <= 0)
				{
					numArray1 = bytes;
					numArray = numArray1;
					return numArray;
				}
				numArray1 = HttpUtility.InternalUrlEncodeToBytes(bytes, 0, num);
				numArray = numArray1;
				return numArray;
			}
			numArray1 = bytes;
			numArray = numArray1;
			return numArray;
		}

		public static byte[] UrlEncodeToBytes(string s)
		{
			return HttpUtility.UrlEncodeToBytes(s, Encoding.UTF8);
		}

		public static byte[] UrlEncodeToBytes(string s, Encoding encoding)
		{
			byte[] bytes;
			if (s == null)
			{
				bytes = null;
			}
			else if (s.Length != 0)
			{
				byte[] numArray = (encoding ?? Encoding.UTF8).GetBytes(s);
				bytes = HttpUtility.InternalUrlEncodeToBytes(numArray, 0, (int)numArray.Length);
			}
			else
			{
				bytes = new byte[0];
			}
			return bytes;
		}

		public static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count)
		{
			int num = 0;
			byte[] numArray;
			bool flag;
			if (bytes == null)
			{
				flag = true;
			}
			else
			{
				int length = (int)bytes.Length;
				num = length;
				flag = length == 0;
			}
			if (flag)
			{
				numArray = bytes;
			}
			else if (count != 0)
			{
				if ((offset < 0 ? true : offset >= num))
				{
					throw new ArgumentOutOfRangeException("offset");
				}
				if ((count < 0 ? true : count > num - offset))
				{
					throw new ArgumentOutOfRangeException("count");
				}
				numArray = HttpUtility.InternalUrlEncodeToBytes(bytes, offset, count);
			}
			else
			{
				numArray = new byte[0];
			}
			return numArray;
		}

		public static string UrlEncodeUnicode(string s)
		{
			return (s == null || s.Length <= 0 ? s : Encoding.ASCII.GetString(HttpUtility.InternalUrlEncodeUnicodeToBytes(s)));
		}

		public static byte[] UrlEncodeUnicodeToBytes(string s)
		{
			byte[] numArray;
			if (s == null)
			{
				numArray = null;
			}
			else
			{
				numArray = (s.Length == 0 ? new byte[0] : HttpUtility.InternalUrlEncodeUnicodeToBytes(s));
			}
			return numArray;
		}

		private static void urlPathEncode(char c, Stream result)
		{
			if ((c < '!' ? true : c > '~'))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(c.ToString());
				for (int i = 0; i < (int)bytes.Length; i++)
				{
					byte num = bytes[i];
					result.WriteByte(37);
					int num1 = num;
					result.WriteByte((byte)HttpUtility._hexChars[num1 >> 4]);
					result.WriteByte((byte)HttpUtility._hexChars[num1 & 15]);
				}
			}
			else if (c != ' ')
			{
				result.WriteByte((byte)c);
			}
			else
			{
				result.WriteByte(37);
				result.WriteByte(50);
				result.WriteByte(48);
			}
		}

		public static string UrlPathEncode(string s)
		{
			string str;
			if ((s == null ? false : s.Length != 0))
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					string str1 = s;
					for (int i = 0; i < str1.Length; i++)
					{
						HttpUtility.urlPathEncode(str1[i], memoryStream);
					}
					memoryStream.Close();
					str = Encoding.ASCII.GetString(memoryStream.ToArray());
				}
			}
			else
			{
				str = s;
			}
			return str;
		}

		private static void writeCharBytes(char c, IList buffer, Encoding encoding)
		{
			if (c <= 'ÿ')
			{
				buffer.Add((byte)c);
			}
			else
			{
				byte[] bytes = encoding.GetBytes(new char[] { c });
				for (int i = 0; i < (int)bytes.Length; i++)
				{
					buffer.Add(bytes[i]);
				}
			}
		}
	}
}