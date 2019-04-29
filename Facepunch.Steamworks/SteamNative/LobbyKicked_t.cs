using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct LobbyKicked_t
	{
		internal const int CallbackId = 512;

		internal ulong SteamIDLobby;

		internal ulong SteamIDAdmin;

		internal byte KickedDueToDisconnect;

		internal static LobbyKicked_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (LobbyKicked_t)Marshal.PtrToStructure(p, typeof(LobbyKicked_t));
			}
			return (LobbyKicked_t.PackSmall)Marshal.PtrToStructure(p, typeof(LobbyKicked_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return LobbyKicked_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return LobbyKicked_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			LobbyKicked_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			LobbyKicked_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			LobbyKicked_t lobbyKickedT = LobbyKicked_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<LobbyKicked_t>(lobbyKickedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<LobbyKicked_t>(lobbyKickedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			LobbyKicked_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(LobbyKicked_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(LobbyKicked_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(LobbyKicked_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(LobbyKicked_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(LobbyKicked_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(LobbyKicked_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(LobbyKicked_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(LobbyKicked_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(LobbyKicked_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(LobbyKicked_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(LobbyKicked_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(LobbyKicked_t.OnGetSize)
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
				CallbackId = 512
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 512);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(LobbyKicked_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(LobbyKicked_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamIDLobby;

			internal ulong SteamIDAdmin;

			internal byte KickedDueToDisconnect;

			public static implicit operator LobbyKicked_t(LobbyKicked_t.PackSmall d)
			{
				LobbyKicked_t lobbyKickedT = new LobbyKicked_t()
				{
					SteamIDLobby = d.SteamIDLobby,
					SteamIDAdmin = d.SteamIDAdmin,
					KickedDueToDisconnect = d.KickedDueToDisconnect
				};
				return lobbyKickedT;
			}
		}
	}
}