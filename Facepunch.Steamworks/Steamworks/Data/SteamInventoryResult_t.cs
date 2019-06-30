using System;

namespace Steamworks.Data
{
	internal struct SteamInventoryResult_t : IEquatable<SteamInventoryResult_t>, IComparable<SteamInventoryResult_t>
	{
		public int Value;

		public int CompareTo(SteamInventoryResult_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((SteamInventoryResult_t)p);
		}

		public bool Equals(SteamInventoryResult_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(SteamInventoryResult_t a, SteamInventoryResult_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator SteamInventoryResult_t(int value)
		{
			return new SteamInventoryResult_t()
			{
				Value = value
			};
		}

		public static implicit operator Int32(SteamInventoryResult_t value)
		{
			return value.Value;
		}

		public static bool operator !=(SteamInventoryResult_t a, SteamInventoryResult_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}