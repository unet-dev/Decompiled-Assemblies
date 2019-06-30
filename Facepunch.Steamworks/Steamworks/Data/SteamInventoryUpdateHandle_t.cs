using System;

namespace Steamworks.Data
{
	internal struct SteamInventoryUpdateHandle_t : IEquatable<SteamInventoryUpdateHandle_t>, IComparable<SteamInventoryUpdateHandle_t>
	{
		public ulong Value;

		public int CompareTo(SteamInventoryUpdateHandle_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((SteamInventoryUpdateHandle_t)p);
		}

		public bool Equals(SteamInventoryUpdateHandle_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(SteamInventoryUpdateHandle_t a, SteamInventoryUpdateHandle_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator SteamInventoryUpdateHandle_t(ulong value)
		{
			return new SteamInventoryUpdateHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(SteamInventoryUpdateHandle_t value)
		{
			return value.Value;
		}

		public static bool operator !=(SteamInventoryUpdateHandle_t a, SteamInventoryUpdateHandle_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}