using System;

namespace SteamNative
{
	internal enum FriendRelationship
	{
		None,
		Blocked,
		RequestRecipient,
		Friend,
		RequestInitiator,
		Ignored,
		IgnoredFriend,
		Suggested_DEPRECATED,
		Max
	}
}