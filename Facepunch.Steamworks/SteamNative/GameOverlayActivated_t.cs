using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GameOverlayActivated_t
	{
		internal const int CallbackId = 331;

		internal byte Active;

		internal static GameOverlayActivated_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GameOverlayActivated_t)Marshal.PtrToStructure(p, typeof(GameOverlayActivated_t));
			}
			return (GameOverlayActivated_t.PackSmall)Marshal.PtrToStructure(p, typeof(GameOverlayActivated_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GameOverlayActivated_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GameOverlayActivated_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GameOverlayActivated_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GameOverlayActivated_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GameOverlayActivated_t gameOverlayActivatedT = GameOverlayActivated_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GameOverlayActivated_t>(gameOverlayActivatedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GameOverlayActivated_t>(gameOverlayActivatedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GameOverlayActivated_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GameOverlayActivated_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GameOverlayActivated_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GameOverlayActivated_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GameOverlayActivated_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GameOverlayActivated_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GameOverlayActivated_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GameOverlayActivated_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GameOverlayActivated_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GameOverlayActivated_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GameOverlayActivated_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GameOverlayActivated_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GameOverlayActivated_t.OnGetSize)
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
				CallbackId = 331
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 331);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GameOverlayActivated_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GameOverlayActivated_t));
		}

		internal struct PackSmall
		{
			internal byte Active;

			public static implicit operator GameOverlayActivated_t(GameOverlayActivated_t.PackSmall d)
			{
				return new GameOverlayActivated_t()
				{
					Active = d.Active
				};
			}
		}
	}
}