using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct LeaderboardUGCSet_t
	{
		internal Steamworks.Result Result;

		internal ulong SteamLeaderboard;

		internal readonly static int StructSize;

		private static Action<LeaderboardUGCSet_t> actionClient;

		private static Action<LeaderboardUGCSet_t> actionServer;

		static LeaderboardUGCSet_t()
		{
			LeaderboardUGCSet_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(LeaderboardUGCSet_t) : typeof(LeaderboardUGCSet_t.Pack8)));
		}

		internal static LeaderboardUGCSet_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (LeaderboardUGCSet_t)Marshal.PtrToStructure(p, typeof(LeaderboardUGCSet_t)) : (LeaderboardUGCSet_t.Pack8)Marshal.PtrToStructure(p, typeof(LeaderboardUGCSet_t.Pack8)));
		}

		public static async Task<LeaderboardUGCSet_t?> GetResultAsync(SteamAPICall_t handle)
		{
			LeaderboardUGCSet_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(LeaderboardUGCSet_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, LeaderboardUGCSet_t.StructSize, 1111, ref flag) | flag))
					{
						nullable = new LeaderboardUGCSet_t?(LeaderboardUGCSet_t.Fill(intPtr));
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

		public static void Install(Action<LeaderboardUGCSet_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(LeaderboardUGCSet_t.OnClient), LeaderboardUGCSet_t.StructSize, 1111, false);
				LeaderboardUGCSet_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(LeaderboardUGCSet_t.OnServer), LeaderboardUGCSet_t.StructSize, 1111, true);
				LeaderboardUGCSet_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LeaderboardUGCSet_t> action = LeaderboardUGCSet_t.actionClient;
			if (action != null)
			{
				action(LeaderboardUGCSet_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LeaderboardUGCSet_t> action = LeaderboardUGCSet_t.actionServer;
			if (action != null)
			{
				action(LeaderboardUGCSet_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal ulong SteamLeaderboard;

			public static implicit operator LeaderboardUGCSet_t(LeaderboardUGCSet_t.Pack8 d)
			{
				LeaderboardUGCSet_t leaderboardUGCSetT = new LeaderboardUGCSet_t()
				{
					Result = d.Result,
					SteamLeaderboard = d.SteamLeaderboard
				};
				return leaderboardUGCSetT;
			}

			public static implicit operator Pack8(LeaderboardUGCSet_t d)
			{
				LeaderboardUGCSet_t.Pack8 pack8 = new LeaderboardUGCSet_t.Pack8()
				{
					Result = d.Result,
					SteamLeaderboard = d.SteamLeaderboard
				};
				return pack8;
			}
		}
	}
}