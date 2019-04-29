using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace WebSocketSharp.Server
{
	public class WebSocketServiceManager
	{
		private volatile bool _clean;

		private Dictionary<string, WebSocketServiceHost> _hosts;

		private Logger _logger;

		private volatile ServerState _state;

		private object _sync;

		private TimeSpan _waitTime;

		public int Count
		{
			get
			{
				int count;
				object obj = this._sync;
				Monitor.Enter(obj);
				try
				{
					count = this._hosts.Count;
				}
				finally
				{
					Monitor.Exit(obj);
				}
				return count;
			}
		}

		public IEnumerable<WebSocketServiceHost> Hosts
		{
			get
			{
				IEnumerable<WebSocketServiceHost> list;
				object obj = this._sync;
				Monitor.Enter(obj);
				try
				{
					list = this._hosts.Values.ToList<WebSocketServiceHost>();
				}
				finally
				{
					Monitor.Exit(obj);
				}
				return list;
			}
		}

		public WebSocketServiceHost this[string path]
		{
			get
			{
				WebSocketServiceHost webSocketServiceHost;
				this.TryGetServiceHost(path, out webSocketServiceHost);
				return webSocketServiceHost;
			}
		}

		public bool KeepClean
		{
			get
			{
				return this._clean;
			}
			internal set
			{
				object obj = this._sync;
				Monitor.Enter(obj);
				try
				{
					if (value != this._clean)
					{
						this._clean = value;
						foreach (WebSocketServiceHost webSocketServiceHost in this._hosts.Values)
						{
							webSocketServiceHost.KeepClean = value;
						}
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
		}

		public IEnumerable<string> Paths
		{
			get
			{
				IEnumerable<string> list;
				object obj = this._sync;
				Monitor.Enter(obj);
				try
				{
					list = this._hosts.Keys.ToList<string>();
				}
				finally
				{
					Monitor.Exit(obj);
				}
				return list;
			}
		}

		public int SessionCount
		{
			get
			{
				int count = 0;
				foreach (WebSocketServiceHost host in this.Hosts)
				{
					if (this._state == ServerState.Start)
					{
						count += host.Sessions.Count;
					}
					else
					{
						break;
					}
				}
				return count;
			}
		}

		public TimeSpan WaitTime
		{
			get
			{
				return this._waitTime;
			}
			internal set
			{
				object obj = this._sync;
				Monitor.Enter(obj);
				try
				{
					if (value != this._waitTime)
					{
						this._waitTime = value;
						foreach (WebSocketServiceHost webSocketServiceHost in this._hosts.Values)
						{
							webSocketServiceHost.WaitTime = value;
						}
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
		}

		internal WebSocketServiceManager() : this(new Logger())
		{
		}

		internal WebSocketServiceManager(Logger logger)
		{
			this._logger = logger;
			this._clean = true;
			this._hosts = new Dictionary<string, WebSocketServiceHost>();
			this._state = ServerState.Ready;
			this._sync = ((ICollection)this._hosts).SyncRoot;
			this._waitTime = TimeSpan.FromSeconds(1);
		}

		internal void Add<TBehavior>(string path, Func<TBehavior> initializer)
		where TBehavior : WebSocketBehavior
		{
			WebSocketServiceHost webSocketServiceHost;
			object obj = this._sync;
			Monitor.Enter(obj);
			try
			{
				path = HttpUtility.UrlDecode(path).TrimEndSlash();
				if (!this._hosts.TryGetValue(path, out webSocketServiceHost))
				{
					webSocketServiceHost = new WebSocketServiceHost<TBehavior>(path, initializer, this._logger);
					if (!this._clean)
					{
						webSocketServiceHost.KeepClean = false;
					}
					if (this._waitTime != webSocketServiceHost.WaitTime)
					{
						webSocketServiceHost.WaitTime = this._waitTime;
					}
					if (this._state == ServerState.Start)
					{
						webSocketServiceHost.Start();
					}
					this._hosts.Add(path, webSocketServiceHost);
				}
				else
				{
					this._logger.Error(string.Concat("A WebSocket service with the specified path already exists:\n  path: ", path));
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}

		private void broadcast(Opcode opcode, byte[] data, Action completed)
		{
			Dictionary<CompressionMethod, byte[]> compressionMethods = new Dictionary<CompressionMethod, byte[]>();
			try
			{
				try
				{
					foreach (WebSocketServiceHost host in this.Hosts)
					{
						if (this._state == ServerState.Start)
						{
							host.Sessions.Broadcast(opcode, data, compressionMethods);
						}
						else
						{
							break;
						}
					}
					if (completed != null)
					{
						completed();
					}
				}
				catch (Exception exception)
				{
					this._logger.Fatal(exception.ToString());
				}
			}
			finally
			{
				compressionMethods.Clear();
			}
		}

		private void broadcast(Opcode opcode, Stream stream, Action completed)
		{
			Dictionary<CompressionMethod, Stream> compressionMethods = new Dictionary<CompressionMethod, Stream>();
			try
			{
				try
				{
					foreach (WebSocketServiceHost host in this.Hosts)
					{
						if (this._state == ServerState.Start)
						{
							host.Sessions.Broadcast(opcode, stream, compressionMethods);
						}
						else
						{
							break;
						}
					}
					if (completed != null)
					{
						completed();
					}
				}
				catch (Exception exception)
				{
					this._logger.Fatal(exception.ToString());
				}
			}
			finally
			{
				foreach (Stream value in compressionMethods.Values)
				{
					value.Dispose();
				}
				compressionMethods.Clear();
			}
		}

		public void Broadcast(byte[] data)
		{
			string str = ((ServerState)this._state).CheckIfAvailable(false, true, false) ?? WebSocket.CheckSendParameter(data);
			if (str != null)
			{
				this._logger.Error(str);
			}
			else if ((long)data.Length > (long)WebSocket.FragmentLength)
			{
				this.broadcast(Opcode.Binary, new MemoryStream(data), null);
			}
			else
			{
				this.broadcast(Opcode.Binary, data, null);
			}
		}

		public void Broadcast(string data)
		{
			string str = ((ServerState)this._state).CheckIfAvailable(false, true, false) ?? WebSocket.CheckSendParameter(data);
			if (str == null)
			{
				byte[] numArray = data.UTF8Encode();
				if ((long)numArray.Length > (long)WebSocket.FragmentLength)
				{
					this.broadcast(Opcode.Text, new MemoryStream(numArray), null);
				}
				else
				{
					this.broadcast(Opcode.Text, numArray, null);
				}
			}
			else
			{
				this._logger.Error(str);
			}
		}

		private void broadcastAsync(Opcode opcode, byte[] data, Action completed)
		{
			ThreadPool.QueueUserWorkItem((object state) => this.broadcast(opcode, data, completed));
		}

		private void broadcastAsync(Opcode opcode, Stream stream, Action completed)
		{
			ThreadPool.QueueUserWorkItem((object state) => this.broadcast(opcode, stream, completed));
		}

		public void BroadcastAsync(byte[] data, Action completed)
		{
			string str = ((ServerState)this._state).CheckIfAvailable(false, true, false) ?? WebSocket.CheckSendParameter(data);
			if (str != null)
			{
				this._logger.Error(str);
			}
			else if ((long)data.Length > (long)WebSocket.FragmentLength)
			{
				this.broadcastAsync(Opcode.Binary, new MemoryStream(data), completed);
			}
			else
			{
				this.broadcastAsync(Opcode.Binary, data, completed);
			}
		}

		public void BroadcastAsync(string data, Action completed)
		{
			string str = ((ServerState)this._state).CheckIfAvailable(false, true, false) ?? WebSocket.CheckSendParameter(data);
			if (str == null)
			{
				byte[] numArray = data.UTF8Encode();
				if ((long)numArray.Length > (long)WebSocket.FragmentLength)
				{
					this.broadcastAsync(Opcode.Text, new MemoryStream(numArray), completed);
				}
				else
				{
					this.broadcastAsync(Opcode.Text, numArray, completed);
				}
			}
			else
			{
				this._logger.Error(str);
			}
		}

		public void BroadcastAsync(Stream stream, int length, Action completed)
		{
			string str = ((ServerState)this._state).CheckIfAvailable(false, true, false) ?? WebSocket.CheckSendParameters(stream, length);
			if (str == null)
			{
				stream.ReadBytesAsync(length, (byte[] data) => {
					int num = (int)data.Length;
					if (num != 0)
					{
						if (num < length)
						{
							this._logger.Warn(string.Format("The data with 'length' cannot be read from 'stream':\n  expected: {0}\n  actual: {1}", length, num));
						}
						if (num > WebSocket.FragmentLength)
						{
							this.broadcast(Opcode.Binary, new MemoryStream(data), completed);
						}
						else
						{
							this.broadcast(Opcode.Binary, data, completed);
						}
					}
					else
					{
						this._logger.Error("The data cannot be read from 'stream'.");
					}
				}, (Exception ex) => this._logger.Fatal(ex.ToString()));
			}
			else
			{
				this._logger.Error(str);
			}
		}

		private Dictionary<string, Dictionary<string, bool>> broadping(byte[] frameAsBytes, TimeSpan timeout)
		{
			Dictionary<string, Dictionary<string, bool>> strs = new Dictionary<string, Dictionary<string, bool>>();
			foreach (WebSocketServiceHost host in this.Hosts)
			{
				if (this._state == ServerState.Start)
				{
					strs.Add(host.Path, host.Sessions.Broadping(frameAsBytes, timeout));
				}
				else
				{
					break;
				}
			}
			return strs;
		}

		public Dictionary<string, Dictionary<string, bool>> Broadping()
		{
			Dictionary<string, Dictionary<string, bool>> strs;
			string str = ((ServerState)this._state).CheckIfAvailable(false, true, false);
			if (str == null)
			{
				strs = this.broadping(WebSocketFrame.EmptyPingBytes, this._waitTime);
			}
			else
			{
				this._logger.Error(str);
				strs = null;
			}
			return strs;
		}

		public Dictionary<string, Dictionary<string, bool>> Broadping(string message)
		{
			Dictionary<string, Dictionary<string, bool>> strs;
			if ((message == null ? false : message.Length != 0))
			{
				byte[] numArray = null;
				string str = ((ServerState)this._state).CheckIfAvailable(false, true, false) ?? WebSocket.CheckPingParameter(message, out numArray);
				if (str == null)
				{
					strs = this.broadping(WebSocketFrame.CreatePingFrame(numArray, false).ToArray(), this._waitTime);
				}
				else
				{
					this._logger.Error(str);
					strs = null;
				}
			}
			else
			{
				strs = this.Broadping();
			}
			return strs;
		}

		internal bool InternalTryGetServiceHost(string path, out WebSocketServiceHost host)
		{
			bool flag;
			object obj = this._sync;
			Monitor.Enter(obj);
			try
			{
				path = HttpUtility.UrlDecode(path).TrimEndSlash();
				flag = this._hosts.TryGetValue(path, out host);
			}
			finally
			{
				Monitor.Exit(obj);
			}
			if (!flag)
			{
				this._logger.Error(string.Concat("A WebSocket service with the specified path isn't found:\n  path: ", path));
			}
			return flag;
		}

		internal bool Remove(string path)
		{
			WebSocketServiceHost webSocketServiceHost;
			bool flag;
			object obj = this._sync;
			Monitor.Enter(obj);
			try
			{
				path = HttpUtility.UrlDecode(path).TrimEndSlash();
				if (this._hosts.TryGetValue(path, out webSocketServiceHost))
				{
					this._hosts.Remove(path);
				}
				else
				{
					this._logger.Error(string.Concat("A WebSocket service with the specified path isn't found:\n  path: ", path));
					flag = false;
					return flag;
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
			if (webSocketServiceHost.State == ServerState.Start)
			{
				webSocketServiceHost.Stop(1001, null);
			}
			flag = true;
			return flag;
		}

		internal void Start()
		{
			object obj = this._sync;
			Monitor.Enter(obj);
			try
			{
				foreach (WebSocketServiceHost value in this._hosts.Values)
				{
					value.Start();
				}
				this._state = ServerState.Start;
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}

		internal void Stop(CloseEventArgs e, bool send, bool receive)
		{
			byte[] array;
			object obj = this._sync;
			Monitor.Enter(obj);
			try
			{
				this._state = ServerState.ShuttingDown;
				if (send)
				{
					array = WebSocketFrame.CreateCloseFrame(e.PayloadData, false).ToArray();
				}
				else
				{
					array = null;
				}
				byte[] numArray = array;
				foreach (WebSocketServiceHost value in this._hosts.Values)
				{
					value.Sessions.Stop(e, numArray, receive);
				}
				this._hosts.Clear();
				this._state = ServerState.Stop;
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}

		public bool TryGetServiceHost(string path, out WebSocketServiceHost host)
		{
			bool flag;
			string str = ((ServerState)this._state).CheckIfAvailable(false, true, false) ?? path.CheckIfValidServicePath();
			if (str == null)
			{
				flag = this.InternalTryGetServiceHost(path, out host);
			}
			else
			{
				this._logger.Error(str);
				host = null;
				flag = false;
			}
			return flag;
		}
	}
}