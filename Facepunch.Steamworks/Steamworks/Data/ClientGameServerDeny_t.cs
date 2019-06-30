using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct ClientGameServerDeny_t
	{
		internal uint AppID;

		internal uint GameServerIP;

		internal ushort GameServerPort;

		internal ushort Secure;

		internal uint Reason;

		internal readonly static int StructSize;

		private static Action<ClientGameServerDeny_t> actionClient;

		private static Action<ClientGameServerDeny_t> actionServer;

		static ClientGameServerDeny_t()
		{
			ClientGameServerDeny_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(ClientGameServerDeny_t) : typeof(ClientGameServerDeny_t.Pack8)));
		}

		internal static ClientGameServerDeny_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (ClientGameServerDeny_t)Marshal.PtrToStructure(p, typeof(ClientGameServerDeny_t)) : (ClientGameServerDeny_t.Pack8)Marshal.PtrToStructure(p, typeof(ClientGameServerDeny_t.Pack8)));
		}

		public static async Task<ClientGameServerDeny_t?> GetResultAsync(SteamAPICall_t handle)
		{
			ClientGameServerDeny_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(ClientGameServerDeny_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, ClientGameServerDeny_t.StructSize, 113, ref flag) | flag))
					{
						nullable = new ClientGameServerDeny_t?(ClientGameServerDeny_t.Fill(intPtr));
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

		public static void Install(Action<ClientGameServerDeny_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(ClientGameServerDeny_t.OnClient), ClientGameServerDeny_t.StructSize, 113, false);
				ClientGameServerDeny_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(ClientGameServerDeny_t.OnServer), ClientGameServerDeny_t.StructSize, 113, true);
				ClientGameServerDeny_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ClientGameServerDeny_t> action = ClientGameServerDeny_t.actionClient;
			if (action != null)
			{
				action(ClientGameServerDeny_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ClientGameServerDeny_t> action = ClientGameServerDeny_t.actionServer;
			if (action != null)
			{
				action(ClientGameServerDeny_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint AppID;

			internal uint GameServerIP;

			internal ushort GameServerPort;

			internal ushort Secure;

			internal uint Reason;

			public static implicit operator ClientGameServerDeny_t(ClientGameServerDeny_t.Pack8 d)
			{
				ClientGameServerDeny_t clientGameServerDenyT = new ClientGameServerDeny_t()
				{
					AppID = d.AppID,
					GameServerIP = d.GameServerIP,
					GameServerPort = d.GameServerPort,
					Secure = d.Secure,
					Reason = d.Reason
				};
				return clientGameServerDenyT;
			}

			public static implicit operator Pack8(ClientGameServerDeny_t d)
			{
				ClientGameServerDeny_t.Pack8 pack8 = new ClientGameServerDeny_t.Pack8()
				{
					AppID = d.AppID,
					GameServerIP = d.GameServerIP,
					GameServerPort = d.GameServerPort,
					Secure = d.Secure,
					Reason = d.Reason
				};
				return pack8;
			}
		}
	}
}