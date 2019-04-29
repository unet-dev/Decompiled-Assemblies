using System;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	[Map]
	public struct Statvfs : IEquatable<Statvfs>
	{
		public ulong f_bsize;

		public ulong f_frsize;

		[fsblkcnt_t]
		public ulong f_blocks;

		[fsblkcnt_t]
		public ulong f_bfree;

		[fsblkcnt_t]
		public ulong f_bavail;

		[fsfilcnt_t]
		public ulong f_files;

		[fsfilcnt_t]
		public ulong f_ffree;

		[fsfilcnt_t]
		public ulong f_favail;

		public ulong f_fsid;

		public MountFlags f_flag;

		public ulong f_namemax;

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != this.GetType())
			{
				return false;
			}
			Statvfs statvf = (Statvfs)obj;
			return (statvf.f_bsize != this.f_bsize || statvf.f_frsize != this.f_frsize || statvf.f_blocks != this.f_blocks || statvf.f_bfree != this.f_bfree || statvf.f_bavail != this.f_bavail || statvf.f_files != this.f_files || statvf.f_ffree != this.f_ffree || statvf.f_favail != this.f_favail || statvf.f_fsid != this.f_fsid || statvf.f_flag != this.f_flag ? false : statvf.f_namemax == this.f_namemax);
		}

		public bool Equals(Statvfs value)
		{
			return (value.f_bsize != this.f_bsize || value.f_frsize != this.f_frsize || value.f_blocks != this.f_blocks || value.f_bfree != this.f_bfree || value.f_bavail != this.f_bavail || value.f_files != this.f_files || value.f_ffree != this.f_ffree || value.f_favail != this.f_favail || value.f_fsid != this.f_fsid || value.f_flag != this.f_flag ? false : value.f_namemax == this.f_namemax);
		}

		public override int GetHashCode()
		{
			return this.f_bsize.GetHashCode() ^ this.f_frsize.GetHashCode() ^ this.f_blocks.GetHashCode() ^ this.f_bfree.GetHashCode() ^ this.f_bavail.GetHashCode() ^ this.f_files.GetHashCode() ^ this.f_ffree.GetHashCode() ^ this.f_favail.GetHashCode() ^ this.f_fsid.GetHashCode() ^ this.f_flag.GetHashCode() ^ this.f_namemax.GetHashCode();
		}

		public static bool operator ==(Statvfs lhs, Statvfs rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Statvfs lhs, Statvfs rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}