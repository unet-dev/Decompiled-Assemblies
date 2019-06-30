using System;

namespace Steamworks.Data
{
	internal struct BundleId_t : IEquatable<BundleId_t>, IComparable<BundleId_t>
	{
		public uint Value;

		public int CompareTo(BundleId_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((BundleId_t)p);
		}

		public bool Equals(BundleId_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(BundleId_t a, BundleId_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator BundleId_t(uint value)
		{
			return new BundleId_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(BundleId_t value)
		{
			return value.Value;
		}

		public static bool operator !=(BundleId_t a, BundleId_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}