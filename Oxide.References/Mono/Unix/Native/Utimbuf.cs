using System;

namespace Mono.Unix.Native
{
	[Map("struct utimbuf")]
	public struct Utimbuf : IEquatable<Utimbuf>
	{
		[time_t]
		public long actime;

		[time_t]
		public long modtime;

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != this.GetType())
			{
				return false;
			}
			Utimbuf utimbuf = (Utimbuf)obj;
			return (utimbuf.actime != this.actime ? false : utimbuf.modtime == this.modtime);
		}

		public bool Equals(Utimbuf value)
		{
			return (value.actime != this.actime ? false : value.modtime == this.modtime);
		}

		public override int GetHashCode()
		{
			return this.actime.GetHashCode() ^ this.modtime.GetHashCode();
		}

		public static bool operator ==(Utimbuf lhs, Utimbuf rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Utimbuf lhs, Utimbuf rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}