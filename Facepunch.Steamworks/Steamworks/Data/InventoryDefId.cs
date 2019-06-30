using System;

namespace Steamworks.Data
{
	public struct InventoryDefId : IEquatable<InventoryDefId>, IComparable<InventoryDefId>
	{
		public int Value;

		public int CompareTo(InventoryDefId other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((InventoryDefId)p);
		}

		public bool Equals(InventoryDefId p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(InventoryDefId a, InventoryDefId b)
		{
			return a.Equals(b);
		}

		public static implicit operator InventoryDefId(int value)
		{
			return new InventoryDefId()
			{
				Value = value
			};
		}

		public static implicit operator Int32(InventoryDefId value)
		{
			return value.Value;
		}

		public static bool operator !=(InventoryDefId a, InventoryDefId b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}