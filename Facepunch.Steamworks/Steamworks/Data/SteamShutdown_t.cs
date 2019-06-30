using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SteamShutdown_t
	{
		internal readonly static int StructSize;

		private static Action<SteamShutdown_t> actionClient;

		private static Action<SteamShutdown_t> actionServer;

		static SteamShutdown_t()
		{
			SteamShutdown_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(SteamShutdown_t) : typeof(SteamShutdown_t.Pack8)));
		}

		internal static SteamShutdown_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamShutdown_t)Marshal.PtrToStructure(p, typeof(SteamShutdown_t)) : (SteamShutdown_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamShutdown_t.Pack8)));
		}

		public static async Task<SteamShutdown_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SteamShutdown_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SteamShutdown_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SteamShutdown_t.StructSize, 704, ref flag) | flag))
					{
						nullable = new SteamShutdown_t?(SteamShutdown_t.Fill(intPtr));
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

		public static void Install(Action<SteamShutdown_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SteamShutdown_t.OnClient), SteamShutdown_t.StructSize, 704, false);
				SteamShutdown_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SteamShutdown_t.OnServer), SteamShutdown_t.StructSize, 704, true);
				SteamShutdown_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamShutdown_t> action = SteamShutdown_t.actionClient;
			if (action != null)
			{
				action(SteamShutdown_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamShutdown_t> action = SteamShutdown_t.actionServer;
			if (action != null)
			{
				action(SteamShutdown_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			public static implicit operator SteamShutdown_t(SteamShutdown_t.Pack8 d)
			{
				return new SteamShutdown_t();
			}

			public static implicit operator Pack8(SteamShutdown_t d)
			{
				return new SteamShutdown_t.Pack8();
			}
		}
	}
}