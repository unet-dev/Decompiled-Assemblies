using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SteamNetConnectionStatusChangedCallback_t
	{
		internal Connection Conn;

		internal ConnectionInfo Nfo;

		internal ConnectionState OldState;

		internal readonly static int StructSize;

		private static Action<SteamNetConnectionStatusChangedCallback_t> actionClient;

		private static Action<SteamNetConnectionStatusChangedCallback_t> actionServer;

		static SteamNetConnectionStatusChangedCallback_t()
		{
			SteamNetConnectionStatusChangedCallback_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(SteamNetConnectionStatusChangedCallback_t) : typeof(SteamNetConnectionStatusChangedCallback_t.Pack8)));
		}

		internal static SteamNetConnectionStatusChangedCallback_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamNetConnectionStatusChangedCallback_t)Marshal.PtrToStructure(p, typeof(SteamNetConnectionStatusChangedCallback_t)) : (SteamNetConnectionStatusChangedCallback_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamNetConnectionStatusChangedCallback_t.Pack8)));
		}

		public static async Task<SteamNetConnectionStatusChangedCallback_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SteamNetConnectionStatusChangedCallback_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SteamNetConnectionStatusChangedCallback_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SteamNetConnectionStatusChangedCallback_t.StructSize, 1221, ref flag) | flag))
					{
						nullable = new SteamNetConnectionStatusChangedCallback_t?(SteamNetConnectionStatusChangedCallback_t.Fill(intPtr));
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

		public static void Install(Action<SteamNetConnectionStatusChangedCallback_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SteamNetConnectionStatusChangedCallback_t.OnClient), SteamNetConnectionStatusChangedCallback_t.StructSize, 1221, false);
				SteamNetConnectionStatusChangedCallback_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SteamNetConnectionStatusChangedCallback_t.OnServer), SteamNetConnectionStatusChangedCallback_t.StructSize, 1221, true);
				SteamNetConnectionStatusChangedCallback_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamNetConnectionStatusChangedCallback_t> action = SteamNetConnectionStatusChangedCallback_t.actionClient;
			if (action != null)
			{
				action(SteamNetConnectionStatusChangedCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamNetConnectionStatusChangedCallback_t> action = SteamNetConnectionStatusChangedCallback_t.actionServer;
			if (action != null)
			{
				action(SteamNetConnectionStatusChangedCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Connection Conn;

			internal ConnectionInfo Nfo;

			internal ConnectionState OldState;

			public static implicit operator SteamNetConnectionStatusChangedCallback_t(SteamNetConnectionStatusChangedCallback_t.Pack8 d)
			{
				SteamNetConnectionStatusChangedCallback_t steamNetConnectionStatusChangedCallbackT = new SteamNetConnectionStatusChangedCallback_t()
				{
					Conn = d.Conn,
					Nfo = d.Nfo,
					OldState = d.OldState
				};
				return steamNetConnectionStatusChangedCallbackT;
			}

			public static implicit operator Pack8(SteamNetConnectionStatusChangedCallback_t d)
			{
				SteamNetConnectionStatusChangedCallback_t.Pack8 pack8 = new SteamNetConnectionStatusChangedCallback_t.Pack8()
				{
					Conn = d.Conn,
					Nfo = d.Nfo,
					OldState = d.OldState
				};
				return pack8;
			}
		}
	}
}