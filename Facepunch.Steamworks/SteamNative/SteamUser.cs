using Facepunch.Steamworks;
using System;
using System.Text;

namespace SteamNative
{
	internal class SteamUser : IDisposable
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

		internal SteamUser(BaseSteamworks steamworks, IntPtr pointer)
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

		public void AdvertiseGame(CSteamID steamIDGameServer, uint unIPServer, ushort usPortServer)
		{
			this.platform.ISteamUser_AdvertiseGame(steamIDGameServer.Value, unIPServer, usPortServer);
		}

		public BeginAuthSessionResult BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, CSteamID steamID)
		{
			return this.platform.ISteamUser_BeginAuthSession(pAuthTicket, cbAuthTicket, steamID.Value);
		}

		public bool BIsBehindNAT()
		{
			return this.platform.ISteamUser_BIsBehindNAT();
		}

		public bool BIsPhoneIdentifying()
		{
			return this.platform.ISteamUser_BIsPhoneIdentifying();
		}

		public bool BIsPhoneRequiringVerification()
		{
			return this.platform.ISteamUser_BIsPhoneRequiringVerification();
		}

		public bool BIsPhoneVerified()
		{
			return this.platform.ISteamUser_BIsPhoneVerified();
		}

		public bool BIsTwoFactorEnabled()
		{
			return this.platform.ISteamUser_BIsTwoFactorEnabled();
		}

		public bool BLoggedOn()
		{
			return this.platform.ISteamUser_BLoggedOn();
		}

		public void CancelAuthTicket(HAuthTicket hAuthTicket)
		{
			this.platform.ISteamUser_CancelAuthTicket(hAuthTicket.Value);
		}

		public VoiceResult DecompressVoice(IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, out uint nBytesWritten, uint nDesiredSampleRate)
		{
			return this.platform.ISteamUser_DecompressVoice(pCompressed, cbCompressed, pDestBuffer, cbDestBufferSize, out nBytesWritten, nDesiredSampleRate);
		}

		public virtual void Dispose()
		{
			if (this.platform != null)
			{
				this.platform.Dispose();
				this.platform = null;
			}
		}

		public void EndAuthSession(CSteamID steamID)
		{
			this.platform.ISteamUser_EndAuthSession(steamID.Value);
		}

		public HAuthTicket GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, out uint pcbTicket)
		{
			return this.platform.ISteamUser_GetAuthSessionTicket(pTicket, cbMaxTicket, out pcbTicket);
		}

		public VoiceResult GetAvailableVoice(out uint pcbCompressed, out uint pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
		{
			return this.platform.ISteamUser_GetAvailableVoice(out pcbCompressed, out pcbUncompressed_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
		}

		public bool GetEncryptedAppTicket(IntPtr pTicket, int cbMaxTicket, out uint pcbTicket)
		{
			return this.platform.ISteamUser_GetEncryptedAppTicket(pTicket, cbMaxTicket, out pcbTicket);
		}

		public int GetGameBadgeLevel(int nSeries, bool bFoil)
		{
			return this.platform.ISteamUser_GetGameBadgeLevel(nSeries, bFoil);
		}

		public HSteamUser GetHSteamUser()
		{
			return this.platform.ISteamUser_GetHSteamUser();
		}

		public int GetPlayerSteamLevel()
		{
			return this.platform.ISteamUser_GetPlayerSteamLevel();
		}

		public ulong GetSteamID()
		{
			return this.platform.ISteamUser_GetSteamID();
		}

		public string GetUserDataFolder()
		{
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			if (!this.platform.ISteamUser_GetUserDataFolder(stringBuilder, 4096))
			{
				return null;
			}
			return stringBuilder.ToString();
		}

		public VoiceResult GetVoice(bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, out uint nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, out uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
		{
			return this.platform.ISteamUser_GetVoice(bWantCompressed, pDestBuffer, cbDestBufferSize, out nBytesWritten, bWantUncompressed_Deprecated, pUncompressedDestBuffer_Deprecated, cbUncompressedDestBufferSize_Deprecated, out nUncompressBytesWritten_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
		}

		public uint GetVoiceOptimalSampleRate()
		{
			return this.platform.ISteamUser_GetVoiceOptimalSampleRate();
		}

		public int InitiateGameConnection(IntPtr pAuthBlob, int cbMaxAuthBlob, CSteamID steamIDGameServer, uint unIPServer, ushort usPortServer, bool bSecure)
		{
			return this.platform.ISteamUser_InitiateGameConnection(pAuthBlob, cbMaxAuthBlob, steamIDGameServer.Value, unIPServer, usPortServer, bSecure);
		}

		public CallbackHandle RequestEncryptedAppTicket(IntPtr pDataToInclude, int cbDataToInclude, Action<EncryptedAppTicketResponse_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUser_RequestEncryptedAppTicket(pDataToInclude, cbDataToInclude);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return EncryptedAppTicketResponse_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle RequestStoreAuthURL(string pchRedirectURL, Action<StoreAuthURLResponse_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUser_RequestStoreAuthURL(pchRedirectURL);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return StoreAuthURLResponse_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public void StartVoiceRecording()
		{
			this.platform.ISteamUser_StartVoiceRecording();
		}

		public void StopVoiceRecording()
		{
			this.platform.ISteamUser_StopVoiceRecording();
		}

		public void TerminateGameConnection(uint unIPServer, ushort usPortServer)
		{
			this.platform.ISteamUser_TerminateGameConnection(unIPServer, usPortServer);
		}

		public void TrackAppUsageEvent(CGameID gameID, int eAppUsageEvent, string pchExtraInfo)
		{
			this.platform.ISteamUser_TrackAppUsageEvent(gameID.Value, eAppUsageEvent, pchExtraInfo);
		}

		public UserHasLicenseForAppResult UserHasLicenseForApp(CSteamID steamID, AppId_t appID)
		{
			return this.platform.ISteamUser_UserHasLicenseForApp(steamID.Value, appID.Value);
		}
	}
}