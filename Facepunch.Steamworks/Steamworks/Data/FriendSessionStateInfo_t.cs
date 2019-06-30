using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct FriendSessionStateInfo_t
	{
		internal uint IOnlineSessionInstances;

		internal byte IPublishedToFriendsSessionInstance;

		internal static FriendSessionStateInfo_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (FriendSessionStateInfo_t)Marshal.PtrToStructure(p, typeof(FriendSessionStateInfo_t)) : (FriendSessionStateInfo_t.Pack8)Marshal.PtrToStructure(p, typeof(FriendSessionStateInfo_t.Pack8)));
		}

		public struct Pack8
		{
			internal uint IOnlineSessionInstances;

			internal byte IPublishedToFriendsSessionInstance;

			public static implicit operator FriendSessionStateInfo_t(FriendSessionStateInfo_t.Pack8 d)
			{
				FriendSessionStateInfo_t friendSessionStateInfoT = new FriendSessionStateInfo_t()
				{
					IOnlineSessionInstances = d.IOnlineSessionInstances,
					IPublishedToFriendsSessionInstance = d.IPublishedToFriendsSessionInstance
				};
				return friendSessionStateInfoT;
			}

			public static implicit operator Pack8(FriendSessionStateInfo_t d)
			{
				FriendSessionStateInfo_t.Pack8 pack8 = new FriendSessionStateInfo_t.Pack8()
				{
					IOnlineSessionInstances = d.IOnlineSessionInstances,
					IPublishedToFriendsSessionInstance = d.IPublishedToFriendsSessionInstance
				};
				return pack8;
			}
		}
	}
}