using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct LobbyMatchList_t
	{
		internal const int CallbackId = 510;

		internal uint LobbiesMatching;

		internal static CallResult<LobbyMatchList_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<LobbyMatchList_t, bool> CallbackFunction)
		{
			return new CallResult<LobbyMatchList_t>(steamworks, call, CallbackFunction, new CallResult<LobbyMatchList_t>.ConvertFromPointer(LobbyMatchList_t.FromPointer), LobbyMatchList_t.StructSize(), 510);
		}

		internal static LobbyMatchList_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (LobbyMatchList_t)Marshal.PtrToStructure(p, typeof(LobbyMatchList_t));
			}
			return (LobbyMatchList_t.PackSmall)Marshal.PtrToStructure(p, typeof(LobbyMatchList_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return LobbyMatchList_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return LobbyMatchList_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			LobbyMatchList_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			LobbyMatchList_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			LobbyMatchList_t lobbyMatchListT = LobbyMatchList_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<LobbyMatchList_t>(lobbyMatchListT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<LobbyMatchList_t>(lobbyMatchListT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			LobbyMatchList_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(LobbyMatchList_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(LobbyMatchList_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(LobbyMatchList_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(LobbyMatchList_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(LobbyMatchList_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(LobbyMatchList_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(LobbyMatchList_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(LobbyMatchList_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(LobbyMatchList_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(LobbyMatchList_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(LobbyMatchList_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(LobbyMatchList_t.OnGetSize)
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
				CallbackId = 510
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 510);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(LobbyMatchList_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(LobbyMatchList_t));
		}

		internal struct PackSmall
		{
			internal uint LobbiesMatching;

			public static implicit operator LobbyMatchList_t(LobbyMatchList_t.PackSmall d)
			{
				return new LobbyMatchList_t()
				{
					LobbiesMatching = d.LobbiesMatching
				};
			}
		}
	}
}