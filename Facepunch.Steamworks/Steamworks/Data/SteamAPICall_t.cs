using System;

namespace Steamworks.Data
{
	internal struct SteamAPICall_t : IEquatable<SteamAPICall_t>, IComparable<SteamAPICall_t>
	{
		public ulong Value;

		public int CompareTo(SteamAPICall_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((SteamAPICall_t)p);
		}

		public bool Equals(SteamAPICall_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(SteamAPICall_t a, SteamAPICall_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator SteamAPICall_t(ulong value)
		{
			return new SteamAPICall_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(SteamAPICall_t value)
		{
			return value.Value;
		}

		public static bool operator !=(SteamAPICall_t a, SteamAPICall_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}