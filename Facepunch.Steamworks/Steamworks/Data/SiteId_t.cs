using System;

namespace Steamworks.Data
{
	internal struct SiteId_t : IEquatable<SiteId_t>, IComparable<SiteId_t>
	{
		public ulong Value;

		public int CompareTo(SiteId_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((SiteId_t)p);
		}

		public bool Equals(SiteId_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(SiteId_t a, SiteId_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator SiteId_t(ulong value)
		{
			return new SiteId_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(SiteId_t value)
		{
			return value.Value;
		}

		public static bool operator !=(SiteId_t a, SiteId_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}