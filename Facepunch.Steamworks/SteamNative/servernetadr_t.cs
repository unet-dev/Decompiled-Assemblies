using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct servernetadr_t
	{
		internal ushort ConnectionPort;

		internal ushort QueryPort;

		internal uint IP;

		internal static servernetadr_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (servernetadr_t)Marshal.PtrToStructure(p, typeof(servernetadr_t));
			}
			return (servernetadr_t.PackSmall)Marshal.PtrToStructure(p, typeof(servernetadr_t.PackSmall));
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(servernetadr_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(servernetadr_t));
		}

		internal struct PackSmall
		{
			internal ushort ConnectionPort;

			internal ushort QueryPort;

			internal uint IP;

			public static implicit operator servernetadr_t(servernetadr_t.PackSmall d)
			{
				servernetadr_t servernetadrT = new servernetadr_t()
				{
					ConnectionPort = d.ConnectionPort,
					QueryPort = d.QueryPort,
					IP = d.IP
				};
				return servernetadrT;
			}
		}
	}
}