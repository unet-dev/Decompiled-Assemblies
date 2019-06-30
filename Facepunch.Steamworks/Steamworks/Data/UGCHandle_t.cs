using System;

namespace Steamworks.Data
{
	internal struct UGCHandle_t : IEquatable<UGCHandle_t>, IComparable<UGCHandle_t>
	{
		public ulong Value;

		public int CompareTo(UGCHandle_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((UGCHandle_t)p);
		}

		public bool Equals(UGCHandle_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(UGCHandle_t a, UGCHandle_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator UGCHandle_t(ulong value)
		{
			return new UGCHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(UGCHandle_t value)
		{
			return value.Value;
		}

		public static bool operator !=(UGCHandle_t a, UGCHandle_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}