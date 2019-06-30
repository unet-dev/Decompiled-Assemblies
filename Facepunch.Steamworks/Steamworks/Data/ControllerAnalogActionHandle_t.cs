using System;

namespace Steamworks.Data
{
	internal struct ControllerAnalogActionHandle_t : IEquatable<ControllerAnalogActionHandle_t>, IComparable<ControllerAnalogActionHandle_t>
	{
		public ulong Value;

		public int CompareTo(ControllerAnalogActionHandle_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((ControllerAnalogActionHandle_t)p);
		}

		public bool Equals(ControllerAnalogActionHandle_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(ControllerAnalogActionHandle_t a, ControllerAnalogActionHandle_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator ControllerAnalogActionHandle_t(ulong value)
		{
			return new ControllerAnalogActionHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(ControllerAnalogActionHandle_t value)
		{
			return value.Value;
		}

		public static bool operator !=(ControllerAnalogActionHandle_t a, ControllerAnalogActionHandle_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}