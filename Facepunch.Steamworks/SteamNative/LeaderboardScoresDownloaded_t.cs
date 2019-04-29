using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct LeaderboardScoresDownloaded_t
	{
		internal const int CallbackId = 1105;

		internal ulong SteamLeaderboard;

		internal ulong SteamLeaderboardEntries;

		internal int CEntryCount;

		internal static CallResult<LeaderboardScoresDownloaded_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<LeaderboardScoresDownloaded_t, bool> CallbackFunction)
		{
			return new CallResult<LeaderboardScoresDownloaded_t>(steamworks, call, CallbackFunction, new CallResult<LeaderboardScoresDownloaded_t>.ConvertFromPointer(LeaderboardScoresDownloaded_t.FromPointer), LeaderboardScoresDownloaded_t.StructSize(), 1105);
		}

		internal static LeaderboardScoresDownloaded_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (LeaderboardScoresDownloaded_t)Marshal.PtrToStructure(p, typeof(LeaderboardScoresDownloaded_t));
			}
			return (LeaderboardScoresDownloaded_t.PackSmall)Marshal.PtrToStructure(p, typeof(LeaderboardScoresDownloaded_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return LeaderboardScoresDownloaded_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return LeaderboardScoresDownloaded_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			LeaderboardScoresDownloaded_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			LeaderboardScoresDownloaded_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			LeaderboardScoresDownloaded_t leaderboardScoresDownloadedT = LeaderboardScoresDownloaded_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<LeaderboardScoresDownloaded_t>(leaderboardScoresDownloadedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<LeaderboardScoresDownloaded_t>(leaderboardScoresDownloadedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			LeaderboardScoresDownloaded_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(LeaderboardScoresDownloaded_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(LeaderboardScoresDownloaded_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(LeaderboardScoresDownloaded_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(LeaderboardScoresDownloaded_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(LeaderboardScoresDownloaded_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(LeaderboardScoresDownloaded_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(LeaderboardScoresDownloaded_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(LeaderboardScoresDownloaded_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(LeaderboardScoresDownloaded_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(LeaderboardScoresDownloaded_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(LeaderboardScoresDownloaded_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(LeaderboardScoresDownloaded_t.OnGetSize)
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
				CallbackId = 1105
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1105);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(LeaderboardScoresDownloaded_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(LeaderboardScoresDownloaded_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamLeaderboard;

			internal ulong SteamLeaderboardEntries;

			internal int CEntryCount;

			public static implicit operator LeaderboardScoresDownloaded_t(LeaderboardScoresDownloaded_t.PackSmall d)
			{
				LeaderboardScoresDownloaded_t leaderboardScoresDownloadedT = new LeaderboardScoresDownloaded_t()
				{
					SteamLeaderboard = d.SteamLeaderboard,
					SteamLeaderboardEntries = d.SteamLeaderboardEntries,
					CEntryCount = d.CEntryCount
				};
				return leaderboardScoresDownloadedT;
			}
		}
	}
}