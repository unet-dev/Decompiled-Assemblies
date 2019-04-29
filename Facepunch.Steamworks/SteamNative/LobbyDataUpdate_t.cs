using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct LobbyDataUpdate_t
	{
		internal const int CallbackId = 505;

		internal ulong SteamIDLobby;

		internal ulong SteamIDMember;

		internal byte Success;

		internal static LobbyDataUpdate_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (LobbyDataUpdate_t)Marshal.PtrToStructure(p, typeof(LobbyDataUpdate_t));
			}
			return (LobbyDataUpdate_t.PackSmall)Marshal.PtrToStructure(p, typeof(LobbyDataUpdate_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return LobbyDataUpdate_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return LobbyDataUpdate_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			LobbyDataUpdate_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			LobbyDataUpdate_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			LobbyDataUpdate_t lobbyDataUpdateT = LobbyDataUpdate_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<LobbyDataUpdate_t>(lobbyDataUpdateT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<LobbyDataUpdate_t>(lobbyDataUpdateT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			LobbyDataUpdate_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(LobbyDataUpdate_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(LobbyDataUpdate_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(LobbyDataUpdate_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(LobbyDataUpdate_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(LobbyDataUpdate_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(LobbyDataUpdate_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(LobbyDataUpdate_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(LobbyDataUpdate_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(LobbyDataUpdate_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(LobbyDataUpdate_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(LobbyDataUpdate_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(LobbyDataUpdate_t.OnGetSize)
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
				CallbackId = 505
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 505);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(LobbyDataUpdate_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(LobbyDataUpdate_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamIDLobby;

			internal ulong SteamIDMember;

			internal byte Success;

			public static implicit operator LobbyDataUpdate_t(LobbyDataUpdate_t.PackSmall d)
			{
				LobbyDataUpdate_t lobbyDataUpdateT = new LobbyDataUpdate_t()
				{
					SteamIDLobby = d.SteamIDLobby,
					SteamIDMember = d.SteamIDMember,
					Success = d.Success
				};
				return lobbyDataUpdateT;
			}
		}
	}
}