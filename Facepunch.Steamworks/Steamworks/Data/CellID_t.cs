using System;

namespace Steamworks.Data
{
	internal struct CellID_t : IEquatable<CellID_t>, IComparable<CellID_t>
	{
		public uint Value;

		public int CompareTo(CellID_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((CellID_t)p);
		}

		public bool Equals(CellID_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(CellID_t a, CellID_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator CellID_t(uint value)
		{
			return new CellID_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(CellID_t value)
		{
			return value.Value;
		}

		public static bool operator !=(CellID_t a, CellID_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}