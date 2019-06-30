using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct UserAchievementIconFetched_t
	{
		internal GameId GameID;

		internal string AchievementName;

		internal bool Achieved;

		internal int IconHandle;

		internal readonly static int StructSize;

		private static Action<UserAchievementIconFetched_t> actionClient;

		private static Action<UserAchievementIconFetched_t> actionServer;

		static UserAchievementIconFetched_t()
		{
			UserAchievementIconFetched_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(UserAchievementIconFetched_t) : typeof(UserAchievementIconFetched_t.Pack8)));
		}

		internal static UserAchievementIconFetched_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (UserAchievementIconFetched_t)Marshal.PtrToStructure(p, typeof(UserAchievementIconFetched_t)) : (UserAchievementIconFetched_t.Pack8)Marshal.PtrToStructure(p, typeof(UserAchievementIconFetched_t.Pack8)));
		}

		public static async Task<UserAchievementIconFetched_t?> GetResultAsync(SteamAPICall_t handle)
		{
			UserAchievementIconFetched_t? nullable;
			bool flag = false;
			while (!SteamUtils.IsCallComplete(handle, out flag))
			{
				await Task.Delay(1);
				if ((SteamClient.IsValid ? false : !SteamServer.IsValid))
				{
					nullable = null;
					return nullable;
				}
			}
			if (!flag)
			{
				IntPtr intPtr = Marshal.AllocHGlobal(UserAchievementIconFetched_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, UserAchievementIconFetched_t.StructSize, 1109, ref flag) | flag))
					{
						nullable = new UserAchievementIconFetched_t?(UserAchievementIconFetched_t.Fill(intPtr));
					}
					else
					{
						nullable = null;
					}
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static void Install(Action<UserAchievementIconFetched_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(UserAchievementIconFetched_t.OnClient), UserAchievementIconFetched_t.StructSize, 1109, false);
				UserAchievementIconFetched_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(UserAchievementIconFetched_t.OnServer), UserAchievementIconFetched_t.StructSize, 1109, true);
				UserAchievementIconFetched_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<UserAchievementIconFetched_t> action = UserAchievementIconFetched_t.actionClient;
			if (action != null)
			{
				action(UserAchievementIconFetched_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<UserAchievementIconFetched_t> action = UserAchievementIconFetched_t.actionServer;
			if (action != null)
			{
				action(UserAchievementIconFetched_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal GameId GameID;

			internal string AchievementName;

			internal bool Achieved;

			internal int IconHandle;

			public static implicit operator UserAchievementIconFetched_t(UserAchievementIconFetched_t.Pack8 d)
			{
				UserAchievementIconFetched_t userAchievementIconFetchedT = new UserAchievementIconFetched_t()
				{
					GameID = d.GameID,
					AchievementName = d.AchievementName,
					Achieved = d.Achieved,
					IconHandle = d.IconHandle
				};
				return userAchievementIconFetchedT;
			}

			public static implicit operator Pack8(UserAchievementIconFetched_t d)
			{
				UserAchievementIconFetched_t.Pack8 pack8 = new UserAchievementIconFetched_t.Pack8()
				{
					GameID = d.GameID,
					AchievementName = d.AchievementName,
					Achieved = d.Achieved,
					IconHandle = d.IconHandle
				};
				return pack8;
			}
		}
	}
}