using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTML_BrowserRestarted_t
	{
		internal uint UnBrowserHandle;

		internal uint UnOldBrowserHandle;

		internal readonly static int StructSize;

		private static Action<HTML_BrowserRestarted_t> actionClient;

		private static Action<HTML_BrowserRestarted_t> actionServer;

		static HTML_BrowserRestarted_t()
		{
			HTML_BrowserRestarted_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTML_BrowserRestarted_t) : typeof(HTML_BrowserRestarted_t.Pack8)));
		}

		internal static HTML_BrowserRestarted_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_BrowserRestarted_t)Marshal.PtrToStructure(p, typeof(HTML_BrowserRestarted_t)) : (HTML_BrowserRestarted_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_BrowserRestarted_t.Pack8)));
		}

		public static async Task<HTML_BrowserRestarted_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTML_BrowserRestarted_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTML_BrowserRestarted_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTML_BrowserRestarted_t.StructSize, 4527, ref flag) | flag))
					{
						nullable = new HTML_BrowserRestarted_t?(HTML_BrowserRestarted_t.Fill(intPtr));
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

		public static void Install(Action<HTML_BrowserRestarted_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTML_BrowserRestarted_t.OnClient), HTML_BrowserRestarted_t.StructSize, 4527, false);
				HTML_BrowserRestarted_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTML_BrowserRestarted_t.OnServer), HTML_BrowserRestarted_t.StructSize, 4527, true);
				HTML_BrowserRestarted_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_BrowserRestarted_t> action = HTML_BrowserRestarted_t.actionClient;
			if (action != null)
			{
				action(HTML_BrowserRestarted_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_BrowserRestarted_t> action = HTML_BrowserRestarted_t.actionServer;
			if (action != null)
			{
				action(HTML_BrowserRestarted_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint UnBrowserHandle;

			internal uint UnOldBrowserHandle;

			public static implicit operator HTML_BrowserRestarted_t(HTML_BrowserRestarted_t.Pack8 d)
			{
				HTML_BrowserRestarted_t hTMLBrowserRestartedT = new HTML_BrowserRestarted_t()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					UnOldBrowserHandle = d.UnOldBrowserHandle
				};
				return hTMLBrowserRestartedT;
			}

			public static implicit operator Pack8(HTML_BrowserRestarted_t d)
			{
				HTML_BrowserRestarted_t.Pack8 pack8 = new HTML_BrowserRestarted_t.Pack8()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					UnOldBrowserHandle = d.UnOldBrowserHandle
				};
				return pack8;
			}
		}
	}
}