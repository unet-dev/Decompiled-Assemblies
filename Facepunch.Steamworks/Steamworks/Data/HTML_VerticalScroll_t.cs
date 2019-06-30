using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTML_VerticalScroll_t
	{
		internal uint UnBrowserHandle;

		internal uint UnScrollMax;

		internal uint UnScrollCurrent;

		internal float FlPageScale;

		internal bool BVisible;

		internal uint UnPageSize;

		internal readonly static int StructSize;

		private static Action<HTML_VerticalScroll_t> actionClient;

		private static Action<HTML_VerticalScroll_t> actionServer;

		static HTML_VerticalScroll_t()
		{
			HTML_VerticalScroll_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTML_VerticalScroll_t) : typeof(HTML_VerticalScroll_t.Pack8)));
		}

		internal static HTML_VerticalScroll_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_VerticalScroll_t)Marshal.PtrToStructure(p, typeof(HTML_VerticalScroll_t)) : (HTML_VerticalScroll_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_VerticalScroll_t.Pack8)));
		}

		public static async Task<HTML_VerticalScroll_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTML_VerticalScroll_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTML_VerticalScroll_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTML_VerticalScroll_t.StructSize, 4512, ref flag) | flag))
					{
						nullable = new HTML_VerticalScroll_t?(HTML_VerticalScroll_t.Fill(intPtr));
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

		public static void Install(Action<HTML_VerticalScroll_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTML_VerticalScroll_t.OnClient), HTML_VerticalScroll_t.StructSize, 4512, false);
				HTML_VerticalScroll_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTML_VerticalScroll_t.OnServer), HTML_VerticalScroll_t.StructSize, 4512, true);
				HTML_VerticalScroll_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_VerticalScroll_t> action = HTML_VerticalScroll_t.actionClient;
			if (action != null)
			{
				action(HTML_VerticalScroll_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_VerticalScroll_t> action = HTML_VerticalScroll_t.actionServer;
			if (action != null)
			{
				action(HTML_VerticalScroll_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint UnBrowserHandle;

			internal uint UnScrollMax;

			internal uint UnScrollCurrent;

			internal float FlPageScale;

			internal bool BVisible;

			internal uint UnPageSize;

			public static implicit operator HTML_VerticalScroll_t(HTML_VerticalScroll_t.Pack8 d)
			{
				HTML_VerticalScroll_t hTMLVerticalScrollT = new HTML_VerticalScroll_t()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					UnScrollMax = d.UnScrollMax,
					UnScrollCurrent = d.UnScrollCurrent,
					FlPageScale = d.FlPageScale,
					BVisible = d.BVisible,
					UnPageSize = d.UnPageSize
				};
				return hTMLVerticalScrollT;
			}

			public static implicit operator Pack8(HTML_VerticalScroll_t d)
			{
				HTML_VerticalScroll_t.Pack8 pack8 = new HTML_VerticalScroll_t.Pack8()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					UnScrollMax = d.UnScrollMax,
					UnScrollCurrent = d.UnScrollCurrent,
					FlPageScale = d.FlPageScale,
					BVisible = d.BVisible,
					UnPageSize = d.UnPageSize
				};
				return pack8;
			}
		}
	}
}