using Steamworks;
using Steamworks.Data;
using System;

namespace Steamworks.ServerList
{
	public class Friends : Base
	{
		public Friends()
		{
		}

		internal override void LaunchQuery()
		{
			MatchMakingKeyValuePair_t[] filters = this.GetFilters();
			this.request = Base.Internal.RequestFriendsServerList(base.AppId.Value, ref filters, (uint)filters.Length, IntPtr.Zero);
		}
	}
}