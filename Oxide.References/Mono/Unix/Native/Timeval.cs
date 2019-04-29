using System;

namespace Mono.Unix.Native
{
	[Map("struct timeval")]
	public struct Timeval : IEquatable<Timeval>
	{
		[time_t]
		public long tv_sec;

		[suseconds_t]
		public long tv_usec;

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != this.GetType())
			{
				return false;
			}
			Timeval timeval = (Timeval)obj;
			return (timeval.tv_sec != this.tv_sec ? false : timeval.tv_usec == this.tv_usec);
		}

		public bool Equals(Timeval value)
		{
			return (value.tv_sec != this.tv_sec ? false : value.tv_usec == this.tv_usec);
		}

		public override int GetHashCode()
		{
			return this.tv_sec.GetHashCode() ^ this.tv_usec.GetHashCode();
		}

		public static bool operator ==(Timeval lhs, Timeval rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Timeval lhs, Timeval rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}