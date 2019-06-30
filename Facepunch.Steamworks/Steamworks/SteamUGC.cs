using Steamworks.Data;
using Steamworks.Ugc;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Steamworks
{
	public static class SteamUGC
	{
		private static ISteamUGC _internal;

		internal static ISteamUGC Internal
		{
			get
			{
				if (SteamUGC._internal == null)
				{
					SteamUGC._internal = new ISteamUGC();
					SteamUGC._internal.Init();
				}
				return SteamUGC._internal;
			}
		}

		public static async Task<bool> DeleteFileAsync(PublishedFileId fileId)
		{
			bool flag;
			DeleteItemResult_t? nullable = await SteamUGC.Internal.DeleteItem(fileId);
			DeleteItemResult_t? nullable1 = nullable;
			nullable = null;
			ref Nullable nullablePointer = ref nullable1;
			flag = (nullablePointer.HasValue ? nullablePointer.GetValueOrDefault().Result == Result.OK : false);
			return flag;
		}

		public static bool Download(PublishedFileId fileId, bool highPriority = false)
		{
			return SteamUGC.Internal.DownloadItem(fileId, highPriority);
		}

		public static async Task<Item?> QueryFileAsync(PublishedFileId fileId)
		{
			Item? nullable;
			bool flag;
			Query all = Query.All;
			all = all.WithFileId(new PublishedFileId[] { fileId });
			ResultPage? pageAsync = await all.GetPageAsync(1);
			ResultPage? nullable1 = pageAsync;
			pageAsync = null;
			flag = (!nullable1.HasValue ? true : nullable1.Value.ResultCount != 1);
			if (!flag)
			{
				Item item = nullable1.Value.Entries.First<Item>();
				nullable1.Value.Dispose();
				nullable = new Item?(item);
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		internal static void Shutdown()
		{
			SteamUGC._internal = null;
		}
	}
}