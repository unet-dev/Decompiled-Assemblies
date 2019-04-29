using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace WebSocketSharp.Net.WebSockets
{
	public class HttpListenerWebSocketContext : WebSocketContext
	{
		private WebSocketSharp.Net.HttpListenerContext _context;

		private WebSocketSharp.WebSocket _websocket;

		public override WebSocketSharp.Net.CookieCollection CookieCollection
		{
			get
			{
				return this._context.Request.Cookies;
			}
		}

		public override NameValueCollection Headers
		{
			get
			{
				return this._context.Request.Headers;
			}
		}

		public override string Host
		{
			get
			{
				return this._context.Request.Headers["Host"];
			}
		}

		public override bool IsAuthenticated
		{
			get
			{
				return this._context.User != null;
			}
		}

		public override bool IsLocal
		{
			get
			{
				return this._context.Request.IsLocal;
			}
		}

		public override bool IsSecureConnection
		{
			get
			{
				return this._context.Connection.IsSecure;
			}
		}

		public override bool IsWebSocketRequest
		{
			get
			{
				return this._context.Request.IsWebSocketRequest;
			}
		}

		internal Logger Log
		{
			get
			{
				return this._context.Listener.Log;
			}
		}

		public override string Origin
		{
			get
			{
				return this._context.Request.Headers["Origin"];
			}
		}

		public override NameValueCollection QueryString
		{
			get
			{
				return this._context.Request.QueryString;
			}
		}

		public override Uri RequestUri
		{
			get
			{
				return this._context.Request.Url;
			}
		}

		public override string SecWebSocketKey
		{
			get
			{
				return this._context.Request.Headers["Sec-WebSocket-Key"];
			}
		}

		public override IEnumerable<string> SecWebSocketProtocols
		{
			get
			{
				string item = this._context.Request.Headers["Sec-WebSocket-Protocol"];
				if (item != null)
				{
					string[] strArrays = item.Split(new char[] { ',' });
					for (int i = 0; i < (int)strArrays.Length; i++)
					{
						string str = strArrays[i];
						yield return str.Trim();
						str = null;
					}
					strArrays = null;
				}
			}
		}

		public override string SecWebSocketVersion
		{
			get
			{
				return this._context.Request.Headers["Sec-WebSocket-Version"];
			}
		}

		public override IPEndPoint ServerEndPoint
		{
			get
			{
				return this._context.Connection.LocalEndPoint;
			}
		}

		internal System.IO.Stream Stream
		{
			get
			{
				return this._context.Connection.Stream;
			}
		}

		public override IPrincipal User
		{
			get
			{
				return this._context.User;
			}
		}

		public override IPEndPoint UserEndPoint
		{
			get
			{
				return this._context.Connection.RemoteEndPoint;
			}
		}

		public override WebSocketSharp.WebSocket WebSocket
		{
			get
			{
				return this._websocket;
			}
		}

		internal HttpListenerWebSocketContext(WebSocketSharp.Net.HttpListenerContext context, string protocol)
		{
			this._context = context;
			this._websocket = new WebSocketSharp.WebSocket(this, protocol);
		}

		internal void Close()
		{
			this._context.Connection.Close(true);
		}

		internal void Close(WebSocketSharp.Net.HttpStatusCode code)
		{
			this._context.Response.Close(code);
		}

		public override string ToString()
		{
			return this._context.Request.ToString();
		}
	}
}