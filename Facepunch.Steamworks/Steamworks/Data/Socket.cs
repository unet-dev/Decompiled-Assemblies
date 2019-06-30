using Steamworks;
using System;

namespace Steamworks.Data
{
	public struct Socket
	{
		internal uint Id;

		public SocketInterface Interface
		{
			get
			{
				return SteamNetworkingSockets.GetSocketInterface(this.Id);
			}
			set
			{
				SteamNetworkingSockets.SetSocketInterface(this.Id, value);
			}
		}

		public bool Close()
		{
			return SteamNetworkingSockets.Internal.CloseListenSocket(this);
		}

		public override string ToString()
		{
			return this.Id.ToString();
		}
	}
}