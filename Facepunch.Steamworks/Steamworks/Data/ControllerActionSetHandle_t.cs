using System;

namespace Steamworks.Data
{
	internal struct ControllerActionSetHandle_t : IEquatable<ControllerActionSetHandle_t>, IComparable<ControllerActionSetHandle_t>
	{
		public ulong Value;

		public int CompareTo(ControllerActionSetHandle_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((ControllerActionSetHandle_t)p);
		}

		public bool Equals(ControllerActionSetHandle_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(ControllerActionSetHandle_t a, ControllerActionSetHandle_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator ControllerActionSetHandle_t(ulong value)
		{
			return new ControllerActionSetHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(ControllerActionSetHandle_t value)
		{
			return value.Value;
		}

		public static bool operator !=(ControllerActionSetHandle_t a, ControllerActionSetHandle_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}