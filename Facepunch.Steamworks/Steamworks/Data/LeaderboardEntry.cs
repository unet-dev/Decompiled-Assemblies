using Steamworks;
using System;
using System.Linq;

namespace Steamworks.Data
{
	public struct LeaderboardEntry
	{
		public Friend User;

		public int GlobalRank;

		public int Score;

		public int[] Details;

		internal static LeaderboardEntry From(LeaderboardEntry_t e, int[] detailsBuffer)
		{
			LeaderboardEntry leaderboardEntry = new LeaderboardEntry()
			{
				User = new Friend(e.SteamIDUser),
				GlobalRank = e.GlobalRank,
				Score = e.Score,
				Details = null
			};
			LeaderboardEntry array = leaderboardEntry;
			if (e.CDetails > 0)
			{
				array.Details = detailsBuffer.Take<int>(e.CDetails).ToArray<int>();
			}
			return array;
		}
	}
}