using System;
using System.Net.Sockets;

namespace Mono.Posix
{
	[Obsolete("Use Mono.Unix.PeerCred")]
	public class PeerCred
	{
		private const int so_peercred = 10001;

		private PeerCredData data;

		public int GroupID
		{
			get
			{
				return this.data.gid;
			}
		}

		public int ProcessID
		{
			get
			{
				return this.data.pid;
			}
		}

		public int UserID
		{
			get
			{
				return this.data.uid;
			}
		}

		public PeerCred(Socket sock)
		{
			if (sock.AddressFamily != AddressFamily.Unix)
			{
				throw new ArgumentException("Only Unix sockets are supported", "sock");
			}
			this.data = (PeerCredData)sock.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Debug | SocketOptionName.DontRoute | SocketOptionName.OutOfBandInline | SocketOptionName.IPOptions | SocketOptionName.DropSourceMembership | SocketOptionName.BlockSource | SocketOptionName.NoDelay | SocketOptionName.NoChecksum);
		}
	}
}