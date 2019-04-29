using Facepunch.Steamworks;
using System;

namespace SteamNative
{
	internal class SteamMusic : IDisposable
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

		internal SteamMusic(BaseSteamworks steamworks, IntPtr pointer)
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

		public bool BIsEnabled()
		{
			return this.platform.ISteamMusic_BIsEnabled();
		}

		public bool BIsPlaying()
		{
			return this.platform.ISteamMusic_BIsPlaying();
		}

		public virtual void Dispose()
		{
			if (this.platform != null)
			{
				this.platform.Dispose();
				this.platform = null;
			}
		}

		public AudioPlayback_Status GetPlaybackStatus()
		{
			return this.platform.ISteamMusic_GetPlaybackStatus();
		}

		public float GetVolume()
		{
			return this.platform.ISteamMusic_GetVolume();
		}

		public void Pause()
		{
			this.platform.ISteamMusic_Pause();
		}

		public void Play()
		{
			this.platform.ISteamMusic_Play();
		}

		public void PlayNext()
		{
			this.platform.ISteamMusic_PlayNext();
		}

		public void PlayPrevious()
		{
			this.platform.ISteamMusic_PlayPrevious();
		}

		public void SetVolume(float flVolume)
		{
			this.platform.ISteamMusic_SetVolume(flVolume);
		}
	}
}