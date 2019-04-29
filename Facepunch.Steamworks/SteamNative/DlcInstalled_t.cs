using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct DlcInstalled_t
	{
		internal const int CallbackId = 1005;

		internal uint AppID;

		internal static DlcInstalled_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (DlcInstalled_t)Marshal.PtrToStructure(p, typeof(DlcInstalled_t));
			}
			return (DlcInstalled_t.PackSmall)Marshal.PtrToStructure(p, typeof(DlcInstalled_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return DlcInstalled_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return DlcInstalled_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			DlcInstalled_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			DlcInstalled_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			DlcInstalled_t dlcInstalledT = DlcInstalled_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<DlcInstalled_t>(dlcInstalledT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<DlcInstalled_t>(dlcInstalledT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			DlcInstalled_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(DlcInstalled_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(DlcInstalled_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(DlcInstalled_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(DlcInstalled_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(DlcInstalled_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(DlcInstalled_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(DlcInstalled_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(DlcInstalled_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(DlcInstalled_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(DlcInstalled_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(DlcInstalled_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(DlcInstalled_t.OnGetSize)
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
				CallbackId = 1005
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1005);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(DlcInstalled_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(DlcInstalled_t));
		}

		internal struct PackSmall
		{
			internal uint AppID;

			public static implicit operator DlcInstalled_t(DlcInstalled_t.PackSmall d)
			{
				return new DlcInstalled_t()
				{
					AppID = d.AppID
				};
			}
		}
	}
}