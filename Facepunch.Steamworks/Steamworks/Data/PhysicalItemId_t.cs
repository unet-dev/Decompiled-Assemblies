using System;

namespace Steamworks.Data
{
	internal struct PhysicalItemId_t : IEquatable<PhysicalItemId_t>, IComparable<PhysicalItemId_t>
	{
		public uint Value;

		public int CompareTo(PhysicalItemId_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((PhysicalItemId_t)p);
		}

		public bool Equals(PhysicalItemId_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(PhysicalItemId_t a, PhysicalItemId_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator PhysicalItemId_t(uint value)
		{
			return new PhysicalItemId_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(PhysicalItemId_t value)
		{
			return value.Value;
		}

		public static bool operator !=(PhysicalItemId_t a, PhysicalItemId_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}