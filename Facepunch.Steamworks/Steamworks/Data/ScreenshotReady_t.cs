using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct ScreenshotReady_t
	{
		internal uint Local;

		internal Steamworks.Result Result;

		internal readonly static int StructSize;

		private static Action<ScreenshotReady_t> actionClient;

		private static Action<ScreenshotReady_t> actionServer;

		static ScreenshotReady_t()
		{
			ScreenshotReady_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(ScreenshotReady_t) : typeof(ScreenshotReady_t.Pack8)));
		}

		internal static ScreenshotReady_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (ScreenshotReady_t)Marshal.PtrToStructure(p, typeof(ScreenshotReady_t)) : (ScreenshotReady_t.Pack8)Marshal.PtrToStructure(p, typeof(ScreenshotReady_t.Pack8)));
		}

		public static async Task<ScreenshotReady_t?> GetResultAsync(SteamAPICall_t handle)
		{
			ScreenshotReady_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(ScreenshotReady_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, ScreenshotReady_t.StructSize, 2301, ref flag) | flag))
					{
						nullable = new ScreenshotReady_t?(ScreenshotReady_t.Fill(intPtr));
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

		public static void Install(Action<ScreenshotReady_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(ScreenshotReady_t.OnClient), ScreenshotReady_t.StructSize, 2301, false);
				ScreenshotReady_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(ScreenshotReady_t.OnServer), ScreenshotReady_t.StructSize, 2301, true);
				ScreenshotReady_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ScreenshotReady_t> action = ScreenshotReady_t.actionClient;
			if (action != null)
			{
				action(ScreenshotReady_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ScreenshotReady_t> action = ScreenshotReady_t.actionServer;
			if (action != null)
			{
				action(ScreenshotReady_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint Local;

			internal Steamworks.Result Result;

			public static implicit operator ScreenshotReady_t(ScreenshotReady_t.Pack8 d)
			{
				ScreenshotReady_t screenshotReadyT = new ScreenshotReady_t()
				{
					Local = d.Local,
					Result = d.Result
				};
				return screenshotReadyT;
			}

			public static implicit operator Pack8(ScreenshotReady_t d)
			{
				ScreenshotReady_t.Pack8 pack8 = new ScreenshotReady_t.Pack8()
				{
					Local = d.Local,
					Result = d.Result
				};
				return pack8;
			}
		}
	}
}