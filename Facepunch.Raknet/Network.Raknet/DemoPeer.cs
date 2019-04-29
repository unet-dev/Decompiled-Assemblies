using Network;
using System;
using System.Runtime.InteropServices;

namespace Facepunch.Network.Raknet
{
	internal class DemoPeer : Peer
	{
		public byte[] Packet;

		public int Position;

		public override uint incomingAddressInt
		{
			get
			{
				return (uint)0;
			}
		}

		public override int incomingBits
		{
			get
			{
				return (int)this.Packet.Length * 8;
			}
		}

		public override int incomingBitsUnread
		{
			get
			{
				return ((int)this.Packet.Length - this.Position) * 8;
			}
		}

		public override ulong incomingGUID
		{
			get
			{
				return (ulong)0;
			}
		}

		public override uint incomingPort
		{
			get
			{
				return (uint)0;
			}
		}

		public DemoPeer()
		{
		}

		public override int GetPingAverage(ulong guid)
		{
			return 1;
		}

		public override int GetPingLast(ulong guid)
		{
			return 1;
		}

		public override int GetPingLowest(ulong guid)
		{
			return 1;
		}

		public override ulong GetStat(Connection connection, NetworkPeer.StatTypeLong type)
		{
			return (ulong)1;
		}

		public override Native.RaknetStats GetStatistics(ulong guid)
		{
			return new Native.RaknetStats();
		}

		public override string GetStatisticsString(ulong guid)
		{
			return string.Empty;
		}

		protected override unsafe bool Read(byte* data, int length)
		{
			if (this.Position + length > (int)this.Packet.Length)
			{
				return false;
			}
			Marshal.Copy(this.Packet, this.Position, (IntPtr)data, length);
			this.Position += length;
			return true;
		}

		public override uint SendBroadcast(Priority priority, SendMethod reliability, sbyte channel)
		{
			return (uint)0;
		}

		public override void SendStart()
		{
		}

		public override uint SendTo(ulong guid, Priority priority, SendMethod reliability, sbyte channel)
		{
			return (uint)0;
		}

		public override void SetReadPos(int bitsOffset)
		{
			this.Position = bitsOffset * 8;
		}

		protected override unsafe void Write(byte* data, int length)
		{
		}
	}
}