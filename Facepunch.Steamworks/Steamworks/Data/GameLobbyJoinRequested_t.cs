using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GameLobbyJoinRequested_t
	{
		internal ulong SteamIDLobby;

		internal ulong SteamIDFriend;

		internal readonly static int StructSize;

		private static Action<GameLobbyJoinRequested_t> actionClient;

		private static Action<GameLobbyJoinRequested_t> actionServer;

		static GameLobbyJoinRequested_t()
		{
			GameLobbyJoinRequested_t.StructSize = Marshal.SizeOf(typeof(GameLobbyJoinRequested_t));
		}

		internal static GameLobbyJoinRequested_t Fill(IntPtr p)
		{
			return (GameLobbyJoinRequested_t)Marshal.PtrToStructure(p, typeof(GameLobbyJoinRequested_t));
		}

		public static async Task<GameLobbyJoinRequested_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GameLobbyJoinRequested_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GameLobbyJoinRequested_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GameLobbyJoinRequested_t.StructSize, 333, ref flag) | flag))
					{
						nullable = new GameLobbyJoinRequested_t?(GameLobbyJoinRequested_t.Fill(intPtr));
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

		public static void Install(Action<GameLobbyJoinRequested_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GameLobbyJoinRequested_t.OnClient), GameLobbyJoinRequested_t.StructSize, 333, false);
				GameLobbyJoinRequested_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GameLobbyJoinRequested_t.OnServer), GameLobbyJoinRequested_t.StructSize, 333, true);
				GameLobbyJoinRequested_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GameLobbyJoinRequested_t> action = GameLobbyJoinRequested_t.actionClient;
			if (action != null)
			{
				action(GameLobbyJoinRequested_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GameLobbyJoinRequested_t> action = GameLobbyJoinRequested_t.actionServer;
			if (action != null)
			{
				action(GameLobbyJoinRequested_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}