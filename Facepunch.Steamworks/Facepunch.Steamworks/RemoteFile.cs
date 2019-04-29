using Facepunch.Steamworks.Callbacks;
using SteamNative;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Facepunch.Steamworks
{
	public class RemoteFile
	{
		internal readonly RemoteStorage remoteStorage;

		private readonly bool _isUgc;

		private string _fileName;

		private int _sizeInBytes = -1;

		private long _timestamp;

		private UGCHandle_t _handle;

		private ulong _ownerId;

		private bool _isDownloading;

		private byte[] _downloadedData;

		public bool Exists
		{
			get;
			internal set;
		}

		public string FileName
		{
			get
			{
				if (this._fileName != null)
				{
					return this._fileName;
				}
				this.GetUGCDetails();
				return this._fileName;
			}
		}

		public long FileTimestamp
		{
			get
			{
				if (this._timestamp != 0)
				{
					return this._timestamp;
				}
				if (this._isUgc)
				{
					throw new NotImplementedException();
				}
				this._timestamp = this.remoteStorage.native.GetFileTimestamp(this.FileName);
				return this._timestamp;
			}
			internal set
			{
				this._timestamp = value;
			}
		}

		public bool IsDownloaded
		{
			get
			{
				if (!this._isUgc)
				{
					return true;
				}
				return this._downloadedData != null;
			}
		}

		public bool IsDownloading
		{
			get
			{
				if (!this._isUgc || !this._isDownloading)
				{
					return false;
				}
				return this._downloadedData == null;
			}
		}

		public bool IsShared
		{
			get
			{
				return this._handle.Value != (long)0;
			}
		}

		public ulong OwnerId
		{
			get
			{
				if (this._ownerId != 0)
				{
					return this._ownerId;
				}
				this.GetUGCDetails();
				return this._ownerId;
			}
		}

		public ulong SharingId
		{
			get
			{
				return this.UGCHandle.Value;
			}
		}

		public int SizeInBytes
		{
			get
			{
				if (this._sizeInBytes != -1)
				{
					return this._sizeInBytes;
				}
				if (this._isUgc)
				{
					throw new NotImplementedException();
				}
				this._sizeInBytes = this.remoteStorage.native.GetFileSize(this.FileName);
				return this._sizeInBytes;
			}
			internal set
			{
				this._sizeInBytes = value;
			}
		}

		internal UGCHandle_t UGCHandle
		{
			get
			{
				return this._handle;
			}
		}

		internal RemoteFile(RemoteStorage r, UGCHandle_t handle)
		{
			this.Exists = true;
			this.remoteStorage = r;
			this._isUgc = true;
			this._handle = handle;
		}

		internal RemoteFile(RemoteStorage r, string name, ulong ownerId, int sizeInBytes = -1, long timestamp = 0L)
		{
			this.remoteStorage = r;
			this._isUgc = false;
			this._fileName = name;
			this._ownerId = ownerId;
			this._sizeInBytes = sizeInBytes;
			this._timestamp = timestamp;
		}

		public bool Delete()
		{
			if (!this.Exists)
			{
				return false;
			}
			if (this._isUgc)
			{
				return false;
			}
			if (!this.remoteStorage.native.FileDelete(this.FileName))
			{
				return false;
			}
			this.Exists = false;
			this.remoteStorage.InvalidateFiles();
			return true;
		}

		public bool Download(RemoteFile.DownloadCallback onSuccess = null, FailureCallback onFailure = null)
		{
			// 
			// Current member / type: System.Boolean Facepunch.Steamworks.RemoteFile::Download(Facepunch.Steamworks.RemoteFile/DownloadCallback,Facepunch.Steamworks.Callbacks.FailureCallback)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Boolean Download(Facepunch.Steamworks.RemoteFile/DownloadCallback,Facepunch.Steamworks.Callbacks.FailureCallback)
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 109
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 143
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 73
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 72
			//    at ...( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildAnonymousDelegatesStep.cs:line 317
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 127
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..Visit[,]( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 286
			//    at ..Visit( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 322
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 499
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 87
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..(BinaryExpression ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 529
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 97
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 383
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 59
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ...Match( , Int32 ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildAnonymousDelegatesStep.cs:line 112
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildAnonymousDelegatesStep.cs:line 28
			//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildAnonymousDelegatesStep.cs:line 21
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public bool Forget()
		{
			if (!this.Exists)
			{
				return false;
			}
			if (this._isUgc)
			{
				return false;
			}
			return this.remoteStorage.native.FileForget(this.FileName);
		}

		public bool GetDownloadProgress(out int bytesDownloaded, out int bytesExpected)
		{
			return this.remoteStorage.native.GetUGCDownloadProgress(this._handle, out bytesDownloaded, out bytesExpected);
		}

		private void GetUGCDetails()
		{
			CSteamID cSteamID;
			if (!this._isUgc)
			{
				throw new InvalidOperationException();
			}
			AppId_t appIdT = new AppId_t()
			{
				Value = this.remoteStorage.native.steamworks.AppId
			};
			AppId_t appIdT1 = appIdT;
			this.remoteStorage.native.GetUGCDetails(this._handle, ref appIdT1, out this._fileName, out cSteamID);
			this._ownerId = cSteamID.Value;
		}

		public Stream OpenRead()
		{
			return new MemoryStream(this.ReadAllBytes(), false);
		}

		public RemoteFileWriteStream OpenWrite()
		{
			if (this._isUgc)
			{
				throw new InvalidOperationException("Cannot write to a shared file.");
			}
			return new RemoteFileWriteStream(this.remoteStorage, this);
		}

		public unsafe byte[] ReadAllBytes()
		{
			// 
			// Current member / type: System.Byte[] Facepunch.Steamworks.RemoteFile::ReadAllBytes()
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Byte[] ReadAllBytes()
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 109
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 143
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 73
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public string ReadAllText(Encoding encoding = null)
		{
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			return encoding.GetString(this.ReadAllBytes());
		}

		public bool Share(RemoteFile.ShareCallback onSuccess = null, FailureCallback onFailure = null)
		{
			if (this._isUgc)
			{
				return false;
			}
			if (this._handle.Value != 0)
			{
				return false;
			}
			this.remoteStorage.native.FileShare(this.FileName, (RemoteStorageFileShareResult_t result, bool error) => {
				if (error || result.Result != SteamNative.Result.OK)
				{
					FailureCallback failureCallback = onFailure;
					if (failureCallback == null)
					{
						return;
					}
					failureCallback(((int)result.Result == 0 ? Facepunch.Steamworks.Callbacks.Result.IOFailure : result.Result));
					return;
				}
				this._handle.Value = result.File;
				RemoteFile.ShareCallback shareCallback = onSuccess;
				if (shareCallback == null)
				{
					return;
				}
				shareCallback();
			});
			return true;
		}

		public void WriteAllBytes(byte[] buffer)
		{
			using (RemoteFileWriteStream remoteFileWriteStream = this.OpenWrite())
			{
				remoteFileWriteStream.Write(buffer, 0, (int)buffer.Length);
			}
		}

		public void WriteAllText(string text, Encoding encoding = null)
		{
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			this.WriteAllBytes(encoding.GetBytes(text));
		}

		public delegate void DownloadCallback();

		public delegate void ShareCallback();
	}
}