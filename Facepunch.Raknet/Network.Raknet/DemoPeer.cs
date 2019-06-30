using Network;
using System;

namespace Facepunch.Network.Raknet
{
	internal class DemoPeer : Peer
	{
		public unsafe byte* Data;

		public int Length;

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
				return this.Length * 8;
			}
		}

		public override int incomingBitsUnread
		{
			get
			{
				return (this.Length - this.Position) * 8;
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

		protected override void Check()
		{
			base.Check();
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

		public override IntPtr RawData()
		{
			return (IntPtr)this.Data;
		}

		protected override unsafe bool Read(byte* data, int length)
		{
			if (this.Position + length > this.Length)
			{
				return false;
			}
			Buffer.MemoryCopy(this.Data + this.Position, data, (long)length, (long)length);
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