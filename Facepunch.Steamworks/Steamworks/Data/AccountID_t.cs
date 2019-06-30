using System;

namespace Steamworks.Data
{
	internal struct AccountID_t : IEquatable<AccountID_t>, IComparable<AccountID_t>
	{
		public uint Value;

		public int CompareTo(AccountID_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((AccountID_t)p);
		}

		public bool Equals(AccountID_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(AccountID_t a, AccountID_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator AccountID_t(uint value)
		{
			return new AccountID_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(AccountID_t value)
		{
			return value.Value;
		}

		public static bool operator !=(AccountID_t a, AccountID_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}