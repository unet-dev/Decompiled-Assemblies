using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct UserFavoriteItemsListChanged_t
	{
		internal const int CallbackId = 3407;

		internal ulong PublishedFileId;

		internal SteamNative.Result Result;

		internal bool WasAddRequest;

		internal static CallResult<UserFavoriteItemsListChanged_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<UserFavoriteItemsListChanged_t, bool> CallbackFunction)
		{
			return new CallResult<UserFavoriteItemsListChanged_t>(steamworks, call, CallbackFunction, new CallResult<UserFavoriteItemsListChanged_t>.ConvertFromPointer(UserFavoriteItemsListChanged_t.FromPointer), UserFavoriteItemsListChanged_t.StructSize(), 3407);
		}

		internal static UserFavoriteItemsListChanged_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (UserFavoriteItemsListChanged_t)Marshal.PtrToStructure(p, typeof(UserFavoriteItemsListChanged_t));
			}
			return (UserFavoriteItemsListChanged_t.PackSmall)Marshal.PtrToStructure(p, typeof(UserFavoriteItemsListChanged_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return UserFavoriteItemsListChanged_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return UserFavoriteItemsListChanged_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			UserFavoriteItemsListChanged_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			UserFavoriteItemsListChanged_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			UserFavoriteItemsListChanged_t userFavoriteItemsListChangedT = UserFavoriteItemsListChanged_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<UserFavoriteItemsListChanged_t>(userFavoriteItemsListChangedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<UserFavoriteItemsListChanged_t>(userFavoriteItemsListChangedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			UserFavoriteItemsListChanged_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(UserFavoriteItemsListChanged_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(UserFavoriteItemsListChanged_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(UserFavoriteItemsListChanged_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(UserFavoriteItemsListChanged_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(UserFavoriteItemsListChanged_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(UserFavoriteItemsListChanged_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(UserFavoriteItemsListChanged_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(UserFavoriteItemsListChanged_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(UserFavoriteItemsListChanged_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(UserFavoriteItemsListChanged_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(UserFavoriteItemsListChanged_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(UserFavoriteItemsListChanged_t.OnGetSize)
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
				CallbackId = 3407
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 3407);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(UserFavoriteItemsListChanged_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(UserFavoriteItemsListChanged_t));
		}

		internal struct PackSmall
		{
			internal ulong PublishedFileId;

			internal SteamNative.Result Result;

			internal bool WasAddRequest;

			public static implicit operator UserFavoriteItemsListChanged_t(UserFavoriteItemsListChanged_t.PackSmall d)
			{
				UserFavoriteItemsListChanged_t userFavoriteItemsListChangedT = new UserFavoriteItemsListChanged_t()
				{
					PublishedFileId = d.PublishedFileId,
					Result = d.Result,
					WasAddRequest = d.WasAddRequest
				};
				return userFavoriteItemsListChangedT;
			}
		}
	}
}