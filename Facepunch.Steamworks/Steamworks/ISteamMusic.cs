using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	internal class ISteamMusic : SteamInterface
	{
		private ISteamMusic.FBIsEnabled _BIsEnabled;

		private ISteamMusic.FBIsPlaying _BIsPlaying;

		private ISteamMusic.FGetPlaybackStatus _GetPlaybackStatus;

		private ISteamMusic.FPlay _Play;

		private ISteamMusic.FPause _Pause;

		private ISteamMusic.FPlayPrevious _PlayPrevious;

		private ISteamMusic.FPlayNext _PlayNext;

		private ISteamMusic.FSetVolume _SetVolume;

		private ISteamMusic.FGetVolume _GetVolume;

		public override string InterfaceName
		{
			get
			{
				return "STEAMMUSIC_INTERFACE_VERSION001";
			}
		}

		public ISteamMusic()
		{
		}

		internal bool BIsEnabled()
		{
			return this._BIsEnabled(this.Self);
		}

		internal bool BIsPlaying()
		{
			return this._BIsPlaying(this.Self);
		}

		internal MusicStatus GetPlaybackStatus()
		{
			return this._GetPlaybackStatus(this.Self);
		}

		internal float GetVolume()
		{
			return this._GetVolume(this.Self);
		}

		public override void InitInternals()
		{
			this._BIsEnabled = Marshal.GetDelegateForFunctionPointer<ISteamMusic.FBIsEnabled>(Marshal.ReadIntPtr(this.VTable, 0));
			this._BIsPlaying = Marshal.GetDelegateForFunctionPointer<ISteamMusic.FBIsPlaying>(Marshal.ReadIntPtr(this.VTable, 8));
			this._GetPlaybackStatus = Marshal.GetDelegateForFunctionPointer<ISteamMusic.FGetPlaybackStatus>(Marshal.ReadIntPtr(this.VTable, 16));
			this._Play = Marshal.GetDelegateForFunctionPointer<ISteamMusic.FPlay>(Marshal.ReadIntPtr(this.VTable, 24));
			this._Pause = Marshal.GetDelegateForFunctionPointer<ISteamMusic.FPause>(Marshal.ReadIntPtr(this.VTable, 32));
			this._PlayPrevious = Marshal.GetDelegateForFunctionPointer<ISteamMusic.FPlayPrevious>(Marshal.ReadIntPtr(this.VTable, 40));
			this._PlayNext = Marshal.GetDelegateForFunctionPointer<ISteamMusic.FPlayNext>(Marshal.ReadIntPtr(this.VTable, 48));
			this._SetVolume = Marshal.GetDelegateForFunctionPointer<ISteamMusic.FSetVolume>(Marshal.ReadIntPtr(this.VTable, 56));
			this._GetVolume = Marshal.GetDelegateForFunctionPointer<ISteamMusic.FGetVolume>(Marshal.ReadIntPtr(this.VTable, 64));
		}

		internal void Pause()
		{
			this._Pause(this.Self);
		}

		internal void Play()
		{
			this._Play(this.Self);
		}

		internal void PlayNext()
		{
			this._PlayNext(this.Self);
		}

		internal void PlayPrevious()
		{
			this._PlayPrevious(this.Self);
		}

		internal void SetVolume(float flVolume)
		{
			this._SetVolume(this.Self, flVolume);
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._BIsEnabled = null;
			this._BIsPlaying = null;
			this._GetPlaybackStatus = null;
			this._Play = null;
			this._Pause = null;
			this._PlayPrevious = null;
			this._PlayNext = null;
			this._SetVolume = null;
			this._GetVolume = null;
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsEnabled(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsPlaying(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate MusicStatus FGetPlaybackStatus(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate float FGetVolume(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FPause(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FPlay(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FPlayNext(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FPlayPrevious(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetVolume(IntPtr self, float flVolume);
	}
}