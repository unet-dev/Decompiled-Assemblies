using System;

namespace Steamworks.Data
{
	internal struct RTime32 : IEquatable<RTime32>, IComparable<RTime32>
	{
		public uint Value;

		public int CompareTo(RTime32 other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((RTime32)p);
		}

		public bool Equals(RTime32 p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(RTime32 a, RTime32 b)
		{
			return a.Equals(b);
		}

		public static implicit operator RTime32(uint value)
		{
			return new RTime32()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(RTime32 value)
		{
			return value.Value;
		}

		public static bool operator !=(RTime32 a, RTime32 b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}