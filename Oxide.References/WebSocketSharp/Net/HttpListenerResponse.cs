using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using WebSocketSharp;

namespace WebSocketSharp.Net
{
	public sealed class HttpListenerResponse : IDisposable
	{
		private bool _closeConnection;

		private Encoding _contentEncoding;

		private long _contentLength;

		private string _contentType;

		private WebSocketSharp.Net.HttpListenerContext _context;

		private WebSocketSharp.Net.CookieCollection _cookies;

		private bool _disposed;

		private WebSocketSharp.Net.WebHeaderCollection _headers;

		private bool _headersSent;

		private bool _keepAlive;

		private string _location;

		private ResponseStream _outputStream;

		private bool _sendChunked;

		private int _statusCode;

		private string _statusDescription;

		private Version _version;

		internal bool CloseConnection
		{
			get
			{
				return this._closeConnection;
			}
			set
			{
				this._closeConnection = value;
			}
		}

		public Encoding ContentEncoding
		{
			get
			{
				return this._contentEncoding;
			}
			set
			{
				this.checkDisposed();
				this._contentEncoding = value;
			}
		}

		public long ContentLength64
		{
			get
			{
				return this._contentLength;
			}
			set
			{
				this.checkDisposedOrHeadersSent();
				if (value < (long)0)
				{
					throw new ArgumentOutOfRangeException("Less than zero.", "value");
				}
				this._contentLength = value;
			}
		}

		public string ContentType
		{
			get
			{
				return this._contentType;
			}
			set
			{
				this.checkDisposed();
				if ((value == null ? false : value.Length == 0))
				{
					throw new ArgumentException("An empty string.", "value");
				}
				this._contentType = value;
			}
		}

		public WebSocketSharp.Net.CookieCollection Cookies
		{
			get
			{
				WebSocketSharp.Net.CookieCollection cookieCollections = this._cookies;
				if (cookieCollections == null)
				{
					WebSocketSharp.Net.CookieCollection cookieCollections1 = new WebSocketSharp.Net.CookieCollection();
					WebSocketSharp.Net.CookieCollection cookieCollections2 = cookieCollections1;
					this._cookies = cookieCollections1;
					cookieCollections = cookieCollections2;
				}
				return cookieCollections;
			}
			set
			{
				this._cookies = value;
			}
		}

		public WebSocketSharp.Net.WebHeaderCollection Headers
		{
			get
			{
				WebSocketSharp.Net.WebHeaderCollection webHeaderCollection = this._headers;
				if (webHeaderCollection == null)
				{
					WebSocketSharp.Net.WebHeaderCollection webHeaderCollection1 = new WebSocketSharp.Net.WebHeaderCollection(HttpHeaderType.Response, false);
					WebSocketSharp.Net.WebHeaderCollection webHeaderCollection2 = webHeaderCollection1;
					this._headers = webHeaderCollection1;
					webHeaderCollection = webHeaderCollection2;
				}
				return webHeaderCollection;
			}
			set
			{
				if ((value == null ? false : value.State != HttpHeaderType.Response))
				{
					throw new InvalidOperationException("The specified headers aren't valid for a response.");
				}
				this._headers = value;
			}
		}

		internal bool HeadersSent
		{
			get
			{
				return this._headersSent;
			}
			set
			{
				this._headersSent = value;
			}
		}

		public bool KeepAlive
		{
			get
			{
				return this._keepAlive;
			}
			set
			{
				this.checkDisposedOrHeadersSent();
				this._keepAlive = value;
			}
		}

		public Stream OutputStream
		{
			get
			{
				this.checkDisposed();
				ResponseStream responseStream = this._outputStream;
				if (responseStream == null)
				{
					ResponseStream responseStream1 = this._context.Connection.GetResponseStream();
					ResponseStream responseStream2 = responseStream1;
					this._outputStream = responseStream1;
					responseStream = responseStream2;
				}
				return responseStream;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				return this._version;
			}
			set
			{
				bool flag;
				this.checkDisposedOrHeadersSent();
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Major != 1)
				{
					flag = true;
				}
				else
				{
					flag = (value.Minor == 0 ? false : value.Minor != 1);
				}
				if (flag)
				{
					throw new ArgumentException("Not 1.0 or 1.1.", "value");
				}
				this._version = value;
			}
		}

		public string RedirectLocation
		{
			get
			{
				return this._location;
			}
			set
			{
				this.checkDisposed();
				if (value != null)
				{
					Uri uri = null;
					if ((!value.MaybeUri() ? true : !Uri.TryCreate(value, UriKind.Absolute, out uri)))
					{
						throw new ArgumentException("Not an absolute URL.", "value");
					}
					this._location = value;
				}
				else
				{
					this._location = null;
				}
			}
		}

		public bool SendChunked
		{
			get
			{
				return this._sendChunked;
			}
			set
			{
				this.checkDisposedOrHeadersSent();
				this._sendChunked = value;
			}
		}

		public int StatusCode
		{
			get
			{
				return this._statusCode;
			}
			set
			{
				this.checkDisposedOrHeadersSent();
				if ((value < 100 ? true : value > 999))
				{
					throw new ProtocolViolationException("A value isn't between 100 and 999 inclusive.");
				}
				this._statusCode = value;
				this._statusDescription = value.GetStatusDescription();
			}
		}

		public string StatusDescription
		{
			get
			{
				return this._statusDescription;
			}
			set
			{
				this.checkDisposedOrHeadersSent();
				if ((value == null ? false : value.Length != 0))
				{
					if ((!value.IsText() ? true : value.IndexOfAny(new char[] { '\r', '\n' }) > -1))
					{
						throw new ArgumentException("Contains invalid characters.", "value");
					}
					this._statusDescription = value;
				}
				else
				{
					this._statusDescription = this._statusCode.GetStatusDescription();
				}
			}
		}

		internal HttpListenerResponse(WebSocketSharp.Net.HttpListenerContext context)
		{
			this._context = context;
			this._keepAlive = true;
			this._statusCode = 200;
			this._statusDescription = "OK";
			this._version = WebSocketSharp.Net.HttpVersion.Version11;
		}

		public void Abort()
		{
			if (!this._disposed)
			{
				this.close(true);
			}
		}

		public void AddHeader(string name, string value)
		{
			this.Headers.Set(name, value);
		}

		public void AppendCookie(WebSocketSharp.Net.Cookie cookie)
		{
			this.Cookies.Add(cookie);
		}

		public void AppendHeader(string name, string value)
		{
			this.Headers.Add(name, value);
		}

		private bool canAddOrUpdate(WebSocketSharp.Net.Cookie cookie)
		{
			bool flag;
			if ((this._cookies == null ? false : this._cookies.Count != 0))
			{
				List<WebSocketSharp.Net.Cookie> list = this.findCookie(cookie).ToList<WebSocketSharp.Net.Cookie>();
				if (list.Count != 0)
				{
					int version = cookie.Version;
					foreach (WebSocketSharp.Net.Cookie cookie1 in list)
					{
						if (cookie1.Version != version)
						{
							continue;
						}
						flag = true;
						return flag;
					}
					flag = false;
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		private void checkDisposed()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException(this.GetType().ToString());
			}
		}

		private void checkDisposedOrHeadersSent()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException(this.GetType().ToString());
			}
			if (this._headersSent)
			{
				throw new InvalidOperationException("Cannot be changed after the headers are sent.");
			}
		}

		private void close(bool force)
		{
			this._disposed = true;
			this._context.Connection.Close(force);
		}

		public void Close()
		{
			if (!this._disposed)
			{
				this.close(false);
			}
		}

		public void Close(byte[] responseEntity, bool willBlock)
		{
			this.checkDisposed();
			if (responseEntity == null)
			{
				throw new ArgumentNullException("responseEntity");
			}
			int length = (int)responseEntity.Length;
			Stream outputStream = this.OutputStream;
			if (!willBlock)
			{
				outputStream.BeginWrite(responseEntity, 0, length, (IAsyncResult ar) => {
					outputStream.EndWrite(ar);
					this.close(false);
				}, null);
			}
			else
			{
				outputStream.Write(responseEntity, 0, length);
				this.close(false);
			}
		}

		public void CopyFrom(WebSocketSharp.Net.HttpListenerResponse templateResponse)
		{
			if (templateResponse == null)
			{
				throw new ArgumentNullException("templateResponse");
			}
			if (templateResponse._headers != null)
			{
				if (this._headers != null)
				{
					this._headers.Clear();
				}
				this.Headers.Add(templateResponse._headers);
			}
			else if (this._headers != null)
			{
				this._headers = null;
			}
			this._contentLength = templateResponse._contentLength;
			this._statusCode = templateResponse._statusCode;
			this._statusDescription = templateResponse._statusDescription;
			this._keepAlive = templateResponse._keepAlive;
			this._version = templateResponse._version;
		}

		private IEnumerable<WebSocketSharp.Net.Cookie> findCookie(WebSocketSharp.Net.Cookie cookie)
		{
			bool flag;
			string str = cookie.Name;
			string str1 = cookie.Domain;
			string str2 = cookie.Path;
			if (this._cookies != null)
			{
				foreach (WebSocketSharp.Net.Cookie _cooky in this._cookies)
				{
					flag = (!_cooky.Name.Equals(str, StringComparison.OrdinalIgnoreCase) || !_cooky.Domain.Equals(str1, StringComparison.OrdinalIgnoreCase) ? false : _cooky.Path.Equals(str2, StringComparison.Ordinal));
					if (flag)
					{
						yield return _cooky;
					}
				}
			}
		}

		public void Redirect(string url)
		{
			this.checkDisposedOrHeadersSent();
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			Uri uri = null;
			if ((!url.MaybeUri() ? true : !Uri.TryCreate(url, UriKind.Absolute, out uri)))
			{
				throw new ArgumentException("Not an absolute URL.", "url");
			}
			this._location = url;
			this._statusCode = 302;
			this._statusDescription = "Found";
		}

		public void SetCookie(WebSocketSharp.Net.Cookie cookie)
		{
			if (cookie == null)
			{
				throw new ArgumentNullException("cookie");
			}
			if (!this.canAddOrUpdate(cookie))
			{
				throw new ArgumentException("Cannot be replaced.", "cookie");
			}
			this.Cookies.Add(cookie);
		}

		void System.IDisposable.Dispose()
		{
			if (!this._disposed)
			{
				this.close(true);
			}
		}

		internal WebSocketSharp.Net.WebHeaderCollection WriteHeadersTo(MemoryStream destination)
		{
			WebSocketSharp.Net.WebHeaderCollection webHeaderCollection = new WebSocketSharp.Net.WebHeaderCollection(HttpHeaderType.Response, true);
			if (this._headers != null)
			{
				webHeaderCollection.Add(this._headers);
			}
			if (this._contentType != null)
			{
				webHeaderCollection.InternalSet("Content-Type", (this._contentType.IndexOf("charset=", StringComparison.Ordinal) != -1 || this._contentEncoding == null ? this._contentType : string.Format("{0}; charset={1}", this._contentType, this._contentEncoding.WebName)), true);
			}
			if (webHeaderCollection["Server"] == null)
			{
				webHeaderCollection.InternalSet("Server", "websocket-sharp/1.0", true);
			}
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			if (webHeaderCollection["Date"] == null)
			{
				DateTime utcNow = DateTime.UtcNow;
				webHeaderCollection.InternalSet("Date", utcNow.ToString("r", invariantCulture), true);
			}
			if (this._sendChunked)
			{
				webHeaderCollection.InternalSet("Transfer-Encoding", "chunked", true);
			}
			else
			{
				webHeaderCollection.InternalSet("Content-Length", this._contentLength.ToString(invariantCulture), true);
			}
			bool flag = (!this._context.Request.KeepAlive || !this._keepAlive || this._statusCode == 400 || this._statusCode == 408 || this._statusCode == 411 || this._statusCode == 413 || this._statusCode == 414 || this._statusCode == 500 ? true : this._statusCode == 503);
			int reuses = this._context.Connection.Reuses;
			if ((flag ? false : reuses < 100))
			{
				webHeaderCollection.InternalSet("Keep-Alive", string.Format("timeout=15,max={0}", 100 - reuses), true);
				if (this._context.Request.ProtocolVersion < WebSocketSharp.Net.HttpVersion.Version11)
				{
					webHeaderCollection.InternalSet("Connection", "keep-alive", true);
				}
			}
			else
			{
				webHeaderCollection.InternalSet("Connection", "close", true);
			}
			if (this._location != null)
			{
				webHeaderCollection.InternalSet("Location", this._location, true);
			}
			if (this._cookies != null)
			{
				foreach (WebSocketSharp.Net.Cookie _cooky in this._cookies)
				{
					webHeaderCollection.InternalSet("Set-Cookie", _cooky.ToResponseString(), true);
				}
			}
			Encoding @default = this._contentEncoding ?? Encoding.Default;
			StreamWriter streamWriter = new StreamWriter(destination, @default, 256);
			streamWriter.Write("HTTP/{0} {1} {2}\r\n", this._version, this._statusCode, this._statusDescription);
			streamWriter.Write(webHeaderCollection.ToStringMultiValue(true));
			streamWriter.Flush();
			destination.Position = (long)((int)@default.GetPreamble().Length);
			return webHeaderCollection;
		}
	}
}