using Steamworks;
using Steamworks.Data;
using System;

namespace Steamworks.ServerList
{
	public class Favourites : Base
	{
		public Favourites()
		{
		}

		internal override void LaunchQuery()
		{
			MatchMakingKeyValuePair_t[] filters = this.GetFilters();
			this.request = Base.Internal.RequestFavoritesServerList(base.AppId.Value, ref filters, (uint)filters.Length, IntPtr.Zero);
		}
	}
}