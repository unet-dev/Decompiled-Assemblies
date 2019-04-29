using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct LobbyEnter_t
	{
		internal const int CallbackId = 504;

		internal ulong SteamIDLobby;

		internal uint GfChatPermissions;

		internal bool Locked;

		internal uint EChatRoomEnterResponse;

		internal static CallResult<LobbyEnter_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<LobbyEnter_t, bool> CallbackFunction)
		{
			return new CallResult<LobbyEnter_t>(steamworks, call, CallbackFunction, new CallResult<LobbyEnter_t>.ConvertFromPointer(LobbyEnter_t.FromPointer), LobbyEnter_t.StructSize(), 504);
		}

		internal static LobbyEnter_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (LobbyEnter_t)Marshal.PtrToStructure(p, typeof(LobbyEnter_t));
			}
			return (LobbyEnter_t.PackSmall)Marshal.PtrToStructure(p, typeof(LobbyEnter_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return LobbyEnter_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return LobbyEnter_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			LobbyEnter_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			LobbyEnter_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			LobbyEnter_t lobbyEnterT = LobbyEnter_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<LobbyEnter_t>(lobbyEnterT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<LobbyEnter_t>(lobbyEnterT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			LobbyEnter_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(LobbyEnter_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(LobbyEnter_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(LobbyEnter_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(LobbyEnter_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(LobbyEnter_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(LobbyEnter_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(LobbyEnter_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(LobbyEnter_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(LobbyEnter_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(LobbyEnter_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(LobbyEnter_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(LobbyEnter_t.OnGetSize)
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
				CallbackId = 504
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 504);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(LobbyEnter_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(LobbyEnter_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamIDLobby;

			internal uint GfChatPermissions;

			internal bool Locked;

			internal uint EChatRoomEnterResponse;

			public static implicit operator LobbyEnter_t(LobbyEnter_t.PackSmall d)
			{
				LobbyEnter_t lobbyEnterT = new LobbyEnter_t()
				{
					SteamIDLobby = d.SteamIDLobby,
					GfChatPermissions = d.GfChatPermissions,
					Locked = d.Locked,
					EChatRoomEnterResponse = d.EChatRoomEnterResponse
				};
				return lobbyEnterT;
			}
		}
	}
}