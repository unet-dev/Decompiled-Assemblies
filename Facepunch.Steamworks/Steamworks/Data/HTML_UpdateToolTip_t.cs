using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTML_UpdateToolTip_t
	{
		internal uint UnBrowserHandle;

		internal string PchMsg;

		internal readonly static int StructSize;

		private static Action<HTML_UpdateToolTip_t> actionClient;

		private static Action<HTML_UpdateToolTip_t> actionServer;

		static HTML_UpdateToolTip_t()
		{
			HTML_UpdateToolTip_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTML_UpdateToolTip_t) : typeof(HTML_UpdateToolTip_t.Pack8)));
		}

		internal static HTML_UpdateToolTip_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_UpdateToolTip_t)Marshal.PtrToStructure(p, typeof(HTML_UpdateToolTip_t)) : (HTML_UpdateToolTip_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_UpdateToolTip_t.Pack8)));
		}

		public static async Task<HTML_UpdateToolTip_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTML_UpdateToolTip_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTML_UpdateToolTip_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTML_UpdateToolTip_t.StructSize, 4525, ref flag) | flag))
					{
						nullable = new HTML_UpdateToolTip_t?(HTML_UpdateToolTip_t.Fill(intPtr));
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

		public static void Install(Action<HTML_UpdateToolTip_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTML_UpdateToolTip_t.OnClient), HTML_UpdateToolTip_t.StructSize, 4525, false);
				HTML_UpdateToolTip_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTML_UpdateToolTip_t.OnServer), HTML_UpdateToolTip_t.StructSize, 4525, true);
				HTML_UpdateToolTip_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_UpdateToolTip_t> action = HTML_UpdateToolTip_t.actionClient;
			if (action != null)
			{
				action(HTML_UpdateToolTip_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_UpdateToolTip_t> action = HTML_UpdateToolTip_t.actionServer;
			if (action != null)
			{
				action(HTML_UpdateToolTip_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint UnBrowserHandle;

			internal string PchMsg;

			public static implicit operator HTML_UpdateToolTip_t(HTML_UpdateToolTip_t.Pack8 d)
			{
				HTML_UpdateToolTip_t hTMLUpdateToolTipT = new HTML_UpdateToolTip_t()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchMsg = d.PchMsg
				};
				return hTMLUpdateToolTipT;
			}

			public static implicit operator Pack8(HTML_UpdateToolTip_t d)
			{
				HTML_UpdateToolTip_t.Pack8 pack8 = new HTML_UpdateToolTip_t.Pack8()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchMsg = d.PchMsg
				};
				return pack8;
			}
		}
	}
}