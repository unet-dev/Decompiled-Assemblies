using Steamworks.Data;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks
{
	internal class ISteamRemoteStorage : SteamInterface
	{
		private ISteamRemoteStorage.FFileWrite _FileWrite;

		private ISteamRemoteStorage.FFileRead _FileRead;

		private ISteamRemoteStorage.FFileWriteAsync _FileWriteAsync;

		private ISteamRemoteStorage.FFileReadAsync _FileReadAsync;

		private ISteamRemoteStorage.FFileReadAsyncComplete _FileReadAsyncComplete;

		private ISteamRemoteStorage.FFileForget _FileForget;

		private ISteamRemoteStorage.FFileDelete _FileDelete;

		private ISteamRemoteStorage.FFileShare _FileShare;

		private ISteamRemoteStorage.FSetSyncPlatforms _SetSyncPlatforms;

		private ISteamRemoteStorage.FFileWriteStreamOpen _FileWriteStreamOpen;

		private ISteamRemoteStorage.FFileWriteStreamWriteChunk _FileWriteStreamWriteChunk;

		private ISteamRemoteStorage.FFileWriteStreamClose _FileWriteStreamClose;

		private ISteamRemoteStorage.FFileWriteStreamCancel _FileWriteStreamCancel;

		private ISteamRemoteStorage.FFileExists _FileExists;

		private ISteamRemoteStorage.FFilePersisted _FilePersisted;

		private ISteamRemoteStorage.FGetFileSize _GetFileSize;

		private ISteamRemoteStorage.FGetFileTimestamp _GetFileTimestamp;

		private ISteamRemoteStorage.FGetSyncPlatforms _GetSyncPlatforms;

		private ISteamRemoteStorage.FGetFileCount _GetFileCount;

		private ISteamRemoteStorage.FGetFileNameAndSize _GetFileNameAndSize;

		private ISteamRemoteStorage.FGetQuota _GetQuota;

		private ISteamRemoteStorage.FIsCloudEnabledForAccount _IsCloudEnabledForAccount;

		private ISteamRemoteStorage.FIsCloudEnabledForApp _IsCloudEnabledForApp;

		private ISteamRemoteStorage.FSetCloudEnabledForApp _SetCloudEnabledForApp;

		private ISteamRemoteStorage.FUGCDownload _UGCDownload;

		private ISteamRemoteStorage.FGetUGCDownloadProgress _GetUGCDownloadProgress;

		private ISteamRemoteStorage.FGetUGCDetails _GetUGCDetails;

		private ISteamRemoteStorage.FUGCRead _UGCRead;

		private ISteamRemoteStorage.FGetCachedUGCCount _GetCachedUGCCount;

		private ISteamRemoteStorage.FUGCDownloadToLocation _UGCDownloadToLocation;

		public override string InterfaceName
		{
			get
			{
				return "STEAMREMOTESTORAGE_INTERFACE_VERSION014";
			}
		}

		public ISteamRemoteStorage()
		{
		}

		internal bool FileDelete(string pchFile)
		{
			return this._FileDelete(this.Self, pchFile);
		}

		internal bool FileExists(string pchFile)
		{
			return this._FileExists(this.Self, pchFile);
		}

		internal bool FileForget(string pchFile)
		{
			return this._FileForget(this.Self, pchFile);
		}

		internal bool FilePersisted(string pchFile)
		{
			return this._FilePersisted(this.Self, pchFile);
		}

		internal int FileRead(string pchFile, IntPtr pvData, int cubDataToRead)
		{
			return this._FileRead(this.Self, pchFile, pvData, cubDataToRead);
		}

		internal async Task<RemoteStorageFileReadAsyncComplete_t?> FileReadAsync(string pchFile, uint nOffset, uint cubToRead)
		{
			RemoteStorageFileReadAsyncComplete_t? resultAsync = await RemoteStorageFileReadAsyncComplete_t.GetResultAsync(this._FileReadAsync(this.Self, pchFile, nOffset, cubToRead));
			return resultAsync;
		}

		internal bool FileReadAsyncComplete(SteamAPICall_t hReadCall, IntPtr pvBuffer, uint cubToRead)
		{
			return this._FileReadAsyncComplete(this.Self, hReadCall, pvBuffer, cubToRead);
		}

		internal async Task<RemoteStorageFileShareResult_t?> FileShare(string pchFile)
		{
			RemoteStorageFileShareResult_t? resultAsync = await RemoteStorageFileShareResult_t.GetResultAsync(this._FileShare(this.Self, pchFile));
			return resultAsync;
		}

		internal bool FileWrite(string pchFile, IntPtr pvData, int cubData)
		{
			return this._FileWrite(this.Self, pchFile, pvData, cubData);
		}

		internal async Task<RemoteStorageFileWriteAsyncComplete_t?> FileWriteAsync(string pchFile, IntPtr pvData, uint cubData)
		{
			RemoteStorageFileWriteAsyncComplete_t? resultAsync = await RemoteStorageFileWriteAsyncComplete_t.GetResultAsync(this._FileWriteAsync(this.Self, pchFile, pvData, cubData));
			return resultAsync;
		}

		internal bool FileWriteStreamCancel(UGCFileWriteStreamHandle_t writeHandle)
		{
			return this._FileWriteStreamCancel(this.Self, writeHandle);
		}

		internal bool FileWriteStreamClose(UGCFileWriteStreamHandle_t writeHandle)
		{
			return this._FileWriteStreamClose(this.Self, writeHandle);
		}

		internal UGCFileWriteStreamHandle_t FileWriteStreamOpen(string pchFile)
		{
			return this._FileWriteStreamOpen(this.Self, pchFile);
		}

		internal bool FileWriteStreamWriteChunk(UGCFileWriteStreamHandle_t writeHandle, IntPtr pvData, int cubData)
		{
			return this._FileWriteStreamWriteChunk(this.Self, writeHandle, pvData, cubData);
		}

		internal int GetCachedUGCCount()
		{
			return this._GetCachedUGCCount(this.Self);
		}

		internal int GetFileCount()
		{
			return this._GetFileCount(this.Self);
		}

		internal string GetFileNameAndSize(int iFile, ref int pnFileSizeInBytes)
		{
			string str = base.GetString(this._GetFileNameAndSize(this.Self, iFile, ref pnFileSizeInBytes));
			return str;
		}

		internal int GetFileSize(string pchFile)
		{
			return this._GetFileSize(this.Self, pchFile);
		}

		internal long GetFileTimestamp(string pchFile)
		{
			return this._GetFileTimestamp(this.Self, pchFile);
		}

		internal bool GetQuota(ref ulong pnTotalBytes, ref ulong puAvailableBytes)
		{
			return this._GetQuota(this.Self, ref pnTotalBytes, ref puAvailableBytes);
		}

		internal RemoteStoragePlatform GetSyncPlatforms(string pchFile)
		{
			return this._GetSyncPlatforms(this.Self, pchFile);
		}

		internal bool GetUGCDetails(UGCHandle_t hContent, ref AppId pnAppID, [In][Out] ref char[] ppchName, ref int pnFileSizeInBytes, ref SteamId pSteamIDOwner)
		{
			bool self = this._GetUGCDetails(this.Self, hContent, ref pnAppID, ref ppchName, ref pnFileSizeInBytes, ref pSteamIDOwner);
			return self;
		}

		internal bool GetUGCDownloadProgress(UGCHandle_t hContent, ref int pnBytesDownloaded, ref int pnBytesExpected)
		{
			return this._GetUGCDownloadProgress(this.Self, hContent, ref pnBytesDownloaded, ref pnBytesExpected);
		}

		public override void InitInternals()
		{
			this._FileWrite = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FFileWrite>(Marshal.ReadIntPtr(this.VTable, 0));
			this._FileRead = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FFileRead>(Marshal.ReadIntPtr(this.VTable, 8));
			this._FileWriteAsync = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FFileWriteAsync>(Marshal.ReadIntPtr(this.VTable, 16));
			this._FileReadAsync = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FFileReadAsync>(Marshal.ReadIntPtr(this.VTable, 24));
			this._FileReadAsyncComplete = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FFileReadAsyncComplete>(Marshal.ReadIntPtr(this.VTable, 32));
			this._FileForget = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FFileForget>(Marshal.ReadIntPtr(this.VTable, 40));
			this._FileDelete = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FFileDelete>(Marshal.ReadIntPtr(this.VTable, 48));
			this._FileShare = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FFileShare>(Marshal.ReadIntPtr(this.VTable, 56));
			this._SetSyncPlatforms = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FSetSyncPlatforms>(Marshal.ReadIntPtr(this.VTable, 64));
			this._FileWriteStreamOpen = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FFileWriteStreamOpen>(Marshal.ReadIntPtr(this.VTable, 72));
			this._FileWriteStreamWriteChunk = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FFileWriteStreamWriteChunk>(Marshal.ReadIntPtr(this.VTable, 80));
			this._FileWriteStreamClose = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FFileWriteStreamClose>(Marshal.ReadIntPtr(this.VTable, 88));
			this._FileWriteStreamCancel = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FFileWriteStreamCancel>(Marshal.ReadIntPtr(this.VTable, 96));
			this._FileExists = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FFileExists>(Marshal.ReadIntPtr(this.VTable, 104));
			this._FilePersisted = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FFilePersisted>(Marshal.ReadIntPtr(this.VTable, 112));
			this._GetFileSize = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FGetFileSize>(Marshal.ReadIntPtr(this.VTable, 120));
			this._GetFileTimestamp = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FGetFileTimestamp>(Marshal.ReadIntPtr(this.VTable, 128));
			this._GetSyncPlatforms = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FGetSyncPlatforms>(Marshal.ReadIntPtr(this.VTable, 136));
			this._GetFileCount = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FGetFileCount>(Marshal.ReadIntPtr(this.VTable, 144));
			this._GetFileNameAndSize = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FGetFileNameAndSize>(Marshal.ReadIntPtr(this.VTable, 152));
			this._GetQuota = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FGetQuota>(Marshal.ReadIntPtr(this.VTable, 160));
			this._IsCloudEnabledForAccount = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FIsCloudEnabledForAccount>(Marshal.ReadIntPtr(this.VTable, 168));
			this._IsCloudEnabledForApp = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FIsCloudEnabledForApp>(Marshal.ReadIntPtr(this.VTable, 176));
			this._SetCloudEnabledForApp = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FSetCloudEnabledForApp>(Marshal.ReadIntPtr(this.VTable, 184));
			this._UGCDownload = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FUGCDownload>(Marshal.ReadIntPtr(this.VTable, 192));
			this._GetUGCDownloadProgress = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FGetUGCDownloadProgress>(Marshal.ReadIntPtr(this.VTable, 200));
			this._GetUGCDetails = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FGetUGCDetails>(Marshal.ReadIntPtr(this.VTable, 208));
			this._UGCRead = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FUGCRead>(Marshal.ReadIntPtr(this.VTable, 216));
			this._GetCachedUGCCount = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FGetCachedUGCCount>(Marshal.ReadIntPtr(this.VTable, 224));
			this._UGCDownloadToLocation = Marshal.GetDelegateForFunctionPointer<ISteamRemoteStorage.FUGCDownloadToLocation>(Marshal.ReadIntPtr(this.VTable, 424));
		}

		internal bool IsCloudEnabledForAccount()
		{
			return this._IsCloudEnabledForAccount(this.Self);
		}

		internal bool IsCloudEnabledForApp()
		{
			return this._IsCloudEnabledForApp(this.Self);
		}

		internal void SetCloudEnabledForApp(bool bEnabled)
		{
			this._SetCloudEnabledForApp(this.Self, bEnabled);
		}

		internal bool SetSyncPlatforms(string pchFile, RemoteStoragePlatform eRemoteStoragePlatform)
		{
			return this._SetSyncPlatforms(this.Self, pchFile, eRemoteStoragePlatform);
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._FileWrite = null;
			this._FileRead = null;
			this._FileWriteAsync = null;
			this._FileReadAsync = null;
			this._FileReadAsyncComplete = null;
			this._FileForget = null;
			this._FileDelete = null;
			this._FileShare = null;
			this._SetSyncPlatforms = null;
			this._FileWriteStreamOpen = null;
			this._FileWriteStreamWriteChunk = null;
			this._FileWriteStreamClose = null;
			this._FileWriteStreamCancel = null;
			this._FileExists = null;
			this._FilePersisted = null;
			this._GetFileSize = null;
			this._GetFileTimestamp = null;
			this._GetSyncPlatforms = null;
			this._GetFileCount = null;
			this._GetFileNameAndSize = null;
			this._GetQuota = null;
			this._IsCloudEnabledForAccount = null;
			this._IsCloudEnabledForApp = null;
			this._SetCloudEnabledForApp = null;
			this._UGCDownload = null;
			this._GetUGCDownloadProgress = null;
			this._GetUGCDetails = null;
			this._UGCRead = null;
			this._GetCachedUGCCount = null;
			this._UGCDownloadToLocation = null;
		}

		internal async Task<RemoteStorageDownloadUGCResult_t?> UGCDownload(UGCHandle_t hContent, uint unPriority)
		{
			RemoteStorageDownloadUGCResult_t? resultAsync = await RemoteStorageDownloadUGCResult_t.GetResultAsync(this._UGCDownload(this.Self, hContent, unPriority));
			return resultAsync;
		}

		internal async Task<RemoteStorageDownloadUGCResult_t?> UGCDownloadToLocation(UGCHandle_t hContent, string pchLocation, uint unPriority)
		{
			RemoteStorageDownloadUGCResult_t? resultAsync = await RemoteStorageDownloadUGCResult_t.GetResultAsync(this._UGCDownloadToLocation(this.Self, hContent, pchLocation, unPriority));
			return resultAsync;
		}

		internal int UGCRead(UGCHandle_t hContent, IntPtr pvData, int cubDataToRead, uint cOffset, UGCReadAction eAction)
		{
			int self = this._UGCRead(this.Self, hContent, pvData, cubDataToRead, cOffset, eAction);
			return self;
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FFileDelete(IntPtr self, string pchFile);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FFileExists(IntPtr self, string pchFile);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FFileForget(IntPtr self, string pchFile);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FFilePersisted(IntPtr self, string pchFile);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FFileRead(IntPtr self, string pchFile, IntPtr pvData, int cubDataToRead);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FFileReadAsync(IntPtr self, string pchFile, uint nOffset, uint cubToRead);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FFileReadAsyncComplete(IntPtr self, SteamAPICall_t hReadCall, IntPtr pvBuffer, uint cubToRead);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FFileShare(IntPtr self, string pchFile);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FFileWrite(IntPtr self, string pchFile, IntPtr pvData, int cubData);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FFileWriteAsync(IntPtr self, string pchFile, IntPtr pvData, uint cubData);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FFileWriteStreamCancel(IntPtr self, UGCFileWriteStreamHandle_t writeHandle);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FFileWriteStreamClose(IntPtr self, UGCFileWriteStreamHandle_t writeHandle);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate UGCFileWriteStreamHandle_t FFileWriteStreamOpen(IntPtr self, string pchFile);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FFileWriteStreamWriteChunk(IntPtr self, UGCFileWriteStreamHandle_t writeHandle, IntPtr pvData, int cubData);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetCachedUGCCount(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetFileCount(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetFileNameAndSize(IntPtr self, int iFile, ref int pnFileSizeInBytes);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetFileSize(IntPtr self, string pchFile);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate long FGetFileTimestamp(IntPtr self, string pchFile);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetQuota(IntPtr self, ref ulong pnTotalBytes, ref ulong puAvailableBytes);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate RemoteStoragePlatform FGetSyncPlatforms(IntPtr self, string pchFile);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetUGCDetails(IntPtr self, UGCHandle_t hContent, ref AppId pnAppID, [In][Out] ref char[] ppchName, ref int pnFileSizeInBytes, ref SteamId pSteamIDOwner);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetUGCDownloadProgress(IntPtr self, UGCHandle_t hContent, ref int pnBytesDownloaded, ref int pnBytesExpected);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIsCloudEnabledForAccount(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIsCloudEnabledForApp(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetCloudEnabledForApp(IntPtr self, bool bEnabled);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetSyncPlatforms(IntPtr self, string pchFile, RemoteStoragePlatform eRemoteStoragePlatform);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FUGCDownload(IntPtr self, UGCHandle_t hContent, uint unPriority);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FUGCDownloadToLocation(IntPtr self, UGCHandle_t hContent, string pchLocation, uint unPriority);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FUGCRead(IntPtr self, UGCHandle_t hContent, IntPtr pvData, int cubDataToRead, uint cOffset, UGCReadAction eAction);
	}
}