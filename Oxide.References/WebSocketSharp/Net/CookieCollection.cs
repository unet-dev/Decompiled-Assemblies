using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using WebSocketSharp;

namespace WebSocketSharp.Net
{
	[Serializable]
	public class CookieCollection : ICollection, IEnumerable
	{
		private List<Cookie> _list;

		private object _sync;

		public int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public Cookie this[int index]
		{
			get
			{
				if ((index < 0 ? true : index >= this._list.Count))
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this._list[index];
			}
		}

		public Cookie this[string name]
		{
			get
			{
				Cookie cookie;
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				foreach (Cookie sorted in this.Sorted)
				{
					if (!sorted.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					{
						continue;
					}
					cookie = sorted;
					return cookie;
				}
				cookie = null;
				return cookie;
			}
		}

		internal IList<Cookie> List
		{
			get
			{
				return this._list;
			}
		}

		internal IEnumerable<Cookie> Sorted
		{
			get
			{
				List<Cookie> cookies = new List<Cookie>(this._list);
				if (cookies.Count > 1)
				{
					cookies.Sort(new Comparison<Cookie>(CookieCollection.compareCookieWithinSorted));
				}
				return cookies;
			}
		}

		public object SyncRoot
		{
			get
			{
				object obj = this._sync;
				if (obj == null)
				{
					object syncRoot = ((ICollection)this._list).SyncRoot;
					object obj1 = syncRoot;
					this._sync = syncRoot;
					obj = obj1;
				}
				return obj;
			}
		}

		public CookieCollection()
		{
			this._list = new List<Cookie>();
		}

		public void Add(Cookie cookie)
		{
			if (cookie == null)
			{
				throw new ArgumentNullException("cookie");
			}
			int num = this.searchCookie(cookie);
			if (num != -1)
			{
				this._list[num] = cookie;
			}
			else
			{
				this._list.Add(cookie);
			}
		}

		public void Add(CookieCollection cookies)
		{
			if (cookies == null)
			{
				throw new ArgumentNullException("cookies");
			}
			foreach (Cookie cooky in cookies)
			{
				this.Add(cooky);
			}
		}

		private static int compareCookieWithinSort(Cookie x, Cookie y)
		{
			int length = x.Name.Length + x.Value.Length - (y.Name.Length + y.Value.Length);
			return length;
		}

		private static int compareCookieWithinSorted(Cookie x, Cookie y)
		{
			int num;
			int num1 = 0;
			int version = x.Version - y.Version;
			num1 = version;
			if (version != 0)
			{
				num = num1;
			}
			else
			{
				int num2 = x.Name.CompareTo(y.Name);
				num1 = num2;
				num = (num2 != 0 ? num1 : y.Path.Length - x.Path.Length);
			}
			return num;
		}

		public void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Less than zero.");
			}
			if (array.Rank > 1)
			{
				throw new ArgumentException("Multidimensional.", "array");
			}
			if (array.Length - index < this._list.Count)
			{
				throw new ArgumentException("The number of elements in this collection is greater than the available space of the destination array.");
			}
			if (!array.GetType().GetElementType().IsAssignableFrom(typeof(Cookie)))
			{
				throw new InvalidCastException("The elements in this collection cannot be cast automatically to the type of the destination array.");
			}
			((ICollection)this._list).CopyTo(array, index);
		}

		public void CopyTo(Cookie[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Less than zero.");
			}
			if ((int)array.Length - index < this._list.Count)
			{
				throw new ArgumentException("The number of elements in this collection is greater than the available space of the destination array.");
			}
			this._list.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this._list.GetEnumerator();
		}

		internal static CookieCollection Parse(string value, bool response)
		{
			return (response ? CookieCollection.parseResponse(value) : CookieCollection.parseRequest(value));
		}

		private static CookieCollection parseRequest(string value)
		{
			string str;
			CookieCollection cookieCollections = new CookieCollection();
			Cookie cookie = null;
			int num = 0;
			string[] strArrays = CookieCollection.splitCookieHeaderValue(value);
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str1 = strArrays[i].Trim();
				if (str1.Length != 0)
				{
					if (str1.StartsWith("$version", StringComparison.InvariantCultureIgnoreCase))
					{
						num = int.Parse(str1.GetValue('=', true));
					}
					else if (str1.StartsWith("$path", StringComparison.InvariantCultureIgnoreCase))
					{
						if (cookie != null)
						{
							cookie.Path = str1.GetValue('=');
						}
					}
					else if (str1.StartsWith("$domain", StringComparison.InvariantCultureIgnoreCase))
					{
						if (cookie != null)
						{
							cookie.Domain = str1.GetValue('=');
						}
					}
					else if (!str1.StartsWith("$port", StringComparison.InvariantCultureIgnoreCase))
					{
						if (cookie != null)
						{
							cookieCollections.Add(cookie);
						}
						string empty = string.Empty;
						int num1 = str1.IndexOf('=');
						if (num1 == -1)
						{
							str = str1;
						}
						else if (num1 != str1.Length - 1)
						{
							str = str1.Substring(0, num1).TrimEnd(new char[] { ' ' });
							empty = str1.Substring(num1 + 1).TrimStart(new char[] { ' ' });
						}
						else
						{
							str = str1.Substring(0, num1).TrimEnd(new char[] { ' ' });
						}
						cookie = new Cookie(str, empty);
						if (num != 0)
						{
							cookie.Version = num;
						}
					}
					else
					{
						string str2 = (str1.Equals("$port", StringComparison.InvariantCultureIgnoreCase) ? "\"\"" : str1.GetValue('='));
						if (cookie != null)
						{
							cookie.Port = str2;
						}
					}
				}
			}
			if (cookie != null)
			{
				cookieCollections.Add(cookie);
			}
			return cookieCollections;
		}

		private static CookieCollection parseResponse(string value)
		{
			DateTime now;
			string str;
			CookieCollection cookieCollections = new CookieCollection();
			Cookie localTime = null;
			string[] strArrays = CookieCollection.splitCookieHeaderValue(value);
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str1 = strArrays[i].Trim();
				if (str1.Length != 0)
				{
					if (str1.StartsWith("version", StringComparison.InvariantCultureIgnoreCase))
					{
						if (localTime != null)
						{
							localTime.Version = int.Parse(str1.GetValue('=', true));
						}
					}
					else if (str1.StartsWith("expires", StringComparison.InvariantCultureIgnoreCase))
					{
						StringBuilder stringBuilder = new StringBuilder(str1.GetValue('='), 32);
						if (i < (int)strArrays.Length - 1)
						{
							int num = i + 1;
							i = num;
							stringBuilder.AppendFormat(", {0}", strArrays[num].Trim());
						}
						if (!DateTime.TryParseExact(stringBuilder.ToString(), new string[] { "ddd, dd'-'MMM'-'yyyy HH':'mm':'ss 'GMT'", "r" }, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out now))
						{
							now = DateTime.Now;
						}
						if ((localTime == null ? false : localTime.Expires == DateTime.MinValue))
						{
							localTime.Expires = now.ToLocalTime();
						}
					}
					else if (str1.StartsWith("max-age", StringComparison.InvariantCultureIgnoreCase))
					{
						int num1 = int.Parse(str1.GetValue('=', true));
						DateTime dateTime = DateTime.Now.AddSeconds((double)num1);
						if (localTime != null)
						{
							localTime.Expires = dateTime;
						}
					}
					else if (str1.StartsWith("path", StringComparison.InvariantCultureIgnoreCase))
					{
						if (localTime != null)
						{
							localTime.Path = str1.GetValue('=');
						}
					}
					else if (str1.StartsWith("domain", StringComparison.InvariantCultureIgnoreCase))
					{
						if (localTime != null)
						{
							localTime.Domain = str1.GetValue('=');
						}
					}
					else if (str1.StartsWith("port", StringComparison.InvariantCultureIgnoreCase))
					{
						string str2 = (str1.Equals("port", StringComparison.InvariantCultureIgnoreCase) ? "\"\"" : str1.GetValue('='));
						if (localTime != null)
						{
							localTime.Port = str2;
						}
					}
					else if (str1.StartsWith("comment", StringComparison.InvariantCultureIgnoreCase))
					{
						if (localTime != null)
						{
							localTime.Comment = str1.GetValue('=').UrlDecode();
						}
					}
					else if (str1.StartsWith("commenturl", StringComparison.InvariantCultureIgnoreCase))
					{
						if (localTime != null)
						{
							localTime.CommentUri = str1.GetValue('=', true).ToUri();
						}
					}
					else if (str1.StartsWith("discard", StringComparison.InvariantCultureIgnoreCase))
					{
						if (localTime != null)
						{
							localTime.Discard = true;
						}
					}
					else if (str1.StartsWith("secure", StringComparison.InvariantCultureIgnoreCase))
					{
						if (localTime != null)
						{
							localTime.Secure = true;
						}
					}
					else if (!str1.StartsWith("httponly", StringComparison.InvariantCultureIgnoreCase))
					{
						if (localTime != null)
						{
							cookieCollections.Add(localTime);
						}
						string empty = string.Empty;
						int num2 = str1.IndexOf('=');
						if (num2 == -1)
						{
							str = str1;
						}
						else if (num2 != str1.Length - 1)
						{
							str = str1.Substring(0, num2).TrimEnd(new char[] { ' ' });
							empty = str1.Substring(num2 + 1).TrimStart(new char[] { ' ' });
						}
						else
						{
							str = str1.Substring(0, num2).TrimEnd(new char[] { ' ' });
						}
						localTime = new Cookie(str, empty);
					}
					else if (localTime != null)
					{
						localTime.HttpOnly = true;
					}
				}
			}
			if (localTime != null)
			{
				cookieCollections.Add(localTime);
			}
			return cookieCollections;
		}

		private int searchCookie(Cookie cookie)
		{
			int num;
			string name = cookie.Name;
			string path = cookie.Path;
			string domain = cookie.Domain;
			int version = cookie.Version;
			int count = this._list.Count - 1;
			while (true)
			{
				if (count >= 0)
				{
					Cookie item = this._list[count];
					if ((!item.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) || !item.Path.Equals(path, StringComparison.InvariantCulture) || !item.Domain.Equals(domain, StringComparison.InvariantCultureIgnoreCase) ? true : item.Version != version))
					{
						count--;
					}
					else
					{
						num = count;
						break;
					}
				}
				else
				{
					num = -1;
					break;
				}
			}
			return num;
		}

		internal void SetOrRemove(Cookie cookie)
		{
			int num = this.searchCookie(cookie);
			if (num == -1)
			{
				if (!cookie.Expired)
				{
					this._list.Add(cookie);
				}
			}
			else if (cookie.Expired)
			{
				this._list.RemoveAt(num);
			}
			else
			{
				this._list[num] = cookie;
			}
		}

		internal void SetOrRemove(CookieCollection cookies)
		{
			foreach (Cookie cooky in cookies)
			{
				this.SetOrRemove(cooky);
			}
		}

		internal void Sort()
		{
			if (this._list.Count > 1)
			{
				this._list.Sort(new Comparison<Cookie>(CookieCollection.compareCookieWithinSort));
			}
		}

		private static string[] splitCookieHeaderValue(string value)
		{
			string[] array = (new List<string>(value.SplitHeaderValue(new char[] { ',', ';' }))).ToArray();
			return array;
		}
	}
}