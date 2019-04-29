using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct ScreenshotRequested_t
	{
		internal const int CallbackId = 2302;

		internal static ScreenshotRequested_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (ScreenshotRequested_t)Marshal.PtrToStructure(p, typeof(ScreenshotRequested_t));
			}
			return (ScreenshotRequested_t.PackSmall)Marshal.PtrToStructure(p, typeof(ScreenshotRequested_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return ScreenshotRequested_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return ScreenshotRequested_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			ScreenshotRequested_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			ScreenshotRequested_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			ScreenshotRequested_t screenshotRequestedT = ScreenshotRequested_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<ScreenshotRequested_t>(screenshotRequestedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<ScreenshotRequested_t>(screenshotRequestedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			ScreenshotRequested_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(ScreenshotRequested_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(ScreenshotRequested_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(ScreenshotRequested_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(ScreenshotRequested_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(ScreenshotRequested_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(ScreenshotRequested_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(ScreenshotRequested_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(ScreenshotRequested_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(ScreenshotRequested_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(ScreenshotRequested_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(ScreenshotRequested_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(ScreenshotRequested_t.OnGetSize)
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
				CallbackId = 2302
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 2302);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(ScreenshotRequested_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(ScreenshotRequested_t));
		}

		internal struct PackSmall
		{
			public static implicit operator ScreenshotRequested_t(ScreenshotRequested_t.PackSmall d)
			{
				return new ScreenshotRequested_t();
			}
		}
	}
}