using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GSClientAchievementStatus_t
	{
		internal ulong SteamID;

		internal string PchAchievement;

		internal bool Unlocked;

		internal readonly static int StructSize;

		private static Action<GSClientAchievementStatus_t> actionClient;

		private static Action<GSClientAchievementStatus_t> actionServer;

		static GSClientAchievementStatus_t()
		{
			GSClientAchievementStatus_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(GSClientAchievementStatus_t) : typeof(GSClientAchievementStatus_t.Pack8)));
		}

		internal static GSClientAchievementStatus_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (GSClientAchievementStatus_t)Marshal.PtrToStructure(p, typeof(GSClientAchievementStatus_t)) : (GSClientAchievementStatus_t.Pack8)Marshal.PtrToStructure(p, typeof(GSClientAchievementStatus_t.Pack8)));
		}

		public static async Task<GSClientAchievementStatus_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GSClientAchievementStatus_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GSClientAchievementStatus_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GSClientAchievementStatus_t.StructSize, 206, ref flag) | flag))
					{
						nullable = new GSClientAchievementStatus_t?(GSClientAchievementStatus_t.Fill(intPtr));
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

		public static void Install(Action<GSClientAchievementStatus_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GSClientAchievementStatus_t.OnClient), GSClientAchievementStatus_t.StructSize, 206, false);
				GSClientAchievementStatus_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GSClientAchievementStatus_t.OnServer), GSClientAchievementStatus_t.StructSize, 206, true);
				GSClientAchievementStatus_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSClientAchievementStatus_t> action = GSClientAchievementStatus_t.actionClient;
			if (action != null)
			{
				action(GSClientAchievementStatus_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSClientAchievementStatus_t> action = GSClientAchievementStatus_t.actionServer;
			if (action != null)
			{
				action(GSClientAchievementStatus_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal ulong SteamID;

			internal string PchAchievement;

			internal bool Unlocked;

			public static implicit operator GSClientAchievementStatus_t(GSClientAchievementStatus_t.Pack8 d)
			{
				GSClientAchievementStatus_t gSClientAchievementStatusT = new GSClientAchievementStatus_t()
				{
					SteamID = d.SteamID,
					PchAchievement = d.PchAchievement,
					Unlocked = d.Unlocked
				};
				return gSClientAchievementStatusT;
			}

			public static implicit operator Pack8(GSClientAchievementStatus_t d)
			{
				GSClientAchievementStatus_t.Pack8 pack8 = new GSClientAchievementStatus_t.Pack8()
				{
					SteamID = d.SteamID,
					PchAchievement = d.PchAchievement,
					Unlocked = d.Unlocked
				};
				return pack8;
			}
		}
	}
}