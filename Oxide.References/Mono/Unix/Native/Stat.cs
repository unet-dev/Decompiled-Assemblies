using System;

namespace Mono.Unix.Native
{
	[Map("struct stat")]
	public struct Stat : IEquatable<Stat>
	{
		[CLSCompliant(false)]
		[dev_t]
		public ulong st_dev;

		[CLSCompliant(false)]
		[ino_t]
		public ulong st_ino;

		[CLSCompliant(false)]
		public FilePermissions st_mode;

		[NonSerialized]
		private uint _padding_;

		[CLSCompliant(false)]
		[nlink_t]
		public ulong st_nlink;

		[CLSCompliant(false)]
		[uid_t]
		public uint st_uid;

		[CLSCompliant(false)]
		[gid_t]
		public uint st_gid;

		[CLSCompliant(false)]
		[dev_t]
		public ulong st_rdev;

		[off_t]
		public long st_size;

		[blksize_t]
		public long st_blksize;

		[blkcnt_t]
		public long st_blocks;

		[time_t]
		public long st_atime;

		[time_t]
		public long st_mtime;

		[time_t]
		public long st_ctime;

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != this.GetType())
			{
				return false;
			}
			Stat stat = (Stat)obj;
			return (stat.st_dev != this.st_dev || stat.st_ino != this.st_ino || stat.st_mode != this.st_mode || stat.st_nlink != this.st_nlink || stat.st_uid != this.st_uid || stat.st_gid != this.st_gid || stat.st_rdev != this.st_rdev || stat.st_size != this.st_size || stat.st_blksize != this.st_blksize || stat.st_blocks != this.st_blocks || stat.st_atime != this.st_atime || stat.st_mtime != this.st_mtime ? false : stat.st_ctime == this.st_ctime);
		}

		public bool Equals(Stat value)
		{
			return (value.st_dev != this.st_dev || value.st_ino != this.st_ino || value.st_mode != this.st_mode || value.st_nlink != this.st_nlink || value.st_uid != this.st_uid || value.st_gid != this.st_gid || value.st_rdev != this.st_rdev || value.st_size != this.st_size || value.st_blksize != this.st_blksize || value.st_blocks != this.st_blocks || value.st_atime != this.st_atime || value.st_mtime != this.st_mtime ? false : value.st_ctime == this.st_ctime);
		}

		public override int GetHashCode()
		{
			return this.st_dev.GetHashCode() ^ this.st_ino.GetHashCode() ^ this.st_mode.GetHashCode() ^ this.st_nlink.GetHashCode() ^ this.st_uid.GetHashCode() ^ this.st_gid.GetHashCode() ^ this.st_rdev.GetHashCode() ^ this.st_size.GetHashCode() ^ this.st_blksize.GetHashCode() ^ this.st_blocks.GetHashCode() ^ this.st_atime.GetHashCode() ^ this.st_mtime.GetHashCode() ^ this.st_ctime.GetHashCode();
		}

		public static bool operator ==(Stat lhs, Stat rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Stat lhs, Stat rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}