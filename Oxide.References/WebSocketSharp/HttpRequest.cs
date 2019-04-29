using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using WebSocketSharp.Net;

namespace WebSocketSharp
{
	internal class HttpRequest : HttpBase
	{
		private string _method;

		private string _uri;

		private bool _websocketRequest;

		private bool _websocketRequestSet;

		public WebSocketSharp.Net.AuthenticationResponse AuthenticationResponse
		{
			get
			{
				WebSocketSharp.Net.AuthenticationResponse authenticationResponse;
				string item = base.Headers["Authorization"];
				if (item == null || item.Length <= 0)
				{
					authenticationResponse = null;
				}
				else
				{
					authenticationResponse = WebSocketSharp.Net.AuthenticationResponse.Parse(item);
				}
				return authenticationResponse;
			}
		}

		public CookieCollection Cookies
		{
			get
			{
				return base.Headers.GetCookies(false);
			}
		}

		public string HttpMethod
		{
			get
			{
				return this._method;
			}
		}

		public bool IsWebSocketRequest
		{
			get
			{
				if (!this._websocketRequestSet)
				{
					NameValueCollection headers = base.Headers;
					this._websocketRequest = (!(this._method == "GET") || !(base.ProtocolVersion > HttpVersion.Version10) || !headers.Contains("Upgrade", "websocket") ? false : headers.Contains("Connection", "Upgrade"));
					this._websocketRequestSet = true;
				}
				return this._websocketRequest;
			}
		}

		public string RequestUri
		{
			get
			{
				return this._uri;
			}
		}

		private HttpRequest(string method, string uri, Version version, NameValueCollection headers) : base(version, headers)
		{
			this._method = method;
			this._uri = uri;
		}

		internal HttpRequest(string method, string uri) : this(method, uri, HttpVersion.Version11, new NameValueCollection())
		{
			base.Headers["User-Agent"] = "websocket-sharp/1.0";
		}

		internal static HttpRequest CreateConnectRequest(Uri uri)
		{
			string dnsSafeHost = uri.DnsSafeHost;
			int port = uri.Port;
			string str = string.Format("{0}:{1}", dnsSafeHost, port);
			HttpRequest httpRequest = new HttpRequest("CONNECT", str);
			httpRequest.Headers["Host"] = (port == 80 ? dnsSafeHost : str);
			return httpRequest;
		}

		internal static HttpRequest CreateWebSocketRequest(Uri uri)
		{
			HttpRequest httpRequest = new HttpRequest("GET", uri.PathAndQuery);
			NameValueCollection headers = httpRequest.Headers;
			int port = uri.Port;
			string scheme = uri.Scheme;
			headers["Host"] = ((port != 80 || !(scheme == "ws")) && (port != 443 || !(scheme == "wss")) ? uri.Authority : uri.DnsSafeHost);
			headers["Upgrade"] = "websocket";
			headers["Connection"] = "Upgrade";
			return httpRequest;
		}

		internal HttpResponse GetResponse(Stream stream, int millisecondsTimeout)
		{
			byte[] byteArray = base.ToByteArray();
			stream.Write(byteArray, 0, (int)byteArray.Length);
			HttpResponse httpResponse = HttpBase.Read<HttpResponse>(stream, new Func<string[], HttpResponse>(HttpResponse.Parse), millisecondsTimeout);
			return httpResponse;
		}

		internal static HttpRequest Parse(string[] headerParts)
		{
			string[] strArrays = headerParts[0].Split(new char[] { ' ' }, 3);
			if ((int)strArrays.Length != 3)
			{
				throw new ArgumentException(string.Concat("Invalid request line: ", headerParts[0]));
			}
			WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
			for (int i = 1; i < (int)headerParts.Length; i++)
			{
				webHeaderCollection.InternalSet(headerParts[i], false);
			}
			HttpRequest httpRequest = new HttpRequest(strArrays[0], strArrays[1], new Version(strArrays[2].Substring(5)), webHeaderCollection);
			return httpRequest;
		}

		internal static HttpRequest Read(Stream stream, int millisecondsTimeout)
		{
			HttpRequest httpRequest = HttpBase.Read<HttpRequest>(stream, new Func<string[], HttpRequest>(HttpRequest.Parse), millisecondsTimeout);
			return httpRequest;
		}

		public void SetCookies(CookieCollection cookies)
		{
			if ((cookies == null ? false : cookies.Count != 0))
			{
				StringBuilder stringBuilder = new StringBuilder(64);
				foreach (Cookie sorted in cookies.Sorted)
				{
					if (sorted.Expired)
					{
						continue;
					}
					stringBuilder.AppendFormat("{0}; ", sorted.ToString());
				}
				int length = stringBuilder.Length;
				if (length > 2)
				{
					stringBuilder.Length = length - 2;
					base.Headers["Cookie"] = stringBuilder.ToString();
				}
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0} {1} HTTP/{2}{3}", new object[] { this._method, this._uri, base.ProtocolVersion, "\r\n" });
			NameValueCollection headers = base.Headers;
			string[] allKeys = headers.AllKeys;
			for (int i = 0; i < (int)allKeys.Length; i++)
			{
				string str = allKeys[i];
				stringBuilder.AppendFormat("{0}: {1}{2}", str, headers[str], "\r\n");
			}
			stringBuilder.Append("\r\n");
			string entityBody = base.EntityBody;
			if (entityBody.Length > 0)
			{
				stringBuilder.Append(entityBody);
			}
			return stringBuilder.ToString();
		}
	}
}