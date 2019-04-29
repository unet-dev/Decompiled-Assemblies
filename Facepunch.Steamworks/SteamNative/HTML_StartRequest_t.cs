using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct HTML_StartRequest_t
	{
		internal uint UnBrowserHandle;

		internal string PchURL;

		internal string PchTarget;

		internal string PchPostData;

		internal bool BIsRedirect;

		internal static HTML_StartRequest_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (HTML_StartRequest_t)Marshal.PtrToStructure(p, typeof(HTML_StartRequest_t));
			}
			return (HTML_StartRequest_t.PackSmall)Marshal.PtrToStructure(p, typeof(HTML_StartRequest_t.PackSmall));
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(HTML_StartRequest_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(HTML_StartRequest_t));
		}

		internal struct PackSmall
		{
			internal uint UnBrowserHandle;

			internal string PchURL;

			internal string PchTarget;

			internal string PchPostData;

			internal bool BIsRedirect;

			public static implicit operator HTML_StartRequest_t(HTML_StartRequest_t.PackSmall d)
			{
				HTML_StartRequest_t hTMLStartRequestT = new HTML_StartRequest_t()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchURL = d.PchURL,
					PchTarget = d.PchTarget,
					PchPostData = d.PchPostData,
					BIsRedirect = d.BIsRedirect
				};
				return hTMLStartRequestT;
			}
		}
	}
}