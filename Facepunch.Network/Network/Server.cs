using Facepunch;
using Network.Visibility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
	public abstract class Server : NetworkPeer
	{
		public string ip = "";

		public int port = 5678;

		public bool compressionEnabled;

		public bool logging;

		public Manager visibility;

		public IServerCallback callbackHandler;

		public bool debug;

		internal uint lastValueGiven = 1024;

		public List<Connection> connections = new List<Connection>();

		private Dictionary<ulong, Connection> connectionByGUID = new Dictionary<ulong, Connection>();

		protected Server()
		{
		}

		public Networkable CreateNetworkable()
		{
			Networkable networkable = Pool.Get<Networkable>();
			networkable.ID = this.TakeUID();
			networkable.sv = this;
			return networkable;
		}

		public Networkable CreateNetworkable(uint uid)
		{
			Networkable networkable = Pool.Get<Networkable>();
			networkable.ID = uid;
			networkable.sv = this;
			if (uid > this.lastValueGiven)
			{
				this.lastValueGiven = uid;
			}
			return networkable;
		}

		public virtual void Cycle()
		{
		}

		public void DestroyNetworkable(ref Networkable networkable)
		{
			networkable.Destroy();
			Pool.Free<Networkable>(ref networkable);
		}

		protected Connection FindConnection(ulong guid)
		{
			Connection connection;
			if (this.connectionByGUID.TryGetValue(guid, out connection))
			{
				return connection;
			}
			return null;
		}

		public abstract int GetAveragePing(Connection connection);

		public abstract bool IsConnected();

		public abstract void Kick(Connection cn, string message);

		protected void OnDisconnected(string strReason, Connection cn)
		{
			if (cn == null)
			{
				return;
			}
			cn.connected = false;
			cn.active = false;
			if (this.callbackHandler != null)
			{
				this.callbackHandler.OnDisconnected(strReason, cn);
			}
			this.RemoveConnection(cn);
		}

		protected abstract void OnNewConnection();

		protected void OnNewConnection(Connection connection)
		{
			connection.connectionTime = DateTime.Now;
			this.connections.Add(connection);
			this.connectionByGUID.Add(connection.guid, connection);
			if (this.write.Start())
			{
				this.write.PacketID(Message.Type.RequestUserInformation);
				this.write.Send(new SendInfo(connection));
			}
		}

		protected void RemoveConnection(Connection connection)
		{
			this.connectionByGUID.Remove(connection.guid);
			this.connections.Remove(connection);
			connection.OnDisconnected();
		}

		public void Reset()
		{
			this.ResetUIDs();
		}

		internal void ResetUIDs()
		{
			this.lastValueGiven = 1024;
		}

		public void ReturnUID(uint uid)
		{
		}

		public abstract void SendUnconnected(uint netAddr, ushort netPort, byte[] steamResponseBuffer, int packetSize);

		public virtual bool Start()
		{
			return true;
		}

		public virtual void Stop(string shutdownMsg)
		{
		}

		public uint TakeUID()
		{
			if (this.lastValueGiven > -33)
			{
				Debug.LogError(string.Concat("TakeUID - hitting ceiling limit!", this.lastValueGiven));
			}
			this.lastValueGiven++;
			return this.lastValueGiven;
		}
	}
}