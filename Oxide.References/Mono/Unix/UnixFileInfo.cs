using Mono.Unix.Native;
using System;
using System.IO;

namespace Mono.Unix
{
	public sealed class UnixFileInfo : UnixFileSystemInfo
	{
		public UnixDirectoryInfo Directory
		{
			get
			{
				return new UnixDirectoryInfo(this.DirectoryName);
			}
		}

		public string DirectoryName
		{
			get
			{
				return UnixPath.GetDirectoryName(base.FullPath);
			}
		}

		public override string Name
		{
			get
			{
				return UnixPath.GetFileName(base.FullPath);
			}
		}

		public UnixFileInfo(string path) : base(path)
		{
		}

		internal UnixFileInfo(string path, Stat stat) : base(path, stat)
		{
		}

		public UnixStream Create()
		{
			return this.Create(FilePermissions.S_IRUSR | FilePermissions.S_IWUSR | FilePermissions.S_IRGRP | FilePermissions.S_IROTH);
		}

		[CLSCompliant(false)]
		public UnixStream Create(FilePermissions mode)
		{
			int num = Syscall.creat(base.FullPath, mode);
			if (num < 0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			base.Refresh();
			return new UnixStream(num);
		}

		public UnixStream Create(Mono.Unix.FileAccessPermissions mode)
		{
			return this.Create((FilePermissions)mode);
		}

		public override void Delete()
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.unlink(base.FullPath));
			base.Refresh();
		}

		[CLSCompliant(false)]
		public UnixStream Open(OpenFlags flags)
		{
			if ((flags & OpenFlags.O_CREAT) != OpenFlags.O_RDONLY)
			{
				throw new ArgumentException("Cannot specify OpenFlags.O_CREAT without providing FilePermissions.  Use the Open(OpenFlags, FilePermissions) method instead");
			}
			int num = Syscall.open(base.FullPath, flags);
			if (num < 0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return new UnixStream(num);
		}

		[CLSCompliant(false)]
		public UnixStream Open(OpenFlags flags, FilePermissions mode)
		{
			int num = Syscall.open(base.FullPath, flags, mode);
			if (num < 0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return new UnixStream(num);
		}

		public UnixStream Open(FileMode mode)
		{
			return this.Open(NativeConvert.ToOpenFlags(mode, FileAccess.ReadWrite));
		}

		public UnixStream Open(FileMode mode, FileAccess access)
		{
			return this.Open(NativeConvert.ToOpenFlags(mode, access));
		}

		[CLSCompliant(false)]
		public UnixStream Open(FileMode mode, FileAccess access, FilePermissions perms)
		{
			OpenFlags openFlags = NativeConvert.ToOpenFlags(mode, access);
			int num = Syscall.open(base.FullPath, openFlags, perms);
			if (num < 0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return new UnixStream(num);
		}

		public UnixStream OpenRead()
		{
			return this.Open(FileMode.Open, FileAccess.Read);
		}

		public UnixStream OpenWrite()
		{
			return this.Open(FileMode.OpenOrCreate, FileAccess.Write);
		}
	}
}