using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct HTML_CloseBrowser_t
	{
		internal uint UnBrowserHandle;

		internal static HTML_CloseBrowser_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_CloseBrowser_t)Marshal.PtrToStructure(p, typeof(HTML_CloseBrowser_t)) : (HTML_CloseBrowser_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_CloseBrowser_t.Pack8)));
		}

		public struct Pack8
		{
			internal uint UnBrowserHandle;

			public static implicit operator HTML_CloseBrowser_t(HTML_CloseBrowser_t.Pack8 d)
			{
				return new HTML_CloseBrowser_t()
				{
					UnBrowserHandle = d.UnBrowserHandle
				};
			}

			public static implicit operator Pack8(HTML_CloseBrowser_t d)
			{
				return new HTML_CloseBrowser_t.Pack8()
				{
					UnBrowserHandle = d.UnBrowserHandle
				};
			}
		}
	}
}