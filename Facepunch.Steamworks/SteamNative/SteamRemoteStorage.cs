using Facepunch.Steamworks;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SteamNative
{
	internal class SteamRemoteStorage : IDisposable
	{
		internal Platform.Interface platform;

		internal BaseSteamworks steamworks;

		public bool IsValid
		{
			get
			{
				if (this.platform == null)
				{
					return false;
				}
				return this.platform.IsValid;
			}
		}

		internal SteamRemoteStorage(BaseSteamworks steamworks, IntPtr pointer)
		{
			this.steamworks = steamworks;
			if (Platform.IsWindows64)
			{
				this.platform = new Platform.Win64(pointer);
				return;
			}
			if (Platform.IsWindows32)
			{
				this.platform = new Platform.Win32(pointer);
				return;
			}
			if (Platform.IsLinux32)
			{
				this.platform = new Platform.Linux32(pointer);
				return;
			}
			if (Platform.IsLinux64)
			{
				this.platform = new Platform.Linux64(pointer);
				return;
			}
			if (Platform.IsOsx)
			{
				this.platform = new Platform.Mac(pointer);
			}
		}

		public CallbackHandle CommitPublishedFileUpdate(PublishedFileUpdateHandle_t updateHandle, Action<RemoteStorageUpdatePublishedFileResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamRemoteStorage_CommitPublishedFileUpdate(updateHandle.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStorageUpdatePublishedFileResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public PublishedFileUpdateHandle_t CreatePublishedFileUpdateRequest(PublishedFileId_t unPublishedFileId)
		{
			return this.platform.ISteamRemoteStorage_CreatePublishedFileUpdateRequest(unPublishedFileId.Value);
		}

		public CallbackHandle DeletePublishedFile(PublishedFileId_t unPublishedFileId, Action<RemoteStorageDeletePublishedFileResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamRemoteStorage_DeletePublishedFile(unPublishedFileId.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStorageDeletePublishedFileResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public virtual void Dispose()
		{
			if (this.platform != null)
			{
				this.platform.Dispose();
				this.platform = null;
			}
		}

		public CallbackHandle EnumeratePublishedFilesByUserAction(WorkshopFileAction eAction, uint unStartIndex, Action<RemoteStorageEnumeratePublishedFilesByUserActionResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamRemoteStorage_EnumeratePublishedFilesByUserAction(eAction, unStartIndex);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStorageEnumeratePublishedFilesByUserActionResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle EnumeratePublishedWorkshopFiles(WorkshopEnumerationType eEnumerationType, uint unStartIndex, uint unCount, uint unDays, string[] pTags, ref SteamParamStringArray_t pUserTags, Action<RemoteStorageEnumerateWorkshopFilesResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			IntPtr[] hGlobalAnsi = new IntPtr[(int)pTags.Length];
			for (int i = 0; i < (int)pTags.Length; i++)
			{
				hGlobalAnsi[i] = Marshal.StringToHGlobalAnsi(pTags[i]);
			}
			try
			{
				IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)) * (int)hGlobalAnsi.Length);
				Marshal.Copy(hGlobalAnsi, 0, intPtr, (int)hGlobalAnsi.Length);
				SteamParamStringArray_t steamParamStringArrayT = new SteamParamStringArray_t()
				{
					Strings = intPtr,
					NumStrings = (int)pTags.Length
				};
				steamAPICallT = this.platform.ISteamRemoteStorage_EnumeratePublishedWorkshopFiles(eEnumerationType, unStartIndex, unCount, unDays, ref steamParamStringArrayT, ref pUserTags);
			}
			finally
			{
				IntPtr[] intPtrArray = hGlobalAnsi;
				for (int j = 0; j < (int)intPtrArray.Length; j++)
				{
					Marshal.FreeHGlobal(intPtrArray[j]);
				}
			}
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStorageEnumerateWorkshopFilesResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle EnumerateUserPublishedFiles(uint unStartIndex, Action<RemoteStorageEnumerateUserPublishedFilesResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamRemoteStorage_EnumerateUserPublishedFiles(unStartIndex);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStorageEnumerateUserPublishedFilesResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle EnumerateUserSharedWorkshopFiles(CSteamID steamId, uint unStartIndex, string[] pRequiredTags, ref SteamParamStringArray_t pExcludedTags, Action<RemoteStorageEnumerateUserPublishedFilesResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			IntPtr[] hGlobalAnsi = new IntPtr[(int)pRequiredTags.Length];
			for (int i = 0; i < (int)pRequiredTags.Length; i++)
			{
				hGlobalAnsi[i] = Marshal.StringToHGlobalAnsi(pRequiredTags[i]);
			}
			try
			{
				IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)) * (int)hGlobalAnsi.Length);
				Marshal.Copy(hGlobalAnsi, 0, intPtr, (int)hGlobalAnsi.Length);
				SteamParamStringArray_t steamParamStringArrayT = new SteamParamStringArray_t()
				{
					Strings = intPtr,
					NumStrings = (int)pRequiredTags.Length
				};
				steamAPICallT = this.platform.ISteamRemoteStorage_EnumerateUserSharedWorkshopFiles(steamId.Value, unStartIndex, ref steamParamStringArrayT, ref pExcludedTags);
			}
			finally
			{
				IntPtr[] intPtrArray = hGlobalAnsi;
				for (int j = 0; j < (int)intPtrArray.Length; j++)
				{
					Marshal.FreeHGlobal(intPtrArray[j]);
				}
			}
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStorageEnumerateUserPublishedFilesResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle EnumerateUserSubscribedFiles(uint unStartIndex, Action<RemoteStorageEnumerateUserSubscribedFilesResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamRemoteStorage_EnumerateUserSubscribedFiles(unStartIndex);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStorageEnumerateUserSubscribedFilesResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool FileDelete(string pchFile)
		{
			return this.platform.ISteamRemoteStorage_FileDelete(pchFile);
		}

		public bool FileExists(string pchFile)
		{
			return this.platform.ISteamRemoteStorage_FileExists(pchFile);
		}

		public bool FileForget(string pchFile)
		{
			return this.platform.ISteamRemoteStorage_FileForget(pchFile);
		}

		public bool FilePersisted(string pchFile)
		{
			return this.platform.ISteamRemoteStorage_FilePersisted(pchFile);
		}

		public int FileRead(string pchFile, IntPtr pvData, int cubDataToRead)
		{
			return this.platform.ISteamRemoteStorage_FileRead(pchFile, pvData, cubDataToRead);
		}

		public CallbackHandle FileReadAsync(string pchFile, uint nOffset, uint cubToRead, Action<RemoteStorageFileReadAsyncComplete_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamRemoteStorage_FileReadAsync(pchFile, nOffset, cubToRead);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStorageFileReadAsyncComplete_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool FileReadAsyncComplete(SteamAPICall_t hReadCall, IntPtr pvBuffer, uint cubToRead)
		{
			return this.platform.ISteamRemoteStorage_FileReadAsyncComplete(hReadCall.Value, pvBuffer, cubToRead);
		}

		public CallbackHandle FileShare(string pchFile, Action<RemoteStorageFileShareResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamRemoteStorage_FileShare(pchFile);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStorageFileShareResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool FileWrite(string pchFile, IntPtr pvData, int cubData)
		{
			return this.platform.ISteamRemoteStorage_FileWrite(pchFile, pvData, cubData);
		}

		public CallbackHandle FileWriteAsync(string pchFile, IntPtr pvData, uint cubData, Action<RemoteStorageFileWriteAsyncComplete_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamRemoteStorage_FileWriteAsync(pchFile, pvData, cubData);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStorageFileWriteAsyncComplete_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool FileWriteStreamCancel(UGCFileWriteStreamHandle_t writeHandle)
		{
			return this.platform.ISteamRemoteStorage_FileWriteStreamCancel(writeHandle.Value);
		}

		public bool FileWriteStreamClose(UGCFileWriteStreamHandle_t writeHandle)
		{
			return this.platform.ISteamRemoteStorage_FileWriteStreamClose(writeHandle.Value);
		}

		public UGCFileWriteStreamHandle_t FileWriteStreamOpen(string pchFile)
		{
			return this.platform.ISteamRemoteStorage_FileWriteStreamOpen(pchFile);
		}

		public bool FileWriteStreamWriteChunk(UGCFileWriteStreamHandle_t writeHandle, IntPtr pvData, int cubData)
		{
			return this.platform.ISteamRemoteStorage_FileWriteStreamWriteChunk(writeHandle.Value, pvData, cubData);
		}

		public int GetCachedUGCCount()
		{
			return this.platform.ISteamRemoteStorage_GetCachedUGCCount();
		}

		public UGCHandle_t GetCachedUGCHandle(int iCachedContent)
		{
			return this.platform.ISteamRemoteStorage_GetCachedUGCHandle(iCachedContent);
		}

		public int GetFileCount()
		{
			return this.platform.ISteamRemoteStorage_GetFileCount();
		}

		public string GetFileNameAndSize(int iFile, out int pnFileSizeInBytes)
		{
			return Marshal.PtrToStringAnsi(this.platform.ISteamRemoteStorage_GetFileNameAndSize(iFile, out pnFileSizeInBytes));
		}

		public int GetFileSize(string pchFile)
		{
			return this.platform.ISteamRemoteStorage_GetFileSize(pchFile);
		}

		public long GetFileTimestamp(string pchFile)
		{
			return this.platform.ISteamRemoteStorage_GetFileTimestamp(pchFile);
		}

		public CallbackHandle GetPublishedFileDetails(PublishedFileId_t unPublishedFileId, uint unMaxSecondsOld, Action<RemoteStorageGetPublishedFileDetailsResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamRemoteStorage_GetPublishedFileDetails(unPublishedFileId.Value, unMaxSecondsOld);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStorageGetPublishedFileDetailsResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle GetPublishedItemVoteDetails(PublishedFileId_t unPublishedFileId, Action<RemoteStorageGetPublishedItemVoteDetailsResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamRemoteStorage_GetPublishedItemVoteDetails(unPublishedFileId.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStorageGetPublishedItemVoteDetailsResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool GetQuota(out ulong pnTotalBytes, out ulong puAvailableBytes)
		{
			return this.platform.ISteamRemoteStorage_GetQuota(out pnTotalBytes, out puAvailableBytes);
		}

		public RemoteStoragePlatform GetSyncPlatforms(string pchFile)
		{
			return this.platform.ISteamRemoteStorage_GetSyncPlatforms(pchFile);
		}

		public bool GetUGCDetails(UGCHandle_t hContent, ref AppId_t pnAppID, out string ppchName, out CSteamID pSteamIDOwner)
		{
			pSteamIDOwner = new CSteamID();
			bool flag = false;
			ppchName = string.Empty;
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			int num = 4096;
			flag = this.platform.ISteamRemoteStorage_GetUGCDetails(hContent.Value, ref pnAppID.Value, stringBuilder, out num, out pSteamIDOwner.Value);
			if (!flag)
			{
				return flag;
			}
			ppchName = stringBuilder.ToString();
			return flag;
		}

		public bool GetUGCDownloadProgress(UGCHandle_t hContent, out int pnBytesDownloaded, out int pnBytesExpected)
		{
			return this.platform.ISteamRemoteStorage_GetUGCDownloadProgress(hContent.Value, out pnBytesDownloaded, out pnBytesExpected);
		}

		public CallbackHandle GetUserPublishedItemVoteDetails(PublishedFileId_t unPublishedFileId, Action<RemoteStorageGetPublishedItemVoteDetailsResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamRemoteStorage_GetUserPublishedItemVoteDetails(unPublishedFileId.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStorageGetPublishedItemVoteDetailsResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool IsCloudEnabledForAccount()
		{
			return this.platform.ISteamRemoteStorage_IsCloudEnabledForAccount();
		}

		public bool IsCloudEnabledForApp()
		{
			return this.platform.ISteamRemoteStorage_IsCloudEnabledForApp();
		}

		public CallbackHandle PublishVideo(WorkshopVideoProvider eVideoProvider, string pchVideoAccount, string pchVideoIdentifier, string pchPreviewFile, AppId_t nConsumerAppId, string pchTitle, string pchDescription, RemoteStoragePublishedFileVisibility eVisibility, string[] pTags, Action<RemoteStoragePublishFileProgress_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			IntPtr[] hGlobalAnsi = new IntPtr[(int)pTags.Length];
			for (int i = 0; i < (int)pTags.Length; i++)
			{
				hGlobalAnsi[i] = Marshal.StringToHGlobalAnsi(pTags[i]);
			}
			try
			{
				IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)) * (int)hGlobalAnsi.Length);
				Marshal.Copy(hGlobalAnsi, 0, intPtr, (int)hGlobalAnsi.Length);
				SteamParamStringArray_t steamParamStringArrayT = new SteamParamStringArray_t()
				{
					Strings = intPtr,
					NumStrings = (int)pTags.Length
				};
				steamAPICallT = this.platform.ISteamRemoteStorage_PublishVideo(eVideoProvider, pchVideoAccount, pchVideoIdentifier, pchPreviewFile, nConsumerAppId.Value, pchTitle, pchDescription, eVisibility, ref steamParamStringArrayT);
			}
			finally
			{
				IntPtr[] intPtrArray = hGlobalAnsi;
				for (int j = 0; j < (int)intPtrArray.Length; j++)
				{
					Marshal.FreeHGlobal(intPtrArray[j]);
				}
			}
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStoragePublishFileProgress_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle PublishWorkshopFile(string pchFile, string pchPreviewFile, AppId_t nConsumerAppId, string pchTitle, string pchDescription, RemoteStoragePublishedFileVisibility eVisibility, string[] pTags, WorkshopFileType eWorkshopFileType, Action<RemoteStoragePublishFileProgress_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			IntPtr[] hGlobalAnsi = new IntPtr[(int)pTags.Length];
			for (int i = 0; i < (int)pTags.Length; i++)
			{
				hGlobalAnsi[i] = Marshal.StringToHGlobalAnsi(pTags[i]);
			}
			try
			{
				IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)) * (int)hGlobalAnsi.Length);
				Marshal.Copy(hGlobalAnsi, 0, intPtr, (int)hGlobalAnsi.Length);
				SteamParamStringArray_t steamParamStringArrayT = new SteamParamStringArray_t()
				{
					Strings = intPtr,
					NumStrings = (int)pTags.Length
				};
				steamAPICallT = this.platform.ISteamRemoteStorage_PublishWorkshopFile(pchFile, pchPreviewFile, nConsumerAppId.Value, pchTitle, pchDescription, eVisibility, ref steamParamStringArrayT, eWorkshopFileType);
			}
			finally
			{
				IntPtr[] intPtrArray = hGlobalAnsi;
				for (int j = 0; j < (int)intPtrArray.Length; j++)
				{
					Marshal.FreeHGlobal(intPtrArray[j]);
				}
			}
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStoragePublishFileProgress_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public void SetCloudEnabledForApp(bool bEnabled)
		{
			this.platform.ISteamRemoteStorage_SetCloudEnabledForApp(bEnabled);
		}

		public bool SetSyncPlatforms(string pchFile, RemoteStoragePlatform eRemoteStoragePlatform)
		{
			return this.platform.ISteamRemoteStorage_SetSyncPlatforms(pchFile, eRemoteStoragePlatform);
		}

		public CallbackHandle SetUserPublishedFileAction(PublishedFileId_t unPublishedFileId, WorkshopFileAction eAction, Action<RemoteStorageSetUserPublishedFileActionResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamRemoteStorage_SetUserPublishedFileAction(unPublishedFileId.Value, eAction);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStorageSetUserPublishedFileActionResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle SubscribePublishedFile(PublishedFileId_t unPublishedFileId, Action<RemoteStorageSubscribePublishedFileResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamRemoteStorage_SubscribePublishedFile(unPublishedFileId.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStorageSubscribePublishedFileResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle UGCDownload(UGCHandle_t hContent, uint unPriority, Action<RemoteStorageDownloadUGCResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamRemoteStorage_UGCDownload(hContent.Value, unPriority);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStorageDownloadUGCResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle UGCDownloadToLocation(UGCHandle_t hContent, string pchLocation, uint unPriority, Action<RemoteStorageDownloadUGCResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t location = (long)0;
			location = this.platform.ISteamRemoteStorage_UGCDownloadToLocation(hContent.Value, pchLocation, unPriority);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (location == 0)
			{
				return null;
			}
			return RemoteStorageDownloadUGCResult_t.CallResult(this.steamworks, location, CallbackFunction);
		}

		public int UGCRead(UGCHandle_t hContent, IntPtr pvData, int cubDataToRead, uint cOffset, UGCReadAction eAction)
		{
			return this.platform.ISteamRemoteStorage_UGCRead(hContent.Value, pvData, cubDataToRead, cOffset, eAction);
		}

		public CallbackHandle UnsubscribePublishedFile(PublishedFileId_t unPublishedFileId, Action<RemoteStorageUnsubscribePublishedFileResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamRemoteStorage_UnsubscribePublishedFile(unPublishedFileId.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStorageUnsubscribePublishedFileResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool UpdatePublishedFileDescription(PublishedFileUpdateHandle_t updateHandle, string pchDescription)
		{
			return this.platform.ISteamRemoteStorage_UpdatePublishedFileDescription(updateHandle.Value, pchDescription);
		}

		public bool UpdatePublishedFileFile(PublishedFileUpdateHandle_t updateHandle, string pchFile)
		{
			return this.platform.ISteamRemoteStorage_UpdatePublishedFileFile(updateHandle.Value, pchFile);
		}

		public bool UpdatePublishedFilePreviewFile(PublishedFileUpdateHandle_t updateHandle, string pchPreviewFile)
		{
			return this.platform.ISteamRemoteStorage_UpdatePublishedFilePreviewFile(updateHandle.Value, pchPreviewFile);
		}

		public bool UpdatePublishedFileSetChangeDescription(PublishedFileUpdateHandle_t updateHandle, string pchChangeDescription)
		{
			return this.platform.ISteamRemoteStorage_UpdatePublishedFileSetChangeDescription(updateHandle.Value, pchChangeDescription);
		}

		public bool UpdatePublishedFileTags(PublishedFileUpdateHandle_t updateHandle, string[] pTags)
		{
			bool flag;
			IntPtr[] hGlobalAnsi = new IntPtr[(int)pTags.Length];
			for (int i = 0; i < (int)pTags.Length; i++)
			{
				hGlobalAnsi[i] = Marshal.StringToHGlobalAnsi(pTags[i]);
			}
			try
			{
				IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)) * (int)hGlobalAnsi.Length);
				Marshal.Copy(hGlobalAnsi, 0, intPtr, (int)hGlobalAnsi.Length);
				SteamParamStringArray_t steamParamStringArrayT = new SteamParamStringArray_t()
				{
					Strings = intPtr,
					NumStrings = (int)pTags.Length
				};
				flag = this.platform.ISteamRemoteStorage_UpdatePublishedFileTags(updateHandle.Value, ref steamParamStringArrayT);
			}
			finally
			{
				IntPtr[] intPtrArray = hGlobalAnsi;
				for (int j = 0; j < (int)intPtrArray.Length; j++)
				{
					Marshal.FreeHGlobal(intPtrArray[j]);
				}
			}
			return flag;
		}

		public bool UpdatePublishedFileTitle(PublishedFileUpdateHandle_t updateHandle, string pchTitle)
		{
			return this.platform.ISteamRemoteStorage_UpdatePublishedFileTitle(updateHandle.Value, pchTitle);
		}

		public bool UpdatePublishedFileVisibility(PublishedFileUpdateHandle_t updateHandle, RemoteStoragePublishedFileVisibility eVisibility)
		{
			return this.platform.ISteamRemoteStorage_UpdatePublishedFileVisibility(updateHandle.Value, eVisibility);
		}

		public CallbackHandle UpdateUserPublishedItemVote(PublishedFileId_t unPublishedFileId, bool bVoteUp, Action<RemoteStorageUpdateUserPublishedItemVoteResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamRemoteStorage_UpdateUserPublishedItemVote(unPublishedFileId.Value, bVoteUp);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoteStorageUpdateUserPublishedItemVoteResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}
	}
}