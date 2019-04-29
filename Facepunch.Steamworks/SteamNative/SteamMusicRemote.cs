using Facepunch.Steamworks;
using System;

namespace SteamNative
{
	internal class SteamMusicRemote : IDisposable
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

		internal SteamMusicRemote(BaseSteamworks steamworks, IntPtr pointer)
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

		public bool BActivationSuccess(bool bValue)
		{
			return this.platform.ISteamMusicRemote_BActivationSuccess(bValue);
		}

		public bool BIsCurrentMusicRemote()
		{
			return this.platform.ISteamMusicRemote_BIsCurrentMusicRemote();
		}

		public bool CurrentEntryDidChange()
		{
			return this.platform.ISteamMusicRemote_CurrentEntryDidChange();
		}

		public bool CurrentEntryIsAvailable(bool bAvailable)
		{
			return this.platform.ISteamMusicRemote_CurrentEntryIsAvailable(bAvailable);
		}

		public bool CurrentEntryWillChange()
		{
			return this.platform.ISteamMusicRemote_CurrentEntryWillChange();
		}

		public bool DeregisterSteamMusicRemote()
		{
			return this.platform.ISteamMusicRemote_DeregisterSteamMusicRemote();
		}

		public virtual void Dispose()
		{
			if (this.platform != null)
			{
				this.platform.Dispose();
				this.platform = null;
			}
		}

		public bool EnableLooped(bool bValue)
		{
			return this.platform.ISteamMusicRemote_EnableLooped(bValue);
		}

		public bool EnablePlaylists(bool bValue)
		{
			return this.platform.ISteamMusicRemote_EnablePlaylists(bValue);
		}

		public bool EnablePlayNext(bool bValue)
		{
			return this.platform.ISteamMusicRemote_EnablePlayNext(bValue);
		}

		public bool EnablePlayPrevious(bool bValue)
		{
			return this.platform.ISteamMusicRemote_EnablePlayPrevious(bValue);
		}

		public bool EnableQueue(bool bValue)
		{
			return this.platform.ISteamMusicRemote_EnableQueue(bValue);
		}

		public bool EnableShuffled(bool bValue)
		{
			return this.platform.ISteamMusicRemote_EnableShuffled(bValue);
		}

		public bool PlaylistDidChange()
		{
			return this.platform.ISteamMusicRemote_PlaylistDidChange();
		}

		public bool PlaylistWillChange()
		{
			return this.platform.ISteamMusicRemote_PlaylistWillChange();
		}

		public bool QueueDidChange()
		{
			return this.platform.ISteamMusicRemote_QueueDidChange();
		}

		public bool QueueWillChange()
		{
			return this.platform.ISteamMusicRemote_QueueWillChange();
		}

		public bool RegisterSteamMusicRemote(string pchName)
		{
			return this.platform.ISteamMusicRemote_RegisterSteamMusicRemote(pchName);
		}

		public bool ResetPlaylistEntries()
		{
			return this.platform.ISteamMusicRemote_ResetPlaylistEntries();
		}

		public bool ResetQueueEntries()
		{
			return this.platform.ISteamMusicRemote_ResetQueueEntries();
		}

		public bool SetCurrentPlaylistEntry(int nID)
		{
			return this.platform.ISteamMusicRemote_SetCurrentPlaylistEntry(nID);
		}

		public bool SetCurrentQueueEntry(int nID)
		{
			return this.platform.ISteamMusicRemote_SetCurrentQueueEntry(nID);
		}

		public bool SetDisplayName(string pchDisplayName)
		{
			return this.platform.ISteamMusicRemote_SetDisplayName(pchDisplayName);
		}

		public bool SetPlaylistEntry(int nID, int nPosition, string pchEntryText)
		{
			return this.platform.ISteamMusicRemote_SetPlaylistEntry(nID, nPosition, pchEntryText);
		}

		public bool SetPNGIcon_64x64(IntPtr pvBuffer, uint cbBufferLength)
		{
			return this.platform.ISteamMusicRemote_SetPNGIcon_64x64(pvBuffer, cbBufferLength);
		}

		public bool SetQueueEntry(int nID, int nPosition, string pchEntryText)
		{
			return this.platform.ISteamMusicRemote_SetQueueEntry(nID, nPosition, pchEntryText);
		}

		public bool UpdateCurrentEntryCoverArt(IntPtr pvBuffer, uint cbBufferLength)
		{
			return this.platform.ISteamMusicRemote_UpdateCurrentEntryCoverArt(pvBuffer, cbBufferLength);
		}

		public bool UpdateCurrentEntryElapsedSeconds(int nValue)
		{
			return this.platform.ISteamMusicRemote_UpdateCurrentEntryElapsedSeconds(nValue);
		}

		public bool UpdateCurrentEntryText(string pchText)
		{
			return this.platform.ISteamMusicRemote_UpdateCurrentEntryText(pchText);
		}

		public bool UpdateLooped(bool bValue)
		{
			return this.platform.ISteamMusicRemote_UpdateLooped(bValue);
		}

		public bool UpdatePlaybackStatus(AudioPlayback_Status nStatus)
		{
			return this.platform.ISteamMusicRemote_UpdatePlaybackStatus(nStatus);
		}

		public bool UpdateShuffled(bool bValue)
		{
			return this.platform.ISteamMusicRemote_UpdateShuffled(bValue);
		}

		public bool UpdateVolume(float flValue)
		{
			return this.platform.ISteamMusicRemote_UpdateVolume(flValue);
		}
	}
}