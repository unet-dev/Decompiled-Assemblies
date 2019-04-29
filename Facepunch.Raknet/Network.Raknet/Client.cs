using Facepunch;
using Network;
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Facepunch.Network.Raknet
{
	public class Client : Network.Client
	{
		public static float MaxReceiveTime;

		public const string DemoHeader = "RUST DEMO FORMAT";

		private Peer peer;

		public static byte[] ReusableBytes;

		private Stopwatch cycleTimer = Stopwatch.StartNew();

		protected FileStream recordStream;

		protected BinaryWriter recordWriter;

		protected Stopwatch recordTime;

		protected FileStream playbackStream;

		protected BinaryReader playbackReader;

		protected long playbackTime;

		protected Stopwatch playbackTimer;

		private long lastPlayedPacketTime;

		public override bool IsPlaying
		{
			get
			{
				return this.playbackStream != null;
			}
		}

		public override bool IsRecording
		{
			get
			{
				return this.recordStream != null;
			}
		}

		public override bool PlayingFinished
		{
			get
			{
				return this.playbackStream.Position >= this.playbackStream.Length;
			}
		}

		static Client()
		{
			Facepunch.Network.Raknet.Client.MaxReceiveTime = 10f;
			Facepunch.Network.Raknet.Client.ReusableBytes = new byte[1048576];
		}

		public Client()
		{
		}

		public override bool Connect(string strURL, int port)
		{
			base.Connect(strURL, port);
			this.peer = Peer.CreateConnection(strURL, port, 12, 400, 0);
			if (this.peer == null)
			{
				return false;
			}
			this.write = new StreamWrite(this, this.peer);
			this.read = new StreamRead(this, this.peer);
			this.connectedAddress = strURL;
			this.connectedPort = port;
			this.ServerName = "";
			base.Connection = new Network.Connection();
			return true;
		}

		public override void Cycle()
		{
			using (TimeWarning timeWarning = TimeWarning.New("Raknet.Client.Cycle", 0.1f))
			{
				if (!this.IsPlaying)
				{
					if (this.IsConnected())
					{
						this.cycleTimer.Reset();
						this.cycleTimer.Start();
						while (this.peer.Receive())
						{
							using (TimeWarning timeWarning1 = TimeWarning.New("HandleMessage", 0.1f))
							{
								this.HandleMessage();
							}
							if (this.cycleTimer.Elapsed.TotalMilliseconds <= (double)Facepunch.Network.Raknet.Client.MaxReceiveTime)
							{
								if (this.IsConnected())
								{
									continue;
								}
								return;
							}
							else
							{
								return;
							}
						}
					}
				}
			}
		}

		public override void Disconnect(string reason, bool sendReasonToServer)
		{
			if (sendReasonToServer && this.write != null && this.write.Start())
			{
				this.write.PacketID(Message.Type.DisconnectReason);
				this.write.String(reason);
				Write write = this.write;
				SendInfo sendInfo = new SendInfo(base.Connection)
				{
					method = SendMethod.ReliableUnordered,
					priority = Priority.Immediate
				};
				write.Send(sendInfo);
			}
			if (this.peer != null)
			{
				this.peer.Close();
				this.peer = null;
			}
			this.write = null;
			this.read = null;
			this.connectedAddress = "";
			this.connectedPort = 0;
			base.Connection = null;
			base.OnDisconnected(reason);
		}

		public override int GetAveragePing()
		{
			if (base.Connection == null)
			{
				return 1;
			}
			return this.peer.GetPingAverage(base.Connection.guid);
		}

		public override string GetDebug(Network.Connection connection)
		{
			if (this.peer == null)
			{
				return "";
			}
			if (connection == null)
			{
				return this.peer.GetStatisticsString((ulong)0);
			}
			return this.peer.GetStatisticsString(connection.guid);
		}

		public override int GetLastPing()
		{
			if (base.Connection == null)
			{
				return 1;
			}
			return this.peer.GetPingLast(base.Connection.guid);
		}

		public override int GetLowestPing()
		{
			if (base.Connection == null)
			{
				return 1;
			}
			return this.peer.GetPingLowest(base.Connection.guid);
		}

		public override ulong GetStat(Network.Connection connection, NetworkPeer.StatTypeLong type)
		{
			if (this.peer == null)
			{
				return (ulong)0;
			}
			return this.peer.GetStat(connection, type);
		}

		protected void HandleMessage()
		{
			this.read.Start(base.Connection);
			if (this.IsRecording)
			{
				int num = this.read.unread;
				int num1 = this.read.position;
				this.read.Position = (long)0;
				this.read.Read(Facepunch.Network.Raknet.Client.ReusableBytes, 0, num);
				this.read.Position = (long)num1;
				this.recordWriter.Write(num);
				this.recordWriter.Write((long)this.recordTime.Elapsed.TotalMilliseconds);
				this.recordWriter.Write(Facepunch.Network.Raknet.Client.ReusableBytes, 0, num);
				this.recordWriter.Write('\0');
				this.recordWriter.Write('\0');
			}
			byte num2 = this.read.PacketID();
			if (this.HandleRaknetPacket(num2))
			{
				return;
			}
			num2 = (byte)(num2 - 140);
			if (!this.IsPlaying && this.peer.incomingGUID != base.Connection.guid)
			{
				this.IncomingStats.Add("Error", "WrongGuid", this.read.Length);
				return;
			}
			if (!this.IsPlaying && base.Connection == null)
			{
				UnityEngine.Debug.LogWarning(string.Concat(new object[] { "[CLIENT] Ignoring message ", (Message.Type)num2, " ", num2, " clientConnection is null" }));
				return;
			}
			if (num2 > 23)
			{
				UnityEngine.Debug.LogWarning(string.Concat("Invalid Packet (higher than ", Message.Type.EntityFlags, ")"));
				this.Disconnect(string.Concat(new object[] { "Invalid Packet (", num2, ") ", this.peer.incomingBytes, "b" }), true);
				return;
			}
			Message message = base.StartMessage((Message.Type)num2, base.Connection);
			if (this.callbackHandler != null)
			{
				try
				{
					using (TimeWarning timeWarning = TimeWarning.New("OnMessage", 0.1f))
					{
						this.callbackHandler.OnNetworkMessage(message);
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					UnityEngine.Debug.LogException(exception);
					if (!this.IsPlaying)
					{
						this.Disconnect(string.Concat(exception.Message, "\n", exception.StackTrace), true);
					}
				}
			}
			message.Clear();
			Pool.Free<Message>(ref message);
		}

		internal bool HandleRaknetPacket(byte type)
		{
			object[] objArray;
			if (type >= 140)
			{
				return false;
			}
			if (this.IsPlaying)
			{
				return true;
			}
			switch (type)
			{
				case 16:
				{
					base.ConnectionAccepted = true;
					if (base.Connection.guid != 0)
					{
						Console.WriteLine("Multiple PacketType.CONNECTION_REQUEST_ACCEPTED");
					}
					base.Connection.guid = this.peer.incomingGUID;
					this.IncomingStats.Add("Unconnected", "RequestAccepted", this.read.Length);
					return true;
				}
				case 17:
				{
					this.Disconnect("Connection Attempt Failed", false);
					return true;
				}
				case 18:
				case 19:
				{
					this.IncomingStats.Add("Unconnected", "Unhandled", this.read.Length);
					if (base.Connection == null || this.peer.incomingGUID == base.Connection.guid)
					{
						UnityEngine.Debug.LogWarning(string.Concat("Unhandled Raknet packet ", type));
						return true;
					}
					objArray = new object[] { "[CLIENT] Unhandled Raknet packet ", type, " from unknown source ", this.peer.incomingAddress };
					UnityEngine.Debug.LogWarning(string.Concat(objArray));
					return true;
				}
				case 20:
				{
					this.Disconnect("Server is Full", false);
					return true;
				}
				case 21:
				{
					if (base.Connection != null && base.Connection.guid != this.peer.incomingGUID)
					{
						return true;
					}
					this.Disconnect(Network.Client.disconnectReason, false);
					return true;
				}
				case 22:
				{
					if (base.Connection == null && base.Connection.guid != this.peer.incomingGUID)
					{
						return true;
					}
					this.Disconnect("Timed Out", false);
					return true;
				}
				case 23:
				{
					if (base.Connection == null && base.Connection.guid != this.peer.incomingGUID)
					{
						return true;
					}
					this.Disconnect("Connection Banned", false);
					return true;
				}
				default:
				{
					this.IncomingStats.Add("Unconnected", "Unhandled", this.read.Length);
					if (base.Connection == null || this.peer.incomingGUID == base.Connection.guid)
					{
						UnityEngine.Debug.LogWarning(string.Concat("Unhandled Raknet packet ", type));
						return true;
					}
					objArray = new object[] { "[CLIENT] Unhandled Raknet packet ", type, " from unknown source ", this.peer.incomingAddress };
					UnityEngine.Debug.LogWarning(string.Concat(objArray));
					return true;
				}
			}
		}

		public override bool IsConnected()
		{
			return this.peer != null;
		}

		public override void ManualRecordPacket(byte packetId, byte[] data, int length)
		{
			if (!this.IsRecording)
			{
				return;
			}
			this.recordWriter.Write(length + 1);
			this.recordWriter.Write((long)this.recordTime.Elapsed.TotalMilliseconds);
			this.recordWriter.Write((byte)(packetId + 140));
			this.recordWriter.Write(data, 0, length);
			this.recordWriter.Write('\0');
			this.recordWriter.Write('\0');
		}

		private bool PlaybackPacket()
		{
			if (this.PlayingFinished)
			{
				return false;
			}
			long position = this.playbackReader.BaseStream.Position;
			int num = this.playbackReader.ReadInt32();
			long num1 = this.playbackReader.ReadInt64();
			if (num1 > this.playbackTime)
			{
				this.playbackReader.BaseStream.Position = position;
				return false;
			}
			byte[] numArray = this.playbackReader.ReadBytes(num);
			if (this.playbackReader.ReadChar() != 0)
			{
				UnityEngine.Debug.LogWarning("Invalid sequence in demo");
				this.StopPlayback();
				return false;
			}
			if (this.playbackReader.ReadChar() != 0)
			{
				UnityEngine.Debug.LogWarning("Invalid sequence in demo");
				this.StopPlayback();
				return false;
			}
			DemoPeer demoPeer = (DemoPeer)this.peer;
			try
			{
				this.PlaybackStats.Packets++;
				this.lastPlayedPacketTime = num1;
				demoPeer.Packet = numArray;
				demoPeer.SetReadPos(0);
				this.HandleMessage();
			}
			finally
			{
				demoPeer.Packet = null;
			}
			return this.IsPlaying;
		}

		public override byte[] StartPlayback(string filename)
		{
			this.playbackStream = new FileStream(filename, FileMode.Open);
			if (this.playbackStream == null)
			{
				return null;
			}
			this.playbackReader = new BinaryReader(this.playbackStream);
			if (this.playbackReader.ReadString() != "RUST DEMO FORMAT")
			{
				UnityEngine.Debug.LogWarning("Demo has invalid header #1");
				this.StopPlayback();
				return null;
			}
			int num = this.playbackReader.ReadInt32();
			byte[] numArray = this.playbackReader.ReadBytes(num);
			this.playbackTime = (long)0;
			if (this.playbackReader.ReadChar() != 0)
			{
				UnityEngine.Debug.LogWarning("Demo has invalid header #2");
				this.StopPlayback();
				return null;
			}
			this.peer = new DemoPeer();
			this.write = new StreamWrite(this, this.peer);
			this.read = new StreamRead(this, this.peer);
			this.PlaybackStats = new Network.Client.PlaybackStatsData();
			this.playbackTimer = Stopwatch.StartNew();
			return numArray;
		}

		public override bool StartRecording(string targetFilename, byte[] header)
		{
			if (this.recordStream != null)
			{
				return false;
			}
			this.recordStream = new FileStream(targetFilename, FileMode.Create);
			this.recordWriter = new BinaryWriter(this.recordStream);
			this.recordTime = Stopwatch.StartNew();
			this.recordWriter.Write("RUST DEMO FORMAT");
			this.recordWriter.Write((int)header.Length);
			this.recordWriter.Write(header);
			this.recordWriter.Write('\0');
			return true;
		}

		public override void StopPlayback()
		{
			if (this.playbackReader != null)
			{
				this.playbackReader.Close();
				this.playbackReader = null;
			}
			if (this.playbackStream != null)
			{
				this.playbackStream.Dispose();
				this.playbackStream = null;
			}
			this.peer = null;
			this.write = null;
			this.read = null;
			this.PlaybackStats.DemoLength = TimeSpan.FromMilliseconds((double)this.lastPlayedPacketTime);
			this.PlaybackStats.TotalTime = this.playbackTimer.Elapsed;
		}

		public override void StopRecording()
		{
			if (this.recordStream == null)
			{
				return;
			}
			this.recordTime = null;
			this.recordWriter.Close();
			this.recordWriter = null;
			this.recordStream.Dispose();
			this.recordStream = null;
		}

		public override void UpdatePlayback(long frameTime, long maxTime)
		{
			if (this.PlayingFinished)
			{
				return;
			}
			this.PlaybackStats.Frames++;
			this.playbackTime += frameTime;
			Stopwatch stopwatch = Stopwatch.StartNew();
			while (this.PlaybackPacket())
			{
				if (stopwatch.Elapsed.TotalMilliseconds <= (double)maxTime)
				{
					continue;
				}
				this.playbackTime = this.lastPlayedPacketTime;
				return;
			}
		}
	}
}