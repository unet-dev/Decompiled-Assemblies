using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GameConnectedFriendChatMsg_t
	{
		internal const int CallbackId = 343;

		internal ulong SteamIDUser;

		internal int MessageID;

		internal static GameConnectedFriendChatMsg_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GameConnectedFriendChatMsg_t)Marshal.PtrToStructure(p, typeof(GameConnectedFriendChatMsg_t));
			}
			return (GameConnectedFriendChatMsg_t.PackSmall)Marshal.PtrToStructure(p, typeof(GameConnectedFriendChatMsg_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GameConnectedFriendChatMsg_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GameConnectedFriendChatMsg_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GameConnectedFriendChatMsg_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GameConnectedFriendChatMsg_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GameConnectedFriendChatMsg_t gameConnectedFriendChatMsgT = GameConnectedFriendChatMsg_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GameConnectedFriendChatMsg_t>(gameConnectedFriendChatMsgT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GameConnectedFriendChatMsg_t>(gameConnectedFriendChatMsgT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GameConnectedFriendChatMsg_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GameConnectedFriendChatMsg_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GameConnectedFriendChatMsg_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GameConnectedFriendChatMsg_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GameConnectedFriendChatMsg_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GameConnectedFriendChatMsg_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GameConnectedFriendChatMsg_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GameConnectedFriendChatMsg_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GameConnectedFriendChatMsg_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GameConnectedFriendChatMsg_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GameConnectedFriendChatMsg_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GameConnectedFriendChatMsg_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GameConnectedFriendChatMsg_t.OnGetSize)
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
				CallbackId = 343
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 343);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GameConnectedFriendChatMsg_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GameConnectedFriendChatMsg_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamIDUser;

			internal int MessageID;

			public static implicit operator GameConnectedFriendChatMsg_t(GameConnectedFriendChatMsg_t.PackSmall d)
			{
				GameConnectedFriendChatMsg_t gameConnectedFriendChatMsgT = new GameConnectedFriendChatMsg_t()
				{
					SteamIDUser = d.SteamIDUser,
					MessageID = d.MessageID
				};
				return gameConnectedFriendChatMsgT;
			}
		}
	}
}