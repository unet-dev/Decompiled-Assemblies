using System;

namespace Mono.Unix.Native
{
	[Map("struct timespec")]
	public struct Timespec : IEquatable<Timespec>
	{
		[time_t]
		public long tv_sec;

		public long tv_nsec;

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != this.GetType())
			{
				return false;
			}
			Timespec timespec = (Timespec)obj;
			return (timespec.tv_sec != this.tv_sec ? false : timespec.tv_nsec == this.tv_nsec);
		}

		public bool Equals(Timespec value)
		{
			return (value.tv_sec != this.tv_sec ? false : value.tv_nsec == this.tv_nsec);
		}

		public override int GetHashCode()
		{
			return this.tv_sec.GetHashCode() ^ this.tv_nsec.GetHashCode();
		}

		public static bool operator ==(Timespec lhs, Timespec rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Timespec lhs, Timespec rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}