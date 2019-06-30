using Facepunch;
using Network.Visibility;
using System;
using System.Runtime.CompilerServices;

namespace Network
{
	public abstract class Client : NetworkPeer
	{
		public Manager visibility;

		public static string disconnectReason;

		public string connectedAddress = "unset";

		public int connectedPort;

		public string ServerName;

		public bool IsOfficialServer;

		public Stats IncomingStats = new Stats();

		public IClientCallback callbackHandler;

		public Client.PlaybackStatsData PlaybackStats = new Client.PlaybackStatsData();

		public Network.Connection Connection
		{
			get;
			protected set;
		}

		public bool ConnectionAccepted
		{
			get;
			protected set;
		}

		public abstract bool IsPlaying
		{
			get;
		}

		public abstract bool IsRecording
		{
			get;
		}

		public abstract bool PlayingFinished
		{
			get;
		}

		protected Client()
		{
		}

		public virtual bool Connect(string strURL, int port)
		{
			this.ConnectionAccepted = false;
			Client.disconnectReason = "Disconnected";
			return true;
		}

		public Networkable CreateNetworkable(uint networkID, uint networkGroup)
		{
			Networkable networkable = Pool.Get<Networkable>();
			networkable.ID = networkID;
			networkable.SwitchGroup(this.visibility.Get(networkGroup));
			return networkable;
		}

		public abstract void Cycle();

		public void DestroyNetworkable(ref Networkable networkable)
		{
			networkable.Destroy();
			Pool.Free<Networkable>(ref networkable);
		}

		public abstract void Disconnect(string reason, bool sendReasonToServer = true);

		public abstract int GetAveragePing();

		public abstract int GetLastPing();

		public abstract int GetLowestPing();

		public abstract bool IsConnected();

		public abstract void ManualRecordPacket(byte packetId, byte[] data, int length);

		protected void OnDisconnected(string str)
		{
			if (this.callbackHandler != null)
			{
				this.callbackHandler.OnClientDisconnected(str);
			}
		}

		public void SetupNetworkable(Networkable net)
		{
			net.cl = this;
		}

		public abstract byte[] StartPlayback(string filename);

		public abstract bool StartRecording(string targetFilename, byte[] header);

		public abstract void StopPlayback();

		public abstract void StopRecording();

		public abstract void UpdatePlayback(long frameTime, long maxTime);

		public class PlaybackStatsData
		{
			public int Frames;

			public int Packets;

			public TimeSpan TotalTime;

			public TimeSpan DemoLength;

			public PlaybackStatsData()
			{
			}
		}
	}
}