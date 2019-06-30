using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GameServerChangeRequested_t
	{
		internal string Server;

		internal string Password;

		internal readonly static int StructSize;

		private static Action<GameServerChangeRequested_t> actionClient;

		private static Action<GameServerChangeRequested_t> actionServer;

		static GameServerChangeRequested_t()
		{
			GameServerChangeRequested_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(GameServerChangeRequested_t) : typeof(GameServerChangeRequested_t.Pack8)));
		}

		internal static GameServerChangeRequested_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (GameServerChangeRequested_t)Marshal.PtrToStructure(p, typeof(GameServerChangeRequested_t)) : (GameServerChangeRequested_t.Pack8)Marshal.PtrToStructure(p, typeof(GameServerChangeRequested_t.Pack8)));
		}

		public static async Task<GameServerChangeRequested_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GameServerChangeRequested_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GameServerChangeRequested_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GameServerChangeRequested_t.StructSize, 332, ref flag) | flag))
					{
						nullable = new GameServerChangeRequested_t?(GameServerChangeRequested_t.Fill(intPtr));
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

		public static void Install(Action<GameServerChangeRequested_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GameServerChangeRequested_t.OnClient), GameServerChangeRequested_t.StructSize, 332, false);
				GameServerChangeRequested_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GameServerChangeRequested_t.OnServer), GameServerChangeRequested_t.StructSize, 332, true);
				GameServerChangeRequested_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GameServerChangeRequested_t> action = GameServerChangeRequested_t.actionClient;
			if (action != null)
			{
				action(GameServerChangeRequested_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GameServerChangeRequested_t> action = GameServerChangeRequested_t.actionServer;
			if (action != null)
			{
				action(GameServerChangeRequested_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal string Server;

			internal string Password;

			public static implicit operator GameServerChangeRequested_t(GameServerChangeRequested_t.Pack8 d)
			{
				GameServerChangeRequested_t gameServerChangeRequestedT = new GameServerChangeRequested_t()
				{
					Server = d.Server,
					Password = d.Password
				};
				return gameServerChangeRequestedT;
			}

			public static implicit operator Pack8(GameServerChangeRequested_t d)
			{
				GameServerChangeRequested_t.Pack8 pack8 = new GameServerChangeRequested_t.Pack8()
				{
					Server = d.Server,
					Password = d.Password
				};
				return pack8;
			}
		}
	}
}