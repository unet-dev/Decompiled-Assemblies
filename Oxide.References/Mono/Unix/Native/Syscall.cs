using Mono.Unix;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	public sealed class Syscall : Stdlib
	{
		internal const string LIBC = "libc";

		internal static object readdir_lock;

		internal static object fstab_lock;

		internal static object grp_lock;

		internal static object pwd_lock;

		private static object signal_lock;

		public readonly static int L_ctermid;

		public readonly static int L_cuserid;

		internal static object getlogin_lock;

		public readonly static IntPtr MAP_FAILED;

		private static object tty_lock;

		internal static object usershell_lock;

		static Syscall()
		{
			Syscall.readdir_lock = new object();
			Syscall.fstab_lock = new object();
			Syscall.grp_lock = new object();
			Syscall.pwd_lock = new object();
			Syscall.signal_lock = new object();
			Syscall.L_ctermid = Syscall._L_ctermid();
			Syscall.L_cuserid = Syscall._L_cuserid();
			Syscall.getlogin_lock = new object();
			Syscall.MAP_FAILED = (IntPtr)-1;
			Syscall.tty_lock = new object();
			Syscall.usershell_lock = new object();
		}

		private Syscall()
		{
		}

		[CLSCompliant(false)]
		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int _exit(int status);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_L_ctermid", ExactSpelling=false)]
		private static extern int _L_ctermid();

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_L_cuserid", ExactSpelling=false)]
		private static extern int _L_cuserid();

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_WIFEXITED", ExactSpelling=false)]
		private static extern int _WIFEXITED(int status);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_WIFSIGNALED", ExactSpelling=false)]
		private static extern int _WIFSIGNALED(int status);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_WIFSTOPPED", ExactSpelling=false)]
		private static extern int _WIFSTOPPED(int status);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_WSTOPSIG", ExactSpelling=false)]
		private static extern int _WSTOPSIG(int status);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_WTERMSIG", ExactSpelling=false)]
		private static extern int _WTERMSIG(int status);

		public static int access(string pathname, AccessModes mode)
		{
			return Syscall.sys_access(pathname, NativeConvert.FromAccessModes(mode));
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int acct(string filename);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern uint alarm(uint seconds);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int brk(IntPtr end_data_segment);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int chdir(string path);

		public static int chmod(string path, FilePermissions mode)
		{
			return Syscall.sys_chmod(path, NativeConvert.FromFilePermissions(mode));
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int chown(string path, uint owner, uint group);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int chroot(string path);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int close(int fd);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int closedir(IntPtr dir);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_closelog", ExactSpelling=false, SetLastError=true)]
		public static extern int closelog();

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_confstr", ExactSpelling=false, SetLastError=true)]
		public static extern ulong confstr(ConfstrName name, [Out] StringBuilder buf, ulong len);

		private static void CopyDirent(Dirent to, ref Syscall._Dirent from)
		{
			try
			{
				to.d_ino = from.d_ino;
				to.d_off = from.d_off;
				to.d_reclen = from.d_reclen;
				to.d_type = from.d_type;
				to.d_name = UnixMarshal.PtrToString(from.d_name);
			}
			finally
			{
				Stdlib.free(from.d_name);
				from.d_name = IntPtr.Zero;
			}
		}

		private static void CopyFstab(Fstab to, ref Syscall._Fstab from)
		{
			try
			{
				to.fs_spec = UnixMarshal.PtrToString(from.fs_spec);
				to.fs_file = UnixMarshal.PtrToString(from.fs_file);
				to.fs_vfstype = UnixMarshal.PtrToString(from.fs_vfstype);
				to.fs_mntops = UnixMarshal.PtrToString(from.fs_mntops);
				to.fs_type = UnixMarshal.PtrToString(from.fs_type);
				to.fs_freq = from.fs_freq;
				to.fs_passno = from.fs_passno;
			}
			finally
			{
				Stdlib.free(from._fs_buf_);
				from._fs_buf_ = IntPtr.Zero;
			}
		}

		private static void CopyGroup(Group to, ref Syscall._Group from)
		{
			try
			{
				to.gr_gid = from.gr_gid;
				to.gr_name = UnixMarshal.PtrToString(from.gr_name);
				to.gr_passwd = UnixMarshal.PtrToString(from.gr_passwd);
				to.gr_mem = UnixMarshal.PtrToStringArray(from._gr_nmem_, from.gr_mem);
			}
			finally
			{
				Stdlib.free(from.gr_mem);
				Stdlib.free(from._gr_buf_);
				from.gr_mem = IntPtr.Zero;
				from._gr_buf_ = IntPtr.Zero;
			}
		}

		private static void CopyPasswd(Passwd to, ref Syscall._Passwd from)
		{
			try
			{
				to.pw_name = UnixMarshal.PtrToString(from.pw_name);
				to.pw_passwd = UnixMarshal.PtrToString(from.pw_passwd);
				to.pw_uid = from.pw_uid;
				to.pw_gid = from.pw_gid;
				to.pw_gecos = UnixMarshal.PtrToString(from.pw_gecos);
				to.pw_dir = UnixMarshal.PtrToString(from.pw_dir);
				to.pw_shell = UnixMarshal.PtrToString(from.pw_shell);
			}
			finally
			{
				Stdlib.free(from._pw_buf_);
				from._pw_buf_ = IntPtr.Zero;
			}
		}

		private static void CopyUtsname(ref Utsname to, ref Syscall._Utsname from)
		{
			try
			{
				to = new Utsname()
				{
					sysname = UnixMarshal.PtrToString(from.sysname),
					nodename = UnixMarshal.PtrToString(from.nodename),
					release = UnixMarshal.PtrToString(from.release),
					version = UnixMarshal.PtrToString(from.version),
					machine = UnixMarshal.PtrToString(from.machine),
					domainname = UnixMarshal.PtrToString(from.domainname)
				};
			}
			finally
			{
				Stdlib.free(from._buf_);
				from._buf_ = IntPtr.Zero;
			}
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_creat", ExactSpelling=false, SetLastError=true)]
		public static extern int creat(string pathname, FilePermissions mode);

		[Obsolete("This is insecure and should not be used", true)]
		public static string crypt(string key, string salt)
		{
			throw new SecurityException("crypt(3) has been broken.  Use something more secure.");
		}

		[Obsolete("\"Nobody knows precisely what cuserid() does... DO NOT USE cuserid().\n`string' must hold L_cuserid characters.  Use getlogin_r instead.")]
		public static string cuserid(StringBuilder @string)
		{
			// 
			// Current member / type: System.String Mono.Unix.Native.Syscall::cuserid(System.Text.StringBuilder)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Oxide.References.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.String cuserid(System.Text.StringBuilder)
			// 
			// Object reference not set to an instance of an object.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:line 93
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:line 24
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 69
			//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:line 19
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int dirfd(IntPtr dir);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int dup(int fd);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int dup2(int fd, int fd2);

		[Obsolete("This is insecure and should not be used", true)]
		public static int encrypt(byte[] block, bool decode)
		{
			throw new SecurityException("crypt(3) has been broken.  Use something more secure.");
		}

		public static int endfsent()
		{
			int num;
			object fstabLock = Syscall.fstab_lock;
			Monitor.Enter(fstabLock);
			try
			{
				num = Syscall.sys_endfsent();
			}
			finally
			{
				Monitor.Exit(fstabLock);
			}
			return num;
		}

		public static int endgrent()
		{
			int num;
			object grpLock = Syscall.grp_lock;
			Monitor.Enter(grpLock);
			try
			{
				num = Syscall.sys_endgrent();
			}
			finally
			{
				Monitor.Exit(grpLock);
			}
			return num;
		}

		public static int endpwent()
		{
			int num;
			object pwdLock = Syscall.pwd_lock;
			Monitor.Enter(pwdLock);
			try
			{
				num = Syscall.sys_endpwent();
			}
			finally
			{
				Monitor.Exit(pwdLock);
			}
			return num;
		}

		public static int endusershell()
		{
			int num;
			object usershellLock = Syscall.usershell_lock;
			Monitor.Enter(usershellLock);
			try
			{
				num = Syscall.sys_endusershell();
			}
			finally
			{
				Monitor.Exit(usershellLock);
			}
			return num;
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int execv(string path, string[] argv);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int execve(string path, string[] argv, string[] envp);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int execvp(string path, string[] argv);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int fchdir(int fd);

		public static int fchmod(int filedes, FilePermissions mode)
		{
			return Syscall.sys_fchmod(filedes, NativeConvert.FromFilePermissions(mode));
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int fchown(int fd, uint owner, uint group);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_fcntl", ExactSpelling=false, SetLastError=true)]
		public static extern int fcntl(int fd, FcntlCommand cmd);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_fcntl_arg", ExactSpelling=false, SetLastError=true)]
		public static extern int fcntl(int fd, FcntlCommand cmd, long arg);

		public static int fcntl(int fd, FcntlCommand cmd, DirectoryNotifyFlags arg)
		{
			if (cmd != FcntlCommand.F_NOTIFY)
			{
				Stdlib.SetLastError(Errno.EINVAL);
				return -1;
			}
			long num = (long)NativeConvert.FromDirectoryNotifyFlags(arg);
			return Syscall.fcntl(fd, FcntlCommand.F_NOTIFY, num);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_fcntl_lock", ExactSpelling=false, SetLastError=true)]
		public static extern int fcntl(int fd, FcntlCommand cmd, ref Flock @lock);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int fdatasync(int fd);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int fexecve(int fd, string[] argv, string[] envp);

		public static Group fgetgrent(IntPtr stream)
		{
			Syscall._Group __Group;
			int num;
			object grpLock = Syscall.grp_lock;
			Monitor.Enter(grpLock);
			try
			{
				num = Syscall.sys_fgetgrent(stream, out __Group);
			}
			finally
			{
				Monitor.Exit(grpLock);
			}
			if (num != 0)
			{
				return null;
			}
			Group group = new Group();
			Syscall.CopyGroup(group, ref __Group);
			return group;
		}

		public static Passwd fgetpwent(IntPtr stream)
		{
			Syscall._Passwd __Passwd;
			int num;
			object pwdLock = Syscall.pwd_lock;
			Monitor.Enter(pwdLock);
			try
			{
				num = Syscall.sys_fgetpwent(stream, out __Passwd);
			}
			finally
			{
				Monitor.Exit(pwdLock);
			}
			if (num != 0)
			{
				return null;
			}
			Passwd passwd = new Passwd();
			Syscall.CopyPasswd(passwd, ref __Passwd);
			return passwd;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_fgetxattr", ExactSpelling=false, SetLastError=true)]
		public static extern long fgetxattr(int fd, string name, byte[] value, ulong size);

		public static long fgetxattr(int fd, string name, byte[] value)
		{
			return Syscall.fgetxattr(fd, name, value, (ulong)((int)value.Length));
		}

		public static long fgetxattr(int fd, string name, out byte[] value)
		{
			value = null;
			long num = Syscall.fgetxattr(fd, name, value, (ulong)0);
			if (num <= (long)0)
			{
				return num;
			}
			value = new byte[checked((IntPtr)num)];
			return Syscall.fgetxattr(fd, name, value, (ulong)num);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_flistxattr", ExactSpelling=false, SetLastError=true)]
		public static extern long flistxattr(int fd, byte[] list, ulong size);

		public static long flistxattr(int fd, Encoding encoding, out string[] values)
		{
			values = null;
			long num = Syscall.flistxattr(fd, null, (ulong)0);
			if (num == 0)
			{
				values = new string[0];
			}
			if (num <= (long)0)
			{
				return (long)((int)num);
			}
			byte[] numArray = new byte[checked((IntPtr)num)];
			long num1 = Syscall.flistxattr(fd, numArray, (ulong)num);
			if (num1 < (long)0)
			{
				return (long)((int)num1);
			}
			Syscall.GetValues(numArray, encoding, out values);
			return (long)0;
		}

		public static long flistxattr(int fd, out string[] values)
		{
			return Syscall.flistxattr(fd, UnixEncoding.Instance, out values);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_fpathconf", ExactSpelling=false, SetLastError=true)]
		public static extern long fpathconf(int filedes, PathconfName name, Errno defaultError);

		public static long fpathconf(int filedes, PathconfName name)
		{
			return Syscall.fpathconf(filedes, name, 0);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_fremovexattr", ExactSpelling=false, SetLastError=true)]
		public static extern int fremovexattr(int fd, string name);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_fsetxattr", ExactSpelling=false, SetLastError=true)]
		public static extern int fsetxattr(int fd, string name, byte[] value, ulong size, XattrFlags flags);

		public static int fsetxattr(int fd, string name, byte[] value, ulong size)
		{
			return Syscall.fsetxattr(fd, name, value, size, XattrFlags.XATTR_AUTO);
		}

		public static int fsetxattr(int fd, string name, byte[] value, XattrFlags flags)
		{
			return Syscall.fsetxattr(fd, name, value, (ulong)((int)value.Length), flags);
		}

		public static int fsetxattr(int fd, string name, byte[] value)
		{
			return Syscall.fsetxattr(fd, name, value, (ulong)((int)value.Length));
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_fstat", ExactSpelling=false, SetLastError=true)]
		public static extern int fstat(int filedes, out Stat buf);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_fstatvfs", ExactSpelling=false, SetLastError=true)]
		public static extern int fstatvfs(int fd, out Statvfs buf);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int fsync(int fd);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_ftruncate", ExactSpelling=false, SetLastError=true)]
		public static extern int ftruncate(int fd, long length);

		public static int futimes(int fd, Timeval[] tvp)
		{
			if (tvp != null && (int)tvp.Length != 2)
			{
				Stdlib.SetLastError(Errno.EINVAL);
				return -1;
			}
			return Syscall.sys_futimes(fd, tvp);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_getcwd", ExactSpelling=false, SetLastError=true)]
		public static extern IntPtr getcwd([Out] StringBuilder buf, ulong size);

		public static StringBuilder getcwd(StringBuilder buf)
		{
			Syscall.getcwd(buf, (ulong)buf.Capacity);
			return buf;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_getdomainname", ExactSpelling=false, SetLastError=true)]
		public static extern int getdomainname([Out] StringBuilder name, ulong len);

		public static int getdomainname(StringBuilder name)
		{
			return Syscall.getdomainname(name, (ulong)name.Capacity);
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int getdtablesize();

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern uint getegid();

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern uint geteuid();

		public static Fstab getfsent()
		{
			Syscall._Fstab __Fstab;
			int num;
			object fstabLock = Syscall.fstab_lock;
			Monitor.Enter(fstabLock);
			try
			{
				num = Syscall.sys_getfsent(out __Fstab);
			}
			finally
			{
				Monitor.Exit(fstabLock);
			}
			if (num != 0)
			{
				return null;
			}
			Fstab fstab = new Fstab();
			Syscall.CopyFstab(fstab, ref __Fstab);
			return fstab;
		}

		public static Fstab getfsfile(string mount_point)
		{
			Syscall._Fstab __Fstab;
			int num;
			object fstabLock = Syscall.fstab_lock;
			Monitor.Enter(fstabLock);
			try
			{
				num = Syscall.sys_getfsfile(mount_point, out __Fstab);
			}
			finally
			{
				Monitor.Exit(fstabLock);
			}
			if (num != 0)
			{
				return null;
			}
			Fstab fstab = new Fstab();
			Syscall.CopyFstab(fstab, ref __Fstab);
			return fstab;
		}

		public static Fstab getfsspec(string special_file)
		{
			Syscall._Fstab __Fstab;
			int num;
			object fstabLock = Syscall.fstab_lock;
			Monitor.Enter(fstabLock);
			try
			{
				num = Syscall.sys_getfsspec(special_file, out __Fstab);
			}
			finally
			{
				Monitor.Exit(fstabLock);
			}
			if (num != 0)
			{
				return null;
			}
			Fstab fstab = new Fstab();
			Syscall.CopyFstab(fstab, ref __Fstab);
			return fstab;
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern uint getgid();

		public static Group getgrent()
		{
			Syscall._Group __Group;
			int num;
			object grpLock = Syscall.grp_lock;
			Monitor.Enter(grpLock);
			try
			{
				num = Syscall.sys_getgrent(out __Group);
			}
			finally
			{
				Monitor.Exit(grpLock);
			}
			if (num != 0)
			{
				return null;
			}
			Group group = new Group();
			Syscall.CopyGroup(group, ref __Group);
			return group;
		}

		public static Group getgrgid(uint uid)
		{
			Syscall._Group __Group;
			int num;
			object grpLock = Syscall.grp_lock;
			Monitor.Enter(grpLock);
			try
			{
				num = Syscall.sys_getgrgid(uid, out __Group);
			}
			finally
			{
				Monitor.Exit(grpLock);
			}
			if (num != 0)
			{
				return null;
			}
			Group group = new Group();
			Syscall.CopyGroup(group, ref __Group);
			return group;
		}

		public static int getgrgid_r(uint uid, Group grbuf, out Group grbufp)
		{
			Syscall._Group __Group;
			IntPtr intPtr;
			grbufp = null;
			int num = Syscall.sys_getgrgid_r(uid, out __Group, out intPtr);
			if (num == 0 && intPtr != IntPtr.Zero)
			{
				Syscall.CopyGroup(grbuf, ref __Group);
				grbufp = grbuf;
			}
			return num;
		}

		public static Group getgrnam(string name)
		{
			Syscall._Group __Group;
			int num;
			object grpLock = Syscall.grp_lock;
			Monitor.Enter(grpLock);
			try
			{
				num = Syscall.sys_getgrnam(name, out __Group);
			}
			finally
			{
				Monitor.Exit(grpLock);
			}
			if (num != 0)
			{
				return null;
			}
			Group group = new Group();
			Syscall.CopyGroup(group, ref __Group);
			return group;
		}

		public static int getgrnam_r(string name, Group grbuf, out Group grbufp)
		{
			Syscall._Group __Group;
			IntPtr intPtr;
			grbufp = null;
			int num = Syscall.sys_getgrnam_r(name, out __Group, out intPtr);
			if (num == 0 && intPtr != IntPtr.Zero)
			{
				Syscall.CopyGroup(grbuf, ref __Group);
				grbufp = grbuf;
			}
			return num;
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int getgroups(int size, uint[] list);

		public static int getgroups(uint[] list)
		{
			return Syscall.getgroups((int)list.Length, list);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_gethostid", ExactSpelling=false, SetLastError=true)]
		public static extern long gethostid();

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_gethostname", ExactSpelling=false, SetLastError=true)]
		public static extern int gethostname([Out] StringBuilder name, ulong len);

		public static int gethostname(StringBuilder name)
		{
			return Syscall.gethostname(name, (ulong)name.Capacity);
		}

		public static string getlogin()
		{
			string str;
			object getloginLock = Syscall.getlogin_lock;
			Monitor.Enter(getloginLock);
			try
			{
				str = UnixMarshal.PtrToString(Syscall.sys_getlogin());
			}
			finally
			{
				Monitor.Exit(getloginLock);
			}
			return str;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_getlogin_r", ExactSpelling=false, SetLastError=true)]
		public static extern int getlogin_r([Out] StringBuilder name, ulong bufsize);

		public static int getlogin_r(StringBuilder name)
		{
			return Syscall.getlogin_r(name, (ulong)name.Capacity);
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		[Obsolete("Dropped in POSIX 1003.1-2001.  Use Syscall.sysconf (SysconfName._SC_PAGESIZE).")]
		public static extern int getpagesize();

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int getpgid(int pid);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int getpgrp();

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int getpid();

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int getppid();

		public static Passwd getpwent()
		{
			Syscall._Passwd __Passwd;
			int num;
			object pwdLock = Syscall.pwd_lock;
			Monitor.Enter(pwdLock);
			try
			{
				num = Syscall.sys_getpwent(out __Passwd);
			}
			finally
			{
				Monitor.Exit(pwdLock);
			}
			if (num != 0)
			{
				return null;
			}
			Passwd passwd = new Passwd();
			Syscall.CopyPasswd(passwd, ref __Passwd);
			return passwd;
		}

		public static Passwd getpwnam(string name)
		{
			Syscall._Passwd __Passwd;
			int num;
			object pwdLock = Syscall.pwd_lock;
			Monitor.Enter(pwdLock);
			try
			{
				num = Syscall.sys_getpwnam(name, out __Passwd);
			}
			finally
			{
				Monitor.Exit(pwdLock);
			}
			if (num != 0)
			{
				return null;
			}
			Passwd passwd = new Passwd();
			Syscall.CopyPasswd(passwd, ref __Passwd);
			return passwd;
		}

		public static int getpwnam_r(string name, Passwd pwbuf, out Passwd pwbufp)
		{
			Syscall._Passwd __Passwd;
			IntPtr intPtr;
			pwbufp = null;
			int num = Syscall.sys_getpwnam_r(name, out __Passwd, out intPtr);
			if (num == 0 && intPtr != IntPtr.Zero)
			{
				Syscall.CopyPasswd(pwbuf, ref __Passwd);
				pwbufp = pwbuf;
			}
			return num;
		}

		public static Passwd getpwuid(uint uid)
		{
			Syscall._Passwd __Passwd;
			int num;
			object pwdLock = Syscall.pwd_lock;
			Monitor.Enter(pwdLock);
			try
			{
				num = Syscall.sys_getpwuid(uid, out __Passwd);
			}
			finally
			{
				Monitor.Exit(pwdLock);
			}
			if (num != 0)
			{
				return null;
			}
			Passwd passwd = new Passwd();
			Syscall.CopyPasswd(passwd, ref __Passwd);
			return passwd;
		}

		public static int getpwuid_r(uint uid, Passwd pwbuf, out Passwd pwbufp)
		{
			Syscall._Passwd __Passwd;
			IntPtr intPtr;
			pwbufp = null;
			int num = Syscall.sys_getpwuid_r(uid, out __Passwd, out intPtr);
			if (num == 0 && intPtr != IntPtr.Zero)
			{
				Syscall.CopyPasswd(pwbuf, ref __Passwd);
				pwbufp = pwbuf;
			}
			return num;
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int getresgid(out uint rgid, out uint egid, out uint sgid);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int getresuid(out uint ruid, out uint euid, out uint suid);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int getsid(int pid);

		private static string GetSyslogMessage(string message)
		{
			return UnixMarshal.EscapeFormatString(message, new char[] { 'm' });
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_gettimeofday", ExactSpelling=false, SetLastError=true)]
		public static extern int gettimeofday(out Timeval tv, out Timezone tz);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_gettimeofday", ExactSpelling=false, SetLastError=true)]
		private static extern int gettimeofday(out Timeval tv, IntPtr ignore);

		public static int gettimeofday(out Timeval tv)
		{
			return Syscall.gettimeofday(out tv, IntPtr.Zero);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_gettimeofday", ExactSpelling=false, SetLastError=true)]
		private static extern int gettimeofday(IntPtr ignore, out Timezone tz);

		public static int gettimeofday(out Timezone tz)
		{
			return Syscall.gettimeofday(IntPtr.Zero, out tz);
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern uint getuid();

		public static string getusershell()
		{
			string str;
			object usershellLock = Syscall.usershell_lock;
			Monitor.Enter(usershellLock);
			try
			{
				str = UnixMarshal.PtrToString(Syscall.sys_getusershell());
			}
			finally
			{
				Monitor.Exit(usershellLock);
			}
			return str;
		}

		private static void GetValues(byte[] list, Encoding encoding, out string[] values)
		{
			int num = 0;
			for (int i = 0; i < (int)list.Length; i++)
			{
				if (list[i] == 0)
				{
					num++;
				}
			}
			values = new string[num];
			num = 0;
			int num1 = 0;
			for (int j = 0; j < (int)list.Length; j++)
			{
				if (list[j] == 0)
				{
					int num2 = num;
					num = num2 + 1;
					values[num2] = encoding.GetString(list, num1, j - num1);
					num1 = j + 1;
				}
			}
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_getxattr", ExactSpelling=false, SetLastError=true)]
		public static extern long getxattr(string path, string name, byte[] value, ulong size);

		public static long getxattr(string path, string name, byte[] value)
		{
			return Syscall.getxattr(path, name, value, (ulong)((int)value.Length));
		}

		public static long getxattr(string path, string name, out byte[] value)
		{
			value = null;
			long num = Syscall.getxattr(path, name, value, (ulong)0);
			if (num <= (long)0)
			{
				return num;
			}
			value = new byte[checked((IntPtr)num)];
			return Syscall.getxattr(path, name, value, (ulong)num);
		}

		public static bool isatty(int fd)
		{
			return Syscall.sys_isatty(fd) == 1;
		}

		public static int kill(int pid, Signum sig)
		{
			return Syscall.sys_kill(pid, NativeConvert.FromSignum(sig));
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int lchown(string path, uint owner, uint group);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_lgetxattr", ExactSpelling=false, SetLastError=true)]
		public static extern long lgetxattr(string path, string name, byte[] value, ulong size);

		public static long lgetxattr(string path, string name, byte[] value)
		{
			return Syscall.lgetxattr(path, name, value, (ulong)((int)value.Length));
		}

		public static long lgetxattr(string path, string name, out byte[] value)
		{
			value = null;
			long num = Syscall.lgetxattr(path, name, value, (ulong)0);
			if (num <= (long)0)
			{
				return num;
			}
			value = new byte[checked((IntPtr)num)];
			return Syscall.lgetxattr(path, name, value, (ulong)num);
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int link(string oldpath, string newpath);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_listxattr", ExactSpelling=false, SetLastError=true)]
		public static extern long listxattr(string path, byte[] list, ulong size);

		public static long listxattr(string path, Encoding encoding, out string[] values)
		{
			values = null;
			long num = Syscall.listxattr(path, null, (ulong)0);
			if (num == 0)
			{
				values = new string[0];
			}
			if (num <= (long)0)
			{
				return (long)((int)num);
			}
			byte[] numArray = new byte[checked((IntPtr)num)];
			long num1 = Syscall.listxattr(path, numArray, (ulong)num);
			if (num1 < (long)0)
			{
				return (long)((int)num1);
			}
			Syscall.GetValues(numArray, encoding, out values);
			return (long)0;
		}

		public static long listxattr(string path, out string[] values)
		{
			return Syscall.listxattr(path, UnixEncoding.Instance, out values);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_llistxattr", ExactSpelling=false, SetLastError=true)]
		public static extern long llistxattr(string path, byte[] list, ulong size);

		public static long llistxattr(string path, Encoding encoding, out string[] values)
		{
			values = null;
			long num = Syscall.llistxattr(path, null, (ulong)0);
			if (num == 0)
			{
				values = new string[0];
			}
			if (num <= (long)0)
			{
				return (long)((int)num);
			}
			byte[] numArray = new byte[checked((IntPtr)num)];
			long num1 = Syscall.llistxattr(path, numArray, (ulong)num);
			if (num1 < (long)0)
			{
				return (long)((int)num1);
			}
			Syscall.GetValues(numArray, encoding, out values);
			return (long)0;
		}

		public static long llistxattr(string path, out string[] values)
		{
			return Syscall.llistxattr(path, UnixEncoding.Instance, out values);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_lockf", ExactSpelling=false, SetLastError=true)]
		public static extern int lockf(int fd, LockfCommand cmd, long len);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_lremovexattr", ExactSpelling=false, SetLastError=true)]
		public static extern int lremovexattr(string path, string name);

		public static long lseek(int fd, long offset, SeekFlags whence)
		{
			return Syscall.sys_lseek(fd, offset, NativeConvert.FromSeekFlags(whence));
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_lsetxattr", ExactSpelling=false, SetLastError=true)]
		public static extern int lsetxattr(string path, string name, byte[] value, ulong size, XattrFlags flags);

		public static int lsetxattr(string path, string name, byte[] value, ulong size)
		{
			return Syscall.lsetxattr(path, name, value, size, XattrFlags.XATTR_AUTO);
		}

		public static int lsetxattr(string path, string name, byte[] value, XattrFlags flags)
		{
			return Syscall.lsetxattr(path, name, value, (ulong)((int)value.Length), flags);
		}

		public static int lsetxattr(string path, string name, byte[] value)
		{
			return Syscall.lsetxattr(path, name, value, (ulong)((int)value.Length));
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_lstat", ExactSpelling=false, SetLastError=true)]
		public static extern int lstat(string file_name, out Stat buf);

		public static int lutimes(string filename, Timeval[] tvp)
		{
			if (tvp != null && (int)tvp.Length != 2)
			{
				Stdlib.SetLastError(Errno.EINVAL);
				return -1;
			}
			return Syscall.sys_lutimes(filename, tvp);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_mincore", ExactSpelling=false, SetLastError=true)]
		public static extern int mincore(IntPtr start, ulong length, byte[] vec);

		public static int mkdir(string oldpath, FilePermissions mode)
		{
			return Syscall.sys_mkdir(oldpath, NativeConvert.FromFilePermissions(mode));
		}

		public static int mkfifo(string pathname, FilePermissions mode)
		{
			return Syscall.sys_mkfifo(pathname, NativeConvert.FromFilePermissions(mode));
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_mknod", ExactSpelling=false, SetLastError=true)]
		public static extern int mknod(string pathname, FilePermissions mode, ulong dev);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int mkstemp(StringBuilder template);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_mlock", ExactSpelling=false, SetLastError=true)]
		public static extern int mlock(IntPtr start, ulong len);

		public static int mlockall(MlockallFlags flags)
		{
			return Syscall.sys_mlockall(NativeConvert.FromMlockallFlags(flags));
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_mmap", ExactSpelling=false, SetLastError=true)]
		public static extern IntPtr mmap(IntPtr start, ulong length, MmapProts prot, MmapFlags flags, int fd, long offset);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_mprotect", ExactSpelling=false, SetLastError=true)]
		public static extern int mprotect(IntPtr start, ulong len, MmapProts prot);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_mremap", ExactSpelling=false, SetLastError=true)]
		public static extern IntPtr mremap(IntPtr old_address, ulong old_size, ulong new_size, MremapFlags flags);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_msync", ExactSpelling=false, SetLastError=true)]
		public static extern int msync(IntPtr start, ulong len, MsyncFlags flags);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_munlock", ExactSpelling=false, SetLastError=true)]
		public static extern int munlock(IntPtr start, ulong len);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int munlockall();

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_munmap", ExactSpelling=false, SetLastError=true)]
		public static extern int munmap(IntPtr start, ulong length);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_nanosleep", ExactSpelling=false, SetLastError=true)]
		public static extern int nanosleep(ref Timespec req, ref Timespec rem);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int nice(int inc);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_open", ExactSpelling=false, SetLastError=true)]
		public static extern int open(string pathname, OpenFlags flags);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_open_mode", ExactSpelling=false, SetLastError=true)]
		public static extern int open(string pathname, OpenFlags flags, FilePermissions mode);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern IntPtr opendir(string name);

		public static int openlog(IntPtr ident, SyslogOptions option, SyslogFacility defaultFacility)
		{
			int num = NativeConvert.FromSyslogOptions(option);
			return Syscall.sys_openlog(ident, num, NativeConvert.FromSyslogFacility(defaultFacility));
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_pathconf", ExactSpelling=false, SetLastError=true)]
		public static extern long pathconf(string path, PathconfName name, Errno defaultError);

		public static long pathconf(string path, PathconfName name)
		{
			return Syscall.pathconf(path, name, 0);
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int pause();

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_pipe", ExactSpelling=false, SetLastError=true)]
		public static extern int pipe(out int reading, out int writing);

		public static int pipe(int[] filedes)
		{
			int num;
			int num1;
			if (filedes == null || (int)filedes.Length != 2)
			{
				return -1;
			}
			int num2 = Syscall.pipe(out num, out num1);
			filedes[0] = num;
			filedes[1] = num1;
			return num2;
		}

		public static int poll(Pollfd[] fds, uint nfds, int timeout)
		{
			unsafe
			{
				if ((long)((int)fds.Length) < (ulong)nfds)
				{
					throw new ArgumentOutOfRangeException("fds", "Must refer to at least `nfds' elements");
				}
				Syscall._pollfd[] _pollfdArray = new Syscall._pollfd[nfds];
				for (int i = 0; i < (int)_pollfdArray.Length; i++)
				{
					_pollfdArray[i].fd = fds[i].fd;
					_pollfdArray[i].events = NativeConvert.FromPollEvents(fds[i].events);
				}
				int num = Syscall.sys_poll(_pollfdArray, nfds, timeout);
				for (int j = 0; j < (int)_pollfdArray.Length; j++)
				{
					fds[j].revents = NativeConvert.ToPollEvents(_pollfdArray[j].revents);
				}
				return num;
			}
		}

		public static int poll(Pollfd[] fds, int timeout)
		{
			return Syscall.poll(fds, (uint)fds.Length, timeout);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_posix_fadvise", ExactSpelling=false, SetLastError=true)]
		public static extern int posix_fadvise(int fd, long offset, long len, PosixFadviseAdvice advice);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_posix_fallocate", ExactSpelling=false, SetLastError=true)]
		public static extern int posix_fallocate(int fd, long offset, ulong len);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_posix_madvise", ExactSpelling=false, SetLastError=true)]
		public static extern int posix_madvise(IntPtr addr, ulong len, PosixMadviseAdvice advice);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_pread", ExactSpelling=false, SetLastError=true)]
		public static extern long pread(int fd, IntPtr buf, ulong count, long offset);

		public static unsafe long pread(int fd, void* buf, ulong count, long offset)
		{
			return Syscall.pread(fd, (IntPtr)buf, count, offset);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_psignal", ExactSpelling=false, SetLastError=true)]
		private static extern int psignal(int sig, string s);

		public static int psignal(Signum sig, string s)
		{
			return Syscall.psignal(NativeConvert.FromSignum(sig), s);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_pwrite", ExactSpelling=false, SetLastError=true)]
		public static extern long pwrite(int fd, IntPtr buf, ulong count, long offset);

		public static unsafe long pwrite(int fd, void* buf, ulong count, long offset)
		{
			return Syscall.pwrite(fd, (IntPtr)buf, count, offset);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_read", ExactSpelling=false, SetLastError=true)]
		public static extern long read(int fd, IntPtr buf, ulong count);

		public static unsafe long read(int fd, void* buf, ulong count)
		{
			return Syscall.read(fd, (IntPtr)buf, count);
		}

		public static Dirent readdir(IntPtr dir)
		{
			Syscall._Dirent __Dirent;
			int num;
			object readdirLock = Syscall.readdir_lock;
			Monitor.Enter(readdirLock);
			try
			{
				num = Syscall.sys_readdir(dir, out __Dirent);
			}
			finally
			{
				Monitor.Exit(readdirLock);
			}
			if (num != 0)
			{
				return null;
			}
			Dirent dirent = new Dirent();
			Syscall.CopyDirent(dirent, ref __Dirent);
			return dirent;
		}

		public static int readdir_r(IntPtr dirp, Dirent entry, out IntPtr result)
		{
			Syscall._Dirent __Dirent;
			entry.d_ino = (ulong)0;
			entry.d_off = (long)0;
			entry.d_reclen = 0;
			entry.d_type = 0;
			entry.d_name = null;
			int num = Syscall.sys_readdir_r(dirp, out __Dirent, out result);
			if (num == 0 && (void*)result != IntPtr.Zero)
			{
				Syscall.CopyDirent(entry, ref __Dirent);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_readlink", ExactSpelling=false, SetLastError=true)]
		public static extern int readlink(string path, [Out] StringBuilder buf, ulong bufsiz);

		public static int readlink(string path, [Out] StringBuilder buf)
		{
			return Syscall.readlink(path, buf, (ulong)buf.Capacity);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_remap_file_pages", ExactSpelling=false, SetLastError=true)]
		public static extern int remap_file_pages(IntPtr start, ulong size, MmapProts prot, long pgoff, MmapFlags flags);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_removexattr", ExactSpelling=false, SetLastError=true)]
		public static extern int removexattr(string path, string name);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int revoke(string file);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_rewinddir", ExactSpelling=false, SetLastError=true)]
		public static extern int rewinddir(IntPtr dir);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int rmdir(string pathname);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern IntPtr sbrk(IntPtr increment);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_seekdir", ExactSpelling=false, SetLastError=true)]
		public static extern int seekdir(IntPtr dir, long offset);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_sendfile", ExactSpelling=false, SetLastError=true)]
		public static extern long sendfile(int out_fd, int in_fd, ref long offset, ulong count);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_setdomainname", ExactSpelling=false, SetLastError=true)]
		public static extern int setdomainname(string name, ulong len);

		public static int setdomainname(string name)
		{
			return Syscall.setdomainname(name, (ulong)name.Length);
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int setegid(uint uid);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int seteuid(uint euid);

		public static int setfsent()
		{
			int num;
			object fstabLock = Syscall.fstab_lock;
			Monitor.Enter(fstabLock);
			try
			{
				num = Syscall.sys_setfsent();
			}
			finally
			{
				Monitor.Exit(fstabLock);
			}
			return num;
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int setgid(uint gid);

		public static int setgrent()
		{
			int num;
			object grpLock = Syscall.grp_lock;
			Monitor.Enter(grpLock);
			try
			{
				num = Syscall.sys_setgrent();
			}
			finally
			{
				Monitor.Exit(grpLock);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_setgroups", ExactSpelling=false, SetLastError=true)]
		public static extern int setgroups(ulong size, uint[] list);

		public static int setgroups(uint[] list)
		{
			return Syscall.setgroups((ulong)((int)list.Length), list);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_sethostid", ExactSpelling=false, SetLastError=true)]
		public static extern int sethostid(long hostid);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_sethostname", ExactSpelling=false, SetLastError=true)]
		public static extern int sethostname(string name, ulong len);

		public static int sethostname(string name)
		{
			return Syscall.sethostname(name, (ulong)name.Length);
		}

		[Obsolete("This is insecure and should not be used", true)]
		public static int setkey(string key)
		{
			throw new SecurityException("crypt(3) has been broken.  Use something more secure.");
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int setlogin(string name);

		public static int setlogmask(SyslogLevel mask)
		{
			return Syscall.sys_setlogmask(NativeConvert.FromSyslogLevel(mask));
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int setpgid(int pid, int pgid);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int setpgrp();

		public static int setpwent()
		{
			int num;
			object pwdLock = Syscall.pwd_lock;
			Monitor.Enter(pwdLock);
			try
			{
				num = Syscall.sys_setpwent();
			}
			finally
			{
				Monitor.Exit(pwdLock);
			}
			return num;
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int setregid(uint rgid, uint egid);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int setresgid(uint rgid, uint egid, uint sgid);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int setresuid(uint ruid, uint euid, uint suid);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int setreuid(uint ruid, uint euid);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int setsid();

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_settimeofday", ExactSpelling=false, SetLastError=true)]
		public static extern int settimeofday(ref Timeval tv, ref Timezone tz);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_gettimeofday", ExactSpelling=false, SetLastError=true)]
		private static extern int settimeofday(ref Timeval tv, IntPtr ignore);

		public static int settimeofday(ref Timeval tv)
		{
			return Syscall.settimeofday(ref tv, IntPtr.Zero);
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int setuid(uint uid);

		public static int setusershell()
		{
			int num;
			object usershellLock = Syscall.usershell_lock;
			Monitor.Enter(usershellLock);
			try
			{
				num = Syscall.sys_setusershell();
			}
			finally
			{
				Monitor.Exit(usershellLock);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_setxattr", ExactSpelling=false, SetLastError=true)]
		public static extern int setxattr(string path, string name, byte[] value, ulong size, XattrFlags flags);

		public static int setxattr(string path, string name, byte[] value, ulong size)
		{
			return Syscall.setxattr(path, name, value, size, XattrFlags.XATTR_AUTO);
		}

		public static int setxattr(string path, string name, byte[] value, XattrFlags flags)
		{
			return Syscall.setxattr(path, name, value, (ulong)((int)value.Length), flags);
		}

		public static int setxattr(string path, string name, byte[] value)
		{
			return Syscall.setxattr(path, name, value, (ulong)((int)value.Length));
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern uint sleep(uint seconds);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_stat", ExactSpelling=false, SetLastError=true)]
		public static extern int stat(string file_name, out Stat buf);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_statvfs", ExactSpelling=false, SetLastError=true)]
		public static extern int statvfs(string path, out Statvfs buf);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_stime", ExactSpelling=false, SetLastError=true)]
		public static extern int stime(ref long t);

		public static int strerror_r(Errno errnum, StringBuilder buf, ulong n)
		{
			return Syscall.sys_strerror_r(NativeConvert.FromErrno(errnum), buf, n);
		}

		public static int strerror_r(Errno errnum, StringBuilder buf)
		{
			return Syscall.strerror_r(errnum, buf, (ulong)buf.Capacity);
		}

		public static string strsignal(Signum sig)
		{
			string str;
			int num = NativeConvert.FromSignum(sig);
			object signalLock = Syscall.signal_lock;
			Monitor.Enter(signalLock);
			try
			{
				str = UnixMarshal.PtrToString(Syscall.sys_strsignal(num));
			}
			finally
			{
				Monitor.Exit(signalLock);
			}
			return str;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_swab", ExactSpelling=false, SetLastError=true)]
		public static extern int swab(IntPtr from, IntPtr to, long n);

		public static unsafe void swab(void* from, void* to, long n)
		{
			Syscall.swab((IntPtr)from, (IntPtr)to, n);
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int symlink(string oldpath, string newpath);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_sync", ExactSpelling=false, SetLastError=true)]
		public static extern int sync();

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="access", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_access(string pathname, int mode);

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="chmod", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_chmod(string path, uint mode);

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="cuserid", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr sys_cuserid([Out] StringBuilder @string);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_endfsent", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_endfsent();

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_endgrent", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_endgrent();

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_endpwent", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_endpwent();

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_endusershell", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_endusershell();

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="fchmod", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_fchmod(int filedes, uint mode);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_fgetgrent", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_fgetgrent(IntPtr stream, out Syscall._Group grbuf);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_fgetpwent", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_fgetpwent(IntPtr stream, out Syscall._Passwd pwbuf);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_futimes", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_futimes(int fd, Timeval[] tvp);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_getfsent", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_getfsent(out Syscall._Fstab fs);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_getfsfile", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_getfsfile(string mount_point, out Syscall._Fstab fs);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_getfsspec", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_getfsspec(string special_file, out Syscall._Fstab fs);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_getgrent", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_getgrent(out Syscall._Group grbuf);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_getgrgid", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_getgrgid(uint uid, out Syscall._Group group);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_getgrgid_r", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_getgrgid_r(uint uid, out Syscall._Group grbuf, out IntPtr grbufp);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_getgrnam", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_getgrnam(string name, out Syscall._Group group);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_getgrnam_r", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_getgrnam_r(string name, out Syscall._Group grbuf, out IntPtr grbufp);

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="getlogin", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr sys_getlogin();

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_getpwent", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_getpwent(out Syscall._Passwd pwbuf);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_getpwnam", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_getpwnam(string name, out Syscall._Passwd passwd);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_getpwnam_r", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_getpwnam_r(string name, out Syscall._Passwd pwbuf, out IntPtr pwbufp);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_getpwuid", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_getpwuid(uint uid, out Syscall._Passwd passwd);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_getpwuid_r", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_getpwuid_r(uint uid, out Syscall._Passwd pwbuf, out IntPtr pwbufp);

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="getusershell", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr sys_getusershell();

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="isatty", ExactSpelling=false)]
		private static extern int sys_isatty(int fd);

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="kill", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_kill(int pid, int sig);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_lseek", ExactSpelling=false, SetLastError=true)]
		private static extern long sys_lseek(int fd, long offset, int whence);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_lutimes", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_lutimes(string filename, Timeval[] tvp);

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="mkdir", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_mkdir(string oldpath, uint mode);

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="mkfifo", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_mkfifo(string pathname, uint mode);

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="mlockall", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_mlockall(int flags);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_openlog", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_openlog(IntPtr ident, int option, int facility);

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="poll", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_poll(Syscall._pollfd[] ufds, uint nfds, int timeout);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_readdir", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_readdir(IntPtr dir, out Syscall._Dirent dentry);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_readdir_r", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_readdir_r(IntPtr dirp, out Syscall._Dirent entry, out IntPtr result);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_setfsent", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_setfsent();

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_setgrent", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_setgrent();

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="setlogmask", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_setlogmask(int mask);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_setpwent", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_setpwent();

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_setusershell", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_setusershell();

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_strerror_r", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_strerror_r(int errnum, [Out] StringBuilder buf, ulong n);

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="strsignal", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr sys_strsignal(int sig);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_syslog", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_syslog(int priority, string message);

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="ttyname", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr sys_ttyname(int fd);

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="umask", ExactSpelling=false, SetLastError=true)]
		private static extern uint sys_umask(uint mask);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_uname", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_uname(out Syscall._Utsname buf);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_utime", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_utime(string filename, ref Utimbuf buf, int use_buf);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_utimes", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_utimes(string filename, Timeval[] tvp);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_sysconf", ExactSpelling=false, SetLastError=true)]
		public static extern long sysconf(SysconfName name, Errno defaultError);

		public static long sysconf(SysconfName name)
		{
			return Syscall.sysconf(name, 0);
		}

		public static int syslog(SyslogFacility facility, SyslogLevel level, string message)
		{
			int num = NativeConvert.FromSyslogFacility(facility);
			int num1 = NativeConvert.FromSyslogLevel(level);
			return Syscall.sys_syslog(num | num1, Syscall.GetSyslogMessage(message));
		}

		public static int syslog(SyslogLevel level, string message)
		{
			int num = NativeConvert.FromSyslogLevel(level);
			return Syscall.sys_syslog(num, Syscall.GetSyslogMessage(message));
		}

		[Obsolete("Not necessarily portable due to cdecl restrictions.\nUse syslog(SyslogFacility, SyslogLevel, string) instead.")]
		public static int syslog(SyslogFacility facility, SyslogLevel level, string format, params object[] parameters)
		{
			int num = NativeConvert.FromSyslogFacility(facility);
			int num1 = NativeConvert.FromSyslogLevel(level);
			object[] objArray = new object[checked((int)parameters.Length + 2)];
			objArray[0] = num | num1;
			objArray[1] = format;
			Array.Copy(parameters, 0, objArray, 2, (int)parameters.Length);
			return (int)XPrintfFunctions.syslog(objArray);
		}

		[Obsolete("Not necessarily portable due to cdecl restrictions.\nUse syslog(SyslogLevel, string) instead.")]
		public static int syslog(SyslogLevel level, string format, params object[] parameters)
		{
			int num = NativeConvert.FromSyslogLevel(level);
			object[] objArray = new object[checked((int)parameters.Length + 2)];
			objArray[0] = num;
			objArray[1] = format;
			Array.Copy(parameters, 0, objArray, 2, (int)parameters.Length);
			return (int)XPrintfFunctions.syslog(objArray);
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int tcgetpgrp(int fd);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int tcsetpgrp(int fd, int pgrp);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_telldir", ExactSpelling=false, SetLastError=true)]
		public static extern long telldir(IntPtr dir);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_time", ExactSpelling=false, SetLastError=true)]
		public static extern long time(out long t);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_truncate", ExactSpelling=false, SetLastError=true)]
		public static extern int truncate(string path, long length);

		public static string ttyname(int fd)
		{
			string str;
			object ttyLock = Syscall.tty_lock;
			Monitor.Enter(ttyLock);
			try
			{
				str = UnixMarshal.PtrToString(Syscall.sys_ttyname(fd));
			}
			finally
			{
				Monitor.Exit(ttyLock);
			}
			return str;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_ttyname_r", ExactSpelling=false, SetLastError=true)]
		public static extern int ttyname_r(int fd, [Out] StringBuilder buf, ulong buflen);

		public static int ttyname_r(int fd, StringBuilder buf)
		{
			return Syscall.ttyname_r(fd, buf, (ulong)buf.Capacity);
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int ttyslot();

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern uint ualarm(uint usecs, uint interval);

		public static FilePermissions umask(FilePermissions mask)
		{
			uint num = NativeConvert.FromFilePermissions(mask);
			return NativeConvert.ToFilePermissions(Syscall.sys_umask(num));
		}

		public static int uname(out Utsname buf)
		{
			Syscall._Utsname __Utsname;
			int num = Syscall.sys_uname(out __Utsname);
			buf = new Utsname();
			if (num == 0)
			{
				Syscall.CopyUtsname(ref buf, ref __Utsname);
			}
			return num;
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int unlink(string pathname);

		public static int utime(string filename, ref Utimbuf buf)
		{
			return Syscall.sys_utime(filename, ref buf, 1);
		}

		public static int utime(string filename)
		{
			Utimbuf utimbuf = new Utimbuf();
			return Syscall.sys_utime(filename, ref utimbuf, 0);
		}

		public static int utimes(string filename, Timeval[] tvp)
		{
			if (tvp != null && (int)tvp.Length != 2)
			{
				Stdlib.SetLastError(Errno.EINVAL);
				return -1;
			}
			return Syscall.sys_utimes(filename, tvp);
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int vhangup();

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int wait(out int status);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern int waitpid(int pid, out int status, int options);

		public static int waitpid(int pid, out int status, WaitOptions options)
		{
			return Syscall.waitpid(pid, out status, NativeConvert.FromWaitOptions(options));
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_WEXITSTATUS", ExactSpelling=false)]
		public static extern int WEXITSTATUS(int status);

		public static bool WIFEXITED(int status)
		{
			return Syscall._WIFEXITED(status) != 0;
		}

		public static bool WIFSIGNALED(int status)
		{
			return Syscall._WIFSIGNALED(status) != 0;
		}

		public static bool WIFSTOPPED(int status)
		{
			return Syscall._WIFSTOPPED(status) != 0;
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="Mono_Posix_Syscall_write", ExactSpelling=false, SetLastError=true)]
		public static extern long write(int fd, IntPtr buf, ulong count);

		public static unsafe long write(int fd, void* buf, ulong count)
		{
			return Syscall.write(fd, (IntPtr)buf, count);
		}

		public static Signum WSTOPSIG(int status)
		{
			return NativeConvert.ToSignum(Syscall._WSTOPSIG(status));
		}

		public static Signum WTERMSIG(int status)
		{
			return NativeConvert.ToSignum(Syscall._WTERMSIG(status));
		}

		private struct _Dirent
		{
			[ino_t]
			public ulong d_ino;

			[off_t]
			public long d_off;

			public ushort d_reclen;

			public byte d_type;

			public IntPtr d_name;
		}

		[Map]
		private struct _Fstab
		{
			public IntPtr fs_spec;

			public IntPtr fs_file;

			public IntPtr fs_vfstype;

			public IntPtr fs_mntops;

			public IntPtr fs_type;

			public int fs_freq;

			public int fs_passno;

			public IntPtr _fs_buf_;
		}

		[Map]
		private struct _Group
		{
			public IntPtr gr_name;

			public IntPtr gr_passwd;

			[gid_t]
			public uint gr_gid;

			public int _gr_nmem_;

			public IntPtr gr_mem;

			public IntPtr _gr_buf_;
		}

		[Map]
		private struct _Passwd
		{
			public IntPtr pw_name;

			public IntPtr pw_passwd;

			[uid_t]
			public uint pw_uid;

			[gid_t]
			public uint pw_gid;

			public IntPtr pw_gecos;

			public IntPtr pw_dir;

			public IntPtr pw_shell;

			public IntPtr _pw_buf_;
		}

		private struct _pollfd
		{
			public int fd;

			public short events;

			public short revents;
		}

		[Map]
		private struct _Utsname
		{
			public IntPtr sysname;

			public IntPtr nodename;

			public IntPtr release;

			public IntPtr version;

			public IntPtr machine;

			public IntPtr domainname;

			public IntPtr _buf_;
		}
	}
}