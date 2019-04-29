using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace WebSocketSharp.Net
{
	internal sealed class EndPointListener
	{
		private List<HttpListenerPrefix> _all;

		private readonly static string _defaultCertFolderPath;

		private IPEndPoint _endpoint;

		private Dictionary<HttpListenerPrefix, WebSocketSharp.Net.HttpListener> _prefixes;

		private bool _secure;

		private Socket _socket;

		private ServerSslConfiguration _sslConfig;

		private List<HttpListenerPrefix> _unhandled;

		private Dictionary<HttpConnection, HttpConnection> _unregistered;

		private object _unregisteredSync;

		public IPAddress Address
		{
			get
			{
				return this._endpoint.Address;
			}
		}

		public bool IsSecure
		{
			get
			{
				return this._secure;
			}
		}

		public int Port
		{
			get
			{
				return this._endpoint.Port;
			}
		}

		public ServerSslConfiguration SslConfiguration
		{
			get
			{
				return this._sslConfig;
			}
		}

		static EndPointListener()
		{
			EndPointListener._defaultCertFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		}

		internal EndPointListener(IPEndPoint endpoint, bool secure, string certificateFolderPath, ServerSslConfiguration sslConfig, bool reuseAddress)
		{
			if (secure)
			{
				X509Certificate2 certificate = EndPointListener.getCertificate(endpoint.Port, certificateFolderPath, sslConfig.ServerCertificate);
				if (certificate == null)
				{
					throw new ArgumentException("No server certificate could be found.");
				}
				this._secure = true;
				this._sslConfig = new ServerSslConfiguration(certificate, sslConfig.ClientCertificateRequired, sslConfig.EnabledSslProtocols, sslConfig.CheckCertificateRevocation)
				{
					ClientCertificateValidationCallback = sslConfig.ClientCertificateValidationCallback
				};
			}
			this._endpoint = endpoint;
			this._prefixes = new Dictionary<HttpListenerPrefix, WebSocketSharp.Net.HttpListener>();
			this._unregistered = new Dictionary<HttpConnection, HttpConnection>();
			this._unregisteredSync = ((ICollection)this._unregistered).SyncRoot;
			this._socket = new Socket(endpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			if (reuseAddress)
			{
				this._socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			}
			this._socket.Bind(endpoint);
			this._socket.Listen(500);
			this._socket.BeginAccept(new AsyncCallback(EndPointListener.onAccept), this);
		}

		public void AddPrefix(HttpListenerPrefix prefix, WebSocketSharp.Net.HttpListener listener)
		{
			List<HttpListenerPrefix> httpListenerPrefixes;
			List<HttpListenerPrefix> httpListenerPrefixes1;
			Dictionary<HttpListenerPrefix, WebSocketSharp.Net.HttpListener> httpListenerPrefixes2;
			Dictionary<HttpListenerPrefix, WebSocketSharp.Net.HttpListener> httpListenerPrefixes3;
			if (prefix.Host == "*")
			{
				do
				{
					httpListenerPrefixes = this._unhandled;
					httpListenerPrefixes1 = (httpListenerPrefixes != null ? new List<HttpListenerPrefix>(httpListenerPrefixes) : new List<HttpListenerPrefix>());
					prefix.Listener = listener;
					EndPointListener.addSpecial(httpListenerPrefixes1, prefix);
				}
				while (Interlocked.CompareExchange<List<HttpListenerPrefix>>(ref this._unhandled, httpListenerPrefixes1, httpListenerPrefixes) != httpListenerPrefixes);
			}
			else if (prefix.Host != "+")
			{
				do
				{
					httpListenerPrefixes2 = this._prefixes;
					if (!httpListenerPrefixes2.ContainsKey(prefix))
					{
						httpListenerPrefixes3 = new Dictionary<HttpListenerPrefix, WebSocketSharp.Net.HttpListener>(httpListenerPrefixes2);
						httpListenerPrefixes3[prefix] = listener;
					}
					else
					{
						if (httpListenerPrefixes2[prefix] != listener)
						{
							throw new WebSocketSharp.Net.HttpListenerException(87, string.Format("There's another listener for {0}.", prefix));
						}
						break;
					}
				}
				while (Interlocked.CompareExchange<Dictionary<HttpListenerPrefix, WebSocketSharp.Net.HttpListener>>(ref this._prefixes, httpListenerPrefixes3, httpListenerPrefixes2) != httpListenerPrefixes2);
			}
			else
			{
				do
				{
					httpListenerPrefixes = this._all;
					httpListenerPrefixes1 = (httpListenerPrefixes != null ? new List<HttpListenerPrefix>(httpListenerPrefixes) : new List<HttpListenerPrefix>());
					prefix.Listener = listener;
					EndPointListener.addSpecial(httpListenerPrefixes1, prefix);
				}
				while (Interlocked.CompareExchange<List<HttpListenerPrefix>>(ref this._all, httpListenerPrefixes1, httpListenerPrefixes) != httpListenerPrefixes);
			}
		}

		private static void addSpecial(List<HttpListenerPrefix> prefixes, HttpListenerPrefix prefix)
		{
			string path = prefix.Path;
			foreach (HttpListenerPrefix httpListenerPrefix in prefixes)
			{
				if (httpListenerPrefix.Path == path)
				{
					throw new WebSocketSharp.Net.HttpListenerException(87, "The prefix is already in use.");
				}
			}
			prefixes.Add(prefix);
		}

		internal static bool CertificateExists(int port, string folderPath)
		{
			if ((folderPath == null ? true : folderPath.Length == 0))
			{
				folderPath = EndPointListener._defaultCertFolderPath;
			}
			string str = Path.Combine(folderPath, string.Format("{0}.cer", port));
			string str1 = Path.Combine(folderPath, string.Format("{0}.key", port));
			return (!File.Exists(str) ? false : File.Exists(str1));
		}

		public void Close()
		{
			this._socket.Close();
			HttpConnection[] httpConnectionArray = null;
			object obj = this._unregisteredSync;
			Monitor.Enter(obj);
			try
			{
				if (this._unregistered.Count != 0)
				{
					Dictionary<HttpConnection, HttpConnection>.KeyCollection keys = this._unregistered.Keys;
					httpConnectionArray = new HttpConnection[keys.Count];
					keys.CopyTo(httpConnectionArray, 0);
					this._unregistered.Clear();
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
			for (int i = (int)httpConnectionArray.Length - 1; i >= 0; i--)
			{
				httpConnectionArray[i].Close(true);
			}
		}

		private static RSACryptoServiceProvider createRSAFromFile(string filename)
		{
			byte[] numArray = null;
			using (FileStream fileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				numArray = new byte[checked((IntPtr)fileStream.Length)];
				fileStream.Read(numArray, 0, (int)numArray.Length);
			}
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
			rSACryptoServiceProvider.ImportCspBlob(numArray);
			return rSACryptoServiceProvider;
		}

		private static X509Certificate2 getCertificate(int port, string folderPath, X509Certificate2 defaultCertificate)
		{
			X509Certificate2 x509Certificate2;
			if ((folderPath == null ? true : folderPath.Length == 0))
			{
				folderPath = EndPointListener._defaultCertFolderPath;
			}
			try
			{
				string str = Path.Combine(folderPath, string.Format("{0}.cer", port));
				string str1 = Path.Combine(folderPath, string.Format("{0}.key", port));
				if ((!File.Exists(str) ? false : File.Exists(str1)))
				{
					X509Certificate2 x509Certificate21 = new X509Certificate2(str)
					{
						PrivateKey = EndPointListener.createRSAFromFile(str1)
					};
					x509Certificate2 = x509Certificate21;
					return x509Certificate2;
				}
			}
			catch
			{
			}
			x509Certificate2 = defaultCertificate;
			return x509Certificate2;
		}

		private void leaveIfNoPrefix()
		{
			if (this._prefixes.Count <= 0)
			{
				List<HttpListenerPrefix> httpListenerPrefixes = this._unhandled;
				if ((httpListenerPrefixes == null ? true : httpListenerPrefixes.Count <= 0))
				{
					httpListenerPrefixes = this._all;
					if ((httpListenerPrefixes == null ? true : httpListenerPrefixes.Count <= 0))
					{
						EndPointManager.RemoveEndPoint(this._endpoint);
					}
				}
			}
		}

		private static void onAccept(IAsyncResult asyncResult)
		{
			EndPointListener asyncState = (EndPointListener)asyncResult.AsyncState;
			Socket socket = null;
			try
			{
				socket = asyncState._socket.EndAccept(asyncResult);
			}
			catch (SocketException socketException)
			{
			}
			catch (ObjectDisposedException objectDisposedException)
			{
				return;
			}
			try
			{
				asyncState._socket.BeginAccept(new AsyncCallback(EndPointListener.onAccept), asyncState);
			}
			catch
			{
				if (socket != null)
				{
					socket.Close();
				}
				return;
			}
			if (socket != null)
			{
				EndPointListener.processAccepted(socket, asyncState);
			}
		}

		private static void processAccepted(Socket socket, EndPointListener listener)
		{
			HttpConnection httpConnection = null;
			try
			{
				httpConnection = new HttpConnection(socket, listener);
				object obj = listener._unregisteredSync;
				Monitor.Enter(obj);
				try
				{
					listener._unregistered[httpConnection] = httpConnection;
				}
				finally
				{
					Monitor.Exit(obj);
				}
				httpConnection.BeginReadRequest();
			}
			catch
			{
				if (httpConnection == null)
				{
					socket.Close();
				}
				else
				{
					httpConnection.Close(true);
				}
			}
		}

		internal void RemoveConnection(HttpConnection connection)
		{
			object obj = this._unregisteredSync;
			Monitor.Enter(obj);
			try
			{
				this._unregistered.Remove(connection);
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}

		public void RemovePrefix(HttpListenerPrefix prefix, WebSocketSharp.Net.HttpListener listener)
		{
			List<HttpListenerPrefix> httpListenerPrefixes;
			List<HttpListenerPrefix> httpListenerPrefixes1;
			Dictionary<HttpListenerPrefix, WebSocketSharp.Net.HttpListener> httpListenerPrefixes2;
			Dictionary<HttpListenerPrefix, WebSocketSharp.Net.HttpListener> httpListenerPrefixes3;
			if (prefix.Host == "*")
			{
				do
				{
					httpListenerPrefixes = this._unhandled;
					if (httpListenerPrefixes != null)
					{
						httpListenerPrefixes1 = new List<HttpListenerPrefix>(httpListenerPrefixes);
						if (EndPointListener.removeSpecial(httpListenerPrefixes1, prefix))
						{
							continue;
						}
						break;
					}
					else
					{
						break;
					}
				}
				while (Interlocked.CompareExchange<List<HttpListenerPrefix>>(ref this._unhandled, httpListenerPrefixes1, httpListenerPrefixes) != httpListenerPrefixes);
				this.leaveIfNoPrefix();
			}
			else if (prefix.Host != "+")
			{
				do
				{
					httpListenerPrefixes2 = this._prefixes;
					if (httpListenerPrefixes2.ContainsKey(prefix))
					{
						httpListenerPrefixes3 = new Dictionary<HttpListenerPrefix, WebSocketSharp.Net.HttpListener>(httpListenerPrefixes2);
						httpListenerPrefixes3.Remove(prefix);
					}
					else
					{
						break;
					}
				}
				while (Interlocked.CompareExchange<Dictionary<HttpListenerPrefix, WebSocketSharp.Net.HttpListener>>(ref this._prefixes, httpListenerPrefixes3, httpListenerPrefixes2) != httpListenerPrefixes2);
				this.leaveIfNoPrefix();
			}
			else
			{
				do
				{
					httpListenerPrefixes = this._all;
					if (httpListenerPrefixes != null)
					{
						httpListenerPrefixes1 = new List<HttpListenerPrefix>(httpListenerPrefixes);
						if (EndPointListener.removeSpecial(httpListenerPrefixes1, prefix))
						{
							continue;
						}
						break;
					}
					else
					{
						break;
					}
				}
				while (Interlocked.CompareExchange<List<HttpListenerPrefix>>(ref this._all, httpListenerPrefixes1, httpListenerPrefixes) != httpListenerPrefixes);
				this.leaveIfNoPrefix();
			}
		}

		private static bool removeSpecial(List<HttpListenerPrefix> prefixes, HttpListenerPrefix prefix)
		{
			bool flag;
			string path = prefix.Path;
			int count = prefixes.Count;
			int num = 0;
			while (true)
			{
				if (num >= count)
				{
					flag = false;
					break;
				}
				else if (prefixes[num].Path != path)
				{
					num++;
				}
				else
				{
					prefixes.RemoveAt(num);
					flag = true;
					break;
				}
			}
			return flag;
		}

		private static WebSocketSharp.Net.HttpListener searchHttpListenerFromSpecial(string path, List<HttpListenerPrefix> prefixes)
		{
			WebSocketSharp.Net.HttpListener httpListener;
			if (prefixes != null)
			{
				WebSocketSharp.Net.HttpListener listener = null;
				int num = -1;
				foreach (HttpListenerPrefix prefix in prefixes)
				{
					string str = prefix.Path;
					int length = str.Length;
					if (length >= num)
					{
						if (path.StartsWith(str))
						{
							num = length;
							listener = prefix.Listener;
						}
					}
				}
				httpListener = listener;
			}
			else
			{
				httpListener = null;
			}
			return httpListener;
		}

		internal bool TrySearchHttpListener(Uri uri, out WebSocketSharp.Net.HttpListener listener)
		{
			bool flag;
			listener = null;
			if (uri != null)
			{
				string host = uri.Host;
				bool flag1 = Uri.CheckHostName(host) == UriHostNameType.Dns;
				string str = uri.Port.ToString();
				string str1 = HttpUtility.UrlDecode(uri.AbsolutePath);
				string str2 = (str1[str1.Length - 1] != '/' ? string.Concat(str1, "/") : str1);
				if ((host == null ? false : host.Length > 0))
				{
					int num = -1;
					foreach (HttpListenerPrefix key in this._prefixes.Keys)
					{
						if (flag1)
						{
							string host1 = key.Host;
							if ((Uri.CheckHostName(host1) != UriHostNameType.Dns ? false : host1 != host))
							{
								continue;
							}
						}
						if (key.Port == str)
						{
							string path = key.Path;
							int length = path.Length;
							if (length >= num)
							{
								if ((str1.StartsWith(path) ? true : str2.StartsWith(path)))
								{
									num = length;
									listener = this._prefixes[key];
								}
							}
						}
					}
					if (num != -1)
					{
						flag = true;
						return flag;
					}
				}
				List<HttpListenerPrefix> httpListenerPrefixes = this._unhandled;
				listener = EndPointListener.searchHttpListenerFromSpecial(str1, httpListenerPrefixes);
				if ((listener != null ? false : str2 != str1))
				{
					listener = EndPointListener.searchHttpListenerFromSpecial(str2, httpListenerPrefixes);
				}
				if (listener == null)
				{
					httpListenerPrefixes = this._all;
					listener = EndPointListener.searchHttpListenerFromSpecial(str1, httpListenerPrefixes);
					if ((listener != null ? false : str2 != str1))
					{
						listener = EndPointListener.searchHttpListenerFromSpecial(str2, httpListenerPrefixes);
					}
					flag = listener != null;
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = false;
			}
			return flag;
		}
	}
}