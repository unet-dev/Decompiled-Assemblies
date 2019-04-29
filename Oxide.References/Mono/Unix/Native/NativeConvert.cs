using Mono.Unix;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	public sealed class NativeConvert
	{
		private const string LIB = "MonoPosixHelper";

		public readonly static DateTime LocalUnixEpoch;

		public readonly static TimeSpan LocalUtcOffset;

		private readonly static string[][] fopen_modes;

		static NativeConvert()
		{
			NativeConvert.LocalUnixEpoch = new DateTime(1970, 1, 1);
			NativeConvert.LocalUtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.UtcNow);
			NativeConvert.fopen_modes = new string[][] { new string[] { "Can't Read+Create", "wb", "w+b" }, new string[] { "Can't Read+Create", "wb", "w+b" }, new string[] { "rb", "wb", "r+b" }, new string[] { "rb", "wb", "r+b" }, new string[] { "Cannot Truncate and Read", "wb", "w+b" }, new string[] { "Cannot Append and Read", "ab", "a+b" } };
		}

		private NativeConvert()
		{
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromAccessModes", ExactSpelling=false)]
		private static extern int FromAccessModes(AccessModes value, out int rval);

		public static int FromAccessModes(AccessModes value)
		{
			int num;
			if (NativeConvert.FromAccessModes(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromConfstrName", ExactSpelling=false)]
		private static extern int FromConfstrName(ConfstrName value, out int rval);

		public static int FromConfstrName(ConfstrName value)
		{
			int num;
			if (NativeConvert.FromConfstrName(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		public static long FromDateTime(DateTime time)
		{
			return NativeConvert.ToTimeT(time);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromDirectoryNotifyFlags", ExactSpelling=false)]
		private static extern int FromDirectoryNotifyFlags(DirectoryNotifyFlags value, out int rval);

		public static int FromDirectoryNotifyFlags(DirectoryNotifyFlags value)
		{
			int num;
			if (NativeConvert.FromDirectoryNotifyFlags(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromErrno", ExactSpelling=false)]
		private static extern int FromErrno(Errno value, out int rval);

		public static int FromErrno(Errno value)
		{
			int num;
			if (NativeConvert.FromErrno(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromFcntlCommand", ExactSpelling=false)]
		private static extern int FromFcntlCommand(FcntlCommand value, out int rval);

		public static int FromFcntlCommand(FcntlCommand value)
		{
			int num;
			if (NativeConvert.FromFcntlCommand(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromFilePermissions", ExactSpelling=false)]
		private static extern int FromFilePermissions(FilePermissions value, out uint rval);

		public static uint FromFilePermissions(FilePermissions value)
		{
			uint num;
			if (NativeConvert.FromFilePermissions(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromFlock", ExactSpelling=false)]
		private static extern int FromFlock(ref Flock source, IntPtr destination);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromLockfCommand", ExactSpelling=false)]
		private static extern int FromLockfCommand(LockfCommand value, out int rval);

		public static int FromLockfCommand(LockfCommand value)
		{
			int num;
			if (NativeConvert.FromLockfCommand(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromLockType", ExactSpelling=false)]
		private static extern int FromLockType(LockType value, out short rval);

		public static short FromLockType(LockType value)
		{
			short num;
			if (NativeConvert.FromLockType(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromMlockallFlags", ExactSpelling=false)]
		private static extern int FromMlockallFlags(MlockallFlags value, out int rval);

		public static int FromMlockallFlags(MlockallFlags value)
		{
			int num;
			if (NativeConvert.FromMlockallFlags(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromMmapFlags", ExactSpelling=false)]
		private static extern int FromMmapFlags(MmapFlags value, out int rval);

		public static int FromMmapFlags(MmapFlags value)
		{
			int num;
			if (NativeConvert.FromMmapFlags(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromMmapProts", ExactSpelling=false)]
		private static extern int FromMmapProts(MmapProts value, out int rval);

		public static int FromMmapProts(MmapProts value)
		{
			int num;
			if (NativeConvert.FromMmapProts(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromMountFlags", ExactSpelling=false)]
		private static extern int FromMountFlags(MountFlags value, out ulong rval);

		public static ulong FromMountFlags(MountFlags value)
		{
			ulong num;
			if (NativeConvert.FromMountFlags(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromMremapFlags", ExactSpelling=false)]
		private static extern int FromMremapFlags(MremapFlags value, out ulong rval);

		public static ulong FromMremapFlags(MremapFlags value)
		{
			ulong num;
			if (NativeConvert.FromMremapFlags(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromMsyncFlags", ExactSpelling=false)]
		private static extern int FromMsyncFlags(MsyncFlags value, out int rval);

		public static int FromMsyncFlags(MsyncFlags value)
		{
			int num;
			if (NativeConvert.FromMsyncFlags(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		public static FilePermissions FromOctalPermissionString(string value)
		{
			return NativeConvert.ToFilePermissions(Convert.ToUInt32(value, 8));
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromOpenFlags", ExactSpelling=false)]
		private static extern int FromOpenFlags(OpenFlags value, out int rval);

		public static int FromOpenFlags(OpenFlags value)
		{
			int num;
			if (NativeConvert.FromOpenFlags(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromPathconfName", ExactSpelling=false)]
		private static extern int FromPathconfName(PathconfName value, out int rval);

		public static int FromPathconfName(PathconfName value)
		{
			int num;
			if (NativeConvert.FromPathconfName(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromPollEvents", ExactSpelling=false)]
		private static extern int FromPollEvents(PollEvents value, out short rval);

		public static short FromPollEvents(PollEvents value)
		{
			short num;
			if (NativeConvert.FromPollEvents(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromPollfd", ExactSpelling=false)]
		private static extern int FromPollfd(ref Pollfd source, IntPtr destination);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromPosixFadviseAdvice", ExactSpelling=false)]
		private static extern int FromPosixFadviseAdvice(PosixFadviseAdvice value, out int rval);

		public static int FromPosixFadviseAdvice(PosixFadviseAdvice value)
		{
			int num;
			if (NativeConvert.FromPosixFadviseAdvice(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromPosixMadviseAdvice", ExactSpelling=false)]
		private static extern int FromPosixMadviseAdvice(PosixMadviseAdvice value, out int rval);

		public static int FromPosixMadviseAdvice(PosixMadviseAdvice value)
		{
			int num;
			if (NativeConvert.FromPosixMadviseAdvice(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromRealTimeSignum", ExactSpelling=false)]
		private static extern int FromRealTimeSignum(int offset, out int rval);

		public static int FromRealTimeSignum(RealTimeSignum sig)
		{
			int num;
			if (NativeConvert.FromRealTimeSignum(sig.Offset, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(sig.Offset);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromSeekFlags", ExactSpelling=false)]
		private static extern int FromSeekFlags(SeekFlags value, out short rval);

		public static short FromSeekFlags(SeekFlags value)
		{
			short num;
			if (NativeConvert.FromSeekFlags(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromSignum", ExactSpelling=false)]
		private static extern int FromSignum(Signum value, out int rval);

		public static int FromSignum(Signum value)
		{
			int num;
			if (NativeConvert.FromSignum(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromStat", ExactSpelling=false)]
		private static extern int FromStat(ref Stat source, IntPtr destination);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromStatvfs", ExactSpelling=false)]
		private static extern int FromStatvfs(ref Statvfs source, IntPtr destination);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromSysconfName", ExactSpelling=false)]
		private static extern int FromSysconfName(SysconfName value, out int rval);

		public static int FromSysconfName(SysconfName value)
		{
			int num;
			if (NativeConvert.FromSysconfName(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromSyslogFacility", ExactSpelling=false)]
		private static extern int FromSyslogFacility(SyslogFacility value, out int rval);

		public static int FromSyslogFacility(SyslogFacility value)
		{
			int num;
			if (NativeConvert.FromSyslogFacility(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromSyslogLevel", ExactSpelling=false)]
		private static extern int FromSyslogLevel(SyslogLevel value, out int rval);

		public static int FromSyslogLevel(SyslogLevel value)
		{
			int num;
			if (NativeConvert.FromSyslogLevel(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromSyslogOptions", ExactSpelling=false)]
		private static extern int FromSyslogOptions(SyslogOptions value, out int rval);

		public static int FromSyslogOptions(SyslogOptions value)
		{
			int num;
			if (NativeConvert.FromSyslogOptions(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromTimespec", ExactSpelling=false)]
		private static extern int FromTimespec(ref Timespec source, IntPtr destination);

		public static DateTime FromTimeT(long time)
		{
			DateTime localUnixEpoch = NativeConvert.LocalUnixEpoch;
			return localUnixEpoch.AddSeconds((double)time + NativeConvert.LocalUtcOffset.TotalSeconds);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromTimeval", ExactSpelling=false)]
		private static extern int FromTimeval(ref Timeval source, IntPtr destination);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromTimezone", ExactSpelling=false)]
		private static extern int FromTimezone(ref Timezone source, IntPtr destination);

		public static FilePermissions FromUnixPermissionString(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Length != 9 && value.Length != 10)
			{
				throw new ArgumentException("value", "must contain 9 or 10 characters");
			}
			int num = 0;
			FilePermissions unixPermissionDevice = (FilePermissions)0;
			if (value.Length == 10)
			{
				unixPermissionDevice |= NativeConvert.GetUnixPermissionDevice(value[num]);
				num++;
			}
			int num1 = num;
			num = num1 + 1;
			char chr = value[num1];
			int num2 = num;
			num = num2 + 1;
			int num3 = num;
			num = num3 + 1;
			unixPermissionDevice |= NativeConvert.GetUnixPermissionGroup(chr, FilePermissions.S_IRUSR, value[num2], FilePermissions.S_IWUSR, value[num3], FilePermissions.S_IXUSR, 's', 'S', FilePermissions.S_ISUID);
			int num4 = num;
			num = num4 + 1;
			char chr1 = value[num4];
			int num5 = num;
			num = num5 + 1;
			int num6 = num;
			num = num6 + 1;
			unixPermissionDevice |= NativeConvert.GetUnixPermissionGroup(chr1, FilePermissions.S_IRGRP, value[num5], FilePermissions.S_IWGRP, value[num6], FilePermissions.S_IXGRP, 's', 'S', FilePermissions.S_ISGID);
			int num7 = num;
			num = num7 + 1;
			char chr2 = value[num7];
			int num8 = num;
			num = num8 + 1;
			int num9 = num;
			num = num9 + 1;
			unixPermissionDevice |= NativeConvert.GetUnixPermissionGroup(chr2, FilePermissions.S_IROTH, value[num8], FilePermissions.S_IWOTH, value[num9], FilePermissions.S_IXOTH, 't', 'T', FilePermissions.S_ISVTX);
			return unixPermissionDevice;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromUtimbuf", ExactSpelling=false)]
		private static extern int FromUtimbuf(ref Utimbuf source, IntPtr destination);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromWaitOptions", ExactSpelling=false)]
		private static extern int FromWaitOptions(WaitOptions value, out int rval);

		public static int FromWaitOptions(WaitOptions value)
		{
			int num;
			if (NativeConvert.FromWaitOptions(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_FromXattrFlags", ExactSpelling=false)]
		private static extern int FromXattrFlags(XattrFlags value, out int rval);

		public static int FromXattrFlags(XattrFlags value)
		{
			int num;
			if (NativeConvert.FromXattrFlags(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		private static char GetSymbolicMode(FilePermissions value, FilePermissions xbit, char both, char setonly, FilePermissions setxbit)
		{
			bool flag = UnixFileSystemInfo.IsSet(value, xbit);
			bool flag1 = UnixFileSystemInfo.IsSet(value, setxbit);
			if (flag && flag1)
			{
				return both;
			}
			if (flag1)
			{
				return setonly;
			}
			if (flag)
			{
				return 'x';
			}
			return '-';
		}

		private static FilePermissions GetUnixPermissionDevice(char value)
		{
			char chr = value;
			switch (chr)
			{
				case 'p':
				{
					return FilePermissions.S_IFIFO;
				}
				case 's':
				{
					return FilePermissions.S_IFSOCK;
				}
				default:
				{
					switch (chr)
					{
						case 'b':
						{
							return FilePermissions.S_IFBLK;
						}
						case 'c':
						{
							return FilePermissions.S_IFCHR;
						}
						case 'd':
						{
							return FilePermissions.S_IFDIR;
						}
						default:
						{
							if (chr != '-')
							{
								if (chr != 'l')
								{
									throw new ArgumentException("value", string.Concat("invalid device specification: ", value));
								}
								return FilePermissions.S_IFLNK;
							}
							break;
						}
					}
					break;
				}
			}
			return FilePermissions.S_IFREG;
		}

		private static FilePermissions GetUnixPermissionGroup(char read, FilePermissions readb, char write, FilePermissions writeb, char exec, FilePermissions execb, char xboth, char xbitonly, FilePermissions xbit)
		{
			FilePermissions filePermission = (FilePermissions)0;
			if (read == 'r')
			{
				filePermission |= readb;
			}
			if (write == 'w')
			{
				filePermission |= writeb;
			}
			if (exec == 'x')
			{
				filePermission |= execb;
			}
			else if (exec == xbitonly)
			{
				filePermission |= xbit;
			}
			else if (exec == xboth)
			{
				filePermission = filePermission | execb | xbit;
			}
			return filePermission;
		}

		private static void SetUnixPermissionGroup(FilePermissions value, char[] access, int index, FilePermissions read, FilePermissions write, FilePermissions exec, char both, char setonly, FilePermissions setxbit)
		{
			if (UnixFileSystemInfo.IsSet(value, read))
			{
				access[index] = 'r';
			}
			if (UnixFileSystemInfo.IsSet(value, write))
			{
				access[index + 1] = 'w';
			}
			access[index + 2] = NativeConvert.GetSymbolicMode(value, exec, both, setonly, setxbit);
		}

		private static void ThrowArgumentException(object value)
		{
			throw new ArgumentOutOfRangeException("value", value, Locale.GetText("Current platform doesn't support this value."));
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToAccessModes", ExactSpelling=false)]
		private static extern int ToAccessModes(int value, out AccessModes rval);

		public static AccessModes ToAccessModes(int value)
		{
			AccessModes accessMode;
			if (NativeConvert.ToAccessModes(value, out accessMode) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return accessMode;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToConfstrName", ExactSpelling=false)]
		private static extern int ToConfstrName(int value, out ConfstrName rval);

		public static ConfstrName ToConfstrName(int value)
		{
			ConfstrName confstrName;
			if (NativeConvert.ToConfstrName(value, out confstrName) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return confstrName;
		}

		public static DateTime ToDateTime(long time)
		{
			return NativeConvert.FromTimeT(time);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToDirectoryNotifyFlags", ExactSpelling=false)]
		private static extern int ToDirectoryNotifyFlags(int value, out DirectoryNotifyFlags rval);

		public static DirectoryNotifyFlags ToDirectoryNotifyFlags(int value)
		{
			DirectoryNotifyFlags directoryNotifyFlag;
			if (NativeConvert.ToDirectoryNotifyFlags(value, out directoryNotifyFlag) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return directoryNotifyFlag;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToErrno", ExactSpelling=false)]
		private static extern int ToErrno(int value, out Errno rval);

		public static Errno ToErrno(int value)
		{
			Errno errno;
			if (NativeConvert.ToErrno(value, out errno) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return errno;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToFcntlCommand", ExactSpelling=false)]
		private static extern int ToFcntlCommand(int value, out FcntlCommand rval);

		public static FcntlCommand ToFcntlCommand(int value)
		{
			FcntlCommand fcntlCommand;
			if (NativeConvert.ToFcntlCommand(value, out fcntlCommand) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return fcntlCommand;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToFilePermissions", ExactSpelling=false)]
		private static extern int ToFilePermissions(uint value, out FilePermissions rval);

		public static FilePermissions ToFilePermissions(uint value)
		{
			FilePermissions filePermission;
			if (NativeConvert.ToFilePermissions(value, out filePermission) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return filePermission;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToFlock", ExactSpelling=false)]
		private static extern int ToFlock(IntPtr source, out Flock destination);

		public static string ToFopenMode(FileAccess access)
		{
			switch (access)
			{
				case FileAccess.Read:
				{
					return "rb";
				}
				case FileAccess.Write:
				{
					return "wb";
				}
				case FileAccess.ReadWrite:
				{
					return "r+b";
				}
			}
			throw new ArgumentOutOfRangeException("access");
		}

		public static string ToFopenMode(FileMode mode)
		{
			switch (mode)
			{
				case FileMode.CreateNew:
				case FileMode.Create:
				{
					return "w+b";
				}
				case FileMode.Open:
				case FileMode.OpenOrCreate:
				{
					return "r+b";
				}
				case FileMode.Truncate:
				{
					return "w+b";
				}
				case FileMode.Append:
				{
					return "a+b";
				}
			}
			throw new ArgumentOutOfRangeException("mode");
		}

		public static string ToFopenMode(FileMode mode, FileAccess access)
		{
			int num = -1;
			int num1 = -1;
			switch (mode)
			{
				case FileMode.CreateNew:
				{
					num = 0;
					break;
				}
				case FileMode.Create:
				{
					num = 1;
					break;
				}
				case FileMode.Open:
				{
					num = 2;
					break;
				}
				case FileMode.OpenOrCreate:
				{
					num = 3;
					break;
				}
				case FileMode.Truncate:
				{
					num = 4;
					break;
				}
				case FileMode.Append:
				{
					num = 5;
					break;
				}
			}
			switch (access)
			{
				case FileAccess.Read:
				{
					num1 = 0;
					break;
				}
				case FileAccess.Write:
				{
					num1 = 1;
					break;
				}
				case FileAccess.ReadWrite:
				{
					num1 = 2;
					break;
				}
			}
			if (num == -1)
			{
				throw new ArgumentOutOfRangeException("mode");
			}
			if (num1 == -1)
			{
				throw new ArgumentOutOfRangeException("access");
			}
			string fopenModes = NativeConvert.fopen_modes[num][num1];
			if (fopenModes[0] != 'r' && fopenModes[0] != 'w' && fopenModes[0] != 'a')
			{
				throw new ArgumentException(fopenModes);
			}
			return fopenModes;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToLockfCommand", ExactSpelling=false)]
		private static extern int ToLockfCommand(int value, out LockfCommand rval);

		public static LockfCommand ToLockfCommand(int value)
		{
			LockfCommand lockfCommand;
			if (NativeConvert.ToLockfCommand(value, out lockfCommand) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return lockfCommand;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToLockType", ExactSpelling=false)]
		private static extern int ToLockType(short value, out LockType rval);

		public static LockType ToLockType(short value)
		{
			LockType lockType;
			if (NativeConvert.ToLockType(value, out lockType) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return lockType;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToMlockallFlags", ExactSpelling=false)]
		private static extern int ToMlockallFlags(int value, out MlockallFlags rval);

		public static MlockallFlags ToMlockallFlags(int value)
		{
			MlockallFlags mlockallFlag;
			if (NativeConvert.ToMlockallFlags(value, out mlockallFlag) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return mlockallFlag;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToMmapFlags", ExactSpelling=false)]
		private static extern int ToMmapFlags(int value, out MmapFlags rval);

		public static MmapFlags ToMmapFlags(int value)
		{
			MmapFlags mmapFlag;
			if (NativeConvert.ToMmapFlags(value, out mmapFlag) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return mmapFlag;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToMmapProts", ExactSpelling=false)]
		private static extern int ToMmapProts(int value, out MmapProts rval);

		public static MmapProts ToMmapProts(int value)
		{
			MmapProts mmapProt;
			if (NativeConvert.ToMmapProts(value, out mmapProt) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return mmapProt;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToMountFlags", ExactSpelling=false)]
		private static extern int ToMountFlags(ulong value, out MountFlags rval);

		public static MountFlags ToMountFlags(ulong value)
		{
			MountFlags mountFlag;
			if (NativeConvert.ToMountFlags(value, out mountFlag) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return mountFlag;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToMremapFlags", ExactSpelling=false)]
		private static extern int ToMremapFlags(ulong value, out MremapFlags rval);

		public static MremapFlags ToMremapFlags(ulong value)
		{
			MremapFlags mremapFlag;
			if (NativeConvert.ToMremapFlags(value, out mremapFlag) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return mremapFlag;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToMsyncFlags", ExactSpelling=false)]
		private static extern int ToMsyncFlags(int value, out MsyncFlags rval);

		public static MsyncFlags ToMsyncFlags(int value)
		{
			MsyncFlags msyncFlag;
			if (NativeConvert.ToMsyncFlags(value, out msyncFlag) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return msyncFlag;
		}

		public static string ToOctalPermissionString(FilePermissions value)
		{
			string str = Convert.ToString((int)(value & (FilePermissions.S_ISUID | FilePermissions.S_ISGID | FilePermissions.S_ISVTX | FilePermissions.S_IRUSR | FilePermissions.S_IWUSR | FilePermissions.S_IXUSR | FilePermissions.S_IRGRP | FilePermissions.S_IWGRP | FilePermissions.S_IXGRP | FilePermissions.S_IROTH | FilePermissions.S_IWOTH | FilePermissions.S_IXOTH | FilePermissions.S_IRWXG | FilePermissions.S_IRWXU | FilePermissions.S_IRWXO | FilePermissions.ACCESSPERMS | FilePermissions.ALLPERMS | FilePermissions.DEFFILEMODE)), 8);
			return string.Concat(new string('0', 4 - str.Length), str);
		}

		public static OpenFlags ToOpenFlags(FileMode mode, FileAccess access)
		{
			int num;
			OpenFlags openFlag = OpenFlags.O_RDONLY;
			switch (mode)
			{
				case FileMode.CreateNew:
				{
					openFlag = OpenFlags.O_CREAT | OpenFlags.O_EXCL;
					break;
				}
				case FileMode.Create:
				{
					openFlag = OpenFlags.O_CREAT | OpenFlags.O_TRUNC;
					break;
				}
				case FileMode.Open:
				{
					break;
				}
				case FileMode.OpenOrCreate:
				{
					openFlag = OpenFlags.O_CREAT;
					break;
				}
				case FileMode.Truncate:
				{
					openFlag = OpenFlags.O_TRUNC;
					break;
				}
				case FileMode.Append:
				{
					openFlag = OpenFlags.O_APPEND;
					break;
				}
				default:
				{
					throw new ArgumentException(Locale.GetText("Unsupported mode value"), "mode");
				}
			}
			if (NativeConvert.TryFromOpenFlags(OpenFlags.O_LARGEFILE, out num))
			{
				openFlag |= OpenFlags.O_LARGEFILE;
			}
			switch (access)
			{
				case FileAccess.Read:
				{
					openFlag |= OpenFlags.O_RDONLY;
					break;
				}
				case FileAccess.Write:
				{
					openFlag |= OpenFlags.O_WRONLY;
					break;
				}
				case FileAccess.ReadWrite:
				{
					openFlag |= OpenFlags.O_RDWR;
					break;
				}
				default:
				{
					throw new ArgumentOutOfRangeException(Locale.GetText("Unsupported access value"), "access");
				}
			}
			return openFlag;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToOpenFlags", ExactSpelling=false)]
		private static extern int ToOpenFlags(int value, out OpenFlags rval);

		public static OpenFlags ToOpenFlags(int value)
		{
			OpenFlags openFlag;
			if (NativeConvert.ToOpenFlags(value, out openFlag) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return openFlag;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToPathconfName", ExactSpelling=false)]
		private static extern int ToPathconfName(int value, out PathconfName rval);

		public static PathconfName ToPathconfName(int value)
		{
			PathconfName pathconfName;
			if (NativeConvert.ToPathconfName(value, out pathconfName) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return pathconfName;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToPollEvents", ExactSpelling=false)]
		private static extern int ToPollEvents(short value, out PollEvents rval);

		public static PollEvents ToPollEvents(short value)
		{
			PollEvents pollEvent;
			if (NativeConvert.ToPollEvents(value, out pollEvent) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return pollEvent;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToPollfd", ExactSpelling=false)]
		private static extern int ToPollfd(IntPtr source, out Pollfd destination);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToPosixFadviseAdvice", ExactSpelling=false)]
		private static extern int ToPosixFadviseAdvice(int value, out PosixFadviseAdvice rval);

		public static PosixFadviseAdvice ToPosixFadviseAdvice(int value)
		{
			PosixFadviseAdvice posixFadviseAdvice;
			if (NativeConvert.ToPosixFadviseAdvice(value, out posixFadviseAdvice) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return posixFadviseAdvice;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToPosixMadviseAdvice", ExactSpelling=false)]
		private static extern int ToPosixMadviseAdvice(int value, out PosixMadviseAdvice rval);

		public static PosixMadviseAdvice ToPosixMadviseAdvice(int value)
		{
			PosixMadviseAdvice posixMadviseAdvice;
			if (NativeConvert.ToPosixMadviseAdvice(value, out posixMadviseAdvice) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return posixMadviseAdvice;
		}

		public static RealTimeSignum ToRealTimeSignum(int offset)
		{
			return new RealTimeSignum(offset);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToSeekFlags", ExactSpelling=false)]
		private static extern int ToSeekFlags(short value, out SeekFlags rval);

		public static SeekFlags ToSeekFlags(short value)
		{
			SeekFlags seekFlag;
			if (NativeConvert.ToSeekFlags(value, out seekFlag) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return seekFlag;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToSignum", ExactSpelling=false)]
		private static extern int ToSignum(int value, out Signum rval);

		public static Signum ToSignum(int value)
		{
			Signum signum;
			if (NativeConvert.ToSignum(value, out signum) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return signum;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToStat", ExactSpelling=false)]
		private static extern int ToStat(IntPtr source, out Stat destination);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToStatvfs", ExactSpelling=false)]
		private static extern int ToStatvfs(IntPtr source, out Statvfs destination);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToSysconfName", ExactSpelling=false)]
		private static extern int ToSysconfName(int value, out SysconfName rval);

		public static SysconfName ToSysconfName(int value)
		{
			SysconfName sysconfName;
			if (NativeConvert.ToSysconfName(value, out sysconfName) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return sysconfName;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToSyslogFacility", ExactSpelling=false)]
		private static extern int ToSyslogFacility(int value, out SyslogFacility rval);

		public static SyslogFacility ToSyslogFacility(int value)
		{
			SyslogFacility syslogFacility;
			if (NativeConvert.ToSyslogFacility(value, out syslogFacility) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return syslogFacility;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToSyslogLevel", ExactSpelling=false)]
		private static extern int ToSyslogLevel(int value, out SyslogLevel rval);

		public static SyslogLevel ToSyslogLevel(int value)
		{
			SyslogLevel syslogLevel;
			if (NativeConvert.ToSyslogLevel(value, out syslogLevel) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return syslogLevel;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToSyslogOptions", ExactSpelling=false)]
		private static extern int ToSyslogOptions(int value, out SyslogOptions rval);

		public static SyslogOptions ToSyslogOptions(int value)
		{
			SyslogOptions syslogOption;
			if (NativeConvert.ToSyslogOptions(value, out syslogOption) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return syslogOption;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToTimespec", ExactSpelling=false)]
		private static extern int ToTimespec(IntPtr source, out Timespec destination);

		public static long ToTimeT(DateTime time)
		{
			TimeSpan timeSpan = time.Subtract(NativeConvert.LocalUnixEpoch) - NativeConvert.LocalUtcOffset;
			return (long)timeSpan.TotalSeconds;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToTimeval", ExactSpelling=false)]
		private static extern int ToTimeval(IntPtr source, out Timeval destination);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToTimezone", ExactSpelling=false)]
		private static extern int ToTimezone(IntPtr source, out Timezone destination);

		public static string ToUnixPermissionString(FilePermissions value)
		{
			char[] chrArray = new char[] { '-', '-', '-', '-', '-', '-', '-', '-', '-', '-' };
			bool flag = true;
			FilePermissions filePermission = value & FilePermissions.S_IFMT;
			if (filePermission == FilePermissions.S_IFIFO)
			{
				chrArray[0] = 'p';
			}
			else if (filePermission == FilePermissions.S_IFCHR)
			{
				chrArray[0] = 'c';
			}
			else if (filePermission == FilePermissions.S_IFDIR)
			{
				chrArray[0] = 'd';
			}
			else if (filePermission == FilePermissions.S_IFBLK)
			{
				chrArray[0] = 'b';
			}
			else if (filePermission == FilePermissions.S_IFREG)
			{
				chrArray[0] = '-';
			}
			else if (filePermission == FilePermissions.S_IFLNK)
			{
				chrArray[0] = 'l';
			}
			else if (filePermission == FilePermissions.S_IFSOCK)
			{
				chrArray[0] = 's';
			}
			else
			{
				flag = false;
			}
			NativeConvert.SetUnixPermissionGroup(value, chrArray, 1, FilePermissions.S_IRUSR, FilePermissions.S_IWUSR, FilePermissions.S_IXUSR, 's', 'S', FilePermissions.S_ISUID);
			NativeConvert.SetUnixPermissionGroup(value, chrArray, 4, FilePermissions.S_IRGRP, FilePermissions.S_IWGRP, FilePermissions.S_IXGRP, 's', 'S', FilePermissions.S_ISGID);
			NativeConvert.SetUnixPermissionGroup(value, chrArray, 7, FilePermissions.S_IROTH, FilePermissions.S_IWOTH, FilePermissions.S_IXOTH, 't', 'T', FilePermissions.S_ISVTX);
			return (!flag ? new string(chrArray, 1, 9) : new string(chrArray));
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToUtimbuf", ExactSpelling=false)]
		private static extern int ToUtimbuf(IntPtr source, out Utimbuf destination);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToWaitOptions", ExactSpelling=false)]
		private static extern int ToWaitOptions(int value, out WaitOptions rval);

		public static WaitOptions ToWaitOptions(int value)
		{
			WaitOptions waitOption;
			if (NativeConvert.ToWaitOptions(value, out waitOption) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return waitOption;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_ToXattrFlags", ExactSpelling=false)]
		private static extern int ToXattrFlags(int value, out XattrFlags rval);

		public static XattrFlags ToXattrFlags(int value)
		{
			XattrFlags xattrFlag;
			if (NativeConvert.ToXattrFlags(value, out xattrFlag) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return xattrFlag;
		}

		public static bool TryCopy(ref Statvfs source, IntPtr destination)
		{
			return NativeConvert.FromStatvfs(ref source, destination) == 0;
		}

		public static bool TryCopy(IntPtr source, out Statvfs destination)
		{
			return NativeConvert.ToStatvfs(source, out destination) == 0;
		}

		public static bool TryCopy(ref Flock source, IntPtr destination)
		{
			return NativeConvert.FromFlock(ref source, destination) == 0;
		}

		public static bool TryCopy(IntPtr source, out Flock destination)
		{
			return NativeConvert.ToFlock(source, out destination) == 0;
		}

		public static bool TryCopy(ref Pollfd source, IntPtr destination)
		{
			return NativeConvert.FromPollfd(ref source, destination) == 0;
		}

		public static bool TryCopy(IntPtr source, out Pollfd destination)
		{
			return NativeConvert.ToPollfd(source, out destination) == 0;
		}

		public static bool TryCopy(ref Stat source, IntPtr destination)
		{
			return NativeConvert.FromStat(ref source, destination) == 0;
		}

		public static bool TryCopy(IntPtr source, out Stat destination)
		{
			return NativeConvert.ToStat(source, out destination) == 0;
		}

		public static bool TryCopy(ref Timespec source, IntPtr destination)
		{
			return NativeConvert.FromTimespec(ref source, destination) == 0;
		}

		public static bool TryCopy(IntPtr source, out Timespec destination)
		{
			return NativeConvert.ToTimespec(source, out destination) == 0;
		}

		public static bool TryCopy(ref Timeval source, IntPtr destination)
		{
			return NativeConvert.FromTimeval(ref source, destination) == 0;
		}

		public static bool TryCopy(IntPtr source, out Timeval destination)
		{
			return NativeConvert.ToTimeval(source, out destination) == 0;
		}

		public static bool TryCopy(ref Timezone source, IntPtr destination)
		{
			return NativeConvert.FromTimezone(ref source, destination) == 0;
		}

		public static bool TryCopy(IntPtr source, out Timezone destination)
		{
			return NativeConvert.ToTimezone(source, out destination) == 0;
		}

		public static bool TryCopy(ref Utimbuf source, IntPtr destination)
		{
			return NativeConvert.FromUtimbuf(ref source, destination) == 0;
		}

		public static bool TryCopy(IntPtr source, out Utimbuf destination)
		{
			return NativeConvert.ToUtimbuf(source, out destination) == 0;
		}

		public static bool TryFromAccessModes(AccessModes value, out int rval)
		{
			return NativeConvert.FromAccessModes(value, out rval) == 0;
		}

		public static bool TryFromConfstrName(ConfstrName value, out int rval)
		{
			return NativeConvert.FromConfstrName(value, out rval) == 0;
		}

		public static bool TryFromDirectoryNotifyFlags(DirectoryNotifyFlags value, out int rval)
		{
			return NativeConvert.FromDirectoryNotifyFlags(value, out rval) == 0;
		}

		public static bool TryFromErrno(Errno value, out int rval)
		{
			return NativeConvert.FromErrno(value, out rval) == 0;
		}

		public static bool TryFromFcntlCommand(FcntlCommand value, out int rval)
		{
			return NativeConvert.FromFcntlCommand(value, out rval) == 0;
		}

		public static bool TryFromFilePermissions(FilePermissions value, out uint rval)
		{
			return NativeConvert.FromFilePermissions(value, out rval) == 0;
		}

		public static bool TryFromLockfCommand(LockfCommand value, out int rval)
		{
			return NativeConvert.FromLockfCommand(value, out rval) == 0;
		}

		public static bool TryFromLockType(LockType value, out short rval)
		{
			return NativeConvert.FromLockType(value, out rval) == 0;
		}

		public static bool TryFromMlockallFlags(MlockallFlags value, out int rval)
		{
			return NativeConvert.FromMlockallFlags(value, out rval) == 0;
		}

		public static bool TryFromMmapFlags(MmapFlags value, out int rval)
		{
			return NativeConvert.FromMmapFlags(value, out rval) == 0;
		}

		public static bool TryFromMmapProts(MmapProts value, out int rval)
		{
			return NativeConvert.FromMmapProts(value, out rval) == 0;
		}

		public static bool TryFromMountFlags(MountFlags value, out ulong rval)
		{
			return NativeConvert.FromMountFlags(value, out rval) == 0;
		}

		public static bool TryFromMremapFlags(MremapFlags value, out ulong rval)
		{
			return NativeConvert.FromMremapFlags(value, out rval) == 0;
		}

		public static bool TryFromMsyncFlags(MsyncFlags value, out int rval)
		{
			return NativeConvert.FromMsyncFlags(value, out rval) == 0;
		}

		public static bool TryFromOpenFlags(OpenFlags value, out int rval)
		{
			return NativeConvert.FromOpenFlags(value, out rval) == 0;
		}

		public static bool TryFromPathconfName(PathconfName value, out int rval)
		{
			return NativeConvert.FromPathconfName(value, out rval) == 0;
		}

		public static bool TryFromPollEvents(PollEvents value, out short rval)
		{
			return NativeConvert.FromPollEvents(value, out rval) == 0;
		}

		public static bool TryFromPosixFadviseAdvice(PosixFadviseAdvice value, out int rval)
		{
			return NativeConvert.FromPosixFadviseAdvice(value, out rval) == 0;
		}

		public static bool TryFromPosixMadviseAdvice(PosixMadviseAdvice value, out int rval)
		{
			return NativeConvert.FromPosixMadviseAdvice(value, out rval) == 0;
		}

		public static bool TryFromSeekFlags(SeekFlags value, out short rval)
		{
			return NativeConvert.FromSeekFlags(value, out rval) == 0;
		}

		public static bool TryFromSignum(Signum value, out int rval)
		{
			return NativeConvert.FromSignum(value, out rval) == 0;
		}

		public static bool TryFromSysconfName(SysconfName value, out int rval)
		{
			return NativeConvert.FromSysconfName(value, out rval) == 0;
		}

		public static bool TryFromSyslogFacility(SyslogFacility value, out int rval)
		{
			return NativeConvert.FromSyslogFacility(value, out rval) == 0;
		}

		public static bool TryFromSyslogLevel(SyslogLevel value, out int rval)
		{
			return NativeConvert.FromSyslogLevel(value, out rval) == 0;
		}

		public static bool TryFromSyslogOptions(SyslogOptions value, out int rval)
		{
			return NativeConvert.FromSyslogOptions(value, out rval) == 0;
		}

		public static bool TryFromWaitOptions(WaitOptions value, out int rval)
		{
			return NativeConvert.FromWaitOptions(value, out rval) == 0;
		}

		public static bool TryFromXattrFlags(XattrFlags value, out int rval)
		{
			return NativeConvert.FromXattrFlags(value, out rval) == 0;
		}

		public static bool TryToAccessModes(int value, out AccessModes rval)
		{
			return NativeConvert.ToAccessModes(value, out rval) == 0;
		}

		public static bool TryToConfstrName(int value, out ConfstrName rval)
		{
			return NativeConvert.ToConfstrName(value, out rval) == 0;
		}

		public static bool TryToDirectoryNotifyFlags(int value, out DirectoryNotifyFlags rval)
		{
			return NativeConvert.ToDirectoryNotifyFlags(value, out rval) == 0;
		}

		public static bool TryToErrno(int value, out Errno rval)
		{
			return NativeConvert.ToErrno(value, out rval) == 0;
		}

		public static bool TryToFcntlCommand(int value, out FcntlCommand rval)
		{
			return NativeConvert.ToFcntlCommand(value, out rval) == 0;
		}

		public static bool TryToFilePermissions(uint value, out FilePermissions rval)
		{
			return NativeConvert.ToFilePermissions(value, out rval) == 0;
		}

		public static bool TryToLockfCommand(int value, out LockfCommand rval)
		{
			return NativeConvert.ToLockfCommand(value, out rval) == 0;
		}

		public static bool TryToLockType(short value, out LockType rval)
		{
			return NativeConvert.ToLockType(value, out rval) == 0;
		}

		public static bool TryToMlockallFlags(int value, out MlockallFlags rval)
		{
			return NativeConvert.ToMlockallFlags(value, out rval) == 0;
		}

		public static bool TryToMmapFlags(int value, out MmapFlags rval)
		{
			return NativeConvert.ToMmapFlags(value, out rval) == 0;
		}

		public static bool TryToMmapProts(int value, out MmapProts rval)
		{
			return NativeConvert.ToMmapProts(value, out rval) == 0;
		}

		public static bool TryToMountFlags(ulong value, out MountFlags rval)
		{
			return NativeConvert.ToMountFlags(value, out rval) == 0;
		}

		public static bool TryToMremapFlags(ulong value, out MremapFlags rval)
		{
			return NativeConvert.ToMremapFlags(value, out rval) == 0;
		}

		public static bool TryToMsyncFlags(int value, out MsyncFlags rval)
		{
			return NativeConvert.ToMsyncFlags(value, out rval) == 0;
		}

		public static bool TryToOpenFlags(int value, out OpenFlags rval)
		{
			return NativeConvert.ToOpenFlags(value, out rval) == 0;
		}

		public static bool TryToPathconfName(int value, out PathconfName rval)
		{
			return NativeConvert.ToPathconfName(value, out rval) == 0;
		}

		public static bool TryToPollEvents(short value, out PollEvents rval)
		{
			return NativeConvert.ToPollEvents(value, out rval) == 0;
		}

		public static bool TryToPosixFadviseAdvice(int value, out PosixFadviseAdvice rval)
		{
			return NativeConvert.ToPosixFadviseAdvice(value, out rval) == 0;
		}

		public static bool TryToPosixMadviseAdvice(int value, out PosixMadviseAdvice rval)
		{
			return NativeConvert.ToPosixMadviseAdvice(value, out rval) == 0;
		}

		public static bool TryToSeekFlags(short value, out SeekFlags rval)
		{
			return NativeConvert.ToSeekFlags(value, out rval) == 0;
		}

		public static bool TryToSignum(int value, out Signum rval)
		{
			return NativeConvert.ToSignum(value, out rval) == 0;
		}

		public static bool TryToSysconfName(int value, out SysconfName rval)
		{
			return NativeConvert.ToSysconfName(value, out rval) == 0;
		}

		public static bool TryToSyslogFacility(int value, out SyslogFacility rval)
		{
			return NativeConvert.ToSyslogFacility(value, out rval) == 0;
		}

		public static bool TryToSyslogLevel(int value, out SyslogLevel rval)
		{
			return NativeConvert.ToSyslogLevel(value, out rval) == 0;
		}

		public static bool TryToSyslogOptions(int value, out SyslogOptions rval)
		{
			return NativeConvert.ToSyslogOptions(value, out rval) == 0;
		}

		public static bool TryToWaitOptions(int value, out WaitOptions rval)
		{
			return NativeConvert.ToWaitOptions(value, out rval) == 0;
		}

		public static bool TryToXattrFlags(int value, out XattrFlags rval)
		{
			return NativeConvert.ToXattrFlags(value, out rval) == 0;
		}
	}
}