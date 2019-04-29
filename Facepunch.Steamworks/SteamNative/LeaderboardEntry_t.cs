using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct LeaderboardEntry_t
	{
		internal ulong SteamIDUser;

		internal int GlobalRank;

		internal int Score;

		internal int CDetails;

		internal ulong UGC;

		internal static LeaderboardEntry_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (LeaderboardEntry_t)Marshal.PtrToStructure(p, typeof(LeaderboardEntry_t));
			}
			return (LeaderboardEntry_t.PackSmall)Marshal.PtrToStructure(p, typeof(LeaderboardEntry_t.PackSmall));
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(LeaderboardEntry_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(LeaderboardEntry_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamIDUser;

			internal int GlobalRank;

			internal int Score;

			internal int CDetails;

			internal ulong UGC;

			public static implicit operator LeaderboardEntry_t(LeaderboardEntry_t.PackSmall d)
			{
				LeaderboardEntry_t leaderboardEntryT = new LeaderboardEntry_t()
				{
					SteamIDUser = d.SteamIDUser,
					GlobalRank = d.GlobalRank,
					Score = d.Score,
					CDetails = d.CDetails,
					UGC = d.UGC
				};
				return leaderboardEntryT;
			}
		}
	}
}