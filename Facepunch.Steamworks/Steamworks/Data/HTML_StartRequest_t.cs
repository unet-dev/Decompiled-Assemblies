using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct HTML_StartRequest_t
	{
		internal uint UnBrowserHandle;

		internal string PchURL;

		internal string PchTarget;

		internal string PchPostData;

		internal bool BIsRedirect;

		internal static HTML_StartRequest_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_StartRequest_t)Marshal.PtrToStructure(p, typeof(HTML_StartRequest_t)) : (HTML_StartRequest_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_StartRequest_t.Pack8)));
		}

		public struct Pack8
		{
			internal uint UnBrowserHandle;

			internal string PchURL;

			internal string PchTarget;

			internal string PchPostData;

			internal bool BIsRedirect;

			public static implicit operator HTML_StartRequest_t(HTML_StartRequest_t.Pack8 d)
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

			public static implicit operator Pack8(HTML_StartRequest_t d)
			{
				HTML_StartRequest_t.Pack8 pack8 = new HTML_StartRequest_t.Pack8()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchURL = d.PchURL,
					PchTarget = d.PchTarget,
					PchPostData = d.PchPostData,
					BIsRedirect = d.BIsRedirect
				};
				return pack8;
			}
		}
	}
}