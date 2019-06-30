using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct LobbyMatchList_t
	{
		internal uint LobbiesMatching;

		internal readonly static int StructSize;

		private static Action<LobbyMatchList_t> actionClient;

		private static Action<LobbyMatchList_t> actionServer;

		static LobbyMatchList_t()
		{
			LobbyMatchList_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(LobbyMatchList_t) : typeof(LobbyMatchList_t.Pack8)));
		}

		internal static LobbyMatchList_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (LobbyMatchList_t)Marshal.PtrToStructure(p, typeof(LobbyMatchList_t)) : (LobbyMatchList_t.Pack8)Marshal.PtrToStructure(p, typeof(LobbyMatchList_t.Pack8)));
		}

		public static async Task<LobbyMatchList_t?> GetResultAsync(SteamAPICall_t handle)
		{
			LobbyMatchList_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(LobbyMatchList_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, LobbyMatchList_t.StructSize, 510, ref flag) | flag))
					{
						nullable = new LobbyMatchList_t?(LobbyMatchList_t.Fill(intPtr));
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

		public static void Install(Action<LobbyMatchList_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(LobbyMatchList_t.OnClient), LobbyMatchList_t.StructSize, 510, false);
				LobbyMatchList_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(LobbyMatchList_t.OnServer), LobbyMatchList_t.StructSize, 510, true);
				LobbyMatchList_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LobbyMatchList_t> action = LobbyMatchList_t.actionClient;
			if (action != null)
			{
				action(LobbyMatchList_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LobbyMatchList_t> action = LobbyMatchList_t.actionServer;
			if (action != null)
			{
				action(LobbyMatchList_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint LobbiesMatching;

			public static implicit operator LobbyMatchList_t(LobbyMatchList_t.Pack8 d)
			{
				return new LobbyMatchList_t()
				{
					LobbiesMatching = d.LobbiesMatching
				};
			}

			public static implicit operator Pack8(LobbyMatchList_t d)
			{
				return new LobbyMatchList_t.Pack8()
				{
					LobbiesMatching = d.LobbiesMatching
				};
			}
		}
	}
}