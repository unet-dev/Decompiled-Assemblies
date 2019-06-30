using Steamworks.Data;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks
{
	internal class ISteamMatchmaking : SteamInterface
	{
		private ISteamMatchmaking.FGetFavoriteGameCount _GetFavoriteGameCount;

		private ISteamMatchmaking.FGetFavoriteGame _GetFavoriteGame;

		private ISteamMatchmaking.FAddFavoriteGame _AddFavoriteGame;

		private ISteamMatchmaking.FRemoveFavoriteGame _RemoveFavoriteGame;

		private ISteamMatchmaking.FRequestLobbyList _RequestLobbyList;

		private ISteamMatchmaking.FAddRequestLobbyListStringFilter _AddRequestLobbyListStringFilter;

		private ISteamMatchmaking.FAddRequestLobbyListNumericalFilter _AddRequestLobbyListNumericalFilter;

		private ISteamMatchmaking.FAddRequestLobbyListNearValueFilter _AddRequestLobbyListNearValueFilter;

		private ISteamMatchmaking.FAddRequestLobbyListFilterSlotsAvailable _AddRequestLobbyListFilterSlotsAvailable;

		private ISteamMatchmaking.FAddRequestLobbyListDistanceFilter _AddRequestLobbyListDistanceFilter;

		private ISteamMatchmaking.FAddRequestLobbyListResultCountFilter _AddRequestLobbyListResultCountFilter;

		private ISteamMatchmaking.FAddRequestLobbyListCompatibleMembersFilter _AddRequestLobbyListCompatibleMembersFilter;

		private ISteamMatchmaking.FGetLobbyByIndex _GetLobbyByIndex;

		private ISteamMatchmaking.FGetLobbyByIndex_Windows _GetLobbyByIndex_Windows;

		private ISteamMatchmaking.FCreateLobby _CreateLobby;

		private ISteamMatchmaking.FJoinLobby _JoinLobby;

		private ISteamMatchmaking.FLeaveLobby _LeaveLobby;

		private ISteamMatchmaking.FInviteUserToLobby _InviteUserToLobby;

		private ISteamMatchmaking.FGetNumLobbyMembers _GetNumLobbyMembers;

		private ISteamMatchmaking.FGetLobbyMemberByIndex _GetLobbyMemberByIndex;

		private ISteamMatchmaking.FGetLobbyMemberByIndex_Windows _GetLobbyMemberByIndex_Windows;

		private ISteamMatchmaking.FGetLobbyData _GetLobbyData;

		private ISteamMatchmaking.FSetLobbyData _SetLobbyData;

		private ISteamMatchmaking.FGetLobbyDataCount _GetLobbyDataCount;

		private ISteamMatchmaking.FGetLobbyDataByIndex _GetLobbyDataByIndex;

		private ISteamMatchmaking.FDeleteLobbyData _DeleteLobbyData;

		private ISteamMatchmaking.FGetLobbyMemberData _GetLobbyMemberData;

		private ISteamMatchmaking.FSetLobbyMemberData _SetLobbyMemberData;

		private ISteamMatchmaking.FSendLobbyChatMsg _SendLobbyChatMsg;

		private ISteamMatchmaking.FGetLobbyChatEntry _GetLobbyChatEntry;

		private ISteamMatchmaking.FRequestLobbyData _RequestLobbyData;

		private ISteamMatchmaking.FSetLobbyGameServer _SetLobbyGameServer;

		private ISteamMatchmaking.FGetLobbyGameServer _GetLobbyGameServer;

		private ISteamMatchmaking.FSetLobbyMemberLimit _SetLobbyMemberLimit;

		private ISteamMatchmaking.FGetLobbyMemberLimit _GetLobbyMemberLimit;

		private ISteamMatchmaking.FSetLobbyType _SetLobbyType;

		private ISteamMatchmaking.FSetLobbyJoinable _SetLobbyJoinable;

		private ISteamMatchmaking.FGetLobbyOwner _GetLobbyOwner;

		private ISteamMatchmaking.FGetLobbyOwner_Windows _GetLobbyOwner_Windows;

		private ISteamMatchmaking.FSetLobbyOwner _SetLobbyOwner;

		private ISteamMatchmaking.FSetLinkedLobby _SetLinkedLobby;

		public override string InterfaceName
		{
			get
			{
				return "SteamMatchMaking009";
			}
		}

		public ISteamMatchmaking()
		{
		}

		internal int AddFavoriteGame(AppId nAppID, uint nIP, ushort nConnPort, ushort nQueryPort, uint unFlags, uint rTime32LastPlayedOnServer)
		{
			int self = this._AddFavoriteGame(this.Self, nAppID, nIP, nConnPort, nQueryPort, unFlags, rTime32LastPlayedOnServer);
			return self;
		}

		internal void AddRequestLobbyListCompatibleMembersFilter(SteamId steamIDLobby)
		{
			this._AddRequestLobbyListCompatibleMembersFilter(this.Self, steamIDLobby);
		}

		internal void AddRequestLobbyListDistanceFilter(LobbyDistanceFilter eLobbyDistanceFilter)
		{
			this._AddRequestLobbyListDistanceFilter(this.Self, eLobbyDistanceFilter);
		}

		internal void AddRequestLobbyListFilterSlotsAvailable(int nSlotsAvailable)
		{
			this._AddRequestLobbyListFilterSlotsAvailable(this.Self, nSlotsAvailable);
		}

		internal void AddRequestLobbyListNearValueFilter(string pchKeyToMatch, int nValueToBeCloseTo)
		{
			this._AddRequestLobbyListNearValueFilter(this.Self, pchKeyToMatch, nValueToBeCloseTo);
		}

		internal void AddRequestLobbyListNumericalFilter(string pchKeyToMatch, int nValueToMatch, LobbyComparison eComparisonType)
		{
			this._AddRequestLobbyListNumericalFilter(this.Self, pchKeyToMatch, nValueToMatch, eComparisonType);
		}

		internal void AddRequestLobbyListResultCountFilter(int cMaxResults)
		{
			this._AddRequestLobbyListResultCountFilter(this.Self, cMaxResults);
		}

		internal void AddRequestLobbyListStringFilter(string pchKeyToMatch, string pchValueToMatch, LobbyComparison eComparisonType)
		{
			this._AddRequestLobbyListStringFilter(this.Self, pchKeyToMatch, pchValueToMatch, eComparisonType);
		}

		internal async Task<LobbyCreated_t?> CreateLobby(LobbyType eLobbyType, int cMaxMembers)
		{
			LobbyCreated_t? resultAsync = await LobbyCreated_t.GetResultAsync(this._CreateLobby(this.Self, eLobbyType, cMaxMembers));
			return resultAsync;
		}

		internal bool DeleteLobbyData(SteamId steamIDLobby, string pchKey)
		{
			return this._DeleteLobbyData(this.Self, steamIDLobby, pchKey);
		}

		internal bool GetFavoriteGame(int iGame, ref AppId pnAppID, ref uint pnIP, ref ushort pnConnPort, ref ushort pnQueryPort, ref uint punFlags, ref uint pRTime32LastPlayedOnServer)
		{
			bool self = this._GetFavoriteGame(this.Self, iGame, ref pnAppID, ref pnIP, ref pnConnPort, ref pnQueryPort, ref punFlags, ref pRTime32LastPlayedOnServer);
			return self;
		}

		internal int GetFavoriteGameCount()
		{
			return this._GetFavoriteGameCount(this.Self);
		}

		internal SteamId GetLobbyByIndex(int iLobby)
		{
			SteamId self;
			if (Config.Os != OsType.Windows)
			{
				self = this._GetLobbyByIndex(this.Self, iLobby);
			}
			else
			{
				SteamId steamId = new SteamId();
				this._GetLobbyByIndex_Windows(this.Self, ref steamId, iLobby);
				self = steamId;
			}
			return self;
		}

		internal int GetLobbyChatEntry(SteamId steamIDLobby, int iChatID, ref SteamId pSteamIDUser, IntPtr pvData, int cubData, ref ChatEntryType peChatEntryType)
		{
			int self = this._GetLobbyChatEntry(this.Self, steamIDLobby, iChatID, ref pSteamIDUser, pvData, cubData, ref peChatEntryType);
			return self;
		}

		internal string GetLobbyData(SteamId steamIDLobby, string pchKey)
		{
			string str = base.GetString(this._GetLobbyData(this.Self, steamIDLobby, pchKey));
			return str;
		}

		internal bool GetLobbyDataByIndex(SteamId steamIDLobby, int iLobbyData, StringBuilder pchKey, int cchKeyBufferSize, StringBuilder pchValue, int cchValueBufferSize)
		{
			bool self = this._GetLobbyDataByIndex(this.Self, steamIDLobby, iLobbyData, pchKey, cchKeyBufferSize, pchValue, cchValueBufferSize);
			return self;
		}

		internal int GetLobbyDataCount(SteamId steamIDLobby)
		{
			return this._GetLobbyDataCount(this.Self, steamIDLobby);
		}

		internal bool GetLobbyGameServer(SteamId steamIDLobby, ref uint punGameServerIP, ref ushort punGameServerPort, ref SteamId psteamIDGameServer)
		{
			bool self = this._GetLobbyGameServer(this.Self, steamIDLobby, ref punGameServerIP, ref punGameServerPort, ref psteamIDGameServer);
			return self;
		}

		internal SteamId GetLobbyMemberByIndex(SteamId steamIDLobby, int iMember)
		{
			SteamId self;
			if (Config.Os != OsType.Windows)
			{
				self = this._GetLobbyMemberByIndex(this.Self, steamIDLobby, iMember);
			}
			else
			{
				SteamId steamId = new SteamId();
				this._GetLobbyMemberByIndex_Windows(this.Self, ref steamId, steamIDLobby, iMember);
				self = steamId;
			}
			return self;
		}

		internal string GetLobbyMemberData(SteamId steamIDLobby, SteamId steamIDUser, string pchKey)
		{
			string str = base.GetString(this._GetLobbyMemberData(this.Self, steamIDLobby, steamIDUser, pchKey));
			return str;
		}

		internal int GetLobbyMemberLimit(SteamId steamIDLobby)
		{
			return this._GetLobbyMemberLimit(this.Self, steamIDLobby);
		}

		internal SteamId GetLobbyOwner(SteamId steamIDLobby)
		{
			SteamId self;
			if (Config.Os != OsType.Windows)
			{
				self = this._GetLobbyOwner(this.Self, steamIDLobby);
			}
			else
			{
				SteamId steamId = new SteamId();
				this._GetLobbyOwner_Windows(this.Self, ref steamId, steamIDLobby);
				self = steamId;
			}
			return self;
		}

		internal int GetNumLobbyMembers(SteamId steamIDLobby)
		{
			return this._GetNumLobbyMembers(this.Self, steamIDLobby);
		}

		public override void InitInternals()
		{
			this._GetFavoriteGameCount = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FGetFavoriteGameCount>(Marshal.ReadIntPtr(this.VTable, 0));
			this._GetFavoriteGame = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FGetFavoriteGame>(Marshal.ReadIntPtr(this.VTable, 8));
			this._AddFavoriteGame = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FAddFavoriteGame>(Marshal.ReadIntPtr(this.VTable, 16));
			this._RemoveFavoriteGame = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FRemoveFavoriteGame>(Marshal.ReadIntPtr(this.VTable, 24));
			this._RequestLobbyList = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FRequestLobbyList>(Marshal.ReadIntPtr(this.VTable, 32));
			this._AddRequestLobbyListStringFilter = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FAddRequestLobbyListStringFilter>(Marshal.ReadIntPtr(this.VTable, 40));
			this._AddRequestLobbyListNumericalFilter = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FAddRequestLobbyListNumericalFilter>(Marshal.ReadIntPtr(this.VTable, 48));
			this._AddRequestLobbyListNearValueFilter = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FAddRequestLobbyListNearValueFilter>(Marshal.ReadIntPtr(this.VTable, 56));
			this._AddRequestLobbyListFilterSlotsAvailable = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FAddRequestLobbyListFilterSlotsAvailable>(Marshal.ReadIntPtr(this.VTable, 64));
			this._AddRequestLobbyListDistanceFilter = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FAddRequestLobbyListDistanceFilter>(Marshal.ReadIntPtr(this.VTable, 72));
			this._AddRequestLobbyListResultCountFilter = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FAddRequestLobbyListResultCountFilter>(Marshal.ReadIntPtr(this.VTable, 80));
			this._AddRequestLobbyListCompatibleMembersFilter = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FAddRequestLobbyListCompatibleMembersFilter>(Marshal.ReadIntPtr(this.VTable, 88));
			this._GetLobbyByIndex = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FGetLobbyByIndex>(Marshal.ReadIntPtr(this.VTable, 96));
			this._GetLobbyByIndex_Windows = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FGetLobbyByIndex_Windows>(Marshal.ReadIntPtr(this.VTable, 96));
			this._CreateLobby = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FCreateLobby>(Marshal.ReadIntPtr(this.VTable, 104));
			this._JoinLobby = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FJoinLobby>(Marshal.ReadIntPtr(this.VTable, 112));
			this._LeaveLobby = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FLeaveLobby>(Marshal.ReadIntPtr(this.VTable, 120));
			this._InviteUserToLobby = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FInviteUserToLobby>(Marshal.ReadIntPtr(this.VTable, 128));
			this._GetNumLobbyMembers = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FGetNumLobbyMembers>(Marshal.ReadIntPtr(this.VTable, 136));
			this._GetLobbyMemberByIndex = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FGetLobbyMemberByIndex>(Marshal.ReadIntPtr(this.VTable, 144));
			this._GetLobbyMemberByIndex_Windows = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FGetLobbyMemberByIndex_Windows>(Marshal.ReadIntPtr(this.VTable, 144));
			this._GetLobbyData = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FGetLobbyData>(Marshal.ReadIntPtr(this.VTable, 152));
			this._SetLobbyData = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FSetLobbyData>(Marshal.ReadIntPtr(this.VTable, 160));
			this._GetLobbyDataCount = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FGetLobbyDataCount>(Marshal.ReadIntPtr(this.VTable, 168));
			this._GetLobbyDataByIndex = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FGetLobbyDataByIndex>(Marshal.ReadIntPtr(this.VTable, 176));
			this._DeleteLobbyData = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FDeleteLobbyData>(Marshal.ReadIntPtr(this.VTable, 184));
			this._GetLobbyMemberData = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FGetLobbyMemberData>(Marshal.ReadIntPtr(this.VTable, 192));
			this._SetLobbyMemberData = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FSetLobbyMemberData>(Marshal.ReadIntPtr(this.VTable, 200));
			this._SendLobbyChatMsg = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FSendLobbyChatMsg>(Marshal.ReadIntPtr(this.VTable, 208));
			this._GetLobbyChatEntry = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FGetLobbyChatEntry>(Marshal.ReadIntPtr(this.VTable, 216));
			this._RequestLobbyData = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FRequestLobbyData>(Marshal.ReadIntPtr(this.VTable, 224));
			this._SetLobbyGameServer = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FSetLobbyGameServer>(Marshal.ReadIntPtr(this.VTable, 232));
			this._GetLobbyGameServer = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FGetLobbyGameServer>(Marshal.ReadIntPtr(this.VTable, 240));
			this._SetLobbyMemberLimit = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FSetLobbyMemberLimit>(Marshal.ReadIntPtr(this.VTable, 248));
			this._GetLobbyMemberLimit = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FGetLobbyMemberLimit>(Marshal.ReadIntPtr(this.VTable, 256));
			this._SetLobbyType = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FSetLobbyType>(Marshal.ReadIntPtr(this.VTable, 264));
			this._SetLobbyJoinable = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FSetLobbyJoinable>(Marshal.ReadIntPtr(this.VTable, 272));
			this._GetLobbyOwner = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FGetLobbyOwner>(Marshal.ReadIntPtr(this.VTable, 280));
			this._GetLobbyOwner_Windows = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FGetLobbyOwner_Windows>(Marshal.ReadIntPtr(this.VTable, 280));
			this._SetLobbyOwner = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FSetLobbyOwner>(Marshal.ReadIntPtr(this.VTable, 288));
			this._SetLinkedLobby = Marshal.GetDelegateForFunctionPointer<ISteamMatchmaking.FSetLinkedLobby>(Marshal.ReadIntPtr(this.VTable, 296));
		}

		internal bool InviteUserToLobby(SteamId steamIDLobby, SteamId steamIDInvitee)
		{
			return this._InviteUserToLobby(this.Self, steamIDLobby, steamIDInvitee);
		}

		internal async Task<LobbyEnter_t?> JoinLobby(SteamId steamIDLobby)
		{
			LobbyEnter_t? resultAsync = await LobbyEnter_t.GetResultAsync(this._JoinLobby(this.Self, steamIDLobby));
			return resultAsync;
		}

		internal void LeaveLobby(SteamId steamIDLobby)
		{
			this._LeaveLobby(this.Self, steamIDLobby);
		}

		internal bool RemoveFavoriteGame(AppId nAppID, uint nIP, ushort nConnPort, ushort nQueryPort, uint unFlags)
		{
			bool self = this._RemoveFavoriteGame(this.Self, nAppID, nIP, nConnPort, nQueryPort, unFlags);
			return self;
		}

		internal bool RequestLobbyData(SteamId steamIDLobby)
		{
			return this._RequestLobbyData(this.Self, steamIDLobby);
		}

		internal async Task<LobbyMatchList_t?> RequestLobbyList()
		{
			LobbyMatchList_t? resultAsync = await LobbyMatchList_t.GetResultAsync(this._RequestLobbyList(this.Self));
			return resultAsync;
		}

		internal bool SendLobbyChatMsg(SteamId steamIDLobby, IntPtr pvMsgBody, int cubMsgBody)
		{
			return this._SendLobbyChatMsg(this.Self, steamIDLobby, pvMsgBody, cubMsgBody);
		}

		internal bool SetLinkedLobby(SteamId steamIDLobby, SteamId steamIDLobbyDependent)
		{
			return this._SetLinkedLobby(this.Self, steamIDLobby, steamIDLobbyDependent);
		}

		internal bool SetLobbyData(SteamId steamIDLobby, string pchKey, string pchValue)
		{
			return this._SetLobbyData(this.Self, steamIDLobby, pchKey, pchValue);
		}

		internal void SetLobbyGameServer(SteamId steamIDLobby, uint unGameServerIP, ushort unGameServerPort, SteamId steamIDGameServer)
		{
			this._SetLobbyGameServer(this.Self, steamIDLobby, unGameServerIP, unGameServerPort, steamIDGameServer);
		}

		internal bool SetLobbyJoinable(SteamId steamIDLobby, bool bLobbyJoinable)
		{
			return this._SetLobbyJoinable(this.Self, steamIDLobby, bLobbyJoinable);
		}

		internal void SetLobbyMemberData(SteamId steamIDLobby, string pchKey, string pchValue)
		{
			this._SetLobbyMemberData(this.Self, steamIDLobby, pchKey, pchValue);
		}

		internal bool SetLobbyMemberLimit(SteamId steamIDLobby, int cMaxMembers)
		{
			return this._SetLobbyMemberLimit(this.Self, steamIDLobby, cMaxMembers);
		}

		internal bool SetLobbyOwner(SteamId steamIDLobby, SteamId steamIDNewOwner)
		{
			return this._SetLobbyOwner(this.Self, steamIDLobby, steamIDNewOwner);
		}

		internal bool SetLobbyType(SteamId steamIDLobby, LobbyType eLobbyType)
		{
			return this._SetLobbyType(this.Self, steamIDLobby, eLobbyType);
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._GetFavoriteGameCount = null;
			this._GetFavoriteGame = null;
			this._AddFavoriteGame = null;
			this._RemoveFavoriteGame = null;
			this._RequestLobbyList = null;
			this._AddRequestLobbyListStringFilter = null;
			this._AddRequestLobbyListNumericalFilter = null;
			this._AddRequestLobbyListNearValueFilter = null;
			this._AddRequestLobbyListFilterSlotsAvailable = null;
			this._AddRequestLobbyListDistanceFilter = null;
			this._AddRequestLobbyListResultCountFilter = null;
			this._AddRequestLobbyListCompatibleMembersFilter = null;
			this._GetLobbyByIndex = null;
			this._GetLobbyByIndex_Windows = null;
			this._CreateLobby = null;
			this._JoinLobby = null;
			this._LeaveLobby = null;
			this._InviteUserToLobby = null;
			this._GetNumLobbyMembers = null;
			this._GetLobbyMemberByIndex = null;
			this._GetLobbyMemberByIndex_Windows = null;
			this._GetLobbyData = null;
			this._SetLobbyData = null;
			this._GetLobbyDataCount = null;
			this._GetLobbyDataByIndex = null;
			this._DeleteLobbyData = null;
			this._GetLobbyMemberData = null;
			this._SetLobbyMemberData = null;
			this._SendLobbyChatMsg = null;
			this._GetLobbyChatEntry = null;
			this._RequestLobbyData = null;
			this._SetLobbyGameServer = null;
			this._GetLobbyGameServer = null;
			this._SetLobbyMemberLimit = null;
			this._GetLobbyMemberLimit = null;
			this._SetLobbyType = null;
			this._SetLobbyJoinable = null;
			this._GetLobbyOwner = null;
			this._GetLobbyOwner_Windows = null;
			this._SetLobbyOwner = null;
			this._SetLinkedLobby = null;
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FAddFavoriteGame(IntPtr self, AppId nAppID, uint nIP, ushort nConnPort, ushort nQueryPort, uint unFlags, uint rTime32LastPlayedOnServer);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FAddRequestLobbyListCompatibleMembersFilter(IntPtr self, SteamId steamIDLobby);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FAddRequestLobbyListDistanceFilter(IntPtr self, LobbyDistanceFilter eLobbyDistanceFilter);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FAddRequestLobbyListFilterSlotsAvailable(IntPtr self, int nSlotsAvailable);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FAddRequestLobbyListNearValueFilter(IntPtr self, string pchKeyToMatch, int nValueToBeCloseTo);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FAddRequestLobbyListNumericalFilter(IntPtr self, string pchKeyToMatch, int nValueToMatch, LobbyComparison eComparisonType);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FAddRequestLobbyListResultCountFilter(IntPtr self, int cMaxResults);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FAddRequestLobbyListStringFilter(IntPtr self, string pchKeyToMatch, string pchValueToMatch, LobbyComparison eComparisonType);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FCreateLobby(IntPtr self, LobbyType eLobbyType, int cMaxMembers);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FDeleteLobbyData(IntPtr self, SteamId steamIDLobby, string pchKey);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetFavoriteGame(IntPtr self, int iGame, ref AppId pnAppID, ref uint pnIP, ref ushort pnConnPort, ref ushort pnQueryPort, ref uint punFlags, ref uint pRTime32LastPlayedOnServer);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetFavoriteGameCount(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamId FGetLobbyByIndex(IntPtr self, int iLobby);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FGetLobbyByIndex_Windows(IntPtr self, ref SteamId retVal, int iLobby);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetLobbyChatEntry(IntPtr self, SteamId steamIDLobby, int iChatID, ref SteamId pSteamIDUser, IntPtr pvData, int cubData, ref ChatEntryType peChatEntryType);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetLobbyData(IntPtr self, SteamId steamIDLobby, string pchKey);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetLobbyDataByIndex(IntPtr self, SteamId steamIDLobby, int iLobbyData, StringBuilder pchKey, int cchKeyBufferSize, StringBuilder pchValue, int cchValueBufferSize);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetLobbyDataCount(IntPtr self, SteamId steamIDLobby);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetLobbyGameServer(IntPtr self, SteamId steamIDLobby, ref uint punGameServerIP, ref ushort punGameServerPort, ref SteamId psteamIDGameServer);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamId FGetLobbyMemberByIndex(IntPtr self, SteamId steamIDLobby, int iMember);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FGetLobbyMemberByIndex_Windows(IntPtr self, ref SteamId retVal, SteamId steamIDLobby, int iMember);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetLobbyMemberData(IntPtr self, SteamId steamIDLobby, SteamId steamIDUser, string pchKey);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetLobbyMemberLimit(IntPtr self, SteamId steamIDLobby);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamId FGetLobbyOwner(IntPtr self, SteamId steamIDLobby);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FGetLobbyOwner_Windows(IntPtr self, ref SteamId retVal, SteamId steamIDLobby);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetNumLobbyMembers(IntPtr self, SteamId steamIDLobby);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FInviteUserToLobby(IntPtr self, SteamId steamIDLobby, SteamId steamIDInvitee);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FJoinLobby(IntPtr self, SteamId steamIDLobby);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FLeaveLobby(IntPtr self, SteamId steamIDLobby);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FRemoveFavoriteGame(IntPtr self, AppId nAppID, uint nIP, ushort nConnPort, ushort nQueryPort, uint unFlags);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FRequestLobbyData(IntPtr self, SteamId steamIDLobby);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FRequestLobbyList(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSendLobbyChatMsg(IntPtr self, SteamId steamIDLobby, IntPtr pvMsgBody, int cubMsgBody);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetLinkedLobby(IntPtr self, SteamId steamIDLobby, SteamId steamIDLobbyDependent);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetLobbyData(IntPtr self, SteamId steamIDLobby, string pchKey, string pchValue);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetLobbyGameServer(IntPtr self, SteamId steamIDLobby, uint unGameServerIP, ushort unGameServerPort, SteamId steamIDGameServer);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetLobbyJoinable(IntPtr self, SteamId steamIDLobby, bool bLobbyJoinable);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetLobbyMemberData(IntPtr self, SteamId steamIDLobby, string pchKey, string pchValue);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetLobbyMemberLimit(IntPtr self, SteamId steamIDLobby, int cMaxMembers);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetLobbyOwner(IntPtr self, SteamId steamIDLobby, SteamId steamIDNewOwner);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetLobbyType(IntPtr self, SteamId steamIDLobby, LobbyType eLobbyType);
	}
}