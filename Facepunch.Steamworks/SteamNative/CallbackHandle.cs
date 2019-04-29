using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal class CallbackHandle : IDisposable
	{
		internal BaseSteamworks Steamworks;

		internal GCHandle FuncA;

		internal GCHandle FuncB;

		internal GCHandle FuncC;

		internal IntPtr vTablePtr;

		internal GCHandle PinnedCallback;

		public virtual bool IsValid
		{
			get
			{
				return true;
			}
		}

		internal CallbackHandle(BaseSteamworks steamworks)
		{
			this.Steamworks = steamworks;
		}

		public void Dispose()
		{
			this.UnregisterCallback();
			if (this.FuncA.IsAllocated)
			{
				this.FuncA.Free();
			}
			if (this.FuncB.IsAllocated)
			{
				this.FuncB.Free();
			}
			if (this.FuncC.IsAllocated)
			{
				this.FuncC.Free();
			}
			if (this.PinnedCallback.IsAllocated)
			{
				this.PinnedCallback.Free();
			}
			if (this.vTablePtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.vTablePtr);
				this.vTablePtr = IntPtr.Zero;
			}
		}

		private void UnregisterCallback()
		{
			if (!this.PinnedCallback.IsAllocated)
			{
				return;
			}
			this.Steamworks.native.api.SteamAPI_UnregisterCallback(this.PinnedCallback.AddrOfPinnedObject());
		}
	}
}