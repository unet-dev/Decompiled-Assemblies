using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GameConnectedChatLeave_t
	{
		internal ulong SteamIDClanChat;

		internal ulong SteamIDUser;

		internal bool Kicked;

		internal bool Dropped;

		internal readonly static int StructSize;

		private static Action<GameConnectedChatLeave_t> actionClient;

		private static Action<GameConnectedChatLeave_t> actionServer;

		static GameConnectedChatLeave_t()
		{
			GameConnectedChatLeave_t.StructSize = Marshal.SizeOf(typeof(GameConnectedChatLeave_t));
		}

		internal static GameConnectedChatLeave_t Fill(IntPtr p)
		{
			return (GameConnectedChatLeave_t)Marshal.PtrToStructure(p, typeof(GameConnectedChatLeave_t));
		}

		public static async Task<GameConnectedChatLeave_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GameConnectedChatLeave_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GameConnectedChatLeave_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GameConnectedChatLeave_t.StructSize, 340, ref flag) | flag))
					{
						nullable = new GameConnectedChatLeave_t?(GameConnectedChatLeave_t.Fill(intPtr));
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

		public static void Install(Action<GameConnectedChatLeave_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GameConnectedChatLeave_t.OnClient), GameConnectedChatLeave_t.StructSize, 340, false);
				GameConnectedChatLeave_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GameConnectedChatLeave_t.OnServer), GameConnectedChatLeave_t.StructSize, 340, true);
				GameConnectedChatLeave_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GameConnectedChatLeave_t> action = GameConnectedChatLeave_t.actionClient;
			if (action != null)
			{
				action(GameConnectedChatLeave_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GameConnectedChatLeave_t> action = GameConnectedChatLeave_t.actionServer;
			if (action != null)
			{
				action(GameConnectedChatLeave_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}