using System;

namespace Steamworks
{
	public enum AuthResponse
	{
		OK,
		UserNotConnectedToSteam,
		NoLicenseOrExpired,
		VACBanned,
		LoggedInElseWhere,
		VACCheckTimedOut,
		AuthTicketCanceled,
		AuthTicketInvalidAlreadyUsed,
		AuthTicketInvalid,
		PublisherIssuedBan
	}
}