using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct FriendsIsFollowing_t
	{
		internal const int CallbackId = 345;

		internal SteamNative.Result Result;

		internal ulong SteamID;

		internal bool IsFollowing;

		internal static CallResult<FriendsIsFollowing_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<FriendsIsFollowing_t, bool> CallbackFunction)
		{
			return new CallResult<FriendsIsFollowing_t>(steamworks, call, CallbackFunction, new CallResult<FriendsIsFollowing_t>.ConvertFromPointer(FriendsIsFollowing_t.FromPointer), FriendsIsFollowing_t.StructSize(), 345);
		}

		internal static FriendsIsFollowing_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (FriendsIsFollowing_t)Marshal.PtrToStructure(p, typeof(FriendsIsFollowing_t));
			}
			return (FriendsIsFollowing_t.PackSmall)Marshal.PtrToStructure(p, typeof(FriendsIsFollowing_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return FriendsIsFollowing_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return FriendsIsFollowing_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			FriendsIsFollowing_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			FriendsIsFollowing_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			FriendsIsFollowing_t friendsIsFollowingT = FriendsIsFollowing_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<FriendsIsFollowing_t>(friendsIsFollowingT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<FriendsIsFollowing_t>(friendsIsFollowingT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			FriendsIsFollowing_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(FriendsIsFollowing_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(FriendsIsFollowing_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(FriendsIsFollowing_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(FriendsIsFollowing_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(FriendsIsFollowing_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(FriendsIsFollowing_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(FriendsIsFollowing_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(FriendsIsFollowing_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(FriendsIsFollowing_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(FriendsIsFollowing_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(FriendsIsFollowing_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(FriendsIsFollowing_t.OnGetSize)
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
				CallbackId = 345
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 345);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(FriendsIsFollowing_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(FriendsIsFollowing_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong SteamID;

			internal bool IsFollowing;

			public static implicit operator FriendsIsFollowing_t(FriendsIsFollowing_t.PackSmall d)
			{
				FriendsIsFollowing_t friendsIsFollowingT = new FriendsIsFollowing_t()
				{
					Result = d.Result,
					SteamID = d.SteamID,
					IsFollowing = d.IsFollowing
				};
				return friendsIsFollowingT;
			}
		}
	}
}