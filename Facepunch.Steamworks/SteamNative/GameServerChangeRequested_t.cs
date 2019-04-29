using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GameServerChangeRequested_t
	{
		internal const int CallbackId = 332;

		internal string Server;

		internal string Password;

		internal static GameServerChangeRequested_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GameServerChangeRequested_t)Marshal.PtrToStructure(p, typeof(GameServerChangeRequested_t));
			}
			return (GameServerChangeRequested_t.PackSmall)Marshal.PtrToStructure(p, typeof(GameServerChangeRequested_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GameServerChangeRequested_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GameServerChangeRequested_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GameServerChangeRequested_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GameServerChangeRequested_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GameServerChangeRequested_t gameServerChangeRequestedT = GameServerChangeRequested_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GameServerChangeRequested_t>(gameServerChangeRequestedT);
			}
			if (Facepunch.Steamworks.Server.Instance != null)
			{
				Facepunch.Steamworks.Server.Instance.OnCallback<GameServerChangeRequested_t>(gameServerChangeRequestedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GameServerChangeRequested_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GameServerChangeRequested_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GameServerChangeRequested_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GameServerChangeRequested_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GameServerChangeRequested_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GameServerChangeRequested_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GameServerChangeRequested_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GameServerChangeRequested_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GameServerChangeRequested_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GameServerChangeRequested_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GameServerChangeRequested_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GameServerChangeRequested_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GameServerChangeRequested_t.OnGetSize)
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
				CallbackId = 332
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 332);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GameServerChangeRequested_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GameServerChangeRequested_t));
		}

		internal struct PackSmall
		{
			internal string Server;

			internal string Password;

			public static implicit operator GameServerChangeRequested_t(GameServerChangeRequested_t.PackSmall d)
			{
				GameServerChangeRequested_t gameServerChangeRequestedT = new GameServerChangeRequested_t()
				{
					Server = d.Server,
					Password = d.Password
				};
				return gameServerChangeRequestedT;
			}
		}
	}
}