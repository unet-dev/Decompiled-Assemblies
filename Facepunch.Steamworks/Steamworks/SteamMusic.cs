using Steamworks.Data;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Steamworks
{
	public static class SteamMusic
	{
		private static ISteamMusic _internal;

		internal static ISteamMusic Internal
		{
			get
			{
				if (SteamMusic._internal == null)
				{
					SteamMusic._internal = new ISteamMusic();
					SteamMusic._internal.Init();
				}
				return SteamMusic._internal;
			}
		}

		public static bool IsEnabled
		{
			get
			{
				return SteamMusic.Internal.BIsEnabled();
			}
		}

		public static bool IsPlaying
		{
			get
			{
				return SteamMusic.Internal.BIsPlaying();
			}
		}

		public static MusicStatus Status
		{
			get
			{
				return SteamMusic.Internal.GetPlaybackStatus();
			}
		}

		public static float Volume
		{
			get
			{
				return SteamMusic.Internal.GetVolume();
			}
			set
			{
				SteamMusic.Internal.SetVolume(value);
			}
		}

		internal static void InstallEvents()
		{
			PlaybackStatusHasChanged_t.Install((PlaybackStatusHasChanged_t x) => {
				Action onPlaybackChanged = SteamMusic.OnPlaybackChanged;
				if (onPlaybackChanged != null)
				{
					onPlaybackChanged();
				}
				else
				{
				}
			}, false);
			VolumeHasChanged_t.Install((VolumeHasChanged_t x) => {
				Action<float> onVolumeChanged = SteamMusic.OnVolumeChanged;
				if (onVolumeChanged != null)
				{
					onVolumeChanged(x.NewVolume);
				}
				else
				{
				}
			}, false);
		}

		public static void Pause()
		{
			SteamMusic.Internal.Pause();
		}

		public static void Play()
		{
			SteamMusic.Internal.Play();
		}

		public static void PlayNext()
		{
			SteamMusic.Internal.PlayNext();
		}

		public static void PlayPrevious()
		{
			SteamMusic.Internal.PlayPrevious();
		}

		internal static void Shutdown()
		{
			SteamMusic._internal = null;
		}

		public static event Action OnPlaybackChanged;

		public static event Action<float> OnVolumeChanged;
	}
}