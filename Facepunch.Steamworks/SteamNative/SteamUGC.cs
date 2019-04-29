using Facepunch.Steamworks;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SteamNative
{
	internal class SteamUGC : IDisposable
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

		internal SteamUGC(BaseSteamworks steamworks, IntPtr pointer)
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

		public CallbackHandle AddAppDependency(PublishedFileId_t nPublishedFileID, AppId_t nAppID, Action<AddAppDependencyResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUGC_AddAppDependency(nPublishedFileID.Value, nAppID.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return AddAppDependencyResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle AddDependency(PublishedFileId_t nParentPublishedFileID, PublishedFileId_t nChildPublishedFileID, Action<AddUGCDependencyResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUGC_AddDependency(nParentPublishedFileID.Value, nChildPublishedFileID.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return AddUGCDependencyResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool AddExcludedTag(UGCQueryHandle_t handle, string pTagName)
		{
			return this.platform.ISteamUGC_AddExcludedTag(handle.Value, pTagName);
		}

		public bool AddItemKeyValueTag(UGCUpdateHandle_t handle, string pchKey, string pchValue)
		{
			return this.platform.ISteamUGC_AddItemKeyValueTag(handle.Value, pchKey, pchValue);
		}

		public bool AddItemPreviewFile(UGCUpdateHandle_t handle, string pszPreviewFile, ItemPreviewType type)
		{
			return this.platform.ISteamUGC_AddItemPreviewFile(handle.Value, pszPreviewFile, type);
		}

		public bool AddItemPreviewVideo(UGCUpdateHandle_t handle, string pszVideoID)
		{
			return this.platform.ISteamUGC_AddItemPreviewVideo(handle.Value, pszVideoID);
		}

		public CallbackHandle AddItemToFavorites(AppId_t nAppId, PublishedFileId_t nPublishedFileID, Action<UserFavoriteItemsListChanged_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t favorites = (long)0;
			favorites = this.platform.ISteamUGC_AddItemToFavorites(nAppId.Value, nPublishedFileID.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (favorites == 0)
			{
				return null;
			}
			return UserFavoriteItemsListChanged_t.CallResult(this.steamworks, favorites, CallbackFunction);
		}

		public bool AddRequiredKeyValueTag(UGCQueryHandle_t handle, string pKey, string pValue)
		{
			return this.platform.ISteamUGC_AddRequiredKeyValueTag(handle.Value, pKey, pValue);
		}

		public bool AddRequiredTag(UGCQueryHandle_t handle, string pTagName)
		{
			return this.platform.ISteamUGC_AddRequiredTag(handle.Value, pTagName);
		}

		public bool BInitWorkshopForGameServer(DepotId_t unWorkshopDepotID, string pszFolder)
		{
			return this.platform.ISteamUGC_BInitWorkshopForGameServer(unWorkshopDepotID.Value, pszFolder);
		}

		public CallbackHandle CreateItem(AppId_t nConsumerAppId, WorkshopFileType eFileType, Action<CreateItemResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUGC_CreateItem(nConsumerAppId.Value, eFileType);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return CreateItemResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public UGCQueryHandle_t CreateQueryAllUGCRequest(UGCQuery eQueryType, UGCMatchingUGCType eMatchingeMatchingUGCTypeFileType, AppId_t nCreatorAppID, AppId_t nConsumerAppID, uint unPage)
		{
			return this.platform.ISteamUGC_CreateQueryAllUGCRequest(eQueryType, eMatchingeMatchingUGCTypeFileType, nCreatorAppID.Value, nConsumerAppID.Value, unPage);
		}

		public unsafe UGCQueryHandle_t CreateQueryUGCDetailsRequest(PublishedFileId_t[] pvecPublishedFileID)
		{
			// 
			// Current member / type: SteamNative.UGCQueryHandle_t SteamNative.SteamUGC::CreateQueryUGCDetailsRequest(SteamNative.PublishedFileId_t[])
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: SteamNative.UGCQueryHandle_t CreateQueryUGCDetailsRequest(SteamNative.PublishedFileId_t[])
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

		public UGCQueryHandle_t CreateQueryUserUGCRequest(AccountID_t unAccountID, UserUGCList eListType, UGCMatchingUGCType eMatchingUGCType, UserUGCListSortOrder eSortOrder, AppId_t nCreatorAppID, AppId_t nConsumerAppID, uint unPage)
		{
			return this.platform.ISteamUGC_CreateQueryUserUGCRequest(unAccountID.Value, eListType, eMatchingUGCType, eSortOrder, nCreatorAppID.Value, nConsumerAppID.Value, unPage);
		}

		public CallbackHandle DeleteItem(PublishedFileId_t nPublishedFileID, Action<DeleteItemResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUGC_DeleteItem(nPublishedFileID.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return DeleteItemResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public virtual void Dispose()
		{
			if (this.platform != null)
			{
				this.platform.Dispose();
				this.platform = null;
			}
		}

		public bool DownloadItem(PublishedFileId_t nPublishedFileID, bool bHighPriority)
		{
			return this.platform.ISteamUGC_DownloadItem(nPublishedFileID.Value, bHighPriority);
		}

		public CallbackHandle GetAppDependencies(PublishedFileId_t nPublishedFileID, Action<GetAppDependenciesResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUGC_GetAppDependencies(nPublishedFileID.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return GetAppDependenciesResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool GetItemDownloadInfo(PublishedFileId_t nPublishedFileID, out ulong punBytesDownloaded, out ulong punBytesTotal)
		{
			return this.platform.ISteamUGC_GetItemDownloadInfo(nPublishedFileID.Value, out punBytesDownloaded, out punBytesTotal);
		}

		public bool GetItemInstallInfo(PublishedFileId_t nPublishedFileID, out ulong punSizeOnDisk, out string pchFolder, out uint punTimeStamp)
		{
			bool flag = false;
			pchFolder = string.Empty;
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			uint num = 4096;
			flag = this.platform.ISteamUGC_GetItemInstallInfo(nPublishedFileID.Value, out punSizeOnDisk, stringBuilder, num, out punTimeStamp);
			if (!flag)
			{
				return flag;
			}
			pchFolder = stringBuilder.ToString();
			return flag;
		}

		public uint GetItemState(PublishedFileId_t nPublishedFileID)
		{
			return this.platform.ISteamUGC_GetItemState(nPublishedFileID.Value);
		}

		public ItemUpdateStatus GetItemUpdateProgress(UGCUpdateHandle_t handle, out ulong punBytesProcessed, out ulong punBytesTotal)
		{
			return this.platform.ISteamUGC_GetItemUpdateProgress(handle.Value, out punBytesProcessed, out punBytesTotal);
		}

		public uint GetNumSubscribedItems()
		{
			return this.platform.ISteamUGC_GetNumSubscribedItems();
		}

		public bool GetQueryUGCAdditionalPreview(UGCQueryHandle_t handle, uint index, uint previewIndex, out string pchURLOrVideoID, out string pchOriginalFileName, out ItemPreviewType pPreviewType)
		{
			bool flag = false;
			pchURLOrVideoID = string.Empty;
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			uint num = 4096;
			pchOriginalFileName = string.Empty;
			StringBuilder stringBuilder1 = Helpers.TakeStringBuilder();
			uint num1 = 4096;
			flag = this.platform.ISteamUGC_GetQueryUGCAdditionalPreview(handle.Value, index, previewIndex, stringBuilder, num, stringBuilder1, num1, out pPreviewType);
			if (!flag)
			{
				return flag;
			}
			pchOriginalFileName = stringBuilder1.ToString();
			if (!flag)
			{
				return flag;
			}
			pchURLOrVideoID = stringBuilder.ToString();
			return flag;
		}

		public unsafe bool GetQueryUGCChildren(UGCQueryHandle_t handle, uint index, PublishedFileId_t* pvecPublishedFileID, uint cMaxEntries)
		{
			return this.platform.ISteamUGC_GetQueryUGCChildren(handle.Value, index, (IntPtr)pvecPublishedFileID, cMaxEntries);
		}

		public bool GetQueryUGCKeyValueTag(UGCQueryHandle_t handle, uint index, uint keyValueTagIndex, out string pchKey, out string pchValue)
		{
			bool flag = false;
			pchKey = string.Empty;
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			uint num = 4096;
			pchValue = string.Empty;
			StringBuilder stringBuilder1 = Helpers.TakeStringBuilder();
			uint num1 = 4096;
			flag = this.platform.ISteamUGC_GetQueryUGCKeyValueTag(handle.Value, index, keyValueTagIndex, stringBuilder, num, stringBuilder1, num1);
			if (!flag)
			{
				return flag;
			}
			pchValue = stringBuilder1.ToString();
			if (!flag)
			{
				return flag;
			}
			pchKey = stringBuilder.ToString();
			return flag;
		}

		public bool GetQueryUGCMetadata(UGCQueryHandle_t handle, uint index, out string pchMetadata)
		{
			bool flag = false;
			pchMetadata = string.Empty;
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			uint num = 4096;
			flag = this.platform.ISteamUGC_GetQueryUGCMetadata(handle.Value, index, stringBuilder, num);
			if (!flag)
			{
				return flag;
			}
			pchMetadata = stringBuilder.ToString();
			return flag;
		}

		public uint GetQueryUGCNumAdditionalPreviews(UGCQueryHandle_t handle, uint index)
		{
			return this.platform.ISteamUGC_GetQueryUGCNumAdditionalPreviews(handle.Value, index);
		}

		public uint GetQueryUGCNumKeyValueTags(UGCQueryHandle_t handle, uint index)
		{
			return this.platform.ISteamUGC_GetQueryUGCNumKeyValueTags(handle.Value, index);
		}

		public bool GetQueryUGCPreviewURL(UGCQueryHandle_t handle, uint index, out string pchURL)
		{
			bool flag = false;
			pchURL = string.Empty;
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			uint num = 4096;
			flag = this.platform.ISteamUGC_GetQueryUGCPreviewURL(handle.Value, index, stringBuilder, num);
			if (!flag)
			{
				return flag;
			}
			pchURL = stringBuilder.ToString();
			return flag;
		}

		public bool GetQueryUGCResult(UGCQueryHandle_t handle, uint index, ref SteamUGCDetails_t pDetails)
		{
			return this.platform.ISteamUGC_GetQueryUGCResult(handle.Value, index, ref pDetails);
		}

		public bool GetQueryUGCStatistic(UGCQueryHandle_t handle, uint index, ItemStatistic eStatType, out ulong pStatValue)
		{
			return this.platform.ISteamUGC_GetQueryUGCStatistic(handle.Value, index, eStatType, out pStatValue);
		}

		public unsafe uint GetSubscribedItems(PublishedFileId_t* pvecPublishedFileID, uint cMaxEntries)
		{
			return this.platform.ISteamUGC_GetSubscribedItems((IntPtr)pvecPublishedFileID, cMaxEntries);
		}

		public CallbackHandle GetUserItemVote(PublishedFileId_t nPublishedFileID, Action<GetUserItemVoteResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUGC_GetUserItemVote(nPublishedFileID.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return GetUserItemVoteResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool ReleaseQueryUGCRequest(UGCQueryHandle_t handle)
		{
			return this.platform.ISteamUGC_ReleaseQueryUGCRequest(handle.Value);
		}

		public CallbackHandle RemoveAppDependency(PublishedFileId_t nPublishedFileID, AppId_t nAppID, Action<RemoveAppDependencyResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUGC_RemoveAppDependency(nPublishedFileID.Value, nAppID.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoveAppDependencyResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle RemoveDependency(PublishedFileId_t nParentPublishedFileID, PublishedFileId_t nChildPublishedFileID, Action<RemoveUGCDependencyResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUGC_RemoveDependency(nParentPublishedFileID.Value, nChildPublishedFileID.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return RemoveUGCDependencyResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle RemoveItemFromFavorites(AppId_t nAppId, PublishedFileId_t nPublishedFileID, Action<UserFavoriteItemsListChanged_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUGC_RemoveItemFromFavorites(nAppId.Value, nPublishedFileID.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return UserFavoriteItemsListChanged_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool RemoveItemKeyValueTags(UGCUpdateHandle_t handle, string pchKey)
		{
			return this.platform.ISteamUGC_RemoveItemKeyValueTags(handle.Value, pchKey);
		}

		public bool RemoveItemPreview(UGCUpdateHandle_t handle, uint index)
		{
			return this.platform.ISteamUGC_RemoveItemPreview(handle.Value, index);
		}

		public SteamAPICall_t RequestUGCDetails(PublishedFileId_t nPublishedFileID, uint unMaxAgeSeconds)
		{
			return this.platform.ISteamUGC_RequestUGCDetails(nPublishedFileID.Value, unMaxAgeSeconds);
		}

		public CallbackHandle SendQueryUGCRequest(UGCQueryHandle_t handle, Action<SteamUGCQueryCompleted_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUGC_SendQueryUGCRequest(handle.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return SteamUGCQueryCompleted_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool SetAllowCachedResponse(UGCQueryHandle_t handle, uint unMaxAgeSeconds)
		{
			return this.platform.ISteamUGC_SetAllowCachedResponse(handle.Value, unMaxAgeSeconds);
		}

		public bool SetCloudFileNameFilter(UGCQueryHandle_t handle, string pMatchCloudFileName)
		{
			return this.platform.ISteamUGC_SetCloudFileNameFilter(handle.Value, pMatchCloudFileName);
		}

		public bool SetItemContent(UGCUpdateHandle_t handle, string pszContentFolder)
		{
			return this.platform.ISteamUGC_SetItemContent(handle.Value, pszContentFolder);
		}

		public bool SetItemDescription(UGCUpdateHandle_t handle, string pchDescription)
		{
			return this.platform.ISteamUGC_SetItemDescription(handle.Value, pchDescription);
		}

		public bool SetItemMetadata(UGCUpdateHandle_t handle, string pchMetaData)
		{
			return this.platform.ISteamUGC_SetItemMetadata(handle.Value, pchMetaData);
		}

		public bool SetItemPreview(UGCUpdateHandle_t handle, string pszPreviewFile)
		{
			return this.platform.ISteamUGC_SetItemPreview(handle.Value, pszPreviewFile);
		}

		public bool SetItemTags(UGCUpdateHandle_t updateHandle, string[] pTags)
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
				flag = this.platform.ISteamUGC_SetItemTags(updateHandle.Value, ref steamParamStringArrayT);
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

		public bool SetItemTitle(UGCUpdateHandle_t handle, string pchTitle)
		{
			return this.platform.ISteamUGC_SetItemTitle(handle.Value, pchTitle);
		}

		public bool SetItemUpdateLanguage(UGCUpdateHandle_t handle, string pchLanguage)
		{
			return this.platform.ISteamUGC_SetItemUpdateLanguage(handle.Value, pchLanguage);
		}

		public bool SetItemVisibility(UGCUpdateHandle_t handle, RemoteStoragePublishedFileVisibility eVisibility)
		{
			return this.platform.ISteamUGC_SetItemVisibility(handle.Value, eVisibility);
		}

		public bool SetLanguage(UGCQueryHandle_t handle, string pchLanguage)
		{
			return this.platform.ISteamUGC_SetLanguage(handle.Value, pchLanguage);
		}

		public bool SetMatchAnyTag(UGCQueryHandle_t handle, bool bMatchAnyTag)
		{
			return this.platform.ISteamUGC_SetMatchAnyTag(handle.Value, bMatchAnyTag);
		}

		public bool SetRankedByTrendDays(UGCQueryHandle_t handle, uint unDays)
		{
			return this.platform.ISteamUGC_SetRankedByTrendDays(handle.Value, unDays);
		}

		public bool SetReturnAdditionalPreviews(UGCQueryHandle_t handle, bool bReturnAdditionalPreviews)
		{
			return this.platform.ISteamUGC_SetReturnAdditionalPreviews(handle.Value, bReturnAdditionalPreviews);
		}

		public bool SetReturnChildren(UGCQueryHandle_t handle, bool bReturnChildren)
		{
			return this.platform.ISteamUGC_SetReturnChildren(handle.Value, bReturnChildren);
		}

		public bool SetReturnKeyValueTags(UGCQueryHandle_t handle, bool bReturnKeyValueTags)
		{
			return this.platform.ISteamUGC_SetReturnKeyValueTags(handle.Value, bReturnKeyValueTags);
		}

		public bool SetReturnLongDescription(UGCQueryHandle_t handle, bool bReturnLongDescription)
		{
			return this.platform.ISteamUGC_SetReturnLongDescription(handle.Value, bReturnLongDescription);
		}

		public bool SetReturnMetadata(UGCQueryHandle_t handle, bool bReturnMetadata)
		{
			return this.platform.ISteamUGC_SetReturnMetadata(handle.Value, bReturnMetadata);
		}

		public bool SetReturnOnlyIDs(UGCQueryHandle_t handle, bool bReturnOnlyIDs)
		{
			return this.platform.ISteamUGC_SetReturnOnlyIDs(handle.Value, bReturnOnlyIDs);
		}

		public bool SetReturnPlaytimeStats(UGCQueryHandle_t handle, uint unDays)
		{
			return this.platform.ISteamUGC_SetReturnPlaytimeStats(handle.Value, unDays);
		}

		public bool SetReturnTotalOnly(UGCQueryHandle_t handle, bool bReturnTotalOnly)
		{
			return this.platform.ISteamUGC_SetReturnTotalOnly(handle.Value, bReturnTotalOnly);
		}

		public bool SetSearchText(UGCQueryHandle_t handle, string pSearchText)
		{
			return this.platform.ISteamUGC_SetSearchText(handle.Value, pSearchText);
		}

		public CallbackHandle SetUserItemVote(PublishedFileId_t nPublishedFileID, bool bVoteUp, Action<SetUserItemVoteResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUGC_SetUserItemVote(nPublishedFileID.Value, bVoteUp);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return SetUserItemVoteResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public UGCUpdateHandle_t StartItemUpdate(AppId_t nConsumerAppId, PublishedFileId_t nPublishedFileID)
		{
			return this.platform.ISteamUGC_StartItemUpdate(nConsumerAppId.Value, nPublishedFileID.Value);
		}

		public unsafe CallbackHandle StartPlaytimeTracking(PublishedFileId_t[] pvecPublishedFileID, Action<StartPlaytimeTrackingResult_t, bool> CallbackFunction = null)
		{
			// 
			// Current member / type: SteamNative.CallbackHandle SteamNative.SteamUGC::StartPlaytimeTracking(SteamNative.PublishedFileId_t[],System.Action`2<SteamNative.StartPlaytimeTrackingResult_t,System.Boolean>)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: SteamNative.CallbackHandle StartPlaytimeTracking(SteamNative.PublishedFileId_t[],System.Action<SteamNative.StartPlaytimeTrackingResult_t,System.Boolean>)
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

		public unsafe CallbackHandle StopPlaytimeTracking(PublishedFileId_t[] pvecPublishedFileID, Action<StopPlaytimeTrackingResult_t, bool> CallbackFunction = null)
		{
			// 
			// Current member / type: SteamNative.CallbackHandle SteamNative.SteamUGC::StopPlaytimeTracking(SteamNative.PublishedFileId_t[],System.Action`2<SteamNative.StopPlaytimeTrackingResult_t,System.Boolean>)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: SteamNative.CallbackHandle StopPlaytimeTracking(SteamNative.PublishedFileId_t[],System.Action<SteamNative.StopPlaytimeTrackingResult_t,System.Boolean>)
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

		public CallbackHandle StopPlaytimeTrackingForAllItems(Action<StopPlaytimeTrackingResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUGC_StopPlaytimeTrackingForAllItems();
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return StopPlaytimeTrackingResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle SubmitItemUpdate(UGCUpdateHandle_t handle, string pchChangeNote, Action<SubmitItemUpdateResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUGC_SubmitItemUpdate(handle.Value, pchChangeNote);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return SubmitItemUpdateResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle SubscribeItem(PublishedFileId_t nPublishedFileID, Action<RemoteStorageSubscribePublishedFileResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUGC_SubscribeItem(nPublishedFileID.Value);
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

		public void SuspendDownloads(bool bSuspend)
		{
			this.platform.ISteamUGC_SuspendDownloads(bSuspend);
		}

		public CallbackHandle UnsubscribeItem(PublishedFileId_t nPublishedFileID, Action<RemoteStorageUnsubscribePublishedFileResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUGC_UnsubscribeItem(nPublishedFileID.Value);
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

		public bool UpdateItemPreviewFile(UGCUpdateHandle_t handle, uint index, string pszPreviewFile)
		{
			return this.platform.ISteamUGC_UpdateItemPreviewFile(handle.Value, index, pszPreviewFile);
		}

		public bool UpdateItemPreviewVideo(UGCUpdateHandle_t handle, uint index, string pszVideoID)
		{
			return this.platform.ISteamUGC_UpdateItemPreviewVideo(handle.Value, index, pszVideoID);
		}
	}
}