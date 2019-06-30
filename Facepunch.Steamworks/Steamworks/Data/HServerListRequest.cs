using System;

namespace Steamworks.Data
{
	internal struct HServerListRequest : IEquatable<HServerListRequest>, IComparable<HServerListRequest>
	{
		public IntPtr Value;

		public int CompareTo(HServerListRequest other)
		{
			long num = this.Value.ToInt64();
			return num.CompareTo(other.Value.ToInt64());
		}

		public override bool Equals(object p)
		{
			return this.Equals((HServerListRequest)p);
		}

		public bool Equals(HServerListRequest p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(HServerListRequest a, HServerListRequest b)
		{
			return a.Equals(b);
		}

		public static implicit operator HServerListRequest(IntPtr value)
		{
			return new HServerListRequest()
			{
				Value = value
			};
		}

		public static implicit operator IntPtr(HServerListRequest value)
		{
			return value.Value;
		}

		public static bool operator !=(HServerListRequest a, HServerListRequest b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}