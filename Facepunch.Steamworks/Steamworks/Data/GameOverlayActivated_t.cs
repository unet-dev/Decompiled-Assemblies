using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GameOverlayActivated_t
	{
		internal byte Active;

		internal readonly static int StructSize;

		private static Action<GameOverlayActivated_t> actionClient;

		private static Action<GameOverlayActivated_t> actionServer;

		static GameOverlayActivated_t()
		{
			GameOverlayActivated_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(GameOverlayActivated_t) : typeof(GameOverlayActivated_t.Pack8)));
		}

		internal static GameOverlayActivated_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (GameOverlayActivated_t)Marshal.PtrToStructure(p, typeof(GameOverlayActivated_t)) : (GameOverlayActivated_t.Pack8)Marshal.PtrToStructure(p, typeof(GameOverlayActivated_t.Pack8)));
		}

		public static async Task<GameOverlayActivated_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GameOverlayActivated_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GameOverlayActivated_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GameOverlayActivated_t.StructSize, 331, ref flag) | flag))
					{
						nullable = new GameOverlayActivated_t?(GameOverlayActivated_t.Fill(intPtr));
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

		public static void Install(Action<GameOverlayActivated_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GameOverlayActivated_t.OnClient), GameOverlayActivated_t.StructSize, 331, false);
				GameOverlayActivated_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GameOverlayActivated_t.OnServer), GameOverlayActivated_t.StructSize, 331, true);
				GameOverlayActivated_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GameOverlayActivated_t> action = GameOverlayActivated_t.actionClient;
			if (action != null)
			{
				action(GameOverlayActivated_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GameOverlayActivated_t> action = GameOverlayActivated_t.actionServer;
			if (action != null)
			{
				action(GameOverlayActivated_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal byte Active;

			public static implicit operator GameOverlayActivated_t(GameOverlayActivated_t.Pack8 d)
			{
				return new GameOverlayActivated_t()
				{
					Active = d.Active
				};
			}

			public static implicit operator Pack8(GameOverlayActivated_t d)
			{
				return new GameOverlayActivated_t.Pack8()
				{
					Active = d.Active
				};
			}
		}
	}
}