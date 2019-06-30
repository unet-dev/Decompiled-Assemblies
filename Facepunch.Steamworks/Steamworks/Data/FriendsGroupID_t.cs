using System;

namespace Steamworks.Data
{
	internal struct FriendsGroupID_t : IEquatable<FriendsGroupID_t>, IComparable<FriendsGroupID_t>
	{
		public short Value;

		public int CompareTo(FriendsGroupID_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((FriendsGroupID_t)p);
		}

		public bool Equals(FriendsGroupID_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(FriendsGroupID_t a, FriendsGroupID_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator FriendsGroupID_t(short value)
		{
			return new FriendsGroupID_t()
			{
				Value = value
			};
		}

		public static implicit operator Int16(FriendsGroupID_t value)
		{
			return value.Value;
		}

		public static bool operator !=(FriendsGroupID_t a, FriendsGroupID_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}