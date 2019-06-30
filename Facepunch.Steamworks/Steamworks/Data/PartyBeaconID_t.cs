using System;

namespace Steamworks.Data
{
	internal struct PartyBeaconID_t : IEquatable<PartyBeaconID_t>, IComparable<PartyBeaconID_t>
	{
		public ulong Value;

		public int CompareTo(PartyBeaconID_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((PartyBeaconID_t)p);
		}

		public bool Equals(PartyBeaconID_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(PartyBeaconID_t a, PartyBeaconID_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator PartyBeaconID_t(ulong value)
		{
			return new PartyBeaconID_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(PartyBeaconID_t value)
		{
			return value.Value;
		}

		public static bool operator !=(PartyBeaconID_t a, PartyBeaconID_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}