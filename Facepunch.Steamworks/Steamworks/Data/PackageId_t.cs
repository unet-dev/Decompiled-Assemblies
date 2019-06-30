using System;

namespace Steamworks.Data
{
	internal struct PackageId_t : IEquatable<PackageId_t>, IComparable<PackageId_t>
	{
		public uint Value;

		public int CompareTo(PackageId_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((PackageId_t)p);
		}

		public bool Equals(PackageId_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(PackageId_t a, PackageId_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator PackageId_t(uint value)
		{
			return new PackageId_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(PackageId_t value)
		{
			return value.Value;
		}

		public static bool operator !=(PackageId_t a, PackageId_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}