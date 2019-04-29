using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp
{
	public class WebSocket : IDisposable
	{
		private AuthenticationChallenge _authChallenge;

		private string _base64Key;

		private bool _client;

		private Action _closeContext;

		private CompressionMethod _compression;

		private WebSocketContext _context;

		private WebSocketSharp.Net.CookieCollection _cookies;

		private NetworkCredential _credentials;

		private bool _emitOnPing;

		private bool _enableRedirection;

		private AutoResetEvent _exitReceiving;

		private string _extensions;

		private bool _extensionsRequested;

		private object _forMessageEventQueue;

		private object _forSend;

		private object _forState;

		private MemoryStream _fragmentsBuffer;

		private bool _fragmentsCompressed;

		private Opcode _fragmentsOpcode;

		private const string _guid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

		private Func<WebSocketContext, string> _handshakeRequestChecker;

		private bool _ignoreExtensions;

		private bool _inContinuation;

		private volatile bool _inMessage;

		private volatile Logger _logger;

		private Action<MessageEventArgs> _message;

		private Queue<MessageEventArgs> _messageEventQueue;

		private uint _nonceCount;

		private string _origin;

		private bool _preAuth;

		private string _protocol;

		private string[] _protocols;

		private bool _protocolsRequested;

		private NetworkCredential _proxyCredentials;

		private Uri _proxyUri;

		private volatile WebSocketState _readyState;

		private AutoResetEvent _receivePong;

		private bool _secure;

		private ClientSslConfiguration _sslConfig;

		private Stream _stream;

		private TcpClient _tcpClient;

		private Uri _uri;

		private const string _version = "13";

		private TimeSpan _waitTime;

		internal readonly static byte[] EmptyBytes;

		internal readonly static int FragmentLength;

		internal readonly static RandomNumberGenerator RandomNumber;

		public CompressionMethod Compression
		{
			get
			{
				return this._compression;
			}
			set
			{
				string str;
				object obj = this._forState;
				Monitor.Enter(obj);
				try
				{
					if (this.checkIfAvailable(true, false, true, false, false, true, out str))
					{
						this._compression = value;
					}
					else
					{
						this._logger.Error(str);
						this.error("An error has occurred in setting the compression.", null);
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
		}

		internal WebSocketSharp.Net.CookieCollection CookieCollection
		{
			get
			{
				return this._cookies;
			}
		}

		public IEnumerable<Cookie> Cookies
		{
			get
			{
				object syncRoot = this._cookies.SyncRoot;
				Monitor.Enter(syncRoot);
				try
				{
					foreach (Cookie _cooky in this._cookies)
					{
						yield return _cooky;
					}
				}
				finally
				{
					Monitor.Exit(syncRoot);
				}
				syncRoot = null;
			}
		}

		public NetworkCredential Credentials
		{
			get
			{
				return this._credentials;
			}
		}

		internal Func<WebSocketContext, string> CustomHandshakeRequestChecker
		{
			get
			{
				return this._handshakeRequestChecker;
			}
			set
			{
				this._handshakeRequestChecker = value;
			}
		}

		public bool EmitOnPing
		{
			get
			{
				return this._emitOnPing;
			}
			set
			{
				this._emitOnPing = value;
			}
		}

		public bool EnableRedirection
		{
			get
			{
				return this._enableRedirection;
			}
			set
			{
				string str;
				object obj = this._forState;
				Monitor.Enter(obj);
				try
				{
					if (this.checkIfAvailable(true, false, true, false, false, true, out str))
					{
						this._enableRedirection = value;
					}
					else
					{
						this._logger.Error(str);
						this.error("An error has occurred in setting the enable redirection.", null);
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
		}

		public string Extensions
		{
			get
			{
				return this._extensions ?? string.Empty;
			}
		}

		internal bool HasMessage
		{
			get
			{
				bool count;
				object obj = this._forMessageEventQueue;
				Monitor.Enter(obj);
				try
				{
					count = this._messageEventQueue.Count > 0;
				}
				finally
				{
					Monitor.Exit(obj);
				}
				return count;
			}
		}

		internal bool IgnoreExtensions
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

		public bool IsAlive
		{
			get
			{
				return this.Ping();
			}
		}

		internal bool IsConnected
		{
			get
			{
				return (this._readyState == WebSocketState.Open ? true : this._readyState == WebSocketState.Closing);
			}
		}

		public bool IsSecure
		{
			get
			{
				return this._secure;
			}
		}

		public Logger Log
		{
			get
			{
				return this._logger;
			}
			internal set
			{
				this._logger = value;
			}
		}

		public string Origin
		{
			get
			{
				return this._origin;
			}
			set
			{
				string str;
				Uri uri;
				object obj = this._forState;
				Monitor.Enter(obj);
				try
				{
					if (!this.checkIfAvailable(true, false, true, false, false, true, out str))
					{
						this._logger.Error(str);
						this.error("An error has occurred in setting the origin.", null);
					}
					else if (value.IsNullOrEmpty())
					{
						this._origin = value;
					}
					else if ((!Uri.TryCreate(value, UriKind.Absolute, out uri) ? false : (int)uri.Segments.Length <= 1))
					{
						this._origin = value.TrimEnd(new char[] { '/' });
					}
					else
					{
						this._logger.Error("The syntax of an origin must be '<scheme>://<host>[:<port>]'.");
						this.error("An error has occurred in setting the origin.", null);
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
		}

		public string Protocol
		{
			get
			{
				return this._protocol ?? string.Empty;
			}
			internal set
			{
				this._protocol = value;
			}
		}

		public WebSocketState ReadyState
		{
			get
			{
				return this._readyState;
			}
		}

		public ClientSslConfiguration SslConfiguration
		{
			get
			{
				object obj;
				if (this._client)
				{
					obj = this._sslConfig;
					if (obj == null)
					{
						ClientSslConfiguration clientSslConfiguration = new ClientSslConfiguration(this._uri.DnsSafeHost);
						ClientSslConfiguration clientSslConfiguration1 = clientSslConfiguration;
						this._sslConfig = clientSslConfiguration;
						obj = clientSslConfiguration1;
					}
				}
				else
				{
					obj = null;
				}
				return (ClientSslConfiguration)obj;
			}
			set
			{
				string str;
				object obj = this._forState;
				Monitor.Enter(obj);
				try
				{
					if (this.checkIfAvailable(true, false, true, false, false, true, out str))
					{
						this._sslConfig = value;
					}
					else
					{
						this._logger.Error(str);
						this.error("An error has occurred in setting the ssl configuration.", null);
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
		}

		public Uri Url
		{
			get
			{
				return (this._client ? this._uri : this._context.RequestUri);
			}
		}

		public TimeSpan WaitTime
		{
			get
			{
				return this._waitTime;
			}
			set
			{
				string str;
				object obj = this._forState;
				Monitor.Enter(obj);
				try
				{
					if ((!this.checkIfAvailable(true, true, true, false, false, true, out str) ? false : value.CheckWaitTime(out str)))
					{
						this._waitTime = value;
					}
					else
					{
						this._logger.Error(str);
						this.error("An error has occurred in setting the wait time.", null);
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
		}

		static WebSocket()
		{
			WebSocket.EmptyBytes = new byte[0];
			WebSocket.FragmentLength = 1016;
			WebSocket.RandomNumber = new RNGCryptoServiceProvider();
		}

		internal WebSocket(HttpListenerWebSocketContext context, string protocol)
		{
			this._context = context;
			this._protocol = protocol;
			this._closeContext = new Action(context.Close);
			this._logger = context.Log;
			this._message = new Action<MessageEventArgs>(this.messages);
			this._secure = context.IsSecureConnection;
			this._stream = context.Stream;
			this._waitTime = TimeSpan.FromSeconds(1);
			this.init();
		}

		internal WebSocket(TcpListenerWebSocketContext context, string protocol)
		{
			this._context = context;
			this._protocol = protocol;
			this._closeContext = new Action(context.Close);
			this._logger = context.Log;
			this._message = new Action<MessageEventArgs>(this.messages);
			this._secure = context.IsSecureConnection;
			this._stream = context.Stream;
			this._waitTime = TimeSpan.FromSeconds(1);
			this.init();
		}

		public WebSocket(string url, params string[] protocols)
		{
			string str;
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			if (url.Length == 0)
			{
				throw new ArgumentException("An empty string.", "url");
			}
			if (!url.TryCreateWebSocketUri(out this._uri, out str))
			{
				throw new ArgumentException(str, "url");
			}
			if ((protocols == null ? false : protocols.Length != 0))
			{
				str = protocols.CheckIfValidProtocols();
				if (str != null)
				{
					throw new ArgumentException(str, "protocols");
				}
				this._protocols = protocols;
			}
			this._base64Key = WebSocket.CreateBase64Key();
			this._client = true;
			this._logger = new Logger();
			this._message = new Action<MessageEventArgs>(this.messagec);
			this._secure = this._uri.Scheme == "wss";
			this._waitTime = TimeSpan.FromSeconds(5);
			this.init();
		}

		private bool accept()
		{
			string str;
			bool flag;
			object obj = this._forState;
			Monitor.Enter(obj);
			try
			{
				if (this.checkIfAvailable(true, false, false, false, out str))
				{
					try
					{
						if (this.acceptHandshake())
						{
							this._readyState = WebSocketState.Open;
						}
						else
						{
							flag = false;
							return flag;
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						this._logger.Fatal(exception.ToString());
						this.fatal("An exception has occurred while accepting.", exception);
						flag = false;
						return flag;
					}
					flag = true;
				}
				else
				{
					this._logger.Error(str);
					this.error("An error has occurred in accepting.", null);
					flag = false;
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return flag;
		}

		public void Accept()
		{
			string str;
			if (!this.checkIfAvailable(false, true, true, false, false, false, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in accepting.", null);
			}
			else if (this.accept())
			{
				this.open();
			}
		}

		public void AcceptAsync()
		{
			string str;
			if (this.checkIfAvailable(false, true, true, false, false, false, out str))
			{
				Func<bool> func = new Func<bool>(this.accept);
				func.BeginInvoke((IAsyncResult ar) => {
					if (func.EndInvoke(ar))
					{
						this.open();
					}
				}, null);
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in accepting.", null);
			}
		}

		private bool acceptHandshake()
		{
			string str;
			bool flag;
			this._logger.Debug(string.Format("A request from {0}:\n{1}", this._context.UserEndPoint, this._context));
			if (!this.checkHandshakeRequest(this._context, out str))
			{
				this.sendHttpResponse(this.createHandshakeFailureResponse(HttpStatusCode.BadRequest));
				this._logger.Fatal(str);
				this.fatal("An error has occurred while accepting.", CloseStatusCode.ProtocolError);
				flag = false;
			}
			else if (this.customCheckHandshakeRequest(this._context, out str))
			{
				this._base64Key = this._context.Headers["Sec-WebSocket-Key"];
				if (this._protocol != null)
				{
					this.processSecWebSocketProtocolHeader(this._context.SecWebSocketProtocols);
				}
				if (!this._ignoreExtensions)
				{
					this.processSecWebSocketExtensionsClientHeader(this._context.Headers["Sec-WebSocket-Extensions"]);
				}
				flag = this.sendHttpResponse(this.createHandshakeResponse());
			}
			else
			{
				this.sendHttpResponse(this.createHandshakeFailureResponse(HttpStatusCode.BadRequest));
				this._logger.Fatal(str);
				this.fatal("An error has occurred while accepting.", CloseStatusCode.PolicyViolation);
				flag = false;
			}
			return flag;
		}

		private bool checkHandshakeRequest(WebSocketContext context, out string message)
		{
			bool flag;
			message = null;
			if (context.RequestUri == null)
			{
				message = "Specifies an invalid Request-URI.";
				flag = false;
			}
			else if (context.IsWebSocketRequest)
			{
				NameValueCollection headers = context.Headers;
				if (!this.validateSecWebSocketKeyHeader(headers["Sec-WebSocket-Key"]))
				{
					message = "Includes no Sec-WebSocket-Key header, or it has an invalid value.";
					flag = false;
				}
				else if (!this.validateSecWebSocketVersionClientHeader(headers["Sec-WebSocket-Version"]))
				{
					message = "Includes no Sec-WebSocket-Version header, or it has an invalid value.";
					flag = false;
				}
				else if (!this.validateSecWebSocketProtocolClientHeader(headers["Sec-WebSocket-Protocol"]))
				{
					message = "Includes an invalid Sec-WebSocket-Protocol header.";
					flag = false;
				}
				else if ((this._ignoreExtensions ? true : this.validateSecWebSocketExtensionsClientHeader(headers["Sec-WebSocket-Extensions"])))
				{
					flag = true;
				}
				else
				{
					message = "Includes an invalid Sec-WebSocket-Extensions header.";
					flag = false;
				}
			}
			else
			{
				message = "Not a WebSocket handshake request.";
				flag = false;
			}
			return flag;
		}

		private bool checkHandshakeResponse(HttpResponse response, out string message)
		{
			bool flag;
			message = null;
			if (response.IsRedirect)
			{
				message = "Indicates the redirection.";
				flag = false;
			}
			else if (response.IsUnauthorized)
			{
				message = "Requires the authentication.";
				flag = false;
			}
			else if (response.IsWebSocketResponse)
			{
				NameValueCollection headers = response.Headers;
				if (!this.validateSecWebSocketAcceptHeader(headers["Sec-WebSocket-Accept"]))
				{
					message = "Includes no Sec-WebSocket-Accept header, or it has an invalid value.";
					flag = false;
				}
				else if (!this.validateSecWebSocketProtocolServerHeader(headers["Sec-WebSocket-Protocol"]))
				{
					message = "Includes no Sec-WebSocket-Protocol header, or it has an invalid value.";
					flag = false;
				}
				else if (!this.validateSecWebSocketExtensionsServerHeader(headers["Sec-WebSocket-Extensions"]))
				{
					message = "Includes an invalid Sec-WebSocket-Extensions header.";
					flag = false;
				}
				else if (this.validateSecWebSocketVersionServerHeader(headers["Sec-WebSocket-Version"]))
				{
					flag = true;
				}
				else
				{
					message = "Includes an invalid Sec-WebSocket-Version header.";
					flag = false;
				}
			}
			else
			{
				message = "Not a WebSocket handshake response.";
				flag = false;
			}
			return flag;
		}

		private bool checkIfAvailable(bool connecting, bool open, bool closing, bool closed, out string message)
		{
			bool flag;
			message = null;
			if ((connecting ? false : this._readyState == WebSocketState.Connecting))
			{
				message = "This operation is not available in: connecting";
				flag = false;
			}
			else if ((open ? false : this._readyState == WebSocketState.Open))
			{
				message = "This operation is not available in: open";
				flag = false;
			}
			else if ((closing ? false : this._readyState == WebSocketState.Closing))
			{
				message = "This operation is not available in: closing";
				flag = false;
			}
			else if ((closed ? true : this._readyState != WebSocketState.Closed))
			{
				flag = true;
			}
			else
			{
				message = "This operation is not available in: closed";
				flag = false;
			}
			return flag;
		}

		private bool checkIfAvailable(bool client, bool server, bool connecting, bool open, bool closing, bool closed, out string message)
		{
			bool flag;
			message = null;
			if ((client ? false : this._client))
			{
				message = "This operation is not available in: client";
				flag = false;
			}
			else if ((server ? true : this._client))
			{
				flag = this.checkIfAvailable(connecting, open, closing, closed, out message);
			}
			else
			{
				message = "This operation is not available in: server";
				flag = false;
			}
			return flag;
		}

		internal static bool CheckParametersForClose(ushort code, string reason, bool client, out string message)
		{
			bool flag;
			message = null;
			if (!code.IsCloseStatusCode())
			{
				message = "'code' is an invalid status code.";
				flag = false;
			}
			else if ((code != 1005 ? false : !reason.IsNullOrEmpty()))
			{
				message = "'code' cannot have a reason.";
				flag = false;
			}
			else if ((code != 1010 ? false : !client))
			{
				message = "'code' cannot be used by a server.";
				flag = false;
			}
			else if (code == 1011 & client)
			{
				message = "'code' cannot be used by a client.";
				flag = false;
			}
			else if ((reason.IsNullOrEmpty() ? true : (int)reason.UTF8Encode().Length <= 123))
			{
				flag = true;
			}
			else
			{
				message = "The size of 'reason' is greater than the allowable max size.";
				flag = false;
			}
			return flag;
		}

		internal static bool CheckParametersForClose(CloseStatusCode code, string reason, bool client, out string message)
		{
			bool flag;
			message = null;
			if ((code != CloseStatusCode.NoStatus ? false : !reason.IsNullOrEmpty()))
			{
				message = "'code' cannot have a reason.";
				flag = false;
			}
			else if ((code != CloseStatusCode.MandatoryExtension ? false : !client))
			{
				message = "'code' cannot be used by a server.";
				flag = false;
			}
			else if (code == CloseStatusCode.ServerError & client)
			{
				message = "'code' cannot be used by a client.";
				flag = false;
			}
			else if ((reason.IsNullOrEmpty() ? true : (int)reason.UTF8Encode().Length <= 123))
			{
				flag = true;
			}
			else
			{
				message = "The size of 'reason' is greater than the allowable max size.";
				flag = false;
			}
			return flag;
		}

		private static bool checkParametersForSetCredentials(string username, string password, out string message)
		{
			bool flag;
			message = null;
			if (username.IsNullOrEmpty())
			{
				flag = true;
			}
			else if ((username.Contains(new char[] { ':' }) ? true : !username.IsText()))
			{
				message = "'username' contains an invalid character.";
				flag = false;
			}
			else if (password.IsNullOrEmpty())
			{
				flag = true;
			}
			else if (password.IsText())
			{
				flag = true;
			}
			else
			{
				message = "'password' contains an invalid character.";
				flag = false;
			}
			return flag;
		}

		private static bool checkParametersForSetProxy(string url, string username, string password, out string message)
		{
			Uri uri;
			bool flag;
			message = null;
			if (url.IsNullOrEmpty())
			{
				flag = true;
			}
			else if ((!Uri.TryCreate(url, UriKind.Absolute, out uri) || uri.Scheme != "http" ? true : (int)uri.Segments.Length > 1))
			{
				message = "'url' is an invalid URL.";
				flag = false;
			}
			else if (username.IsNullOrEmpty())
			{
				flag = true;
			}
			else if ((username.Contains(new char[] { ':' }) ? true : !username.IsText()))
			{
				message = "'username' contains an invalid character.";
				flag = false;
			}
			else if (password.IsNullOrEmpty())
			{
				flag = true;
			}
			else if (password.IsText())
			{
				flag = true;
			}
			else
			{
				message = "'password' contains an invalid character.";
				flag = false;
			}
			return flag;
		}

		internal static string CheckPingParameter(string message, out byte[] bytes)
		{
			string str;
			bytes = message.UTF8Encode();
			if ((int)bytes.Length > 125)
			{
				str = "A message has greater than the allowable max size.";
			}
			else
			{
				str = null;
			}
			return str;
		}

		private bool checkReceivedFrame(WebSocketFrame frame, out string message)
		{
			bool flag;
			message = null;
			bool isMasked = frame.IsMasked;
			if (this._client & isMasked)
			{
				message = "A frame from the server is masked.";
				flag = false;
			}
			else if ((this._client ? false : !isMasked))
			{
				message = "A frame from a client is not masked.";
				flag = false;
			}
			else if ((!this._inContinuation ? false : frame.IsData))
			{
				message = "A data frame has been received while receiving continuation frames.";
				flag = false;
			}
			else if ((!frame.IsCompressed ? false : this._compression == CompressionMethod.None))
			{
				message = "A compressed frame has been received without any agreement for it.";
				flag = false;
			}
			else if (frame.Rsv2 == Rsv.On)
			{
				message = "The RSV2 of a frame is non-zero without any negotiation for it.";
				flag = false;
			}
			else if (frame.Rsv3 != Rsv.On)
			{
				flag = true;
			}
			else
			{
				message = "The RSV3 of a frame is non-zero without any negotiation for it.";
				flag = false;
			}
			return flag;
		}

		internal static string CheckSendParameter(byte[] data)
		{
			string str;
			if (data == null)
			{
				str = "'data' is null.";
			}
			else
			{
				str = null;
			}
			return str;
		}

		internal static string CheckSendParameter(FileInfo file)
		{
			string str;
			if (file == null)
			{
				str = "'file' is null.";
			}
			else
			{
				str = null;
			}
			return str;
		}

		internal static string CheckSendParameter(string data)
		{
			string str;
			if (data == null)
			{
				str = "'data' is null.";
			}
			else
			{
				str = null;
			}
			return str;
		}

		internal static string CheckSendParameters(Stream stream, int length)
		{
			string str;
			if (stream == null)
			{
				str = "'stream' is null.";
			}
			else if (!stream.CanRead)
			{
				str = "'stream' cannot be read.";
			}
			else if (length < 1)
			{
				str = "'length' is less than 1.";
			}
			else
			{
				str = null;
			}
			return str;
		}

		private void close(ushort code, string reason)
		{
			if (code != 1005)
			{
				bool flag = !code.IsReserved();
				this.close(new CloseEventArgs(code, reason), flag, flag, false);
			}
			else
			{
				this.close(new CloseEventArgs(), true, true, false);
			}
		}

		private void close(CloseEventArgs e, bool send, bool receive, bool received)
		{
			byte[] array;
			object obj = this._forState;
			Monitor.Enter(obj);
			try
			{
				if (this._readyState == WebSocketState.Closing)
				{
					this._logger.Info("The closing is already in progress.");
					return;
				}
				else if (this._readyState != WebSocketState.Closed)
				{
					send = (!send ? false : this._readyState == WebSocketState.Open);
					receive &= send;
					this._readyState = WebSocketState.Closing;
				}
				else
				{
					this._logger.Info("The connection has been closed.");
					return;
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
			this._logger.Trace("Begin closing the connection.");
			if (send)
			{
				array = WebSocketFrame.CreateCloseFrame(e.PayloadData, this._client).ToArray();
			}
			else
			{
				array = null;
			}
			e.WasClean = this.closeHandshake(array, receive, received);
			this.releaseResources();
			this._logger.Trace("End closing the connection.");
			this._readyState = WebSocketState.Closed;
			try
			{
				this.OnClose.Emit<CloseEventArgs>(this, e);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this._logger.Error(exception.ToString());
				this.error("An exception has occurred during the OnClose event.", exception);
			}
		}

		internal void Close(HttpResponse response)
		{
			this._readyState = WebSocketState.Closing;
			this.sendHttpResponse(response);
			this.releaseServerResources();
			this._readyState = WebSocketState.Closed;
		}

		internal void Close(HttpStatusCode code)
		{
			this.Close(this.createHandshakeFailureResponse(code));
		}

		internal void Close(CloseEventArgs e, byte[] frameAsBytes, bool receive)
		{
			object obj = this._forState;
			Monitor.Enter(obj);
			try
			{
				if (this._readyState == WebSocketState.Closing)
				{
					this._logger.Info("The closing is already in progress.");
					return;
				}
				else if (this._readyState != WebSocketState.Closed)
				{
					this._readyState = WebSocketState.Closing;
				}
				else
				{
					this._logger.Info("The connection has been closed.");
					return;
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
			e.WasClean = this.closeHandshake(frameAsBytes, receive, false);
			this.releaseServerResources();
			this.releaseCommonResources();
			this._readyState = WebSocketState.Closed;
			try
			{
				this.OnClose.Emit<CloseEventArgs>(this, e);
			}
			catch (Exception exception)
			{
				this._logger.Error(exception.ToString());
			}
		}

		public void Close()
		{
			string str;
			if (this.checkIfAvailable(true, true, false, false, out str))
			{
				this.close(new CloseEventArgs(), true, true, false);
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
			}
		}

		public void Close(ushort code)
		{
			string str;
			if (!this.checkIfAvailable(true, true, false, false, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
			}
			else if (WebSocket.CheckParametersForClose(code, null, this._client, out str))
			{
				this.close(code, null);
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
			}
		}

		public void Close(CloseStatusCode code)
		{
			string str;
			if (!this.checkIfAvailable(true, true, false, false, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
			}
			else if (WebSocket.CheckParametersForClose(code, null, this._client, out str))
			{
				this.close((ushort)code, null);
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
			}
		}

		public void Close(ushort code, string reason)
		{
			string str;
			if (!this.checkIfAvailable(true, true, false, false, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
			}
			else if (WebSocket.CheckParametersForClose(code, reason, this._client, out str))
			{
				this.close(code, reason);
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
			}
		}

		public void Close(CloseStatusCode code, string reason)
		{
			string str;
			if (!this.checkIfAvailable(true, true, false, false, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
			}
			else if (WebSocket.CheckParametersForClose(code, reason, this._client, out str))
			{
				this.close((ushort)code, reason);
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
			}
		}

		private void closeAsync(ushort code, string reason)
		{
			if (code != 1005)
			{
				bool flag = !code.IsReserved();
				this.closeAsync(new CloseEventArgs(code, reason), flag, flag, false);
			}
			else
			{
				this.closeAsync(new CloseEventArgs(), true, true, false);
			}
		}

		private void closeAsync(CloseEventArgs e, bool send, bool receive, bool received)
		{
			Action<CloseEventArgs, bool, bool, bool> action = new Action<CloseEventArgs, bool, bool, bool>(this.close);
			action.BeginInvoke(e, send, receive, received, (IAsyncResult ar) => action.EndInvoke(ar), null);
		}

		public void CloseAsync()
		{
			string str;
			if (this.checkIfAvailable(true, true, false, false, out str))
			{
				this.closeAsync(new CloseEventArgs(), true, true, false);
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
			}
		}

		public void CloseAsync(ushort code)
		{
			string str;
			if (!this.checkIfAvailable(true, true, false, false, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
			}
			else if (WebSocket.CheckParametersForClose(code, null, this._client, out str))
			{
				this.closeAsync(code, null);
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
			}
		}

		public void CloseAsync(CloseStatusCode code)
		{
			string str;
			if (!this.checkIfAvailable(true, true, false, false, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
			}
			else if (WebSocket.CheckParametersForClose(code, null, this._client, out str))
			{
				this.closeAsync((ushort)code, null);
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
			}
		}

		public void CloseAsync(ushort code, string reason)
		{
			string str;
			if (!this.checkIfAvailable(true, true, false, false, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
			}
			else if (WebSocket.CheckParametersForClose(code, reason, this._client, out str))
			{
				this.closeAsync(code, reason);
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
			}
		}

		public void CloseAsync(CloseStatusCode code, string reason)
		{
			string str;
			if (!this.checkIfAvailable(true, true, false, false, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
			}
			else if (WebSocket.CheckParametersForClose(code, reason, this._client, out str))
			{
				this.closeAsync((ushort)code, reason);
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
			}
		}

		private bool closeHandshake(byte[] frameAsBytes, bool receive, bool received)
		{
			bool flag;
			bool flag1 = (frameAsBytes == null ? false : this.sendBytes(frameAsBytes));
			if (received)
			{
				flag = true;
			}
			else
			{
				flag = (!(receive & flag1) || this._exitReceiving == null ? false : this._exitReceiving.WaitOne(this._waitTime));
			}
			received = flag;
			bool flag2 = flag1 & received;
			this._logger.Debug(string.Format("Was clean?: {0}\n  sent: {1}\n  received: {2}", flag2, flag1, received));
			return flag2;
		}

		private bool connect()
		{
			string str;
			bool flag;
			object obj = this._forState;
			Monitor.Enter(obj);
			try
			{
				if (this.checkIfAvailable(true, false, false, true, out str))
				{
					try
					{
						this._readyState = WebSocketState.Connecting;
						if (this.doHandshake())
						{
							this._readyState = WebSocketState.Open;
						}
						else
						{
							flag = false;
							return flag;
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						this._logger.Fatal(exception.ToString());
						this.fatal("An exception has occurred while connecting.", exception);
						flag = false;
						return flag;
					}
					flag = true;
				}
				else
				{
					this._logger.Error(str);
					this.error("An error has occurred in connecting.", null);
					flag = false;
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return flag;
		}

		public void Connect()
		{
			string str;
			if (!this.checkIfAvailable(true, false, true, false, false, true, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in connecting.", null);
			}
			else if (this.connect())
			{
				this.open();
			}
		}

		public void ConnectAsync()
		{
			string str;
			if (this.checkIfAvailable(true, false, true, false, false, true, out str))
			{
				Func<bool> func = new Func<bool>(this.connect);
				func.BeginInvoke((IAsyncResult ar) => {
					if (func.EndInvoke(ar))
					{
						this.open();
					}
				}, null);
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in connecting.", null);
			}
		}

		internal static string CreateBase64Key()
		{
			byte[] numArray = new byte[16];
			WebSocket.RandomNumber.GetBytes(numArray);
			return Convert.ToBase64String(numArray);
		}

		private string createExtensions()
		{
			string str;
			StringBuilder stringBuilder = new StringBuilder(80);
			if (this._compression != CompressionMethod.None)
			{
				string extensionString = this._compression.ToExtensionString(new string[] { "server_no_context_takeover", "client_no_context_takeover" });
				stringBuilder.AppendFormat("{0}, ", extensionString);
			}
			int length = stringBuilder.Length;
			if (length <= 2)
			{
				str = null;
			}
			else
			{
				stringBuilder.Length = length - 2;
				str = stringBuilder.ToString();
			}
			return str;
		}

		private HttpResponse createHandshakeFailureResponse(HttpStatusCode code)
		{
			HttpResponse httpResponse = HttpResponse.CreateCloseResponse(code);
			httpResponse.Headers["Sec-WebSocket-Version"] = "13";
			return httpResponse;
		}

		private HttpRequest createHandshakeRequest()
		{
			HttpRequest httpRequest = HttpRequest.CreateWebSocketRequest(this._uri);
			NameValueCollection headers = httpRequest.Headers;
			if (!this._origin.IsNullOrEmpty())
			{
				headers["Origin"] = this._origin;
			}
			headers["Sec-WebSocket-Key"] = this._base64Key;
			this._protocolsRequested = this._protocols != null;
			if (this._protocolsRequested)
			{
				headers["Sec-WebSocket-Protocol"] = this._protocols.ToString<string>(", ");
			}
			this._extensionsRequested = this._compression != CompressionMethod.None;
			if (this._extensionsRequested)
			{
				headers["Sec-WebSocket-Extensions"] = this.createExtensions();
			}
			headers["Sec-WebSocket-Version"] = "13";
			AuthenticationResponse authenticationResponse = null;
			if ((this._authChallenge == null ? false : this._credentials != null))
			{
				authenticationResponse = new AuthenticationResponse(this._authChallenge, this._credentials, this._nonceCount);
				this._nonceCount = authenticationResponse.NonceCount;
			}
			else if (this._preAuth)
			{
				authenticationResponse = new AuthenticationResponse(this._credentials);
			}
			if (authenticationResponse != null)
			{
				headers["Authorization"] = authenticationResponse.ToString();
			}
			if (this._cookies.Count > 0)
			{
				httpRequest.SetCookies(this._cookies);
			}
			return httpRequest;
		}

		private HttpResponse createHandshakeResponse()
		{
			HttpResponse httpResponse = HttpResponse.CreateWebSocketResponse();
			NameValueCollection headers = httpResponse.Headers;
			headers["Sec-WebSocket-Accept"] = WebSocket.CreateResponseKey(this._base64Key);
			if (this._protocol != null)
			{
				headers["Sec-WebSocket-Protocol"] = this._protocol;
			}
			if (this._extensions != null)
			{
				headers["Sec-WebSocket-Extensions"] = this._extensions;
			}
			if (this._cookies.Count > 0)
			{
				httpResponse.SetCookies(this._cookies);
			}
			return httpResponse;
		}

		internal static string CreateResponseKey(string base64Key)
		{
			StringBuilder stringBuilder = new StringBuilder(base64Key, 64);
			stringBuilder.Append("258EAFA5-E914-47DA-95CA-C5AB0DC85B11");
			SHA1 sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();
			byte[] numArray = sHA1CryptoServiceProvider.ComputeHash(stringBuilder.ToString().UTF8Encode());
			return Convert.ToBase64String(numArray);
		}

		private bool customCheckHandshakeRequest(WebSocketContext context, out string message)
		{
			bool flag;
			message = null;
			if (this._handshakeRequestChecker == null)
			{
				flag = true;
			}
			else
			{
				string str = this._handshakeRequestChecker(context);
				string str1 = str;
				message = str;
				flag = str1 == null;
			}
			return flag;
		}

		private MessageEventArgs dequeueFromMessageEventQueue()
		{
			MessageEventArgs messageEventArg;
			MessageEventArgs messageEventArg1;
			object obj = this._forMessageEventQueue;
			Monitor.Enter(obj);
			try
			{
				if (this._messageEventQueue.Count > 0)
				{
					messageEventArg1 = this._messageEventQueue.Dequeue();
				}
				else
				{
					messageEventArg1 = null;
				}
				messageEventArg = messageEventArg1;
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return messageEventArg;
		}

		private bool doHandshake()
		{
			string str;
			bool flag;
			this.setClientStream();
			HttpResponse httpResponse = this.sendHandshakeRequest();
			if (this.checkHandshakeResponse(httpResponse, out str))
			{
				if (this._protocolsRequested)
				{
					this._protocol = httpResponse.Headers["Sec-WebSocket-Protocol"];
				}
				if (this._extensionsRequested)
				{
					this.processSecWebSocketExtensionsServerHeader(httpResponse.Headers["Sec-WebSocket-Extensions"]);
				}
				this.processCookies(httpResponse.Cookies);
				flag = true;
			}
			else
			{
				this._logger.Fatal(str);
				this.fatal("An error has occurred while connecting.", CloseStatusCode.ProtocolError);
				flag = false;
			}
			return flag;
		}

		private void enqueueToMessageEventQueue(MessageEventArgs e)
		{
			object obj = this._forMessageEventQueue;
			Monitor.Enter(obj);
			try
			{
				this._messageEventQueue.Enqueue(e);
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}

		private void error(string message, Exception exception)
		{
			try
			{
				this.OnError.Emit<WebSocketSharp.ErrorEventArgs>(this, new WebSocketSharp.ErrorEventArgs(message, exception));
			}
			catch (Exception exception1)
			{
				this._logger.Error(exception1.ToString());
			}
		}

		private void fatal(string message, Exception exception)
		{
			this.fatal(message, (exception is WebSocketException ? ((WebSocketException)exception).Code : CloseStatusCode.Abnormal));
		}

		private void fatal(string message, CloseStatusCode code)
		{
			this.close(new CloseEventArgs(code, message), !code.IsReserved(), false, false);
		}

		private void init()
		{
			this._compression = CompressionMethod.None;
			this._cookies = new WebSocketSharp.Net.CookieCollection();
			this._forSend = new object();
			this._forState = new object();
			this._messageEventQueue = new Queue<MessageEventArgs>();
			this._forMessageEventQueue = ((ICollection)this._messageEventQueue).SyncRoot;
			this._readyState = WebSocketState.Connecting;
		}

		internal void InternalAccept()
		{
			try
			{
				if (this.acceptHandshake())
				{
					this._readyState = WebSocketState.Open;
				}
				else
				{
					return;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this._logger.Fatal(exception.ToString());
				this.fatal("An exception has occurred while accepting.", exception);
				return;
			}
			this.open();
		}

		private void message()
		{
			MessageEventArgs messageEventArg = null;
			object obj = this._forMessageEventQueue;
			Monitor.Enter(obj);
			try
			{
				if ((this._inMessage || this._messageEventQueue.Count == 0 ? false : this._readyState == WebSocketState.Open))
				{
					this._inMessage = true;
					messageEventArg = this._messageEventQueue.Dequeue();
				}
				else
				{
					return;
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
			this._message(messageEventArg);
		}

		private void messagec(MessageEventArgs e)
		{
			// 
			// Current member / type: System.Void WebSocketSharp.WebSocket::messagec(WebSocketSharp.MessageEventArgs)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Oxide.References.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void messagec(WebSocketSharp.MessageEventArgs)
			// 
			// Object reference not set to an instance of an object.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:line 93
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:line 24
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 69
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 437
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 90
			//    at ..Visit(IEnumerable ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 383
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:line 24
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 69
			//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:line 19
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		private void messages(MessageEventArgs e)
		{
			// 
			// Current member / type: System.Void WebSocketSharp.WebSocket::messages(WebSocketSharp.MessageEventArgs)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Oxide.References.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void messages(WebSocketSharp.MessageEventArgs)
			// 
			// Object reference not set to an instance of an object.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:line 93
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:line 24
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 69
			//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:line 19
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		private void open()
		{
			this._inMessage = true;
			this.startReceiving();
			try
			{
				this.OnOpen.Emit(this, EventArgs.Empty);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this._logger.Error(exception.ToString());
				this.error("An exception has occurred during the OnOpen event.", exception);
			}
			MessageEventArgs messageEventArg = null;
			object obj = this._forMessageEventQueue;
			Monitor.Enter(obj);
			try
			{
				if ((this._messageEventQueue.Count == 0 ? false : this._readyState == WebSocketState.Open))
				{
					messageEventArg = this._messageEventQueue.Dequeue();
				}
				else
				{
					this._inMessage = false;
					return;
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
			this._message.BeginInvoke(messageEventArg, (IAsyncResult ar) => this._message.EndInvoke(ar), null);
		}

		internal bool Ping(byte[] frameAsBytes, TimeSpan timeout)
		{
			bool flag;
			if (this._readyState != WebSocketState.Open)
			{
				flag = false;
			}
			else if (this.send(frameAsBytes))
			{
				AutoResetEvent autoResetEvent = this._receivePong;
				flag = (autoResetEvent != null ? autoResetEvent.WaitOne(timeout) : false);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public bool Ping()
		{
			return this.Ping((this._client ? WebSocketFrame.CreatePingFrame(true).ToArray() : WebSocketFrame.EmptyPingBytes), this._waitTime);
		}

		public bool Ping(string message)
		{
			byte[] numArray;
			bool flag;
			if ((message == null ? false : message.Length != 0))
			{
				string str = WebSocket.CheckPingParameter(message, out numArray);
				if (str == null)
				{
					flag = this.Ping(WebSocketFrame.CreatePingFrame(numArray, this._client).ToArray(), this._waitTime);
				}
				else
				{
					this._logger.Error(str);
					this.error("An error has occurred in sending a ping.", null);
					flag = false;
				}
			}
			else
			{
				flag = this.Ping();
			}
			return flag;
		}

		private bool processCloseFrame(WebSocketFrame frame)
		{
			PayloadData payloadData = frame.PayloadData;
			this.close(new CloseEventArgs(payloadData), !payloadData.HasReservedCode, false, true);
			return false;
		}

		private void processCookies(WebSocketSharp.Net.CookieCollection cookies)
		{
			if (cookies.Count != 0)
			{
				this._cookies.SetOrRemove(cookies);
			}
		}

		private bool processDataFrame(WebSocketFrame frame)
		{
			this.enqueueToMessageEventQueue((frame.IsCompressed ? new MessageEventArgs(frame.Opcode, frame.PayloadData.ApplicationData.Decompress(this._compression)) : new MessageEventArgs(frame)));
			return true;
		}

		private bool processFragmentFrame(WebSocketFrame frame)
		{
			bool flag;
			byte[] numArray;
			if (!this._inContinuation)
			{
				if (frame.IsContinuation)
				{
					flag = true;
					return flag;
				}
				this._fragmentsOpcode = frame.Opcode;
				this._fragmentsCompressed = frame.IsCompressed;
				this._fragmentsBuffer = new MemoryStream();
				this._inContinuation = true;
			}
			this._fragmentsBuffer.WriteBytes(frame.PayloadData.ApplicationData, 1024);
			if (frame.IsFinal)
			{
				using (this._fragmentsBuffer)
				{
					numArray = (this._fragmentsCompressed ? this._fragmentsBuffer.DecompressToArray(this._compression) : this._fragmentsBuffer.ToArray());
					this.enqueueToMessageEventQueue(new MessageEventArgs(this._fragmentsOpcode, numArray));
				}
				this._fragmentsBuffer = null;
				this._inContinuation = false;
			}
			flag = true;
			return flag;
		}

		private bool processPingFrame(WebSocketFrame frame)
		{
			if (this.send((new WebSocketFrame(Opcode.Pong, frame.PayloadData, this._client)).ToArray()))
			{
				this._logger.Trace("Returned a pong.");
			}
			if (this._emitOnPing)
			{
				this.enqueueToMessageEventQueue(new MessageEventArgs(frame));
			}
			return true;
		}

		private bool processPongFrame(WebSocketFrame frame)
		{
			this._receivePong.Set();
			this._logger.Trace("Received a pong.");
			return true;
		}

		private bool processReceivedFrame(WebSocketFrame frame)
		{
			string str;
			bool flag;
			if (!this.checkReceivedFrame(frame, out str))
			{
				throw new WebSocketException(CloseStatusCode.ProtocolError, str);
			}
			frame.Unmask();
			if (frame.IsFragment)
			{
				flag = this.processFragmentFrame(frame);
			}
			else if (frame.IsData)
			{
				flag = this.processDataFrame(frame);
			}
			else if (frame.IsPing)
			{
				flag = this.processPingFrame(frame);
			}
			else if (frame.IsPong)
			{
				flag = this.processPongFrame(frame);
			}
			else
			{
				flag = (frame.IsClose ? this.processCloseFrame(frame) : this.processUnsupportedFrame(frame));
			}
			return flag;
		}

		private void processSecWebSocketExtensionsClientHeader(string value)
		{
			if (value != null)
			{
				StringBuilder stringBuilder = new StringBuilder(80);
				bool flag = false;
				foreach (string str in value.SplitHeaderValue(new char[] { ',' }))
				{
					string str1 = str.Trim();
					if ((flag ? false : str1.IsCompressionExtension(CompressionMethod.Deflate)))
					{
						this._compression = CompressionMethod.Deflate;
						stringBuilder.AppendFormat("{0}, ", this._compression.ToExtensionString(new string[] { "client_no_context_takeover", "server_no_context_takeover" }));
						flag = true;
					}
				}
				int length = stringBuilder.Length;
				if (length > 2)
				{
					stringBuilder.Length = length - 2;
					this._extensions = stringBuilder.ToString();
				}
			}
		}

		private void processSecWebSocketExtensionsServerHeader(string value)
		{
			if (value != null)
			{
				this._extensions = value;
			}
			else
			{
				this._compression = CompressionMethod.None;
			}
		}

		private void processSecWebSocketProtocolHeader(IEnumerable<string> values)
		{
			if (!values.Contains<string>((string p) => p == this._protocol))
			{
				this._protocol = null;
			}
		}

		private bool processUnsupportedFrame(WebSocketFrame frame)
		{
			this._logger.Fatal(string.Concat("An unsupported frame:", frame.PrintToString(false)));
			this.fatal("There is no way to handle it.", CloseStatusCode.PolicyViolation);
			return false;
		}

		private void releaseClientResources()
		{
			if (this._stream != null)
			{
				this._stream.Dispose();
				this._stream = null;
			}
			if (this._tcpClient != null)
			{
				this._tcpClient.Close();
				this._tcpClient = null;
			}
		}

		private void releaseCommonResources()
		{
			if (this._fragmentsBuffer != null)
			{
				this._fragmentsBuffer.Dispose();
				this._fragmentsBuffer = null;
				this._inContinuation = false;
			}
			if (this._receivePong != null)
			{
				this._receivePong.Close();
				this._receivePong = null;
			}
			if (this._exitReceiving != null)
			{
				this._exitReceiving.Close();
				this._exitReceiving = null;
			}
		}

		private void releaseResources()
		{
			if (!this._client)
			{
				this.releaseServerResources();
			}
			else
			{
				this.releaseClientResources();
			}
			this.releaseCommonResources();
		}

		private void releaseServerResources()
		{
			if (this._closeContext != null)
			{
				this._closeContext();
				this._closeContext = null;
				this._stream = null;
				this._context = null;
			}
		}

		private bool send(byte[] frameAsBytes)
		{
			bool flag;
			object obj = this._forState;
			Monitor.Enter(obj);
			try
			{
				if (this._readyState == WebSocketState.Open)
				{
					flag = this.sendBytes(frameAsBytes);
				}
				else
				{
					this._logger.Error("The sending has been interrupted.");
					flag = false;
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return flag;
		}

		private bool send(Opcode opcode, Stream stream)
		{
			bool flag;
			object obj = this._forSend;
			Monitor.Enter(obj);
			try
			{
				Stream stream1 = stream;
				bool flag1 = false;
				bool flag2 = false;
				try
				{
					try
					{
						if (this._compression != CompressionMethod.None)
						{
							stream = stream.Compress(this._compression);
							flag1 = true;
						}
						flag2 = this.send(opcode, stream, flag1);
						if (!flag2)
						{
							this.error("The sending has been interrupted.", null);
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						this._logger.Error(exception.ToString());
						this.error("An exception has occurred while sending data.", exception);
					}
				}
				finally
				{
					if (flag1)
					{
						stream.Dispose();
					}
					stream1.Dispose();
				}
				flag = flag2;
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return flag;
		}

		private bool send(Opcode opcode, Stream stream, bool compressed)
		{
			bool flag;
			long length = stream.Length;
			if (length != (long)0)
			{
				long fragmentLength = length / (long)WebSocket.FragmentLength;
				int num = (int)(length % (long)WebSocket.FragmentLength);
				byte[] numArray = null;
				if (fragmentLength != (long)0)
				{
					numArray = new byte[WebSocket.FragmentLength];
					if ((fragmentLength != (long)1 ? false : num == 0))
					{
						flag = (stream.Read(numArray, 0, WebSocket.FragmentLength) != WebSocket.FragmentLength ? false : this.send(Fin.Final, opcode, numArray, compressed));
					}
					else if ((stream.Read(numArray, 0, WebSocket.FragmentLength) != WebSocket.FragmentLength ? false : this.send(Fin.More, opcode, numArray, compressed)))
					{
						long num1 = (num == 0 ? fragmentLength - (long)2 : fragmentLength - (long)1);
						long num2 = (long)0;
						while (num2 < num1)
						{
							if ((stream.Read(numArray, 0, WebSocket.FragmentLength) != WebSocket.FragmentLength ? false : this.send(Fin.More, Opcode.Cont, numArray, compressed)))
							{
								num2 += (long)1;
							}
							else
							{
								flag = false;
								return flag;
							}
						}
						if (num != 0)
						{
							numArray = new byte[num];
						}
						else
						{
							num = WebSocket.FragmentLength;
						}
						flag = (stream.Read(numArray, 0, num) != num ? false : this.send(Fin.Final, Opcode.Cont, numArray, compressed));
					}
					else
					{
						flag = false;
					}
				}
				else
				{
					numArray = new byte[num];
					flag = (stream.Read(numArray, 0, num) != num ? false : this.send(Fin.Final, opcode, numArray, compressed));
				}
			}
			else
			{
				flag = this.send(Fin.Final, opcode, WebSocket.EmptyBytes, compressed);
			}
			return flag;
		}

		private bool send(Fin fin, Opcode opcode, byte[] data, bool compressed)
		{
			bool flag;
			object obj = this._forState;
			Monitor.Enter(obj);
			try
			{
				if (this._readyState == WebSocketState.Open)
				{
					flag = this.sendBytes((new WebSocketFrame(fin, opcode, data, compressed, this._client)).ToArray());
				}
				else
				{
					this._logger.Error("The sending has been interrupted.");
					flag = false;
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return flag;
		}

		internal void Send(Opcode opcode, byte[] data, Dictionary<CompressionMethod, byte[]> cache)
		{
			byte[] array;
			object obj = this._forSend;
			Monitor.Enter(obj);
			try
			{
				object obj1 = this._forState;
				Monitor.Enter(obj1);
				try
				{
					if (this._readyState == WebSocketState.Open)
					{
						try
						{
							if (!cache.TryGetValue(this._compression, out array))
							{
								array = (new WebSocketFrame(Fin.Final, opcode, data.Compress(this._compression), this._compression != CompressionMethod.None, false)).ToArray();
								cache.Add(this._compression, array);
							}
							this.sendBytes(array);
						}
						catch (Exception exception)
						{
							this._logger.Error(exception.ToString());
						}
					}
					else
					{
						this._logger.Error("The sending has been interrupted.");
						return;
					}
				}
				finally
				{
					Monitor.Exit(obj1);
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}

		internal void Send(Opcode opcode, Stream stream, Dictionary<CompressionMethod, Stream> cache)
		{
			Stream stream1;
			object obj = this._forSend;
			Monitor.Enter(obj);
			try
			{
				try
				{
					if (cache.TryGetValue(this._compression, out stream1))
					{
						stream1.Position = (long)0;
					}
					else
					{
						stream1 = stream.Compress(this._compression);
						cache.Add(this._compression, stream1);
					}
					this.send(opcode, stream1, this._compression != CompressionMethod.None);
				}
				catch (Exception exception)
				{
					this._logger.Error(exception.ToString());
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}

		public void Send(byte[] data)
		{
			string str = ((WebSocketState)this._readyState).CheckIfAvailable(false, true, false, false) ?? WebSocket.CheckSendParameter(data);
			if (str == null)
			{
				this.send(Opcode.Binary, new MemoryStream(data));
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in sending data.", null);
			}
		}

		public void Send(FileInfo file)
		{
			string str = ((WebSocketState)this._readyState).CheckIfAvailable(false, true, false, false) ?? WebSocket.CheckSendParameter(file);
			if (str == null)
			{
				this.send(Opcode.Binary, file.OpenRead());
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in sending data.", null);
			}
		}

		public void Send(string data)
		{
			string str = ((WebSocketState)this._readyState).CheckIfAvailable(false, true, false, false) ?? WebSocket.CheckSendParameter(data);
			if (str == null)
			{
				this.send(Opcode.Text, new MemoryStream(data.UTF8Encode()));
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in sending data.", null);
			}
		}

		private void sendAsync(Opcode opcode, Stream stream, Action<bool> completed)
		{
			Func<Opcode, Stream, bool> func = new Func<Opcode, Stream, bool>(this.send);
			func.BeginInvoke(opcode, stream, (IAsyncResult ar) => {
				try
				{
					bool flag = func.EndInvoke(ar);
					if (completed != null)
					{
						completed(flag);
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					this._logger.Error(exception.ToString());
					this.error("An exception has occurred during a send callback.", exception);
				}
			}, null);
		}

		public void SendAsync(byte[] data, Action<bool> completed)
		{
			string str = ((WebSocketState)this._readyState).CheckIfAvailable(false, true, false, false) ?? WebSocket.CheckSendParameter(data);
			if (str == null)
			{
				this.sendAsync(Opcode.Binary, new MemoryStream(data), completed);
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in sending data.", null);
			}
		}

		public void SendAsync(FileInfo file, Action<bool> completed)
		{
			string str = ((WebSocketState)this._readyState).CheckIfAvailable(false, true, false, false) ?? WebSocket.CheckSendParameter(file);
			if (str == null)
			{
				this.sendAsync(Opcode.Binary, file.OpenRead(), completed);
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in sending data.", null);
			}
		}

		public void SendAsync(string data, Action<bool> completed)
		{
			string str = ((WebSocketState)this._readyState).CheckIfAvailable(false, true, false, false) ?? WebSocket.CheckSendParameter(data);
			if (str == null)
			{
				this.sendAsync(Opcode.Text, new MemoryStream(data.UTF8Encode()), completed);
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in sending data.", null);
			}
		}

		public void SendAsync(Stream stream, int length, Action<bool> completed)
		{
			string str = ((WebSocketState)this._readyState).CheckIfAvailable(false, true, false, false) ?? WebSocket.CheckSendParameters(stream, length);
			if (str == null)
			{
				stream.ReadBytesAsync(length, (byte[] data) => {
					int num = (int)data.Length;
					if (num != 0)
					{
						if (num < length)
						{
							this._logger.Warn(string.Format("The length of the data is less than 'length':\n  expected: {0}\n  actual: {1}", length, num));
						}
						bool flag = this.send(Opcode.Binary, new MemoryStream(data));
						if (completed != null)
						{
							completed(flag);
						}
					}
					else
					{
						this._logger.Error("The data cannot be read from 'stream'.");
						this.error("An error has occurred in sending data.", null);
					}
				}, (Exception ex) => {
					this._logger.Error(ex.ToString());
					this.error("An exception has occurred while sending data.", ex);
				});
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in sending data.", null);
			}
		}

		private bool sendBytes(byte[] bytes)
		{
			bool flag;
			try
			{
				this._stream.Write(bytes, 0, (int)bytes.Length);
				flag = true;
			}
			catch (Exception exception)
			{
				this._logger.Error(exception.ToString());
				flag = false;
			}
			return flag;
		}

		private HttpResponse sendHandshakeRequest()
		{
			HttpResponse httpResponse;
			string item;
			Uri uri;
			string str;
			bool flag;
			HttpRequest httpRequest = this.createHandshakeRequest();
			HttpResponse httpResponse1 = this.sendHttpRequest(httpRequest, 90000);
			if (httpResponse1.IsUnauthorized)
			{
				string item1 = httpResponse1.Headers["WWW-Authenticate"];
				this._logger.Warn(string.Format("Received an authentication requirement for '{0}'.", item1));
				if (!item1.IsNullOrEmpty())
				{
					this._authChallenge = AuthenticationChallenge.Parse(item1);
					if (this._authChallenge == null)
					{
						this._logger.Error("An invalid authentication challenge is specified.");
						httpResponse = httpResponse1;
						return httpResponse;
					}
					if (this._credentials == null)
					{
						flag = false;
					}
					else
					{
						flag = (!this._preAuth ? true : this._authChallenge.Scheme == AuthenticationSchemes.Digest);
					}
					if (flag)
					{
						if (httpResponse1.HasConnectionClose)
						{
							this.releaseClientResources();
							this.setClientStream();
						}
						AuthenticationResponse authenticationResponse = new AuthenticationResponse(this._authChallenge, this._credentials, this._nonceCount);
						this._nonceCount = authenticationResponse.NonceCount;
						httpRequest.Headers["Authorization"] = authenticationResponse.ToString();
						httpResponse1 = this.sendHttpRequest(httpRequest, 15000);
					}
				}
				else
				{
					this._logger.Error("No authentication challenge is specified.");
					httpResponse = httpResponse1;
					return httpResponse;
				}
			}
			if (httpResponse1.IsRedirect)
			{
				item = httpResponse1.Headers["Location"];
				this._logger.Warn(string.Format("Received a redirection to '{0}'.", item));
				if (this._enableRedirection)
				{
					goto Label2;
				}
			}
			httpResponse = httpResponse1;
			return httpResponse;
			if (item.IsNullOrEmpty())
			{
				this._logger.Error("No url to redirect is located.");
				httpResponse = httpResponse1;
				return httpResponse;
			}
			else if (item.TryCreateWebSocketUri(out uri, out str))
			{
				this.releaseClientResources();
				this._uri = uri;
				this._secure = uri.Scheme == "wss";
				this.setClientStream();
				httpResponse = this.sendHandshakeRequest();
				return httpResponse;
			}
			else
			{
				this._logger.Error(string.Concat("An invalid url to redirect is located: ", str));
				httpResponse = httpResponse1;
				return httpResponse;
			}
		}

		private HttpResponse sendHttpRequest(HttpRequest request, int millisecondsTimeout)
		{
			this._logger.Debug(string.Concat("A request to the server:\n", request.ToString()));
			HttpResponse response = request.GetResponse(this._stream, millisecondsTimeout);
			this._logger.Debug(string.Concat("A response to this request:\n", response.ToString()));
			return response;
		}

		private bool sendHttpResponse(HttpResponse response)
		{
			this._logger.Debug(string.Concat("A response to this request:\n", response.ToString()));
			return this.sendBytes(response.ToByteArray());
		}

		private void sendProxyConnectRequest()
		{
			HttpRequest str = HttpRequest.CreateConnectRequest(this._uri);
			HttpResponse httpResponse = this.sendHttpRequest(str, 90000);
			if (httpResponse.IsProxyAuthenticationRequired)
			{
				string item = httpResponse.Headers["Proxy-Authenticate"];
				this._logger.Warn(string.Format("Received a proxy authentication requirement for '{0}'.", item));
				if (item.IsNullOrEmpty())
				{
					throw new WebSocketException("No proxy authentication challenge is specified.");
				}
				AuthenticationChallenge authenticationChallenge = AuthenticationChallenge.Parse(item);
				if (authenticationChallenge == null)
				{
					throw new WebSocketException("An invalid proxy authentication challenge is specified.");
				}
				if (this._proxyCredentials != null)
				{
					if (httpResponse.HasConnectionClose)
					{
						this.releaseClientResources();
						this._tcpClient = new TcpClient(this._proxyUri.DnsSafeHost, this._proxyUri.Port);
						this._stream = this._tcpClient.GetStream();
					}
					AuthenticationResponse authenticationResponse = new AuthenticationResponse(authenticationChallenge, this._proxyCredentials, 0);
					str.Headers["Proxy-Authorization"] = authenticationResponse.ToString();
					httpResponse = this.sendHttpRequest(str, 15000);
				}
				if (httpResponse.IsProxyAuthenticationRequired)
				{
					throw new WebSocketException("A proxy authentication is required.");
				}
			}
			if (httpResponse.StatusCode[0] != '2')
			{
				throw new WebSocketException("The proxy has failed a connection to the requested host and port.");
			}
		}

		private void setClientStream()
		{
			if (this._proxyUri == null)
			{
				this._tcpClient = new TcpClient(this._uri.DnsSafeHost, this._uri.Port);
				this._stream = this._tcpClient.GetStream();
			}
			else
			{
				this._tcpClient = new TcpClient(this._proxyUri.DnsSafeHost, this._proxyUri.Port);
				this._stream = this._tcpClient.GetStream();
				this.sendProxyConnectRequest();
			}
			if (this._secure)
			{
				ClientSslConfiguration sslConfiguration = this.SslConfiguration;
				string targetHost = sslConfiguration.TargetHost;
				if (targetHost != this._uri.DnsSafeHost)
				{
					throw new WebSocketException(CloseStatusCode.TlsHandshakeFailure, "An invalid host name is specified.");
				}
				try
				{
					SslStream sslStream = new SslStream(this._stream, false, sslConfiguration.ServerCertificateValidationCallback, sslConfiguration.ClientCertificateSelectionCallback);
					sslStream.AuthenticateAsClient(targetHost, sslConfiguration.ClientCertificates, sslConfiguration.EnabledSslProtocols, sslConfiguration.CheckCertificateRevocation);
					this._stream = sslStream;
				}
				catch (Exception exception)
				{
					throw new WebSocketException(CloseStatusCode.TlsHandshakeFailure, exception);
				}
			}
		}

		public void SetCookie(Cookie cookie)
		{
			string str;
			if (!this.checkIfAvailable(true, false, true, false, false, true, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in setting a cookie.", null);
			}
			else if (cookie != null)
			{
				object obj = this._forState;
				Monitor.Enter(obj);
				try
				{
					if (this.checkIfAvailable(true, false, false, true, out str))
					{
						object syncRoot = this._cookies.SyncRoot;
						Monitor.Enter(syncRoot);
						try
						{
							this._cookies.SetOrRemove(cookie);
						}
						finally
						{
							Monitor.Exit(syncRoot);
						}
					}
					else
					{
						this._logger.Error(str);
						this.error("An error has occurred in setting a cookie.", null);
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
			else
			{
				this._logger.Error("'cookie' is null.");
				this.error("An error has occurred in setting a cookie.", null);
			}
		}

		public void SetCredentials(string username, string password, bool preAuth)
		{
			string str;
			if (!this.checkIfAvailable(true, false, true, false, false, true, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in setting the credentials.", null);
			}
			else if (WebSocket.checkParametersForSetCredentials(username, password, out str))
			{
				object obj = this._forState;
				Monitor.Enter(obj);
				try
				{
					if (!this.checkIfAvailable(true, false, false, true, out str))
					{
						this._logger.Error(str);
						this.error("An error has occurred in setting the credentials.", null);
					}
					else if (!username.IsNullOrEmpty())
					{
						this._credentials = new NetworkCredential(username, password, this._uri.PathAndQuery, new string[0]);
						this._preAuth = preAuth;
					}
					else
					{
						this._logger.Warn("The credentials are initialized.");
						this._credentials = null;
						this._preAuth = false;
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in setting the credentials.", null);
			}
		}

		public void SetProxy(string url, string username, string password)
		{
			string str;
			if (!this.checkIfAvailable(true, false, true, false, false, true, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in setting the proxy.", null);
			}
			else if (WebSocket.checkParametersForSetProxy(url, username, password, out str))
			{
				object obj = this._forState;
				Monitor.Enter(obj);
				try
				{
					if (!this.checkIfAvailable(true, false, false, true, out str))
					{
						this._logger.Error(str);
						this.error("An error has occurred in setting the proxy.", null);
					}
					else if (!url.IsNullOrEmpty())
					{
						this._proxyUri = new Uri(url);
						if (!username.IsNullOrEmpty())
						{
							this._proxyCredentials = new NetworkCredential(username, password, string.Format("{0}:{1}", this._uri.DnsSafeHost, this._uri.Port), new string[0]);
						}
						else
						{
							this._logger.Warn("The credentials for the proxy are initialized.");
							this._proxyCredentials = null;
						}
					}
					else
					{
						this._logger.Warn("The url and credentials for the proxy are initialized.");
						this._proxyUri = null;
						this._proxyCredentials = null;
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
			else
			{
				this._logger.Error(str);
				this.error("An error has occurred in setting the proxy.", null);
			}
		}

		private void startReceiving()
		{
			Action<WebSocketFrame> action2 = null;
			if (this._messageEventQueue.Count > 0)
			{
				this._messageEventQueue.Clear();
			}
			this._exitReceiving = new AutoResetEvent(false);
			this._receivePong = new AutoResetEvent(false);
			Action action3 = null;
			action3 = () => {
				Stream stream = this._stream;
				Action<WebSocketFrame> u003cu003e9_1 = action2;
				if (u003cu003e9_1 == null)
				{
					Action<WebSocketFrame> action = (WebSocketFrame frame) => {
						if ((!this.processReceivedFrame(frame) ? false : this._readyState != WebSocketState.Closed))
						{
							action3();
							if ((this._inMessage || !this.HasMessage ? false : this._readyState == WebSocketState.Open))
							{
								this.message();
							}
						}
						else
						{
							AutoResetEvent u003cu003e4_this = this._exitReceiving;
							if (u003cu003e4_this != null)
							{
								u003cu003e4_this.Set();
							}
						}
					};
					Action<WebSocketFrame> action1 = action;
					action2 = action;
					u003cu003e9_1 = action1;
				}
				WebSocketFrame.ReadFrameAsync(stream, false, u003cu003e9_1, (Exception ex) => {
					this._logger.Fatal(ex.ToString());
					this.fatal("An exception has occurred while receiving.", ex);
				});
			};
			action3();
		}

		void System.IDisposable.Dispose()
		{
			this.close(new CloseEventArgs(1001), true, true, false);
		}

		private bool validateSecWebSocketAcceptHeader(string value)
		{
			return (value == null ? false : value == WebSocket.CreateResponseKey(this._base64Key));
		}

		private bool validateSecWebSocketExtensionsClientHeader(string value)
		{
			return (value == null ? true : value.Length > 0);
		}

		private bool validateSecWebSocketExtensionsServerHeader(string value)
		{
			bool flag;
			if (value == null)
			{
				flag = true;
			}
			else if (value.Length == 0)
			{
				flag = false;
			}
			else if (this._extensionsRequested)
			{
				bool flag1 = this._compression != CompressionMethod.None;
				foreach (string str in value.SplitHeaderValue(new char[] { ',' }))
				{
					string str1 = str.Trim();
					if ((!flag1 ? true : !str1.IsCompressionExtension(this._compression)))
					{
						flag = false;
						return flag;
					}
					else if (str1.Contains("server_no_context_takeover"))
					{
						if (!str1.Contains("client_no_context_takeover"))
						{
							this._logger.Warn("The server hasn't sent back 'client_no_context_takeover'.");
						}
						string extensionString = this._compression.ToExtensionString(new string[0]);
						if (str1.SplitHeaderValue(new char[] { ';' }).Contains<string>((string t) => {
							t = t.Trim();
							return (!(t != extensionString) || !(t != "server_no_context_takeover") ? false : t != "client_no_context_takeover");
						}))
						{
							flag = false;
							return flag;
						}
					}
					else
					{
						this._logger.Error("The server hasn't sent back 'server_no_context_takeover'.");
						flag = false;
						return flag;
					}
				}
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		private bool validateSecWebSocketKeyHeader(string value)
		{
			return (value == null ? false : value.Length > 0);
		}

		private bool validateSecWebSocketProtocolClientHeader(string value)
		{
			return (value == null ? true : value.Length > 0);
		}

		private bool validateSecWebSocketProtocolServerHeader(string value)
		{
			bool flag;
			if (value == null)
			{
				flag = !this._protocolsRequested;
			}
			else if (value.Length != 0)
			{
				flag = (!this._protocolsRequested ? false : this._protocols.Contains<string>((string p) => p == value));
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		private bool validateSecWebSocketVersionClientHeader(string value)
		{
			return (value == null ? false : value == "13");
		}

		private bool validateSecWebSocketVersionServerHeader(string value)
		{
			return (value == null ? true : value == "13");
		}

		public event EventHandler<CloseEventArgs> OnClose;

		public event EventHandler<WebSocketSharp.ErrorEventArgs> OnError;

		public event EventHandler<MessageEventArgs> OnMessage;

		public event EventHandler OnOpen;
	}
}