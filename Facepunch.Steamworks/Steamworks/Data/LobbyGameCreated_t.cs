using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct LobbyGameCreated_t
	{
		internal ulong SteamIDLobby;

		internal ulong SteamIDGameServer;

		internal uint IP;

		internal ushort Port;

		internal readonly static int StructSize;

		private static Action<LobbyGameCreated_t> actionClient;

		private static Action<LobbyGameCreated_t> actionServer;

		static LobbyGameCreated_t()
		{
			LobbyGameCreated_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(LobbyGameCreated_t) : typeof(LobbyGameCreated_t.Pack8)));
		}

		internal static LobbyGameCreated_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (LobbyGameCreated_t)Marshal.PtrToStructure(p, typeof(LobbyGameCreated_t)) : (LobbyGameCreated_t.Pack8)Marshal.PtrToStructure(p, typeof(LobbyGameCreated_t.Pack8)));
		}

		public static async Task<LobbyGameCreated_t?> GetResultAsync(SteamAPICall_t handle)
		{
			LobbyGameCreated_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(LobbyGameCreated_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, LobbyGameCreated_t.StructSize, 509, ref flag) | flag))
					{
						nullable = new LobbyGameCreated_t?(LobbyGameCreated_t.Fill(intPtr));
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

		public static void Install(Action<LobbyGameCreated_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(LobbyGameCreated_t.OnClient), LobbyGameCreated_t.StructSize, 509, false);
				LobbyGameCreated_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(LobbyGameCreated_t.OnServer), LobbyGameCreated_t.StructSize, 509, true);
				LobbyGameCreated_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LobbyGameCreated_t> action = LobbyGameCreated_t.actionClient;
			if (action != null)
			{
				action(LobbyGameCreated_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LobbyGameCreated_t> action = LobbyGameCreated_t.actionServer;
			if (action != null)
			{
				action(LobbyGameCreated_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal ulong SteamIDLobby;

			internal ulong SteamIDGameServer;

			internal uint IP;

			internal ushort Port;

			public static implicit operator LobbyGameCreated_t(LobbyGameCreated_t.Pack8 d)
			{
				LobbyGameCreated_t lobbyGameCreatedT = new LobbyGameCreated_t()
				{
					SteamIDLobby = d.SteamIDLobby,
					SteamIDGameServer = d.SteamIDGameServer,
					IP = d.IP,
					Port = d.Port
				};
				return lobbyGameCreatedT;
			}

			public static implicit operator Pack8(LobbyGameCreated_t d)
			{
				LobbyGameCreated_t.Pack8 pack8 = new LobbyGameCreated_t.Pack8()
				{
					SteamIDLobby = d.SteamIDLobby,
					SteamIDGameServer = d.SteamIDGameServer,
					IP = d.IP,
					Port = d.Port
				};
				return pack8;
			}
		}
	}
}