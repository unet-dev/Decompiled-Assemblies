using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct LeaderboardFindResult_t
	{
		internal ulong SteamLeaderboard;

		internal byte LeaderboardFound;

		internal readonly static int StructSize;

		private static Action<LeaderboardFindResult_t> actionClient;

		private static Action<LeaderboardFindResult_t> actionServer;

		static LeaderboardFindResult_t()
		{
			LeaderboardFindResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(LeaderboardFindResult_t) : typeof(LeaderboardFindResult_t.Pack8)));
		}

		internal static LeaderboardFindResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (LeaderboardFindResult_t)Marshal.PtrToStructure(p, typeof(LeaderboardFindResult_t)) : (LeaderboardFindResult_t.Pack8)Marshal.PtrToStructure(p, typeof(LeaderboardFindResult_t.Pack8)));
		}

		public static async Task<LeaderboardFindResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			LeaderboardFindResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(LeaderboardFindResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, LeaderboardFindResult_t.StructSize, 1104, ref flag) | flag))
					{
						nullable = new LeaderboardFindResult_t?(LeaderboardFindResult_t.Fill(intPtr));
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

		public static void Install(Action<LeaderboardFindResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(LeaderboardFindResult_t.OnClient), LeaderboardFindResult_t.StructSize, 1104, false);
				LeaderboardFindResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(LeaderboardFindResult_t.OnServer), LeaderboardFindResult_t.StructSize, 1104, true);
				LeaderboardFindResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LeaderboardFindResult_t> action = LeaderboardFindResult_t.actionClient;
			if (action != null)
			{
				action(LeaderboardFindResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LeaderboardFindResult_t> action = LeaderboardFindResult_t.actionServer;
			if (action != null)
			{
				action(LeaderboardFindResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal ulong SteamLeaderboard;

			internal byte LeaderboardFound;

			public static implicit operator LeaderboardFindResult_t(LeaderboardFindResult_t.Pack8 d)
			{
				LeaderboardFindResult_t leaderboardFindResultT = new LeaderboardFindResult_t()
				{
					SteamLeaderboard = d.SteamLeaderboard,
					LeaderboardFound = d.LeaderboardFound
				};
				return leaderboardFindResultT;
			}

			public static implicit operator Pack8(LeaderboardFindResult_t d)
			{
				LeaderboardFindResult_t.Pack8 pack8 = new LeaderboardFindResult_t.Pack8()
				{
					SteamLeaderboard = d.SteamLeaderboard,
					LeaderboardFound = d.LeaderboardFound
				};
				return pack8;
			}
		}
	}
}