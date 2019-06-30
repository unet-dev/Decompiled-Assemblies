using System;

namespace Steamworks
{
	internal enum GameSearchErrorCode_t
	{
		OK = 1,
		Failed_Search_Already_In_Progress = 2,
		Failed_No_Search_In_Progress = 3,
		Failed_Not_Lobby_Leader = 4,
		Failed_No_Host_Available = 5,
		Failed_Search_Params_Invalid = 6,
		Failed_Offline = 7,
		Failed_NotAuthorized = 8,
		Failed_Unknown_Error = 9
	}
}