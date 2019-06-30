using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GameConnectedChatJoin_t
	{
		internal ulong SteamIDClanChat;

		internal ulong SteamIDUser;

		internal readonly static int StructSize;

		private static Action<GameConnectedChatJoin_t> actionClient;

		private static Action<GameConnectedChatJoin_t> actionServer;

		static GameConnectedChatJoin_t()
		{
			GameConnectedChatJoin_t.StructSize = Marshal.SizeOf(typeof(GameConnectedChatJoin_t));
		}

		internal static GameConnectedChatJoin_t Fill(IntPtr p)
		{
			return (GameConnectedChatJoin_t)Marshal.PtrToStructure(p, typeof(GameConnectedChatJoin_t));
		}

		public static async Task<GameConnectedChatJoin_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GameConnectedChatJoin_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GameConnectedChatJoin_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GameConnectedChatJoin_t.StructSize, 339, ref flag) | flag))
					{
						nullable = new GameConnectedChatJoin_t?(GameConnectedChatJoin_t.Fill(intPtr));
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

		public static void Install(Action<GameConnectedChatJoin_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GameConnectedChatJoin_t.OnClient), GameConnectedChatJoin_t.StructSize, 339, false);
				GameConnectedChatJoin_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GameConnectedChatJoin_t.OnServer), GameConnectedChatJoin_t.StructSize, 339, true);
				GameConnectedChatJoin_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GameConnectedChatJoin_t> action = GameConnectedChatJoin_t.actionClient;
			if (action != null)
			{
				action(GameConnectedChatJoin_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GameConnectedChatJoin_t> action = GameConnectedChatJoin_t.actionServer;
			if (action != null)
			{
				action(GameConnectedChatJoin_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}