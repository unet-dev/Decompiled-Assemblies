using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct SteamServerConnectFailure_t
	{
		internal const int CallbackId = 102;

		internal SteamNative.Result Result;

		internal bool StillRetrying;

		internal static SteamServerConnectFailure_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (SteamServerConnectFailure_t)Marshal.PtrToStructure(p, typeof(SteamServerConnectFailure_t));
			}
			return (SteamServerConnectFailure_t.PackSmall)Marshal.PtrToStructure(p, typeof(SteamServerConnectFailure_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return SteamServerConnectFailure_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return SteamServerConnectFailure_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			SteamServerConnectFailure_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			SteamServerConnectFailure_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			SteamServerConnectFailure_t steamServerConnectFailureT = SteamServerConnectFailure_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<SteamServerConnectFailure_t>(steamServerConnectFailureT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<SteamServerConnectFailure_t>(steamServerConnectFailureT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			SteamServerConnectFailure_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(SteamServerConnectFailure_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(SteamServerConnectFailure_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(SteamServerConnectFailure_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(SteamServerConnectFailure_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(SteamServerConnectFailure_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(SteamServerConnectFailure_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(SteamServerConnectFailure_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(SteamServerConnectFailure_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(SteamServerConnectFailure_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(SteamServerConnectFailure_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(SteamServerConnectFailure_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(SteamServerConnectFailure_t.OnGetSize)
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
				CallbackId = 102
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 102);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(SteamServerConnectFailure_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(SteamServerConnectFailure_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal bool StillRetrying;

			public static implicit operator SteamServerConnectFailure_t(SteamServerConnectFailure_t.PackSmall d)
			{
				SteamServerConnectFailure_t steamServerConnectFailureT = new SteamServerConnectFailure_t()
				{
					Result = d.Result,
					StillRetrying = d.StillRetrying
				};
				return steamServerConnectFailureT;
			}
		}
	}
}