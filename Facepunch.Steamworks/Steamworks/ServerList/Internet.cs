using Steamworks;
using Steamworks.Data;
using System;

namespace Steamworks.ServerList
{
	public class Internet : Base
	{
		public Internet()
		{
		}

		internal override void LaunchQuery()
		{
			MatchMakingKeyValuePair_t[] filters = this.GetFilters();
			this.request = Base.Internal.RequestInternetServerList(base.AppId.Value, ref filters, (uint)filters.Length, IntPtr.Zero);
		}
	}
}