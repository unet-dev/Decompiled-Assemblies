using System;
using System.IO;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp.Server
{
	public abstract class WebSocketBehavior : IWebSocketSession
	{
		private WebSocketContext _context;

		private Func<CookieCollection, CookieCollection, bool> _cookiesValidator;

		private bool _emitOnPing;

		private string _id;

		private bool _ignoreExtensions;

		private Func<string, bool> _originValidator;

		private string _protocol;

		private WebSocketSessionManager _sessions;

		private DateTime _startTime;

		private WebSocket _websocket;

		public WebSocketContext Context
		{
			get
			{
				return this._context;
			}
		}

		public Func<CookieCollection, CookieCollection, bool> CookiesValidator
		{
			get
			{
				return this._cookiesValidator;
			}
			set
			{
				this._cookiesValidator = value;
			}
		}

		public bool EmitOnPing
		{
			get
			{
				return (this._websocket != null ? this._websocket.EmitOnPing : this._emitOnPing);
			}
			set
			{
				if (this._websocket == null)
				{
					this._emitOnPing = value;
				}
				else
				{
					this._websocket.EmitOnPing = value;
				}
			}
		}

		public string ID
		{
			get
			{
				return this._id;
			}
		}

		public bool IgnoreExtensions
		{
			get
			{
				return this._ignoreExtensions;
			}
			set
			{
				this._ignoreExtensions = value;
			}
		}

		protected Logger Log
		{
			get
			{
				Logger log;
				if (this._websocket != null)
				{
					log = this._websocket.Log;
				}
				else
				{
					log = null;
				}
				return log;
			}
		}

		public Func<string, bool> OriginValidator
		{
			get
			{
				return this._originValidator;
			}
			set
			{
				this._originValidator = value;
			}
		}

		public string Protocol
		{
			get
			{
				return JustDecompileGenerated_get_Protocol();
			}
			set
			{
				JustDecompileGenerated_set_Protocol(value);
			}
		}

		public string JustDecompileGenerated_get_Protocol()
		{
			return (this._websocket != null ? this._websocket.Protocol : this._protocol ?? string.Empty);
		}

		public void JustDecompileGenerated_set_Protocol(string value)
		{
			bool flag;
			if (this.State == WebSocketState.Connecting)
			{
				if (value == null)
				{
					flag = false;
				}
				else
				{
					flag = (value.Length == 0 ? true : !value.IsToken());
				}
				if (!flag)
				{
					this._protocol = value;
				}
			}
		}

		protected WebSocketSessionManager Sessions
		{
			get
			{
				return this._sessions;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return this._startTime;
			}
		}

		public WebSocketState State
		{
			get
			{
				return (this._websocket != null ? this._websocket.ReadyState : WebSocketState.Connecting);
			}
		}

		protected WebSocketBehavior()
		{
			this._startTime = DateTime.MaxValue;
		}

		private string checkHandshakeRequest(WebSocketContext context)
		{
			string str;
			if (this._originValidator != null && !this._originValidator(context.Origin))
			{
				str = "Includes no Origin header, or it has an invalid value.";
			}
			else if (this._cookiesValidator == null || this._cookiesValidator(context.CookieCollection, context.WebSocket.CookieCollection))
			{
				str = null;
			}
			else
			{
				str = "Includes no cookie, or an invalid cookie exists.";
			}
			return str;
		}

		protected void Error(string message, Exception exception)
		{
			if ((message == null ? false : message.Length > 0))
			{
				this.OnError(new WebSocketSharp.ErrorEventArgs(message, exception));
			}
		}

		private void onClose(object sender, CloseEventArgs e)
		{
			if (this._id != null)
			{
				this._sessions.Remove(this._id);
				this.OnClose(e);
			}
		}

		protected virtual void OnClose(CloseEventArgs e)
		{
		}

		private void onError(object sender, WebSocketSharp.ErrorEventArgs e)
		{
			this.OnError(e);
		}

		protected virtual void OnError(WebSocketSharp.ErrorEventArgs e)
		{
		}

		private void onMessage(object sender, MessageEventArgs e)
		{
			this.OnMessage(e);
		}

		protected virtual void OnMessage(MessageEventArgs e)
		{
		}

		private void onOpen(object sender, EventArgs e)
		{
			this._id = this._sessions.Add(this);
			if (this._id != null)
			{
				this._startTime = DateTime.Now;
				this.OnOpen();
			}
			else
			{
				this._websocket.Close(CloseStatusCode.Away);
			}
		}

		protected virtual void OnOpen()
		{
		}

		protected void Send(byte[] data)
		{
			if (this._websocket != null)
			{
				this._websocket.Send(data);
			}
		}

		protected void Send(FileInfo file)
		{
			if (this._websocket != null)
			{
				this._websocket.Send(file);
			}
		}

		protected void Send(string data)
		{
			if (this._websocket != null)
			{
				this._websocket.Send(data);
			}
		}

		protected void SendAsync(byte[] data, Action<bool> completed)
		{
			if (this._websocket != null)
			{
				this._websocket.SendAsync(data, completed);
			}
		}

		protected void SendAsync(FileInfo file, Action<bool> completed)
		{
			if (this._websocket != null)
			{
				this._websocket.SendAsync(file, completed);
			}
		}

		protected void SendAsync(string data, Action<bool> completed)
		{
			if (this._websocket != null)
			{
				this._websocket.SendAsync(data, completed);
			}
		}

		protected void SendAsync(Stream stream, int length, Action<bool> completed)
		{
			if (this._websocket != null)
			{
				this._websocket.SendAsync(stream, length, completed);
			}
		}

		internal void Start(WebSocketContext context, WebSocketSessionManager sessions)
		{
			if (this._websocket == null)
			{
				this._context = context;
				this._sessions = sessions;
				this._websocket = context.WebSocket;
				this._websocket.CustomHandshakeRequestChecker = new Func<WebSocketContext, string>(this.checkHandshakeRequest);
				this._websocket.EmitOnPing = this._emitOnPing;
				this._websocket.IgnoreExtensions = this._ignoreExtensions;
				this._websocket.Protocol = this._protocol;
				TimeSpan waitTime = sessions.WaitTime;
				if (waitTime != this._websocket.WaitTime)
				{
					this._websocket.WaitTime = waitTime;
				}
				this._websocket.OnOpen += new EventHandler(this.onOpen);
				this._websocket.OnMessage += new EventHandler<MessageEventArgs>(this.onMessage);
				this._websocket.OnError += new EventHandler<WebSocketSharp.ErrorEventArgs>(this.onError);
				this._websocket.OnClose += new EventHandler<CloseEventArgs>(this.onClose);
				this._websocket.InternalAccept();
			}
			else
			{
				this._websocket.Log.Error("This session has already been started.");
				context.WebSocket.Close(HttpStatusCode.ServiceUnavailable);
			}
		}
	}
}