using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
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

		internal static HTML_NeedsPaint_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_NeedsPaint_t)Marshal.PtrToStructure(p, typeof(HTML_NeedsPaint_t)) : (HTML_NeedsPaint_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_NeedsPaint_t.Pack8)));
		}

		public struct Pack8
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

			public static implicit operator HTML_NeedsPaint_t(HTML_NeedsPaint_t.Pack8 d)
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

			public static implicit operator Pack8(HTML_NeedsPaint_t d)
			{
				HTML_NeedsPaint_t.Pack8 pack8 = new HTML_NeedsPaint_t.Pack8()
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
				return pack8;
			}
		}
	}
}