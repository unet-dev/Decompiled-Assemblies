using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct LobbyCreated_t
	{
		internal const int CallbackId = 513;

		internal SteamNative.Result Result;

		internal ulong SteamIDLobby;

		internal static CallResult<LobbyCreated_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<LobbyCreated_t, bool> CallbackFunction)
		{
			return new CallResult<LobbyCreated_t>(steamworks, call, CallbackFunction, new CallResult<LobbyCreated_t>.ConvertFromPointer(LobbyCreated_t.FromPointer), LobbyCreated_t.StructSize(), 513);
		}

		internal static LobbyCreated_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (LobbyCreated_t)Marshal.PtrToStructure(p, typeof(LobbyCreated_t));
			}
			return (LobbyCreated_t.PackSmall)Marshal.PtrToStructure(p, typeof(LobbyCreated_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return LobbyCreated_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return LobbyCreated_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			LobbyCreated_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			LobbyCreated_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			LobbyCreated_t lobbyCreatedT = LobbyCreated_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<LobbyCreated_t>(lobbyCreatedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<LobbyCreated_t>(lobbyCreatedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			LobbyCreated_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(LobbyCreated_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(LobbyCreated_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(LobbyCreated_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(LobbyCreated_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(LobbyCreated_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(LobbyCreated_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(LobbyCreated_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(LobbyCreated_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(LobbyCreated_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(LobbyCreated_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(LobbyCreated_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(LobbyCreated_t.OnGetSize)
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
				CallbackId = 513
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 513);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(LobbyCreated_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(LobbyCreated_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong SteamIDLobby;

			public static implicit operator LobbyCreated_t(LobbyCreated_t.PackSmall d)
			{
				LobbyCreated_t lobbyCreatedT = new LobbyCreated_t()
				{
					Result = d.Result,
					SteamIDLobby = d.SteamIDLobby
				};
				return lobbyCreatedT;
			}
		}
	}
}