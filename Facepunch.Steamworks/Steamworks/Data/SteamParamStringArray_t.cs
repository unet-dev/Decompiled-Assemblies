using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct SteamParamStringArray_t
	{
		internal IntPtr Strings;

		internal int NumStrings;

		internal static SteamParamStringArray_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamParamStringArray_t)Marshal.PtrToStructure(p, typeof(SteamParamStringArray_t)) : (SteamParamStringArray_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamParamStringArray_t.Pack8)));
		}

		public struct Pack8
		{
			internal IntPtr Strings;

			internal int NumStrings;

			public static implicit operator SteamParamStringArray_t(SteamParamStringArray_t.Pack8 d)
			{
				SteamParamStringArray_t steamParamStringArrayT = new SteamParamStringArray_t()
				{
					Strings = d.Strings,
					NumStrings = d.NumStrings
				};
				return steamParamStringArrayT;
			}

			public static implicit operator Pack8(SteamParamStringArray_t d)
			{
				SteamParamStringArray_t.Pack8 pack8 = new SteamParamStringArray_t.Pack8()
				{
					Strings = d.Strings,
					NumStrings = d.NumStrings
				};
				return pack8;
			}
		}
	}
}