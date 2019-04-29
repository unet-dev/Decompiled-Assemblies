using Mono.Unix.Native;
using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace Mono.Unix
{
	public sealed class UnixDirectoryInfo : UnixFileSystemInfo
	{
		public override string Name
		{
			get
			{
				string fileName = UnixPath.GetFileName(base.FullPath);
				if (fileName != null && fileName.Length != 0)
				{
					return fileName;
				}
				return base.FullPath;
			}
		}

		public UnixDirectoryInfo Parent
		{
			get
			{
				if (base.FullPath == "/")
				{
					return this;
				}
				string directoryName = UnixPath.GetDirectoryName(base.FullPath);
				if (directoryName == string.Empty)
				{
					throw new InvalidOperationException(string.Concat("Do not know parent directory for path `", base.FullPath, "'"));
				}
				return new UnixDirectoryInfo(directoryName);
			}
		}

		public UnixDirectoryInfo Root
		{
			get
			{
				string pathRoot = UnixPath.GetPathRoot(base.FullPath);
				if (pathRoot == null)
				{
					return null;
				}
				return new UnixDirectoryInfo(pathRoot);
			}
		}

		public UnixDirectoryInfo(string path) : base(path)
		{
		}

		internal UnixDirectoryInfo(string path, Stat stat) : base(path, stat)
		{
		}

		[CLSCompliant(false)]
		public void Create(FilePermissions mode)
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.mkdir(base.FullPath, mode));
			base.Refresh();
		}

		public void Create(Mono.Unix.FileAccessPermissions mode)
		{
			this.Create((FilePermissions)mode);
		}

		public void Create()
		{
			this.Create(FilePermissions.ACCESSPERMS);
		}

		public override void Delete()
		{
			this.Delete(false);
		}

		public void Delete(bool recursive)
		{
			if (recursive)
			{
				UnixFileSystemInfo[] fileSystemEntries = this.GetFileSystemEntries();
				for (int i = 0; i < (int)fileSystemEntries.Length; i++)
				{
					UnixFileSystemInfo unixFileSystemInfo = fileSystemEntries[i];
					UnixDirectoryInfo unixDirectoryInfo = unixFileSystemInfo as UnixDirectoryInfo;
					if (unixDirectoryInfo == null)
					{
						unixFileSystemInfo.Delete();
					}
					else
					{
						unixDirectoryInfo.Delete(true);
					}
				}
			}
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.rmdir(base.FullPath));
			base.Refresh();
		}

		public static string GetCurrentDirectory()
		{
			StringBuilder stringBuilder = new StringBuilder(16);
			IntPtr zero = IntPtr.Zero;
			do
			{
				StringBuilder capacity = stringBuilder;
				capacity.Capacity = capacity.Capacity * 2;
				zero = Syscall.getcwd(stringBuilder, (ulong)stringBuilder.Capacity);
			}
			while (zero == IntPtr.Zero && Stdlib.GetLastError() == Errno.ERANGE);
			if (zero == IntPtr.Zero)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return stringBuilder.ToString();
		}

		public Dirent[] GetEntries()
		{
			Dirent[] direntArray;
			IntPtr intPtr = Syscall.opendir(base.FullPath);
			if (intPtr == IntPtr.Zero)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			bool flag = false;
			try
			{
				Dirent[] entries = UnixDirectoryInfo.GetEntries(intPtr);
				flag = true;
				direntArray = entries;
			}
			finally
			{
				int num = Syscall.closedir(intPtr);
				if (flag)
				{
					UnixMarshal.ThrowExceptionForLastErrorIf(num);
				}
			}
			return direntArray;
		}

		private static Dirent[] GetEntries(IntPtr dirp)
		{
			int num;
			IntPtr intPtr;
			ArrayList arrayLists = new ArrayList();
			do
			{
				Dirent dirent = new Dirent();
				num = Syscall.readdir_r(dirp, dirent, out intPtr);
				if (num != 0 || !(intPtr != IntPtr.Zero) || !(dirent.d_name != ".") || !(dirent.d_name != ".."))
				{
					continue;
				}
				arrayLists.Add(dirent);
			}
			while (num == 0 && intPtr != IntPtr.Zero);
			if (num != 0)
			{
				UnixMarshal.ThrowExceptionForLastErrorIf(num);
			}
			return (Dirent[])arrayLists.ToArray(typeof(Dirent));
		}

		public Dirent[] GetEntries(Regex regex)
		{
			Dirent[] entries;
			IntPtr intPtr = Syscall.opendir(base.FullPath);
			if (intPtr == IntPtr.Zero)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			try
			{
				entries = UnixDirectoryInfo.GetEntries(intPtr, regex);
			}
			finally
			{
				UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.closedir(intPtr));
			}
			return entries;
		}

		private static Dirent[] GetEntries(IntPtr dirp, Regex regex)
		{
			int num;
			IntPtr intPtr;
			ArrayList arrayLists = new ArrayList();
			do
			{
				Dirent dirent = new Dirent();
				num = Syscall.readdir_r(dirp, dirent, out intPtr);
				if (num != 0 || !(intPtr != IntPtr.Zero) || !regex.Match(dirent.d_name).Success || !(dirent.d_name != ".") || !(dirent.d_name != ".."))
				{
					continue;
				}
				arrayLists.Add(dirent);
			}
			while (num == 0 && intPtr != IntPtr.Zero);
			if (num != 0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return (Dirent[])arrayLists.ToArray(typeof(Dirent));
		}

		public Dirent[] GetEntries(string regex)
		{
			return this.GetEntries(new Regex(regex));
		}

		public UnixFileSystemInfo[] GetFileSystemEntries()
		{
			return this.GetFileSystemEntries(this.GetEntries());
		}

		private UnixFileSystemInfo[] GetFileSystemEntries(Dirent[] dentries)
		{
			UnixFileSystemInfo[] fileSystemEntry = new UnixFileSystemInfo[(int)dentries.Length];
			for (int i = 0; i != (int)fileSystemEntry.Length; i++)
			{
				fileSystemEntry[i] = UnixFileSystemInfo.GetFileSystemEntry(UnixPath.Combine(base.FullPath, new string[] { dentries[i].d_name }));
			}
			return fileSystemEntry;
		}

		public UnixFileSystemInfo[] GetFileSystemEntries(Regex regex)
		{
			return this.GetFileSystemEntries(this.GetEntries(regex));
		}

		public UnixFileSystemInfo[] GetFileSystemEntries(string regex)
		{
			return this.GetFileSystemEntries(new Regex(regex));
		}

		public static void SetCurrentDirectory(string path)
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.chdir(path));
		}
	}
}