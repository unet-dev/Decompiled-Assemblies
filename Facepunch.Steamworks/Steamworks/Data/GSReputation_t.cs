using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GSReputation_t
	{
		internal Steamworks.Result Result;

		internal uint ReputationScore;

		internal bool Banned;

		internal uint BannedIP;

		internal ushort BannedPort;

		internal ulong BannedGameID;

		internal uint BanExpires;

		internal readonly static int StructSize;

		private static Action<GSReputation_t> actionClient;

		private static Action<GSReputation_t> actionServer;

		static GSReputation_t()
		{
			GSReputation_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(GSReputation_t) : typeof(GSReputation_t.Pack8)));
		}

		internal static GSReputation_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (GSReputation_t)Marshal.PtrToStructure(p, typeof(GSReputation_t)) : (GSReputation_t.Pack8)Marshal.PtrToStructure(p, typeof(GSReputation_t.Pack8)));
		}

		public static async Task<GSReputation_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GSReputation_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GSReputation_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GSReputation_t.StructSize, 209, ref flag) | flag))
					{
						nullable = new GSReputation_t?(GSReputation_t.Fill(intPtr));
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

		public static void Install(Action<GSReputation_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GSReputation_t.OnClient), GSReputation_t.StructSize, 209, false);
				GSReputation_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GSReputation_t.OnServer), GSReputation_t.StructSize, 209, true);
				GSReputation_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSReputation_t> action = GSReputation_t.actionClient;
			if (action != null)
			{
				action(GSReputation_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSReputation_t> action = GSReputation_t.actionServer;
			if (action != null)
			{
				action(GSReputation_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal uint ReputationScore;

			internal bool Banned;

			internal uint BannedIP;

			internal ushort BannedPort;

			internal ulong BannedGameID;

			internal uint BanExpires;

			public static implicit operator GSReputation_t(GSReputation_t.Pack8 d)
			{
				GSReputation_t gSReputationT = new GSReputation_t()
				{
					Result = d.Result,
					ReputationScore = d.ReputationScore,
					Banned = d.Banned,
					BannedIP = d.BannedIP,
					BannedPort = d.BannedPort,
					BannedGameID = d.BannedGameID,
					BanExpires = d.BanExpires
				};
				return gSReputationT;
			}

			public static implicit operator Pack8(GSReputation_t d)
			{
				GSReputation_t.Pack8 pack8 = new GSReputation_t.Pack8()
				{
					Result = d.Result,
					ReputationScore = d.ReputationScore,
					Banned = d.Banned,
					BannedIP = d.BannedIP,
					BannedPort = d.BannedPort,
					BannedGameID = d.BannedGameID,
					BanExpires = d.BanExpires
				};
				return pack8;
			}
		}
	}
}