using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GameRichPresenceJoinRequested_t
	{
		internal const int CallbackId = 337;

		internal ulong SteamIDFriend;

		internal string Connect;

		internal static GameRichPresenceJoinRequested_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GameRichPresenceJoinRequested_t)Marshal.PtrToStructure(p, typeof(GameRichPresenceJoinRequested_t));
			}
			return (GameRichPresenceJoinRequested_t.PackSmall)Marshal.PtrToStructure(p, typeof(GameRichPresenceJoinRequested_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GameRichPresenceJoinRequested_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GameRichPresenceJoinRequested_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GameRichPresenceJoinRequested_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GameRichPresenceJoinRequested_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GameRichPresenceJoinRequested_t gameRichPresenceJoinRequestedT = GameRichPresenceJoinRequested_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GameRichPresenceJoinRequested_t>(gameRichPresenceJoinRequestedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GameRichPresenceJoinRequested_t>(gameRichPresenceJoinRequestedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GameRichPresenceJoinRequested_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GameRichPresenceJoinRequested_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GameRichPresenceJoinRequested_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GameRichPresenceJoinRequested_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GameRichPresenceJoinRequested_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GameRichPresenceJoinRequested_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GameRichPresenceJoinRequested_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GameRichPresenceJoinRequested_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GameRichPresenceJoinRequested_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GameRichPresenceJoinRequested_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GameRichPresenceJoinRequested_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GameRichPresenceJoinRequested_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GameRichPresenceJoinRequested_t.OnGetSize)
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
				CallbackId = 337
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 337);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GameRichPresenceJoinRequested_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GameRichPresenceJoinRequested_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamIDFriend;

			internal string Connect;

			public static implicit operator GameRichPresenceJoinRequested_t(GameRichPresenceJoinRequested_t.PackSmall d)
			{
				GameRichPresenceJoinRequested_t gameRichPresenceJoinRequestedT = new GameRichPresenceJoinRequested_t()
				{
					SteamIDFriend = d.SteamIDFriend,
					Connect = d.Connect
				};
				return gameRichPresenceJoinRequestedT;
			}
		}
	}
}