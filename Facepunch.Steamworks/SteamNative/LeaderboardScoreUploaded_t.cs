using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct LeaderboardScoreUploaded_t
	{
		internal const int CallbackId = 1106;

		internal byte Success;

		internal ulong SteamLeaderboard;

		internal int Score;

		internal byte ScoreChanged;

		internal int GlobalRankNew;

		internal int GlobalRankPrevious;

		internal static CallResult<LeaderboardScoreUploaded_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<LeaderboardScoreUploaded_t, bool> CallbackFunction)
		{
			return new CallResult<LeaderboardScoreUploaded_t>(steamworks, call, CallbackFunction, new CallResult<LeaderboardScoreUploaded_t>.ConvertFromPointer(LeaderboardScoreUploaded_t.FromPointer), LeaderboardScoreUploaded_t.StructSize(), 1106);
		}

		internal static LeaderboardScoreUploaded_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (LeaderboardScoreUploaded_t)Marshal.PtrToStructure(p, typeof(LeaderboardScoreUploaded_t));
			}
			return (LeaderboardScoreUploaded_t.PackSmall)Marshal.PtrToStructure(p, typeof(LeaderboardScoreUploaded_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return LeaderboardScoreUploaded_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return LeaderboardScoreUploaded_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			LeaderboardScoreUploaded_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			LeaderboardScoreUploaded_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			LeaderboardScoreUploaded_t leaderboardScoreUploadedT = LeaderboardScoreUploaded_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<LeaderboardScoreUploaded_t>(leaderboardScoreUploadedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<LeaderboardScoreUploaded_t>(leaderboardScoreUploadedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			LeaderboardScoreUploaded_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(LeaderboardScoreUploaded_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(LeaderboardScoreUploaded_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(LeaderboardScoreUploaded_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(LeaderboardScoreUploaded_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(LeaderboardScoreUploaded_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(LeaderboardScoreUploaded_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(LeaderboardScoreUploaded_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(LeaderboardScoreUploaded_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(LeaderboardScoreUploaded_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(LeaderboardScoreUploaded_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(LeaderboardScoreUploaded_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(LeaderboardScoreUploaded_t.OnGetSize)
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
				CallbackId = 1106
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1106);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(LeaderboardScoreUploaded_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(LeaderboardScoreUploaded_t));
		}

		internal struct PackSmall
		{
			internal byte Success;

			internal ulong SteamLeaderboard;

			internal int Score;

			internal byte ScoreChanged;

			internal int GlobalRankNew;

			internal int GlobalRankPrevious;

			public static implicit operator LeaderboardScoreUploaded_t(LeaderboardScoreUploaded_t.PackSmall d)
			{
				LeaderboardScoreUploaded_t leaderboardScoreUploadedT = new LeaderboardScoreUploaded_t()
				{
					Success = d.Success,
					SteamLeaderboard = d.SteamLeaderboard,
					Score = d.Score,
					ScoreChanged = d.ScoreChanged,
					GlobalRankNew = d.GlobalRankNew,
					GlobalRankPrevious = d.GlobalRankPrevious
				};
				return leaderboardScoreUploadedT;
			}
		}
	}
}