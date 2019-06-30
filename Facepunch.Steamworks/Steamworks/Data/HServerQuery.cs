using System;

namespace Steamworks.Data
{
	internal struct HServerQuery : IEquatable<HServerQuery>, IComparable<HServerQuery>
	{
		public int Value;

		public int CompareTo(HServerQuery other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((HServerQuery)p);
		}

		public bool Equals(HServerQuery p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(HServerQuery a, HServerQuery b)
		{
			return a.Equals(b);
		}

		public static implicit operator HServerQuery(int value)
		{
			return new HServerQuery()
			{
				Value = value
			};
		}

		public static implicit operator Int32(HServerQuery value)
		{
			return value.Value;
		}

		public static bool operator !=(HServerQuery a, HServerQuery b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}