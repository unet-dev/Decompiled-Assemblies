using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GameConnectedChatJoin_t
	{
		internal const int CallbackId = 339;

		internal ulong SteamIDClanChat;

		internal ulong SteamIDUser;

		internal static GameConnectedChatJoin_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GameConnectedChatJoin_t)Marshal.PtrToStructure(p, typeof(GameConnectedChatJoin_t));
			}
			return (GameConnectedChatJoin_t.PackSmall)Marshal.PtrToStructure(p, typeof(GameConnectedChatJoin_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GameConnectedChatJoin_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GameConnectedChatJoin_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GameConnectedChatJoin_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GameConnectedChatJoin_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GameConnectedChatJoin_t gameConnectedChatJoinT = GameConnectedChatJoin_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GameConnectedChatJoin_t>(gameConnectedChatJoinT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GameConnectedChatJoin_t>(gameConnectedChatJoinT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GameConnectedChatJoin_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GameConnectedChatJoin_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GameConnectedChatJoin_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GameConnectedChatJoin_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GameConnectedChatJoin_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GameConnectedChatJoin_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GameConnectedChatJoin_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GameConnectedChatJoin_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GameConnectedChatJoin_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GameConnectedChatJoin_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GameConnectedChatJoin_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GameConnectedChatJoin_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GameConnectedChatJoin_t.OnGetSize)
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
				CallbackId = 339
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 339);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GameConnectedChatJoin_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GameConnectedChatJoin_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamIDClanChat;

			internal ulong SteamIDUser;

			public static implicit operator GameConnectedChatJoin_t(GameConnectedChatJoin_t.PackSmall d)
			{
				GameConnectedChatJoin_t gameConnectedChatJoinT = new GameConnectedChatJoin_t()
				{
					SteamIDClanChat = d.SteamIDClanChat,
					SteamIDUser = d.SteamIDUser
				};
				return gameConnectedChatJoinT;
			}
		}
	}
}