using System;

namespace Steamworks.Data
{
	internal struct InputAnalogActionHandle_t : IEquatable<InputAnalogActionHandle_t>, IComparable<InputAnalogActionHandle_t>
	{
		public ulong Value;

		public int CompareTo(InputAnalogActionHandle_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((InputAnalogActionHandle_t)p);
		}

		public bool Equals(InputAnalogActionHandle_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(InputAnalogActionHandle_t a, InputAnalogActionHandle_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator InputAnalogActionHandle_t(ulong value)
		{
			return new InputAnalogActionHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(InputAnalogActionHandle_t value)
		{
			return value.Value;
		}

		public static bool operator !=(InputAnalogActionHandle_t a, InputAnalogActionHandle_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}