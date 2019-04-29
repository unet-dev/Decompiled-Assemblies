using System;

namespace SteamNative
{
	internal struct FriendsGroupID_t
	{
		public short Value;

		public static implicit operator FriendsGroupID_t(short value)
		{
			return new FriendsGroupID_t()
			{
				Value = value
			};
		}

		public static implicit operator Int16(FriendsGroupID_t value)
		{
			return value.Value;
		}
	}
}