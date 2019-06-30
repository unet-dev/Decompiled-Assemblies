using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Steamworks
{
	public class SocketInterface
	{
		public List<Connection> Connecting = new List<Connection>();

		public List<Connection> Connected = new List<Connection>();

		public Steamworks.Data.Socket Socket
		{
			get;
			internal set;
		}

		public SocketInterface()
		{
		}

		public bool Close()
		{
			return this.Socket.Close();
		}

		public virtual void OnConnected(Connection connection, ConnectionInfo data)
		{
			this.Connecting.Remove(connection);
			this.Connected.Add(connection);
		}

		public virtual void OnConnecting(Connection connection, ConnectionInfo data)
		{
			connection.Accept();
			this.Connecting.Add(connection);
		}

		public virtual void OnConnectionChanged(Connection connection, ConnectionInfo data)
		{
			switch (data.State)
			{
				case ConnectionState.None:
				case ConnectionState.ClosedByPeer:
				case ConnectionState.ProblemDetectedLocally:
				{
					this.OnDisconnected(connection, data);
					return;
				}
				case ConnectionState.Connecting:
				{
					this.OnConnecting(connection, data);
					return;
				}
				case ConnectionState.FindingRoute:
				{
					return;
				}
				case ConnectionState.Connected:
				{
					this.OnConnected(connection, data);
					return;
				}
				default:
				{
					return;
				}
			}
		}

		public virtual void OnDisconnected(Connection connection, ConnectionInfo data)
		{
			connection.Close(false, 0, "Closing Connection");
			this.Connecting.Remove(connection);
			this.Connected.Remove(connection);
		}

		public virtual void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
		{
		}

		public void Receive(int bufferSize = 32)
		{
			int num = 0;
			IntPtr intPtr = Marshal.AllocHGlobal(8 * bufferSize);
			try
			{
				num = SteamNetworkingSockets.Internal.ReceiveMessagesOnListenSocket(this.Socket, intPtr, bufferSize);
				for (int i = 0; i < num; i++)
				{
					this.ReceiveMessage(Marshal.ReadIntPtr(intPtr, i * 8));
				}
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			if (num == bufferSize)
			{
				this.Receive(bufferSize);
			}
		}

		internal void ReceiveMessage(IntPtr msgPtr)
		{
			NetMsg structure = Marshal.PtrToStructure<NetMsg>(msgPtr);
			try
			{
				this.OnMessage(structure.Connection, structure.Identity, structure.DataPtr, structure.DataSize, structure.RecvTime, structure.MessageNumber, structure.Channel);
			}
			finally
			{
				structure.Release(msgPtr);
			}
		}

		public override string ToString()
		{
			return this.Socket.ToString();
		}
	}
}