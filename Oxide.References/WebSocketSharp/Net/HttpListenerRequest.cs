using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using WebSocketSharp;

namespace WebSocketSharp.Net
{
	public sealed class HttpListenerRequest
	{
		private readonly static byte[] _100continue;

		private string[] _acceptTypes;

		private bool _chunked;

		private Encoding _contentEncoding;

		private long _contentLength;

		private bool _contentLengthSet;

		private WebSocketSharp.Net.HttpListenerContext _context;

		private WebSocketSharp.Net.CookieCollection _cookies;

		private WebSocketSharp.Net.WebHeaderCollection _headers;

		private Guid _identifier;

		private Stream _inputStream;

		private bool _keepAlive;

		private bool _keepAliveSet;

		private string _method;

		private NameValueCollection _queryString;

		private Uri _referer;

		private string _uri;

		private Uri _url;

		private string[] _userLanguages;

		private Version _version;

		private bool _websocketRequest;

		private bool _websocketRequestSet;

		public string[] AcceptTypes
		{
			get
			{
				return this._acceptTypes;
			}
		}

		public int ClientCertificateError
		{
			get
			{
				return 0;
			}
		}

		public Encoding ContentEncoding
		{
			get
			{
				Encoding encoding = this._contentEncoding;
				if (encoding == null)
				{
					Encoding @default = Encoding.Default;
					Encoding encoding1 = @default;
					this._contentEncoding = @default;
					encoding = encoding1;
				}
				return encoding;
			}
		}

		public long ContentLength64
		{
			get
			{
				return this._contentLength;
			}
		}

		public string ContentType
		{
			get
			{
				return this._headers["Content-Type"];
			}
		}

		public WebSocketSharp.Net.CookieCollection Cookies
		{
			get
			{
				WebSocketSharp.Net.CookieCollection cookieCollections = this._cookies;
				if (cookieCollections == null)
				{
					WebSocketSharp.Net.CookieCollection cookies = this._headers.GetCookies(false);
					WebSocketSharp.Net.CookieCollection cookieCollections1 = cookies;
					this._cookies = cookies;
					cookieCollections = cookieCollections1;
				}
				return cookieCollections;
			}
		}

		public bool HasEntityBody
		{
			get
			{
				return (this._contentLength > (long)0 ? true : this._chunked);
			}
		}

		public NameValueCollection Headers
		{
			get
			{
				return this._headers;
			}
		}

		public string HttpMethod
		{
			get
			{
				return this._method;
			}
		}

		public Stream InputStream
		{
			get
			{
				Stream requestStream;
				Stream stream = this._inputStream;
				if (stream == null)
				{
					if (this.HasEntityBody)
					{
						requestStream = this._context.Connection.GetRequestStream(this._contentLength, this._chunked);
					}
					else
					{
						requestStream = Stream.Null;
					}
					Stream stream1 = requestStream;
					this._inputStream = requestStream;
					stream = stream1;
				}
				return stream;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return this._context.User != null;
			}
		}

		public bool IsLocal
		{
			get
			{
				return this.RemoteEndPoint.Address.IsLocal();
			}
		}

		public bool IsSecureConnection
		{
			get
			{
				return this._context.Connection.IsSecure;
			}
		}

		public bool IsWebSocketRequest
		{
			get
			{
				if (!this._websocketRequestSet)
				{
					this._websocketRequest = (!(this._method == "GET") || !(this._version > WebSocketSharp.Net.HttpVersion.Version10) || !this._headers.Contains("Upgrade", "websocket") ? false : this._headers.Contains("Connection", "Upgrade"));
					this._websocketRequestSet = true;
				}
				return this._websocketRequest;
			}
		}

		public bool KeepAlive
		{
			get
			{
				bool flag;
				if (!this._keepAliveSet)
				{
					if (this._version > WebSocketSharp.Net.HttpVersion.Version10 || this._headers.Contains("Connection", "keep-alive"))
					{
						flag = true;
					}
					else
					{
						string item = this._headers["Keep-Alive"];
						string str = item;
						flag = (item == null ? false : str != "closed");
					}
					this._keepAlive = flag;
					this._keepAliveSet = true;
				}
				return this._keepAlive;
			}
		}

		public IPEndPoint LocalEndPoint
		{
			get
			{
				return this._context.Connection.LocalEndPoint;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				return this._version;
			}
		}

		public NameValueCollection QueryString
		{
			get
			{
				NameValueCollection nameValueCollection = this._queryString;
				if (nameValueCollection == null)
				{
					NameValueCollection nameValueCollection1 = HttpUtility.InternalParseQueryString(this._url.Query, Encoding.UTF8);
					NameValueCollection nameValueCollection2 = nameValueCollection1;
					this._queryString = nameValueCollection1;
					nameValueCollection = nameValueCollection2;
				}
				return nameValueCollection;
			}
		}

		public string RawUrl
		{
			get
			{
				return this._url.PathAndQuery;
			}
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return this._context.Connection.RemoteEndPoint;
			}
		}

		public Guid RequestTraceIdentifier
		{
			get
			{
				return this._identifier;
			}
		}

		public Uri Url
		{
			get
			{
				return this._url;
			}
		}

		public Uri UrlReferrer
		{
			get
			{
				return this._referer;
			}
		}

		public string UserAgent
		{
			get
			{
				return this._headers["User-Agent"];
			}
		}

		public string UserHostAddress
		{
			get
			{
				return this.LocalEndPoint.ToString();
			}
		}

		public string UserHostName
		{
			get
			{
				return this._headers["Host"];
			}
		}

		public string[] UserLanguages
		{
			get
			{
				return this._userLanguages;
			}
		}

		static HttpListenerRequest()
		{
			WebSocketSharp.Net.HttpListenerRequest._100continue = Encoding.ASCII.GetBytes("HTTP/1.1 100 Continue\r\n\r\n");
		}

		internal HttpListenerRequest(WebSocketSharp.Net.HttpListenerContext context)
		{
			this._context = context;
			this._contentLength = (long)-1;
			this._headers = new WebSocketSharp.Net.WebHeaderCollection();
			this._identifier = Guid.NewGuid();
		}

		internal void AddHeader(string header)
		{
			long num;
			int num1 = header.IndexOf(':');
			if (num1 != -1)
			{
				string str = header.Substring(0, num1).Trim();
				string str1 = header.Substring(num1 + 1).Trim();
				this._headers.InternalSet(str, str1, false);
				string lower = str.ToLower(CultureInfo.InvariantCulture);
				if (lower == "accept")
				{
					this._acceptTypes = (new List<string>(str1.SplitHeaderValue(new char[] { ',' }))).ToArray();
				}
				else if (lower == "accept-language")
				{
					this._userLanguages = str1.Split(new char[] { ',' });
				}
				else if (lower == "content-length")
				{
					if ((!long.TryParse(str1, out num) ? true : num < (long)0))
					{
						this._context.ErrorMessage = "Invalid Content-Length header";
					}
					else
					{
						this._contentLength = num;
						this._contentLengthSet = true;
					}
				}
				else if (lower == "content-type")
				{
					try
					{
						this._contentEncoding = HttpUtility.GetEncoding(str1);
					}
					catch
					{
						this._context.ErrorMessage = "Invalid Content-Type header";
					}
				}
				else if (lower == "referer")
				{
					this._referer = str1.ToUri();
				}
			}
			else
			{
				this._context.ErrorMessage = "Invalid header";
			}
		}

		public IAsyncResult BeginGetClientCertificate(AsyncCallback requestCallback, object state)
		{
			throw new NotImplementedException();
		}

		public X509Certificate2 EndGetClientCertificate(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		internal void FinishInitialization()
		{
			string item = this._headers["Host"];
			bool flag = (item == null ? true : item.Length == 0);
			if (!((this._version > WebSocketSharp.Net.HttpVersion.Version10) & flag))
			{
				if (flag)
				{
					item = this.UserHostAddress;
				}
				this._url = HttpUtility.CreateRequestUrl(this._uri, item, this.IsWebSocketRequest, this.IsSecureConnection);
				if (this._url != null)
				{
					string str = this.Headers["Transfer-Encoding"];
					if ((!(this._version > WebSocketSharp.Net.HttpVersion.Version10) || str == null ? false : str.Length > 0))
					{
						this._chunked = str.ToLower() == "chunked";
						if (!this._chunked)
						{
							this._context.ErrorMessage = string.Empty;
							this._context.ErrorStatus = 501;
							return;
						}
					}
					if ((this._chunked ? false : !this._contentLengthSet))
					{
						string lower = this._method.ToLower();
						if ((lower == "post" ? true : lower == "put"))
						{
							this._context.ErrorMessage = string.Empty;
							this._context.ErrorStatus = 411;
							return;
						}
					}
					string item1 = this.Headers["Expect"];
					if ((item1 == null || item1.Length <= 0 ? false : item1.ToLower() == "100-continue"))
					{
						ResponseStream responseStream = this._context.Connection.GetResponseStream();
						responseStream.InternalWrite(WebSocketSharp.Net.HttpListenerRequest._100continue, 0, (int)WebSocketSharp.Net.HttpListenerRequest._100continue.Length);
					}
				}
				else
				{
					this._context.ErrorMessage = "Invalid request url";
				}
			}
			else
			{
				this._context.ErrorMessage = "Invalid Host header";
			}
		}

		internal bool FlushInput()
		{
			bool flag;
			if (this.HasEntityBody)
			{
				int num = 2048;
				if (this._contentLength > (long)0)
				{
					num = (int)Math.Min(this._contentLength, (long)num);
				}
				byte[] numArray = new byte[num];
				while (true)
				{
					try
					{
						IAsyncResult asyncResult = this.InputStream.BeginRead(numArray, 0, num, null, null);
						if ((asyncResult.IsCompleted ? false : !asyncResult.AsyncWaitHandle.WaitOne(100)))
						{
							flag = false;
							break;
						}
						else if (this.InputStream.EndRead(asyncResult) <= 0)
						{
							flag = true;
							break;
						}
					}
					catch
					{
						flag = false;
						break;
					}
				}
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		public X509Certificate2 GetClientCertificate()
		{
			throw new NotImplementedException();
		}

		internal void SetRequestLine(string requestLine)
		{
			string[] strArrays = requestLine.Split(new char[] { ' ' }, 3);
			if ((int)strArrays.Length == 3)
			{
				this._method = strArrays[0];
				if (this._method.IsToken())
				{
					this._uri = strArrays[1];
					string str = strArrays[2];
					if ((str.Length != 8 || !str.StartsWith("HTTP/") || !WebSocketSharp.Net.HttpListenerRequest.tryCreateVersion(str.Substring(5), out this._version) ? true : this._version.Major < 1))
					{
						this._context.ErrorMessage = "Invalid request line (version)";
					}
				}
				else
				{
					this._context.ErrorMessage = "Invalid request line (method)";
				}
			}
			else
			{
				this._context.ErrorMessage = "Invalid request line (parts)";
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0} {1} HTTP/{2}\r\n", this._method, this._uri, this._version);
			stringBuilder.Append(this._headers.ToString());
			return stringBuilder.ToString();
		}

		private static bool tryCreateVersion(string version, out Version result)
		{
			bool flag;
			try
			{
				result = new Version(version);
				flag = true;
			}
			catch
			{
				result = null;
				flag = false;
			}
			return flag;
		}
	}
}