using System;

namespace Steamworks.Data
{
	internal struct SteamLeaderboardEntries_t : IEquatable<SteamLeaderboardEntries_t>, IComparable<SteamLeaderboardEntries_t>
	{
		public ulong Value;

		public int CompareTo(SteamLeaderboardEntries_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((SteamLeaderboardEntries_t)p);
		}

		public bool Equals(SteamLeaderboardEntries_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(SteamLeaderboardEntries_t a, SteamLeaderboardEntries_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator SteamLeaderboardEntries_t(ulong value)
		{
			return new SteamLeaderboardEntries_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(SteamLeaderboardEntries_t value)
		{
			return value.Value;
		}

		public static bool operator !=(SteamLeaderboardEntries_t a, SteamLeaderboardEntries_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}