using System;

namespace Steamworks.Data
{
	internal struct InputHandle_t : IEquatable<InputHandle_t>, IComparable<InputHandle_t>
	{
		public ulong Value;

		public int CompareTo(InputHandle_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((InputHandle_t)p);
		}

		public bool Equals(InputHandle_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(InputHandle_t a, InputHandle_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator InputHandle_t(ulong value)
		{
			return new InputHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(InputHandle_t value)
		{
			return value.Value;
		}

		public static bool operator !=(InputHandle_t a, InputHandle_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}