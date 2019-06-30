using System;

namespace Steamworks.Data
{
	internal struct DepotId_t : IEquatable<DepotId_t>, IComparable<DepotId_t>
	{
		public uint Value;

		public int CompareTo(DepotId_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((DepotId_t)p);
		}

		public bool Equals(DepotId_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(DepotId_t a, DepotId_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator DepotId_t(uint value)
		{
			return new DepotId_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(DepotId_t value)
		{
			return value.Value;
		}

		public static bool operator !=(DepotId_t a, DepotId_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}