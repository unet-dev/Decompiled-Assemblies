using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct LeaderboardEntry_t
	{
		internal ulong SteamIDUser;

		internal int GlobalRank;

		internal int Score;

		internal int CDetails;

		internal ulong UGC;

		internal static LeaderboardEntry_t Fill(IntPtr p)
		{
			return (LeaderboardEntry_t)Marshal.PtrToStructure(p, typeof(LeaderboardEntry_t));
		}
	}
}