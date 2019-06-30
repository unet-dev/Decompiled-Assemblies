using System;

namespace Steamworks.Data
{
	internal struct BREAKPAD_HANDLE : IEquatable<BREAKPAD_HANDLE>, IComparable<BREAKPAD_HANDLE>
	{
		public IntPtr Value;

		public int CompareTo(BREAKPAD_HANDLE other)
		{
			long num = this.Value.ToInt64();
			return num.CompareTo(other.Value.ToInt64());
		}

		public override bool Equals(object p)
		{
			return this.Equals((BREAKPAD_HANDLE)p);
		}

		public bool Equals(BREAKPAD_HANDLE p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(BREAKPAD_HANDLE a, BREAKPAD_HANDLE b)
		{
			return a.Equals(b);
		}

		public static implicit operator BREAKPAD_HANDLE(IntPtr value)
		{
			return new BREAKPAD_HANDLE()
			{
				Value = value
			};
		}

		public static implicit operator IntPtr(BREAKPAD_HANDLE value)
		{
			return value.Value;
		}

		public static bool operator !=(BREAKPAD_HANDLE a, BREAKPAD_HANDLE b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}