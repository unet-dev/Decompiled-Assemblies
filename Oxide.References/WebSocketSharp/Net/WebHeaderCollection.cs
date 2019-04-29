using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using WebSocketSharp;

namespace WebSocketSharp.Net
{
	[ComVisible(true)]
	[Serializable]
	public class WebHeaderCollection : NameValueCollection, ISerializable
	{
		private readonly static Dictionary<string, HttpHeaderInfo> _headers;

		private bool _internallyUsed;

		private HttpHeaderType _state;

		public override string[] AllKeys
		{
			get
			{
				return base.AllKeys;
			}
		}

		public override int Count
		{
			get
			{
				return base.Count;
			}
		}

		public string this[HttpRequestHeader header]
		{
			get
			{
				return this.Get(WebHeaderCollection.Convert(header));
			}
			set
			{
				this.Add(header, value);
			}
		}

		public string this[HttpResponseHeader header]
		{
			get
			{
				return this.Get(WebHeaderCollection.Convert(header));
			}
			set
			{
				this.Add(header, value);
			}
		}

		public override NameObjectCollectionBase.KeysCollection Keys
		{
			get
			{
				return base.Keys;
			}
		}

		internal HttpHeaderType State
		{
			get
			{
				return this._state;
			}
		}

		static WebHeaderCollection()
		{
			WebHeaderCollection._headers = new Dictionary<string, HttpHeaderInfo>(StringComparer.InvariantCultureIgnoreCase)
			{
				{ "Accept", new HttpHeaderInfo("Accept", HttpHeaderType.Request | HttpHeaderType.Restricted | HttpHeaderType.MultiValue) },
				{ "AcceptCharset", new HttpHeaderInfo("Accept-Charset", HttpHeaderType.Request | HttpHeaderType.MultiValue) },
				{ "AcceptEncoding", new HttpHeaderInfo("Accept-Encoding", HttpHeaderType.Request | HttpHeaderType.MultiValue) },
				{ "AcceptLanguage", new HttpHeaderInfo("Accept-Language", HttpHeaderType.Request | HttpHeaderType.MultiValue) },
				{ "AcceptRanges", new HttpHeaderInfo("Accept-Ranges", HttpHeaderType.Response | HttpHeaderType.MultiValue) },
				{ "Age", new HttpHeaderInfo("Age", HttpHeaderType.Response) },
				{ "Allow", new HttpHeaderInfo("Allow", HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.MultiValue) },
				{ "Authorization", new HttpHeaderInfo("Authorization", HttpHeaderType.Request | HttpHeaderType.MultiValue) },
				{ "CacheControl", new HttpHeaderInfo("Cache-Control", HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.MultiValue) },
				{ "Connection", new HttpHeaderInfo("Connection", HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.Restricted | HttpHeaderType.MultiValue) },
				{ "ContentEncoding", new HttpHeaderInfo("Content-Encoding", HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.MultiValue) },
				{ "ContentLanguage", new HttpHeaderInfo("Content-Language", HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.MultiValue) },
				{ "ContentLength", new HttpHeaderInfo("Content-Length", HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.Restricted) },
				{ "ContentLocation", new HttpHeaderInfo("Content-Location", HttpHeaderType.Request | HttpHeaderType.Response) },
				{ "ContentMd5", new HttpHeaderInfo("Content-MD5", HttpHeaderType.Request | HttpHeaderType.Response) },
				{ "ContentRange", new HttpHeaderInfo("Content-Range", HttpHeaderType.Request | HttpHeaderType.Response) },
				{ "ContentType", new HttpHeaderInfo("Content-Type", HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.Restricted) },
				{ "Cookie", new HttpHeaderInfo("Cookie", HttpHeaderType.Request) },
				{ "Cookie2", new HttpHeaderInfo("Cookie2", HttpHeaderType.Request) },
				{ "Date", new HttpHeaderInfo("Date", HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.Restricted) },
				{ "Expect", new HttpHeaderInfo("Expect", HttpHeaderType.Request | HttpHeaderType.Restricted | HttpHeaderType.MultiValue) },
				{ "Expires", new HttpHeaderInfo("Expires", HttpHeaderType.Request | HttpHeaderType.Response) },
				{ "ETag", new HttpHeaderInfo("ETag", HttpHeaderType.Response) },
				{ "From", new HttpHeaderInfo("From", HttpHeaderType.Request) },
				{ "Host", new HttpHeaderInfo("Host", HttpHeaderType.Request | HttpHeaderType.Restricted) },
				{ "IfMatch", new HttpHeaderInfo("If-Match", HttpHeaderType.Request | HttpHeaderType.MultiValue) },
				{ "IfModifiedSince", new HttpHeaderInfo("If-Modified-Since", HttpHeaderType.Request | HttpHeaderType.Restricted) },
				{ "IfNoneMatch", new HttpHeaderInfo("If-None-Match", HttpHeaderType.Request | HttpHeaderType.MultiValue) },
				{ "IfRange", new HttpHeaderInfo("If-Range", HttpHeaderType.Request) },
				{ "IfUnmodifiedSince", new HttpHeaderInfo("If-Unmodified-Since", HttpHeaderType.Request) },
				{ "KeepAlive", new HttpHeaderInfo("Keep-Alive", HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.MultiValue) },
				{ "LastModified", new HttpHeaderInfo("Last-Modified", HttpHeaderType.Request | HttpHeaderType.Response) },
				{ "Location", new HttpHeaderInfo("Location", HttpHeaderType.Response) },
				{ "MaxForwards", new HttpHeaderInfo("Max-Forwards", HttpHeaderType.Request) },
				{ "Pragma", new HttpHeaderInfo("Pragma", HttpHeaderType.Request | HttpHeaderType.Response) },
				{ "ProxyAuthenticate", new HttpHeaderInfo("Proxy-Authenticate", HttpHeaderType.Response | HttpHeaderType.MultiValue) },
				{ "ProxyAuthorization", new HttpHeaderInfo("Proxy-Authorization", HttpHeaderType.Request) },
				{ "ProxyConnection", new HttpHeaderInfo("Proxy-Connection", HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.Restricted) },
				{ "Public", new HttpHeaderInfo("Public", HttpHeaderType.Response | HttpHeaderType.MultiValue) },
				{ "Range", new HttpHeaderInfo("Range", HttpHeaderType.Request | HttpHeaderType.Restricted | HttpHeaderType.MultiValue) },
				{ "Referer", new HttpHeaderInfo("Referer", HttpHeaderType.Request | HttpHeaderType.Restricted) },
				{ "RetryAfter", new HttpHeaderInfo("Retry-After", HttpHeaderType.Response) },
				{ "SecWebSocketAccept", new HttpHeaderInfo("Sec-WebSocket-Accept", HttpHeaderType.Response | HttpHeaderType.Restricted) },
				{ "SecWebSocketExtensions", new HttpHeaderInfo("Sec-WebSocket-Extensions", HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.Restricted | HttpHeaderType.MultiValueInRequest) },
				{ "SecWebSocketKey", new HttpHeaderInfo("Sec-WebSocket-Key", HttpHeaderType.Request | HttpHeaderType.Restricted) },
				{ "SecWebSocketProtocol", new HttpHeaderInfo("Sec-WebSocket-Protocol", HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.MultiValueInRequest) },
				{ "SecWebSocketVersion", new HttpHeaderInfo("Sec-WebSocket-Version", HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.Restricted | HttpHeaderType.MultiValueInResponse) },
				{ "Server", new HttpHeaderInfo("Server", HttpHeaderType.Response) },
				{ "SetCookie", new HttpHeaderInfo("Set-Cookie", HttpHeaderType.Response | HttpHeaderType.MultiValue) },
				{ "SetCookie2", new HttpHeaderInfo("Set-Cookie2", HttpHeaderType.Response | HttpHeaderType.MultiValue) },
				{ "Te", new HttpHeaderInfo("TE", HttpHeaderType.Request) },
				{ "Trailer", new HttpHeaderInfo("Trailer", HttpHeaderType.Request | HttpHeaderType.Response) },
				{ "TransferEncoding", new HttpHeaderInfo("Transfer-Encoding", HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.Restricted | HttpHeaderType.MultiValue) },
				{ "Translate", new HttpHeaderInfo("Translate", HttpHeaderType.Request) },
				{ "Upgrade", new HttpHeaderInfo("Upgrade", HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.MultiValue) },
				{ "UserAgent", new HttpHeaderInfo("User-Agent", HttpHeaderType.Request | HttpHeaderType.Restricted) },
				{ "Vary", new HttpHeaderInfo("Vary", HttpHeaderType.Response | HttpHeaderType.MultiValue) },
				{ "Via", new HttpHeaderInfo("Via", HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.MultiValue) },
				{ "Warning", new HttpHeaderInfo("Warning", HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.MultiValue) },
				{ "WwwAuthenticate", new HttpHeaderInfo("WWW-Authenticate", HttpHeaderType.Response | HttpHeaderType.Restricted | HttpHeaderType.MultiValue) }
			};
		}

		internal WebHeaderCollection(HttpHeaderType state, bool internallyUsed)
		{
			this._state = state;
			this._internallyUsed = internallyUsed;
		}

		protected WebHeaderCollection(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			if (serializationInfo == null)
			{
				throw new ArgumentNullException("serializationInfo");
			}
			try
			{
				this._internallyUsed = serializationInfo.GetBoolean("InternallyUsed");
				this._state = (HttpHeaderType)serializationInfo.GetInt32("State");
				int num = serializationInfo.GetInt32("Count");
				for (int i = 0; i < num; i++)
				{
					int num1 = num + i;
					base.Add(serializationInfo.GetString(i.ToString()), serializationInfo.GetString(num1.ToString()));
				}
			}
			catch (SerializationException serializationException1)
			{
				SerializationException serializationException = serializationException1;
				throw new ArgumentException(serializationException.Message, "serializationInfo", serializationException);
			}
		}

		public WebHeaderCollection()
		{
		}

		private void @add(string name, string value, bool ignoreRestricted)
		{
			this.doWithCheckingState((ignoreRestricted ? new Action<string, string>(this.addWithoutCheckingNameAndRestricted) : new Action<string, string>(this.addWithoutCheckingName)), WebHeaderCollection.checkName(name), value, true);
		}

		public void Add(string header)
		{
			if ((header == null ? true : header.Length == 0))
			{
				throw new ArgumentNullException("header");
			}
			int num = WebHeaderCollection.checkColonSeparated(header);
			this.@add(header.Substring(0, num), header.Substring(num + 1), false);
		}

		public void Add(HttpRequestHeader header, string value)
		{
			this.doWithCheckingState(new Action<string, string>(this.addWithoutCheckingName), WebHeaderCollection.Convert(header), value, false, true);
		}

		public void Add(HttpResponseHeader header, string value)
		{
			this.doWithCheckingState(new Action<string, string>(this.addWithoutCheckingName), WebHeaderCollection.Convert(header), value, true, true);
		}

		public override void Add(string name, string value)
		{
			this.@add(name, value, false);
		}

		private void addWithoutCheckingName(string name, string value)
		{
			this.doWithoutCheckingName(new Action<string, string>(this.Add), name, value);
		}

		private void addWithoutCheckingNameAndRestricted(string name, string value)
		{
			base.Add(name, WebHeaderCollection.checkValue(value));
		}

		protected void AddWithoutValidate(string headerName, string headerValue)
		{
			this.@add(headerName, headerValue, true);
		}

		private static int checkColonSeparated(string header)
		{
			int num = header.IndexOf(':');
			if (num == -1)
			{
				throw new ArgumentException("No colon could be found.", "header");
			}
			return num;
		}

		private static HttpHeaderType checkHeaderType(string name)
		{
			HttpHeaderType httpHeaderType;
			HttpHeaderInfo headerInfo = WebHeaderCollection.getHeaderInfo(name);
			if (headerInfo == null)
			{
				httpHeaderType = HttpHeaderType.Unspecified;
			}
			else if (!headerInfo.IsRequest || headerInfo.IsResponse)
			{
				httpHeaderType = (headerInfo.IsRequest || !headerInfo.IsResponse ? HttpHeaderType.Unspecified : HttpHeaderType.Response);
			}
			else
			{
				httpHeaderType = HttpHeaderType.Request;
			}
			return httpHeaderType;
		}

		private static string checkName(string name)
		{
			if ((name == null ? true : name.Length == 0))
			{
				throw new ArgumentNullException("name");
			}
			name = name.Trim();
			if (!WebHeaderCollection.IsHeaderName(name))
			{
				throw new ArgumentException("Contains invalid characters.", "name");
			}
			return name;
		}

		private void checkRestricted(string name)
		{
			if ((this._internallyUsed ? false : WebHeaderCollection.isRestricted(name, true)))
			{
				throw new ArgumentException("This header must be modified with the appropiate property.");
			}
		}

		private void checkState(bool response)
		{
			if (this._state != HttpHeaderType.Unspecified)
			{
				if ((!response ? false : this._state == HttpHeaderType.Request))
				{
					throw new InvalidOperationException("This collection has already been used to store the request headers.");
				}
				if ((response ? false : this._state == HttpHeaderType.Response))
				{
					throw new InvalidOperationException("This collection has already been used to store the response headers.");
				}
			}
		}

		private static string checkValue(string value)
		{
			string empty;
			if ((value == null ? false : value.Length != 0))
			{
				value = value.Trim();
				if (value.Length > 65535)
				{
					throw new ArgumentOutOfRangeException("value", "Greater than 65,535 characters.");
				}
				if (!WebHeaderCollection.IsHeaderValue(value))
				{
					throw new ArgumentException("Contains invalid characters.", "value");
				}
				empty = value;
			}
			else
			{
				empty = string.Empty;
			}
			return empty;
		}

		public override void Clear()
		{
			base.Clear();
			this._state = HttpHeaderType.Unspecified;
		}

		private static string convert(string key)
		{
			HttpHeaderInfo httpHeaderInfo;
			return (WebHeaderCollection._headers.TryGetValue(key, out httpHeaderInfo) ? httpHeaderInfo.Name : string.Empty);
		}

		internal static string Convert(HttpRequestHeader header)
		{
			return WebHeaderCollection.convert(header.ToString());
		}

		internal static string Convert(HttpResponseHeader header)
		{
			return WebHeaderCollection.convert(header.ToString());
		}

		private void doWithCheckingState(Action<string, string> action, string name, string value, bool setState)
		{
			HttpHeaderType httpHeaderType = WebHeaderCollection.checkHeaderType(name);
			if (httpHeaderType == HttpHeaderType.Request)
			{
				this.doWithCheckingState(action, name, value, false, setState);
			}
			else if (httpHeaderType != HttpHeaderType.Response)
			{
				action(name, value);
			}
			else
			{
				this.doWithCheckingState(action, name, value, true, setState);
			}
		}

		private void doWithCheckingState(Action<string, string> action, string name, string value, bool response, bool setState)
		{
			this.checkState(response);
			action(name, value);
			if ((!setState ? false : this._state == HttpHeaderType.Unspecified))
			{
				this._state = (response ? HttpHeaderType.Response : HttpHeaderType.Request);
			}
		}

		private void doWithoutCheckingName(Action<string, string> action, string name, string value)
		{
			this.checkRestricted(name);
			action(name, WebHeaderCollection.checkValue(value));
		}

		public override string Get(int index)
		{
			return base.Get(index);
		}

		public override string Get(string name)
		{
			return base.Get(name);
		}

		public override IEnumerator GetEnumerator()
		{
			return base.GetEnumerator();
		}

		private static HttpHeaderInfo getHeaderInfo(string name)
		{
			HttpHeaderInfo httpHeaderInfo;
			foreach (HttpHeaderInfo value in WebHeaderCollection._headers.Values)
			{
				if (!value.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}
				httpHeaderInfo = value;
				return httpHeaderInfo;
			}
			httpHeaderInfo = null;
			return httpHeaderInfo;
		}

		public override string GetKey(int index)
		{
			return base.GetKey(index);
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			if (serializationInfo == null)
			{
				throw new ArgumentNullException("serializationInfo");
			}
			serializationInfo.AddValue("InternallyUsed", this._internallyUsed);
			serializationInfo.AddValue("State", (int)this._state);
			int count = this.Count;
			serializationInfo.AddValue("Count", count);
			count.Times((int i) => {
				serializationInfo.AddValue(i.ToString(), this.GetKey(i));
				serializationInfo.AddValue((count + i).ToString(), this.Get(i));
			});
		}

		public override string[] GetValues(int index)
		{
			string[] strArrays;
			string[] values = base.GetValues(index);
			if (values == null || values.Length == 0)
			{
				strArrays = null;
			}
			else
			{
				strArrays = values;
			}
			return strArrays;
		}

		public override string[] GetValues(string header)
		{
			string[] strArrays;
			string[] values = base.GetValues(header);
			if (values == null || values.Length == 0)
			{
				strArrays = null;
			}
			else
			{
				strArrays = values;
			}
			return strArrays;
		}

		internal void InternalRemove(string name)
		{
			base.Remove(name);
		}

		internal void InternalSet(string header, bool response)
		{
			int num = WebHeaderCollection.checkColonSeparated(header);
			this.InternalSet(header.Substring(0, num), header.Substring(num + 1), response);
		}

		internal void InternalSet(string name, string value, bool response)
		{
			value = WebHeaderCollection.checkValue(value);
			if (!WebHeaderCollection.IsMultiValue(name, response))
			{
				base.Set(name, value);
			}
			else
			{
				base.Add(name, value);
			}
		}

		internal static bool IsHeaderName(string name)
		{
			return (name == null || name.Length <= 0 ? false : name.IsToken());
		}

		internal static bool IsHeaderValue(string value)
		{
			return value.IsText();
		}

		internal static bool IsMultiValue(string headerName, bool response)
		{
			bool flag;
			if ((headerName == null ? false : headerName.Length != 0))
			{
				HttpHeaderInfo headerInfo = WebHeaderCollection.getHeaderInfo(headerName);
				flag = (headerInfo == null ? false : headerInfo.IsMultiValue(response));
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		private static bool isRestricted(string name, bool response)
		{
			HttpHeaderInfo headerInfo = WebHeaderCollection.getHeaderInfo(name);
			return (headerInfo == null ? false : headerInfo.IsRestricted(response));
		}

		public static bool IsRestricted(string headerName)
		{
			return WebHeaderCollection.isRestricted(WebHeaderCollection.checkName(headerName), false);
		}

		public static bool IsRestricted(string headerName, bool response)
		{
			return WebHeaderCollection.isRestricted(WebHeaderCollection.checkName(headerName), response);
		}

		public override void OnDeserialization(object sender)
		{
		}

		public void Remove(HttpRequestHeader header)
		{
			this.doWithCheckingState(new Action<string, string>(this.removeWithoutCheckingName), WebHeaderCollection.Convert(header), null, false, false);
		}

		public void Remove(HttpResponseHeader header)
		{
			this.doWithCheckingState(new Action<string, string>(this.removeWithoutCheckingName), WebHeaderCollection.Convert(header), null, true, false);
		}

		public override void Remove(string name)
		{
			this.doWithCheckingState(new Action<string, string>(this.removeWithoutCheckingName), WebHeaderCollection.checkName(name), null, false);
		}

		private void removeWithoutCheckingName(string name, string unuse)
		{
			this.checkRestricted(name);
			base.Remove(name);
		}

		public void Set(HttpRequestHeader header, string value)
		{
			this.doWithCheckingState(new Action<string, string>(this.setWithoutCheckingName), WebHeaderCollection.Convert(header), value, false, true);
		}

		public void Set(HttpResponseHeader header, string value)
		{
			this.doWithCheckingState(new Action<string, string>(this.setWithoutCheckingName), WebHeaderCollection.Convert(header), value, true, true);
		}

		public override void Set(string name, string value)
		{
			this.doWithCheckingState(new Action<string, string>(this.setWithoutCheckingName), WebHeaderCollection.checkName(name), value, true);
		}

		private void setWithoutCheckingName(string name, string value)
		{
			this.doWithoutCheckingName(new Action<string, string>(this.Set), name, value);
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter, SerializationFormatter=true)]
		void System.Runtime.Serialization.ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			this.GetObjectData(serializationInfo, streamingContext);
		}

		public byte[] ToByteArray()
		{
			return Encoding.UTF8.GetBytes(this.ToString());
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.Count.Times((int i) => stringBuilder.AppendFormat("{0}: {1}\r\n", this.GetKey(i), this.Get(i)));
			return stringBuilder.Append("\r\n").ToString();
		}

		internal string ToStringMultiValue(bool response)
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.Count.Times((int i) => {
				string key = this.GetKey(i);
				if (!WebHeaderCollection.IsMultiValue(key, response))
				{
					stringBuilder.AppendFormat("{0}: {1}\r\n", key, this.Get(i));
				}
				else
				{
					string[] values = this.GetValues(i);
					for (int num = 0; num < (int)values.Length; num++)
					{
						string str = values[num];
						stringBuilder.AppendFormat("{0}: {1}\r\n", key, str);
					}
				}
			});
			return stringBuilder.Append("\r\n").ToString();
		}
	}
}