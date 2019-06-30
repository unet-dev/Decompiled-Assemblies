using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTML_OpenLinkInNewTab_t
	{
		internal uint UnBrowserHandle;

		internal string PchURL;

		internal readonly static int StructSize;

		private static Action<HTML_OpenLinkInNewTab_t> actionClient;

		private static Action<HTML_OpenLinkInNewTab_t> actionServer;

		static HTML_OpenLinkInNewTab_t()
		{
			HTML_OpenLinkInNewTab_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTML_OpenLinkInNewTab_t) : typeof(HTML_OpenLinkInNewTab_t.Pack8)));
		}

		internal static HTML_OpenLinkInNewTab_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_OpenLinkInNewTab_t)Marshal.PtrToStructure(p, typeof(HTML_OpenLinkInNewTab_t)) : (HTML_OpenLinkInNewTab_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_OpenLinkInNewTab_t.Pack8)));
		}

		public static async Task<HTML_OpenLinkInNewTab_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTML_OpenLinkInNewTab_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTML_OpenLinkInNewTab_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTML_OpenLinkInNewTab_t.StructSize, 4507, ref flag) | flag))
					{
						nullable = new HTML_OpenLinkInNewTab_t?(HTML_OpenLinkInNewTab_t.Fill(intPtr));
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

		public static void Install(Action<HTML_OpenLinkInNewTab_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTML_OpenLinkInNewTab_t.OnClient), HTML_OpenLinkInNewTab_t.StructSize, 4507, false);
				HTML_OpenLinkInNewTab_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTML_OpenLinkInNewTab_t.OnServer), HTML_OpenLinkInNewTab_t.StructSize, 4507, true);
				HTML_OpenLinkInNewTab_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_OpenLinkInNewTab_t> action = HTML_OpenLinkInNewTab_t.actionClient;
			if (action != null)
			{
				action(HTML_OpenLinkInNewTab_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_OpenLinkInNewTab_t> action = HTML_OpenLinkInNewTab_t.actionServer;
			if (action != null)
			{
				action(HTML_OpenLinkInNewTab_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint UnBrowserHandle;

			internal string PchURL;

			public static implicit operator HTML_OpenLinkInNewTab_t(HTML_OpenLinkInNewTab_t.Pack8 d)
			{
				HTML_OpenLinkInNewTab_t hTMLOpenLinkInNewTabT = new HTML_OpenLinkInNewTab_t()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchURL = d.PchURL
				};
				return hTMLOpenLinkInNewTabT;
			}

			public static implicit operator Pack8(HTML_OpenLinkInNewTab_t d)
			{
				HTML_OpenLinkInNewTab_t.Pack8 pack8 = new HTML_OpenLinkInNewTab_t.Pack8()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchURL = d.PchURL
				};
				return pack8;
			}
		}
	}
}