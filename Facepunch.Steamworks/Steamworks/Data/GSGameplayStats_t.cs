using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GSGameplayStats_t
	{
		internal Steamworks.Result Result;

		internal int Rank;

		internal uint TotalConnects;

		internal uint TotalMinutesPlayed;

		internal readonly static int StructSize;

		private static Action<GSGameplayStats_t> actionClient;

		private static Action<GSGameplayStats_t> actionServer;

		static GSGameplayStats_t()
		{
			GSGameplayStats_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(GSGameplayStats_t) : typeof(GSGameplayStats_t.Pack8)));
		}

		internal static GSGameplayStats_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (GSGameplayStats_t)Marshal.PtrToStructure(p, typeof(GSGameplayStats_t)) : (GSGameplayStats_t.Pack8)Marshal.PtrToStructure(p, typeof(GSGameplayStats_t.Pack8)));
		}

		public static async Task<GSGameplayStats_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GSGameplayStats_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GSGameplayStats_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GSGameplayStats_t.StructSize, 207, ref flag) | flag))
					{
						nullable = new GSGameplayStats_t?(GSGameplayStats_t.Fill(intPtr));
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

		public static void Install(Action<GSGameplayStats_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GSGameplayStats_t.OnClient), GSGameplayStats_t.StructSize, 207, false);
				GSGameplayStats_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GSGameplayStats_t.OnServer), GSGameplayStats_t.StructSize, 207, true);
				GSGameplayStats_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSGameplayStats_t> action = GSGameplayStats_t.actionClient;
			if (action != null)
			{
				action(GSGameplayStats_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSGameplayStats_t> action = GSGameplayStats_t.actionServer;
			if (action != null)
			{
				action(GSGameplayStats_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal int Rank;

			internal uint TotalConnects;

			internal uint TotalMinutesPlayed;

			public static implicit operator GSGameplayStats_t(GSGameplayStats_t.Pack8 d)
			{
				GSGameplayStats_t gSGameplayStatsT = new GSGameplayStats_t()
				{
					Result = d.Result,
					Rank = d.Rank,
					TotalConnects = d.TotalConnects,
					TotalMinutesPlayed = d.TotalMinutesPlayed
				};
				return gSGameplayStatsT;
			}

			public static implicit operator Pack8(GSGameplayStats_t d)
			{
				GSGameplayStats_t.Pack8 pack8 = new GSGameplayStats_t.Pack8()
				{
					Result = d.Result,
					Rank = d.Rank,
					TotalConnects = d.TotalConnects,
					TotalMinutesPlayed = d.TotalMinutesPlayed
				};
				return pack8;
			}
		}
	}
}