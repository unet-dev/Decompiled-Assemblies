using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Mono.Posix
{
	[CLSCompliant(false)]
	[Obsolete("Use Mono.Unix.Native.Syscall.")]
	public class Syscall
	{
		public Syscall()
		{
		}

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="strerror", ExactSpelling=false)]
		private static extern IntPtr _strerror(int errnum);

		public static int access(string pathname, AccessMode mode)
		{
			return Syscall.syscall_access(pathname, Syscall.map_Mono_Posix_AccessMode(mode));
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern uint alarm(uint seconds);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int chdir(string path);

		public static int chmod(string path, FileMode mode)
		{
			return Syscall.syscall_chmod(path, Syscall.map_Mono_Posix_FileMode(mode));
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int chown(string path, int owner, int group);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int chroot(string path);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int close(int fileDescriptor);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int closedir(IntPtr dir);

		public static int creat(string pathname, FileMode flags)
		{
			return Syscall.syscall_creat(pathname, Syscall.map_Mono_Posix_FileMode(flags));
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int dup(int fileDescriptor);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int dup2(int oldFileDescriptor, int newFileDescriptor);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int exit(int status);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int fork();

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int getegid();

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int geteuid();

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int getgid();

		public static string getgroupname(int gid)
		{
			return Syscall.helper_Mono_Posix_GetGroupName(gid);
		}

		[CLSCompliant(false)]
		public static string gethostname()
		{
			return Syscall.GetHostName();
		}

		public static string GetHostName()
		{
			byte[] numArray = new byte[256];
			int num = Syscall.syscall_gethostname(numArray, (int)numArray.Length);
			if (num == -1)
			{
				return "localhost";
			}
			num = 0;
			while (num < (int)numArray.Length)
			{
				if (numArray[num] != 0)
				{
					num++;
				}
				else
				{
					break;
				}
			}
			return Encoding.UTF8.GetString(numArray, 0, num);
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int getpgrp();

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int getpid();

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int getppid();

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int getuid();

		public static string getusername(int uid)
		{
			return Syscall.helper_Mono_Posix_GetUserName(uid);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern string helper_Mono_Posix_GetGroupName(int gid);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern string helper_Mono_Posix_GetUserName(int uid);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern int helper_Mono_Posix_Stat(string filename, bool dereference, out int device, out int inode, out int mode, out int nlinks, out int uid, out int gid, out int rdev, out long size, out long blksize, out long blocks, out long atime, out long mtime, out long ctime);

		public static bool isatty(int desc)
		{
			if (Syscall.syscall_isatty(desc) == 1)
			{
				return true;
			}
			return false;
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern void kill(int pid, int sig);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int lchown(string path, int owner, int group);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int link(string oldPath, string newPath);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int lseek(int fileDescriptor, int offset, int whence);

		public static int lstat(string filename, out Stat stat)
		{
			return Syscall.stat2(filename, true, out stat);
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern int map_Mono_Posix_AccessMode(AccessMode mode);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern int map_Mono_Posix_FileMode(FileMode mode);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern int map_Mono_Posix_OpenFlags(OpenFlags flags);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern int map_Mono_Posix_WaitOptions(WaitOptions wait_options);

		public static int mkdir(string pathname, FileMode mode)
		{
			return Syscall.syscall_mkdir(pathname, Syscall.map_Mono_Posix_FileMode(mode));
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int nice(int increment);

		public static int open(string pathname, OpenFlags flags)
		{
			if ((flags & OpenFlags.O_CREAT) != OpenFlags.O_RDONLY)
			{
				throw new ArgumentException("If you pass O_CREAT, you must call the method with the mode flag");
			}
			return Syscall.syscall_open(pathname, Syscall.map_Mono_Posix_OpenFlags(flags), 0);
		}

		public static int open(string pathname, OpenFlags flags, FileMode mode)
		{
			int num = Syscall.map_Mono_Posix_OpenFlags(flags);
			return Syscall.syscall_open(pathname, num, Syscall.map_Mono_Posix_FileMode(mode));
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr opendir(string path);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int pause();

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern unsafe IntPtr read(int fileDescriptor, void* buf, IntPtr count);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="helper_Mono_Posix_readdir", ExactSpelling=false)]
		public static extern string readdir(IntPtr dir);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern int readlink(string path, byte[] buffer, int buflen);

		public static string readlink(string path)
		{
			byte[] numArray = new byte[512];
			int num = Syscall.readlink(path, numArray, (int)numArray.Length);
			if (num == -1)
			{
				return null;
			}
			char[] chrArray = new char[512];
			int chars = Encoding.Default.GetChars(numArray, 0, num, chrArray, 0);
			return new string(chrArray, 0, chars);
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int rename(string oldPath, string newPath);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int rmdir(string path);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int setgid(int gid);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int setpgid(int pid, int pgid);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int setregid(int rgid, int egid);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int setreuid(int ruid, int euid);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int setsid();

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int setuid(int uid);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int signal(int signum, Syscall.sighandler_t handler);

		public static int stat(string filename, out Stat stat)
		{
			return Syscall.stat2(filename, false, out stat);
		}

		private static int stat2(string filename, bool dereference, out Stat stat)
		{
			int num;
			int num1;
			int num2;
			int num3;
			int num4;
			int num5;
			int num6;
			long num7;
			long num8;
			long num9;
			long num10;
			long num11;
			long num12;
			int num13 = Syscall.helper_Mono_Posix_Stat(filename, dereference, out num, out num1, out num2, out num3, out num4, out num5, out num6, out num7, out num8, out num9, out num10, out num11, out num12);
			stat = new Stat(num, num1, num2, num3, num4, num5, num6, num7, num8, num9, num10, num11, num12);
			if (num13 != 0)
			{
				return num13;
			}
			return 0;
		}

		public static string strerror(int errnum)
		{
			return Marshal.PtrToStringAnsi(Syscall._strerror(errnum));
		}

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int symlink(string oldpath, string newpath);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern void sync();

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="access", ExactSpelling=false, SetLastError=true)]
		internal static extern int syscall_access(string pathname, int mode);

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="chmod", ExactSpelling=false, SetLastError=true)]
		internal static extern int syscall_chmod(string path, int mode);

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="creat", ExactSpelling=false, SetLastError=true)]
		internal static extern int syscall_creat(string pathname, int flags);

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="gethostname", ExactSpelling=false, SetLastError=true)]
		private static extern int syscall_gethostname(byte[] p, int len);

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="isatty", ExactSpelling=false)]
		private static extern int syscall_isatty(int desc);

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="mkdir", ExactSpelling=false, SetLastError=true)]
		internal static extern int syscall_mkdir(string pathname, int mode);

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="open", ExactSpelling=false, SetLastError=true)]
		internal static extern int syscall_open(string pathname, int flags, int mode);

		[DllImport("libc", CharSet=CharSet.None, EntryPoint="waitpid", ExactSpelling=false, SetLastError=true)]
		internal static extern unsafe int syscall_waitpid(int pid, int* status, int options);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int umask(int umask);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int unlink(string path);

		public static int waitpid(int pid, out int status, WaitOptions options)
		{
			int num = 0;
			int num1 = Syscall.syscall_waitpid(pid, ref num, Syscall.map_Mono_Posix_WaitOptions(options));
			status = num;
			return num1;
		}

		public static int waitpid(int pid, WaitOptions options)
		{
			unsafe
			{
				return Syscall.syscall_waitpid(pid, 0, Syscall.map_Mono_Posix_WaitOptions(options));
			}
		}

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="wexitstatus", ExactSpelling=false)]
		public static extern int WEXITSTATUS(int status);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="wifexited", ExactSpelling=false)]
		public static extern int WIFEXITED(int status);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="wifsignaled", ExactSpelling=false)]
		public static extern int WIFSIGNALED(int status);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="wifstopped", ExactSpelling=false)]
		public static extern int WIFSTOPPED(int status);

		[DllImport("libc", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern unsafe IntPtr write(int fileDescriptor, void* buf, IntPtr count);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="wstopsig", ExactSpelling=false)]
		public static extern int WSTOPSIG(int status);

		[DllImport("MonoPosixHelper", CharSet=CharSet.None, EntryPoint="wtermsig", ExactSpelling=false)]
		public static extern int WTERMSIG(int status);

		public delegate void sighandler_t(int v);
	}
}