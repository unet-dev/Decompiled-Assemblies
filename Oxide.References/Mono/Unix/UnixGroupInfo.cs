using Mono.Unix.Native;
using System;
using System.Collections;
using System.Threading;

namespace Mono.Unix
{
	public sealed class UnixGroupInfo
	{
		private Group @group;

		public long GroupId
		{
			get
			{
				return (long)this.@group.gr_gid;
			}
		}

		public string GroupName
		{
			get
			{
				return this.@group.gr_name;
			}
		}

		public string Password
		{
			get
			{
				return this.@group.gr_passwd;
			}
		}

		public UnixGroupInfo(string group)
		{
			Group group1;
			this.@group = new Group();
			if (Syscall.getgrnam_r(group, this.@group, out group1) != 0 || group1 == null)
			{
				throw new ArgumentException(Locale.GetText("invalid group name"), "group");
			}
		}

		public UnixGroupInfo(long group)
		{
			Group group1;
			this.@group = new Group();
			if (Syscall.getgrgid_r(Convert.ToUInt32(group), this.@group, out group1) != 0 || group1 == null)
			{
				throw new ArgumentException(Locale.GetText("invalid group id"), "group");
			}
		}

		public UnixGroupInfo(Group group)
		{
			this.@group = UnixGroupInfo.CopyGroup(group);
		}

		private static Group CopyGroup(Group group)
		{
			Group group1 = new Group()
			{
				gr_gid = group.gr_gid,
				gr_mem = group.gr_mem,
				gr_name = group.gr_name,
				gr_passwd = group.gr_passwd
			};
			return group1;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || this.GetType() != obj.GetType())
			{
				return false;
			}
			return this.@group.Equals(((UnixGroupInfo)obj).@group);
		}

		public override int GetHashCode()
		{
			return this.@group.GetHashCode();
		}

		public static UnixGroupInfo[] GetLocalGroups()
		{
			ArrayList arrayLists = new ArrayList();
			object grpLock = Syscall.grp_lock;
			Monitor.Enter(grpLock);
			try
			{
				if (Syscall.setgrent() != 0)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
				try
				{
					while (true)
					{
						Group group = Syscall.getgrent();
						Group group1 = group;
						if (group == null)
						{
							break;
						}
						arrayLists.Add(new UnixGroupInfo(group1));
					}
					if ((int)Stdlib.GetLastError() != 0)
					{
						UnixMarshal.ThrowExceptionForLastError();
					}
				}
				finally
				{
					Syscall.endgrent();
				}
			}
			finally
			{
				Monitor.Exit(grpLock);
			}
			return (UnixGroupInfo[])arrayLists.ToArray(typeof(UnixGroupInfo));
		}

		public string[] GetMemberNames()
		{
			return (string[])this.@group.gr_mem.Clone();
		}

		public UnixUserInfo[] GetMembers()
		{
			ArrayList arrayLists = new ArrayList((int)this.@group.gr_mem.Length);
			for (int i = 0; i < (int)this.@group.gr_mem.Length; i++)
			{
				try
				{
					arrayLists.Add(new UnixUserInfo(this.@group.gr_mem[i]));
				}
				catch (ArgumentException argumentException)
				{
				}
			}
			return (UnixUserInfo[])arrayLists.ToArray(typeof(UnixUserInfo));
		}

		public Group ToGroup()
		{
			return UnixGroupInfo.CopyGroup(this.@group);
		}

		public override string ToString()
		{
			return this.@group.ToString();
		}
	}
}