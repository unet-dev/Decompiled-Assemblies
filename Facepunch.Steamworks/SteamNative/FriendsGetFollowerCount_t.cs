using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct FriendsGetFollowerCount_t
	{
		internal const int CallbackId = 344;

		internal SteamNative.Result Result;

		internal ulong SteamID;

		internal int Count;

		internal static CallResult<FriendsGetFollowerCount_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<FriendsGetFollowerCount_t, bool> CallbackFunction)
		{
			return new CallResult<FriendsGetFollowerCount_t>(steamworks, call, CallbackFunction, new CallResult<FriendsGetFollowerCount_t>.ConvertFromPointer(FriendsGetFollowerCount_t.FromPointer), FriendsGetFollowerCount_t.StructSize(), 344);
		}

		internal static FriendsGetFollowerCount_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (FriendsGetFollowerCount_t)Marshal.PtrToStructure(p, typeof(FriendsGetFollowerCount_t));
			}
			return (FriendsGetFollowerCount_t.PackSmall)Marshal.PtrToStructure(p, typeof(FriendsGetFollowerCount_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return FriendsGetFollowerCount_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return FriendsGetFollowerCount_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			FriendsGetFollowerCount_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			FriendsGetFollowerCount_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			FriendsGetFollowerCount_t friendsGetFollowerCountT = FriendsGetFollowerCount_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<FriendsGetFollowerCount_t>(friendsGetFollowerCountT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<FriendsGetFollowerCount_t>(friendsGetFollowerCountT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			FriendsGetFollowerCount_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(FriendsGetFollowerCount_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(FriendsGetFollowerCount_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(FriendsGetFollowerCount_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(FriendsGetFollowerCount_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(FriendsGetFollowerCount_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(FriendsGetFollowerCount_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(FriendsGetFollowerCount_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(FriendsGetFollowerCount_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(FriendsGetFollowerCount_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(FriendsGetFollowerCount_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(FriendsGetFollowerCount_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(FriendsGetFollowerCount_t.OnGetSize)
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
				CallbackId = 344
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 344);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(FriendsGetFollowerCount_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(FriendsGetFollowerCount_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong SteamID;

			internal int Count;

			public static implicit operator FriendsGetFollowerCount_t(FriendsGetFollowerCount_t.PackSmall d)
			{
				FriendsGetFollowerCount_t friendsGetFollowerCountT = new FriendsGetFollowerCount_t()
				{
					Result = d.Result,
					SteamID = d.SteamID,
					Count = d.Count
				};
				return friendsGetFollowerCountT;
			}
		}
	}
}