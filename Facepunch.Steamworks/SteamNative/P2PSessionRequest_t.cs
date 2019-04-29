using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct P2PSessionRequest_t
	{
		internal const int CallbackId = 1202;

		internal ulong SteamIDRemote;

		internal static P2PSessionRequest_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (P2PSessionRequest_t)Marshal.PtrToStructure(p, typeof(P2PSessionRequest_t));
			}
			return (P2PSessionRequest_t.PackSmall)Marshal.PtrToStructure(p, typeof(P2PSessionRequest_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return P2PSessionRequest_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return P2PSessionRequest_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			P2PSessionRequest_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			P2PSessionRequest_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			P2PSessionRequest_t p2PSessionRequestT = P2PSessionRequest_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<P2PSessionRequest_t>(p2PSessionRequestT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<P2PSessionRequest_t>(p2PSessionRequestT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			P2PSessionRequest_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(P2PSessionRequest_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(P2PSessionRequest_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(P2PSessionRequest_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(P2PSessionRequest_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(P2PSessionRequest_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(P2PSessionRequest_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(P2PSessionRequest_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(P2PSessionRequest_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(P2PSessionRequest_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(P2PSessionRequest_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(P2PSessionRequest_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(P2PSessionRequest_t.OnGetSize)
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
				CallbackId = 1202
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1202);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(P2PSessionRequest_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(P2PSessionRequest_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamIDRemote;

			public static implicit operator P2PSessionRequest_t(P2PSessionRequest_t.PackSmall d)
			{
				return new P2PSessionRequest_t()
				{
					SteamIDRemote = d.SteamIDRemote
				};
			}
		}
	}
}