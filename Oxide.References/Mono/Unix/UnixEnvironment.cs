using Mono.Unix.Native;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace Mono.Unix
{
	public sealed class UnixEnvironment
	{
		public static string CurrentDirectory
		{
			get
			{
				return UnixDirectoryInfo.GetCurrentDirectory();
			}
			set
			{
				UnixDirectoryInfo.SetCurrentDirectory(value);
			}
		}

		public static UnixGroupInfo EffectiveGroup
		{
			get
			{
				return new UnixGroupInfo(UnixEnvironment.EffectiveGroupId);
			}
			set
			{
				UnixEnvironment.EffectiveGroupId = value.GroupId;
			}
		}

		public static long EffectiveGroupId
		{
			get
			{
				return (long)Syscall.getegid();
			}
			set
			{
				Syscall.setegid(Convert.ToUInt32(value));
			}
		}

		public static UnixUserInfo EffectiveUser
		{
			get
			{
				return new UnixUserInfo(UnixEnvironment.EffectiveUserId);
			}
			set
			{
				UnixEnvironment.EffectiveUserId = value.UserId;
			}
		}

		public static long EffectiveUserId
		{
			get
			{
				return (long)Syscall.geteuid();
			}
			set
			{
				Syscall.seteuid(Convert.ToUInt32(value));
			}
		}

		public static string Login
		{
			get
			{
				return UnixUserInfo.GetRealUser().UserName;
			}
		}

		public static string MachineName
		{
			get
			{
				Utsname utsname;
				if (Syscall.uname(out utsname) != 0)
				{
					throw UnixMarshal.CreateExceptionForLastError();
				}
				return utsname.nodename;
			}
			set
			{
				UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.sethostname(value));
			}
		}

		public static UnixGroupInfo RealGroup
		{
			get
			{
				return new UnixGroupInfo(UnixEnvironment.RealGroupId);
			}
		}

		public static long RealGroupId
		{
			get
			{
				return (long)Syscall.getgid();
			}
		}

		public static UnixUserInfo RealUser
		{
			get
			{
				return new UnixUserInfo(UnixEnvironment.RealUserId);
			}
		}

		public static long RealUserId
		{
			get
			{
				return (long)Syscall.getuid();
			}
		}

		public static string UserName
		{
			get
			{
				return UnixUserInfo.GetRealUser().UserName;
			}
		}

		private UnixEnvironment()
		{
		}

		private static uint[] _GetSupplementaryGroupIds()
		{
			int num = Syscall.getgroups(0, new uint[0]);
			if (num == -1)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			uint[] numArray = new uint[num];
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.getgroups(numArray));
			return numArray;
		}

		public static int CreateSession()
		{
			int num = Syscall.setsid();
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
			return num;
		}

		[CLSCompliant(false)]
		public static string GetConfigurationString(ConfstrName name)
		{
			ulong num = Syscall.confstr(name, null, (ulong)0);
			if (num == (long)-1)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			if (num == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder((int)num + 1);
			num = Syscall.confstr(name, stringBuilder, num);
			if (num == (long)-1)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return stringBuilder.ToString();
		}

		[CLSCompliant(false)]
		public static long GetConfigurationValue(SysconfName name)
		{
			long num = Syscall.sysconf(name);
			if (num == (long)-1 && (int)Stdlib.GetLastError() != 0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return num;
		}

		public static UnixProcess GetParentProcess()
		{
			return new UnixProcess(UnixEnvironment.GetParentProcessId());
		}

		public static int GetParentProcessId()
		{
			return Syscall.getppid();
		}

		public static int GetProcessGroup()
		{
			return Syscall.getpgrp();
		}

		public static long[] GetSupplementaryGroupIds()
		{
			uint[] numArray = UnixEnvironment._GetSupplementaryGroupIds();
			long[] numArray1 = new long[(int)numArray.Length];
			for (int i = 0; i < (int)numArray1.Length; i++)
			{
				numArray1[i] = (long)numArray[i];
			}
			return numArray1;
		}

		public static UnixGroupInfo[] GetSupplementaryGroups()
		{
			uint[] numArray = UnixEnvironment._GetSupplementaryGroupIds();
			UnixGroupInfo[] unixGroupInfo = new UnixGroupInfo[(int)numArray.Length];
			for (int i = 0; i < (int)unixGroupInfo.Length; i++)
			{
				unixGroupInfo[i] = new UnixGroupInfo((long)numArray[i]);
			}
			return unixGroupInfo;
		}

		public static string[] GetUserShells()
		{
			ArrayList arrayLists = new ArrayList();
			object usershellLock = Syscall.usershell_lock;
			Monitor.Enter(usershellLock);
			try
			{
				try
				{
					if (Syscall.setusershell() != 0)
					{
						UnixMarshal.ThrowExceptionForLastError();
					}
					while (true)
					{
						string str = Syscall.getusershell();
						string str1 = str;
						if (str == null)
						{
							break;
						}
						arrayLists.Add(str1);
					}
				}
				finally
				{
					Syscall.endusershell();
				}
			}
			finally
			{
				Monitor.Exit(usershellLock);
			}
			return (string[])arrayLists.ToArray(typeof(string));
		}

		public static void SetNiceValue(int inc)
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.nice(inc));
		}

		public static void SetProcessGroup()
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.setpgrp());
		}

		public static void SetSupplementaryGroupIds(long[] list)
		{
			uint[] num = new uint[(int)list.Length];
			for (int i = 0; i < (int)num.Length; i++)
			{
				num[i] = Convert.ToUInt32(list[i]);
			}
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.setgroups(num));
		}

		public static void SetSupplementaryGroups(UnixGroupInfo[] groups)
		{
			uint[] num = new uint[(int)groups.Length];
			for (int i = 0; i < (int)num.Length; i++)
			{
				num[i] = Convert.ToUInt32(groups[i].GroupId);
			}
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.setgroups(num));
		}
	}
}