using System;

namespace Steamworks.Data
{
	public struct SteamNetworkingPOPID
	{
		public uint Value;

		public static implicit operator SteamNetworkingPOPID(uint value)
		{
			return new SteamNetworkingPOPID()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(SteamNetworkingPOPID value)
		{
			return value.Value;
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}