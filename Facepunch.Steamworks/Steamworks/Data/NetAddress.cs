using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Explicit)]
	public struct NetAddress
	{
		[FieldOffset(0)]
		internal NetAddress.IPV4 ip;

		[FieldOffset(16)]
		internal ushort port;

		public static NetAddress AnyIp(ushort port)
		{
			NetAddress netAddress = new NetAddress();
			NetAddress.IPV4 pV4 = new NetAddress.IPV4()
			{
				m_8zeros = (ulong)0,
				m_0000 = 0,
				m_ffff = 0,
				ip0 = 0,
				ip1 = 0,
				ip2 = 0,
				ip3 = 0
			};
			netAddress.ip = pV4;
			netAddress.port = port;
			return netAddress;
		}

		public static NetAddress From(string addrStr, ushort port)
		{
			return NetAddress.From(IPAddress.Parse(addrStr), port);
		}

		public static NetAddress From(IPAddress address, ushort port)
		{
			byte[] addressBytes = address.GetAddressBytes();
			if ((int)addressBytes.Length != 4)
			{
				throw new NotImplementedException("Oops - no IPV6 support yet?");
			}
			NetAddress netAddress = new NetAddress();
			NetAddress.IPV4 pV4 = new NetAddress.IPV4()
			{
				m_8zeros = (ulong)0,
				m_0000 = 0,
				m_ffff = 65535,
				ip0 = addressBytes[0],
				ip1 = addressBytes[1],
				ip2 = addressBytes[2],
				ip3 = addressBytes[3]
			};
			netAddress.ip = pV4;
			netAddress.port = port;
			return netAddress;
		}

		public static NetAddress LocalHost(ushort port)
		{
			NetAddress netAddress = new NetAddress();
			NetAddress.IPV4 pV4 = new NetAddress.IPV4()
			{
				m_8zeros = (ulong)0,
				m_0000 = 0,
				m_ffff = 0,
				ip0 = 0,
				ip1 = 0,
				ip2 = 0,
				ip3 = 1
			};
			netAddress.ip = pV4;
			netAddress.port = port;
			return netAddress;
		}

		internal struct IPV4
		{
			internal ulong m_8zeros;

			internal ushort m_0000;

			internal ushort m_ffff;

			internal byte ip0;

			internal byte ip1;

			internal byte ip2;

			internal byte ip3;
		}
	}
}