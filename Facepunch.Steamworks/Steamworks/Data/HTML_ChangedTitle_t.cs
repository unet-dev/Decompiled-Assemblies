using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTML_ChangedTitle_t
	{
		internal uint UnBrowserHandle;

		internal string PchTitle;

		internal readonly static int StructSize;

		private static Action<HTML_ChangedTitle_t> actionClient;

		private static Action<HTML_ChangedTitle_t> actionServer;

		static HTML_ChangedTitle_t()
		{
			HTML_ChangedTitle_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTML_ChangedTitle_t) : typeof(HTML_ChangedTitle_t.Pack8)));
		}

		internal static HTML_ChangedTitle_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_ChangedTitle_t)Marshal.PtrToStructure(p, typeof(HTML_ChangedTitle_t)) : (HTML_ChangedTitle_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_ChangedTitle_t.Pack8)));
		}

		public static async Task<HTML_ChangedTitle_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTML_ChangedTitle_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTML_ChangedTitle_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTML_ChangedTitle_t.StructSize, 4508, ref flag) | flag))
					{
						nullable = new HTML_ChangedTitle_t?(HTML_ChangedTitle_t.Fill(intPtr));
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

		public static void Install(Action<HTML_ChangedTitle_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTML_ChangedTitle_t.OnClient), HTML_ChangedTitle_t.StructSize, 4508, false);
				HTML_ChangedTitle_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTML_ChangedTitle_t.OnServer), HTML_ChangedTitle_t.StructSize, 4508, true);
				HTML_ChangedTitle_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_ChangedTitle_t> action = HTML_ChangedTitle_t.actionClient;
			if (action != null)
			{
				action(HTML_ChangedTitle_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_ChangedTitle_t> action = HTML_ChangedTitle_t.actionServer;
			if (action != null)
			{
				action(HTML_ChangedTitle_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint UnBrowserHandle;

			internal string PchTitle;

			public static implicit operator HTML_ChangedTitle_t(HTML_ChangedTitle_t.Pack8 d)
			{
				HTML_ChangedTitle_t hTMLChangedTitleT = new HTML_ChangedTitle_t()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchTitle = d.PchTitle
				};
				return hTMLChangedTitleT;
			}

			public static implicit operator Pack8(HTML_ChangedTitle_t d)
			{
				HTML_ChangedTitle_t.Pack8 pack8 = new HTML_ChangedTitle_t.Pack8()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchTitle = d.PchTitle
				};
				return pack8;
			}
		}
	}
}