using System;
using System.Text;

namespace Mono.Unix.Native
{
	public sealed class Group : IEquatable<Group>
	{
		public string gr_name;

		public string gr_passwd;

		[CLSCompliant(false)]
		public uint gr_gid;

		public string[] gr_mem;

		public Group()
		{
		}

		public override bool Equals(object obj)
		{
			if (obj == null || this.GetType() != obj.GetType())
			{
				return false;
			}
			return this.Equals((Group)obj);
		}

		public bool Equals(Group value)
		{
			if (value == null)
			{
				return false;
			}
			if (value.gr_gid != this.gr_gid)
			{
				return false;
			}
			if (value.gr_gid != this.gr_gid || !(value.gr_name == this.gr_name) || !(value.gr_passwd == this.gr_passwd))
			{
				return false;
			}
			if (value.gr_mem == this.gr_mem)
			{
				return true;
			}
			if (value.gr_mem == null || this.gr_mem == null)
			{
				return false;
			}
			if ((int)value.gr_mem.Length != (int)this.gr_mem.Length)
			{
				return false;
			}
			for (int i = 0; i < (int)this.gr_mem.Length; i++)
			{
				if (this.gr_mem[i] != value.gr_mem[i])
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			for (int i = 0; i < (int)this.gr_mem.Length; i++)
			{
				hashCode ^= this.gr_mem[i].GetHashCode();
			}
			return this.gr_name.GetHashCode() ^ this.gr_passwd.GetHashCode() ^ this.gr_gid.GetHashCode() ^ hashCode;
		}

		private static void GetMembers(StringBuilder sb, string[] members)
		{
			if ((int)members.Length > 0)
			{
				sb.Append(members[0]);
			}
			for (int i = 1; i < (int)members.Length; i++)
			{
				sb.Append(",");
				sb.Append(members[i]);
			}
		}

		public static bool operator ==(Group lhs, Group rhs)
		{
			return object.Equals(lhs, rhs);
		}

		public static bool operator !=(Group lhs, Group rhs)
		{
			return !object.Equals(lhs, rhs);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.gr_name).Append(":").Append(this.gr_passwd).Append(":");
			stringBuilder.Append(this.gr_gid).Append(":");
			Group.GetMembers(stringBuilder, this.gr_mem);
			return stringBuilder.ToString();
		}
	}
}