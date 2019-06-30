using System;

namespace Steamworks.Data
{
	internal struct UGCFileWriteStreamHandle_t : IEquatable<UGCFileWriteStreamHandle_t>, IComparable<UGCFileWriteStreamHandle_t>
	{
		public ulong Value;

		public int CompareTo(UGCFileWriteStreamHandle_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((UGCFileWriteStreamHandle_t)p);
		}

		public bool Equals(UGCFileWriteStreamHandle_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(UGCFileWriteStreamHandle_t a, UGCFileWriteStreamHandle_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator UGCFileWriteStreamHandle_t(ulong value)
		{
			return new UGCFileWriteStreamHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(UGCFileWriteStreamHandle_t value)
		{
			return value.Value;
		}

		public static bool operator !=(UGCFileWriteStreamHandle_t a, UGCFileWriteStreamHandle_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}