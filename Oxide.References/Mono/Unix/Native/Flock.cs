using System;

namespace Mono.Unix.Native
{
	[Map("struct flock")]
	public struct Flock : IEquatable<Flock>
	{
		[CLSCompliant(false)]
		public LockType l_type;

		[CLSCompliant(false)]
		public SeekFlags l_whence;

		[off_t]
		public long l_start;

		[off_t]
		public long l_len;

		[pid_t]
		public int l_pid;

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != this.GetType())
			{
				return false;
			}
			Flock flock = (Flock)obj;
			return (this.l_type != flock.l_type || this.l_whence != flock.l_whence || this.l_start != flock.l_start || this.l_len != flock.l_len ? false : this.l_pid == flock.l_pid);
		}

		public bool Equals(Flock value)
		{
			return (this.l_type != value.l_type || this.l_whence != value.l_whence || this.l_start != value.l_start || this.l_len != value.l_len ? false : this.l_pid == value.l_pid);
		}

		public override int GetHashCode()
		{
			return this.l_type.GetHashCode() ^ this.l_whence.GetHashCode() ^ this.l_start.GetHashCode() ^ this.l_len.GetHashCode() ^ this.l_pid.GetHashCode();
		}

		public static bool operator ==(Flock lhs, Flock rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Flock lhs, Flock rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}