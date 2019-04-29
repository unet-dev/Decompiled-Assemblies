using Facepunch.Steamworks;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SteamNative
{
	internal class SteamMatchmaking : IDisposable
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

		internal SteamMatchmaking(BaseSteamworks steamworks, IntPtr pointer)
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

		public int AddFavoriteGame(AppId_t nAppID, uint nIP, ushort nConnPort, ushort nQueryPort, uint unFlags, uint rTime32LastPlayedOnServer)
		{
			return this.platform.ISteamMatchmaking_AddFavoriteGame(nAppID.Value, nIP, nConnPort, nQueryPort, unFlags, rTime32LastPlayedOnServer);
		}

		public void AddRequestLobbyListCompatibleMembersFilter(CSteamID steamIDLobby)
		{
			this.platform.ISteamMatchmaking_AddRequestLobbyListCompatibleMembersFilter(steamIDLobby.Value);
		}

		public void AddRequestLobbyListDistanceFilter(LobbyDistanceFilter eLobbyDistanceFilter)
		{
			this.platform.ISteamMatchmaking_AddRequestLobbyListDistanceFilter(eLobbyDistanceFilter);
		}

		public void AddRequestLobbyListFilterSlotsAvailable(int nSlotsAvailable)
		{
			this.platform.ISteamMatchmaking_AddRequestLobbyListFilterSlotsAvailable(nSlotsAvailable);
		}

		public void AddRequestLobbyListNearValueFilter(string pchKeyToMatch, int nValueToBeCloseTo)
		{
			this.platform.ISteamMatchmaking_AddRequestLobbyListNearValueFilter(pchKeyToMatch, nValueToBeCloseTo);
		}

		public void AddRequestLobbyListNumericalFilter(string pchKeyToMatch, int nValueToMatch, LobbyComparison eComparisonType)
		{
			this.platform.ISteamMatchmaking_AddRequestLobbyListNumericalFilter(pchKeyToMatch, nValueToMatch, eComparisonType);
		}

		public void AddRequestLobbyListResultCountFilter(int cMaxResults)
		{
			this.platform.ISteamMatchmaking_AddRequestLobbyListResultCountFilter(cMaxResults);
		}

		public void AddRequestLobbyListStringFilter(string pchKeyToMatch, string pchValueToMatch, LobbyComparison eComparisonType)
		{
			this.platform.ISteamMatchmaking_AddRequestLobbyListStringFilter(pchKeyToMatch, pchValueToMatch, eComparisonType);
		}

		public CallbackHandle CreateLobby(LobbyType eLobbyType, int cMaxMembers, Action<LobbyCreated_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamMatchmaking_CreateLobby(eLobbyType, cMaxMembers);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return LobbyCreated_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool DeleteLobbyData(CSteamID steamIDLobby, string pchKey)
		{
			return this.platform.ISteamMatchmaking_DeleteLobbyData(steamIDLobby.Value, pchKey);
		}

		public virtual void Dispose()
		{
			if (this.platform != null)
			{
				this.platform.Dispose();
				this.platform = null;
			}
		}

		public bool GetFavoriteGame(int iGame, ref AppId_t pnAppID, out uint pnIP, out ushort pnConnPort, out ushort pnQueryPort, out uint punFlags, out uint pRTime32LastPlayedOnServer)
		{
			return this.platform.ISteamMatchmaking_GetFavoriteGame(iGame, ref pnAppID.Value, out pnIP, out pnConnPort, out pnQueryPort, out punFlags, out pRTime32LastPlayedOnServer);
		}

		public int GetFavoriteGameCount()
		{
			return this.platform.ISteamMatchmaking_GetFavoriteGameCount();
		}

		public ulong GetLobbyByIndex(int iLobby)
		{
			return this.platform.ISteamMatchmaking_GetLobbyByIndex(iLobby);
		}

		public int GetLobbyChatEntry(CSteamID steamIDLobby, int iChatID, out CSteamID pSteamIDUser, IntPtr pvData, int cubData, out ChatEntryType peChatEntryType)
		{
			pSteamIDUser = new CSteamID();
			return this.platform.ISteamMatchmaking_GetLobbyChatEntry(steamIDLobby.Value, iChatID, out pSteamIDUser.Value, pvData, cubData, out peChatEntryType);
		}

		public string GetLobbyData(CSteamID steamIDLobby, string pchKey)
		{
			return Marshal.PtrToStringAnsi(this.platform.ISteamMatchmaking_GetLobbyData(steamIDLobby.Value, pchKey));
		}

		public bool GetLobbyDataByIndex(CSteamID steamIDLobby, int iLobbyData, out string pchKey, out string pchValue)
		{
			bool flag = false;
			pchKey = string.Empty;
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			int num = 4096;
			pchValue = string.Empty;
			StringBuilder stringBuilder1 = Helpers.TakeStringBuilder();
			int num1 = 4096;
			flag = this.platform.ISteamMatchmaking_GetLobbyDataByIndex(steamIDLobby.Value, iLobbyData, stringBuilder, num, stringBuilder1, num1);
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

		public int GetLobbyDataCount(CSteamID steamIDLobby)
		{
			return this.platform.ISteamMatchmaking_GetLobbyDataCount(steamIDLobby.Value);
		}

		public bool GetLobbyGameServer(CSteamID steamIDLobby, out uint punGameServerIP, out ushort punGameServerPort, out CSteamID psteamIDGameServer)
		{
			psteamIDGameServer = new CSteamID();
			return this.platform.ISteamMatchmaking_GetLobbyGameServer(steamIDLobby.Value, out punGameServerIP, out punGameServerPort, out psteamIDGameServer.Value);
		}

		public ulong GetLobbyMemberByIndex(CSteamID steamIDLobby, int iMember)
		{
			return this.platform.ISteamMatchmaking_GetLobbyMemberByIndex(steamIDLobby.Value, iMember);
		}

		public string GetLobbyMemberData(CSteamID steamIDLobby, CSteamID steamIDUser, string pchKey)
		{
			return Marshal.PtrToStringAnsi(this.platform.ISteamMatchmaking_GetLobbyMemberData(steamIDLobby.Value, steamIDUser.Value, pchKey));
		}

		public int GetLobbyMemberLimit(CSteamID steamIDLobby)
		{
			return this.platform.ISteamMatchmaking_GetLobbyMemberLimit(steamIDLobby.Value);
		}

		public ulong GetLobbyOwner(CSteamID steamIDLobby)
		{
			return this.platform.ISteamMatchmaking_GetLobbyOwner(steamIDLobby.Value);
		}

		public int GetNumLobbyMembers(CSteamID steamIDLobby)
		{
			return this.platform.ISteamMatchmaking_GetNumLobbyMembers(steamIDLobby.Value);
		}

		public bool InviteUserToLobby(CSteamID steamIDLobby, CSteamID steamIDInvitee)
		{
			return this.platform.ISteamMatchmaking_InviteUserToLobby(steamIDLobby.Value, steamIDInvitee.Value);
		}

		public CallbackHandle JoinLobby(CSteamID steamIDLobby, Action<LobbyEnter_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamMatchmaking_JoinLobby(steamIDLobby.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return LobbyEnter_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public void LeaveLobby(CSteamID steamIDLobby)
		{
			this.platform.ISteamMatchmaking_LeaveLobby(steamIDLobby.Value);
		}

		public bool RemoveFavoriteGame(AppId_t nAppID, uint nIP, ushort nConnPort, ushort nQueryPort, uint unFlags)
		{
			return this.platform.ISteamMatchmaking_RemoveFavoriteGame(nAppID.Value, nIP, nConnPort, nQueryPort, unFlags);
		}

		public bool RequestLobbyData(CSteamID steamIDLobby)
		{
			return this.platform.ISteamMatchmaking_RequestLobbyData(steamIDLobby.Value);
		}

		public CallbackHandle RequestLobbyList(Action<LobbyMatchList_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamMatchmaking_RequestLobbyList();
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return LobbyMatchList_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool SendLobbyChatMsg(CSteamID steamIDLobby, IntPtr pvMsgBody, int cubMsgBody)
		{
			return this.platform.ISteamMatchmaking_SendLobbyChatMsg(steamIDLobby.Value, pvMsgBody, cubMsgBody);
		}

		public bool SetLinkedLobby(CSteamID steamIDLobby, CSteamID steamIDLobbyDependent)
		{
			return this.platform.ISteamMatchmaking_SetLinkedLobby(steamIDLobby.Value, steamIDLobbyDependent.Value);
		}

		public bool SetLobbyData(CSteamID steamIDLobby, string pchKey, string pchValue)
		{
			return this.platform.ISteamMatchmaking_SetLobbyData(steamIDLobby.Value, pchKey, pchValue);
		}

		public void SetLobbyGameServer(CSteamID steamIDLobby, uint unGameServerIP, ushort unGameServerPort, CSteamID steamIDGameServer)
		{
			this.platform.ISteamMatchmaking_SetLobbyGameServer(steamIDLobby.Value, unGameServerIP, unGameServerPort, steamIDGameServer.Value);
		}

		public bool SetLobbyJoinable(CSteamID steamIDLobby, bool bLobbyJoinable)
		{
			return this.platform.ISteamMatchmaking_SetLobbyJoinable(steamIDLobby.Value, bLobbyJoinable);
		}

		public void SetLobbyMemberData(CSteamID steamIDLobby, string pchKey, string pchValue)
		{
			this.platform.ISteamMatchmaking_SetLobbyMemberData(steamIDLobby.Value, pchKey, pchValue);
		}

		public bool SetLobbyMemberLimit(CSteamID steamIDLobby, int cMaxMembers)
		{
			return this.platform.ISteamMatchmaking_SetLobbyMemberLimit(steamIDLobby.Value, cMaxMembers);
		}

		public bool SetLobbyOwner(CSteamID steamIDLobby, CSteamID steamIDNewOwner)
		{
			return this.platform.ISteamMatchmaking_SetLobbyOwner(steamIDLobby.Value, steamIDNewOwner.Value);
		}

		public bool SetLobbyType(CSteamID steamIDLobby, LobbyType eLobbyType)
		{
			return this.platform.ISteamMatchmaking_SetLobbyType(steamIDLobby.Value, eLobbyType);
		}
	}
}