using System;

namespace Mono.Unix.Native
{
	public sealed class Dirent : IEquatable<Dirent>
	{
		[CLSCompliant(false)]
		public ulong d_ino;

		public long d_off;

		[CLSCompliant(false)]
		public ushort d_reclen;

		public byte d_type;

		public string d_name;

		public Dirent()
		{
		}

		public override bool Equals(object obj)
		{
			if (obj == null || this.GetType() != obj.GetType())
			{
				return false;
			}
			return this.Equals((Dirent)obj);
		}

		public bool Equals(Dirent value)
		{
			if (value == null)
			{
				return false;
			}
			return (value.d_ino != this.d_ino || value.d_off != this.d_off || value.d_reclen != this.d_reclen || value.d_type != this.d_type ? false : value.d_name == this.d_name);
		}

		public override int GetHashCode()
		{
			return this.d_ino.GetHashCode() ^ this.d_off.GetHashCode() ^ this.d_reclen.GetHashCode() ^ this.d_type.GetHashCode() ^ this.d_name.GetHashCode();
		}

		public static bool operator ==(Dirent lhs, Dirent rhs)
		{
			return object.Equals(lhs, rhs);
		}

		public static bool operator !=(Dirent lhs, Dirent rhs)
		{
			return !object.Equals(lhs, rhs);
		}

		public override string ToString()
		{
			return this.d_name;
		}
	}
}