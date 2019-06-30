using System;

namespace Steamworks.Data
{
	internal struct TxnID_t : IEquatable<TxnID_t>, IComparable<TxnID_t>
	{
		public GID_t Value;

		public int CompareTo(TxnID_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((TxnID_t)p);
		}

		public bool Equals(TxnID_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(TxnID_t a, TxnID_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator TxnID_t(GID_t value)
		{
			return new TxnID_t()
			{
				Value = value
			};
		}

		public static implicit operator GID_t(TxnID_t value)
		{
			return value.Value;
		}

		public static bool operator !=(TxnID_t a, TxnID_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}