using System;

namespace Steamworks.Data
{
	internal struct InputActionSetHandle_t : IEquatable<InputActionSetHandle_t>, IComparable<InputActionSetHandle_t>
	{
		public ulong Value;

		public int CompareTo(InputActionSetHandle_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((InputActionSetHandle_t)p);
		}

		public bool Equals(InputActionSetHandle_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(InputActionSetHandle_t a, InputActionSetHandle_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator InputActionSetHandle_t(ulong value)
		{
			return new InputActionSetHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(InputActionSetHandle_t value)
		{
			return value.Value;
		}

		public static bool operator !=(InputActionSetHandle_t a, InputActionSetHandle_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}