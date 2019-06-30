using Steamworks.Data;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks
{
	internal class ISteamFriends : SteamInterface
	{
		private ISteamFriends.FGetPersonaName _GetPersonaName;

		private ISteamFriends.FSetPersonaName _SetPersonaName;

		private ISteamFriends.FGetPersonaState _GetPersonaState;

		private ISteamFriends.FGetFriendCount _GetFriendCount;

		private ISteamFriends.FGetFriendByIndex _GetFriendByIndex;

		private ISteamFriends.FGetFriendByIndex_Windows _GetFriendByIndex_Windows;

		private ISteamFriends.FGetFriendRelationship _GetFriendRelationship;

		private ISteamFriends.FGetFriendPersonaState _GetFriendPersonaState;

		private ISteamFriends.FGetFriendPersonaName _GetFriendPersonaName;

		private ISteamFriends.FGetFriendGamePlayed _GetFriendGamePlayed;

		private ISteamFriends.FGetFriendPersonaNameHistory _GetFriendPersonaNameHistory;

		private ISteamFriends.FGetFriendSteamLevel _GetFriendSteamLevel;

		private ISteamFriends.FGetPlayerNickname _GetPlayerNickname;

		private ISteamFriends.FGetFriendsGroupCount _GetFriendsGroupCount;

		private ISteamFriends.FGetFriendsGroupIDByIndex _GetFriendsGroupIDByIndex;

		private ISteamFriends.FGetFriendsGroupName _GetFriendsGroupName;

		private ISteamFriends.FGetFriendsGroupMembersCount _GetFriendsGroupMembersCount;

		private ISteamFriends.FGetFriendsGroupMembersList _GetFriendsGroupMembersList;

		private ISteamFriends.FHasFriend _HasFriend;

		private ISteamFriends.FGetClanCount _GetClanCount;

		private ISteamFriends.FGetClanByIndex _GetClanByIndex;

		private ISteamFriends.FGetClanByIndex_Windows _GetClanByIndex_Windows;

		private ISteamFriends.FGetClanName _GetClanName;

		private ISteamFriends.FGetClanTag _GetClanTag;

		private ISteamFriends.FGetClanActivityCounts _GetClanActivityCounts;

		private ISteamFriends.FDownloadClanActivityCounts _DownloadClanActivityCounts;

		private ISteamFriends.FGetFriendCountFromSource _GetFriendCountFromSource;

		private ISteamFriends.FGetFriendFromSourceByIndex _GetFriendFromSourceByIndex;

		private ISteamFriends.FGetFriendFromSourceByIndex_Windows _GetFriendFromSourceByIndex_Windows;

		private ISteamFriends.FIsUserInSource _IsUserInSource;

		private ISteamFriends.FSetInGameVoiceSpeaking _SetInGameVoiceSpeaking;

		private ISteamFriends.FActivateGameOverlay _ActivateGameOverlay;

		private ISteamFriends.FActivateGameOverlayToUser _ActivateGameOverlayToUser;

		private ISteamFriends.FActivateGameOverlayToWebPage _ActivateGameOverlayToWebPage;

		private ISteamFriends.FActivateGameOverlayToStore _ActivateGameOverlayToStore;

		private ISteamFriends.FSetPlayedWith _SetPlayedWith;

		private ISteamFriends.FActivateGameOverlayInviteDialog _ActivateGameOverlayInviteDialog;

		private ISteamFriends.FGetSmallFriendAvatar _GetSmallFriendAvatar;

		private ISteamFriends.FGetMediumFriendAvatar _GetMediumFriendAvatar;

		private ISteamFriends.FGetLargeFriendAvatar _GetLargeFriendAvatar;

		private ISteamFriends.FRequestUserInformation _RequestUserInformation;

		private ISteamFriends.FRequestClanOfficerList _RequestClanOfficerList;

		private ISteamFriends.FGetClanOwner _GetClanOwner;

		private ISteamFriends.FGetClanOwner_Windows _GetClanOwner_Windows;

		private ISteamFriends.FGetClanOfficerCount _GetClanOfficerCount;

		private ISteamFriends.FGetClanOfficerByIndex _GetClanOfficerByIndex;

		private ISteamFriends.FGetClanOfficerByIndex_Windows _GetClanOfficerByIndex_Windows;

		private ISteamFriends.FGetUserRestrictions _GetUserRestrictions;

		private ISteamFriends.FSetRichPresence _SetRichPresence;

		private ISteamFriends.FClearRichPresence _ClearRichPresence;

		private ISteamFriends.FGetFriendRichPresence _GetFriendRichPresence;

		private ISteamFriends.FGetFriendRichPresenceKeyCount _GetFriendRichPresenceKeyCount;

		private ISteamFriends.FGetFriendRichPresenceKeyByIndex _GetFriendRichPresenceKeyByIndex;

		private ISteamFriends.FRequestFriendRichPresence _RequestFriendRichPresence;

		private ISteamFriends.FInviteUserToGame _InviteUserToGame;

		private ISteamFriends.FGetCoplayFriendCount _GetCoplayFriendCount;

		private ISteamFriends.FGetCoplayFriend _GetCoplayFriend;

		private ISteamFriends.FGetCoplayFriend_Windows _GetCoplayFriend_Windows;

		private ISteamFriends.FGetFriendCoplayTime _GetFriendCoplayTime;

		private ISteamFriends.FGetFriendCoplayGame _GetFriendCoplayGame;

		private ISteamFriends.FJoinClanChatRoom _JoinClanChatRoom;

		private ISteamFriends.FLeaveClanChatRoom _LeaveClanChatRoom;

		private ISteamFriends.FGetClanChatMemberCount _GetClanChatMemberCount;

		private ISteamFriends.FGetChatMemberByIndex _GetChatMemberByIndex;

		private ISteamFriends.FGetChatMemberByIndex_Windows _GetChatMemberByIndex_Windows;

		private ISteamFriends.FSendClanChatMessage _SendClanChatMessage;

		private ISteamFriends.FGetClanChatMessage _GetClanChatMessage;

		private ISteamFriends.FIsClanChatAdmin _IsClanChatAdmin;

		private ISteamFriends.FIsClanChatWindowOpenInSteam _IsClanChatWindowOpenInSteam;

		private ISteamFriends.FOpenClanChatWindowInSteam _OpenClanChatWindowInSteam;

		private ISteamFriends.FCloseClanChatWindowInSteam _CloseClanChatWindowInSteam;

		private ISteamFriends.FSetListenForFriendsMessages _SetListenForFriendsMessages;

		private ISteamFriends.FReplyToFriendMessage _ReplyToFriendMessage;

		private ISteamFriends.FGetFriendMessage _GetFriendMessage;

		private ISteamFriends.FGetFollowerCount _GetFollowerCount;

		private ISteamFriends.FIsFollowing _IsFollowing;

		private ISteamFriends.FEnumerateFollowingList _EnumerateFollowingList;

		private ISteamFriends.FIsClanPublic _IsClanPublic;

		private ISteamFriends.FIsClanOfficialGameGroup _IsClanOfficialGameGroup;

		private ISteamFriends.FGetNumChatsWithUnreadPriorityMessages _GetNumChatsWithUnreadPriorityMessages;

		public override string InterfaceName
		{
			get
			{
				return "SteamFriends017";
			}
		}

		public ISteamFriends()
		{
		}

		internal void ActivateGameOverlay(string pchDialog)
		{
			this._ActivateGameOverlay(this.Self, pchDialog);
		}

		internal void ActivateGameOverlayInviteDialog(SteamId steamIDLobby)
		{
			this._ActivateGameOverlayInviteDialog(this.Self, steamIDLobby);
		}

		internal void ActivateGameOverlayToStore(AppId nAppID, OverlayToStoreFlag eFlag)
		{
			this._ActivateGameOverlayToStore(this.Self, nAppID, eFlag);
		}

		internal void ActivateGameOverlayToUser(string pchDialog, SteamId steamID)
		{
			this._ActivateGameOverlayToUser(this.Self, pchDialog, steamID);
		}

		internal void ActivateGameOverlayToWebPage(string pchURL, ActivateGameOverlayToWebPageMode eMode)
		{
			this._ActivateGameOverlayToWebPage(this.Self, pchURL, eMode);
		}

		internal void ClearRichPresence()
		{
			this._ClearRichPresence(this.Self);
		}

		internal bool CloseClanChatWindowInSteam(SteamId steamIDClanChat)
		{
			return this._CloseClanChatWindowInSteam(this.Self, steamIDClanChat);
		}

		internal async Task<DownloadClanActivityCountsResult_t?> DownloadClanActivityCounts([In][Out] SteamId[] psteamIDClans, int cClansToRequest)
		{
			DownloadClanActivityCountsResult_t? resultAsync = await DownloadClanActivityCountsResult_t.GetResultAsync(this._DownloadClanActivityCounts(this.Self, psteamIDClans, cClansToRequest));
			return resultAsync;
		}

		internal async Task<FriendsEnumerateFollowingList_t?> EnumerateFollowingList(uint unStartIndex)
		{
			FriendsEnumerateFollowingList_t? resultAsync = await FriendsEnumerateFollowingList_t.GetResultAsync(this._EnumerateFollowingList(this.Self, unStartIndex));
			return resultAsync;
		}

		internal SteamId GetChatMemberByIndex(SteamId steamIDClan, int iUser)
		{
			SteamId self;
			if (Config.Os != OsType.Windows)
			{
				self = this._GetChatMemberByIndex(this.Self, steamIDClan, iUser);
			}
			else
			{
				SteamId steamId = new SteamId();
				this._GetChatMemberByIndex_Windows(this.Self, ref steamId, steamIDClan, iUser);
				self = steamId;
			}
			return self;
		}

		internal bool GetClanActivityCounts(SteamId steamIDClan, ref int pnOnline, ref int pnInGame, ref int pnChatting)
		{
			bool self = this._GetClanActivityCounts(this.Self, steamIDClan, ref pnOnline, ref pnInGame, ref pnChatting);
			return self;
		}

		internal SteamId GetClanByIndex(int iClan)
		{
			SteamId self;
			if (Config.Os != OsType.Windows)
			{
				self = this._GetClanByIndex(this.Self, iClan);
			}
			else
			{
				SteamId steamId = new SteamId();
				this._GetClanByIndex_Windows(this.Self, ref steamId, iClan);
				self = steamId;
			}
			return self;
		}

		internal int GetClanChatMemberCount(SteamId steamIDClan)
		{
			return this._GetClanChatMemberCount(this.Self, steamIDClan);
		}

		internal int GetClanChatMessage(SteamId steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, ref ChatEntryType peChatEntryType, ref SteamId psteamidChatter)
		{
			int self = this._GetClanChatMessage(this.Self, steamIDClanChat, iMessage, prgchText, cchTextMax, ref peChatEntryType, ref psteamidChatter);
			return self;
		}

		internal int GetClanCount()
		{
			return this._GetClanCount(this.Self);
		}

		internal string GetClanName(SteamId steamIDClan)
		{
			string str = base.GetString(this._GetClanName(this.Self, steamIDClan));
			return str;
		}

		internal SteamId GetClanOfficerByIndex(SteamId steamIDClan, int iOfficer)
		{
			SteamId self;
			if (Config.Os != OsType.Windows)
			{
				self = this._GetClanOfficerByIndex(this.Self, steamIDClan, iOfficer);
			}
			else
			{
				SteamId steamId = new SteamId();
				this._GetClanOfficerByIndex_Windows(this.Self, ref steamId, steamIDClan, iOfficer);
				self = steamId;
			}
			return self;
		}

		internal int GetClanOfficerCount(SteamId steamIDClan)
		{
			return this._GetClanOfficerCount(this.Self, steamIDClan);
		}

		internal SteamId GetClanOwner(SteamId steamIDClan)
		{
			SteamId self;
			if (Config.Os != OsType.Windows)
			{
				self = this._GetClanOwner(this.Self, steamIDClan);
			}
			else
			{
				SteamId steamId = new SteamId();
				this._GetClanOwner_Windows(this.Self, ref steamId, steamIDClan);
				self = steamId;
			}
			return self;
		}

		internal string GetClanTag(SteamId steamIDClan)
		{
			string str = base.GetString(this._GetClanTag(this.Self, steamIDClan));
			return str;
		}

		internal SteamId GetCoplayFriend(int iCoplayFriend)
		{
			SteamId self;
			if (Config.Os != OsType.Windows)
			{
				self = this._GetCoplayFriend(this.Self, iCoplayFriend);
			}
			else
			{
				SteamId steamId = new SteamId();
				this._GetCoplayFriend_Windows(this.Self, ref steamId, iCoplayFriend);
				self = steamId;
			}
			return self;
		}

		internal int GetCoplayFriendCount()
		{
			return this._GetCoplayFriendCount(this.Self);
		}

		internal async Task<FriendsGetFollowerCount_t?> GetFollowerCount(SteamId steamID)
		{
			FriendsGetFollowerCount_t? resultAsync = await FriendsGetFollowerCount_t.GetResultAsync(this._GetFollowerCount(this.Self, steamID));
			return resultAsync;
		}

		internal SteamId GetFriendByIndex(int iFriend, int iFriendFlags)
		{
			SteamId self;
			if (Config.Os != OsType.Windows)
			{
				self = this._GetFriendByIndex(this.Self, iFriend, iFriendFlags);
			}
			else
			{
				SteamId steamId = new SteamId();
				this._GetFriendByIndex_Windows(this.Self, ref steamId, iFriend, iFriendFlags);
				self = steamId;
			}
			return self;
		}

		internal AppId GetFriendCoplayGame(SteamId steamIDFriend)
		{
			return this._GetFriendCoplayGame(this.Self, steamIDFriend);
		}

		internal int GetFriendCoplayTime(SteamId steamIDFriend)
		{
			return this._GetFriendCoplayTime(this.Self, steamIDFriend);
		}

		internal int GetFriendCount(int iFriendFlags)
		{
			return this._GetFriendCount(this.Self, iFriendFlags);
		}

		internal int GetFriendCountFromSource(SteamId steamIDSource)
		{
			return this._GetFriendCountFromSource(this.Self, steamIDSource);
		}

		internal SteamId GetFriendFromSourceByIndex(SteamId steamIDSource, int iFriend)
		{
			SteamId self;
			if (Config.Os != OsType.Windows)
			{
				self = this._GetFriendFromSourceByIndex(this.Self, steamIDSource, iFriend);
			}
			else
			{
				SteamId steamId = new SteamId();
				this._GetFriendFromSourceByIndex_Windows(this.Self, ref steamId, steamIDSource, iFriend);
				self = steamId;
			}
			return self;
		}

		internal bool GetFriendGamePlayed(SteamId steamIDFriend, ref FriendGameInfo_t pFriendGameInfo)
		{
			return this._GetFriendGamePlayed(this.Self, steamIDFriend, ref pFriendGameInfo);
		}

		internal int GetFriendMessage(SteamId steamIDFriend, int iMessageID, IntPtr pvData, int cubData, ref ChatEntryType peChatEntryType)
		{
			int self = this._GetFriendMessage(this.Self, steamIDFriend, iMessageID, pvData, cubData, ref peChatEntryType);
			return self;
		}

		internal string GetFriendPersonaName(SteamId steamIDFriend)
		{
			string str = base.GetString(this._GetFriendPersonaName(this.Self, steamIDFriend));
			return str;
		}

		internal string GetFriendPersonaNameHistory(SteamId steamIDFriend, int iPersonaName)
		{
			string str = base.GetString(this._GetFriendPersonaNameHistory(this.Self, steamIDFriend, iPersonaName));
			return str;
		}

		internal FriendState GetFriendPersonaState(SteamId steamIDFriend)
		{
			return this._GetFriendPersonaState(this.Self, steamIDFriend);
		}

		internal Relationship GetFriendRelationship(SteamId steamIDFriend)
		{
			return this._GetFriendRelationship(this.Self, steamIDFriend);
		}

		internal string GetFriendRichPresence(SteamId steamIDFriend, string pchKey)
		{
			string str = base.GetString(this._GetFriendRichPresence(this.Self, steamIDFriend, pchKey));
			return str;
		}

		internal string GetFriendRichPresenceKeyByIndex(SteamId steamIDFriend, int iKey)
		{
			string str = base.GetString(this._GetFriendRichPresenceKeyByIndex(this.Self, steamIDFriend, iKey));
			return str;
		}

		internal int GetFriendRichPresenceKeyCount(SteamId steamIDFriend)
		{
			return this._GetFriendRichPresenceKeyCount(this.Self, steamIDFriend);
		}

		internal int GetFriendsGroupCount()
		{
			return this._GetFriendsGroupCount(this.Self);
		}

		internal FriendsGroupID_t GetFriendsGroupIDByIndex(int iFG)
		{
			return this._GetFriendsGroupIDByIndex(this.Self, iFG);
		}

		internal int GetFriendsGroupMembersCount(FriendsGroupID_t friendsGroupID)
		{
			return this._GetFriendsGroupMembersCount(this.Self, friendsGroupID);
		}

		internal void GetFriendsGroupMembersList(FriendsGroupID_t friendsGroupID, [In][Out] SteamId[] pOutSteamIDMembers, int nMembersCount)
		{
			this._GetFriendsGroupMembersList(this.Self, friendsGroupID, pOutSteamIDMembers, nMembersCount);
		}

		internal string GetFriendsGroupName(FriendsGroupID_t friendsGroupID)
		{
			string str = base.GetString(this._GetFriendsGroupName(this.Self, friendsGroupID));
			return str;
		}

		internal int GetFriendSteamLevel(SteamId steamIDFriend)
		{
			return this._GetFriendSteamLevel(this.Self, steamIDFriend);
		}

		internal int GetLargeFriendAvatar(SteamId steamIDFriend)
		{
			return this._GetLargeFriendAvatar(this.Self, steamIDFriend);
		}

		internal int GetMediumFriendAvatar(SteamId steamIDFriend)
		{
			return this._GetMediumFriendAvatar(this.Self, steamIDFriend);
		}

		internal int GetNumChatsWithUnreadPriorityMessages()
		{
			return this._GetNumChatsWithUnreadPriorityMessages(this.Self);
		}

		internal string GetPersonaName()
		{
			return base.GetString(this._GetPersonaName(this.Self));
		}

		internal FriendState GetPersonaState()
		{
			return this._GetPersonaState(this.Self);
		}

		internal string GetPlayerNickname(SteamId steamIDPlayer)
		{
			string str = base.GetString(this._GetPlayerNickname(this.Self, steamIDPlayer));
			return str;
		}

		internal int GetSmallFriendAvatar(SteamId steamIDFriend)
		{
			return this._GetSmallFriendAvatar(this.Self, steamIDFriend);
		}

		internal uint GetUserRestrictions()
		{
			return this._GetUserRestrictions(this.Self);
		}

		internal bool HasFriend(SteamId steamIDFriend, int iFriendFlags)
		{
			return this._HasFriend(this.Self, steamIDFriend, iFriendFlags);
		}

		public override void InitInternals()
		{
			this._GetPersonaName = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetPersonaName>(Marshal.ReadIntPtr(this.VTable, 0));
			this._SetPersonaName = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FSetPersonaName>(Marshal.ReadIntPtr(this.VTable, 8));
			this._GetPersonaState = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetPersonaState>(Marshal.ReadIntPtr(this.VTable, 16));
			this._GetFriendCount = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendCount>(Marshal.ReadIntPtr(this.VTable, 24));
			this._GetFriendByIndex = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendByIndex>(Marshal.ReadIntPtr(this.VTable, 32));
			this._GetFriendByIndex_Windows = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendByIndex_Windows>(Marshal.ReadIntPtr(this.VTable, 32));
			this._GetFriendRelationship = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendRelationship>(Marshal.ReadIntPtr(this.VTable, 40));
			this._GetFriendPersonaState = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendPersonaState>(Marshal.ReadIntPtr(this.VTable, 48));
			this._GetFriendPersonaName = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendPersonaName>(Marshal.ReadIntPtr(this.VTable, 56));
			this._GetFriendGamePlayed = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendGamePlayed>(Marshal.ReadIntPtr(this.VTable, 64));
			this._GetFriendPersonaNameHistory = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendPersonaNameHistory>(Marshal.ReadIntPtr(this.VTable, 72));
			this._GetFriendSteamLevel = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendSteamLevel>(Marshal.ReadIntPtr(this.VTable, 80));
			this._GetPlayerNickname = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetPlayerNickname>(Marshal.ReadIntPtr(this.VTable, 88));
			this._GetFriendsGroupCount = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendsGroupCount>(Marshal.ReadIntPtr(this.VTable, 96));
			this._GetFriendsGroupIDByIndex = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendsGroupIDByIndex>(Marshal.ReadIntPtr(this.VTable, 104));
			this._GetFriendsGroupName = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendsGroupName>(Marshal.ReadIntPtr(this.VTable, 112));
			this._GetFriendsGroupMembersCount = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendsGroupMembersCount>(Marshal.ReadIntPtr(this.VTable, 120));
			this._GetFriendsGroupMembersList = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendsGroupMembersList>(Marshal.ReadIntPtr(this.VTable, 128));
			this._HasFriend = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FHasFriend>(Marshal.ReadIntPtr(this.VTable, 136));
			this._GetClanCount = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetClanCount>(Marshal.ReadIntPtr(this.VTable, 144));
			this._GetClanByIndex = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetClanByIndex>(Marshal.ReadIntPtr(this.VTable, 152));
			this._GetClanByIndex_Windows = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetClanByIndex_Windows>(Marshal.ReadIntPtr(this.VTable, 152));
			this._GetClanName = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetClanName>(Marshal.ReadIntPtr(this.VTable, 160));
			this._GetClanTag = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetClanTag>(Marshal.ReadIntPtr(this.VTable, 168));
			this._GetClanActivityCounts = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetClanActivityCounts>(Marshal.ReadIntPtr(this.VTable, 176));
			this._DownloadClanActivityCounts = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FDownloadClanActivityCounts>(Marshal.ReadIntPtr(this.VTable, 184));
			this._GetFriendCountFromSource = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendCountFromSource>(Marshal.ReadIntPtr(this.VTable, 192));
			this._GetFriendFromSourceByIndex = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendFromSourceByIndex>(Marshal.ReadIntPtr(this.VTable, 200));
			this._GetFriendFromSourceByIndex_Windows = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendFromSourceByIndex_Windows>(Marshal.ReadIntPtr(this.VTable, 200));
			this._IsUserInSource = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FIsUserInSource>(Marshal.ReadIntPtr(this.VTable, 208));
			this._SetInGameVoiceSpeaking = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FSetInGameVoiceSpeaking>(Marshal.ReadIntPtr(this.VTable, 216));
			this._ActivateGameOverlay = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FActivateGameOverlay>(Marshal.ReadIntPtr(this.VTable, 224));
			this._ActivateGameOverlayToUser = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FActivateGameOverlayToUser>(Marshal.ReadIntPtr(this.VTable, 232));
			this._ActivateGameOverlayToWebPage = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FActivateGameOverlayToWebPage>(Marshal.ReadIntPtr(this.VTable, 240));
			this._ActivateGameOverlayToStore = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FActivateGameOverlayToStore>(Marshal.ReadIntPtr(this.VTable, 248));
			this._SetPlayedWith = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FSetPlayedWith>(Marshal.ReadIntPtr(this.VTable, 256));
			this._ActivateGameOverlayInviteDialog = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FActivateGameOverlayInviteDialog>(Marshal.ReadIntPtr(this.VTable, 264));
			this._GetSmallFriendAvatar = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetSmallFriendAvatar>(Marshal.ReadIntPtr(this.VTable, 272));
			this._GetMediumFriendAvatar = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetMediumFriendAvatar>(Marshal.ReadIntPtr(this.VTable, 280));
			this._GetLargeFriendAvatar = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetLargeFriendAvatar>(Marshal.ReadIntPtr(this.VTable, 288));
			this._RequestUserInformation = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FRequestUserInformation>(Marshal.ReadIntPtr(this.VTable, 296));
			this._RequestClanOfficerList = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FRequestClanOfficerList>(Marshal.ReadIntPtr(this.VTable, 304));
			this._GetClanOwner = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetClanOwner>(Marshal.ReadIntPtr(this.VTable, 312));
			this._GetClanOwner_Windows = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetClanOwner_Windows>(Marshal.ReadIntPtr(this.VTable, 312));
			this._GetClanOfficerCount = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetClanOfficerCount>(Marshal.ReadIntPtr(this.VTable, 320));
			this._GetClanOfficerByIndex = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetClanOfficerByIndex>(Marshal.ReadIntPtr(this.VTable, 328));
			this._GetClanOfficerByIndex_Windows = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetClanOfficerByIndex_Windows>(Marshal.ReadIntPtr(this.VTable, 328));
			this._GetUserRestrictions = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetUserRestrictions>(Marshal.ReadIntPtr(this.VTable, 336));
			this._SetRichPresence = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FSetRichPresence>(Marshal.ReadIntPtr(this.VTable, 344));
			this._ClearRichPresence = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FClearRichPresence>(Marshal.ReadIntPtr(this.VTable, 352));
			this._GetFriendRichPresence = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendRichPresence>(Marshal.ReadIntPtr(this.VTable, 360));
			this._GetFriendRichPresenceKeyCount = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendRichPresenceKeyCount>(Marshal.ReadIntPtr(this.VTable, 368));
			this._GetFriendRichPresenceKeyByIndex = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendRichPresenceKeyByIndex>(Marshal.ReadIntPtr(this.VTable, 376));
			this._RequestFriendRichPresence = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FRequestFriendRichPresence>(Marshal.ReadIntPtr(this.VTable, 384));
			this._InviteUserToGame = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FInviteUserToGame>(Marshal.ReadIntPtr(this.VTable, 392));
			this._GetCoplayFriendCount = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetCoplayFriendCount>(Marshal.ReadIntPtr(this.VTable, 400));
			this._GetCoplayFriend = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetCoplayFriend>(Marshal.ReadIntPtr(this.VTable, 408));
			this._GetCoplayFriend_Windows = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetCoplayFriend_Windows>(Marshal.ReadIntPtr(this.VTable, 408));
			this._GetFriendCoplayTime = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendCoplayTime>(Marshal.ReadIntPtr(this.VTable, 416));
			this._GetFriendCoplayGame = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendCoplayGame>(Marshal.ReadIntPtr(this.VTable, 424));
			this._JoinClanChatRoom = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FJoinClanChatRoom>(Marshal.ReadIntPtr(this.VTable, 432));
			this._LeaveClanChatRoom = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FLeaveClanChatRoom>(Marshal.ReadIntPtr(this.VTable, 440));
			this._GetClanChatMemberCount = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetClanChatMemberCount>(Marshal.ReadIntPtr(this.VTable, 448));
			this._GetChatMemberByIndex = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetChatMemberByIndex>(Marshal.ReadIntPtr(this.VTable, 456));
			this._GetChatMemberByIndex_Windows = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetChatMemberByIndex_Windows>(Marshal.ReadIntPtr(this.VTable, 456));
			this._SendClanChatMessage = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FSendClanChatMessage>(Marshal.ReadIntPtr(this.VTable, 464));
			this._GetClanChatMessage = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetClanChatMessage>(Marshal.ReadIntPtr(this.VTable, 472));
			this._IsClanChatAdmin = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FIsClanChatAdmin>(Marshal.ReadIntPtr(this.VTable, 480));
			this._IsClanChatWindowOpenInSteam = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FIsClanChatWindowOpenInSteam>(Marshal.ReadIntPtr(this.VTable, 488));
			this._OpenClanChatWindowInSteam = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FOpenClanChatWindowInSteam>(Marshal.ReadIntPtr(this.VTable, 496));
			this._CloseClanChatWindowInSteam = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FCloseClanChatWindowInSteam>(Marshal.ReadIntPtr(this.VTable, 504));
			this._SetListenForFriendsMessages = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FSetListenForFriendsMessages>(Marshal.ReadIntPtr(this.VTable, 512));
			this._ReplyToFriendMessage = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FReplyToFriendMessage>(Marshal.ReadIntPtr(this.VTable, 520));
			this._GetFriendMessage = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFriendMessage>(Marshal.ReadIntPtr(this.VTable, 528));
			this._GetFollowerCount = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetFollowerCount>(Marshal.ReadIntPtr(this.VTable, 536));
			this._IsFollowing = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FIsFollowing>(Marshal.ReadIntPtr(this.VTable, 544));
			this._EnumerateFollowingList = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FEnumerateFollowingList>(Marshal.ReadIntPtr(this.VTable, 552));
			this._IsClanPublic = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FIsClanPublic>(Marshal.ReadIntPtr(this.VTable, 560));
			this._IsClanOfficialGameGroup = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FIsClanOfficialGameGroup>(Marshal.ReadIntPtr(this.VTable, 568));
			this._GetNumChatsWithUnreadPriorityMessages = Marshal.GetDelegateForFunctionPointer<ISteamFriends.FGetNumChatsWithUnreadPriorityMessages>(Marshal.ReadIntPtr(this.VTable, 576));
		}

		internal bool InviteUserToGame(SteamId steamIDFriend, string pchConnectString)
		{
			return this._InviteUserToGame(this.Self, steamIDFriend, pchConnectString);
		}

		internal bool IsClanChatAdmin(SteamId steamIDClanChat, SteamId steamIDUser)
		{
			return this._IsClanChatAdmin(this.Self, steamIDClanChat, steamIDUser);
		}

		internal bool IsClanChatWindowOpenInSteam(SteamId steamIDClanChat)
		{
			return this._IsClanChatWindowOpenInSteam(this.Self, steamIDClanChat);
		}

		internal bool IsClanOfficialGameGroup(SteamId steamIDClan)
		{
			return this._IsClanOfficialGameGroup(this.Self, steamIDClan);
		}

		internal bool IsClanPublic(SteamId steamIDClan)
		{
			return this._IsClanPublic(this.Self, steamIDClan);
		}

		internal async Task<FriendsIsFollowing_t?> IsFollowing(SteamId steamID)
		{
			FriendsIsFollowing_t? resultAsync = await FriendsIsFollowing_t.GetResultAsync(this._IsFollowing(this.Self, steamID));
			return resultAsync;
		}

		internal bool IsUserInSource(SteamId steamIDUser, SteamId steamIDSource)
		{
			return this._IsUserInSource(this.Self, steamIDUser, steamIDSource);
		}

		internal async Task<JoinClanChatRoomCompletionResult_t?> JoinClanChatRoom(SteamId steamIDClan)
		{
			JoinClanChatRoomCompletionResult_t? resultAsync = await JoinClanChatRoomCompletionResult_t.GetResultAsync(this._JoinClanChatRoom(this.Self, steamIDClan));
			return resultAsync;
		}

		internal bool LeaveClanChatRoom(SteamId steamIDClan)
		{
			return this._LeaveClanChatRoom(this.Self, steamIDClan);
		}

		internal bool OpenClanChatWindowInSteam(SteamId steamIDClanChat)
		{
			return this._OpenClanChatWindowInSteam(this.Self, steamIDClanChat);
		}

		internal bool ReplyToFriendMessage(SteamId steamIDFriend, string pchMsgToSend)
		{
			return this._ReplyToFriendMessage(this.Self, steamIDFriend, pchMsgToSend);
		}

		internal async Task<ClanOfficerListResponse_t?> RequestClanOfficerList(SteamId steamIDClan)
		{
			ClanOfficerListResponse_t? resultAsync = await ClanOfficerListResponse_t.GetResultAsync(this._RequestClanOfficerList(this.Self, steamIDClan));
			return resultAsync;
		}

		internal void RequestFriendRichPresence(SteamId steamIDFriend)
		{
			this._RequestFriendRichPresence(this.Self, steamIDFriend);
		}

		internal bool RequestUserInformation(SteamId steamIDUser, bool bRequireNameOnly)
		{
			return this._RequestUserInformation(this.Self, steamIDUser, bRequireNameOnly);
		}

		internal bool SendClanChatMessage(SteamId steamIDClanChat, string pchText)
		{
			return this._SendClanChatMessage(this.Self, steamIDClanChat, pchText);
		}

		internal void SetInGameVoiceSpeaking(SteamId steamIDUser, bool bSpeaking)
		{
			this._SetInGameVoiceSpeaking(this.Self, steamIDUser, bSpeaking);
		}

		internal bool SetListenForFriendsMessages(bool bInterceptEnabled)
		{
			return this._SetListenForFriendsMessages(this.Self, bInterceptEnabled);
		}

		internal async Task<SetPersonaNameResponse_t?> SetPersonaName(string pchPersonaName)
		{
			SetPersonaNameResponse_t? resultAsync = await SetPersonaNameResponse_t.GetResultAsync(this._SetPersonaName(this.Self, pchPersonaName));
			return resultAsync;
		}

		internal void SetPlayedWith(SteamId steamIDUserPlayedWith)
		{
			this._SetPlayedWith(this.Self, steamIDUserPlayedWith);
		}

		internal bool SetRichPresence(string pchKey, string pchValue)
		{
			return this._SetRichPresence(this.Self, pchKey, pchValue);
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._GetPersonaName = null;
			this._SetPersonaName = null;
			this._GetPersonaState = null;
			this._GetFriendCount = null;
			this._GetFriendByIndex = null;
			this._GetFriendByIndex_Windows = null;
			this._GetFriendRelationship = null;
			this._GetFriendPersonaState = null;
			this._GetFriendPersonaName = null;
			this._GetFriendGamePlayed = null;
			this._GetFriendPersonaNameHistory = null;
			this._GetFriendSteamLevel = null;
			this._GetPlayerNickname = null;
			this._GetFriendsGroupCount = null;
			this._GetFriendsGroupIDByIndex = null;
			this._GetFriendsGroupName = null;
			this._GetFriendsGroupMembersCount = null;
			this._GetFriendsGroupMembersList = null;
			this._HasFriend = null;
			this._GetClanCount = null;
			this._GetClanByIndex = null;
			this._GetClanByIndex_Windows = null;
			this._GetClanName = null;
			this._GetClanTag = null;
			this._GetClanActivityCounts = null;
			this._DownloadClanActivityCounts = null;
			this._GetFriendCountFromSource = null;
			this._GetFriendFromSourceByIndex = null;
			this._GetFriendFromSourceByIndex_Windows = null;
			this._IsUserInSource = null;
			this._SetInGameVoiceSpeaking = null;
			this._ActivateGameOverlay = null;
			this._ActivateGameOverlayToUser = null;
			this._ActivateGameOverlayToWebPage = null;
			this._ActivateGameOverlayToStore = null;
			this._SetPlayedWith = null;
			this._ActivateGameOverlayInviteDialog = null;
			this._GetSmallFriendAvatar = null;
			this._GetMediumFriendAvatar = null;
			this._GetLargeFriendAvatar = null;
			this._RequestUserInformation = null;
			this._RequestClanOfficerList = null;
			this._GetClanOwner = null;
			this._GetClanOwner_Windows = null;
			this._GetClanOfficerCount = null;
			this._GetClanOfficerByIndex = null;
			this._GetClanOfficerByIndex_Windows = null;
			this._GetUserRestrictions = null;
			this._SetRichPresence = null;
			this._ClearRichPresence = null;
			this._GetFriendRichPresence = null;
			this._GetFriendRichPresenceKeyCount = null;
			this._GetFriendRichPresenceKeyByIndex = null;
			this._RequestFriendRichPresence = null;
			this._InviteUserToGame = null;
			this._GetCoplayFriendCount = null;
			this._GetCoplayFriend = null;
			this._GetCoplayFriend_Windows = null;
			this._GetFriendCoplayTime = null;
			this._GetFriendCoplayGame = null;
			this._JoinClanChatRoom = null;
			this._LeaveClanChatRoom = null;
			this._GetClanChatMemberCount = null;
			this._GetChatMemberByIndex = null;
			this._GetChatMemberByIndex_Windows = null;
			this._SendClanChatMessage = null;
			this._GetClanChatMessage = null;
			this._IsClanChatAdmin = null;
			this._IsClanChatWindowOpenInSteam = null;
			this._OpenClanChatWindowInSteam = null;
			this._CloseClanChatWindowInSteam = null;
			this._SetListenForFriendsMessages = null;
			this._ReplyToFriendMessage = null;
			this._GetFriendMessage = null;
			this._GetFollowerCount = null;
			this._IsFollowing = null;
			this._EnumerateFollowingList = null;
			this._IsClanPublic = null;
			this._IsClanOfficialGameGroup = null;
			this._GetNumChatsWithUnreadPriorityMessages = null;
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FActivateGameOverlay(IntPtr self, string pchDialog);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FActivateGameOverlayInviteDialog(IntPtr self, SteamId steamIDLobby);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FActivateGameOverlayToStore(IntPtr self, AppId nAppID, OverlayToStoreFlag eFlag);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FActivateGameOverlayToUser(IntPtr self, string pchDialog, SteamId steamID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FActivateGameOverlayToWebPage(IntPtr self, string pchURL, ActivateGameOverlayToWebPageMode eMode);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FClearRichPresence(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FCloseClanChatWindowInSteam(IntPtr self, SteamId steamIDClanChat);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FDownloadClanActivityCounts(IntPtr self, [In][Out] SteamId[] psteamIDClans, int cClansToRequest);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FEnumerateFollowingList(IntPtr self, uint unStartIndex);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamId FGetChatMemberByIndex(IntPtr self, SteamId steamIDClan, int iUser);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FGetChatMemberByIndex_Windows(IntPtr self, ref SteamId retVal, SteamId steamIDClan, int iUser);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetClanActivityCounts(IntPtr self, SteamId steamIDClan, ref int pnOnline, ref int pnInGame, ref int pnChatting);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamId FGetClanByIndex(IntPtr self, int iClan);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FGetClanByIndex_Windows(IntPtr self, ref SteamId retVal, int iClan);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetClanChatMemberCount(IntPtr self, SteamId steamIDClan);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetClanChatMessage(IntPtr self, SteamId steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, ref ChatEntryType peChatEntryType, ref SteamId psteamidChatter);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetClanCount(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetClanName(IntPtr self, SteamId steamIDClan);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamId FGetClanOfficerByIndex(IntPtr self, SteamId steamIDClan, int iOfficer);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FGetClanOfficerByIndex_Windows(IntPtr self, ref SteamId retVal, SteamId steamIDClan, int iOfficer);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetClanOfficerCount(IntPtr self, SteamId steamIDClan);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamId FGetClanOwner(IntPtr self, SteamId steamIDClan);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FGetClanOwner_Windows(IntPtr self, ref SteamId retVal, SteamId steamIDClan);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetClanTag(IntPtr self, SteamId steamIDClan);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamId FGetCoplayFriend(IntPtr self, int iCoplayFriend);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FGetCoplayFriend_Windows(IntPtr self, ref SteamId retVal, int iCoplayFriend);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetCoplayFriendCount(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FGetFollowerCount(IntPtr self, SteamId steamID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamId FGetFriendByIndex(IntPtr self, int iFriend, int iFriendFlags);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FGetFriendByIndex_Windows(IntPtr self, ref SteamId retVal, int iFriend, int iFriendFlags);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate AppId FGetFriendCoplayGame(IntPtr self, SteamId steamIDFriend);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetFriendCoplayTime(IntPtr self, SteamId steamIDFriend);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetFriendCount(IntPtr self, int iFriendFlags);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetFriendCountFromSource(IntPtr self, SteamId steamIDSource);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamId FGetFriendFromSourceByIndex(IntPtr self, SteamId steamIDSource, int iFriend);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FGetFriendFromSourceByIndex_Windows(IntPtr self, ref SteamId retVal, SteamId steamIDSource, int iFriend);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetFriendGamePlayed(IntPtr self, SteamId steamIDFriend, ref FriendGameInfo_t pFriendGameInfo);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetFriendMessage(IntPtr self, SteamId steamIDFriend, int iMessageID, IntPtr pvData, int cubData, ref ChatEntryType peChatEntryType);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetFriendPersonaName(IntPtr self, SteamId steamIDFriend);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetFriendPersonaNameHistory(IntPtr self, SteamId steamIDFriend, int iPersonaName);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate FriendState FGetFriendPersonaState(IntPtr self, SteamId steamIDFriend);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate Relationship FGetFriendRelationship(IntPtr self, SteamId steamIDFriend);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetFriendRichPresence(IntPtr self, SteamId steamIDFriend, string pchKey);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetFriendRichPresenceKeyByIndex(IntPtr self, SteamId steamIDFriend, int iKey);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetFriendRichPresenceKeyCount(IntPtr self, SteamId steamIDFriend);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetFriendsGroupCount(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate FriendsGroupID_t FGetFriendsGroupIDByIndex(IntPtr self, int iFG);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetFriendsGroupMembersCount(IntPtr self, FriendsGroupID_t friendsGroupID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FGetFriendsGroupMembersList(IntPtr self, FriendsGroupID_t friendsGroupID, [In][Out] SteamId[] pOutSteamIDMembers, int nMembersCount);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetFriendsGroupName(IntPtr self, FriendsGroupID_t friendsGroupID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetFriendSteamLevel(IntPtr self, SteamId steamIDFriend);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetLargeFriendAvatar(IntPtr self, SteamId steamIDFriend);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetMediumFriendAvatar(IntPtr self, SteamId steamIDFriend);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetNumChatsWithUnreadPriorityMessages(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetPersonaName(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate FriendState FGetPersonaState(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetPlayerNickname(IntPtr self, SteamId steamIDPlayer);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetSmallFriendAvatar(IntPtr self, SteamId steamIDFriend);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetUserRestrictions(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FHasFriend(IntPtr self, SteamId steamIDFriend, int iFriendFlags);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FInviteUserToGame(IntPtr self, SteamId steamIDFriend, string pchConnectString);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIsClanChatAdmin(IntPtr self, SteamId steamIDClanChat, SteamId steamIDUser);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIsClanChatWindowOpenInSteam(IntPtr self, SteamId steamIDClanChat);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIsClanOfficialGameGroup(IntPtr self, SteamId steamIDClan);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIsClanPublic(IntPtr self, SteamId steamIDClan);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FIsFollowing(IntPtr self, SteamId steamID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIsUserInSource(IntPtr self, SteamId steamIDUser, SteamId steamIDSource);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FJoinClanChatRoom(IntPtr self, SteamId steamIDClan);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FLeaveClanChatRoom(IntPtr self, SteamId steamIDClan);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FOpenClanChatWindowInSteam(IntPtr self, SteamId steamIDClanChat);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FReplyToFriendMessage(IntPtr self, SteamId steamIDFriend, string pchMsgToSend);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FRequestClanOfficerList(IntPtr self, SteamId steamIDClan);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FRequestFriendRichPresence(IntPtr self, SteamId steamIDFriend);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FRequestUserInformation(IntPtr self, SteamId steamIDUser, bool bRequireNameOnly);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSendClanChatMessage(IntPtr self, SteamId steamIDClanChat, string pchText);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetInGameVoiceSpeaking(IntPtr self, SteamId steamIDUser, bool bSpeaking);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetListenForFriendsMessages(IntPtr self, bool bInterceptEnabled);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FSetPersonaName(IntPtr self, string pchPersonaName);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetPlayedWith(IntPtr self, SteamId steamIDUserPlayedWith);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetRichPresence(IntPtr self, string pchKey, string pchValue);
	}
}