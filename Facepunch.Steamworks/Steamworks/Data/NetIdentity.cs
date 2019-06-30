using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Explicit)]
	public struct NetIdentity
	{
		[FieldOffset(0)]
		internal NetIdentity.IdentityType type;

		[FieldOffset(4)]
		internal int m_cbSize;

		[FieldOffset(8)]
		internal SteamId steamID;

		public static implicit operator NetIdentity(SteamId value)
		{
			NetIdentity netIdentity = new NetIdentity()
			{
				steamID = value,
				type = NetIdentity.IdentityType.SteamID,
				m_cbSize = 8
			};
			return netIdentity;
		}

		public static implicit operator SteamId(NetIdentity value)
		{
			return value.steamID;
		}

		public override string ToString()
		{
			return String.Format("{0};{1};{2}", this.type, this.m_cbSize, this.steamID);
		}

		internal enum IdentityType
		{
			Invalid = 0,
			IPAddress = 1,
			GenericString = 2,
			GenericBytes = 3,
			SteamID = 16
		}
	}
}