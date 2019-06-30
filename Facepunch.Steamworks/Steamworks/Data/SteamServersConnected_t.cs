using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SteamServersConnected_t
	{
		internal readonly static int StructSize;

		private static Action<SteamServersConnected_t> actionClient;

		private static Action<SteamServersConnected_t> actionServer;

		static SteamServersConnected_t()
		{
			SteamServersConnected_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(SteamServersConnected_t) : typeof(SteamServersConnected_t.Pack8)));
		}

		internal static SteamServersConnected_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamServersConnected_t)Marshal.PtrToStructure(p, typeof(SteamServersConnected_t)) : (SteamServersConnected_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamServersConnected_t.Pack8)));
		}

		public static async Task<SteamServersConnected_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SteamServersConnected_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SteamServersConnected_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SteamServersConnected_t.StructSize, 101, ref flag) | flag))
					{
						nullable = new SteamServersConnected_t?(SteamServersConnected_t.Fill(intPtr));
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

		public static void Install(Action<SteamServersConnected_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SteamServersConnected_t.OnClient), SteamServersConnected_t.StructSize, 101, false);
				SteamServersConnected_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SteamServersConnected_t.OnServer), SteamServersConnected_t.StructSize, 101, true);
				SteamServersConnected_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamServersConnected_t> action = SteamServersConnected_t.actionClient;
			if (action != null)
			{
				action(SteamServersConnected_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamServersConnected_t> action = SteamServersConnected_t.actionServer;
			if (action != null)
			{
				action(SteamServersConnected_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			public static implicit operator SteamServersConnected_t(SteamServersConnected_t.Pack8 d)
			{
				return new SteamServersConnected_t();
			}

			public static implicit operator Pack8(SteamServersConnected_t d)
			{
				return new SteamServersConnected_t.Pack8();
			}
		}
	}
}