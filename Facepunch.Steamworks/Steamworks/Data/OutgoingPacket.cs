using System;
using System.Runtime.CompilerServices;

namespace Steamworks.Data
{
	public struct OutgoingPacket
	{
		public uint Address
		{
			get;
			internal set;
		}

		public byte[] Data
		{
			get;
			internal set;
		}

		public ushort Port
		{
			get;
			internal set;
		}

		public int Size
		{
			get;
			internal set;
		}
	}
}