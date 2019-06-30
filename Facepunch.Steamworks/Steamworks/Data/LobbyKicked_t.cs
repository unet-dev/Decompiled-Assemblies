using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct LobbyKicked_t
	{
		internal ulong SteamIDLobby;

		internal ulong SteamIDAdmin;

		internal byte KickedDueToDisconnect;

		internal readonly static int StructSize;

		private static Action<LobbyKicked_t> actionClient;

		private static Action<LobbyKicked_t> actionServer;

		static LobbyKicked_t()
		{
			LobbyKicked_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(LobbyKicked_t) : typeof(LobbyKicked_t.Pack8)));
		}

		internal static LobbyKicked_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (LobbyKicked_t)Marshal.PtrToStructure(p, typeof(LobbyKicked_t)) : (LobbyKicked_t.Pack8)Marshal.PtrToStructure(p, typeof(LobbyKicked_t.Pack8)));
		}

		public static async Task<LobbyKicked_t?> GetResultAsync(SteamAPICall_t handle)
		{
			LobbyKicked_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(LobbyKicked_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, LobbyKicked_t.StructSize, 512, ref flag) | flag))
					{
						nullable = new LobbyKicked_t?(LobbyKicked_t.Fill(intPtr));
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

		public static void Install(Action<LobbyKicked_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(LobbyKicked_t.OnClient), LobbyKicked_t.StructSize, 512, false);
				LobbyKicked_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(LobbyKicked_t.OnServer), LobbyKicked_t.StructSize, 512, true);
				LobbyKicked_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LobbyKicked_t> action = LobbyKicked_t.actionClient;
			if (action != null)
			{
				action(LobbyKicked_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LobbyKicked_t> action = LobbyKicked_t.actionServer;
			if (action != null)
			{
				action(LobbyKicked_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal ulong SteamIDLobby;

			internal ulong SteamIDAdmin;

			internal byte KickedDueToDisconnect;

			public static implicit operator LobbyKicked_t(LobbyKicked_t.Pack8 d)
			{
				LobbyKicked_t lobbyKickedT = new LobbyKicked_t()
				{
					SteamIDLobby = d.SteamIDLobby,
					SteamIDAdmin = d.SteamIDAdmin,
					KickedDueToDisconnect = d.KickedDueToDisconnect
				};
				return lobbyKickedT;
			}

			public static implicit operator Pack8(LobbyKicked_t d)
			{
				LobbyKicked_t.Pack8 pack8 = new LobbyKicked_t.Pack8()
				{
					SteamIDLobby = d.SteamIDLobby,
					SteamIDAdmin = d.SteamIDAdmin,
					KickedDueToDisconnect = d.KickedDueToDisconnect
				};
				return pack8;
			}
		}
	}
}