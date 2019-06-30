using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct ScreenshotRequested_t
	{
		internal readonly static int StructSize;

		private static Action<ScreenshotRequested_t> actionClient;

		private static Action<ScreenshotRequested_t> actionServer;

		static ScreenshotRequested_t()
		{
			ScreenshotRequested_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(ScreenshotRequested_t) : typeof(ScreenshotRequested_t.Pack8)));
		}

		internal static ScreenshotRequested_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (ScreenshotRequested_t)Marshal.PtrToStructure(p, typeof(ScreenshotRequested_t)) : (ScreenshotRequested_t.Pack8)Marshal.PtrToStructure(p, typeof(ScreenshotRequested_t.Pack8)));
		}

		public static async Task<ScreenshotRequested_t?> GetResultAsync(SteamAPICall_t handle)
		{
			ScreenshotRequested_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(ScreenshotRequested_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, ScreenshotRequested_t.StructSize, 2302, ref flag) | flag))
					{
						nullable = new ScreenshotRequested_t?(ScreenshotRequested_t.Fill(intPtr));
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

		public static void Install(Action<ScreenshotRequested_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(ScreenshotRequested_t.OnClient), ScreenshotRequested_t.StructSize, 2302, false);
				ScreenshotRequested_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(ScreenshotRequested_t.OnServer), ScreenshotRequested_t.StructSize, 2302, true);
				ScreenshotRequested_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ScreenshotRequested_t> action = ScreenshotRequested_t.actionClient;
			if (action != null)
			{
				action(ScreenshotRequested_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ScreenshotRequested_t> action = ScreenshotRequested_t.actionServer;
			if (action != null)
			{
				action(ScreenshotRequested_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			public static implicit operator ScreenshotRequested_t(ScreenshotRequested_t.Pack8 d)
			{
				return new ScreenshotRequested_t();
			}

			public static implicit operator Pack8(ScreenshotRequested_t d)
			{
				return new ScreenshotRequested_t.Pack8();
			}
		}
	}
}