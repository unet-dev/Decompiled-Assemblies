using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct FavoritesListAccountsUpdated_t
	{
		internal const int CallbackId = 516;

		internal SteamNative.Result Result;

		internal static FavoritesListAccountsUpdated_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (FavoritesListAccountsUpdated_t)Marshal.PtrToStructure(p, typeof(FavoritesListAccountsUpdated_t));
			}
			return (FavoritesListAccountsUpdated_t.PackSmall)Marshal.PtrToStructure(p, typeof(FavoritesListAccountsUpdated_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return FavoritesListAccountsUpdated_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return FavoritesListAccountsUpdated_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			FavoritesListAccountsUpdated_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			FavoritesListAccountsUpdated_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			FavoritesListAccountsUpdated_t favoritesListAccountsUpdatedT = FavoritesListAccountsUpdated_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<FavoritesListAccountsUpdated_t>(favoritesListAccountsUpdatedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<FavoritesListAccountsUpdated_t>(favoritesListAccountsUpdatedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			FavoritesListAccountsUpdated_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(FavoritesListAccountsUpdated_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(FavoritesListAccountsUpdated_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(FavoritesListAccountsUpdated_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(FavoritesListAccountsUpdated_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(FavoritesListAccountsUpdated_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(FavoritesListAccountsUpdated_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(FavoritesListAccountsUpdated_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(FavoritesListAccountsUpdated_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(FavoritesListAccountsUpdated_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(FavoritesListAccountsUpdated_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(FavoritesListAccountsUpdated_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(FavoritesListAccountsUpdated_t.OnGetSize)
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
				CallbackId = 516
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 516);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(FavoritesListAccountsUpdated_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(FavoritesListAccountsUpdated_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			public static implicit operator FavoritesListAccountsUpdated_t(FavoritesListAccountsUpdated_t.PackSmall d)
			{
				return new FavoritesListAccountsUpdated_t()
				{
					Result = d.Result
				};
			}
		}
	}
}