using Mono.Unix.Native;
using System;

namespace Mono.Unix
{
	public abstract class UnixFileSystemInfo
	{
		internal const Mono.Unix.FileSpecialAttributes AllSpecialAttributes = Mono.Unix.FileSpecialAttributes.SetUserId | Mono.Unix.FileSpecialAttributes.SetGroupId | Mono.Unix.FileSpecialAttributes.Sticky;

		internal const FileTypes AllFileTypes = FileTypes.Directory | FileTypes.CharacterDevice | FileTypes.BlockDevice | FileTypes.RegularFile | FileTypes.Fifo | FileTypes.SymbolicLink | FileTypes.Socket;

		private Stat stat;

		private string fullPath;

		private string originalPath;

		private bool valid;

		public long BlocksAllocated
		{
			get
			{
				this.AssertValid();
				return this.stat.st_blocks;
			}
		}

		public long BlockSize
		{
			get
			{
				this.AssertValid();
				return this.stat.st_blksize;
			}
		}

		public long Device
		{
			get
			{
				this.AssertValid();
				return Convert.ToInt64(this.stat.st_dev);
			}
		}

		public long DeviceType
		{
			get
			{
				this.AssertValid();
				return Convert.ToInt64(this.stat.st_rdev);
			}
		}

		public bool Exists
		{
			get
			{
				this.Refresh(true);
				return this.valid;
			}
		}

		public Mono.Unix.FileAccessPermissions FileAccessPermissions
		{
			get
			{
				this.AssertValid();
				return (Mono.Unix.FileAccessPermissions)(this.stat.st_mode & FilePermissions.ACCESSPERMS);
			}
			set
			{
				this.AssertValid();
				int stMode = (int)this.stat.st_mode;
				this.Protection = (FilePermissions)(stMode & -512 | (int)value);
			}
		}

		public Mono.Unix.FileSpecialAttributes FileSpecialAttributes
		{
			get
			{
				this.AssertValid();
				return (Mono.Unix.FileSpecialAttributes)(this.stat.st_mode & (FilePermissions.S_ISUID | FilePermissions.S_ISGID | FilePermissions.S_ISVTX));
			}
			set
			{
				this.AssertValid();
				int stMode = (int)this.stat.st_mode;
				this.Protection = (FilePermissions)(stMode & -3585 | (int)value);
			}
		}

		public FileTypes FileType
		{
			get
			{
				this.AssertValid();
				return (FileTypes)(this.stat.st_mode & FilePermissions.S_IFMT);
			}
		}

		public virtual string FullName
		{
			get
			{
				return this.FullPath;
			}
		}

		protected string FullPath
		{
			get
			{
				return this.fullPath;
			}
			set
			{
				if (this.fullPath != value)
				{
					UnixPath.CheckPath(value);
					this.valid = false;
					this.fullPath = value;
				}
			}
		}

		public long Inode
		{
			get
			{
				this.AssertValid();
				return Convert.ToInt64(this.stat.st_ino);
			}
		}

		public bool IsBlockDevice
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsFileType(this.stat.st_mode, FilePermissions.S_IFBLK);
			}
		}

		public bool IsCharacterDevice
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsFileType(this.stat.st_mode, FilePermissions.S_IFCHR);
			}
		}

		public bool IsDirectory
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsFileType(this.stat.st_mode, FilePermissions.S_IFDIR);
			}
		}

		public bool IsFifo
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsFileType(this.stat.st_mode, FilePermissions.S_IFIFO);
			}
		}

		public bool IsRegularFile
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsFileType(this.stat.st_mode, FilePermissions.S_IFREG);
			}
		}

		public bool IsSetGroup
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsSet(this.stat.st_mode, FilePermissions.S_ISGID);
			}
		}

		public bool IsSetUser
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsSet(this.stat.st_mode, FilePermissions.S_ISUID);
			}
		}

		public bool IsSocket
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsFileType(this.stat.st_mode, FilePermissions.S_IFSOCK);
			}
		}

		public bool IsSticky
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsSet(this.stat.st_mode, FilePermissions.S_ISVTX);
			}
		}

		public bool IsSymbolicLink
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsFileType(this.stat.st_mode, FilePermissions.S_IFLNK);
			}
		}

		public DateTime LastAccessTime
		{
			get
			{
				this.AssertValid();
				return NativeConvert.ToDateTime(this.stat.st_atime);
			}
		}

		public DateTime LastAccessTimeUtc
		{
			get
			{
				return this.LastAccessTime.ToUniversalTime();
			}
		}

		public DateTime LastStatusChangeTime
		{
			get
			{
				this.AssertValid();
				return NativeConvert.ToDateTime(this.stat.st_ctime);
			}
		}

		public DateTime LastStatusChangeTimeUtc
		{
			get
			{
				return this.LastStatusChangeTime.ToUniversalTime();
			}
		}

		public DateTime LastWriteTime
		{
			get
			{
				this.AssertValid();
				return NativeConvert.ToDateTime(this.stat.st_mtime);
			}
		}

		public DateTime LastWriteTimeUtc
		{
			get
			{
				return this.LastWriteTime.ToUniversalTime();
			}
		}

		public long Length
		{
			get
			{
				this.AssertValid();
				return this.stat.st_size;
			}
		}

		public long LinkCount
		{
			get
			{
				this.AssertValid();
				return Convert.ToInt64(this.stat.st_nlink);
			}
		}

		public abstract string Name
		{
			get;
		}

		protected string OriginalPath
		{
			get
			{
				return this.originalPath;
			}
			set
			{
				this.originalPath = value;
			}
		}

		public UnixGroupInfo OwnerGroup
		{
			get
			{
				this.AssertValid();
				return new UnixGroupInfo((long)this.stat.st_gid);
			}
		}

		public long OwnerGroupId
		{
			get
			{
				this.AssertValid();
				return (long)this.stat.st_gid;
			}
		}

		public UnixUserInfo OwnerUser
		{
			get
			{
				this.AssertValid();
				return new UnixUserInfo(this.stat.st_uid);
			}
		}

		public long OwnerUserId
		{
			get
			{
				this.AssertValid();
				return (long)this.stat.st_uid;
			}
		}

		[CLSCompliant(false)]
		public FilePermissions Protection
		{
			get
			{
				this.AssertValid();
				return this.stat.st_mode;
			}
			set
			{
				UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.chmod(this.FullPath, value));
			}
		}

		protected UnixFileSystemInfo(string path)
		{
			UnixPath.CheckPath(path);
			this.originalPath = path;
			this.fullPath = UnixPath.GetFullPath(path);
			this.Refresh(true);
		}

		internal UnixFileSystemInfo(string path, Stat stat)
		{
			this.originalPath = path;
			this.fullPath = UnixPath.GetFullPath(path);
			this.stat = stat;
			this.valid = true;
		}

		private void AssertValid()
		{
			this.Refresh(false);
			if (!this.valid)
			{
				throw new InvalidOperationException("Path doesn't exist!");
			}
		}

		[CLSCompliant(false)]
		public bool CanAccess(AccessModes mode)
		{
			return Syscall.access(this.FullPath, mode) == 0;
		}

		public UnixFileSystemInfo CreateLink(string path)
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.link(this.FullName, path));
			return UnixFileSystemInfo.GetFileSystemEntry(path);
		}

		public UnixSymbolicLinkInfo CreateSymbolicLink(string path)
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.symlink(this.FullName, path));
			return new UnixSymbolicLinkInfo(path);
		}

		public abstract void Delete();

		[CLSCompliant(false)]
		public long GetConfigurationValue(PathconfName name)
		{
			long num = Syscall.pathconf(this.FullPath, name);
			if (num == (long)-1 && (int)Stdlib.GetLastError() != 0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return num;
		}

		protected virtual bool GetFileStatus(string path, out Stat stat)
		{
			return Syscall.stat(path, out stat) == 0;
		}

		public static UnixFileSystemInfo GetFileSystemEntry(string path)
		{
			Stat stat;
			int num = Syscall.lstat(path, out stat);
			if (num == -1 && Stdlib.GetLastError() == Errno.ENOENT)
			{
				return new UnixFileInfo(path);
			}
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
			if (UnixFileSystemInfo.IsFileType(stat.st_mode, FilePermissions.S_IFDIR))
			{
				return new UnixDirectoryInfo(path, stat);
			}
			if (UnixFileSystemInfo.IsFileType(stat.st_mode, FilePermissions.S_IFLNK))
			{
				return new UnixSymbolicLinkInfo(path, stat);
			}
			return new UnixFileInfo(path, stat);
		}

		internal static bool IsFileType(FilePermissions mode, FilePermissions type)
		{
			return (mode & FilePermissions.S_IFMT) == type;
		}

		internal static bool IsSet(FilePermissions mode, FilePermissions type)
		{
			return (mode & type) == type;
		}

		public void Refresh()
		{
			this.Refresh(true);
		}

		internal void Refresh(bool force)
		{
			if (this.valid && !force)
			{
				return;
			}
			this.valid = this.GetFileStatus(this.FullPath, out this.stat);
		}

		public void SetLength(long length)
		{
			int num;
			do
			{
				num = Syscall.truncate(this.FullPath, length);
			}
			while (UnixMarshal.ShouldRetrySyscall(num));
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		public virtual void SetOwner(long owner, long group)
		{
			uint num = Convert.ToUInt32(owner);
			uint num1 = Convert.ToUInt32(group);
			int num2 = Syscall.chown(this.FullPath, num, num1);
			UnixMarshal.ThrowExceptionForLastErrorIf(num2);
		}

		public void SetOwner(string owner)
		{
			Passwd passwd = Syscall.getpwnam(owner);
			if (passwd == null)
			{
				throw new ArgumentException(Locale.GetText("invalid username"), "owner");
			}
			uint pwUid = passwd.pw_uid;
			uint pwGid = passwd.pw_gid;
			this.SetOwner((long)pwUid, (long)pwGid);
		}

		public void SetOwner(string owner, string group)
		{
			long userId = (long)-1;
			if (owner != null)
			{
				userId = (new UnixUserInfo(owner)).UserId;
			}
			long groupId = (long)-1;
			if (group != null)
			{
				groupId = (new UnixGroupInfo(group)).GroupId;
			}
			this.SetOwner(userId, groupId);
		}

		public void SetOwner(UnixUserInfo owner)
		{
			long num = (long)-1;
			long groupId = num;
			long userId = num;
			if (owner != null)
			{
				userId = owner.UserId;
				groupId = owner.GroupId;
			}
			this.SetOwner(userId, groupId);
		}

		public void SetOwner(UnixUserInfo owner, UnixGroupInfo group)
		{
			long num = (long)-1;
			long groupId = num;
			long userId = num;
			if (owner != null)
			{
				userId = owner.UserId;
			}
			if (group != null)
			{
				groupId = owner.GroupId;
			}
			this.SetOwner(userId, groupId);
		}

		public Stat ToStat()
		{
			this.AssertValid();
			return this.stat;
		}

		public override string ToString()
		{
			return this.FullPath;
		}
	}
}