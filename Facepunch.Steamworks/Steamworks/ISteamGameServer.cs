using Steamworks.Data;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks
{
	internal class ISteamGameServer : SteamInterface
	{
		private ISteamGameServer.FInitGameServer _InitGameServer;

		private ISteamGameServer.FSetProduct _SetProduct;

		private ISteamGameServer.FSetGameDescription _SetGameDescription;

		private ISteamGameServer.FSetModDir _SetModDir;

		private ISteamGameServer.FSetDedicatedServer _SetDedicatedServer;

		private ISteamGameServer.FLogOn _LogOn;

		private ISteamGameServer.FLogOnAnonymous _LogOnAnonymous;

		private ISteamGameServer.FLogOff _LogOff;

		private ISteamGameServer.FBLoggedOn _BLoggedOn;

		private ISteamGameServer.FBSecure _BSecure;

		private ISteamGameServer.FGetSteamID _GetSteamID;

		private ISteamGameServer.FGetSteamID_Windows _GetSteamID_Windows;

		private ISteamGameServer.FWasRestartRequested _WasRestartRequested;

		private ISteamGameServer.FSetMaxPlayerCount _SetMaxPlayerCount;

		private ISteamGameServer.FSetBotPlayerCount _SetBotPlayerCount;

		private ISteamGameServer.FSetServerName _SetServerName;

		private ISteamGameServer.FSetMapName _SetMapName;

		private ISteamGameServer.FSetPasswordProtected _SetPasswordProtected;

		private ISteamGameServer.FSetSpectatorPort _SetSpectatorPort;

		private ISteamGameServer.FSetSpectatorServerName _SetSpectatorServerName;

		private ISteamGameServer.FClearAllKeyValues _ClearAllKeyValues;

		private ISteamGameServer.FSetKeyValue _SetKeyValue;

		private ISteamGameServer.FSetGameTags _SetGameTags;

		private ISteamGameServer.FSetGameData _SetGameData;

		private ISteamGameServer.FSetRegion _SetRegion;

		private ISteamGameServer.FSendUserConnectAndAuthenticate _SendUserConnectAndAuthenticate;

		private ISteamGameServer.FCreateUnauthenticatedUserConnection _CreateUnauthenticatedUserConnection;

		private ISteamGameServer.FCreateUnauthenticatedUserConnection_Windows _CreateUnauthenticatedUserConnection_Windows;

		private ISteamGameServer.FSendUserDisconnect _SendUserDisconnect;

		private ISteamGameServer.FBUpdateUserData _BUpdateUserData;

		private ISteamGameServer.FGetAuthSessionTicket _GetAuthSessionTicket;

		private ISteamGameServer.FBeginAuthSession _BeginAuthSession;

		private ISteamGameServer.FEndAuthSession _EndAuthSession;

		private ISteamGameServer.FCancelAuthTicket _CancelAuthTicket;

		private ISteamGameServer.FUserHasLicenseForApp _UserHasLicenseForApp;

		private ISteamGameServer.FRequestUserGroupStatus _RequestUserGroupStatus;

		private ISteamGameServer.FGetGameplayStats _GetGameplayStats;

		private ISteamGameServer.FGetServerReputation _GetServerReputation;

		private ISteamGameServer.FGetPublicIP _GetPublicIP;

		private ISteamGameServer.FHandleIncomingPacket _HandleIncomingPacket;

		private ISteamGameServer.FGetNextOutgoingPacket _GetNextOutgoingPacket;

		private ISteamGameServer.FEnableHeartbeats _EnableHeartbeats;

		private ISteamGameServer.FSetHeartbeatInterval _SetHeartbeatInterval;

		private ISteamGameServer.FForceHeartbeat _ForceHeartbeat;

		private ISteamGameServer.FAssociateWithClan _AssociateWithClan;

		private ISteamGameServer.FComputeNewPlayerCompatibility _ComputeNewPlayerCompatibility;

		public override string InterfaceName
		{
			get
			{
				return "SteamGameServer012";
			}
		}

		public ISteamGameServer()
		{
		}

		internal async Task<AssociateWithClanResult_t?> AssociateWithClan(SteamId steamIDClan)
		{
			AssociateWithClanResult_t? resultAsync = await AssociateWithClanResult_t.GetResultAsync(this._AssociateWithClan(this.Self, steamIDClan));
			return resultAsync;
		}

		internal BeginAuthResult BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, SteamId steamID)
		{
			return this._BeginAuthSession(this.Self, pAuthTicket, cbAuthTicket, steamID);
		}

		internal bool BLoggedOn()
		{
			return this._BLoggedOn(this.Self);
		}

		internal bool BSecure()
		{
			return this._BSecure(this.Self);
		}

		internal bool BUpdateUserData(SteamId steamIDUser, string pchPlayerName, uint uScore)
		{
			return this._BUpdateUserData(this.Self, steamIDUser, pchPlayerName, uScore);
		}

		internal void CancelAuthTicket(HAuthTicket hAuthTicket)
		{
			this._CancelAuthTicket(this.Self, hAuthTicket);
		}

		internal void ClearAllKeyValues()
		{
			this._ClearAllKeyValues(this.Self);
		}

		internal async Task<ComputeNewPlayerCompatibilityResult_t?> ComputeNewPlayerCompatibility(SteamId steamIDNewPlayer)
		{
			ComputeNewPlayerCompatibilityResult_t? resultAsync = await ComputeNewPlayerCompatibilityResult_t.GetResultAsync(this._ComputeNewPlayerCompatibility(this.Self, steamIDNewPlayer));
			return resultAsync;
		}

		internal SteamId CreateUnauthenticatedUserConnection()
		{
			SteamId self;
			if (Config.Os != OsType.Windows)
			{
				self = this._CreateUnauthenticatedUserConnection(this.Self);
			}
			else
			{
				SteamId steamId = new SteamId();
				this._CreateUnauthenticatedUserConnection_Windows(this.Self, ref steamId);
				self = steamId;
			}
			return self;
		}

		internal void EnableHeartbeats(bool bActive)
		{
			this._EnableHeartbeats(this.Self, bActive);
		}

		internal void EndAuthSession(SteamId steamID)
		{
			this._EndAuthSession(this.Self, steamID);
		}

		internal void ForceHeartbeat()
		{
			this._ForceHeartbeat(this.Self);
		}

		internal HAuthTicket GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
		{
			return this._GetAuthSessionTicket(this.Self, pTicket, cbMaxTicket, ref pcbTicket);
		}

		internal void GetGameplayStats()
		{
			this._GetGameplayStats(this.Self);
		}

		internal int GetNextOutgoingPacket(IntPtr pOut, int cbMaxOut, ref uint pNetAdr, ref ushort pPort)
		{
			int self = this._GetNextOutgoingPacket(this.Self, pOut, cbMaxOut, ref pNetAdr, ref pPort);
			return self;
		}

		internal uint GetPublicIP()
		{
			return this._GetPublicIP(this.Self);
		}

		internal async Task<GSReputation_t?> GetServerReputation()
		{
			GSReputation_t? resultAsync = await GSReputation_t.GetResultAsync(this._GetServerReputation(this.Self));
			return resultAsync;
		}

		internal SteamId GetSteamID()
		{
			SteamId self;
			if (Config.Os != OsType.Windows)
			{
				self = this._GetSteamID(this.Self);
			}
			else
			{
				SteamId steamId = new SteamId();
				this._GetSteamID_Windows(this.Self, ref steamId);
				self = steamId;
			}
			return self;
		}

		internal bool HandleIncomingPacket(IntPtr pData, int cbData, uint srcIP, ushort srcPort)
		{
			bool self = this._HandleIncomingPacket(this.Self, pData, cbData, srcIP, srcPort);
			return self;
		}

		internal bool InitGameServer(uint unIP, ushort usGamePort, ushort usQueryPort, uint unFlags, AppId nGameAppId, string pchVersionString)
		{
			bool self = this._InitGameServer(this.Self, unIP, usGamePort, usQueryPort, unFlags, nGameAppId, pchVersionString);
			return self;
		}

		public override void InitInternals()
		{
			this._InitGameServer = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FInitGameServer>(Marshal.ReadIntPtr(this.VTable, 0));
			this._SetProduct = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FSetProduct>(Marshal.ReadIntPtr(this.VTable, 8));
			this._SetGameDescription = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FSetGameDescription>(Marshal.ReadIntPtr(this.VTable, 16));
			this._SetModDir = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FSetModDir>(Marshal.ReadIntPtr(this.VTable, 24));
			this._SetDedicatedServer = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FSetDedicatedServer>(Marshal.ReadIntPtr(this.VTable, 32));
			this._LogOn = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FLogOn>(Marshal.ReadIntPtr(this.VTable, 40));
			this._LogOnAnonymous = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FLogOnAnonymous>(Marshal.ReadIntPtr(this.VTable, 48));
			this._LogOff = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FLogOff>(Marshal.ReadIntPtr(this.VTable, 56));
			this._BLoggedOn = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FBLoggedOn>(Marshal.ReadIntPtr(this.VTable, 64));
			this._BSecure = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FBSecure>(Marshal.ReadIntPtr(this.VTable, 72));
			this._GetSteamID = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FGetSteamID>(Marshal.ReadIntPtr(this.VTable, 80));
			this._GetSteamID_Windows = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FGetSteamID_Windows>(Marshal.ReadIntPtr(this.VTable, 80));
			this._WasRestartRequested = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FWasRestartRequested>(Marshal.ReadIntPtr(this.VTable, 88));
			this._SetMaxPlayerCount = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FSetMaxPlayerCount>(Marshal.ReadIntPtr(this.VTable, 96));
			this._SetBotPlayerCount = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FSetBotPlayerCount>(Marshal.ReadIntPtr(this.VTable, 104));
			this._SetServerName = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FSetServerName>(Marshal.ReadIntPtr(this.VTable, 112));
			this._SetMapName = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FSetMapName>(Marshal.ReadIntPtr(this.VTable, 120));
			this._SetPasswordProtected = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FSetPasswordProtected>(Marshal.ReadIntPtr(this.VTable, 128));
			this._SetSpectatorPort = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FSetSpectatorPort>(Marshal.ReadIntPtr(this.VTable, 136));
			this._SetSpectatorServerName = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FSetSpectatorServerName>(Marshal.ReadIntPtr(this.VTable, 144));
			this._ClearAllKeyValues = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FClearAllKeyValues>(Marshal.ReadIntPtr(this.VTable, 152));
			this._SetKeyValue = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FSetKeyValue>(Marshal.ReadIntPtr(this.VTable, 160));
			this._SetGameTags = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FSetGameTags>(Marshal.ReadIntPtr(this.VTable, 168));
			this._SetGameData = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FSetGameData>(Marshal.ReadIntPtr(this.VTable, 176));
			this._SetRegion = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FSetRegion>(Marshal.ReadIntPtr(this.VTable, 184));
			this._SendUserConnectAndAuthenticate = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FSendUserConnectAndAuthenticate>(Marshal.ReadIntPtr(this.VTable, 192));
			this._CreateUnauthenticatedUserConnection = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FCreateUnauthenticatedUserConnection>(Marshal.ReadIntPtr(this.VTable, 200));
			this._CreateUnauthenticatedUserConnection_Windows = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FCreateUnauthenticatedUserConnection_Windows>(Marshal.ReadIntPtr(this.VTable, 200));
			this._SendUserDisconnect = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FSendUserDisconnect>(Marshal.ReadIntPtr(this.VTable, 208));
			this._BUpdateUserData = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FBUpdateUserData>(Marshal.ReadIntPtr(this.VTable, 216));
			this._GetAuthSessionTicket = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FGetAuthSessionTicket>(Marshal.ReadIntPtr(this.VTable, 224));
			this._BeginAuthSession = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FBeginAuthSession>(Marshal.ReadIntPtr(this.VTable, 232));
			this._EndAuthSession = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FEndAuthSession>(Marshal.ReadIntPtr(this.VTable, 240));
			this._CancelAuthTicket = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FCancelAuthTicket>(Marshal.ReadIntPtr(this.VTable, 248));
			this._UserHasLicenseForApp = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FUserHasLicenseForApp>(Marshal.ReadIntPtr(this.VTable, 256));
			this._RequestUserGroupStatus = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FRequestUserGroupStatus>(Marshal.ReadIntPtr(this.VTable, 264));
			this._GetGameplayStats = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FGetGameplayStats>(Marshal.ReadIntPtr(this.VTable, 272));
			this._GetServerReputation = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FGetServerReputation>(Marshal.ReadIntPtr(this.VTable, 280));
			this._GetPublicIP = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FGetPublicIP>(Marshal.ReadIntPtr(this.VTable, 288));
			this._HandleIncomingPacket = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FHandleIncomingPacket>(Marshal.ReadIntPtr(this.VTable, 296));
			this._GetNextOutgoingPacket = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FGetNextOutgoingPacket>(Marshal.ReadIntPtr(this.VTable, 304));
			this._EnableHeartbeats = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FEnableHeartbeats>(Marshal.ReadIntPtr(this.VTable, 312));
			this._SetHeartbeatInterval = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FSetHeartbeatInterval>(Marshal.ReadIntPtr(this.VTable, 320));
			this._ForceHeartbeat = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FForceHeartbeat>(Marshal.ReadIntPtr(this.VTable, 328));
			this._AssociateWithClan = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FAssociateWithClan>(Marshal.ReadIntPtr(this.VTable, 336));
			this._ComputeNewPlayerCompatibility = Marshal.GetDelegateForFunctionPointer<ISteamGameServer.FComputeNewPlayerCompatibility>(Marshal.ReadIntPtr(this.VTable, 344));
		}

		internal void LogOff()
		{
			this._LogOff(this.Self);
		}

		internal void LogOn(string pszToken)
		{
			this._LogOn(this.Self, pszToken);
		}

		internal void LogOnAnonymous()
		{
			this._LogOnAnonymous(this.Self);
		}

		internal bool RequestUserGroupStatus(SteamId steamIDUser, SteamId steamIDGroup)
		{
			return this._RequestUserGroupStatus(this.Self, steamIDUser, steamIDGroup);
		}

		internal bool SendUserConnectAndAuthenticate(uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, ref SteamId pSteamIDUser)
		{
			bool self = this._SendUserConnectAndAuthenticate(this.Self, unIPClient, pvAuthBlob, cubAuthBlobSize, ref pSteamIDUser);
			return self;
		}

		internal void SendUserDisconnect(SteamId steamIDUser)
		{
			this._SendUserDisconnect(this.Self, steamIDUser);
		}

		internal void SetBotPlayerCount(int cBotplayers)
		{
			this._SetBotPlayerCount(this.Self, cBotplayers);
		}

		internal void SetDedicatedServer(bool bDedicated)
		{
			this._SetDedicatedServer(this.Self, bDedicated);
		}

		internal void SetGameData(string pchGameData)
		{
			this._SetGameData(this.Self, pchGameData);
		}

		internal void SetGameDescription(string pszGameDescription)
		{
			this._SetGameDescription(this.Self, pszGameDescription);
		}

		internal void SetGameTags(string pchGameTags)
		{
			this._SetGameTags(this.Self, pchGameTags);
		}

		internal void SetHeartbeatInterval(int iHeartbeatInterval)
		{
			this._SetHeartbeatInterval(this.Self, iHeartbeatInterval);
		}

		internal void SetKeyValue(string pKey, string pValue)
		{
			this._SetKeyValue(this.Self, pKey, pValue);
		}

		internal void SetMapName(string pszMapName)
		{
			this._SetMapName(this.Self, pszMapName);
		}

		internal void SetMaxPlayerCount(int cPlayersMax)
		{
			this._SetMaxPlayerCount(this.Self, cPlayersMax);
		}

		internal void SetModDir(string pszModDir)
		{
			this._SetModDir(this.Self, pszModDir);
		}

		internal void SetPasswordProtected(bool bPasswordProtected)
		{
			this._SetPasswordProtected(this.Self, bPasswordProtected);
		}

		internal void SetProduct(string pszProduct)
		{
			this._SetProduct(this.Self, pszProduct);
		}

		internal void SetRegion(string pszRegion)
		{
			this._SetRegion(this.Self, pszRegion);
		}

		internal void SetServerName(string pszServerName)
		{
			this._SetServerName(this.Self, pszServerName);
		}

		internal void SetSpectatorPort(ushort unSpectatorPort)
		{
			this._SetSpectatorPort(this.Self, unSpectatorPort);
		}

		internal void SetSpectatorServerName(string pszSpectatorServerName)
		{
			this._SetSpectatorServerName(this.Self, pszSpectatorServerName);
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._InitGameServer = null;
			this._SetProduct = null;
			this._SetGameDescription = null;
			this._SetModDir = null;
			this._SetDedicatedServer = null;
			this._LogOn = null;
			this._LogOnAnonymous = null;
			this._LogOff = null;
			this._BLoggedOn = null;
			this._BSecure = null;
			this._GetSteamID = null;
			this._GetSteamID_Windows = null;
			this._WasRestartRequested = null;
			this._SetMaxPlayerCount = null;
			this._SetBotPlayerCount = null;
			this._SetServerName = null;
			this._SetMapName = null;
			this._SetPasswordProtected = null;
			this._SetSpectatorPort = null;
			this._SetSpectatorServerName = null;
			this._ClearAllKeyValues = null;
			this._SetKeyValue = null;
			this._SetGameTags = null;
			this._SetGameData = null;
			this._SetRegion = null;
			this._SendUserConnectAndAuthenticate = null;
			this._CreateUnauthenticatedUserConnection = null;
			this._CreateUnauthenticatedUserConnection_Windows = null;
			this._SendUserDisconnect = null;
			this._BUpdateUserData = null;
			this._GetAuthSessionTicket = null;
			this._BeginAuthSession = null;
			this._EndAuthSession = null;
			this._CancelAuthTicket = null;
			this._UserHasLicenseForApp = null;
			this._RequestUserGroupStatus = null;
			this._GetGameplayStats = null;
			this._GetServerReputation = null;
			this._GetPublicIP = null;
			this._HandleIncomingPacket = null;
			this._GetNextOutgoingPacket = null;
			this._EnableHeartbeats = null;
			this._SetHeartbeatInterval = null;
			this._ForceHeartbeat = null;
			this._AssociateWithClan = null;
			this._ComputeNewPlayerCompatibility = null;
		}

		internal UserHasLicenseForAppResult UserHasLicenseForApp(SteamId steamID, AppId appID)
		{
			return this._UserHasLicenseForApp(this.Self, steamID, appID);
		}

		internal bool WasRestartRequested()
		{
			return this._WasRestartRequested(this.Self);
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FAssociateWithClan(IntPtr self, SteamId steamIDClan);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate BeginAuthResult FBeginAuthSession(IntPtr self, IntPtr pAuthTicket, int cbAuthTicket, SteamId steamID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBLoggedOn(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBSecure(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBUpdateUserData(IntPtr self, SteamId steamIDUser, string pchPlayerName, uint uScore);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FCancelAuthTicket(IntPtr self, HAuthTicket hAuthTicket);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FClearAllKeyValues(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FComputeNewPlayerCompatibility(IntPtr self, SteamId steamIDNewPlayer);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamId FCreateUnauthenticatedUserConnection(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FCreateUnauthenticatedUserConnection_Windows(IntPtr self, ref SteamId retVal);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FEnableHeartbeats(IntPtr self, bool bActive);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FEndAuthSession(IntPtr self, SteamId steamID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FForceHeartbeat(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate HAuthTicket FGetAuthSessionTicket(IntPtr self, IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FGetGameplayStats(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetNextOutgoingPacket(IntPtr self, IntPtr pOut, int cbMaxOut, ref uint pNetAdr, ref ushort pPort);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetPublicIP(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FGetServerReputation(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamId FGetSteamID(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FGetSteamID_Windows(IntPtr self, ref SteamId retVal);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FHandleIncomingPacket(IntPtr self, IntPtr pData, int cbData, uint srcIP, ushort srcPort);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FInitGameServer(IntPtr self, uint unIP, ushort usGamePort, ushort usQueryPort, uint unFlags, AppId nGameAppId, string pchVersionString);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FLogOff(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FLogOn(IntPtr self, string pszToken);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FLogOnAnonymous(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FRequestUserGroupStatus(IntPtr self, SteamId steamIDUser, SteamId steamIDGroup);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSendUserConnectAndAuthenticate(IntPtr self, uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, ref SteamId pSteamIDUser);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSendUserDisconnect(IntPtr self, SteamId steamIDUser);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetBotPlayerCount(IntPtr self, int cBotplayers);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetDedicatedServer(IntPtr self, bool bDedicated);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetGameData(IntPtr self, string pchGameData);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetGameDescription(IntPtr self, string pszGameDescription);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetGameTags(IntPtr self, string pchGameTags);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetHeartbeatInterval(IntPtr self, int iHeartbeatInterval);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetKeyValue(IntPtr self, string pKey, string pValue);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetMapName(IntPtr self, string pszMapName);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetMaxPlayerCount(IntPtr self, int cPlayersMax);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetModDir(IntPtr self, string pszModDir);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetPasswordProtected(IntPtr self, bool bPasswordProtected);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetProduct(IntPtr self, string pszProduct);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetRegion(IntPtr self, string pszRegion);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetServerName(IntPtr self, string pszServerName);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetSpectatorPort(IntPtr self, ushort unSpectatorPort);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetSpectatorServerName(IntPtr self, string pszSpectatorServerName);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate UserHasLicenseForAppResult FUserHasLicenseForApp(IntPtr self, SteamId steamID, AppId appID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FWasRestartRequested(IntPtr self);
	}
}