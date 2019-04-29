using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using WebSocketSharp.Net;

namespace WebSocketSharp
{
	internal class HttpResponse : HttpBase
	{
		private string _code;

		private string _reason;

		public CookieCollection Cookies
		{
			get
			{
				return base.Headers.GetCookies(true);
			}
		}

		public bool HasConnectionClose
		{
			get
			{
				return base.Headers.Contains("Connection", "close");
			}
		}

		public bool IsProxyAuthenticationRequired
		{
			get
			{
				return this._code == "407";
			}
		}

		public bool IsRedirect
		{
			get
			{
				return (this._code == "301" ? true : this._code == "302");
			}
		}

		public bool IsUnauthorized
		{
			get
			{
				return this._code == "401";
			}
		}

		public bool IsWebSocketResponse
		{
			get
			{
				NameValueCollection headers = base.Headers;
				return (!(base.ProtocolVersion > HttpVersion.Version10) || !(this._code == "101") || !headers.Contains("Upgrade", "websocket") ? false : headers.Contains("Connection", "Upgrade"));
			}
		}

		public string Reason
		{
			get
			{
				return this._reason;
			}
		}

		public string StatusCode
		{
			get
			{
				return this._code;
			}
		}

		private HttpResponse(string code, string reason, Version version, NameValueCollection headers) : base(version, headers)
		{
			this._code = code;
			this._reason = reason;
		}

		internal HttpResponse(HttpStatusCode code) : this(code, code.GetDescription())
		{
		}

		internal HttpResponse(HttpStatusCode code, string reason) : this(code.ToString(), reason, HttpVersion.Version11, new NameValueCollection())
		{
			base.Headers["Server"] = "websocket-sharp/1.0";
		}

		internal static HttpResponse CreateCloseResponse(HttpStatusCode code)
		{
			HttpResponse httpResponse = new HttpResponse(code);
			httpResponse.Headers["Connection"] = "close";
			return httpResponse;
		}

		internal static HttpResponse CreateUnauthorizedResponse(string challenge)
		{
			HttpResponse httpResponse = new HttpResponse(HttpStatusCode.Unauthorized);
			httpResponse.Headers["WWW-Authenticate"] = challenge;
			return httpResponse;
		}

		internal static HttpResponse CreateWebSocketResponse()
		{
			HttpResponse httpResponse = new HttpResponse(HttpStatusCode.SwitchingProtocols);
			NameValueCollection headers = httpResponse.Headers;
			headers["Upgrade"] = "websocket";
			headers["Connection"] = "Upgrade";
			return httpResponse;
		}

		internal static HttpResponse Parse(string[] headerParts)
		{
			string[] strArrays = headerParts[0].Split(new char[] { ' ' }, 3);
			if ((int)strArrays.Length != 3)
			{
				throw new ArgumentException(string.Concat("Invalid status line: ", headerParts[0]));
			}
			WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
			for (int i = 1; i < (int)headerParts.Length; i++)
			{
				webHeaderCollection.InternalSet(headerParts[i], true);
			}
			HttpResponse httpResponse = new HttpResponse(strArrays[1], strArrays[2], new Version(strArrays[0].Substring(5)), webHeaderCollection);
			return httpResponse;
		}

		internal static HttpResponse Read(Stream stream, int millisecondsTimeout)
		{
			HttpResponse httpResponse = HttpBase.Read<HttpResponse>(stream, new Func<string[], HttpResponse>(HttpResponse.Parse), millisecondsTimeout);
			return httpResponse;
		}

		public void SetCookies(CookieCollection cookies)
		{
			if ((cookies == null ? false : cookies.Count != 0))
			{
				NameValueCollection headers = base.Headers;
				foreach (Cookie sorted in cookies.Sorted)
				{
					headers.Add("Set-Cookie", sorted.ToResponseString());
				}
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("HTTP/{0} {1} {2}{3}", new object[] { base.ProtocolVersion, this._code, this._reason, "\r\n" });
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