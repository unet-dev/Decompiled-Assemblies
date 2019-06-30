using System;

namespace Steamworks.Data
{
	internal struct ManifestId_t : IEquatable<ManifestId_t>, IComparable<ManifestId_t>
	{
		public ulong Value;

		public int CompareTo(ManifestId_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((ManifestId_t)p);
		}

		public bool Equals(ManifestId_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(ManifestId_t a, ManifestId_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator ManifestId_t(ulong value)
		{
			return new ManifestId_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(ManifestId_t value)
		{
			return value.Value;
		}

		public static bool operator !=(ManifestId_t a, ManifestId_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}