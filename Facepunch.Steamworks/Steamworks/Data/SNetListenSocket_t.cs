using System;

namespace Steamworks.Data
{
	internal struct SNetListenSocket_t : IEquatable<SNetListenSocket_t>, IComparable<SNetListenSocket_t>
	{
		public uint Value;

		public int CompareTo(SNetListenSocket_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((SNetListenSocket_t)p);
		}

		public bool Equals(SNetListenSocket_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(SNetListenSocket_t a, SNetListenSocket_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator SNetListenSocket_t(uint value)
		{
			return new SNetListenSocket_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(SNetListenSocket_t value)
		{
			return value.Value;
		}

		public static bool operator !=(SNetListenSocket_t a, SNetListenSocket_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}