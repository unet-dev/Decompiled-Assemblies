using System;

namespace SteamNative
{
	internal enum ChatMemberStateChange
	{
		Entered = 1,
		Left = 2,
		Disconnected = 4,
		Kicked = 8,
		Banned = 16
	}
}