using System;

namespace Steamworks.Data
{
	internal struct PartnerId_t : IEquatable<PartnerId_t>, IComparable<PartnerId_t>
	{
		public uint Value;

		public int CompareTo(PartnerId_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((PartnerId_t)p);
		}

		public bool Equals(PartnerId_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(PartnerId_t a, PartnerId_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator PartnerId_t(uint value)
		{
			return new PartnerId_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(PartnerId_t value)
		{
			return value.Value;
		}

		public static bool operator !=(PartnerId_t a, PartnerId_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}