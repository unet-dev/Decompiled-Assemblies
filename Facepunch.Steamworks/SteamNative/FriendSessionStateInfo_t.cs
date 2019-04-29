using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct FriendSessionStateInfo_t
	{
		internal uint IOnlineSessionInstances;

		internal byte IPublishedToFriendsSessionInstance;

		internal static FriendSessionStateInfo_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (FriendSessionStateInfo_t)Marshal.PtrToStructure(p, typeof(FriendSessionStateInfo_t));
			}
			return (FriendSessionStateInfo_t.PackSmall)Marshal.PtrToStructure(p, typeof(FriendSessionStateInfo_t.PackSmall));
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(FriendSessionStateInfo_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(FriendSessionStateInfo_t));
		}

		internal struct PackSmall
		{
			internal uint IOnlineSessionInstances;

			internal byte IPublishedToFriendsSessionInstance;

			public static implicit operator FriendSessionStateInfo_t(FriendSessionStateInfo_t.PackSmall d)
			{
				FriendSessionStateInfo_t friendSessionStateInfoT = new FriendSessionStateInfo_t()
				{
					IOnlineSessionInstances = d.IOnlineSessionInstances,
					IPublishedToFriendsSessionInstance = d.IPublishedToFriendsSessionInstance
				};
				return friendSessionStateInfoT;
			}
		}
	}
}