using System;

namespace Steamworks.Data
{
	internal struct GID_t : IEquatable<GID_t>, IComparable<GID_t>
	{
		public ulong Value;

		public int CompareTo(GID_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((GID_t)p);
		}

		public bool Equals(GID_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(GID_t a, GID_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator GID_t(ulong value)
		{
			return new GID_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(GID_t value)
		{
			return value.Value;
		}

		public static bool operator !=(GID_t a, GID_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}