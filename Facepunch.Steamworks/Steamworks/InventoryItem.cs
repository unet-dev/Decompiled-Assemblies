using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks
{
	public struct InventoryItem : IEquatable<InventoryItem>
	{
		internal InventoryItemId _id;

		internal InventoryDefId _def;

		internal SteamItemFlags _flags;

		internal ushort _quantity;

		internal Dictionary<string, string> _properties;

		public DateTime Acquired
		{
			get
			{
				DateTime dateTime;
				if (this.Properties != null)
				{
					string item = this.Properties["acquired"];
					int num = Int32.Parse(item.Substring(0, 4));
					int num1 = Int32.Parse(item.Substring(4, 2));
					int num2 = Int32.Parse(item.Substring(6, 2));
					int num3 = Int32.Parse(item.Substring(9, 2));
					int num4 = Int32.Parse(item.Substring(11, 2));
					int num5 = Int32.Parse(item.Substring(13, 2));
					dateTime = new DateTime(num, num1, num2, num3, num4, num5, DateTimeKind.Utc);
				}
				else
				{
					dateTime = DateTime.UtcNow;
				}
				return dateTime;
			}
		}

		public InventoryDef Def
		{
			get
			{
				return SteamInventory.FindDefinition(this.DefId);
			}
		}

		public InventoryDefId DefId
		{
			get
			{
				return this._def;
			}
		}

		public InventoryItemId Id
		{
			get
			{
				return this._id;
			}
		}

		public bool IsConsumed
		{
			get
			{
				return this._flags.HasFlag(SteamItemFlags.Consumed);
			}
		}

		public bool IsNoTrade
		{
			get
			{
				return this._flags.HasFlag(SteamItemFlags.NoTrade);
			}
		}

		public bool IsRemoved
		{
			get
			{
				return this._flags.HasFlag(SteamItemFlags.Removed);
			}
		}

		public string Origin
		{
			get
			{
				string item;
				if (this.Properties != null)
				{
					item = this.Properties["origin"];
				}
				else
				{
					item = null;
				}
				return item;
			}
		}

		public Dictionary<string, string> Properties
		{
			get
			{
				return this._properties;
			}
		}

		public int Quantity
		{
			get
			{
				return this._quantity;
			}
		}

		public async Task<InventoryResult?> AddAsync(InventoryItem add, int quantity = 1)
		{
			InventoryResult? async;
			SteamInventoryResult_t steamInventoryResultT = new SteamInventoryResult_t();
			if (SteamInventory.Internal.TransferItemQuantity(ref steamInventoryResultT, add.Id, (uint)quantity, this.Id))
			{
				async = await InventoryResult.GetAsync(steamInventoryResultT);
			}
			else
			{
				async = null;
			}
			return async;
		}

		public async Task<InventoryResult?> ConsumeAsync(int amount = 1)
		{
			InventoryResult? async;
			SteamInventoryResult_t steamInventoryResultT = new SteamInventoryResult_t();
			if (SteamInventory.Internal.ConsumeItem(ref steamInventoryResultT, this.Id, (uint)amount))
			{
				async = await InventoryResult.GetAsync(steamInventoryResultT);
			}
			else
			{
				async = null;
			}
			return async;
		}

		public override bool Equals(object p)
		{
			return this.Equals((InventoryItem)p);
		}

		public bool Equals(InventoryItem p)
		{
			return p._id == this._id;
		}

		internal static InventoryItem From(SteamItemDetails_t details)
		{
			InventoryItem inventoryItem = new InventoryItem()
			{
				_id = details.ItemId,
				_def = details.Definition,
				_flags = details.Flags,
				_quantity = details.Quantity
			};
			return inventoryItem;
		}

		public override int GetHashCode()
		{
			return this._id.GetHashCode();
		}

		internal static Dictionary<string, string> GetProperties(SteamInventoryResult_t result, int index)
		{
			Dictionary<string, string> strs;
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			uint capacity = (uint)stringBuilder.Capacity;
			if (SteamInventory.Internal.GetResultItemProperty(result, (uint)index, null, stringBuilder, ref capacity))
			{
				string str = stringBuilder.ToString();
				Dictionary<string, string> strs1 = new Dictionary<string, string>();
				string[] strArray = str.Split(new Char[] { ',' });
				for (int i = 0; i < (int)strArray.Length; i++)
				{
					string str1 = strArray[i];
					capacity = (uint)stringBuilder.Capacity;
					if (SteamInventory.Internal.GetResultItemProperty(result, (uint)index, str1, stringBuilder, ref capacity))
					{
						strs1.Add(str1, stringBuilder.ToString());
					}
				}
				strs = strs1;
			}
			else
			{
				strs = null;
			}
			return strs;
		}

		public static bool operator ==(InventoryItem a, InventoryItem b)
		{
			return a._id == b._id;
		}

		public static bool operator !=(InventoryItem a, InventoryItem b)
		{
			return a._id != b._id;
		}

		public async Task<InventoryResult?> SplitStackAsync(int quantity = 1)
		{
			InventoryResult? async;
			SteamInventoryResult_t steamInventoryResultT = new SteamInventoryResult_t();
			if (SteamInventory.Internal.TransferItemQuantity(ref steamInventoryResultT, this.Id, (uint)quantity, (long)-1))
			{
				async = await InventoryResult.GetAsync(steamInventoryResultT);
			}
			else
			{
				async = null;
			}
			return async;
		}

		public struct Amount
		{
			public InventoryItem Item;

			public int Quantity;
		}
	}
}