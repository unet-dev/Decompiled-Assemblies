using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp.Server
{
	public class WebSocketServer
	{
		private IPAddress _address;

		private WebSocketSharp.Net.AuthenticationSchemes _authSchemes;

		private readonly static string _defaultRealm;

		private bool _dnsStyle;

		private string _hostname;

		private TcpListener _listener;

		private Logger _logger;

		private int _port;

		private string _realm;

		private Thread _receiveThread;

		private bool _reuseAddress;

		private bool _secure;

		private WebSocketServiceManager _services;

		private ServerSslConfiguration _sslConfig;

		private volatile ServerState _state;

		private object _sync;

		private Func<IIdentity, WebSocketSharp.Net.NetworkCredential> _userCredFinder;

		public IPAddress Address
		{
			get
			{
				return this._address;
			}
		}

		public WebSocketSharp.Net.AuthenticationSchemes AuthenticationSchemes
		{
			get
			{
				return this._authSchemes;
			}
			set
			{
				string str = ((ServerState)this._state).CheckIfAvailable(true, false, false);
				if (str == null)
				{
					this._authSchemes = value;
				}
				else
				{
					this._logger.Error(str);
				}
			}
		}

		public bool IsListening
		{
			get
			{
				return this._state == ServerState.Start;
			}
		}

		public bool IsSecure
		{
			get
			{
				return this._secure;
			}
		}

		public bool KeepClean
		{
			get
			{
				return this._services.KeepClean;
			}
			set
			{
				string str = ((ServerState)this._state).CheckIfAvailable(true, false, false);
				if (str == null)
				{
					this._services.KeepClean = value;
				}
				else
				{
					this._logger.Error(str);
				}
			}
		}

		public Logger Log
		{
			get
			{
				return this._logger;
			}
		}

		public int Port
		{
			get
			{
				return this._port;
			}
		}

		public string Realm
		{
			get
			{
				return this._realm;
			}
			set
			{
				string str = ((ServerState)this._state).CheckIfAvailable(true, false, false);
				if (str == null)
				{
					this._realm = value;
				}
				else
				{
					this._logger.Error(str);
				}
			}
		}

		public bool ReuseAddress
		{
			get
			{
				return this._reuseAddress;
			}
			set
			{
				string str = ((ServerState)this._state).CheckIfAvailable(true, false, false);
				if (str == null)
				{
					this._reuseAddress = value;
				}
				else
				{
					this._logger.Error(str);
				}
			}
		}

		public ServerSslConfiguration SslConfiguration
		{
			get
			{
				ServerSslConfiguration serverSslConfiguration = this._sslConfig;
				if (serverSslConfiguration == null)
				{
					ServerSslConfiguration serverSslConfiguration1 = new ServerSslConfiguration(null);
					ServerSslConfiguration serverSslConfiguration2 = serverSslConfiguration1;
					this._sslConfig = serverSslConfiguration1;
					serverSslConfiguration = serverSslConfiguration2;
				}
				return serverSslConfiguration;
			}
			set
			{
				string str = ((ServerState)this._state).CheckIfAvailable(true, false, false);
				if (str == null)
				{
					this._sslConfig = value;
				}
				else
				{
					this._logger.Error(str);
				}
			}
		}

		public Func<IIdentity, WebSocketSharp.Net.NetworkCredential> UserCredentialsFinder
		{
			get
			{
				return this._userCredFinder;
			}
			set
			{
				string str = ((ServerState)this._state).CheckIfAvailable(true, false, false);
				if (str == null)
				{
					this._userCredFinder = value;
				}
				else
				{
					this._logger.Error(str);
				}
			}
		}

		public TimeSpan WaitTime
		{
			get
			{
				return this._services.WaitTime;
			}
			set
			{
				string str = ((ServerState)this._state).CheckIfAvailable(true, false, false) ?? value.CheckIfValidWaitTime();
				if (str == null)
				{
					this._services.WaitTime = value;
				}
				else
				{
					this._logger.Error(str);
				}
			}
		}

		public WebSocketServiceManager WebSocketServices
		{
			get
			{
				return this._services;
			}
		}

		static WebSocketServer()
		{
			WebSocketServer._defaultRealm = "SECRET AREA";
		}

		public WebSocketServer()
		{
			this.init(null, IPAddress.Any, 80, false);
		}

		public WebSocketServer(int port) : this(port, port == 443)
		{
		}

		public WebSocketServer(string url)
		{
			Uri uri;
			string str;
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			if (url.Length == 0)
			{
				throw new ArgumentException("An empty string.", "url");
			}
			if (!WebSocketServer.tryCreateUri(url, out uri, out str))
			{
				throw new ArgumentException(str, "url");
			}
			string dnsSafeHost = uri.DnsSafeHost;
			IPAddress pAddress = dnsSafeHost.ToIPAddress();
			if (!pAddress.IsLocal())
			{
				throw new ArgumentException(string.Concat("The host part isn't a local host name: ", url), "url");
			}
			this.init(dnsSafeHost, pAddress, uri.Port, uri.Scheme == "wss");
		}

		public WebSocketServer(int port, bool secure)
		{
			if (!port.IsPortNumber())
			{
				throw new ArgumentOutOfRangeException("port", string.Concat("Not between 1 and 65535 inclusive: ", port));
			}
			this.init(null, IPAddress.Any, port, secure);
		}

		public WebSocketServer(IPAddress address, int port) : this(address, port, port == 443)
		{
		}

		public WebSocketServer(IPAddress address, int port, bool secure)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (!address.IsLocal())
			{
				throw new ArgumentException(string.Concat("Not a local IP address: ", address), "address");
			}
			if (!port.IsPortNumber())
			{
				throw new ArgumentOutOfRangeException("port", string.Concat("Not between 1 and 65535 inclusive: ", port));
			}
			this.init(null, address, port, secure);
		}

		private void abort()
		{
			object obj = this._sync;
			Monitor.Enter(obj);
			try
			{
				if (this.IsListening)
				{
					this._state = ServerState.ShuttingDown;
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
			this._listener.Stop();
			this._services.Stop(new CloseEventArgs(CloseStatusCode.ServerError), true, false);
			this._state = ServerState.Stop;
		}

		public void AddWebSocketService<TBehavior>(string path, Func<TBehavior> initializer)
		where TBehavior : WebSocketBehavior
		{
			object obj = path.CheckIfValidServicePath();
			if (obj == null)
			{
				if (initializer == null)
				{
					obj = "'initializer' is null.";
				}
				else
				{
					obj = null;
				}
			}
			string str = (string)obj;
			if (str == null)
			{
				this._services.Add<TBehavior>(path, initializer);
			}
			else
			{
				this._logger.Error(str);
			}
		}

		public void AddWebSocketService<TBehaviorWithNew>(string path)
		where TBehaviorWithNew : WebSocketBehavior, new()
		{
			this.AddWebSocketService<TBehaviorWithNew>(path, () => Activator.CreateInstance<TBehaviorWithNew>());
		}

		private bool checkIfAvailable(bool ready, bool start, bool shutting, bool stop, out string message)
		{
			bool flag;
			message = null;
			if ((ready ? false : this._state == ServerState.Ready))
			{
				message = "This operation is not available in: ready";
				flag = false;
			}
			else if ((start ? false : this._state == ServerState.Start))
			{
				message = "This operation is not available in: start";
				flag = false;
			}
			else if ((shutting ? false : this._state == ServerState.ShuttingDown))
			{
				message = "This operation is not available in: shutting down";
				flag = false;
			}
			else if ((stop ? true : this._state != ServerState.Stop))
			{
				flag = true;
			}
			else
			{
				message = "This operation is not available in: stop";
				flag = false;
			}
			return flag;
		}

		private string checkIfCertificateExists()
		{
			string str;
			if (!this._secure || this._sslConfig != null && this._sslConfig.ServerCertificate != null)
			{
				str = null;
			}
			else
			{
				str = "The secure connection requires a server certificate.";
			}
			return str;
		}

		private string getRealm()
		{
			string str = this._realm;
			return (str == null || str.Length <= 0 ? WebSocketServer._defaultRealm : str);
		}

		private void init(string hostname, IPAddress address, int port, bool secure)
		{
			this._hostname = hostname ?? address.ToString();
			this._address = address;
			this._port = port;
			this._secure = secure;
			this._authSchemes = WebSocketSharp.Net.AuthenticationSchemes.Anonymous;
			this._dnsStyle = Uri.CheckHostName(hostname) == UriHostNameType.Dns;
			this._listener = new TcpListener(address, port);
			this._logger = new Logger();
			this._services = new WebSocketServiceManager(this._logger);
			this._sync = new object();
		}

		private void processRequest(TcpListenerWebSocketContext context)
		{
			WebSocketServiceHost webSocketServiceHost;
			Uri requestUri = context.RequestUri;
			if ((requestUri == null ? false : requestUri.Port == this._port))
			{
				if (this._dnsStyle)
				{
					string dnsSafeHost = requestUri.DnsSafeHost;
					if ((Uri.CheckHostName(dnsSafeHost) != UriHostNameType.Dns ? false : dnsSafeHost != this._hostname))
					{
						context.Close(WebSocketSharp.Net.HttpStatusCode.NotFound);
						return;
					}
				}
				if (this._services.InternalTryGetServiceHost(requestUri.AbsolutePath, out webSocketServiceHost))
				{
					webSocketServiceHost.StartSession(context);
				}
				else
				{
					context.Close(WebSocketSharp.Net.HttpStatusCode.NotImplemented);
				}
			}
			else
			{
				context.Close(WebSocketSharp.Net.HttpStatusCode.BadRequest);
			}
		}

		private void receiveRequest()
		{
			while (true)
			{
				try
				{
					TcpClient tcpClient = this._listener.AcceptTcpClient();
					ThreadPool.QueueUserWorkItem((object state) => {
						try
						{
							TcpListenerWebSocketContext webSocketContext = tcpClient.GetWebSocketContext(null, this._secure, this._sslConfig, this._logger);
							if (webSocketContext.Authenticate(this._authSchemes, this.getRealm(), this._userCredFinder))
							{
								this.processRequest(webSocketContext);
							}
						}
						catch (Exception exception)
						{
							this._logger.Fatal(exception.ToString());
							tcpClient.Close();
						}
					});
				}
				catch (SocketException socketException1)
				{
					SocketException socketException = socketException1;
					this._logger.Warn(string.Concat("Receiving has been stopped.\n  reason: ", socketException.Message));
					break;
				}
				catch (Exception exception1)
				{
					this._logger.Fatal(exception1.ToString());
					break;
				}
			}
			if (this.IsListening)
			{
				this.abort();
			}
		}

		public bool RemoveWebSocketService(string path)
		{
			bool flag;
			string str = path.CheckIfValidServicePath();
			if (str == null)
			{
				flag = this._services.Remove(path);
			}
			else
			{
				this._logger.Error(str);
				flag = false;
			}
			return flag;
		}

		public void Start()
		{
			object obj = this._sync;
			Monitor.Enter(obj);
			try
			{
				string str = ((ServerState)this._state).CheckIfAvailable(true, false, false) ?? this.checkIfCertificateExists();
				if (str == null)
				{
					this._services.Start();
					this.startReceiving();
					this._state = ServerState.Start;
				}
				else
				{
					this._logger.Error(str);
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}

		private void startReceiving()
		{
			if (this._reuseAddress)
			{
				this._listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			}
			this._listener.Start();
			this._receiveThread = new Thread(new ThreadStart(this.receiveRequest))
			{
				IsBackground = true
			};
			this._receiveThread.Start();
		}

		public void Stop()
		{
			string str;
			if (this.checkIfAvailable(false, true, false, false, out str))
			{
				object obj = this._sync;
				Monitor.Enter(obj);
				try
				{
					if (this.checkIfAvailable(false, true, false, false, out str))
					{
						this._state = ServerState.ShuttingDown;
					}
					else
					{
						this._logger.Error(str);
						return;
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
				this.stopReceiving(5000);
				this._services.Stop(new CloseEventArgs(), true, true);
				this._state = ServerState.Stop;
			}
			else
			{
				this._logger.Error(str);
			}
		}

		public void Stop(ushort code, string reason)
		{
			string str;
			if (!this.checkIfAvailable(false, true, false, false, out str))
			{
				this._logger.Error(str);
			}
			else if (WebSocket.CheckParametersForClose(code, reason, false, out str))
			{
				object obj = this._sync;
				Monitor.Enter(obj);
				try
				{
					if (this.checkIfAvailable(false, true, false, false, out str))
					{
						this._state = ServerState.ShuttingDown;
					}
					else
					{
						this._logger.Error(str);
						return;
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
				this.stopReceiving(5000);
				if (code != 1005)
				{
					bool flag = !code.IsReserved();
					this._services.Stop(new CloseEventArgs(code, reason), flag, flag);
				}
				else
				{
					this._services.Stop(new CloseEventArgs(), true, true);
				}
				this._state = ServerState.Stop;
			}
			else
			{
				this._logger.Error(str);
			}
		}

		public void Stop(CloseStatusCode code, string reason)
		{
			string str;
			if (!this.checkIfAvailable(false, true, false, false, out str))
			{
				this._logger.Error(str);
			}
			else if (WebSocket.CheckParametersForClose(code, reason, false, out str))
			{
				object obj = this._sync;
				Monitor.Enter(obj);
				try
				{
					if (this.checkIfAvailable(false, true, false, false, out str))
					{
						this._state = ServerState.ShuttingDown;
					}
					else
					{
						this._logger.Error(str);
						return;
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
				this.stopReceiving(5000);
				if (code != CloseStatusCode.NoStatus)
				{
					bool flag = !code.IsReserved();
					this._services.Stop(new CloseEventArgs(code, reason), flag, flag);
				}
				else
				{
					this._services.Stop(new CloseEventArgs(), true, true);
				}
				this._state = ServerState.Stop;
			}
			else
			{
				this._logger.Error(str);
			}
		}

		private void stopReceiving(int millisecondsTimeout)
		{
			this._listener.Stop();
			this._receiveThread.Join(millisecondsTimeout);
		}

		private static bool tryCreateUri(string uriString, out Uri result, out string message)
		{
			bool flag;
			if (!uriString.TryCreateWebSocketUri(out result, out message))
			{
				flag = false;
			}
			else if (result.PathAndQuery == "/")
			{
				flag = true;
			}
			else
			{
				result = null;
				message = string.Concat("Includes the path or query component: ", uriString);
				flag = false;
			}
			return flag;
		}
	}
}