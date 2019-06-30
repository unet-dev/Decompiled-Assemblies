using System;

namespace Steamworks.Data
{
	internal struct ControllerDigitalActionHandle_t : IEquatable<ControllerDigitalActionHandle_t>, IComparable<ControllerDigitalActionHandle_t>
	{
		public ulong Value;

		public int CompareTo(ControllerDigitalActionHandle_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((ControllerDigitalActionHandle_t)p);
		}

		public bool Equals(ControllerDigitalActionHandle_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(ControllerDigitalActionHandle_t a, ControllerDigitalActionHandle_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator ControllerDigitalActionHandle_t(ulong value)
		{
			return new ControllerDigitalActionHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(ControllerDigitalActionHandle_t value)
		{
			return value.Value;
		}

		public static bool operator !=(ControllerDigitalActionHandle_t a, ControllerDigitalActionHandle_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}