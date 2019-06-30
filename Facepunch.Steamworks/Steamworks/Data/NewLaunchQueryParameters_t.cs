using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct NewLaunchQueryParameters_t
	{
		internal static NewLaunchQueryParameters_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (NewLaunchQueryParameters_t)Marshal.PtrToStructure(p, typeof(NewLaunchQueryParameters_t)) : (NewLaunchQueryParameters_t.Pack8)Marshal.PtrToStructure(p, typeof(NewLaunchQueryParameters_t.Pack8)));
		}

		public struct Pack8
		{
			public static implicit operator NewLaunchQueryParameters_t(NewLaunchQueryParameters_t.Pack8 d)
			{
				return new NewLaunchQueryParameters_t();
			}

			public static implicit operator Pack8(NewLaunchQueryParameters_t d)
			{
				return new NewLaunchQueryParameters_t.Pack8();
			}
		}
	}
}