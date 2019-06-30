using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GameRichPresenceJoinRequested_t
	{
		internal ulong SteamIDFriend;

		internal string Connect;

		internal readonly static int StructSize;

		private static Action<GameRichPresenceJoinRequested_t> actionClient;

		private static Action<GameRichPresenceJoinRequested_t> actionServer;

		static GameRichPresenceJoinRequested_t()
		{
			GameRichPresenceJoinRequested_t.StructSize = Marshal.SizeOf(typeof(GameRichPresenceJoinRequested_t));
		}

		internal static GameRichPresenceJoinRequested_t Fill(IntPtr p)
		{
			return (GameRichPresenceJoinRequested_t)Marshal.PtrToStructure(p, typeof(GameRichPresenceJoinRequested_t));
		}

		public static async Task<GameRichPresenceJoinRequested_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GameRichPresenceJoinRequested_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GameRichPresenceJoinRequested_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GameRichPresenceJoinRequested_t.StructSize, 337, ref flag) | flag))
					{
						nullable = new GameRichPresenceJoinRequested_t?(GameRichPresenceJoinRequested_t.Fill(intPtr));
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

		public static void Install(Action<GameRichPresenceJoinRequested_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GameRichPresenceJoinRequested_t.OnClient), GameRichPresenceJoinRequested_t.StructSize, 337, false);
				GameRichPresenceJoinRequested_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GameRichPresenceJoinRequested_t.OnServer), GameRichPresenceJoinRequested_t.StructSize, 337, true);
				GameRichPresenceJoinRequested_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GameRichPresenceJoinRequested_t> action = GameRichPresenceJoinRequested_t.actionClient;
			if (action != null)
			{
				action(GameRichPresenceJoinRequested_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GameRichPresenceJoinRequested_t> action = GameRichPresenceJoinRequested_t.actionServer;
			if (action != null)
			{
				action(GameRichPresenceJoinRequested_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}