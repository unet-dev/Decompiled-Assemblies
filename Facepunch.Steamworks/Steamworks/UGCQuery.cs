using System;

namespace Steamworks
{
	internal enum UGCQuery
	{
		RankedByVote,
		RankedByPublicationDate,
		AcceptedForGameRankedByAcceptanceDate,
		RankedByTrend,
		FavoritedByFriendsRankedByPublicationDate,
		CreatedByFriendsRankedByPublicationDate,
		RankedByNumTimesReported,
		CreatedByFollowedUsersRankedByPublicationDate,
		NotYetRated,
		RankedByTotalVotesAsc,
		RankedByVotesUp,
		RankedByTextSearch,
		RankedByTotalUniqueSubscriptions,
		RankedByPlaytimeTrend,
		RankedByTotalPlaytime,
		RankedByAveragePlaytimeTrend,
		RankedByLifetimeAveragePlaytime,
		RankedByPlaytimeSessionsTrend,
		RankedByLifetimePlaytimeSessions
	}
}