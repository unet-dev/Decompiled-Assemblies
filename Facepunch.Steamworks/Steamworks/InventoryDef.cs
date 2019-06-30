using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Steamworks
{
	public class InventoryDef : IEquatable<InventoryDef>
	{
		internal InventoryDefId _id;

		internal Dictionary<string, string> _properties;

		private InventoryRecipe[] _recContaining;

		public DateTime Created
		{
			get
			{
				return this.GetProperty<DateTime>("timestamp");
			}
		}

		public string Description
		{
			get
			{
				return this.GetProperty("description");
			}
		}

		public string ExchangeSchema
		{
			get
			{
				return this.GetProperty("exchange");
			}
		}

		public string IconUrl
		{
			get
			{
				return this.GetProperty("icon_url");
			}
		}

		public string IconUrlLarge
		{
			get
			{
				return this.GetProperty("icon_url_large");
			}
		}

		public int Id
		{
			get
			{
				return this._id.Value;
			}
		}

		public bool IsGenerator
		{
			get
			{
				return this.Type == "generator";
			}
		}

		public int LocalBasePrice
		{
			get
			{
				int num;
				ulong num1 = (ulong)0;
				ulong num2 = (ulong)0;
				num = (SteamInventory.Internal.GetItemPrice(this.Id, ref num1, ref num2) ? (int)num2 : 0);
				return num;
			}
		}

		public string LocalBasePriceFormatted
		{
			get
			{
				return Utility.FormatPrice(SteamInventory.Currency, (double)this.LocalPrice / 100);
			}
		}

		public int LocalPrice
		{
			get
			{
				int num;
				ulong num1 = (ulong)0;
				ulong num2 = (ulong)0;
				num = (SteamInventory.Internal.GetItemPrice(this.Id, ref num1, ref num2) ? (int)num1 : 0);
				return num;
			}
		}

		public string LocalPriceFormatted
		{
			get
			{
				return Utility.FormatPrice(SteamInventory.Currency, (double)this.LocalPrice / 100);
			}
		}

		public bool Marketable
		{
			get
			{
				return this.GetBoolProperty("marketable");
			}
		}

		public DateTime Modified
		{
			get
			{
				return this.GetProperty<DateTime>("modified");
			}
		}

		public string Name
		{
			get
			{
				return this.GetProperty("name");
			}
		}

		public string PriceCategory
		{
			get
			{
				return this.GetProperty("price_category");
			}
		}

		public IEnumerable<KeyValuePair<string, string>> Properties
		{
			get
			{
				string property = this.GetProperty(null);
				string[] strArray = property.Split(new Char[] { ',' });
				for (int i = 0; i < (int)strArray.Length; i++)
				{
					string str = strArray[i];
					yield return new KeyValuePair<string, string>(str, this.GetProperty(str));
					str = null;
				}
				strArray = null;
			}
		}

		public bool Tradable
		{
			get
			{
				return this.GetBoolProperty("tradable");
			}
		}

		public string Type
		{
			get
			{
				return this.GetProperty("type");
			}
		}

		public InventoryDef(InventoryDefId defId)
		{
			this._id = defId;
		}

		public override bool Equals(object p)
		{
			return this.Equals((InventoryDef)p);
		}

		public bool Equals(InventoryDef p)
		{
			bool flag;
			flag = (p != null ? p.Id == this.Id : false);
			return flag;
		}

		public bool GetBoolProperty(string name)
		{
			bool flag;
			string property = this.GetProperty(name);
			if (property.Length != 0)
			{
				flag = ((property[0] == '0' || property[0] == 'F' ? false : property[0] != 'f') ? true : false);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}

		public string GetProperty(string name)
		{
			string str = null;
			string str1;
			if ((this._properties == null ? true : !this._properties.TryGetValue(name, out str)))
			{
				StringBuilder stringBuilder = Helpers.TakeStringBuilder();
				uint capacity = (uint)stringBuilder.Capacity;
				if (SteamInventory.Internal.GetItemDefinitionProperty(this.Id, name, stringBuilder, ref capacity))
				{
					if (this._properties == null)
					{
						this._properties = new Dictionary<string, string>();
					}
					string str2 = stringBuilder.ToString();
					this._properties[name] = str2;
					str1 = str2;
				}
				else
				{
					str1 = null;
				}
			}
			else
			{
				str1 = str;
			}
			return str1;
		}

		public T GetProperty<T>(string name)
		{
			T t;
			T t1;
			string property = this.GetProperty(name);
			if (!String.IsNullOrEmpty(property))
			{
				try
				{
					t1 = (T)Convert.ChangeType(property, typeof(T));
				}
				catch (Exception exception)
				{
					t = default(T);
					t1 = t;
				}
			}
			else
			{
				t = default(T);
				t1 = t;
			}
			return t1;
		}

		public InventoryRecipe[] GetRecipes()
		{
			InventoryRecipe[] array;
			if (!String.IsNullOrEmpty(this.ExchangeSchema))
			{
				string[] strArray = this.ExchangeSchema.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				array = (
					from x in strArray
					select InventoryRecipe.FromString(x, this)).ToArray<InventoryRecipe>();
			}
			else
			{
				array = null;
			}
			return array;
		}

		public InventoryRecipe[] GetRecipesContainingThis()
		{
			InventoryRecipe[] inventoryRecipeArray;
			if (this._recContaining == null)
			{
				IEnumerable<InventoryRecipe> inventoryRecipes = (
					from x in (IEnumerable<InventoryDef>)SteamInventory.Definitions
					select x.GetRecipes() into x
					where x != null
					select x).SelectMany<InventoryRecipe[], InventoryRecipe>((InventoryRecipe[] x) => x);
				this._recContaining = (
					from x in inventoryRecipes
					where x.ContainsIngredient(this)
					select x).ToArray<InventoryRecipe>();
				inventoryRecipeArray = this._recContaining;
			}
			else
			{
				inventoryRecipeArray = this._recContaining;
			}
			return inventoryRecipeArray;
		}

		public static bool operator ==(InventoryDef a, InventoryDef b)
		{
			bool flag;
			flag = (a != null ? a.Equals(b) : b == null);
			return flag;
		}

		public static bool operator !=(InventoryDef a, InventoryDef b)
		{
			return !(a == b);
		}
	}
}