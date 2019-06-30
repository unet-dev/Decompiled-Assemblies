using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GameWebCallback_t
	{
		internal string URL;

		internal readonly static int StructSize;

		private static Action<GameWebCallback_t> actionClient;

		private static Action<GameWebCallback_t> actionServer;

		static GameWebCallback_t()
		{
			GameWebCallback_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(GameWebCallback_t) : typeof(GameWebCallback_t.Pack8)));
		}

		internal static GameWebCallback_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (GameWebCallback_t)Marshal.PtrToStructure(p, typeof(GameWebCallback_t)) : (GameWebCallback_t.Pack8)Marshal.PtrToStructure(p, typeof(GameWebCallback_t.Pack8)));
		}

		public static async Task<GameWebCallback_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GameWebCallback_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GameWebCallback_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GameWebCallback_t.StructSize, 164, ref flag) | flag))
					{
						nullable = new GameWebCallback_t?(GameWebCallback_t.Fill(intPtr));
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

		public static void Install(Action<GameWebCallback_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GameWebCallback_t.OnClient), GameWebCallback_t.StructSize, 164, false);
				GameWebCallback_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GameWebCallback_t.OnServer), GameWebCallback_t.StructSize, 164, true);
				GameWebCallback_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GameWebCallback_t> action = GameWebCallback_t.actionClient;
			if (action != null)
			{
				action(GameWebCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GameWebCallback_t> action = GameWebCallback_t.actionServer;
			if (action != null)
			{
				action(GameWebCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal string URL;

			public static implicit operator GameWebCallback_t(GameWebCallback_t.Pack8 d)
			{
				return new GameWebCallback_t()
				{
					URL = d.URL
				};
			}

			public static implicit operator Pack8(GameWebCallback_t d)
			{
				return new GameWebCallback_t.Pack8()
				{
					URL = d.URL
				};
			}
		}
	}
}