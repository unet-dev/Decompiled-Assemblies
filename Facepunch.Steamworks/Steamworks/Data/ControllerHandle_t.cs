using System;

namespace Steamworks.Data
{
	internal struct ControllerHandle_t : IEquatable<ControllerHandle_t>, IComparable<ControllerHandle_t>
	{
		public ulong Value;

		public int CompareTo(ControllerHandle_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((ControllerHandle_t)p);
		}

		public bool Equals(ControllerHandle_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(ControllerHandle_t a, ControllerHandle_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator ControllerHandle_t(ulong value)
		{
			return new ControllerHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(ControllerHandle_t value)
		{
			return value.Value;
		}

		public static bool operator !=(ControllerHandle_t a, ControllerHandle_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}