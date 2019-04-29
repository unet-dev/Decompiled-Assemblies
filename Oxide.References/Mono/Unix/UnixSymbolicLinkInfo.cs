using Mono.Unix.Native;
using System;
using System.Text;

namespace Mono.Unix
{
	public sealed class UnixSymbolicLinkInfo : UnixFileSystemInfo
	{
		[Obsolete("Use GetContents()")]
		public UnixFileSystemInfo Contents
		{
			get
			{
				return this.GetContents();
			}
		}

		public string ContentsPath
		{
			get
			{
				return this.ReadLink();
			}
		}

		public bool HasContents
		{
			get
			{
				return this.TryReadLink() != null;
			}
		}

		public override string Name
		{
			get
			{
				return UnixPath.GetFileName(base.FullPath);
			}
		}

		public UnixSymbolicLinkInfo(string path) : base(path)
		{
		}

		internal UnixSymbolicLinkInfo(string path, Stat stat) : base(path, stat)
		{
		}

		public void CreateSymbolicLinkTo(string path)
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.symlink(path, this.FullName));
		}

		public void CreateSymbolicLinkTo(UnixFileSystemInfo path)
		{
			int num = Syscall.symlink(path.FullName, this.FullName);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		public override void Delete()
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.unlink(base.FullPath));
			base.Refresh();
		}

		public UnixFileSystemInfo GetContents()
		{
			this.ReadLink();
			return UnixFileSystemInfo.GetFileSystemEntry(UnixPath.Combine(UnixPath.GetDirectoryName(base.FullPath), new string[] { this.ContentsPath }));
		}

		protected override bool GetFileStatus(string path, out Stat stat)
		{
			return Syscall.lstat(path, out stat) == 0;
		}

		private string ReadLink()
		{
			string str = this.TryReadLink();
			if (str == null)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return str;
		}

		public override void SetOwner(long owner, long group)
		{
			int num = Syscall.lchown(base.FullPath, Convert.ToUInt32(owner), Convert.ToUInt32(group));
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		private string TryReadLink()
		{
			StringBuilder stringBuilder = new StringBuilder((int)base.Length + 1);
			int num = Syscall.readlink(base.FullPath, stringBuilder);
			if (num == -1)
			{
				return null;
			}
			return stringBuilder.ToString(0, num);
		}
	}
}