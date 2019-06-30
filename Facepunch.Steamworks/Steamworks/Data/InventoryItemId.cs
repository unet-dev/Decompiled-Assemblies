using System;

namespace Steamworks.Data
{
	public struct InventoryItemId : IEquatable<InventoryItemId>, IComparable<InventoryItemId>
	{
		public ulong Value;

		public int CompareTo(InventoryItemId other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((InventoryItemId)p);
		}

		public bool Equals(InventoryItemId p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(InventoryItemId a, InventoryItemId b)
		{
			return a.Equals(b);
		}

		public static implicit operator InventoryItemId(ulong value)
		{
			return new InventoryItemId()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(InventoryItemId value)
		{
			return value.Value;
		}

		public static bool operator !=(InventoryItemId a, InventoryItemId b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}