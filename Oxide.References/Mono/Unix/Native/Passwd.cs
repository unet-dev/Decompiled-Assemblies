using System;

namespace Mono.Unix.Native
{
	public sealed class Passwd : IEquatable<Passwd>
	{
		public string pw_name;

		public string pw_passwd;

		[CLSCompliant(false)]
		public uint pw_uid;

		[CLSCompliant(false)]
		public uint pw_gid;

		public string pw_gecos;

		public string pw_dir;

		public string pw_shell;

		public Passwd()
		{
		}

		public override bool Equals(object obj)
		{
			if (obj == null || this.GetType() != obj.GetType())
			{
				return false;
			}
			return this.Equals((Passwd)obj);
		}

		public bool Equals(Passwd value)
		{
			if (value == null)
			{
				return false;
			}
			return (value.pw_uid != this.pw_uid || value.pw_gid != this.pw_gid || !(value.pw_name == this.pw_name) || !(value.pw_passwd == this.pw_passwd) || !(value.pw_gecos == this.pw_gecos) || !(value.pw_dir == this.pw_dir) ? false : value.pw_shell == this.pw_shell);
		}

		public override int GetHashCode()
		{
			return this.pw_name.GetHashCode() ^ this.pw_passwd.GetHashCode() ^ this.pw_uid.GetHashCode() ^ this.pw_gid.GetHashCode() ^ this.pw_gecos.GetHashCode() ^ this.pw_dir.GetHashCode() ^ this.pw_dir.GetHashCode() ^ this.pw_shell.GetHashCode();
		}

		public static bool operator ==(Passwd lhs, Passwd rhs)
		{
			return object.Equals(lhs, rhs);
		}

		public static bool operator !=(Passwd lhs, Passwd rhs)
		{
			return !object.Equals(lhs, rhs);
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[] { this.pw_name, this.pw_passwd, this.pw_uid, this.pw_gid, this.pw_gecos, this.pw_dir, this.pw_shell });
		}
	}
}