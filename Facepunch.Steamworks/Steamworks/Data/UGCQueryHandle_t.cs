using System;

namespace Steamworks.Data
{
	internal struct UGCQueryHandle_t : IEquatable<UGCQueryHandle_t>, IComparable<UGCQueryHandle_t>
	{
		public ulong Value;

		public int CompareTo(UGCQueryHandle_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((UGCQueryHandle_t)p);
		}

		public bool Equals(UGCQueryHandle_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(UGCQueryHandle_t a, UGCQueryHandle_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator UGCQueryHandle_t(ulong value)
		{
			return new UGCQueryHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(UGCQueryHandle_t value)
		{
			return value.Value;
		}

		public static bool operator !=(UGCQueryHandle_t a, UGCQueryHandle_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}