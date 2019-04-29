using Mono.Unix;
using System;
using System.Net.Sockets;

namespace Mono.Remoting.Channels.Unix
{
	internal class ReusableUnixClient : UnixClient
	{
		public bool IsAlive
		{
			get
			{
				return !base.Client.Poll(0, SelectMode.SelectRead);
			}
		}

		public ReusableUnixClient(string path) : base(path)
		{
		}
	}
}