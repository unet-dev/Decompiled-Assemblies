using System;

namespace Steamworks.Data
{
	internal struct HSteamPipe : IEquatable<HSteamPipe>, IComparable<HSteamPipe>
	{
		public int Value;

		public int CompareTo(HSteamPipe other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((HSteamPipe)p);
		}

		public bool Equals(HSteamPipe p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(HSteamPipe a, HSteamPipe b)
		{
			return a.Equals(b);
		}

		public static implicit operator HSteamPipe(int value)
		{
			return new HSteamPipe()
			{
				Value = value
			};
		}

		public static implicit operator Int32(HSteamPipe value)
		{
			return value.Value;
		}

		public static bool operator !=(HSteamPipe a, HSteamPipe b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}