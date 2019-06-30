using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct LeaderboardScoresDownloaded_t
	{
		internal ulong SteamLeaderboard;

		internal ulong SteamLeaderboardEntries;

		internal int CEntryCount;

		internal readonly static int StructSize;

		private static Action<LeaderboardScoresDownloaded_t> actionClient;

		private static Action<LeaderboardScoresDownloaded_t> actionServer;

		static LeaderboardScoresDownloaded_t()
		{
			LeaderboardScoresDownloaded_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(LeaderboardScoresDownloaded_t) : typeof(LeaderboardScoresDownloaded_t.Pack8)));
		}

		internal static LeaderboardScoresDownloaded_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (LeaderboardScoresDownloaded_t)Marshal.PtrToStructure(p, typeof(LeaderboardScoresDownloaded_t)) : (LeaderboardScoresDownloaded_t.Pack8)Marshal.PtrToStructure(p, typeof(LeaderboardScoresDownloaded_t.Pack8)));
		}

		public static async Task<LeaderboardScoresDownloaded_t?> GetResultAsync(SteamAPICall_t handle)
		{
			LeaderboardScoresDownloaded_t? nullable;
			bool flag = false;
			while (!SteamUtils.IsCallComplete(handle, out flag))
			{
				await Task.Delay(1);
				if ((SteamClient.IsValid ? false : !SteamServer.IsValid))
				{
					nullable = null;
					return nullable;
				}
			}
			if (!flag)
			{
				IntPtr intPtr = Marshal.AllocHGlobal(LeaderboardScoresDownloaded_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, LeaderboardScoresDownloaded_t.StructSize, 1105, ref flag) | flag))
					{
						nullable = new LeaderboardScoresDownloaded_t?(LeaderboardScoresDownloaded_t.Fill(intPtr));
					}
					else
					{
						nullable = null;
					}
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static void Install(Action<LeaderboardScoresDownloaded_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(LeaderboardScoresDownloaded_t.OnClient), LeaderboardScoresDownloaded_t.StructSize, 1105, false);
				LeaderboardScoresDownloaded_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(LeaderboardScoresDownloaded_t.OnServer), LeaderboardScoresDownloaded_t.StructSize, 1105, true);
				LeaderboardScoresDownloaded_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LeaderboardScoresDownloaded_t> action = LeaderboardScoresDownloaded_t.actionClient;
			if (action != null)
			{
				action(LeaderboardScoresDownloaded_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LeaderboardScoresDownloaded_t> action = LeaderboardScoresDownloaded_t.actionServer;
			if (action != null)
			{
				action(LeaderboardScoresDownloaded_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal ulong SteamLeaderboard;

			internal ulong SteamLeaderboardEntries;

			internal int CEntryCount;

			public static implicit operator LeaderboardScoresDownloaded_t(LeaderboardScoresDownloaded_t.Pack8 d)
			{
				LeaderboardScoresDownloaded_t leaderboardScoresDownloadedT = new LeaderboardScoresDownloaded_t()
				{
					SteamLeaderboard = d.SteamLeaderboard,
					SteamLeaderboardEntries = d.SteamLeaderboardEntries,
					CEntryCount = d.CEntryCount
				};
				return leaderboardScoresDownloadedT;
			}

			public static implicit operator Pack8(LeaderboardScoresDownloaded_t d)
			{
				LeaderboardScoresDownloaded_t.Pack8 pack8 = new LeaderboardScoresDownloaded_t.Pack8()
				{
					SteamLeaderboard = d.SteamLeaderboard,
					SteamLeaderboardEntries = d.SteamLeaderboardEntries,
					CEntryCount = d.CEntryCount
				};
				return pack8;
			}
		}
	}
}