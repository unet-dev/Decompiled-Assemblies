using System;
using System.Collections.Specialized;
using System.Security.Principal;
using WebSocketSharp;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp.Net
{
	public sealed class HttpListenerContext
	{
		private HttpConnection _connection;

		private string _error;

		private int _errorStatus;

		private HttpListener _listener;

		private HttpListenerRequest _request;

		private HttpListenerResponse _response;

		private IPrincipal _user;

		private HttpListenerWebSocketContext _websocketContext;

		internal HttpConnection Connection
		{
			get
			{
				return this._connection;
			}
		}

		internal string ErrorMessage
		{
			get
			{
				return this._error;
			}
			set
			{
				this._error = value;
			}
		}

		internal int ErrorStatus
		{
			get
			{
				return this._errorStatus;
			}
			set
			{
				this._errorStatus = value;
			}
		}

		internal bool HasError
		{
			get
			{
				return this._error != null;
			}
		}

		internal HttpListener Listener
		{
			get
			{
				return this._listener;
			}
			set
			{
				this._listener = value;
			}
		}

		public HttpListenerRequest Request
		{
			get
			{
				return this._request;
			}
		}

		public HttpListenerResponse Response
		{
			get
			{
				return this._response;
			}
		}

		public IPrincipal User
		{
			get
			{
				return this._user;
			}
		}

		internal HttpListenerContext(HttpConnection connection)
		{
			this._connection = connection;
			this._errorStatus = 400;
			this._request = new HttpListenerRequest(this);
			this._response = new HttpListenerResponse(this);
		}

		public HttpListenerWebSocketContext AcceptWebSocket(string protocol)
		{
			if (this._websocketContext != null)
			{
				throw new InvalidOperationException("The accepting is already in progress.");
			}
			if (protocol != null)
			{
				if (protocol.Length == 0)
				{
					throw new ArgumentException("An empty string.", "protocol");
				}
				if (!protocol.IsToken())
				{
					throw new ArgumentException("Contains an invalid character.", "protocol");
				}
			}
			this._websocketContext = new HttpListenerWebSocketContext(this, protocol);
			return this._websocketContext;
		}

		internal bool Authenticate()
		{
			bool flag;
			AuthenticationSchemes authenticationScheme = this._listener.SelectAuthenticationScheme(this._request);
			if (authenticationScheme == AuthenticationSchemes.Anonymous)
			{
				flag = true;
			}
			else if (authenticationScheme != AuthenticationSchemes.None)
			{
				string realm = this._listener.GetRealm();
				IPrincipal principal = HttpUtility.CreateUser(this._request.Headers["Authorization"], authenticationScheme, realm, this._request.HttpMethod, this._listener.GetUserCredentialsFinder());
				if ((principal == null ? false : principal.Identity.IsAuthenticated))
				{
					this._user = principal;
					flag = true;
				}
				else
				{
					this._response.CloseWithAuthChallenge((new AuthenticationChallenge(authenticationScheme, realm)).ToString());
					flag = false;
				}
			}
			else
			{
				this._response.Close(HttpStatusCode.Forbidden);
				flag = false;
			}
			return flag;
		}

		internal bool Register()
		{
			return this._listener.RegisterContext(this);
		}

		internal void Unregister()
		{
			this._listener.UnregisterContext(this);
		}
	}
}