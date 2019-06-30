using System;

namespace Steamworks.Data
{
	internal struct HAuthTicket : IEquatable<HAuthTicket>, IComparable<HAuthTicket>
	{
		public uint Value;

		public int CompareTo(HAuthTicket other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((HAuthTicket)p);
		}

		public bool Equals(HAuthTicket p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(HAuthTicket a, HAuthTicket b)
		{
			return a.Equals(b);
		}

		public static implicit operator HAuthTicket(uint value)
		{
			return new HAuthTicket()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(HAuthTicket value)
		{
			return value.Value;
		}

		public static bool operator !=(HAuthTicket a, HAuthTicket b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}