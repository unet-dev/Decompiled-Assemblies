using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct ScreenshotReady_t
	{
		internal const int CallbackId = 2301;

		internal uint Local;

		internal SteamNative.Result Result;

		internal static ScreenshotReady_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (ScreenshotReady_t)Marshal.PtrToStructure(p, typeof(ScreenshotReady_t));
			}
			return (ScreenshotReady_t.PackSmall)Marshal.PtrToStructure(p, typeof(ScreenshotReady_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return ScreenshotReady_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return ScreenshotReady_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			ScreenshotReady_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			ScreenshotReady_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			ScreenshotReady_t screenshotReadyT = ScreenshotReady_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<ScreenshotReady_t>(screenshotReadyT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<ScreenshotReady_t>(screenshotReadyT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			ScreenshotReady_t.OnResultWithInfo(param, failure, call);
		}

		internal static void Register(BaseSteamworks steamworks)
		{
			CallbackHandle callbackHandle = new CallbackHandle(steamworks);
			if (Config.UseThisCall)
			{
				if (!Platform.IsWindows)
				{
					callbackHandle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Callback.VTableThis)));
					Callback.VTableThis vTableThi = new Callback.VTableThis()
					{
						ResultA = new Callback.VTableThis.ResultD(ScreenshotReady_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(ScreenshotReady_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(ScreenshotReady_t.OnGetSizeThis)
					};
					callbackHandle.FuncA = GCHandle.Alloc(vTableThi.ResultA);
					callbackHandle.FuncB = GCHandle.Alloc(vTableThi.ResultB);
					callbackHandle.FuncC = GCHandle.Alloc(vTableThi.GetSize);
					Marshal.StructureToPtr(vTableThi, callbackHandle.vTablePtr, false);
				}
				else
				{
					callbackHandle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Callback.VTableWinThis)));
					Callback.VTableWinThis vTableWinThi = new Callback.VTableWinThis()
					{
						ResultA = new Callback.VTableWinThis.ResultD(ScreenshotReady_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(ScreenshotReady_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(ScreenshotReady_t.OnGetSizeThis)
					};
					callbackHandle.FuncA = GCHandle.Alloc(vTableWinThi.ResultA);
					callbackHandle.FuncB = GCHandle.Alloc(vTableWinThi.ResultB);
					callbackHandle.FuncC = GCHandle.Alloc(vTableWinThi.GetSize);
					Marshal.StructureToPtr(vTableWinThi, callbackHandle.vTablePtr, false);
				}
			}
			else if (!Platform.IsWindows)
			{
				callbackHandle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Callback.VTable)));
				Callback.VTable vTable = new Callback.VTable()
				{
					ResultA = new Callback.VTable.ResultD(ScreenshotReady_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(ScreenshotReady_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(ScreenshotReady_t.OnGetSize)
				};
				callbackHandle.FuncA = GCHandle.Alloc(vTable.ResultA);
				callbackHandle.FuncB = GCHandle.Alloc(vTable.ResultB);
				callbackHandle.FuncC = GCHandle.Alloc(vTable.GetSize);
				Marshal.StructureToPtr(vTable, callbackHandle.vTablePtr, false);
			}
			else
			{
				callbackHandle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Callback.VTableWin)));
				Callback.VTableWin vTableWin = new Callback.VTableWin()
				{
					ResultA = new Callback.VTableWin.ResultD(ScreenshotReady_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(ScreenshotReady_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(ScreenshotReady_t.OnGetSize)
				};
				callbackHandle.FuncA = GCHandle.Alloc(vTableWin.ResultA);
				callbackHandle.FuncB = GCHandle.Alloc(vTableWin.ResultB);
				callbackHandle.FuncC = GCHandle.Alloc(vTableWin.GetSize);
				Marshal.StructureToPtr(vTableWin, callbackHandle.vTablePtr, false);
			}
			Callback callback = new Callback()
			{
				vTablePtr = callbackHandle.vTablePtr,
				CallbackFlags = (byte)((steamworks.IsGameServer ? 2 : 0)),
				CallbackId = 2301
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 2301);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(ScreenshotReady_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(ScreenshotReady_t));
		}

		internal struct PackSmall
		{
			internal uint Local;

			internal SteamNative.Result Result;

			public static implicit operator ScreenshotReady_t(ScreenshotReady_t.PackSmall d)
			{
				ScreenshotReady_t screenshotReadyT = new ScreenshotReady_t()
				{
					Local = d.Local,
					Result = d.Result
				};
				return screenshotReadyT;
			}
		}
	}
}