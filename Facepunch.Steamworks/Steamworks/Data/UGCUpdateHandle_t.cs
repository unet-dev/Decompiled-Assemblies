using System;

namespace Steamworks.Data
{
	internal struct UGCUpdateHandle_t : IEquatable<UGCUpdateHandle_t>, IComparable<UGCUpdateHandle_t>
	{
		public ulong Value;

		public int CompareTo(UGCUpdateHandle_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((UGCUpdateHandle_t)p);
		}

		public bool Equals(UGCUpdateHandle_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(UGCUpdateHandle_t a, UGCUpdateHandle_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator UGCUpdateHandle_t(ulong value)
		{
			return new UGCUpdateHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(UGCUpdateHandle_t value)
		{
			return value.Value;
		}

		public static bool operator !=(UGCUpdateHandle_t a, UGCUpdateHandle_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}