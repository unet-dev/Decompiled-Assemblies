using Oxide.Core.Libraries;
using Oxide.Game.Rust.Libraries.Covalence;
using System;

namespace Oxide.Game.Rust.Libraries
{
	public class Item : Library
	{
		internal readonly static RustCovalenceProvider Covalence;

		static Item()
		{
			Item.Covalence = RustCovalenceProvider.Instance;
		}

		public Item()
		{
		}

		public static Item GetItem(int itemId)
		{
			return ItemManager.CreateByItemID(itemId, 1, (ulong)0);
		}
	}
}