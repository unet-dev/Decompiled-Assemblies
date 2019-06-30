using System;

namespace Steamworks.Data
{
	internal struct HHTMLBrowser : IEquatable<HHTMLBrowser>, IComparable<HHTMLBrowser>
	{
		public uint Value;

		public int CompareTo(HHTMLBrowser other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((HHTMLBrowser)p);
		}

		public bool Equals(HHTMLBrowser p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(HHTMLBrowser a, HHTMLBrowser b)
		{
			return a.Equals(b);
		}

		public static implicit operator HHTMLBrowser(uint value)
		{
			return new HHTMLBrowser()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(HHTMLBrowser value)
		{
			return value.Value;
		}

		public static bool operator !=(HHTMLBrowser a, HHTMLBrowser b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}