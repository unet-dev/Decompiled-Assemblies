using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct SteamParamStringArray_t
	{
		internal IntPtr Strings;

		internal int NumStrings;

		internal static SteamParamStringArray_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (SteamParamStringArray_t)Marshal.PtrToStructure(p, typeof(SteamParamStringArray_t));
			}
			return (SteamParamStringArray_t.PackSmall)Marshal.PtrToStructure(p, typeof(SteamParamStringArray_t.PackSmall));
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(SteamParamStringArray_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(SteamParamStringArray_t));
		}

		internal struct PackSmall
		{
			internal IntPtr Strings;

			internal int NumStrings;

			public static implicit operator SteamParamStringArray_t(SteamParamStringArray_t.PackSmall d)
			{
				SteamParamStringArray_t steamParamStringArrayT = new SteamParamStringArray_t()
				{
					Strings = d.Strings,
					NumStrings = d.NumStrings
				};
				return steamParamStringArrayT;
			}
		}
	}
}