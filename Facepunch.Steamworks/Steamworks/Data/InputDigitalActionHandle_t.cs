using System;

namespace Steamworks.Data
{
	internal struct InputDigitalActionHandle_t : IEquatable<InputDigitalActionHandle_t>, IComparable<InputDigitalActionHandle_t>
	{
		public ulong Value;

		public int CompareTo(InputDigitalActionHandle_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((InputDigitalActionHandle_t)p);
		}

		public bool Equals(InputDigitalActionHandle_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(InputDigitalActionHandle_t a, InputDigitalActionHandle_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator InputDigitalActionHandle_t(ulong value)
		{
			return new InputDigitalActionHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(InputDigitalActionHandle_t value)
		{
			return value.Value;
		}

		public static bool operator !=(InputDigitalActionHandle_t a, InputDigitalActionHandle_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}