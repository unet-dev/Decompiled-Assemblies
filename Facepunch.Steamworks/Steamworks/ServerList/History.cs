using Steamworks;
using Steamworks.Data;
using System;

namespace Steamworks.ServerList
{
	public class History : Base
	{
		public History()
		{
		}

		internal override void LaunchQuery()
		{
			MatchMakingKeyValuePair_t[] filters = this.GetFilters();
			this.request = Base.Internal.RequestHistoryServerList(base.AppId.Value, ref filters, (uint)filters.Length, IntPtr.Zero);
		}
	}
}