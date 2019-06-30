using Steamworks.Data;
using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	public class ConnectionInterface
	{
		public Steamworks.Data.Connection Connection;

		public bool Connected = false;

		public bool Connecting = true;

		public string ConnectionName
		{
			get
			{
				return this.Connection.ConnectionName;
			}
			set
			{
				this.Connection.ConnectionName = value;
			}
		}

		public long UserData
		{
			get
			{
				return this.Connection.UserData;
			}
			set
			{
				this.Connection.UserData = value;
			}
		}

		public ConnectionInterface()
		{
		}

		public void Close()
		{
			this.Connection.Close(false, 0, "Closing Connection");
		}

		public virtual void OnConnected(ConnectionInfo data)
		{
			this.Connected = true;
			this.Connecting = false;
		}

		public virtual void OnConnecting(ConnectionInfo data)
		{
			this.Connecting = true;
		}

		public virtual void OnConnectionChanged(ConnectionInfo data)
		{
			switch (data.State)
			{
				case ConnectionState.None:
				case ConnectionState.ClosedByPeer:
				case ConnectionState.ProblemDetectedLocally:
				{
					this.OnDisconnected(data);
					return;
				}
				case ConnectionState.Connecting:
				{
					this.OnConnecting(data);
					return;
				}
				case ConnectionState.FindingRoute:
				{
					return;
				}
				case ConnectionState.Connected:
				{
					this.OnConnected(data);
					return;
				}
				default:
				{
					return;
				}
			}
		}

		public virtual void OnDisconnected(ConnectionInfo data)
		{
			this.Connected = false;
			this.Connecting = false;
		}

		public virtual void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
		{
		}

		public void Receive(int bufferSize = 32)
		{
			int num = 0;
			IntPtr intPtr = Marshal.AllocHGlobal(8 * bufferSize);
			try
			{
				num = SteamNetworkingSockets.Internal.ReceiveMessagesOnConnection(this.Connection, intPtr, bufferSize);
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
				this.OnMessage(structure.DataPtr, structure.DataSize, structure.RecvTime, structure.MessageNumber, structure.Channel);
			}
			finally
			{
				structure.Release(msgPtr);
			}
		}

		public override string ToString()
		{
			return this.Connection.ToString();
		}
	}
}