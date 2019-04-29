using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct P2PSessionConnectFail_t
	{
		internal const int CallbackId = 1203;

		internal ulong SteamIDRemote;

		internal byte P2PSessionError;

		internal static P2PSessionConnectFail_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (P2PSessionConnectFail_t)Marshal.PtrToStructure(p, typeof(P2PSessionConnectFail_t));
			}
			return (P2PSessionConnectFail_t.PackSmall)Marshal.PtrToStructure(p, typeof(P2PSessionConnectFail_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return P2PSessionConnectFail_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return P2PSessionConnectFail_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			P2PSessionConnectFail_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			P2PSessionConnectFail_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			P2PSessionConnectFail_t p2PSessionConnectFailT = P2PSessionConnectFail_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<P2PSessionConnectFail_t>(p2PSessionConnectFailT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<P2PSessionConnectFail_t>(p2PSessionConnectFailT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			P2PSessionConnectFail_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(P2PSessionConnectFail_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(P2PSessionConnectFail_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(P2PSessionConnectFail_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(P2PSessionConnectFail_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(P2PSessionConnectFail_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(P2PSessionConnectFail_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(P2PSessionConnectFail_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(P2PSessionConnectFail_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(P2PSessionConnectFail_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(P2PSessionConnectFail_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(P2PSessionConnectFail_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(P2PSessionConnectFail_t.OnGetSize)
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
				CallbackId = 1203
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1203);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(P2PSessionConnectFail_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(P2PSessionConnectFail_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamIDRemote;

			internal byte P2PSessionError;

			public static implicit operator P2PSessionConnectFail_t(P2PSessionConnectFail_t.PackSmall d)
			{
				P2PSessionConnectFail_t p2PSessionConnectFailT = new P2PSessionConnectFail_t()
				{
					SteamIDRemote = d.SteamIDRemote,
					P2PSessionError = d.P2PSessionError
				};
				return p2PSessionConnectFailT;
			}
		}
	}
}