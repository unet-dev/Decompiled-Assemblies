using System;

namespace Mono.Unix.Native
{
	public sealed class Utsname : IEquatable<Utsname>
	{
		public string sysname;

		public string nodename;

		public string release;

		public string version;

		public string machine;

		public string domainname;

		public Utsname()
		{
		}

		public override bool Equals(object obj)
		{
			if (obj == null || this.GetType() != obj.GetType())
			{
				return false;
			}
			return this.Equals((Utsname)obj);
		}

		public bool Equals(Utsname value)
		{
			return (!(value.sysname == this.sysname) || !(value.nodename == this.nodename) || !(value.release == this.release) || !(value.version == this.version) || !(value.machine == this.machine) ? false : value.domainname == this.domainname);
		}

		public override int GetHashCode()
		{
			return this.sysname.GetHashCode() ^ this.nodename.GetHashCode() ^ this.release.GetHashCode() ^ this.version.GetHashCode() ^ this.machine.GetHashCode() ^ this.domainname.GetHashCode();
		}

		public static bool operator ==(Utsname lhs, Utsname rhs)
		{
			return object.Equals(lhs, rhs);
		}

		public static bool operator !=(Utsname lhs, Utsname rhs)
		{
			return !object.Equals(lhs, rhs);
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2} {3} {4}", new object[] { this.sysname, this.nodename, this.release, this.version, this.machine });
		}
	}
}