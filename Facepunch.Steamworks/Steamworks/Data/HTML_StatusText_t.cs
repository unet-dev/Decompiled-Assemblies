using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTML_StatusText_t
	{
		internal uint UnBrowserHandle;

		internal string PchMsg;

		internal readonly static int StructSize;

		private static Action<HTML_StatusText_t> actionClient;

		private static Action<HTML_StatusText_t> actionServer;

		static HTML_StatusText_t()
		{
			HTML_StatusText_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTML_StatusText_t) : typeof(HTML_StatusText_t.Pack8)));
		}

		internal static HTML_StatusText_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_StatusText_t)Marshal.PtrToStructure(p, typeof(HTML_StatusText_t)) : (HTML_StatusText_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_StatusText_t.Pack8)));
		}

		public static async Task<HTML_StatusText_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTML_StatusText_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTML_StatusText_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTML_StatusText_t.StructSize, 4523, ref flag) | flag))
					{
						nullable = new HTML_StatusText_t?(HTML_StatusText_t.Fill(intPtr));
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

		public static void Install(Action<HTML_StatusText_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTML_StatusText_t.OnClient), HTML_StatusText_t.StructSize, 4523, false);
				HTML_StatusText_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTML_StatusText_t.OnServer), HTML_StatusText_t.StructSize, 4523, true);
				HTML_StatusText_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_StatusText_t> action = HTML_StatusText_t.actionClient;
			if (action != null)
			{
				action(HTML_StatusText_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_StatusText_t> action = HTML_StatusText_t.actionServer;
			if (action != null)
			{
				action(HTML_StatusText_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint UnBrowserHandle;

			internal string PchMsg;

			public static implicit operator HTML_StatusText_t(HTML_StatusText_t.Pack8 d)
			{
				HTML_StatusText_t hTMLStatusTextT = new HTML_StatusText_t()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchMsg = d.PchMsg
				};
				return hTMLStatusTextT;
			}

			public static implicit operator Pack8(HTML_StatusText_t d)
			{
				HTML_StatusText_t.Pack8 pack8 = new HTML_StatusText_t.Pack8()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchMsg = d.PchMsg
				};
				return pack8;
			}
		}
	}
}