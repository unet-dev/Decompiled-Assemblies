using System;

namespace Steamworks.Data
{
	internal struct HSteamUser : IEquatable<HSteamUser>, IComparable<HSteamUser>
	{
		public int Value;

		public int CompareTo(HSteamUser other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((HSteamUser)p);
		}

		public bool Equals(HSteamUser p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(HSteamUser a, HSteamUser b)
		{
			return a.Equals(b);
		}

		public static implicit operator HSteamUser(int value)
		{
			return new HSteamUser()
			{
				Value = value
			};
		}

		public static implicit operator Int32(HSteamUser value)
		{
			return value.Value;
		}

		public static bool operator !=(HSteamUser a, HSteamUser b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}