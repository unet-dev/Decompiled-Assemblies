using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct LeaderboardFindResult_t
	{
		internal const int CallbackId = 1104;

		internal ulong SteamLeaderboard;

		internal byte LeaderboardFound;

		internal static CallResult<LeaderboardFindResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<LeaderboardFindResult_t, bool> CallbackFunction)
		{
			return new CallResult<LeaderboardFindResult_t>(steamworks, call, CallbackFunction, new CallResult<LeaderboardFindResult_t>.ConvertFromPointer(LeaderboardFindResult_t.FromPointer), LeaderboardFindResult_t.StructSize(), 1104);
		}

		internal static LeaderboardFindResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (LeaderboardFindResult_t)Marshal.PtrToStructure(p, typeof(LeaderboardFindResult_t));
			}
			return (LeaderboardFindResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(LeaderboardFindResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return LeaderboardFindResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return LeaderboardFindResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			LeaderboardFindResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			LeaderboardFindResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			LeaderboardFindResult_t leaderboardFindResultT = LeaderboardFindResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<LeaderboardFindResult_t>(leaderboardFindResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<LeaderboardFindResult_t>(leaderboardFindResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			LeaderboardFindResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(LeaderboardFindResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(LeaderboardFindResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(LeaderboardFindResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(LeaderboardFindResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(LeaderboardFindResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(LeaderboardFindResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(LeaderboardFindResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(LeaderboardFindResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(LeaderboardFindResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(LeaderboardFindResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(LeaderboardFindResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(LeaderboardFindResult_t.OnGetSize)
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
				CallbackId = 1104
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1104);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(LeaderboardFindResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(LeaderboardFindResult_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamLeaderboard;

			internal byte LeaderboardFound;

			public static implicit operator LeaderboardFindResult_t(LeaderboardFindResult_t.PackSmall d)
			{
				LeaderboardFindResult_t leaderboardFindResultT = new LeaderboardFindResult_t()
				{
					SteamLeaderboard = d.SteamLeaderboard,
					LeaderboardFound = d.LeaderboardFound
				};
				return leaderboardFindResultT;
			}
		}
	}
}