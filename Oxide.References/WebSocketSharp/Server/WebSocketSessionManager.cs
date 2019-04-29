using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;
using WebSocketSharp;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp.Server
{
	public class WebSocketSessionManager
	{
		private volatile bool _clean;

		private object _forSweep;

		private Logger _logger;

		private Dictionary<string, IWebSocketSession> _sessions;

		private volatile ServerState _state;

		private volatile bool _sweeping;

		private System.Timers.Timer _sweepTimer;

		private object _sync;

		private TimeSpan _waitTime;

		public IEnumerable<string> ActiveIDs
		{
			get
			{
				foreach (KeyValuePair<string, bool> keyValuePair in this.Broadping(WebSocketFrame.EmptyPingBytes, this._waitTime))
				{
					if (keyValuePair.Value)
					{
						yield return keyValuePair.Key;
					}
					keyValuePair = new KeyValuePair<string, bool>();
				}
			}
		}

		public int Count
		{
			get
			{
				int count;
				object obj = this._sync;
				Monitor.Enter(obj);
				try
				{
					count = this._sessions.Count;
				}
				finally
				{
					Monitor.Exit(obj);
				}
				return count;
			}
		}

		public IEnumerable<string> IDs
		{
			get
			{
				IEnumerable<string> list;
				if (this._state != ServerState.ShuttingDown)
				{
					object obj = this._sync;
					Monitor.Enter(obj);
					try
					{
						list = this._sessions.Keys.ToList<string>();
					}
					finally
					{
						Monitor.Exit(obj);
					}
				}
				else
				{
					list = new string[0];
				}
				return list;
			}
		}

		public IEnumerable<string> InactiveIDs
		{
			get
			{
				foreach (KeyValuePair<string, bool> keyValuePair in this.Broadping(WebSocketFrame.EmptyPingBytes, this._waitTime))
				{
					if (!keyValuePair.Value)
					{
						yield return keyValuePair.Key;
					}
					keyValuePair = new KeyValuePair<string, bool>();
				}
			}
		}

		public IWebSocketSession this[string id]
		{
			get
			{
				IWebSocketSession webSocketSession;
				this.TryGetSession(id, out webSocketSession);
				return webSocketSession;
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
				if (value != this._clean)
				{
					this._clean = value;
					if (this._state == ServerState.Start)
					{
						this._sweepTimer.Enabled = value;
					}
				}
			}
		}

		public IEnumerable<IWebSocketSession> Sessions
		{
			get
			{
				IEnumerable<IWebSocketSession> list;
				if (this._state != ServerState.ShuttingDown)
				{
					object obj = this._sync;
					Monitor.Enter(obj);
					try
					{
						list = this._sessions.Values.ToList<IWebSocketSession>();
					}
					finally
					{
						Monitor.Exit(obj);
					}
				}
				else
				{
					list = new IWebSocketSession[0];
				}
				return list;
			}
		}

		internal ServerState State
		{
			get
			{
				return this._state;
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
				if (value != this._waitTime)
				{
					this._waitTime = value;
					foreach (IWebSocketSession session in this.Sessions)
					{
						session.Context.WebSocket.WaitTime = value;
					}
				}
			}
		}

		internal WebSocketSessionManager() : this(new Logger())
		{
		}

		internal WebSocketSessionManager(Logger logger)
		{
			this._logger = logger;
			this._clean = true;
			this._forSweep = new object();
			this._sessions = new Dictionary<string, IWebSocketSession>();
			this._state = ServerState.Ready;
			this._sync = ((ICollection)this._sessions).SyncRoot;
			this._waitTime = TimeSpan.FromSeconds(1);
			this.setSweepTimer(60000);
		}

		internal string Add(IWebSocketSession session)
		{
			string str;
			object obj = this._sync;
			Monitor.Enter(obj);
			try
			{
				if (this._state == ServerState.Start)
				{
					string str1 = WebSocketSessionManager.createID();
					this._sessions.Add(str1, session);
					str = str1;
				}
				else
				{
					str = null;
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return str;
		}

		private void broadcast(Opcode opcode, byte[] data, Action completed)
		{
			Dictionary<CompressionMethod, byte[]> compressionMethods = new Dictionary<CompressionMethod, byte[]>();
			try
			{
				try
				{
					this.Broadcast(opcode, data, compressionMethods);
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
					this.Broadcast(opcode, stream, compressionMethods);
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

		internal void Broadcast(Opcode opcode, byte[] data, Dictionary<CompressionMethod, byte[]> cache)
		{
			foreach (IWebSocketSession session in this.Sessions)
			{
				if (this._state == ServerState.Start)
				{
					session.Context.WebSocket.Send(opcode, data, cache);
				}
				else
				{
					break;
				}
			}
		}

		internal void Broadcast(Opcode opcode, Stream stream, Dictionary<CompressionMethod, Stream> cache)
		{
			foreach (IWebSocketSession session in this.Sessions)
			{
				if (this._state == ServerState.Start)
				{
					session.Context.WebSocket.Send(opcode, stream, cache);
				}
				else
				{
					break;
				}
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

		internal Dictionary<string, bool> Broadping(byte[] frameAsBytes, TimeSpan timeout)
		{
			Dictionary<string, bool> strs = new Dictionary<string, bool>();
			foreach (IWebSocketSession session in this.Sessions)
			{
				if (this._state == ServerState.Start)
				{
					strs.Add(session.ID, session.Context.WebSocket.Ping(frameAsBytes, timeout));
				}
				else
				{
					break;
				}
			}
			return strs;
		}

		public Dictionary<string, bool> Broadping()
		{
			Dictionary<string, bool> strs;
			string str = ((ServerState)this._state).CheckIfAvailable(false, true, false);
			if (str == null)
			{
				strs = this.Broadping(WebSocketFrame.EmptyPingBytes, this._waitTime);
			}
			else
			{
				this._logger.Error(str);
				strs = null;
			}
			return strs;
		}

		public Dictionary<string, bool> Broadping(string message)
		{
			Dictionary<string, bool> strs;
			if ((message == null ? false : message.Length != 0))
			{
				byte[] numArray = null;
				string str = ((ServerState)this._state).CheckIfAvailable(false, true, false) ?? WebSocket.CheckPingParameter(message, out numArray);
				if (str == null)
				{
					strs = this.Broadping(WebSocketFrame.CreatePingFrame(numArray, false).ToArray(), this._waitTime);
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

		public void CloseSession(string id)
		{
			IWebSocketSession webSocketSession;
			if (this.TryGetSession(id, out webSocketSession))
			{
				webSocketSession.Context.WebSocket.Close();
			}
		}

		public void CloseSession(string id, ushort code, string reason)
		{
			IWebSocketSession webSocketSession;
			if (this.TryGetSession(id, out webSocketSession))
			{
				webSocketSession.Context.WebSocket.Close(code, reason);
			}
		}

		public void CloseSession(string id, CloseStatusCode code, string reason)
		{
			IWebSocketSession webSocketSession;
			if (this.TryGetSession(id, out webSocketSession))
			{
				webSocketSession.Context.WebSocket.Close(code, reason);
			}
		}

		private static string createID()
		{
			return Guid.NewGuid().ToString("N");
		}

		public bool PingTo(string id)
		{
			IWebSocketSession webSocketSession;
			return (!this.TryGetSession(id, out webSocketSession) ? false : webSocketSession.Context.WebSocket.Ping());
		}

		public bool PingTo(string message, string id)
		{
			IWebSocketSession webSocketSession;
			return (!this.TryGetSession(id, out webSocketSession) ? false : webSocketSession.Context.WebSocket.Ping(message));
		}

		internal bool Remove(string id)
		{
			bool flag;
			object obj = this._sync;
			Monitor.Enter(obj);
			try
			{
				flag = this._sessions.Remove(id);
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return flag;
		}

		public void SendTo(byte[] data, string id)
		{
			IWebSocketSession webSocketSession;
			if (this.TryGetSession(id, out webSocketSession))
			{
				webSocketSession.Context.WebSocket.Send(data);
			}
		}

		public void SendTo(string data, string id)
		{
			IWebSocketSession webSocketSession;
			if (this.TryGetSession(id, out webSocketSession))
			{
				webSocketSession.Context.WebSocket.Send(data);
			}
		}

		public void SendToAsync(byte[] data, string id, Action<bool> completed)
		{
			IWebSocketSession webSocketSession;
			if (this.TryGetSession(id, out webSocketSession))
			{
				webSocketSession.Context.WebSocket.SendAsync(data, completed);
			}
		}

		public void SendToAsync(string data, string id, Action<bool> completed)
		{
			IWebSocketSession webSocketSession;
			if (this.TryGetSession(id, out webSocketSession))
			{
				webSocketSession.Context.WebSocket.SendAsync(data, completed);
			}
		}

		public void SendToAsync(Stream stream, int length, string id, Action<bool> completed)
		{
			IWebSocketSession webSocketSession;
			if (this.TryGetSession(id, out webSocketSession))
			{
				webSocketSession.Context.WebSocket.SendAsync(stream, length, completed);
			}
		}

		private void setSweepTimer(double interval)
		{
			this._sweepTimer = new System.Timers.Timer(interval);
			this._sweepTimer.Elapsed += new ElapsedEventHandler((object sender, ElapsedEventArgs e) => this.Sweep());
		}

		internal void Start()
		{
			object obj = this._sync;
			Monitor.Enter(obj);
			try
			{
				this._sweepTimer.Enabled = this._clean;
				this._state = ServerState.Start;
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}

		internal void Stop(CloseEventArgs e, byte[] frameAsBytes, bool receive)
		{
			object obj = this._sync;
			Monitor.Enter(obj);
			try
			{
				this._state = ServerState.ShuttingDown;
				this._sweepTimer.Enabled = false;
				foreach (IWebSocketSession list in this._sessions.Values.ToList<IWebSocketSession>())
				{
					list.Context.WebSocket.Close(e, frameAsBytes, receive);
				}
				this._state = ServerState.Stop;
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}

		public void Sweep()
		{
			IWebSocketSession webSocketSession;
			if ((this._state != ServerState.Start || this._sweeping ? false : this.Count != 0))
			{
				object obj = this._forSweep;
				Monitor.Enter(obj);
				try
				{
					this._sweeping = true;
					foreach (string inactiveID in this.InactiveIDs)
					{
						if (this._state == ServerState.Start)
						{
							object obj1 = this._sync;
							Monitor.Enter(obj1);
							try
							{
								if (this._sessions.TryGetValue(inactiveID, out webSocketSession))
								{
									WebSocketState state = webSocketSession.State;
									if (state == WebSocketState.Open)
									{
										webSocketSession.Context.WebSocket.Close(CloseStatusCode.ProtocolError);
									}
									else if (state != WebSocketState.Closing)
									{
										this._sessions.Remove(inactiveID);
									}
									else
									{
										continue;
									}
								}
							}
							finally
							{
								Monitor.Exit(obj1);
							}
						}
						else
						{
							break;
						}
					}
					this._sweeping = false;
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
		}

		private bool tryGetSession(string id, out IWebSocketSession session)
		{
			bool flag;
			object obj = this._sync;
			Monitor.Enter(obj);
			try
			{
				flag = this._sessions.TryGetValue(id, out session);
			}
			finally
			{
				Monitor.Exit(obj);
			}
			if (!flag)
			{
				this._logger.Error(string.Concat("A session with the specified ID isn't found:\n  ID: ", id));
			}
			return flag;
		}

		public bool TryGetSession(string id, out IWebSocketSession session)
		{
			bool flag;
			string str = ((ServerState)this._state).CheckIfAvailable(false, true, false) ?? id.CheckIfValidSessionID();
			if (str == null)
			{
				flag = this.tryGetSession(id, out session);
			}
			else
			{
				this._logger.Error(str);
				session = null;
				flag = false;
			}
			return flag;
		}
	}
}