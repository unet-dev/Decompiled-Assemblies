using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GameConnectedClanChatMsg_t
	{
		internal const int CallbackId = 338;

		internal ulong SteamIDClanChat;

		internal ulong SteamIDUser;

		internal int MessageID;

		internal static GameConnectedClanChatMsg_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GameConnectedClanChatMsg_t)Marshal.PtrToStructure(p, typeof(GameConnectedClanChatMsg_t));
			}
			return (GameConnectedClanChatMsg_t.PackSmall)Marshal.PtrToStructure(p, typeof(GameConnectedClanChatMsg_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GameConnectedClanChatMsg_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GameConnectedClanChatMsg_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GameConnectedClanChatMsg_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GameConnectedClanChatMsg_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GameConnectedClanChatMsg_t gameConnectedClanChatMsgT = GameConnectedClanChatMsg_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GameConnectedClanChatMsg_t>(gameConnectedClanChatMsgT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GameConnectedClanChatMsg_t>(gameConnectedClanChatMsgT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GameConnectedClanChatMsg_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GameConnectedClanChatMsg_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GameConnectedClanChatMsg_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GameConnectedClanChatMsg_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GameConnectedClanChatMsg_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GameConnectedClanChatMsg_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GameConnectedClanChatMsg_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GameConnectedClanChatMsg_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GameConnectedClanChatMsg_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GameConnectedClanChatMsg_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GameConnectedClanChatMsg_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GameConnectedClanChatMsg_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GameConnectedClanChatMsg_t.OnGetSize)
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
				CallbackId = 338
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 338);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GameConnectedClanChatMsg_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GameConnectedClanChatMsg_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamIDClanChat;

			internal ulong SteamIDUser;

			internal int MessageID;

			public static implicit operator GameConnectedClanChatMsg_t(GameConnectedClanChatMsg_t.PackSmall d)
			{
				GameConnectedClanChatMsg_t gameConnectedClanChatMsgT = new GameConnectedClanChatMsg_t()
				{
					SteamIDClanChat = d.SteamIDClanChat,
					SteamIDUser = d.SteamIDUser,
					MessageID = d.MessageID
				};
				return gameConnectedClanChatMsgT;
			}
		}
	}
}