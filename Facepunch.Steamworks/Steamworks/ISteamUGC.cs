using Steamworks.Data;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks
{
	internal class ISteamUGC : SteamInterface
	{
		private ISteamUGC.FCreateQueryUserUGCRequest _CreateQueryUserUGCRequest;

		private ISteamUGC.FCreateQueryAllUGCRequest1 _CreateQueryAllUGCRequest1;

		private ISteamUGC.FCreateQueryAllUGCRequest2 _CreateQueryAllUGCRequest2;

		private ISteamUGC.FCreateQueryUGCDetailsRequest _CreateQueryUGCDetailsRequest;

		private ISteamUGC.FSendQueryUGCRequest _SendQueryUGCRequest;

		private ISteamUGC.FGetQueryUGCResult _GetQueryUGCResult;

		private ISteamUGC.FGetQueryUGCResult_Windows _GetQueryUGCResult_Windows;

		private ISteamUGC.FGetQueryUGCPreviewURL _GetQueryUGCPreviewURL;

		private ISteamUGC.FGetQueryUGCMetadata _GetQueryUGCMetadata;

		private ISteamUGC.FGetQueryUGCChildren _GetQueryUGCChildren;

		private ISteamUGC.FGetQueryUGCStatistic _GetQueryUGCStatistic;

		private ISteamUGC.FGetQueryUGCNumAdditionalPreviews _GetQueryUGCNumAdditionalPreviews;

		private ISteamUGC.FGetQueryUGCAdditionalPreview _GetQueryUGCAdditionalPreview;

		private ISteamUGC.FGetQueryUGCNumKeyValueTags _GetQueryUGCNumKeyValueTags;

		private ISteamUGC.FGetQueryUGCKeyValueTag _GetQueryUGCKeyValueTag;

		private ISteamUGC.FReleaseQueryUGCRequest _ReleaseQueryUGCRequest;

		private ISteamUGC.FAddRequiredTag _AddRequiredTag;

		private ISteamUGC.FAddExcludedTag _AddExcludedTag;

		private ISteamUGC.FSetReturnOnlyIDs _SetReturnOnlyIDs;

		private ISteamUGC.FSetReturnKeyValueTags _SetReturnKeyValueTags;

		private ISteamUGC.FSetReturnLongDescription _SetReturnLongDescription;

		private ISteamUGC.FSetReturnMetadata _SetReturnMetadata;

		private ISteamUGC.FSetReturnChildren _SetReturnChildren;

		private ISteamUGC.FSetReturnAdditionalPreviews _SetReturnAdditionalPreviews;

		private ISteamUGC.FSetReturnTotalOnly _SetReturnTotalOnly;

		private ISteamUGC.FSetReturnPlaytimeStats _SetReturnPlaytimeStats;

		private ISteamUGC.FSetLanguage _SetLanguage;

		private ISteamUGC.FSetAllowCachedResponse _SetAllowCachedResponse;

		private ISteamUGC.FSetCloudFileNameFilter _SetCloudFileNameFilter;

		private ISteamUGC.FSetMatchAnyTag _SetMatchAnyTag;

		private ISteamUGC.FSetSearchText _SetSearchText;

		private ISteamUGC.FSetRankedByTrendDays _SetRankedByTrendDays;

		private ISteamUGC.FAddRequiredKeyValueTag _AddRequiredKeyValueTag;

		private ISteamUGC.FRequestUGCDetails _RequestUGCDetails;

		private ISteamUGC.FCreateItem _CreateItem;

		private ISteamUGC.FStartItemUpdate _StartItemUpdate;

		private ISteamUGC.FSetItemTitle _SetItemTitle;

		private ISteamUGC.FSetItemDescription _SetItemDescription;

		private ISteamUGC.FSetItemUpdateLanguage _SetItemUpdateLanguage;

		private ISteamUGC.FSetItemMetadata _SetItemMetadata;

		private ISteamUGC.FSetItemVisibility _SetItemVisibility;

		private ISteamUGC.FSetItemTags _SetItemTags;

		private ISteamUGC.FSetItemTags_Windows _SetItemTags_Windows;

		private ISteamUGC.FSetItemContent _SetItemContent;

		private ISteamUGC.FSetItemPreview _SetItemPreview;

		private ISteamUGC.FSetAllowLegacyUpload _SetAllowLegacyUpload;

		private ISteamUGC.FRemoveItemKeyValueTags _RemoveItemKeyValueTags;

		private ISteamUGC.FAddItemKeyValueTag _AddItemKeyValueTag;

		private ISteamUGC.FAddItemPreviewFile _AddItemPreviewFile;

		private ISteamUGC.FAddItemPreviewVideo _AddItemPreviewVideo;

		private ISteamUGC.FUpdateItemPreviewFile _UpdateItemPreviewFile;

		private ISteamUGC.FUpdateItemPreviewVideo _UpdateItemPreviewVideo;

		private ISteamUGC.FRemoveItemPreview _RemoveItemPreview;

		private ISteamUGC.FSubmitItemUpdate _SubmitItemUpdate;

		private ISteamUGC.FGetItemUpdateProgress _GetItemUpdateProgress;

		private ISteamUGC.FSetUserItemVote _SetUserItemVote;

		private ISteamUGC.FGetUserItemVote _GetUserItemVote;

		private ISteamUGC.FAddItemToFavorites _AddItemToFavorites;

		private ISteamUGC.FRemoveItemFromFavorites _RemoveItemFromFavorites;

		private ISteamUGC.FSubscribeItem _SubscribeItem;

		private ISteamUGC.FUnsubscribeItem _UnsubscribeItem;

		private ISteamUGC.FGetNumSubscribedItems _GetNumSubscribedItems;

		private ISteamUGC.FGetSubscribedItems _GetSubscribedItems;

		private ISteamUGC.FGetItemState _GetItemState;

		private ISteamUGC.FGetItemInstallInfo _GetItemInstallInfo;

		private ISteamUGC.FGetItemDownloadInfo _GetItemDownloadInfo;

		private ISteamUGC.FDownloadItem _DownloadItem;

		private ISteamUGC.FBInitWorkshopForGameServer _BInitWorkshopForGameServer;

		private ISteamUGC.FSuspendDownloads _SuspendDownloads;

		private ISteamUGC.FStartPlaytimeTracking _StartPlaytimeTracking;

		private ISteamUGC.FStopPlaytimeTracking _StopPlaytimeTracking;

		private ISteamUGC.FStopPlaytimeTrackingForAllItems _StopPlaytimeTrackingForAllItems;

		private ISteamUGC.FAddDependency _AddDependency;

		private ISteamUGC.FRemoveDependency _RemoveDependency;

		private ISteamUGC.FAddAppDependency _AddAppDependency;

		private ISteamUGC.FRemoveAppDependency _RemoveAppDependency;

		private ISteamUGC.FGetAppDependencies _GetAppDependencies;

		private ISteamUGC.FDeleteItem _DeleteItem;

		public override string InterfaceName
		{
			get
			{
				return "STEAMUGC_INTERFACE_VERSION012";
			}
		}

		public ISteamUGC()
		{
		}

		internal async Task<AddAppDependencyResult_t?> AddAppDependency(PublishedFileId nPublishedFileID, AppId nAppID)
		{
			AddAppDependencyResult_t? resultAsync = await AddAppDependencyResult_t.GetResultAsync(this._AddAppDependency(this.Self, nPublishedFileID, nAppID));
			return resultAsync;
		}

		internal async Task<AddUGCDependencyResult_t?> AddDependency(PublishedFileId nParentPublishedFileID, PublishedFileId nChildPublishedFileID)
		{
			AddUGCDependencyResult_t? resultAsync = await AddUGCDependencyResult_t.GetResultAsync(this._AddDependency(this.Self, nParentPublishedFileID, nChildPublishedFileID));
			return resultAsync;
		}

		internal bool AddExcludedTag(UGCQueryHandle_t handle, string pTagName)
		{
			return this._AddExcludedTag(this.Self, handle, pTagName);
		}

		internal bool AddItemKeyValueTag(UGCUpdateHandle_t handle, string pchKey, string pchValue)
		{
			return this._AddItemKeyValueTag(this.Self, handle, pchKey, pchValue);
		}

		internal bool AddItemPreviewFile(UGCUpdateHandle_t handle, string pszPreviewFile, ItemPreviewType type)
		{
			return this._AddItemPreviewFile(this.Self, handle, pszPreviewFile, type);
		}

		internal bool AddItemPreviewVideo(UGCUpdateHandle_t handle, string pszVideoID)
		{
			return this._AddItemPreviewVideo(this.Self, handle, pszVideoID);
		}

		internal async Task<UserFavoriteItemsListChanged_t?> AddItemToFavorites(AppId nAppId, PublishedFileId nPublishedFileID)
		{
			UserFavoriteItemsListChanged_t? resultAsync = await UserFavoriteItemsListChanged_t.GetResultAsync(this._AddItemToFavorites(this.Self, nAppId, nPublishedFileID));
			return resultAsync;
		}

		internal bool AddRequiredKeyValueTag(UGCQueryHandle_t handle, string pKey, string pValue)
		{
			return this._AddRequiredKeyValueTag(this.Self, handle, pKey, pValue);
		}

		internal bool AddRequiredTag(UGCQueryHandle_t handle, string pTagName)
		{
			return this._AddRequiredTag(this.Self, handle, pTagName);
		}

		internal bool BInitWorkshopForGameServer(DepotId_t unWorkshopDepotID, string pszFolder)
		{
			return this._BInitWorkshopForGameServer(this.Self, unWorkshopDepotID, pszFolder);
		}

		internal async Task<CreateItemResult_t?> CreateItem(AppId nConsumerAppId, WorkshopFileType eFileType)
		{
			CreateItemResult_t? resultAsync = await CreateItemResult_t.GetResultAsync(this._CreateItem(this.Self, nConsumerAppId, eFileType));
			return resultAsync;
		}

		internal UGCQueryHandle_t CreateQueryAllUGCRequest1(UGCQuery eQueryType, UgcType eMatchingeMatchingUGCTypeFileType, AppId nCreatorAppID, AppId nConsumerAppID, uint unPage)
		{
			UGCQueryHandle_t self = this._CreateQueryAllUGCRequest1(this.Self, eQueryType, eMatchingeMatchingUGCTypeFileType, nCreatorAppID, nConsumerAppID, unPage);
			return self;
		}

		internal UGCQueryHandle_t CreateQueryAllUGCRequest2(UGCQuery eQueryType, UgcType eMatchingeMatchingUGCTypeFileType, AppId nCreatorAppID, AppId nConsumerAppID, string pchCursor)
		{
			UGCQueryHandle_t self = this._CreateQueryAllUGCRequest2(this.Self, eQueryType, eMatchingeMatchingUGCTypeFileType, nCreatorAppID, nConsumerAppID, pchCursor);
			return self;
		}

		internal UGCQueryHandle_t CreateQueryUGCDetailsRequest([In][Out] PublishedFileId[] pvecPublishedFileID, uint unNumPublishedFileIDs)
		{
			return this._CreateQueryUGCDetailsRequest(this.Self, pvecPublishedFileID, unNumPublishedFileIDs);
		}

		internal UGCQueryHandle_t CreateQueryUserUGCRequest(AccountID_t unAccountID, UserUGCList eListType, UgcType eMatchingUGCType, UserUGCListSortOrder eSortOrder, AppId nCreatorAppID, AppId nConsumerAppID, uint unPage)
		{
			UGCQueryHandle_t self = this._CreateQueryUserUGCRequest(this.Self, unAccountID, eListType, eMatchingUGCType, eSortOrder, nCreatorAppID, nConsumerAppID, unPage);
			return self;
		}

		internal async Task<DeleteItemResult_t?> DeleteItem(PublishedFileId nPublishedFileID)
		{
			DeleteItemResult_t? resultAsync = await DeleteItemResult_t.GetResultAsync(this._DeleteItem(this.Self, nPublishedFileID));
			return resultAsync;
		}

		internal bool DownloadItem(PublishedFileId nPublishedFileID, bool bHighPriority)
		{
			return this._DownloadItem(this.Self, nPublishedFileID, bHighPriority);
		}

		internal async Task<GetAppDependenciesResult_t?> GetAppDependencies(PublishedFileId nPublishedFileID)
		{
			GetAppDependenciesResult_t? resultAsync = await GetAppDependenciesResult_t.GetResultAsync(this._GetAppDependencies(this.Self, nPublishedFileID));
			return resultAsync;
		}

		internal bool GetItemDownloadInfo(PublishedFileId nPublishedFileID, ref ulong punBytesDownloaded, ref ulong punBytesTotal)
		{
			return this._GetItemDownloadInfo(this.Self, nPublishedFileID, ref punBytesDownloaded, ref punBytesTotal);
		}

		internal bool GetItemInstallInfo(PublishedFileId nPublishedFileID, ref ulong punSizeOnDisk, StringBuilder pchFolder, uint cchFolderSize, ref uint punTimeStamp)
		{
			bool self = this._GetItemInstallInfo(this.Self, nPublishedFileID, ref punSizeOnDisk, pchFolder, cchFolderSize, ref punTimeStamp);
			return self;
		}

		internal uint GetItemState(PublishedFileId nPublishedFileID)
		{
			return this._GetItemState(this.Self, nPublishedFileID);
		}

		internal ItemUpdateStatus GetItemUpdateProgress(UGCUpdateHandle_t handle, ref ulong punBytesProcessed, ref ulong punBytesTotal)
		{
			return this._GetItemUpdateProgress(this.Self, handle, ref punBytesProcessed, ref punBytesTotal);
		}

		internal uint GetNumSubscribedItems()
		{
			return this._GetNumSubscribedItems(this.Self);
		}

		internal bool GetQueryUGCAdditionalPreview(UGCQueryHandle_t handle, uint index, uint previewIndex, StringBuilder pchURLOrVideoID, uint cchURLSize, StringBuilder pchOriginalFileName, uint cchOriginalFileNameSize, ref ItemPreviewType pPreviewType)
		{
			bool self = this._GetQueryUGCAdditionalPreview(this.Self, handle, index, previewIndex, pchURLOrVideoID, cchURLSize, pchOriginalFileName, cchOriginalFileNameSize, ref pPreviewType);
			return self;
		}

		internal bool GetQueryUGCChildren(UGCQueryHandle_t handle, uint index, [In][Out] PublishedFileId[] pvecPublishedFileID, uint cMaxEntries)
		{
			bool self = this._GetQueryUGCChildren(this.Self, handle, index, pvecPublishedFileID, cMaxEntries);
			return self;
		}

		internal bool GetQueryUGCKeyValueTag(UGCQueryHandle_t handle, uint index, uint keyValueTagIndex, StringBuilder pchKey, uint cchKeySize, StringBuilder pchValue, uint cchValueSize)
		{
			bool self = this._GetQueryUGCKeyValueTag(this.Self, handle, index, keyValueTagIndex, pchKey, cchKeySize, pchValue, cchValueSize);
			return self;
		}

		internal bool GetQueryUGCMetadata(UGCQueryHandle_t handle, uint index, StringBuilder pchMetadata, uint cchMetadatasize)
		{
			bool self = this._GetQueryUGCMetadata(this.Self, handle, index, pchMetadata, cchMetadatasize);
			return self;
		}

		internal uint GetQueryUGCNumAdditionalPreviews(UGCQueryHandle_t handle, uint index)
		{
			return this._GetQueryUGCNumAdditionalPreviews(this.Self, handle, index);
		}

		internal uint GetQueryUGCNumKeyValueTags(UGCQueryHandle_t handle, uint index)
		{
			return this._GetQueryUGCNumKeyValueTags(this.Self, handle, index);
		}

		internal bool GetQueryUGCPreviewURL(UGCQueryHandle_t handle, uint index, StringBuilder pchURL, uint cchURLSize)
		{
			bool self = this._GetQueryUGCPreviewURL(this.Self, handle, index, pchURL, cchURLSize);
			return self;
		}

		internal bool GetQueryUGCResult(UGCQueryHandle_t handle, uint index, ref SteamUGCDetails_t pDetails)
		{
			bool self;
			if (Config.Os != OsType.Windows)
			{
				self = this._GetQueryUGCResult(this.Self, handle, index, ref pDetails);
			}
			else
			{
				SteamUGCDetails_t.Pack8 pack8 = pDetails;
				bool _GetQueryUGCResultWindows = this._GetQueryUGCResult_Windows(this.Self, handle, index, ref pack8);
				pDetails = pack8;
				self = _GetQueryUGCResultWindows;
			}
			return self;
		}

		internal bool GetQueryUGCStatistic(UGCQueryHandle_t handle, uint index, ItemStatistic eStatType, ref ulong pStatValue)
		{
			bool self = this._GetQueryUGCStatistic(this.Self, handle, index, eStatType, ref pStatValue);
			return self;
		}

		internal uint GetSubscribedItems([In][Out] PublishedFileId[] pvecPublishedFileID, uint cMaxEntries)
		{
			return this._GetSubscribedItems(this.Self, pvecPublishedFileID, cMaxEntries);
		}

		internal async Task<GetUserItemVoteResult_t?> GetUserItemVote(PublishedFileId nPublishedFileID)
		{
			GetUserItemVoteResult_t? resultAsync = await GetUserItemVoteResult_t.GetResultAsync(this._GetUserItemVote(this.Self, nPublishedFileID));
			return resultAsync;
		}

		public override void InitInternals()
		{
			this._CreateQueryUserUGCRequest = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FCreateQueryUserUGCRequest>(Marshal.ReadIntPtr(this.VTable, 0));
			this._CreateQueryAllUGCRequest1 = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FCreateQueryAllUGCRequest1>(Marshal.ReadIntPtr(this.VTable, (Config.Os == OsType.Windows ? 16 : 8)));
			this._CreateQueryAllUGCRequest2 = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FCreateQueryAllUGCRequest2>(Marshal.ReadIntPtr(this.VTable, (Config.Os == OsType.Windows ? 8 : 16)));
			this._CreateQueryUGCDetailsRequest = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FCreateQueryUGCDetailsRequest>(Marshal.ReadIntPtr(this.VTable, 24));
			this._SendQueryUGCRequest = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSendQueryUGCRequest>(Marshal.ReadIntPtr(this.VTable, 32));
			this._GetQueryUGCResult = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FGetQueryUGCResult>(Marshal.ReadIntPtr(this.VTable, 40));
			this._GetQueryUGCResult_Windows = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FGetQueryUGCResult_Windows>(Marshal.ReadIntPtr(this.VTable, 40));
			this._GetQueryUGCPreviewURL = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FGetQueryUGCPreviewURL>(Marshal.ReadIntPtr(this.VTable, 48));
			this._GetQueryUGCMetadata = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FGetQueryUGCMetadata>(Marshal.ReadIntPtr(this.VTable, 56));
			this._GetQueryUGCChildren = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FGetQueryUGCChildren>(Marshal.ReadIntPtr(this.VTable, 64));
			this._GetQueryUGCStatistic = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FGetQueryUGCStatistic>(Marshal.ReadIntPtr(this.VTable, 72));
			this._GetQueryUGCNumAdditionalPreviews = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FGetQueryUGCNumAdditionalPreviews>(Marshal.ReadIntPtr(this.VTable, 80));
			this._GetQueryUGCAdditionalPreview = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FGetQueryUGCAdditionalPreview>(Marshal.ReadIntPtr(this.VTable, 88));
			this._GetQueryUGCNumKeyValueTags = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FGetQueryUGCNumKeyValueTags>(Marshal.ReadIntPtr(this.VTable, 96));
			this._GetQueryUGCKeyValueTag = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FGetQueryUGCKeyValueTag>(Marshal.ReadIntPtr(this.VTable, 104));
			this._ReleaseQueryUGCRequest = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FReleaseQueryUGCRequest>(Marshal.ReadIntPtr(this.VTable, 112));
			this._AddRequiredTag = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FAddRequiredTag>(Marshal.ReadIntPtr(this.VTable, 120));
			this._AddExcludedTag = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FAddExcludedTag>(Marshal.ReadIntPtr(this.VTable, 128));
			this._SetReturnOnlyIDs = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetReturnOnlyIDs>(Marshal.ReadIntPtr(this.VTable, 136));
			this._SetReturnKeyValueTags = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetReturnKeyValueTags>(Marshal.ReadIntPtr(this.VTable, 144));
			this._SetReturnLongDescription = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetReturnLongDescription>(Marshal.ReadIntPtr(this.VTable, 152));
			this._SetReturnMetadata = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetReturnMetadata>(Marshal.ReadIntPtr(this.VTable, 160));
			this._SetReturnChildren = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetReturnChildren>(Marshal.ReadIntPtr(this.VTable, 168));
			this._SetReturnAdditionalPreviews = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetReturnAdditionalPreviews>(Marshal.ReadIntPtr(this.VTable, 176));
			this._SetReturnTotalOnly = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetReturnTotalOnly>(Marshal.ReadIntPtr(this.VTable, 184));
			this._SetReturnPlaytimeStats = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetReturnPlaytimeStats>(Marshal.ReadIntPtr(this.VTable, 192));
			this._SetLanguage = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetLanguage>(Marshal.ReadIntPtr(this.VTable, 200));
			this._SetAllowCachedResponse = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetAllowCachedResponse>(Marshal.ReadIntPtr(this.VTable, 208));
			this._SetCloudFileNameFilter = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetCloudFileNameFilter>(Marshal.ReadIntPtr(this.VTable, 216));
			this._SetMatchAnyTag = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetMatchAnyTag>(Marshal.ReadIntPtr(this.VTable, 224));
			this._SetSearchText = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetSearchText>(Marshal.ReadIntPtr(this.VTable, 232));
			this._SetRankedByTrendDays = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetRankedByTrendDays>(Marshal.ReadIntPtr(this.VTable, 240));
			this._AddRequiredKeyValueTag = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FAddRequiredKeyValueTag>(Marshal.ReadIntPtr(this.VTable, 248));
			this._RequestUGCDetails = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FRequestUGCDetails>(Marshal.ReadIntPtr(this.VTable, 256));
			this._CreateItem = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FCreateItem>(Marshal.ReadIntPtr(this.VTable, 264));
			this._StartItemUpdate = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FStartItemUpdate>(Marshal.ReadIntPtr(this.VTable, 272));
			this._SetItemTitle = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetItemTitle>(Marshal.ReadIntPtr(this.VTable, 280));
			this._SetItemDescription = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetItemDescription>(Marshal.ReadIntPtr(this.VTable, 288));
			this._SetItemUpdateLanguage = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetItemUpdateLanguage>(Marshal.ReadIntPtr(this.VTable, 296));
			this._SetItemMetadata = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetItemMetadata>(Marshal.ReadIntPtr(this.VTable, 304));
			this._SetItemVisibility = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetItemVisibility>(Marshal.ReadIntPtr(this.VTable, 312));
			this._SetItemTags = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetItemTags>(Marshal.ReadIntPtr(this.VTable, 320));
			this._SetItemTags_Windows = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetItemTags_Windows>(Marshal.ReadIntPtr(this.VTable, 320));
			this._SetItemContent = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetItemContent>(Marshal.ReadIntPtr(this.VTable, 328));
			this._SetItemPreview = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetItemPreview>(Marshal.ReadIntPtr(this.VTable, 336));
			this._SetAllowLegacyUpload = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetAllowLegacyUpload>(Marshal.ReadIntPtr(this.VTable, 344));
			this._RemoveItemKeyValueTags = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FRemoveItemKeyValueTags>(Marshal.ReadIntPtr(this.VTable, 352));
			this._AddItemKeyValueTag = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FAddItemKeyValueTag>(Marshal.ReadIntPtr(this.VTable, 360));
			this._AddItemPreviewFile = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FAddItemPreviewFile>(Marshal.ReadIntPtr(this.VTable, 368));
			this._AddItemPreviewVideo = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FAddItemPreviewVideo>(Marshal.ReadIntPtr(this.VTable, 376));
			this._UpdateItemPreviewFile = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FUpdateItemPreviewFile>(Marshal.ReadIntPtr(this.VTable, 384));
			this._UpdateItemPreviewVideo = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FUpdateItemPreviewVideo>(Marshal.ReadIntPtr(this.VTable, 392));
			this._RemoveItemPreview = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FRemoveItemPreview>(Marshal.ReadIntPtr(this.VTable, 400));
			this._SubmitItemUpdate = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSubmitItemUpdate>(Marshal.ReadIntPtr(this.VTable, 408));
			this._GetItemUpdateProgress = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FGetItemUpdateProgress>(Marshal.ReadIntPtr(this.VTable, 416));
			this._SetUserItemVote = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSetUserItemVote>(Marshal.ReadIntPtr(this.VTable, 424));
			this._GetUserItemVote = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FGetUserItemVote>(Marshal.ReadIntPtr(this.VTable, 432));
			this._AddItemToFavorites = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FAddItemToFavorites>(Marshal.ReadIntPtr(this.VTable, 440));
			this._RemoveItemFromFavorites = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FRemoveItemFromFavorites>(Marshal.ReadIntPtr(this.VTable, 448));
			this._SubscribeItem = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSubscribeItem>(Marshal.ReadIntPtr(this.VTable, 456));
			this._UnsubscribeItem = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FUnsubscribeItem>(Marshal.ReadIntPtr(this.VTable, 464));
			this._GetNumSubscribedItems = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FGetNumSubscribedItems>(Marshal.ReadIntPtr(this.VTable, 472));
			this._GetSubscribedItems = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FGetSubscribedItems>(Marshal.ReadIntPtr(this.VTable, 480));
			this._GetItemState = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FGetItemState>(Marshal.ReadIntPtr(this.VTable, 488));
			this._GetItemInstallInfo = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FGetItemInstallInfo>(Marshal.ReadIntPtr(this.VTable, 496));
			this._GetItemDownloadInfo = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FGetItemDownloadInfo>(Marshal.ReadIntPtr(this.VTable, 504));
			this._DownloadItem = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FDownloadItem>(Marshal.ReadIntPtr(this.VTable, 512));
			this._BInitWorkshopForGameServer = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FBInitWorkshopForGameServer>(Marshal.ReadIntPtr(this.VTable, 520));
			this._SuspendDownloads = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FSuspendDownloads>(Marshal.ReadIntPtr(this.VTable, 528));
			this._StartPlaytimeTracking = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FStartPlaytimeTracking>(Marshal.ReadIntPtr(this.VTable, 536));
			this._StopPlaytimeTracking = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FStopPlaytimeTracking>(Marshal.ReadIntPtr(this.VTable, 544));
			this._StopPlaytimeTrackingForAllItems = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FStopPlaytimeTrackingForAllItems>(Marshal.ReadIntPtr(this.VTable, 552));
			this._AddDependency = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FAddDependency>(Marshal.ReadIntPtr(this.VTable, 560));
			this._RemoveDependency = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FRemoveDependency>(Marshal.ReadIntPtr(this.VTable, 568));
			this._AddAppDependency = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FAddAppDependency>(Marshal.ReadIntPtr(this.VTable, 576));
			this._RemoveAppDependency = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FRemoveAppDependency>(Marshal.ReadIntPtr(this.VTable, 584));
			this._GetAppDependencies = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FGetAppDependencies>(Marshal.ReadIntPtr(this.VTable, 592));
			this._DeleteItem = Marshal.GetDelegateForFunctionPointer<ISteamUGC.FDeleteItem>(Marshal.ReadIntPtr(this.VTable, 600));
		}

		internal bool ReleaseQueryUGCRequest(UGCQueryHandle_t handle)
		{
			return this._ReleaseQueryUGCRequest(this.Self, handle);
		}

		internal async Task<RemoveAppDependencyResult_t?> RemoveAppDependency(PublishedFileId nPublishedFileID, AppId nAppID)
		{
			RemoveAppDependencyResult_t? resultAsync = await RemoveAppDependencyResult_t.GetResultAsync(this._RemoveAppDependency(this.Self, nPublishedFileID, nAppID));
			return resultAsync;
		}

		internal async Task<RemoveUGCDependencyResult_t?> RemoveDependency(PublishedFileId nParentPublishedFileID, PublishedFileId nChildPublishedFileID)
		{
			RemoveUGCDependencyResult_t? resultAsync = await RemoveUGCDependencyResult_t.GetResultAsync(this._RemoveDependency(this.Self, nParentPublishedFileID, nChildPublishedFileID));
			return resultAsync;
		}

		internal async Task<UserFavoriteItemsListChanged_t?> RemoveItemFromFavorites(AppId nAppId, PublishedFileId nPublishedFileID)
		{
			UserFavoriteItemsListChanged_t? resultAsync = await UserFavoriteItemsListChanged_t.GetResultAsync(this._RemoveItemFromFavorites(this.Self, nAppId, nPublishedFileID));
			return resultAsync;
		}

		internal bool RemoveItemKeyValueTags(UGCUpdateHandle_t handle, string pchKey)
		{
			return this._RemoveItemKeyValueTags(this.Self, handle, pchKey);
		}

		internal bool RemoveItemPreview(UGCUpdateHandle_t handle, uint index)
		{
			return this._RemoveItemPreview(this.Self, handle, index);
		}

		internal async Task<SteamUGCRequestUGCDetailsResult_t?> RequestUGCDetails(PublishedFileId nPublishedFileID, uint unMaxAgeSeconds)
		{
			SteamUGCRequestUGCDetailsResult_t? resultAsync = await SteamUGCRequestUGCDetailsResult_t.GetResultAsync(this._RequestUGCDetails(this.Self, nPublishedFileID, unMaxAgeSeconds));
			return resultAsync;
		}

		internal async Task<SteamUGCQueryCompleted_t?> SendQueryUGCRequest(UGCQueryHandle_t handle)
		{
			SteamUGCQueryCompleted_t? resultAsync = await SteamUGCQueryCompleted_t.GetResultAsync(this._SendQueryUGCRequest(this.Self, handle));
			return resultAsync;
		}

		internal bool SetAllowCachedResponse(UGCQueryHandle_t handle, uint unMaxAgeSeconds)
		{
			return this._SetAllowCachedResponse(this.Self, handle, unMaxAgeSeconds);
		}

		internal bool SetAllowLegacyUpload(UGCUpdateHandle_t handle, bool bAllowLegacyUpload)
		{
			return this._SetAllowLegacyUpload(this.Self, handle, bAllowLegacyUpload);
		}

		internal bool SetCloudFileNameFilter(UGCQueryHandle_t handle, string pMatchCloudFileName)
		{
			return this._SetCloudFileNameFilter(this.Self, handle, pMatchCloudFileName);
		}

		internal bool SetItemContent(UGCUpdateHandle_t handle, string pszContentFolder)
		{
			return this._SetItemContent(this.Self, handle, pszContentFolder);
		}

		internal bool SetItemDescription(UGCUpdateHandle_t handle, string pchDescription)
		{
			return this._SetItemDescription(this.Self, handle, pchDescription);
		}

		internal bool SetItemMetadata(UGCUpdateHandle_t handle, string pchMetaData)
		{
			return this._SetItemMetadata(this.Self, handle, pchMetaData);
		}

		internal bool SetItemPreview(UGCUpdateHandle_t handle, string pszPreviewFile)
		{
			return this._SetItemPreview(this.Self, handle, pszPreviewFile);
		}

		internal bool SetItemTags(UGCUpdateHandle_t updateHandle, ref SteamParamStringArray_t pTags)
		{
			bool self;
			if (Config.Os != OsType.Windows)
			{
				self = this._SetItemTags(this.Self, updateHandle, ref pTags);
			}
			else
			{
				SteamParamStringArray_t.Pack8 pack8 = pTags;
				bool _SetItemTagsWindows = this._SetItemTags_Windows(this.Self, updateHandle, ref pack8);
				pTags = pack8;
				self = _SetItemTagsWindows;
			}
			return self;
		}

		internal bool SetItemTitle(UGCUpdateHandle_t handle, string pchTitle)
		{
			return this._SetItemTitle(this.Self, handle, pchTitle);
		}

		internal bool SetItemUpdateLanguage(UGCUpdateHandle_t handle, string pchLanguage)
		{
			return this._SetItemUpdateLanguage(this.Self, handle, pchLanguage);
		}

		internal bool SetItemVisibility(UGCUpdateHandle_t handle, RemoteStoragePublishedFileVisibility eVisibility)
		{
			return this._SetItemVisibility(this.Self, handle, eVisibility);
		}

		internal bool SetLanguage(UGCQueryHandle_t handle, string pchLanguage)
		{
			return this._SetLanguage(this.Self, handle, pchLanguage);
		}

		internal bool SetMatchAnyTag(UGCQueryHandle_t handle, bool bMatchAnyTag)
		{
			return this._SetMatchAnyTag(this.Self, handle, bMatchAnyTag);
		}

		internal bool SetRankedByTrendDays(UGCQueryHandle_t handle, uint unDays)
		{
			return this._SetRankedByTrendDays(this.Self, handle, unDays);
		}

		internal bool SetReturnAdditionalPreviews(UGCQueryHandle_t handle, bool bReturnAdditionalPreviews)
		{
			return this._SetReturnAdditionalPreviews(this.Self, handle, bReturnAdditionalPreviews);
		}

		internal bool SetReturnChildren(UGCQueryHandle_t handle, bool bReturnChildren)
		{
			return this._SetReturnChildren(this.Self, handle, bReturnChildren);
		}

		internal bool SetReturnKeyValueTags(UGCQueryHandle_t handle, bool bReturnKeyValueTags)
		{
			return this._SetReturnKeyValueTags(this.Self, handle, bReturnKeyValueTags);
		}

		internal bool SetReturnLongDescription(UGCQueryHandle_t handle, bool bReturnLongDescription)
		{
			return this._SetReturnLongDescription(this.Self, handle, bReturnLongDescription);
		}

		internal bool SetReturnMetadata(UGCQueryHandle_t handle, bool bReturnMetadata)
		{
			return this._SetReturnMetadata(this.Self, handle, bReturnMetadata);
		}

		internal bool SetReturnOnlyIDs(UGCQueryHandle_t handle, bool bReturnOnlyIDs)
		{
			return this._SetReturnOnlyIDs(this.Self, handle, bReturnOnlyIDs);
		}

		internal bool SetReturnPlaytimeStats(UGCQueryHandle_t handle, uint unDays)
		{
			return this._SetReturnPlaytimeStats(this.Self, handle, unDays);
		}

		internal bool SetReturnTotalOnly(UGCQueryHandle_t handle, bool bReturnTotalOnly)
		{
			return this._SetReturnTotalOnly(this.Self, handle, bReturnTotalOnly);
		}

		internal bool SetSearchText(UGCQueryHandle_t handle, string pSearchText)
		{
			return this._SetSearchText(this.Self, handle, pSearchText);
		}

		internal async Task<SetUserItemVoteResult_t?> SetUserItemVote(PublishedFileId nPublishedFileID, bool bVoteUp)
		{
			SetUserItemVoteResult_t? resultAsync = await SetUserItemVoteResult_t.GetResultAsync(this._SetUserItemVote(this.Self, nPublishedFileID, bVoteUp));
			return resultAsync;
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._CreateQueryUserUGCRequest = null;
			this._CreateQueryAllUGCRequest1 = null;
			this._CreateQueryAllUGCRequest2 = null;
			this._CreateQueryUGCDetailsRequest = null;
			this._SendQueryUGCRequest = null;
			this._GetQueryUGCResult = null;
			this._GetQueryUGCResult_Windows = null;
			this._GetQueryUGCPreviewURL = null;
			this._GetQueryUGCMetadata = null;
			this._GetQueryUGCChildren = null;
			this._GetQueryUGCStatistic = null;
			this._GetQueryUGCNumAdditionalPreviews = null;
			this._GetQueryUGCAdditionalPreview = null;
			this._GetQueryUGCNumKeyValueTags = null;
			this._GetQueryUGCKeyValueTag = null;
			this._ReleaseQueryUGCRequest = null;
			this._AddRequiredTag = null;
			this._AddExcludedTag = null;
			this._SetReturnOnlyIDs = null;
			this._SetReturnKeyValueTags = null;
			this._SetReturnLongDescription = null;
			this._SetReturnMetadata = null;
			this._SetReturnChildren = null;
			this._SetReturnAdditionalPreviews = null;
			this._SetReturnTotalOnly = null;
			this._SetReturnPlaytimeStats = null;
			this._SetLanguage = null;
			this._SetAllowCachedResponse = null;
			this._SetCloudFileNameFilter = null;
			this._SetMatchAnyTag = null;
			this._SetSearchText = null;
			this._SetRankedByTrendDays = null;
			this._AddRequiredKeyValueTag = null;
			this._RequestUGCDetails = null;
			this._CreateItem = null;
			this._StartItemUpdate = null;
			this._SetItemTitle = null;
			this._SetItemDescription = null;
			this._SetItemUpdateLanguage = null;
			this._SetItemMetadata = null;
			this._SetItemVisibility = null;
			this._SetItemTags = null;
			this._SetItemTags_Windows = null;
			this._SetItemContent = null;
			this._SetItemPreview = null;
			this._SetAllowLegacyUpload = null;
			this._RemoveItemKeyValueTags = null;
			this._AddItemKeyValueTag = null;
			this._AddItemPreviewFile = null;
			this._AddItemPreviewVideo = null;
			this._UpdateItemPreviewFile = null;
			this._UpdateItemPreviewVideo = null;
			this._RemoveItemPreview = null;
			this._SubmitItemUpdate = null;
			this._GetItemUpdateProgress = null;
			this._SetUserItemVote = null;
			this._GetUserItemVote = null;
			this._AddItemToFavorites = null;
			this._RemoveItemFromFavorites = null;
			this._SubscribeItem = null;
			this._UnsubscribeItem = null;
			this._GetNumSubscribedItems = null;
			this._GetSubscribedItems = null;
			this._GetItemState = null;
			this._GetItemInstallInfo = null;
			this._GetItemDownloadInfo = null;
			this._DownloadItem = null;
			this._BInitWorkshopForGameServer = null;
			this._SuspendDownloads = null;
			this._StartPlaytimeTracking = null;
			this._StopPlaytimeTracking = null;
			this._StopPlaytimeTrackingForAllItems = null;
			this._AddDependency = null;
			this._RemoveDependency = null;
			this._AddAppDependency = null;
			this._RemoveAppDependency = null;
			this._GetAppDependencies = null;
			this._DeleteItem = null;
		}

		internal UGCUpdateHandle_t StartItemUpdate(AppId nConsumerAppId, PublishedFileId nPublishedFileID)
		{
			return this._StartItemUpdate(this.Self, nConsumerAppId, nPublishedFileID);
		}

		internal async Task<StartPlaytimeTrackingResult_t?> StartPlaytimeTracking([In][Out] PublishedFileId[] pvecPublishedFileID, uint unNumPublishedFileIDs)
		{
			StartPlaytimeTrackingResult_t? resultAsync = await StartPlaytimeTrackingResult_t.GetResultAsync(this._StartPlaytimeTracking(this.Self, pvecPublishedFileID, unNumPublishedFileIDs));
			return resultAsync;
		}

		internal async Task<StopPlaytimeTrackingResult_t?> StopPlaytimeTracking([In][Out] PublishedFileId[] pvecPublishedFileID, uint unNumPublishedFileIDs)
		{
			StopPlaytimeTrackingResult_t? resultAsync = await StopPlaytimeTrackingResult_t.GetResultAsync(this._StopPlaytimeTracking(this.Self, pvecPublishedFileID, unNumPublishedFileIDs));
			return resultAsync;
		}

		internal async Task<StopPlaytimeTrackingResult_t?> StopPlaytimeTrackingForAllItems()
		{
			StopPlaytimeTrackingResult_t? resultAsync = await StopPlaytimeTrackingResult_t.GetResultAsync(this._StopPlaytimeTrackingForAllItems(this.Self));
			return resultAsync;
		}

		internal async Task<SubmitItemUpdateResult_t?> SubmitItemUpdate(UGCUpdateHandle_t handle, string pchChangeNote)
		{
			SubmitItemUpdateResult_t? resultAsync = await SubmitItemUpdateResult_t.GetResultAsync(this._SubmitItemUpdate(this.Self, handle, pchChangeNote));
			return resultAsync;
		}

		internal async Task<RemoteStorageSubscribePublishedFileResult_t?> SubscribeItem(PublishedFileId nPublishedFileID)
		{
			RemoteStorageSubscribePublishedFileResult_t? resultAsync = await RemoteStorageSubscribePublishedFileResult_t.GetResultAsync(this._SubscribeItem(this.Self, nPublishedFileID));
			return resultAsync;
		}

		internal void SuspendDownloads(bool bSuspend)
		{
			this._SuspendDownloads(this.Self, bSuspend);
		}

		internal async Task<RemoteStorageUnsubscribePublishedFileResult_t?> UnsubscribeItem(PublishedFileId nPublishedFileID)
		{
			RemoteStorageUnsubscribePublishedFileResult_t? resultAsync = await RemoteStorageUnsubscribePublishedFileResult_t.GetResultAsync(this._UnsubscribeItem(this.Self, nPublishedFileID));
			return resultAsync;
		}

		internal bool UpdateItemPreviewFile(UGCUpdateHandle_t handle, uint index, string pszPreviewFile)
		{
			return this._UpdateItemPreviewFile(this.Self, handle, index, pszPreviewFile);
		}

		internal bool UpdateItemPreviewVideo(UGCUpdateHandle_t handle, uint index, string pszVideoID)
		{
			return this._UpdateItemPreviewVideo(this.Self, handle, index, pszVideoID);
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FAddAppDependency(IntPtr self, PublishedFileId nPublishedFileID, AppId nAppID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FAddDependency(IntPtr self, PublishedFileId nParentPublishedFileID, PublishedFileId nChildPublishedFileID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FAddExcludedTag(IntPtr self, UGCQueryHandle_t handle, string pTagName);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FAddItemKeyValueTag(IntPtr self, UGCUpdateHandle_t handle, string pchKey, string pchValue);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FAddItemPreviewFile(IntPtr self, UGCUpdateHandle_t handle, string pszPreviewFile, ItemPreviewType type);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FAddItemPreviewVideo(IntPtr self, UGCUpdateHandle_t handle, string pszVideoID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FAddItemToFavorites(IntPtr self, AppId nAppId, PublishedFileId nPublishedFileID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FAddRequiredKeyValueTag(IntPtr self, UGCQueryHandle_t handle, string pKey, string pValue);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FAddRequiredTag(IntPtr self, UGCQueryHandle_t handle, string pTagName);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBInitWorkshopForGameServer(IntPtr self, DepotId_t unWorkshopDepotID, string pszFolder);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FCreateItem(IntPtr self, AppId nConsumerAppId, WorkshopFileType eFileType);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate UGCQueryHandle_t FCreateQueryAllUGCRequest1(IntPtr self, UGCQuery eQueryType, UgcType eMatchingeMatchingUGCTypeFileType, AppId nCreatorAppID, AppId nConsumerAppID, uint unPage);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate UGCQueryHandle_t FCreateQueryAllUGCRequest2(IntPtr self, UGCQuery eQueryType, UgcType eMatchingeMatchingUGCTypeFileType, AppId nCreatorAppID, AppId nConsumerAppID, string pchCursor);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate UGCQueryHandle_t FCreateQueryUGCDetailsRequest(IntPtr self, [In][Out] PublishedFileId[] pvecPublishedFileID, uint unNumPublishedFileIDs);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate UGCQueryHandle_t FCreateQueryUserUGCRequest(IntPtr self, AccountID_t unAccountID, UserUGCList eListType, UgcType eMatchingUGCType, UserUGCListSortOrder eSortOrder, AppId nCreatorAppID, AppId nConsumerAppID, uint unPage);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FDeleteItem(IntPtr self, PublishedFileId nPublishedFileID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FDownloadItem(IntPtr self, PublishedFileId nPublishedFileID, bool bHighPriority);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FGetAppDependencies(IntPtr self, PublishedFileId nPublishedFileID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetItemDownloadInfo(IntPtr self, PublishedFileId nPublishedFileID, ref ulong punBytesDownloaded, ref ulong punBytesTotal);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetItemInstallInfo(IntPtr self, PublishedFileId nPublishedFileID, ref ulong punSizeOnDisk, StringBuilder pchFolder, uint cchFolderSize, ref uint punTimeStamp);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetItemState(IntPtr self, PublishedFileId nPublishedFileID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate ItemUpdateStatus FGetItemUpdateProgress(IntPtr self, UGCUpdateHandle_t handle, ref ulong punBytesProcessed, ref ulong punBytesTotal);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetNumSubscribedItems(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetQueryUGCAdditionalPreview(IntPtr self, UGCQueryHandle_t handle, uint index, uint previewIndex, StringBuilder pchURLOrVideoID, uint cchURLSize, StringBuilder pchOriginalFileName, uint cchOriginalFileNameSize, ref ItemPreviewType pPreviewType);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetQueryUGCChildren(IntPtr self, UGCQueryHandle_t handle, uint index, [In][Out] PublishedFileId[] pvecPublishedFileID, uint cMaxEntries);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetQueryUGCKeyValueTag(IntPtr self, UGCQueryHandle_t handle, uint index, uint keyValueTagIndex, StringBuilder pchKey, uint cchKeySize, StringBuilder pchValue, uint cchValueSize);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetQueryUGCMetadata(IntPtr self, UGCQueryHandle_t handle, uint index, StringBuilder pchMetadata, uint cchMetadatasize);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetQueryUGCNumAdditionalPreviews(IntPtr self, UGCQueryHandle_t handle, uint index);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetQueryUGCNumKeyValueTags(IntPtr self, UGCQueryHandle_t handle, uint index);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetQueryUGCPreviewURL(IntPtr self, UGCQueryHandle_t handle, uint index, StringBuilder pchURL, uint cchURLSize);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetQueryUGCResult(IntPtr self, UGCQueryHandle_t handle, uint index, ref SteamUGCDetails_t pDetails);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetQueryUGCResult_Windows(IntPtr self, UGCQueryHandle_t handle, uint index, ref SteamUGCDetails_t.Pack8 pDetails);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetQueryUGCStatistic(IntPtr self, UGCQueryHandle_t handle, uint index, ItemStatistic eStatType, ref ulong pStatValue);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetSubscribedItems(IntPtr self, [In][Out] PublishedFileId[] pvecPublishedFileID, uint cMaxEntries);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FGetUserItemVote(IntPtr self, PublishedFileId nPublishedFileID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FReleaseQueryUGCRequest(IntPtr self, UGCQueryHandle_t handle);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FRemoveAppDependency(IntPtr self, PublishedFileId nPublishedFileID, AppId nAppID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FRemoveDependency(IntPtr self, PublishedFileId nParentPublishedFileID, PublishedFileId nChildPublishedFileID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FRemoveItemFromFavorites(IntPtr self, AppId nAppId, PublishedFileId nPublishedFileID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FRemoveItemKeyValueTags(IntPtr self, UGCUpdateHandle_t handle, string pchKey);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FRemoveItemPreview(IntPtr self, UGCUpdateHandle_t handle, uint index);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FRequestUGCDetails(IntPtr self, PublishedFileId nPublishedFileID, uint unMaxAgeSeconds);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FSendQueryUGCRequest(IntPtr self, UGCQueryHandle_t handle);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetAllowCachedResponse(IntPtr self, UGCQueryHandle_t handle, uint unMaxAgeSeconds);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetAllowLegacyUpload(IntPtr self, UGCUpdateHandle_t handle, bool bAllowLegacyUpload);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetCloudFileNameFilter(IntPtr self, UGCQueryHandle_t handle, string pMatchCloudFileName);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetItemContent(IntPtr self, UGCUpdateHandle_t handle, string pszContentFolder);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetItemDescription(IntPtr self, UGCUpdateHandle_t handle, string pchDescription);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetItemMetadata(IntPtr self, UGCUpdateHandle_t handle, string pchMetaData);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetItemPreview(IntPtr self, UGCUpdateHandle_t handle, string pszPreviewFile);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetItemTags(IntPtr self, UGCUpdateHandle_t updateHandle, ref SteamParamStringArray_t pTags);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetItemTags_Windows(IntPtr self, UGCUpdateHandle_t updateHandle, ref SteamParamStringArray_t.Pack8 pTags);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetItemTitle(IntPtr self, UGCUpdateHandle_t handle, string pchTitle);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetItemUpdateLanguage(IntPtr self, UGCUpdateHandle_t handle, string pchLanguage);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetItemVisibility(IntPtr self, UGCUpdateHandle_t handle, RemoteStoragePublishedFileVisibility eVisibility);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetLanguage(IntPtr self, UGCQueryHandle_t handle, string pchLanguage);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetMatchAnyTag(IntPtr self, UGCQueryHandle_t handle, bool bMatchAnyTag);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetRankedByTrendDays(IntPtr self, UGCQueryHandle_t handle, uint unDays);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetReturnAdditionalPreviews(IntPtr self, UGCQueryHandle_t handle, bool bReturnAdditionalPreviews);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetReturnChildren(IntPtr self, UGCQueryHandle_t handle, bool bReturnChildren);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetReturnKeyValueTags(IntPtr self, UGCQueryHandle_t handle, bool bReturnKeyValueTags);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetReturnLongDescription(IntPtr self, UGCQueryHandle_t handle, bool bReturnLongDescription);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetReturnMetadata(IntPtr self, UGCQueryHandle_t handle, bool bReturnMetadata);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetReturnOnlyIDs(IntPtr self, UGCQueryHandle_t handle, bool bReturnOnlyIDs);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetReturnPlaytimeStats(IntPtr self, UGCQueryHandle_t handle, uint unDays);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetReturnTotalOnly(IntPtr self, UGCQueryHandle_t handle, bool bReturnTotalOnly);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetSearchText(IntPtr self, UGCQueryHandle_t handle, string pSearchText);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FSetUserItemVote(IntPtr self, PublishedFileId nPublishedFileID, bool bVoteUp);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate UGCUpdateHandle_t FStartItemUpdate(IntPtr self, AppId nConsumerAppId, PublishedFileId nPublishedFileID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FStartPlaytimeTracking(IntPtr self, [In][Out] PublishedFileId[] pvecPublishedFileID, uint unNumPublishedFileIDs);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FStopPlaytimeTracking(IntPtr self, [In][Out] PublishedFileId[] pvecPublishedFileID, uint unNumPublishedFileIDs);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FStopPlaytimeTrackingForAllItems(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FSubmitItemUpdate(IntPtr self, UGCUpdateHandle_t handle, string pchChangeNote);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FSubscribeItem(IntPtr self, PublishedFileId nPublishedFileID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSuspendDownloads(IntPtr self, bool bSuspend);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FUnsubscribeItem(IntPtr self, PublishedFileId nPublishedFileID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FUpdateItemPreviewFile(IntPtr self, UGCUpdateHandle_t handle, uint index, string pszPreviewFile);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FUpdateItemPreviewVideo(IntPtr self, UGCUpdateHandle_t handle, uint index, string pszVideoID);
	}
}