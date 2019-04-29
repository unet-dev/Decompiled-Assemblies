using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct LeaderboardUGCSet_t
	{
		internal const int CallbackId = 1111;

		internal SteamNative.Result Result;

		internal ulong SteamLeaderboard;

		internal static CallResult<LeaderboardUGCSet_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<LeaderboardUGCSet_t, bool> CallbackFunction)
		{
			return new CallResult<LeaderboardUGCSet_t>(steamworks, call, CallbackFunction, new CallResult<LeaderboardUGCSet_t>.ConvertFromPointer(LeaderboardUGCSet_t.FromPointer), LeaderboardUGCSet_t.StructSize(), 1111);
		}

		internal static LeaderboardUGCSet_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (LeaderboardUGCSet_t)Marshal.PtrToStructure(p, typeof(LeaderboardUGCSet_t));
			}
			return (LeaderboardUGCSet_t.PackSmall)Marshal.PtrToStructure(p, typeof(LeaderboardUGCSet_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return LeaderboardUGCSet_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return LeaderboardUGCSet_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			LeaderboardUGCSet_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			LeaderboardUGCSet_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			LeaderboardUGCSet_t leaderboardUGCSetT = LeaderboardUGCSet_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<LeaderboardUGCSet_t>(leaderboardUGCSetT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<LeaderboardUGCSet_t>(leaderboardUGCSetT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			LeaderboardUGCSet_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(LeaderboardUGCSet_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(LeaderboardUGCSet_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(LeaderboardUGCSet_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(LeaderboardUGCSet_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(LeaderboardUGCSet_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(LeaderboardUGCSet_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(LeaderboardUGCSet_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(LeaderboardUGCSet_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(LeaderboardUGCSet_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(LeaderboardUGCSet_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(LeaderboardUGCSet_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(LeaderboardUGCSet_t.OnGetSize)
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
				CallbackId = 1111
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1111);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(LeaderboardUGCSet_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(LeaderboardUGCSet_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong SteamLeaderboard;

			public static implicit operator LeaderboardUGCSet_t(LeaderboardUGCSet_t.PackSmall d)
			{
				LeaderboardUGCSet_t leaderboardUGCSetT = new LeaderboardUGCSet_t()
				{
					Result = d.Result,
					SteamLeaderboard = d.SteamLeaderboard
				};
				return leaderboardUGCSetT;
			}
		}
	}
}