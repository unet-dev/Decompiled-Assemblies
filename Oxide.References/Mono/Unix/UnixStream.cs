using Mono.Unix.Native;
using System;
using System.IO;

namespace Mono.Unix
{
	public sealed class UnixStream : Stream, IDisposable
	{
		public const int InvalidFileDescriptor = -1;

		public const int StandardInputFileDescriptor = 0;

		public const int StandardOutputFileDescriptor = 1;

		public const int StandardErrorFileDescriptor = 2;

		private bool canSeek;

		private bool canRead;

		private bool canWrite;

		private bool owner = true;

		private int fileDescriptor = -1;

		private Stat stat;

		public override bool CanRead
		{
			get
			{
				return this.canRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.canSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.canWrite;
			}
		}

		public Mono.Unix.FileAccessPermissions FileAccessPermissions
		{
			get
			{
				return (Mono.Unix.FileAccessPermissions)(this.Protection & FilePermissions.ACCESSPERMS);
			}
			set
			{
				this.Protection = (FilePermissions)(this.Protection & (FilePermissions.S_ISUID | FilePermissions.S_ISGID | FilePermissions.S_ISVTX | FilePermissions.S_IFMT | FilePermissions.S_IFDIR | FilePermissions.S_IFCHR | FilePermissions.S_IFBLK | FilePermissions.S_IFREG | FilePermissions.S_IFIFO | FilePermissions.S_IFLNK | FilePermissions.S_IFSOCK) | (int)value);
			}
		}

		public Mono.Unix.FileSpecialAttributes FileSpecialAttributes
		{
			get
			{
				return (Mono.Unix.FileSpecialAttributes)(this.Protection & (FilePermissions.S_ISUID | FilePermissions.S_ISGID | FilePermissions.S_ISVTX));
			}
			set
			{
				this.Protection = (FilePermissions)(this.Protection & (FilePermissions.S_IRUSR | FilePermissions.S_IWUSR | FilePermissions.S_IXUSR | FilePermissions.S_IRGRP | FilePermissions.S_IWGRP | FilePermissions.S_IXGRP | FilePermissions.S_IROTH | FilePermissions.S_IWOTH | FilePermissions.S_IXOTH | FilePermissions.S_IRWXG | FilePermissions.S_IRWXU | FilePermissions.S_IRWXO | FilePermissions.ACCESSPERMS | FilePermissions.DEFFILEMODE | FilePermissions.S_IFMT | FilePermissions.S_IFDIR | FilePermissions.S_IFCHR | FilePermissions.S_IFBLK | FilePermissions.S_IFREG | FilePermissions.S_IFIFO | FilePermissions.S_IFLNK | FilePermissions.S_IFSOCK) | (int)value);
			}
		}

		public FileTypes FileType
		{
			get
			{
				return (FileTypes)(this.Protection & FilePermissions.S_IFMT);
			}
		}

		public int Handle
		{
			get
			{
				return this.fileDescriptor;
			}
		}

		public override long Length
		{
			get
			{
				this.AssertNotDisposed();
				if (!this.CanSeek)
				{
					throw new NotSupportedException("File descriptor doesn't support seeking");
				}
				this.RefreshStat();
				return this.stat.st_size;
			}
		}

		public UnixGroupInfo OwnerGroup
		{
			get
			{
				this.RefreshStat();
				return new UnixGroupInfo((long)this.stat.st_gid);
			}
		}

		public long OwnerGroupId
		{
			get
			{
				this.RefreshStat();
				return (long)this.stat.st_gid;
			}
		}

		public UnixUserInfo OwnerUser
		{
			get
			{
				this.RefreshStat();
				return new UnixUserInfo(this.stat.st_uid);
			}
		}

		public long OwnerUserId
		{
			get
			{
				this.RefreshStat();
				return (long)this.stat.st_uid;
			}
		}

		public override long Position
		{
			get
			{
				this.AssertNotDisposed();
				if (!this.CanSeek)
				{
					throw new NotSupportedException("The stream does not support seeking");
				}
				long num = Syscall.lseek(this.fileDescriptor, (long)0, SeekFlags.SEEK_CUR);
				if (num == (long)-1)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
				return num;
			}
			set
			{
				this.Seek(value, SeekOrigin.Begin);
			}
		}

		[CLSCompliant(false)]
		public FilePermissions Protection
		{
			get
			{
				this.RefreshStat();
				return this.stat.st_mode;
			}
			set
			{
				value = value & (FilePermissions.S_ISUID | FilePermissions.S_ISGID | FilePermissions.S_ISVTX | FilePermissions.S_IRUSR | FilePermissions.S_IWUSR | FilePermissions.S_IXUSR | FilePermissions.S_IRGRP | FilePermissions.S_IWGRP | FilePermissions.S_IXGRP | FilePermissions.S_IROTH | FilePermissions.S_IWOTH | FilePermissions.S_IXOTH | FilePermissions.S_IRWXG | FilePermissions.S_IRWXU | FilePermissions.S_IRWXO | FilePermissions.ACCESSPERMS | FilePermissions.ALLPERMS | FilePermissions.DEFFILEMODE);
				UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.fchmod(this.fileDescriptor, value));
			}
		}

		public UnixStream(int fileDescriptor) : this(fileDescriptor, true)
		{
		}

		public UnixStream(int fileDescriptor, bool ownsHandle)
		{
			if (fileDescriptor == -1)
			{
				throw new ArgumentException(Locale.GetText("Invalid file descriptor"), "fileDescriptor");
			}
			this.fileDescriptor = fileDescriptor;
			this.owner = ownsHandle;
			if (Syscall.lseek(fileDescriptor, (long)0, SeekFlags.SEEK_CUR) != (long)-1)
			{
				this.canSeek = true;
			}
			if (Syscall.read(fileDescriptor, IntPtr.Zero, (ulong)0) != (long)-1)
			{
				this.canRead = true;
			}
			if (Syscall.write(fileDescriptor, IntPtr.Zero, (ulong)0) != (long)-1)
			{
				this.canWrite = true;
			}
		}

		public void AdviseFileAccessPattern(FileAccessPattern pattern, long offset, long len)
		{
			FileHandleOperations.AdviseFileAccessPattern(this.fileDescriptor, pattern, offset, len);
		}

		public void AdviseFileAccessPattern(FileAccessPattern pattern)
		{
			this.AdviseFileAccessPattern(pattern, (long)0, (long)0);
		}

		private void AssertNotDisposed()
		{
			if (this.fileDescriptor == -1)
			{
				throw new ObjectDisposedException("Invalid File Descriptor");
			}
		}

		private void AssertValidBuffer(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "< 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			if (offset > (int)buffer.Length)
			{
				throw new ArgumentException("destination offset is beyond array size");
			}
			if (offset > (int)buffer.Length - count)
			{
				throw new ArgumentException("would overrun buffer");
			}
		}

		public override void Close()
		{
			int num;
			if (this.fileDescriptor == -1)
			{
				return;
			}
			this.Flush();
			if (!this.owner)
			{
				return;
			}
			do
			{
				num = Syscall.close(this.fileDescriptor);
			}
			while (UnixMarshal.ShouldRetrySyscall(num));
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
			this.fileDescriptor = -1;
			GC.SuppressFinalize(this);
		}

		protected override void Finalize()
		{
			try
			{
				this.Close();
			}
			finally
			{
				base.Finalize();
			}
		}

		public override void Flush()
		{
		}

		[CLSCompliant(false)]
		public long GetConfigurationValue(PathconfName name)
		{
			this.AssertNotDisposed();
			long num = Syscall.fpathconf(this.fileDescriptor, name);
			if (num == (long)-1 && (int)Stdlib.GetLastError() != 0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return num;
		}

		public override int Read([In][Out] byte[] buffer, int offset, int count)
		{
			unsafe
			{
				this.AssertNotDisposed();
				this.AssertValidBuffer(buffer, offset, count);
				if (!this.CanRead)
				{
					throw new NotSupportedException("Stream does not support reading");
				}
				if ((int)buffer.Length == 0)
				{
					return 0;
				}
				long num = (long)0;
				fixed (byte* numPointer = &buffer[offset])
				{
					do
					{
						num = Syscall.read(this.fileDescriptor, numPointer, (ulong)count);
					}
					while (UnixMarshal.ShouldRetrySyscall((int)num));
				}
				if (num == (long)-1)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
				return (int)num;
			}
		}

		public int ReadAtOffset([In][Out] byte[] buffer, int offset, int count, long fileOffset)
		{
			unsafe
			{
				this.AssertNotDisposed();
				this.AssertValidBuffer(buffer, offset, count);
				if (!this.CanRead)
				{
					throw new NotSupportedException("Stream does not support reading");
				}
				if ((int)buffer.Length == 0)
				{
					return 0;
				}
				long num = (long)0;
				fixed (byte* numPointer = &buffer[offset])
				{
					do
					{
						num = Syscall.pread(this.fileDescriptor, numPointer, (ulong)count, fileOffset);
					}
					while (UnixMarshal.ShouldRetrySyscall((int)num));
				}
				if (num == (long)-1)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
				return (int)num;
			}
		}

		private void RefreshStat()
		{
			this.AssertNotDisposed();
			int num = Syscall.fstat(this.fileDescriptor, out this.stat);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.AssertNotDisposed();
			if (!this.CanSeek)
			{
				throw new NotSupportedException("The File Descriptor does not support seeking");
			}
			SeekFlags seekFlag = SeekFlags.SEEK_CUR;
			switch (origin)
			{
				case SeekOrigin.Begin:
				{
					seekFlag = SeekFlags.SEEK_SET;
					break;
				}
				case SeekOrigin.Current:
				{
					seekFlag = SeekFlags.SEEK_CUR;
					break;
				}
				case SeekOrigin.End:
				{
					seekFlag = SeekFlags.SEEK_END;
					break;
				}
			}
			long num = Syscall.lseek(this.fileDescriptor, offset, seekFlag);
			if (num == (long)-1)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return num;
		}

		public void SendTo(UnixStream output)
		{
			this.SendTo(output, (ulong)output.Length);
		}

		[CLSCompliant(false)]
		public void SendTo(UnixStream output, ulong count)
		{
			this.SendTo(output.Handle, count);
		}

		[CLSCompliant(false)]
		public void SendTo(int out_fd, ulong count)
		{
			if (!this.CanWrite)
			{
				throw new NotSupportedException("Unable to write to the current file descriptor");
			}
			long position = this.Position;
			if (Syscall.sendfile(out_fd, this.fileDescriptor, ref position, count) == (long)-1)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
		}

		public override void SetLength(long value)
		{
			int num;
			this.AssertNotDisposed();
			if (value < (long)0)
			{
				throw new ArgumentOutOfRangeException("value", "< 0");
			}
			if (!this.CanSeek && !this.CanWrite)
			{
				throw new NotSupportedException("You can't truncating the current file descriptor");
			}
			do
			{
				num = Syscall.ftruncate(this.fileDescriptor, value);
			}
			while (UnixMarshal.ShouldRetrySyscall(num));
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		public void SetOwner(long user, long group)
		{
			this.AssertNotDisposed();
			int num = Syscall.fchown(this.fileDescriptor, Convert.ToUInt32(user), Convert.ToUInt32(group));
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		public void SetOwner(string user, string group)
		{
			this.AssertNotDisposed();
			long userId = (new UnixUserInfo(user)).UserId;
			this.SetOwner(userId, (new UnixGroupInfo(group)).GroupId);
		}

		public void SetOwner(string user)
		{
			this.AssertNotDisposed();
			Passwd passwd = Syscall.getpwnam(user);
			if (passwd == null)
			{
				throw new ArgumentException(Locale.GetText("invalid username"), "user");
			}
			this.SetOwner((long)passwd.pw_uid, (long)passwd.pw_gid);
		}

		void System.IDisposable.Dispose()
		{
			this.AssertNotDisposed();
			if (this.owner)
			{
				this.Close();
			}
			GC.SuppressFinalize(this);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			unsafe
			{
				this.AssertNotDisposed();
				this.AssertValidBuffer(buffer, offset, count);
				if (!this.CanWrite)
				{
					throw new NotSupportedException("File Descriptor does not support writing");
				}
				if ((int)buffer.Length == 0)
				{
					return;
				}
				long num = (long)0;
				fixed (byte* numPointer = &buffer[offset])
				{
					do
					{
						num = Syscall.write(this.fileDescriptor, numPointer, (ulong)count);
					}
					while (UnixMarshal.ShouldRetrySyscall((int)num));
				}
				if (num == (long)-1)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
			}
		}

		public void WriteAtOffset(byte[] buffer, int offset, int count, long fileOffset)
		{
			unsafe
			{
				this.AssertNotDisposed();
				this.AssertValidBuffer(buffer, offset, count);
				if (!this.CanWrite)
				{
					throw new NotSupportedException("File Descriptor does not support writing");
				}
				if ((int)buffer.Length == 0)
				{
					return;
				}
				long num = (long)0;
				fixed (byte* numPointer = &buffer[offset])
				{
					do
					{
						num = Syscall.pwrite(this.fileDescriptor, numPointer, (ulong)count, fileOffset);
					}
					while (UnixMarshal.ShouldRetrySyscall((int)num));
				}
				if (num == (long)-1)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
			}
		}
	}
}