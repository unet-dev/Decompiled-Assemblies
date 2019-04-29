using Ionic.Crc;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Rust.Workshop
{
	public class ApprovedSkinInfo
	{
		public bool AllowInCrates { get; private set; } = true;

		public string Desc
		{
			get;
			private set;
		}

		public Rust.Workshop.DropChance DropChance
		{
			get;
			private set;
		}

		public ulong InventoryId
		{
			get;
			private set;
		}

		public bool Marketable
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		internal Rust.Workshop.Price Price
		{
			get;
			private set;
		}

		public Skinnable Skinnable
		{
			get;
			private set;
		}

		public bool Tradable
		{
			get;
			private set;
		}

		public ulong WorkshopdId
		{
			get;
			private set;
		}

		public ApprovedSkinInfo(ulong WorkshopdId, string Name, string Desc, string ItemName)
		{
			this.WorkshopdId = WorkshopdId;
			this.InventoryId = WorkshopdId;
			this.Name = Name;
			this.Desc = Desc;
			this.Skinnable = Skinnable.All.FirstOrDefault<Skinnable>((Skinnable x) => string.Compare(x.Name, ItemName, StringComparison.OrdinalIgnoreCase) == 0);
			if (this.Skinnable == null)
			{
				throw new Exception(string.Concat("Unknown Item Type: ", ItemName));
			}
		}

		public ApprovedSkinInfo Drops(Rust.Workshop.DropChance DropChance)
		{
			this.DropChance = DropChance;
			return this;
		}

		public ApprovedSkinInfo ItemId(ulong InventoryId)
		{
			this.InventoryId = InventoryId;
			return this;
		}

		public ApprovedSkinInfo NoCrate()
		{
			this.AllowInCrates = false;
			return this;
		}

		public static implicit operator Item(ApprovedSkinInfo o)
		{
			string str;
			string str1;
			ItemSchema.Item item = new ItemSchema.Item();
			Category category = o.Skinnable.Category;
			string str2 = "cat.none;";
			if (o.Marketable)
			{
				if (o.Skinnable.Category == Category.Pants || o.Skinnable.Category == Category.Shirt || o.Skinnable.Category == Category.Jacket || o.Skinnable.Category == Category.Hat || o.Skinnable.Category == Category.Mask || o.Skinnable.Category == Category.Footwear)
				{
					str2 = "cat.clothing;breakdown.cloth;";
				}
				if (o.Skinnable.Category == Category.Weapon)
				{
					str2 = "cat.weapon;breakdown.metal;";
				}
				if (o.Skinnable.Category == Category.Misc || o.Skinnable.Category == Category.Deployable)
				{
					str2 = "cat.deployable;breakdown.wood;";
				}
			}
			if (category == Category.Deployable)
			{
				category = Category.Misc;
			}
			if (!o.AllowInCrates)
			{
				str2 = string.Concat(str2, "nocrate;");
			}
			if (string.IsNullOrEmpty(o.Skinnable.ItemName))
			{
				throw new Exception(string.Concat("Item Type Has No ItemName: ", o.Skinnable.Name));
			}
			string str3 = "5";
			if (File.Exists(string.Concat("Prerequisites/SteamInventory/Icons/", o.InventoryId, ".png")))
			{
				byte[] numArray = File.ReadAllBytes(string.Concat("Prerequisites/SteamInventory/Icons/", o.InventoryId, ".png"));
				CRC32 cRC32 = new CRC32();
				cRC32.SlurpBlock(numArray, 0, (int)numArray.Length);
				str3 = cRC32.Crc32Result.ToString();
			}
			item.itemdefid = (uint)o.InventoryId;
			item.name = o.Name;
			item.type = o.Skinnable.Category.ToString();
			item.icon_url = string.Concat(new object[] { "http://s3.amazonaws.com/s3.playrust.com/icons/inventory/rust/", o.InventoryId, "_small.png?", str3 });
			item.icon_url_large = string.Concat(new object[] { "http://s3.amazonaws.com/s3.playrust.com/icons/inventory/rust/", o.InventoryId, "_large.png?", str3 });
			item.marketable = o.Marketable;
			item.tradable = o.Tradable;
			item.commodity = true;
			item.market_hash_name = o.Name;
			item.market_name = o.Name;
			item.description = o.Desc;
			ItemSchema.Item item1 = item;
			if (o.Price == Rust.Workshop.Price.NotForSale)
			{
				str = null;
			}
			else
			{
				str = string.Concat("1;VLV", (int)o.Price);
			}
			item1.price_category = str;
			item.dropChance = o.DropChance;
			item.itemshortname = o.Skinnable.ItemName;
			ItemSchema.Item item2 = item;
			if (o.WorkshopdId > (long)0)
			{
				str1 = o.WorkshopdId.ToString();
			}
			else
			{
				str1 = null;
			}
			item2.workshopdownload = str1;
			item.workshopid = item.workshopdownload;
			item.store_tags = string.Format("{2}subcat.{0};item.{1}", category.ToString().ToLower(), (string.IsNullOrEmpty(o.Skinnable.ItemName) ? "none" : o.Skinnable.ItemName.ToLower()), str2);
			return item;
		}

		public ApprovedSkinInfo Store(Rust.Workshop.Price Price, bool CanBeTraded, bool CanBeSold)
		{
			this.Price = Price;
			this.Tradable = CanBeTraded;
			this.Marketable = CanBeSold;
			return this;
		}
	}
}