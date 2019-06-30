using System;

namespace Steamworks.Data
{
	internal struct ScreenshotHandle : IEquatable<ScreenshotHandle>, IComparable<ScreenshotHandle>
	{
		public uint Value;

		public int CompareTo(ScreenshotHandle other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((ScreenshotHandle)p);
		}

		public bool Equals(ScreenshotHandle p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(ScreenshotHandle a, ScreenshotHandle b)
		{
			return a.Equals(b);
		}

		public static implicit operator ScreenshotHandle(uint value)
		{
			return new ScreenshotHandle()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(ScreenshotHandle value)
		{
			return value.Value;
		}

		public static bool operator !=(ScreenshotHandle a, ScreenshotHandle b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}