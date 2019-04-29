using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct LobbyInvite_t
	{
		internal const int CallbackId = 503;

		internal ulong SteamIDUser;

		internal ulong SteamIDLobby;

		internal ulong GameID;

		internal static LobbyInvite_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (LobbyInvite_t)Marshal.PtrToStructure(p, typeof(LobbyInvite_t));
			}
			return (LobbyInvite_t.PackSmall)Marshal.PtrToStructure(p, typeof(LobbyInvite_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return LobbyInvite_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return LobbyInvite_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			LobbyInvite_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			LobbyInvite_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			LobbyInvite_t lobbyInviteT = LobbyInvite_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<LobbyInvite_t>(lobbyInviteT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<LobbyInvite_t>(lobbyInviteT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			LobbyInvite_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(LobbyInvite_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(LobbyInvite_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(LobbyInvite_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(LobbyInvite_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(LobbyInvite_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(LobbyInvite_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(LobbyInvite_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(LobbyInvite_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(LobbyInvite_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(LobbyInvite_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(LobbyInvite_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(LobbyInvite_t.OnGetSize)
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
				CallbackId = 503
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 503);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(LobbyInvite_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(LobbyInvite_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamIDUser;

			internal ulong SteamIDLobby;

			internal ulong GameID;

			public static implicit operator LobbyInvite_t(LobbyInvite_t.PackSmall d)
			{
				LobbyInvite_t lobbyInviteT = new LobbyInvite_t()
				{
					SteamIDUser = d.SteamIDUser,
					SteamIDLobby = d.SteamIDLobby,
					GameID = d.GameID
				};
				return lobbyInviteT;
			}
		}
	}
}