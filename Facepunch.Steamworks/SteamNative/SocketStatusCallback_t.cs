using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct SocketStatusCallback_t
	{
		internal const int CallbackId = 1201;

		internal uint Socket;

		internal uint ListenSocket;

		internal ulong SteamIDRemote;

		internal int SNetSocketState;

		internal static SocketStatusCallback_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (SocketStatusCallback_t)Marshal.PtrToStructure(p, typeof(SocketStatusCallback_t));
			}
			return (SocketStatusCallback_t.PackSmall)Marshal.PtrToStructure(p, typeof(SocketStatusCallback_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return SocketStatusCallback_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return SocketStatusCallback_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			SocketStatusCallback_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			SocketStatusCallback_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			SocketStatusCallback_t socketStatusCallbackT = SocketStatusCallback_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<SocketStatusCallback_t>(socketStatusCallbackT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<SocketStatusCallback_t>(socketStatusCallbackT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			SocketStatusCallback_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(SocketStatusCallback_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(SocketStatusCallback_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(SocketStatusCallback_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(SocketStatusCallback_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(SocketStatusCallback_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(SocketStatusCallback_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(SocketStatusCallback_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(SocketStatusCallback_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(SocketStatusCallback_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(SocketStatusCallback_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(SocketStatusCallback_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(SocketStatusCallback_t.OnGetSize)
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
				CallbackId = 1201
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1201);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(SocketStatusCallback_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(SocketStatusCallback_t));
		}

		internal struct PackSmall
		{
			internal uint Socket;

			internal uint ListenSocket;

			internal ulong SteamIDRemote;

			internal int SNetSocketState;

			public static implicit operator SocketStatusCallback_t(SocketStatusCallback_t.PackSmall d)
			{
				SocketStatusCallback_t socketStatusCallbackT = new SocketStatusCallback_t()
				{
					Socket = d.Socket,
					ListenSocket = d.ListenSocket,
					SteamIDRemote = d.SteamIDRemote,
					SNetSocketState = d.SNetSocketState
				};
				return socketStatusCallbackT;
			}
		}
	}
}