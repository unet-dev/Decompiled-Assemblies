using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct SteamItemDetails_t
	{
		internal InventoryItemId ItemId;

		internal InventoryDefId Definition;

		internal ushort Quantity;

		internal ushort Flags;

		internal static SteamItemDetails_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamItemDetails_t)Marshal.PtrToStructure(p, typeof(SteamItemDetails_t)) : (SteamItemDetails_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamItemDetails_t.Pack8)));
		}

		public struct Pack8
		{
			internal InventoryItemId ItemId;

			internal InventoryDefId Definition;

			internal ushort Quantity;

			internal ushort Flags;

			public static implicit operator SteamItemDetails_t(SteamItemDetails_t.Pack8 d)
			{
				SteamItemDetails_t steamItemDetailsT = new SteamItemDetails_t()
				{
					ItemId = d.ItemId,
					Definition = d.Definition,
					Quantity = d.Quantity,
					Flags = d.Flags
				};
				return steamItemDetailsT;
			}

			public static implicit operator Pack8(SteamItemDetails_t d)
			{
				SteamItemDetails_t.Pack8 pack8 = new SteamItemDetails_t.Pack8()
				{
					ItemId = d.ItemId,
					Definition = d.Definition,
					Quantity = d.Quantity,
					Flags = d.Flags
				};
				return pack8;
			}
		}
	}
}