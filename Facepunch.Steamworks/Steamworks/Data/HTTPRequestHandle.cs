using System;

namespace Steamworks.Data
{
	internal struct HTTPRequestHandle : IEquatable<HTTPRequestHandle>, IComparable<HTTPRequestHandle>
	{
		public uint Value;

		public int CompareTo(HTTPRequestHandle other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((HTTPRequestHandle)p);
		}

		public bool Equals(HTTPRequestHandle p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(HTTPRequestHandle a, HTTPRequestHandle b)
		{
			return a.Equals(b);
		}

		public static implicit operator HTTPRequestHandle(uint value)
		{
			return new HTTPRequestHandle()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(HTTPRequestHandle value)
		{
			return value.Value;
		}

		public static bool operator !=(HTTPRequestHandle a, HTTPRequestHandle b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}