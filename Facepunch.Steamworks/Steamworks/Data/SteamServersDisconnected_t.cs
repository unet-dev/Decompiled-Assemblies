using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SteamServersDisconnected_t
	{
		internal Steamworks.Result Result;

		internal readonly static int StructSize;

		private static Action<SteamServersDisconnected_t> actionClient;

		private static Action<SteamServersDisconnected_t> actionServer;

		static SteamServersDisconnected_t()
		{
			SteamServersDisconnected_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(SteamServersDisconnected_t) : typeof(SteamServersDisconnected_t.Pack8)));
		}

		internal static SteamServersDisconnected_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamServersDisconnected_t)Marshal.PtrToStructure(p, typeof(SteamServersDisconnected_t)) : (SteamServersDisconnected_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamServersDisconnected_t.Pack8)));
		}

		public static async Task<SteamServersDisconnected_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SteamServersDisconnected_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SteamServersDisconnected_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SteamServersDisconnected_t.StructSize, 103, ref flag) | flag))
					{
						nullable = new SteamServersDisconnected_t?(SteamServersDisconnected_t.Fill(intPtr));
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

		public static void Install(Action<SteamServersDisconnected_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SteamServersDisconnected_t.OnClient), SteamServersDisconnected_t.StructSize, 103, false);
				SteamServersDisconnected_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SteamServersDisconnected_t.OnServer), SteamServersDisconnected_t.StructSize, 103, true);
				SteamServersDisconnected_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamServersDisconnected_t> action = SteamServersDisconnected_t.actionClient;
			if (action != null)
			{
				action(SteamServersDisconnected_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamServersDisconnected_t> action = SteamServersDisconnected_t.actionServer;
			if (action != null)
			{
				action(SteamServersDisconnected_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			public static implicit operator SteamServersDisconnected_t(SteamServersDisconnected_t.Pack8 d)
			{
				return new SteamServersDisconnected_t()
				{
					Result = d.Result
				};
			}

			public static implicit operator Pack8(SteamServersDisconnected_t d)
			{
				return new SteamServersDisconnected_t.Pack8()
				{
					Result = d.Result
				};
			}
		}
	}
}