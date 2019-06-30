using Steamworks.Data;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks
{
	internal class ISteamUser : SteamInterface
	{
		private ISteamUser.FGetHSteamUser _GetHSteamUser;

		private ISteamUser.FBLoggedOn _BLoggedOn;

		private ISteamUser.FGetSteamID _GetSteamID;

		private ISteamUser.FGetSteamID_Windows _GetSteamID_Windows;

		private ISteamUser.FInitiateGameConnection _InitiateGameConnection;

		private ISteamUser.FTerminateGameConnection _TerminateGameConnection;

		private ISteamUser.FTrackAppUsageEvent _TrackAppUsageEvent;

		private ISteamUser.FGetUserDataFolder _GetUserDataFolder;

		private ISteamUser.FStartVoiceRecording _StartVoiceRecording;

		private ISteamUser.FStopVoiceRecording _StopVoiceRecording;

		private ISteamUser.FGetAvailableVoice _GetAvailableVoice;

		private ISteamUser.FGetVoice _GetVoice;

		private ISteamUser.FDecompressVoice _DecompressVoice;

		private ISteamUser.FGetVoiceOptimalSampleRate _GetVoiceOptimalSampleRate;

		private ISteamUser.FGetAuthSessionTicket _GetAuthSessionTicket;

		private ISteamUser.FBeginAuthSession _BeginAuthSession;

		private ISteamUser.FEndAuthSession _EndAuthSession;

		private ISteamUser.FCancelAuthTicket _CancelAuthTicket;

		private ISteamUser.FUserHasLicenseForApp _UserHasLicenseForApp;

		private ISteamUser.FBIsBehindNAT _BIsBehindNAT;

		private ISteamUser.FAdvertiseGame _AdvertiseGame;

		private ISteamUser.FRequestEncryptedAppTicket _RequestEncryptedAppTicket;

		private ISteamUser.FGetEncryptedAppTicket _GetEncryptedAppTicket;

		private ISteamUser.FGetGameBadgeLevel _GetGameBadgeLevel;

		private ISteamUser.FGetPlayerSteamLevel _GetPlayerSteamLevel;

		private ISteamUser.FRequestStoreAuthURL _RequestStoreAuthURL;

		private ISteamUser.FBIsPhoneVerified _BIsPhoneVerified;

		private ISteamUser.FBIsTwoFactorEnabled _BIsTwoFactorEnabled;

		private ISteamUser.FBIsPhoneIdentifying _BIsPhoneIdentifying;

		private ISteamUser.FBIsPhoneRequiringVerification _BIsPhoneRequiringVerification;

		private ISteamUser.FGetMarketEligibility _GetMarketEligibility;

		public override string InterfaceName
		{
			get
			{
				return "SteamUser020";
			}
		}

		public ISteamUser()
		{
		}

		internal void AdvertiseGame(SteamId steamIDGameServer, uint unIPServer, ushort usPortServer)
		{
			this._AdvertiseGame(this.Self, steamIDGameServer, unIPServer, usPortServer);
		}

		internal BeginAuthResult BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, SteamId steamID)
		{
			return this._BeginAuthSession(this.Self, pAuthTicket, cbAuthTicket, steamID);
		}

		internal bool BIsBehindNAT()
		{
			return this._BIsBehindNAT(this.Self);
		}

		internal bool BIsPhoneIdentifying()
		{
			return this._BIsPhoneIdentifying(this.Self);
		}

		internal bool BIsPhoneRequiringVerification()
		{
			return this._BIsPhoneRequiringVerification(this.Self);
		}

		internal bool BIsPhoneVerified()
		{
			return this._BIsPhoneVerified(this.Self);
		}

		internal bool BIsTwoFactorEnabled()
		{
			return this._BIsTwoFactorEnabled(this.Self);
		}

		internal bool BLoggedOn()
		{
			return this._BLoggedOn(this.Self);
		}

		internal void CancelAuthTicket(HAuthTicket hAuthTicket)
		{
			this._CancelAuthTicket(this.Self, hAuthTicket);
		}

		internal VoiceResult DecompressVoice(IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, ref uint nBytesWritten, uint nDesiredSampleRate)
		{
			VoiceResult self = this._DecompressVoice(this.Self, pCompressed, cbCompressed, pDestBuffer, cbDestBufferSize, ref nBytesWritten, nDesiredSampleRate);
			return self;
		}

		internal void EndAuthSession(SteamId steamID)
		{
			this._EndAuthSession(this.Self, steamID);
		}

		internal HAuthTicket GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
		{
			return this._GetAuthSessionTicket(this.Self, pTicket, cbMaxTicket, ref pcbTicket);
		}

		internal VoiceResult GetAvailableVoice(ref uint pcbCompressed, ref uint pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
		{
			return this._GetAvailableVoice(this.Self, ref pcbCompressed, ref pcbUncompressed_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
		}

		internal bool GetEncryptedAppTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
		{
			return this._GetEncryptedAppTicket(this.Self, pTicket, cbMaxTicket, ref pcbTicket);
		}

		internal int GetGameBadgeLevel(int nSeries, bool bFoil)
		{
			return this._GetGameBadgeLevel(this.Self, nSeries, bFoil);
		}

		internal HSteamUser GetHSteamUser()
		{
			return this._GetHSteamUser(this.Self);
		}

		internal async Task<MarketEligibilityResponse_t?> GetMarketEligibility()
		{
			MarketEligibilityResponse_t? resultAsync = await MarketEligibilityResponse_t.GetResultAsync(this._GetMarketEligibility(this.Self));
			return resultAsync;
		}

		internal int GetPlayerSteamLevel()
		{
			return this._GetPlayerSteamLevel(this.Self);
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

		internal bool GetUserDataFolder(StringBuilder pchBuffer, int cubBuffer)
		{
			return this._GetUserDataFolder(this.Self, pchBuffer, cubBuffer);
		}

		internal VoiceResult GetVoice(bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, ref uint nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, ref uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
		{
			VoiceResult self = this._GetVoice(this.Self, bWantCompressed, pDestBuffer, cbDestBufferSize, ref nBytesWritten, bWantUncompressed_Deprecated, pUncompressedDestBuffer_Deprecated, cbUncompressedDestBufferSize_Deprecated, ref nUncompressBytesWritten_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
			return self;
		}

		internal uint GetVoiceOptimalSampleRate()
		{
			return this._GetVoiceOptimalSampleRate(this.Self);
		}

		internal int InitiateGameConnection(IntPtr pAuthBlob, int cbMaxAuthBlob, SteamId steamIDGameServer, uint unIPServer, ushort usPortServer, bool bSecure)
		{
			int self = this._InitiateGameConnection(this.Self, pAuthBlob, cbMaxAuthBlob, steamIDGameServer, unIPServer, usPortServer, bSecure);
			return self;
		}

		public override void InitInternals()
		{
			this._GetHSteamUser = Marshal.GetDelegateForFunctionPointer<ISteamUser.FGetHSteamUser>(Marshal.ReadIntPtr(this.VTable, 0));
			this._BLoggedOn = Marshal.GetDelegateForFunctionPointer<ISteamUser.FBLoggedOn>(Marshal.ReadIntPtr(this.VTable, 8));
			this._GetSteamID = Marshal.GetDelegateForFunctionPointer<ISteamUser.FGetSteamID>(Marshal.ReadIntPtr(this.VTable, 16));
			this._GetSteamID_Windows = Marshal.GetDelegateForFunctionPointer<ISteamUser.FGetSteamID_Windows>(Marshal.ReadIntPtr(this.VTable, 16));
			this._InitiateGameConnection = Marshal.GetDelegateForFunctionPointer<ISteamUser.FInitiateGameConnection>(Marshal.ReadIntPtr(this.VTable, 24));
			this._TerminateGameConnection = Marshal.GetDelegateForFunctionPointer<ISteamUser.FTerminateGameConnection>(Marshal.ReadIntPtr(this.VTable, 32));
			this._TrackAppUsageEvent = Marshal.GetDelegateForFunctionPointer<ISteamUser.FTrackAppUsageEvent>(Marshal.ReadIntPtr(this.VTable, 40));
			this._GetUserDataFolder = Marshal.GetDelegateForFunctionPointer<ISteamUser.FGetUserDataFolder>(Marshal.ReadIntPtr(this.VTable, 48));
			this._StartVoiceRecording = Marshal.GetDelegateForFunctionPointer<ISteamUser.FStartVoiceRecording>(Marshal.ReadIntPtr(this.VTable, 56));
			this._StopVoiceRecording = Marshal.GetDelegateForFunctionPointer<ISteamUser.FStopVoiceRecording>(Marshal.ReadIntPtr(this.VTable, 64));
			this._GetAvailableVoice = Marshal.GetDelegateForFunctionPointer<ISteamUser.FGetAvailableVoice>(Marshal.ReadIntPtr(this.VTable, 72));
			this._GetVoice = Marshal.GetDelegateForFunctionPointer<ISteamUser.FGetVoice>(Marshal.ReadIntPtr(this.VTable, 80));
			this._DecompressVoice = Marshal.GetDelegateForFunctionPointer<ISteamUser.FDecompressVoice>(Marshal.ReadIntPtr(this.VTable, 88));
			this._GetVoiceOptimalSampleRate = Marshal.GetDelegateForFunctionPointer<ISteamUser.FGetVoiceOptimalSampleRate>(Marshal.ReadIntPtr(this.VTable, 96));
			this._GetAuthSessionTicket = Marshal.GetDelegateForFunctionPointer<ISteamUser.FGetAuthSessionTicket>(Marshal.ReadIntPtr(this.VTable, 104));
			this._BeginAuthSession = Marshal.GetDelegateForFunctionPointer<ISteamUser.FBeginAuthSession>(Marshal.ReadIntPtr(this.VTable, 112));
			this._EndAuthSession = Marshal.GetDelegateForFunctionPointer<ISteamUser.FEndAuthSession>(Marshal.ReadIntPtr(this.VTable, 120));
			this._CancelAuthTicket = Marshal.GetDelegateForFunctionPointer<ISteamUser.FCancelAuthTicket>(Marshal.ReadIntPtr(this.VTable, 128));
			this._UserHasLicenseForApp = Marshal.GetDelegateForFunctionPointer<ISteamUser.FUserHasLicenseForApp>(Marshal.ReadIntPtr(this.VTable, 136));
			this._BIsBehindNAT = Marshal.GetDelegateForFunctionPointer<ISteamUser.FBIsBehindNAT>(Marshal.ReadIntPtr(this.VTable, 144));
			this._AdvertiseGame = Marshal.GetDelegateForFunctionPointer<ISteamUser.FAdvertiseGame>(Marshal.ReadIntPtr(this.VTable, 152));
			this._RequestEncryptedAppTicket = Marshal.GetDelegateForFunctionPointer<ISteamUser.FRequestEncryptedAppTicket>(Marshal.ReadIntPtr(this.VTable, 160));
			this._GetEncryptedAppTicket = Marshal.GetDelegateForFunctionPointer<ISteamUser.FGetEncryptedAppTicket>(Marshal.ReadIntPtr(this.VTable, 168));
			this._GetGameBadgeLevel = Marshal.GetDelegateForFunctionPointer<ISteamUser.FGetGameBadgeLevel>(Marshal.ReadIntPtr(this.VTable, 176));
			this._GetPlayerSteamLevel = Marshal.GetDelegateForFunctionPointer<ISteamUser.FGetPlayerSteamLevel>(Marshal.ReadIntPtr(this.VTable, 184));
			this._RequestStoreAuthURL = Marshal.GetDelegateForFunctionPointer<ISteamUser.FRequestStoreAuthURL>(Marshal.ReadIntPtr(this.VTable, 192));
			this._BIsPhoneVerified = Marshal.GetDelegateForFunctionPointer<ISteamUser.FBIsPhoneVerified>(Marshal.ReadIntPtr(this.VTable, 200));
			this._BIsTwoFactorEnabled = Marshal.GetDelegateForFunctionPointer<ISteamUser.FBIsTwoFactorEnabled>(Marshal.ReadIntPtr(this.VTable, 208));
			this._BIsPhoneIdentifying = Marshal.GetDelegateForFunctionPointer<ISteamUser.FBIsPhoneIdentifying>(Marshal.ReadIntPtr(this.VTable, 216));
			this._BIsPhoneRequiringVerification = Marshal.GetDelegateForFunctionPointer<ISteamUser.FBIsPhoneRequiringVerification>(Marshal.ReadIntPtr(this.VTable, 224));
			this._GetMarketEligibility = Marshal.GetDelegateForFunctionPointer<ISteamUser.FGetMarketEligibility>(Marshal.ReadIntPtr(this.VTable, 232));
		}

		internal async Task<EncryptedAppTicketResponse_t?> RequestEncryptedAppTicket(IntPtr pDataToInclude, int cbDataToInclude)
		{
			EncryptedAppTicketResponse_t? resultAsync = await EncryptedAppTicketResponse_t.GetResultAsync(this._RequestEncryptedAppTicket(this.Self, pDataToInclude, cbDataToInclude));
			return resultAsync;
		}

		internal async Task<StoreAuthURLResponse_t?> RequestStoreAuthURL(string pchRedirectURL)
		{
			StoreAuthURLResponse_t? resultAsync = await StoreAuthURLResponse_t.GetResultAsync(this._RequestStoreAuthURL(this.Self, pchRedirectURL));
			return resultAsync;
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._GetHSteamUser = null;
			this._BLoggedOn = null;
			this._GetSteamID = null;
			this._GetSteamID_Windows = null;
			this._InitiateGameConnection = null;
			this._TerminateGameConnection = null;
			this._TrackAppUsageEvent = null;
			this._GetUserDataFolder = null;
			this._StartVoiceRecording = null;
			this._StopVoiceRecording = null;
			this._GetAvailableVoice = null;
			this._GetVoice = null;
			this._DecompressVoice = null;
			this._GetVoiceOptimalSampleRate = null;
			this._GetAuthSessionTicket = null;
			this._BeginAuthSession = null;
			this._EndAuthSession = null;
			this._CancelAuthTicket = null;
			this._UserHasLicenseForApp = null;
			this._BIsBehindNAT = null;
			this._AdvertiseGame = null;
			this._RequestEncryptedAppTicket = null;
			this._GetEncryptedAppTicket = null;
			this._GetGameBadgeLevel = null;
			this._GetPlayerSteamLevel = null;
			this._RequestStoreAuthURL = null;
			this._BIsPhoneVerified = null;
			this._BIsTwoFactorEnabled = null;
			this._BIsPhoneIdentifying = null;
			this._BIsPhoneRequiringVerification = null;
			this._GetMarketEligibility = null;
		}

		internal void StartVoiceRecording()
		{
			this._StartVoiceRecording(this.Self);
		}

		internal void StopVoiceRecording()
		{
			this._StopVoiceRecording(this.Self);
		}

		internal void TerminateGameConnection(uint unIPServer, ushort usPortServer)
		{
			this._TerminateGameConnection(this.Self, unIPServer, usPortServer);
		}

		internal void TrackAppUsageEvent(GameId gameID, int eAppUsageEvent, string pchExtraInfo)
		{
			this._TrackAppUsageEvent(this.Self, gameID, eAppUsageEvent, pchExtraInfo);
		}

		internal UserHasLicenseForAppResult UserHasLicenseForApp(SteamId steamID, AppId appID)
		{
			return this._UserHasLicenseForApp(this.Self, steamID, appID);
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FAdvertiseGame(IntPtr self, SteamId steamIDGameServer, uint unIPServer, ushort usPortServer);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate BeginAuthResult FBeginAuthSession(IntPtr self, IntPtr pAuthTicket, int cbAuthTicket, SteamId steamID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsBehindNAT(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsPhoneIdentifying(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsPhoneRequiringVerification(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsPhoneVerified(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsTwoFactorEnabled(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBLoggedOn(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FCancelAuthTicket(IntPtr self, HAuthTicket hAuthTicket);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate VoiceResult FDecompressVoice(IntPtr self, IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, ref uint nBytesWritten, uint nDesiredSampleRate);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FEndAuthSession(IntPtr self, SteamId steamID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate HAuthTicket FGetAuthSessionTicket(IntPtr self, IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate VoiceResult FGetAvailableVoice(IntPtr self, ref uint pcbCompressed, ref uint pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetEncryptedAppTicket(IntPtr self, IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetGameBadgeLevel(IntPtr self, int nSeries, bool bFoil);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate HSteamUser FGetHSteamUser(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FGetMarketEligibility(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetPlayerSteamLevel(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamId FGetSteamID(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FGetSteamID_Windows(IntPtr self, ref SteamId retVal);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetUserDataFolder(IntPtr self, StringBuilder pchBuffer, int cubBuffer);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate VoiceResult FGetVoice(IntPtr self, bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, ref uint nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, ref uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetVoiceOptimalSampleRate(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FInitiateGameConnection(IntPtr self, IntPtr pAuthBlob, int cbMaxAuthBlob, SteamId steamIDGameServer, uint unIPServer, ushort usPortServer, bool bSecure);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FRequestEncryptedAppTicket(IntPtr self, IntPtr pDataToInclude, int cbDataToInclude);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FRequestStoreAuthURL(IntPtr self, string pchRedirectURL);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FStartVoiceRecording(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FStopVoiceRecording(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FTerminateGameConnection(IntPtr self, uint unIPServer, ushort usPortServer);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FTrackAppUsageEvent(IntPtr self, GameId gameID, int eAppUsageEvent, string pchExtraInfo);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate UserHasLicenseForAppResult FUserHasLicenseForApp(IntPtr self, SteamId steamID, AppId appID);
	}
}