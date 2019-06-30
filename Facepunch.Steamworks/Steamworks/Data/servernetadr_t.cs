using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct servernetadr_t
	{
		internal ushort ConnectionPort;

		internal ushort QueryPort;

		internal uint IP;

		internal static servernetadr_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (servernetadr_t)Marshal.PtrToStructure(p, typeof(servernetadr_t)) : (servernetadr_t.Pack8)Marshal.PtrToStructure(p, typeof(servernetadr_t.Pack8)));
		}

		public struct Pack8
		{
			internal ushort ConnectionPort;

			internal ushort QueryPort;

			internal uint IP;

			public static implicit operator servernetadr_t(servernetadr_t.Pack8 d)
			{
				servernetadr_t servernetadrT = new servernetadr_t()
				{
					ConnectionPort = d.ConnectionPort,
					QueryPort = d.QueryPort,
					IP = d.IP
				};
				return servernetadrT;
			}

			public static implicit operator Pack8(servernetadr_t d)
			{
				servernetadr_t.Pack8 pack8 = new servernetadr_t.Pack8()
				{
					ConnectionPort = d.ConnectionPort,
					QueryPort = d.QueryPort,
					IP = d.IP
				};
				return pack8;
			}
		}
	}
}