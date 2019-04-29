using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GameLobbyJoinRequested_t
	{
		internal const int CallbackId = 333;

		internal ulong SteamIDLobby;

		internal ulong SteamIDFriend;

		internal static GameLobbyJoinRequested_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GameLobbyJoinRequested_t)Marshal.PtrToStructure(p, typeof(GameLobbyJoinRequested_t));
			}
			return (GameLobbyJoinRequested_t.PackSmall)Marshal.PtrToStructure(p, typeof(GameLobbyJoinRequested_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GameLobbyJoinRequested_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GameLobbyJoinRequested_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GameLobbyJoinRequested_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GameLobbyJoinRequested_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GameLobbyJoinRequested_t gameLobbyJoinRequestedT = GameLobbyJoinRequested_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GameLobbyJoinRequested_t>(gameLobbyJoinRequestedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GameLobbyJoinRequested_t>(gameLobbyJoinRequestedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GameLobbyJoinRequested_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GameLobbyJoinRequested_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GameLobbyJoinRequested_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GameLobbyJoinRequested_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GameLobbyJoinRequested_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GameLobbyJoinRequested_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GameLobbyJoinRequested_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GameLobbyJoinRequested_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GameLobbyJoinRequested_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GameLobbyJoinRequested_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GameLobbyJoinRequested_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GameLobbyJoinRequested_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GameLobbyJoinRequested_t.OnGetSize)
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
				CallbackId = 333
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 333);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GameLobbyJoinRequested_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GameLobbyJoinRequested_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamIDLobby;

			internal ulong SteamIDFriend;

			public static implicit operator GameLobbyJoinRequested_t(GameLobbyJoinRequested_t.PackSmall d)
			{
				GameLobbyJoinRequested_t gameLobbyJoinRequestedT = new GameLobbyJoinRequested_t()
				{
					SteamIDLobby = d.SteamIDLobby,
					SteamIDFriend = d.SteamIDFriend
				};
				return gameLobbyJoinRequestedT;
			}
		}
	}
}