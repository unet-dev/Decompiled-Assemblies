using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct HTML_CloseBrowser_t
	{
		internal uint UnBrowserHandle;

		internal static HTML_CloseBrowser_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (HTML_CloseBrowser_t)Marshal.PtrToStructure(p, typeof(HTML_CloseBrowser_t));
			}
			return (HTML_CloseBrowser_t.PackSmall)Marshal.PtrToStructure(p, typeof(HTML_CloseBrowser_t.PackSmall));
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(HTML_CloseBrowser_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(HTML_CloseBrowser_t));
		}

		internal struct PackSmall
		{
			internal uint UnBrowserHandle;

			public static implicit operator HTML_CloseBrowser_t(HTML_CloseBrowser_t.PackSmall d)
			{
				return new HTML_CloseBrowser_t()
				{
					UnBrowserHandle = d.UnBrowserHandle
				};
			}
		}
	}
}