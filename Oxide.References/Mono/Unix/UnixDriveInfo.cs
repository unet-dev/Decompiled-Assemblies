using Mono.Unix.Native;
using System;
using System.Collections;
using System.IO;
using System.Threading;

namespace Mono.Unix
{
	public sealed class UnixDriveInfo
	{
		private Statvfs stat;

		private string fstype;

		private string mount_point;

		private string block_device;

		public long AvailableFreeSpace
		{
			get
			{
				this.Refresh();
				return Convert.ToInt64(this.stat.f_bavail * this.stat.f_frsize);
			}
		}

		public string DriveFormat
		{
			get
			{
				return this.fstype;
			}
		}

		public UnixDriveType DriveType
		{
			get
			{
				return UnixDriveType.Unknown;
			}
		}

		public bool IsReady
		{
			get
			{
				Statvfs statvf;
				bool flag = this.Refresh(false);
				if (this.mount_point == "/" || !flag)
				{
					return flag;
				}
				if (Syscall.statvfs(this.RootDirectory.Parent.FullName, out statvf) != 0)
				{
					return false;
				}
				return statvf.f_fsid != this.stat.f_fsid;
			}
		}

		public long MaximumFilenameLength
		{
			get
			{
				this.Refresh();
				return Convert.ToInt64(this.stat.f_namemax);
			}
		}

		public string Name
		{
			get
			{
				return this.mount_point;
			}
		}

		public UnixDirectoryInfo RootDirectory
		{
			get
			{
				return new UnixDirectoryInfo(this.mount_point);
			}
		}

		public long TotalFreeSpace
		{
			get
			{
				this.Refresh();
				return (long)(this.stat.f_bfree * this.stat.f_frsize);
			}
		}

		public long TotalSize
		{
			get
			{
				this.Refresh();
				return (long)(this.stat.f_frsize * this.stat.f_blocks);
			}
		}

		public string VolumeLabel
		{
			get
			{
				return this.block_device;
			}
		}

		public UnixDriveInfo(string mountPoint)
		{
			if (mountPoint == null)
			{
				throw new ArgumentNullException("mountPoint");
			}
			Fstab fstab = Syscall.getfsfile(mountPoint);
			if (fstab == null)
			{
				this.mount_point = mountPoint;
				this.block_device = string.Empty;
				this.fstype = "Unknown";
			}
			else
			{
				this.FromFstab(fstab);
			}
		}

		private UnixDriveInfo(Fstab fstab)
		{
			this.FromFstab(fstab);
		}

		private void FromFstab(Fstab fstab)
		{
			this.fstype = fstab.fs_vfstype;
			this.mount_point = fstab.fs_file;
			this.block_device = fstab.fs_spec;
		}

		public static UnixDriveInfo[] GetDrives()
		{
			ArrayList arrayLists = new ArrayList();
			object fstabLock = Syscall.fstab_lock;
			Monitor.Enter(fstabLock);
			try
			{
				if (Syscall.setfsent() != 1)
				{
					throw new IOException("Error calling setfsent(3)", new UnixIOException());
				}
				try
				{
					while (true)
					{
						Fstab fstab = Syscall.getfsent();
						Fstab fstab1 = fstab;
						if (fstab == null)
						{
							break;
						}
						if (fstab1.fs_file.StartsWith("/"))
						{
							arrayLists.Add(new UnixDriveInfo(fstab1));
						}
					}
				}
				finally
				{
					Syscall.endfsent();
				}
			}
			finally
			{
				Monitor.Exit(fstabLock);
			}
			return (UnixDriveInfo[])arrayLists.ToArray(typeof(UnixDriveInfo));
		}

		public static UnixDriveInfo GetForSpecialFile(string specialFile)
		{
			if (specialFile == null)
			{
				throw new ArgumentNullException("specialFile");
			}
			Fstab fstab = Syscall.getfsspec(specialFile);
			if (fstab == null)
			{
				throw new ArgumentException(string.Concat("specialFile isn't valid: ", specialFile));
			}
			return new UnixDriveInfo(fstab);
		}

		private void Refresh()
		{
			this.Refresh(true);
		}

		private bool Refresh(bool throwException)
		{
			int num = Syscall.statvfs(this.mount_point, out this.stat);
			if (num == -1 && throwException)
			{
				Errno lastError = Stdlib.GetLastError();
				throw new InvalidOperationException(UnixMarshal.GetErrorDescription(lastError), new UnixIOException(lastError));
			}
			if (num == -1)
			{
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			return this.VolumeLabel;
		}
	}
}