using Steamworks;
using System;

namespace Steamworks.ServerList
{
	public class LocalNetwork : Base
	{
		public LocalNetwork()
		{
		}

		internal override void LaunchQuery()
		{
			this.request = Base.Internal.RequestLANServerList(base.AppId.Value, IntPtr.Zero);
		}
	}
}