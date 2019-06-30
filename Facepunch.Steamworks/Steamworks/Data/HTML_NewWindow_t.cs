using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTML_NewWindow_t
	{
		internal uint UnBrowserHandle;

		internal string PchURL;

		internal uint UnX;

		internal uint UnY;

		internal uint UnWide;

		internal uint UnTall;

		internal uint UnNewWindow_BrowserHandle_IGNORE;

		internal readonly static int StructSize;

		private static Action<HTML_NewWindow_t> actionClient;

		private static Action<HTML_NewWindow_t> actionServer;

		static HTML_NewWindow_t()
		{
			HTML_NewWindow_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTML_NewWindow_t) : typeof(HTML_NewWindow_t.Pack8)));
		}

		internal static HTML_NewWindow_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_NewWindow_t)Marshal.PtrToStructure(p, typeof(HTML_NewWindow_t)) : (HTML_NewWindow_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_NewWindow_t.Pack8)));
		}

		public static async Task<HTML_NewWindow_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTML_NewWindow_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTML_NewWindow_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTML_NewWindow_t.StructSize, 4521, ref flag) | flag))
					{
						nullable = new HTML_NewWindow_t?(HTML_NewWindow_t.Fill(intPtr));
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

		public static void Install(Action<HTML_NewWindow_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTML_NewWindow_t.OnClient), HTML_NewWindow_t.StructSize, 4521, false);
				HTML_NewWindow_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTML_NewWindow_t.OnServer), HTML_NewWindow_t.StructSize, 4521, true);
				HTML_NewWindow_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_NewWindow_t> action = HTML_NewWindow_t.actionClient;
			if (action != null)
			{
				action(HTML_NewWindow_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_NewWindow_t> action = HTML_NewWindow_t.actionServer;
			if (action != null)
			{
				action(HTML_NewWindow_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint UnBrowserHandle;

			internal string PchURL;

			internal uint UnX;

			internal uint UnY;

			internal uint UnWide;

			internal uint UnTall;

			internal uint UnNewWindow_BrowserHandle_IGNORE;

			public static implicit operator HTML_NewWindow_t(HTML_NewWindow_t.Pack8 d)
			{
				HTML_NewWindow_t hTMLNewWindowT = new HTML_NewWindow_t()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchURL = d.PchURL,
					UnX = d.UnX,
					UnY = d.UnY,
					UnWide = d.UnWide,
					UnTall = d.UnTall,
					UnNewWindow_BrowserHandle_IGNORE = d.UnNewWindow_BrowserHandle_IGNORE
				};
				return hTMLNewWindowT;
			}

			public static implicit operator Pack8(HTML_NewWindow_t d)
			{
				HTML_NewWindow_t.Pack8 pack8 = new HTML_NewWindow_t.Pack8()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchURL = d.PchURL,
					UnX = d.UnX,
					UnY = d.UnY,
					UnWide = d.UnWide,
					UnTall = d.UnTall,
					UnNewWindow_BrowserHandle_IGNORE = d.UnNewWindow_BrowserHandle_IGNORE
				};
				return pack8;
			}
		}
	}
}