using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct FavoritesListChanged_t
	{
		internal const int CallbackId = 502;

		internal uint IP;

		internal uint QueryPort;

		internal uint ConnPort;

		internal uint AppID;

		internal uint Flags;

		internal bool Add;

		internal uint AccountId;

		internal static FavoritesListChanged_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (FavoritesListChanged_t)Marshal.PtrToStructure(p, typeof(FavoritesListChanged_t));
			}
			return (FavoritesListChanged_t.PackSmall)Marshal.PtrToStructure(p, typeof(FavoritesListChanged_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return FavoritesListChanged_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return FavoritesListChanged_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			FavoritesListChanged_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			FavoritesListChanged_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			FavoritesListChanged_t favoritesListChangedT = FavoritesListChanged_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<FavoritesListChanged_t>(favoritesListChangedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<FavoritesListChanged_t>(favoritesListChangedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			FavoritesListChanged_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(FavoritesListChanged_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(FavoritesListChanged_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(FavoritesListChanged_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(FavoritesListChanged_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(FavoritesListChanged_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(FavoritesListChanged_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(FavoritesListChanged_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(FavoritesListChanged_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(FavoritesListChanged_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(FavoritesListChanged_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(FavoritesListChanged_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(FavoritesListChanged_t.OnGetSize)
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
				CallbackId = 502
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 502);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(FavoritesListChanged_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(FavoritesListChanged_t));
		}

		internal struct PackSmall
		{
			internal uint IP;

			internal uint QueryPort;

			internal uint ConnPort;

			internal uint AppID;

			internal uint Flags;

			internal bool Add;

			internal uint AccountId;

			public static implicit operator FavoritesListChanged_t(FavoritesListChanged_t.PackSmall d)
			{
				FavoritesListChanged_t favoritesListChangedT = new FavoritesListChanged_t()
				{
					IP = d.IP,
					QueryPort = d.QueryPort,
					ConnPort = d.ConnPort,
					AppID = d.AppID,
					Flags = d.Flags,
					Add = d.Add,
					AccountId = d.AccountId
				};
				return favoritesListChangedT;
			}
		}
	}
}