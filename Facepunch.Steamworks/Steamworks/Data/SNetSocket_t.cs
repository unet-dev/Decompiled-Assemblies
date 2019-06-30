using System;

namespace Steamworks.Data
{
	internal struct SNetSocket_t : IEquatable<SNetSocket_t>, IComparable<SNetSocket_t>
	{
		public uint Value;

		public int CompareTo(SNetSocket_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((SNetSocket_t)p);
		}

		public bool Equals(SNetSocket_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(SNetSocket_t a, SNetSocket_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator SNetSocket_t(uint value)
		{
			return new SNetSocket_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(SNetSocket_t value)
		{
			return value.Value;
		}

		public static bool operator !=(SNetSocket_t a, SNetSocket_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}