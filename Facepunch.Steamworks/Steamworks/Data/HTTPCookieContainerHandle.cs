using System;

namespace Steamworks.Data
{
	internal struct HTTPCookieContainerHandle : IEquatable<HTTPCookieContainerHandle>, IComparable<HTTPCookieContainerHandle>
	{
		public uint Value;

		public int CompareTo(HTTPCookieContainerHandle other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((HTTPCookieContainerHandle)p);
		}

		public bool Equals(HTTPCookieContainerHandle p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(HTTPCookieContainerHandle a, HTTPCookieContainerHandle b)
		{
			return a.Equals(b);
		}

		public static implicit operator HTTPCookieContainerHandle(uint value)
		{
			return new HTTPCookieContainerHandle()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(HTTPCookieContainerHandle value)
		{
			return value.Value;
		}

		public static bool operator !=(HTTPCookieContainerHandle a, HTTPCookieContainerHandle b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}