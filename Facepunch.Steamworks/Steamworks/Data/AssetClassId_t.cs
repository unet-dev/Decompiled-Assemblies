using System;

namespace Steamworks.Data
{
	internal struct AssetClassId_t : IEquatable<AssetClassId_t>, IComparable<AssetClassId_t>
	{
		public ulong Value;

		public int CompareTo(AssetClassId_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((AssetClassId_t)p);
		}

		public bool Equals(AssetClassId_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(AssetClassId_t a, AssetClassId_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator AssetClassId_t(ulong value)
		{
			return new AssetClassId_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(AssetClassId_t value)
		{
			return value.Value;
		}

		public static bool operator !=(AssetClassId_t a, AssetClassId_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}