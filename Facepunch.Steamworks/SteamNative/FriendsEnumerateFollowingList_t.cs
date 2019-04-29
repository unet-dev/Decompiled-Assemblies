using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct FriendsEnumerateFollowingList_t
	{
		internal const int CallbackId = 346;

		internal SteamNative.Result Result;

		internal ulong[] GSteamID;

		internal int ResultsReturned;

		internal int TotalResultCount;

		internal static CallResult<FriendsEnumerateFollowingList_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<FriendsEnumerateFollowingList_t, bool> CallbackFunction)
		{
			return new CallResult<FriendsEnumerateFollowingList_t>(steamworks, call, CallbackFunction, new CallResult<FriendsEnumerateFollowingList_t>.ConvertFromPointer(FriendsEnumerateFollowingList_t.FromPointer), FriendsEnumerateFollowingList_t.StructSize(), 346);
		}

		internal static FriendsEnumerateFollowingList_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (FriendsEnumerateFollowingList_t)Marshal.PtrToStructure(p, typeof(FriendsEnumerateFollowingList_t));
			}
			return (FriendsEnumerateFollowingList_t.PackSmall)Marshal.PtrToStructure(p, typeof(FriendsEnumerateFollowingList_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return FriendsEnumerateFollowingList_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return FriendsEnumerateFollowingList_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			FriendsEnumerateFollowingList_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			FriendsEnumerateFollowingList_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			FriendsEnumerateFollowingList_t friendsEnumerateFollowingListT = FriendsEnumerateFollowingList_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<FriendsEnumerateFollowingList_t>(friendsEnumerateFollowingListT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<FriendsEnumerateFollowingList_t>(friendsEnumerateFollowingListT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			FriendsEnumerateFollowingList_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(FriendsEnumerateFollowingList_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(FriendsEnumerateFollowingList_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(FriendsEnumerateFollowingList_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(FriendsEnumerateFollowingList_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(FriendsEnumerateFollowingList_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(FriendsEnumerateFollowingList_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(FriendsEnumerateFollowingList_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(FriendsEnumerateFollowingList_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(FriendsEnumerateFollowingList_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(FriendsEnumerateFollowingList_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(FriendsEnumerateFollowingList_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(FriendsEnumerateFollowingList_t.OnGetSize)
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
				CallbackId = 346
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 346);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(FriendsEnumerateFollowingList_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(FriendsEnumerateFollowingList_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong[] GSteamID;

			internal int ResultsReturned;

			internal int TotalResultCount;

			public static implicit operator FriendsEnumerateFollowingList_t(FriendsEnumerateFollowingList_t.PackSmall d)
			{
				FriendsEnumerateFollowingList_t friendsEnumerateFollowingListT = new FriendsEnumerateFollowingList_t()
				{
					Result = d.Result,
					GSteamID = d.GSteamID,
					ResultsReturned = d.ResultsReturned,
					TotalResultCount = d.TotalResultCount
				};
				return friendsEnumerateFollowingListT;
			}
		}
	}
}