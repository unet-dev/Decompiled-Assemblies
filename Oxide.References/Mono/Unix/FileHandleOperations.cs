using Mono.Unix.Native;
using System;
using System.IO;

namespace Mono.Unix
{
	public sealed class FileHandleOperations
	{
		private FileHandleOperations()
		{
		}

		public static void AdviseFileAccessPattern(int fd, FileAccessPattern pattern, long offset, long len)
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.posix_fadvise(fd, offset, len, (PosixFadviseAdvice)pattern));
		}

		public static void AdviseFileAccessPattern(int fd, FileAccessPattern pattern)
		{
			FileHandleOperations.AdviseFileAccessPattern(fd, pattern, (long)0, (long)0);
		}

		public static void AdviseFileAccessPattern(FileStream file, FileAccessPattern pattern, long offset, long len)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			IntPtr handle = file.Handle;
			int num = Syscall.posix_fadvise(handle.ToInt32(), offset, len, (PosixFadviseAdvice)pattern);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		public static void AdviseFileAccessPattern(FileStream file, FileAccessPattern pattern)
		{
			FileHandleOperations.AdviseFileAccessPattern(file, pattern, (long)0, (long)0);
		}

		public static void AdviseFileAccessPattern(UnixStream stream, FileAccessPattern pattern, long offset, long len)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			int num = Syscall.posix_fadvise(stream.Handle, offset, len, (PosixFadviseAdvice)pattern);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		public static void AdviseFileAccessPattern(UnixStream stream, FileAccessPattern pattern)
		{
			FileHandleOperations.AdviseFileAccessPattern(stream, pattern, (long)0, (long)0);
		}
	}
}