using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTML_HideToolTip_t
	{
		internal uint UnBrowserHandle;

		internal readonly static int StructSize;

		private static Action<HTML_HideToolTip_t> actionClient;

		private static Action<HTML_HideToolTip_t> actionServer;

		static HTML_HideToolTip_t()
		{
			HTML_HideToolTip_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTML_HideToolTip_t) : typeof(HTML_HideToolTip_t.Pack8)));
		}

		internal static HTML_HideToolTip_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_HideToolTip_t)Marshal.PtrToStructure(p, typeof(HTML_HideToolTip_t)) : (HTML_HideToolTip_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_HideToolTip_t.Pack8)));
		}

		public static async Task<HTML_HideToolTip_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTML_HideToolTip_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTML_HideToolTip_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTML_HideToolTip_t.StructSize, 4526, ref flag) | flag))
					{
						nullable = new HTML_HideToolTip_t?(HTML_HideToolTip_t.Fill(intPtr));
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

		public static void Install(Action<HTML_HideToolTip_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTML_HideToolTip_t.OnClient), HTML_HideToolTip_t.StructSize, 4526, false);
				HTML_HideToolTip_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTML_HideToolTip_t.OnServer), HTML_HideToolTip_t.StructSize, 4526, true);
				HTML_HideToolTip_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_HideToolTip_t> action = HTML_HideToolTip_t.actionClient;
			if (action != null)
			{
				action(HTML_HideToolTip_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_HideToolTip_t> action = HTML_HideToolTip_t.actionServer;
			if (action != null)
			{
				action(HTML_HideToolTip_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint UnBrowserHandle;

			public static implicit operator HTML_HideToolTip_t(HTML_HideToolTip_t.Pack8 d)
			{
				return new HTML_HideToolTip_t()
				{
					UnBrowserHandle = d.UnBrowserHandle
				};
			}

			public static implicit operator Pack8(HTML_HideToolTip_t d)
			{
				return new HTML_HideToolTip_t.Pack8()
				{
					UnBrowserHandle = d.UnBrowserHandle
				};
			}
		}
	}
}