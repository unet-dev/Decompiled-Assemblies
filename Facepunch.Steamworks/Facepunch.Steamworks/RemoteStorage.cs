using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Facepunch.Steamworks
{
	public class RemoteStorage : IDisposable
	{
		internal Client client;

		internal SteamRemoteStorage native;

		private bool _filesInvalid = true;

		private readonly List<RemoteFile> _files = new List<RemoteFile>();

		public int FileCount
		{
			get
			{
				return this.native.GetFileCount();
			}
		}

		public IEnumerable<RemoteFile> Files
		{
			get
			{
				this.UpdateFiles();
				return this._files;
			}
		}

		public bool IsCloudEnabledForAccount
		{
			get
			{
				return this.native.IsCloudEnabledForAccount();
			}
		}

		public bool IsCloudEnabledForApp
		{
			get
			{
				return this.native.IsCloudEnabledForApp();
			}
		}

		public ulong QuotaRemaining
		{
			get
			{
				ulong num = (ulong)0;
				ulong num1 = (ulong)0;
				if (this.native.GetQuota(out num, out num1))
				{
					return num1;
				}
				return (ulong)0;
			}
		}

		public ulong QuotaTotal
		{
			get
			{
				ulong num = (ulong)0;
				ulong num1 = (ulong)0;
				if (this.native.GetQuota(out num, out num1))
				{
					return num;
				}
				return (ulong)0;
			}
		}

		public ulong QuotaUsed
		{
			get
			{
				ulong num = (ulong)0;
				ulong num1 = (ulong)0;
				if (!this.native.GetQuota(out num, out num1))
				{
					return (ulong)0;
				}
				return num - num1;
			}
		}

		internal RemoteStorage(Client c)
		{
			this.client = c;
			this.native = this.client.native.remoteStorage;
		}

		public RemoteFile CreateFile(string path)
		{
			string str = path;
			str = RemoteStorage.NormalizePath(str);
			this.InvalidateFiles();
			return this.Files.FirstOrDefault<RemoteFile>((RemoteFile x) => x.FileName == str) ?? new RemoteFile(this, str, this.client.SteamId, 0, (long)0);
		}

		public void Dispose()
		{
			this.client = null;
			this.native = null;
		}

		public bool FileExists(string path)
		{
			return this.native.FileExists(path);
		}

		internal void InvalidateFiles()
		{
			this._filesInvalid = true;
		}

		private static string NormalizePath(string path)
		{
			if (!Platform.IsWindows)
			{
				return (new FileInfo(string.Concat("/x/", path))).FullName.Substring(3);
			}
			return (new FileInfo(string.Concat("x:/", path))).FullName.Substring(3);
		}

		internal void OnWrittenNewFile(RemoteFile file)
		{
			if (this._files.Any<RemoteFile>((RemoteFile x) => x.FileName == file.FileName))
			{
				return;
			}
			this._files.Add(file);
			file.Exists = true;
			this.InvalidateFiles();
		}

		public RemoteFile OpenFile(string path)
		{
			string str = path;
			str = RemoteStorage.NormalizePath(str);
			this.InvalidateFiles();
			return this.Files.FirstOrDefault<RemoteFile>((RemoteFile x) => x.FileName == str);
		}

		public RemoteFile OpenSharedFile(ulong sharingId)
		{
			return new RemoteFile(this, sharingId);
		}

		public byte[] ReadBytes(string path)
		{
			RemoteFile remoteFile = this.OpenFile(path);
			if (remoteFile == null)
			{
				return null;
			}
			return remoteFile.ReadAllBytes();
		}

		public string ReadString(string path, Encoding encoding = null)
		{
			RemoteFile remoteFile = this.OpenFile(path);
			if (remoteFile == null)
			{
				return null;
			}
			return remoteFile.ReadAllText(encoding);
		}

		private void UpdateFiles()
		{
			int num;
			if (!this._filesInvalid)
			{
				return;
			}
			this._filesInvalid = false;
			foreach (RemoteFile _file in this._files)
			{
				_file.Exists = false;
			}
			int fileCount = this.FileCount;
			for (int i = 0; i < fileCount; i++)
			{
				string str = RemoteStorage.NormalizePath(this.native.GetFileNameAndSize(i, out num));
				long fileTimestamp = this.native.GetFileTimestamp(str);
				RemoteFile remoteFile = this._files.FirstOrDefault<RemoteFile>((RemoteFile x) => x.FileName == str);
				if (remoteFile != null)
				{
					remoteFile.SizeInBytes = num;
					remoteFile.FileTimestamp = fileTimestamp;
				}
				else
				{
					remoteFile = new RemoteFile(this, str, this.client.SteamId, num, fileTimestamp);
					this._files.Add(remoteFile);
				}
				remoteFile.Exists = true;
			}
			for (int j = this._files.Count - 1; j >= 0; j--)
			{
				if (!this._files[j].Exists)
				{
					this._files.RemoveAt(j);
				}
			}
		}

		public bool WriteBytes(string path, byte[] data)
		{
			RemoteFile remoteFile = this.CreateFile(path);
			remoteFile.WriteAllBytes(data);
			return remoteFile.Exists;
		}

		public bool WriteString(string path, string text, Encoding encoding = null)
		{
			RemoteFile remoteFile = this.CreateFile(path);
			remoteFile.WriteAllText(text, encoding);
			return remoteFile.Exists;
		}
	}
}