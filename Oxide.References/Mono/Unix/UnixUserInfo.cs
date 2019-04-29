using Mono.Unix.Native;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace Mono.Unix
{
	public sealed class UnixUserInfo
	{
		private Passwd passwd;

		public UnixGroupInfo Group
		{
			get
			{
				return new UnixGroupInfo((long)this.passwd.pw_gid);
			}
		}

		public long GroupId
		{
			get
			{
				return (long)this.passwd.pw_gid;
			}
		}

		public string GroupName
		{
			get
			{
				return this.Group.GroupName;
			}
		}

		public string HomeDirectory
		{
			get
			{
				return this.passwd.pw_dir;
			}
		}

		public string Password
		{
			get
			{
				return this.passwd.pw_passwd;
			}
		}

		public string RealName
		{
			get
			{
				return this.passwd.pw_gecos;
			}
		}

		public string ShellProgram
		{
			get
			{
				return this.passwd.pw_shell;
			}
		}

		public long UserId
		{
			get
			{
				return (long)this.passwd.pw_uid;
			}
		}

		public string UserName
		{
			get
			{
				return this.passwd.pw_name;
			}
		}

		public UnixUserInfo(string user)
		{
			Passwd passwd;
			this.passwd = new Passwd();
			if (Syscall.getpwnam_r(user, this.passwd, out passwd) != 0 || passwd == null)
			{
				throw new ArgumentException(Locale.GetText("invalid username"), "user");
			}
		}

		[CLSCompliant(false)]
		public UnixUserInfo(uint user)
		{
			Passwd passwd;
			this.passwd = new Passwd();
			if (Syscall.getpwuid_r(user, this.passwd, out passwd) != 0 || passwd == null)
			{
				throw new ArgumentException(Locale.GetText("invalid user id"), "user");
			}
		}

		public UnixUserInfo(long user)
		{
			Passwd passwd;
			this.passwd = new Passwd();
			if (Syscall.getpwuid_r(Convert.ToUInt32(user), this.passwd, out passwd) != 0 || passwd == null)
			{
				throw new ArgumentException(Locale.GetText("invalid user id"), "user");
			}
		}

		public UnixUserInfo(Passwd passwd)
		{
			this.passwd = UnixUserInfo.CopyPasswd(passwd);
		}

		private static Passwd CopyPasswd(Passwd pw)
		{
			Passwd passwd = new Passwd()
			{
				pw_name = pw.pw_name,
				pw_passwd = pw.pw_passwd,
				pw_uid = pw.pw_uid,
				pw_gid = pw.pw_gid,
				pw_gecos = pw.pw_gecos,
				pw_dir = pw.pw_dir,
				pw_shell = pw.pw_shell
			};
			return passwd;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || this.GetType() != obj.GetType())
			{
				return false;
			}
			return this.passwd.Equals(((UnixUserInfo)obj).passwd);
		}

		public override int GetHashCode()
		{
			return this.passwd.GetHashCode();
		}

		public static UnixUserInfo[] GetLocalUsers()
		{
			ArrayList arrayLists = new ArrayList();
			object pwdLock = Syscall.pwd_lock;
			Monitor.Enter(pwdLock);
			try
			{
				if (Syscall.setpwent() != 0)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
				try
				{
					while (true)
					{
						Passwd passwd = Syscall.getpwent();
						Passwd passwd1 = passwd;
						if (passwd == null)
						{
							break;
						}
						arrayLists.Add(new UnixUserInfo(passwd1));
					}
					if ((int)Stdlib.GetLastError() != 0)
					{
						UnixMarshal.ThrowExceptionForLastError();
					}
				}
				finally
				{
					Syscall.endpwent();
				}
			}
			finally
			{
				Monitor.Exit(pwdLock);
			}
			return (UnixUserInfo[])arrayLists.ToArray(typeof(UnixUserInfo));
		}

		public static string GetLoginName()
		{
			int num;
			StringBuilder stringBuilder = new StringBuilder(4);
			do
			{
				StringBuilder capacity = stringBuilder;
				capacity.Capacity = capacity.Capacity * 2;
				num = Syscall.getlogin_r(stringBuilder, (ulong)stringBuilder.Capacity);
			}
			while (num == -1 && Stdlib.GetLastError() == Errno.ERANGE);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
			return stringBuilder.ToString();
		}

		public static UnixUserInfo GetRealUser()
		{
			return new UnixUserInfo(UnixUserInfo.GetRealUserId());
		}

		public static long GetRealUserId()
		{
			return (long)Syscall.getuid();
		}

		public Passwd ToPasswd()
		{
			return UnixUserInfo.CopyPasswd(this.passwd);
		}

		public override string ToString()
		{
			return this.passwd.ToString();
		}
	}
}