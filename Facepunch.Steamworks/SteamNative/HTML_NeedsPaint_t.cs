using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct HTML_NeedsPaint_t
	{
		internal uint UnBrowserHandle;

		internal string PBGRA;

		internal uint UnWide;

		internal uint UnTall;

		internal uint UnUpdateX;

		internal uint UnUpdateY;

		internal uint UnUpdateWide;

		internal uint UnUpdateTall;

		internal uint UnScrollX;

		internal uint UnScrollY;

		internal float FlPageScale;

		internal uint UnPageSerial;

		internal static HTML_NeedsPaint_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (HTML_NeedsPaint_t)Marshal.PtrToStructure(p, typeof(HTML_NeedsPaint_t));
			}
			return (HTML_NeedsPaint_t.PackSmall)Marshal.PtrToStructure(p, typeof(HTML_NeedsPaint_t.PackSmall));
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(HTML_NeedsPaint_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(HTML_NeedsPaint_t));
		}

		internal struct PackSmall
		{
			internal uint UnBrowserHandle;

			internal string PBGRA;

			internal uint UnWide;

			internal uint UnTall;

			internal uint UnUpdateX;

			internal uint UnUpdateY;

			internal uint UnUpdateWide;

			internal uint UnUpdateTall;

			internal uint UnScrollX;

			internal uint UnScrollY;

			internal float FlPageScale;

			internal uint UnPageSerial;

			public static implicit operator HTML_NeedsPaint_t(HTML_NeedsPaint_t.PackSmall d)
			{
				HTML_NeedsPaint_t hTMLNeedsPaintT = new HTML_NeedsPaint_t()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PBGRA = d.PBGRA,
					UnWide = d.UnWide,
					UnTall = d.UnTall,
					UnUpdateX = d.UnUpdateX,
					UnUpdateY = d.UnUpdateY,
					UnUpdateWide = d.UnUpdateWide,
					UnUpdateTall = d.UnUpdateTall,
					UnScrollX = d.UnScrollX,
					UnScrollY = d.UnScrollY,
					FlPageScale = d.FlPageScale,
					UnPageSerial = d.UnPageSerial
				};
				return hTMLNeedsPaintT;
			}
		}
	}
}