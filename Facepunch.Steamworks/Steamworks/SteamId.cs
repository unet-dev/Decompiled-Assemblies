using System;

namespace Steamworks
{
	public struct SteamId
	{
		public ulong Value;

		public uint AccountId
		{
			get
			{
				return (uint)(this.Value & (ulong)-1);
			}
		}

		public static implicit operator SteamId(ulong value)
		{
			return new SteamId()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(SteamId value)
		{
			return value.Value;
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}