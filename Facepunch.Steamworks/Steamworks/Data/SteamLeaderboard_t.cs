using System;

namespace Steamworks.Data
{
	internal struct SteamLeaderboard_t : IEquatable<SteamLeaderboard_t>, IComparable<SteamLeaderboard_t>
	{
		public ulong Value;

		public int CompareTo(SteamLeaderboard_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((SteamLeaderboard_t)p);
		}

		public bool Equals(SteamLeaderboard_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(SteamLeaderboard_t a, SteamLeaderboard_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator SteamLeaderboard_t(ulong value)
		{
			return new SteamLeaderboard_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(SteamLeaderboard_t value)
		{
			return value.Value;
		}

		public static bool operator !=(SteamLeaderboard_t a, SteamLeaderboard_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}