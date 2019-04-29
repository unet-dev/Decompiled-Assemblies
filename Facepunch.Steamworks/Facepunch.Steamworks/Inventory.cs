using Facepunch.Steamworks.Callbacks;
using SteamNative;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Facepunch.Steamworks
{
	public class Inventory : IDisposable
	{
		public Inventory.Item[] Items;

		public byte[] SerializedItems;

		public DateTime SerializedExpireTime;

		public bool EnableItemProperties = true;

		internal uint LastTimestamp;

		internal SteamInventory inventory;

		public Inventory.Definition[] Definitions;

		public string Currency
		{
			get;
			private set;
		}

		public IEnumerable<Inventory.Definition> DefinitionsWithPrices
		{
			get
			{
				Inventory inventory = null;
				if (inventory.Definitions == null)
				{
					yield break;
				}
				for (int i = 0; i < (int)inventory.Definitions.Length; i++)
				{
					if (inventory.Definitions[i].LocalPrice > 0)
					{
						yield return inventory.Definitions[i];
					}
				}
			}
		}

		private bool IsServer
		{
			get;
			set;
		}

		internal Inventory(BaseSteamworks steamworks, SteamInventory c, bool server)
		{
			this.IsServer = server;
			this.inventory = c;
			steamworks.RegisterCallback<SteamInventoryDefinitionUpdate_t>(new Action<SteamInventoryDefinitionUpdate_t>(this.onDefinitionsUpdated));
			Inventory.Result.Pending = new Dictionary<int, Inventory.Result>();
			this.FetchItemDefinitions();
			this.LoadDefinitions();
			this.UpdatePrices();
			if (!server)
			{
				steamworks.RegisterCallback<SteamInventoryResultReady_t>(new Action<SteamInventoryResultReady_t>(this.onResultReady));
				steamworks.RegisterCallback<SteamInventoryFullUpdate_t>(new Action<SteamInventoryFullUpdate_t>(this.onFullUpdate));
				this.Refresh();
			}
		}

		internal void ApplyResult(Inventory.Result r, bool isFullUpdate)
		{
			if (this.IsServer)
			{
				return;
			}
			if (r.IsSuccess && r.Items != null)
			{
				if (this.Items == null)
				{
					this.Items = new Inventory.Item[0];
				}
				if (!isFullUpdate)
				{
					Inventory.Item[] items = this.Items;
					Inventory.Item[] itemArray = r.Items;
					this.Items = (
						from x in ((IEnumerable<Inventory.Item>)items).UnionSelect<Inventory.Item>((IEnumerable<Inventory.Item>)itemArray, (Inventory.Item oldItem, Inventory.Item newItem) => newItem)
						where !r.Removed.Contains<Inventory.Item>(x)
						where !r.Consumed.Contains<Inventory.Item>(x)
						select x).ToArray<Inventory.Item>();
				}
				else
				{
					this.Items = r.Items;
				}
				Action action = this.OnUpdate;
				if (action == null)
				{
					return;
				}
				action();
			}
		}

		public Inventory.Result CraftItem(Inventory.Item[] list, Inventory.Definition target)
		{
			SteamInventoryResult_t steamInventoryResultT = -1;
			SteamItemDef_t[] steamItemDefTArray = new SteamItemDef_t[1];
			SteamItemDef_t steamItemDefT = new SteamItemDef_t()
			{
				Value = target.Id
			};
			steamItemDefTArray[0] = steamItemDefT;
			SteamItemDef_t[] steamItemDefTArray1 = steamItemDefTArray;
			uint[] numArray = new uint[] { 1 };
			SteamItemInstanceID_t[] array = (
				from x in (IEnumerable<Inventory.Item>)list
				select x.Id).ToArray<SteamItemInstanceID_t>();
			uint[] array1 = (
				from x in (IEnumerable<Inventory.Item>)list
				select (uint)1).ToArray<uint>();
			if (!this.inventory.ExchangeItems(ref steamInventoryResultT, steamItemDefTArray1, numArray, 1, array, array1, (uint)array.Length))
			{
				return null;
			}
			return new Inventory.Result(this, steamInventoryResultT, true);
		}

		public Inventory.Result CraftItem(Inventory.Item.Amount[] list, Inventory.Definition target)
		{
			SteamInventoryResult_t steamInventoryResultT = -1;
			SteamItemDef_t[] steamItemDefTArray = new SteamItemDef_t[1];
			SteamItemDef_t steamItemDefT = new SteamItemDef_t()
			{
				Value = target.Id
			};
			steamItemDefTArray[0] = steamItemDefT;
			SteamItemDef_t[] steamItemDefTArray1 = steamItemDefTArray;
			uint[] numArray = new uint[] { 1 };
			SteamItemInstanceID_t[] array = (
				from x in (IEnumerable<Inventory.Item.Amount>)list
				select x.Item.Id).ToArray<SteamItemInstanceID_t>();
			uint[] array1 = (
				from x in (IEnumerable<Inventory.Item.Amount>)list
				select (uint)x.Quantity).ToArray<uint>();
			if (!this.inventory.ExchangeItems(ref steamInventoryResultT, steamItemDefTArray1, numArray, 1, array, array1, (uint)array.Length))
			{
				return null;
			}
			return new Inventory.Result(this, steamInventoryResultT, true);
		}

		public Inventory.Definition CreateDefinition(int id)
		{
			return new Inventory.Definition(this, id);
		}

		public unsafe Inventory.Result Deserialize(byte[] data, int dataLength = -1)
		{
			// 
			// Current member / type: Facepunch.Steamworks.Inventory/Result Facepunch.Steamworks.Inventory::Deserialize(System.Byte[],System.Int32)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: Facepunch.Steamworks.Inventory/Result Deserialize(System.Byte[],System.Int32)
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 109
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 143
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 73
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public void Dispose()
		{
			this.inventory = null;
			this.Items = null;
			this.SerializedItems = null;
			Inventory.Result.Pending = null;
		}

		public void FetchItemDefinitions()
		{
			this.inventory.LoadItemDefinitions();
		}

		public Inventory.Definition FindDefinition(int DefinitionId)
		{
			if (this.Definitions == null)
			{
				return null;
			}
			for (int i = 0; i < (int)this.Definitions.Length; i++)
			{
				if (this.Definitions[i].Id == DefinitionId)
				{
					return this.Definitions[i];
				}
			}
			return null;
		}

		public Inventory.Result GenerateItem(Inventory.Definition target, int amount)
		{
			SteamInventoryResult_t steamInventoryResultT = -1;
			SteamItemDef_t[] steamItemDefTArray = new SteamItemDef_t[1];
			SteamItemDef_t steamItemDefT = new SteamItemDef_t()
			{
				Value = target.Id
			};
			steamItemDefTArray[0] = steamItemDefT;
			SteamItemDef_t[] steamItemDefTArray1 = steamItemDefTArray;
			uint[] numArray = new uint[] { amount };
			if (!this.inventory.GenerateItems(ref steamInventoryResultT, steamItemDefTArray1, numArray, 1))
			{
				return null;
			}
			return new Inventory.Result(this, steamInventoryResultT, true);
		}

		public void GrantAllPromoItems()
		{
			SteamInventoryResult_t steamInventoryResultT = 0;
			this.inventory.GrantPromoItems(ref steamInventoryResultT);
			this.inventory.DestroyResult(steamInventoryResultT);
		}

		internal Inventory.Item ItemFrom(SteamInventoryResult_t handle, SteamItemDetails_t detail, int index)
		{
			string str;
			string str1;
			Dictionary<string, string> strs = null;
			if (this.EnableItemProperties && this.inventory.GetResultItemProperty(handle, (uint)index, null, out str))
			{
				strs = new Dictionary<string, string>();
				string[] strArrays = str.Split(new char[] { ',' });
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					string str2 = strArrays[i];
					if (this.inventory.GetResultItemProperty(handle, (uint)index, str2, out str1))
					{
						if (str2 == "error")
						{
							Console.Write("Steam item error: ");
							Console.WriteLine(str1);
							return null;
						}
						strs.Add(str2, str1);
					}
				}
			}
			return new Inventory.Item(this, detail.ItemId, (int)detail.Quantity, detail.Definition)
			{
				Properties = strs
			};
		}

		private bool LoadDefinitions()
		{
			SteamItemDef_t[] itemDefinitionIDs = this.inventory.GetItemDefinitionIDs();
			if (itemDefinitionIDs == null)
			{
				return false;
			}
			this.Definitions = (
				from x in itemDefinitionIDs
				select this.CreateDefinition(x)).ToArray<Inventory.Definition>();
			Inventory.Definition[] definitions = this.Definitions;
			for (int i = 0; i < (int)definitions.Length; i++)
			{
				definitions[i].Link(this.Definitions);
			}
			return true;
		}

		private void onDefinitionsUpdated(SteamInventoryDefinitionUpdate_t obj)
		{
			this.LoadDefinitions();
			this.UpdatePrices();
			if (this.OnDefinitionsUpdated != null)
			{
				this.OnDefinitionsUpdated();
			}
		}

		private void onFullUpdate(SteamInventoryFullUpdate_t data)
		{
			Inventory.Result result = new Inventory.Result(this, data.Handle, false);
			result.Fill();
			this.onResult(result, true);
		}

		private void onResult(Inventory.Result r, bool isFullUpdate)
		{
			if (r.IsSuccess)
			{
				if (isFullUpdate)
				{
					if (r.Timestamp < this.LastTimestamp)
					{
						return;
					}
					this.SerializedItems = r.Serialize();
					DateTime now = DateTime.Now;
					this.SerializedExpireTime = now.Add(TimeSpan.FromMinutes(60));
				}
				this.LastTimestamp = r.Timestamp;
				this.ApplyResult(r, isFullUpdate);
			}
			r.Dispose();
			r = null;
		}

		private void onResultReady(SteamInventoryResultReady_t data)
		{
			Inventory.Result result;
			if (!Inventory.Result.Pending.TryGetValue(data.Handle, out result))
			{
				result = new Inventory.Result(this, data.Handle, false);
				result.Fill();
			}
			else
			{
				result.OnSteamResult(data);
				if (data.Result == SteamNative.Result.OK)
				{
					this.onResult(result, false);
				}
				Inventory.Result.Pending.Remove(data.Handle);
				result.Dispose();
			}
			Action<Inventory.Result> action = this.OnInventoryResultReady;
			if (action == null)
			{
				return;
			}
			action(result);
		}

		[Obsolete("No longer required, will be removed in a later version")]
		public void PlaytimeHeartbeat()
		{
		}

		public static float PriceCategoryToFloat(string price)
		{
			if (string.IsNullOrEmpty(price))
			{
				return 0f;
			}
			price = price.Replace("1;VLV", "");
			int num = 0;
			if (!int.TryParse(price, out num))
			{
				return 0f;
			}
			return (float)int.Parse(price) / 100f;
		}

		public void Refresh()
		{
			if (this.IsServer)
			{
				return;
			}
			SteamInventoryResult_t steamInventoryResultT = 0;
			if (this.inventory.GetAllItems(ref steamInventoryResultT) && steamInventoryResultT != -1)
			{
				return;
			}
			Console.WriteLine("GetAllItems failed!?");
		}

		public Inventory.Result SplitStack(Inventory.Item item, int quantity = 1)
		{
			return item.SplitStack(quantity);
		}

		public Inventory.Result Stack(Inventory.Item source, Inventory.Item dest, int quantity = 1)
		{
			SteamInventoryResult_t steamInventoryResultT = -1;
			if (!this.inventory.TransferItemQuantity(ref steamInventoryResultT, source.Id, (uint)quantity, dest.Id))
			{
				return null;
			}
			return new Inventory.Result(this, steamInventoryResultT, true);
		}

		public bool StartPurchase(Inventory.Definition[] items, Inventory.StartPurchaseSuccess callback = null)
		{
			IEnumerable<IGrouping<int, Inventory.Definition>> groupings = 
				from x in (IEnumerable<Inventory.Definition>)items
				group x by x.Id;
			SteamItemDef_t[] array = (
				from x in groupings
				select new SteamItemDef_t()
				{
					Value = x.Key
				}).ToArray<SteamItemDef_t>();
			uint[] numArray = (
				from x in groupings
				select (uint)x.Count<Inventory.Definition>()).ToArray<uint>();
			return this.inventory.StartPurchase(array, numArray, (uint)numArray.Length, (SteamInventoryStartPurchaseResult_t result, bool error) => {
				if (error)
				{
					Inventory.StartPurchaseSuccess startPurchaseSuccess = callback;
					if (startPurchaseSuccess == null)
					{
						return;
					}
					startPurchaseSuccess((long)0, (long)0);
					return;
				}
				Inventory.StartPurchaseSuccess startPurchaseSuccess1 = callback;
				if (startPurchaseSuccess1 == null)
				{
					return;
				}
				startPurchaseSuccess1(result.OrderID, result.TransID);
			}) != null;
		}

		public void TriggerItemDrop(int definitionId)
		{
			SteamInventoryResult_t steamInventoryResultT = 0;
			this.inventory.TriggerItemDrop(ref steamInventoryResultT, definitionId);
			this.inventory.DestroyResult(steamInventoryResultT);
		}

		public void TriggerPromoDrop(int definitionId)
		{
			SteamInventoryResult_t steamInventoryResultT = 0;
			this.inventory.AddPromoItem(ref steamInventoryResultT, definitionId);
			this.inventory.DestroyResult(steamInventoryResultT);
		}

		public void Update()
		{
		}

		public void UpdatePrices()
		{
			if (this.IsServer)
			{
				return;
			}
			this.inventory.RequestPrices((SteamInventoryRequestPricesResult_t result, bool b) => {
				this.Currency = result.Currency;
				if (this.Definitions == null)
				{
					return;
				}
				for (int i = 0; i < (int)this.Definitions.Length; i++)
				{
					this.Definitions[i].UpdatePrice();
				}
				Action action = this.OnUpdate;
				if (action == null)
				{
					return;
				}
				action();
			});
		}

		public event Action OnDefinitionsUpdated;

		public event Action<Inventory.Result> OnInventoryResultReady;

		public event Action OnUpdate;

		public class Definition
		{
			internal Inventory inventory;

			private Dictionary<string, string> customProperties;

			public DateTime Created
			{
				get;
				set;
			}

			public string Description
			{
				get;
				set;
			}

			public string ExchangeSchema
			{
				get;
				set;
			}

			public string IconLargeUrl
			{
				get;
				set;
			}

			public string IconUrl
			{
				get;
				set;
			}

			public int Id
			{
				get;
				private set;
			}

			public Inventory.Recipe[] IngredientFor
			{
				get;
				set;
			}

			public bool IsGenerator
			{
				get
				{
					return this.Type == "generator";
				}
			}

			public double LocalPrice
			{
				get;
				internal set;
			}

			public string LocalPriceFormatted
			{
				get;
				internal set;
			}

			public bool Marketable
			{
				get;
				set;
			}

			public DateTime Modified
			{
				get;
				set;
			}

			public string Name
			{
				get;
				set;
			}

			public string PriceCategory
			{
				get;
				set;
			}

			public double PriceDollars
			{
				get;
				internal set;
			}

			public Inventory.Recipe[] Recipes
			{
				get;
				set;
			}

			public string Type
			{
				get;
				set;
			}

			internal Definition(Inventory i, int id)
			{
				this.inventory = i;
				this.Id = id;
				this.SetupCommonProperties();
				this.UpdatePrice();
			}

			public bool GetBoolProperty(string name)
			{
				string stringProperty = this.GetStringProperty(name);
				if (stringProperty.Length == 0)
				{
					return false;
				}
				if (stringProperty[0] != '0' && stringProperty[0] != 'F' && stringProperty[0] != 'f')
				{
					return true;
				}
				return false;
			}

			public string GetCachedStringProperty(string name)
			{
				string empty = string.Empty;
				if (this.customProperties == null)
				{
					this.customProperties = new Dictionary<string, string>();
				}
				if (!this.customProperties.TryGetValue(name, out empty))
				{
					this.inventory.inventory.GetItemDefinitionProperty(this.Id, name, out empty);
					this.customProperties.Add(name, empty);
				}
				return empty;
			}

			public T GetProperty<T>(string name)
			{
				T t;
				string stringProperty = this.GetStringProperty(name);
				if (string.IsNullOrEmpty(stringProperty))
				{
					t = default(T);
					return t;
				}
				try
				{
					t = (T)Convert.ChangeType(stringProperty, typeof(T));
				}
				catch (Exception exception)
				{
					t = default(T);
				}
				return t;
			}

			public string GetStringProperty(string name)
			{
				string empty = string.Empty;
				if (this.customProperties != null && this.customProperties.ContainsKey(name))
				{
					return this.customProperties[name];
				}
				if (!this.inventory.inventory.GetItemDefinitionProperty(this.Id, name, out empty))
				{
					return string.Empty;
				}
				return empty;
			}

			internal void InRecipe(Inventory.Recipe r)
			{
				if (this.IngredientFor == null)
				{
					this.IngredientFor = new Inventory.Recipe[0];
				}
				List<Inventory.Recipe> recipes = new List<Inventory.Recipe>(this.IngredientFor)
				{
					r
				};
				this.IngredientFor = recipes.ToArray();
			}

			internal void Link(Inventory.Definition[] definitions)
			{
				this.LinkExchange(definitions);
			}

			private void LinkExchange(Inventory.Definition[] definitions)
			{
				if (string.IsNullOrEmpty(this.ExchangeSchema))
				{
					return;
				}
				string[] strArrays = this.ExchangeSchema.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				this.Recipes = (
					from x in strArrays
					select Inventory.Recipe.FromString(x, definitions, this)).ToArray<Inventory.Recipe>();
			}

			public void SetProperty(string name, string value)
			{
				if (this.customProperties == null)
				{
					this.customProperties = new Dictionary<string, string>();
				}
				if (!this.customProperties.ContainsKey(name))
				{
					this.customProperties.Add(name, value);
					return;
				}
				this.customProperties[name] = value;
			}

			internal void SetupCommonProperties()
			{
				this.Name = this.GetStringProperty("name");
				this.Description = this.GetStringProperty("description");
				this.Created = this.GetProperty<DateTime>("timestamp");
				this.Modified = this.GetProperty<DateTime>("modified");
				this.ExchangeSchema = this.GetStringProperty("exchange");
				this.IconUrl = this.GetStringProperty("icon_url");
				this.IconLargeUrl = this.GetStringProperty("icon_url_large");
				this.Type = this.GetStringProperty("type");
				this.PriceCategory = this.GetStringProperty("price_category");
				this.Marketable = this.GetBoolProperty("marketable");
				if (!string.IsNullOrEmpty(this.PriceCategory))
				{
					this.PriceDollars = (double)Inventory.PriceCategoryToFloat(this.PriceCategory);
				}
			}

			public void TriggerItemDrop()
			{
				this.inventory.TriggerItemDrop(this.Id);
			}

			public void TriggerPromoDrop()
			{
				this.inventory.TriggerPromoDrop(this.Id);
			}

			internal void UpdatePrice()
			{
				ulong num;
				if (!this.inventory.inventory.GetItemPrice(this.Id, out num))
				{
					this.LocalPrice = 0;
					this.LocalPriceFormatted = null;
					return;
				}
				this.LocalPrice = (double)((float)num) / 100;
				this.LocalPriceFormatted = Utility.FormatPrice(this.inventory.Currency, num);
			}
		}

		public class Item : IEquatable<Inventory.Item>
		{
			public ulong Id;

			public int Quantity;

			public int DefinitionId;

			internal Inventory Inventory;

			private Inventory.Definition _cachedDefinition;

			public bool TradeLocked;

			private SteamInventoryUpdateHandle_t updateHandle;

			public Inventory.Definition Definition
			{
				get
				{
					if (this._cachedDefinition != null)
					{
						return this._cachedDefinition;
					}
					this._cachedDefinition = this.Inventory.FindDefinition(this.DefinitionId);
					return this._cachedDefinition;
				}
			}

			public Dictionary<string, string> Properties
			{
				get;
				internal set;
			}

			internal Item(Facepunch.Steamworks.Inventory Inventory, ulong Id, int Quantity, int DefinitionId)
			{
				this.Inventory = Inventory;
				this.Id = Id;
				this.Quantity = Quantity;
				this.DefinitionId = DefinitionId;
			}

			public Inventory.Result Consume(int amount = 1)
			{
				SteamInventoryResult_t steamInventoryResultT = -1;
				if (!this.Inventory.inventory.ConsumeItem(ref steamInventoryResultT, this.Id, (uint)amount))
				{
					return null;
				}
				return new Inventory.Result(this.Inventory, steamInventoryResultT, true);
			}

			public bool Equals(Inventory.Item other)
			{
				if (other == null)
				{
					return false;
				}
				if ((object)this == (object)other)
				{
					return true;
				}
				return this.Id == other.Id;
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
				{
					return false;
				}
				if (this == obj)
				{
					return true;
				}
				if (obj.GetType() != this.GetType())
				{
					return false;
				}
				return this.Equals((Inventory.Item)obj);
			}

			public override int GetHashCode()
			{
				return this.Id.GetHashCode();
			}

			public static bool operator ==(Inventory.Item left, Inventory.Item right)
			{
				return object.Equals(left, right);
			}

			public static bool operator !=(Inventory.Item left, Inventory.Item right)
			{
				return !object.Equals(left, right);
			}

			public bool SetProperty(string name, string value)
			{
				this.UpdatingProperties();
				this.Properties[name] = value.ToString();
				return this.Inventory.inventory.SetProperty(this.updateHandle, this.Id, name, value);
			}

			public bool SetProperty(string name, bool value)
			{
				this.UpdatingProperties();
				this.Properties[name] = value.ToString();
				return this.Inventory.inventory.SetProperty0(this.updateHandle, this.Id, name, value);
			}

			public bool SetProperty(string name, long value)
			{
				this.UpdatingProperties();
				this.Properties[name] = value.ToString();
				return this.Inventory.inventory.SetProperty1(this.updateHandle, this.Id, name, value);
			}

			public bool SetProperty(string name, float value)
			{
				this.UpdatingProperties();
				this.Properties[name] = value.ToString();
				return this.Inventory.inventory.SetProperty2(this.updateHandle, this.Id, name, value);
			}

			public Inventory.Result SplitStack(int quantity = 1)
			{
				SteamInventoryResult_t steamInventoryResultT = -1;
				if (!this.Inventory.inventory.TransferItemQuantity(ref steamInventoryResultT, this.Id, (uint)quantity, (long)-1))
				{
					return null;
				}
				return new Inventory.Result(this.Inventory, steamInventoryResultT, true);
			}

			public bool SubmitProperties()
			{
				bool flag;
				if (this.updateHandle == 0)
				{
					throw new Exception("SubmitProperties called without updating properties");
				}
				try
				{
					SteamInventoryResult_t steamInventoryResultT = -1;
					if (this.Inventory.inventory.SubmitUpdateProperties(this.updateHandle, ref steamInventoryResultT))
					{
						this.Inventory.inventory.DestroyResult(steamInventoryResultT);
						flag = true;
					}
					else
					{
						flag = false;
					}
				}
				finally
				{
					this.updateHandle = (long)0;
				}
				return flag;
			}

			private void UpdatingProperties()
			{
				if (!this.Inventory.EnableItemProperties)
				{
					throw new InvalidOperationException("Item properties are disabled.");
				}
				if (this.updateHandle != 0)
				{
					return;
				}
				this.updateHandle = this.Inventory.inventory.StartUpdateProperties();
			}

			public struct Amount
			{
				public Inventory.Item Item;

				public int Quantity;
			}
		}

		public struct Recipe
		{
			public Inventory.Definition Result;

			public Inventory.Recipe.Ingredient[] Ingredients;

			internal static Inventory.Recipe FromString(string part, Inventory.Definition[] definitions, Inventory.Definition Result)
			{
				Inventory.Recipe array = new Inventory.Recipe()
				{
					Result = Result
				};
				string[] strArrays = part.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				array.Ingredients = (
					from x in strArrays
					select Inventory.Recipe.Ingredient.FromString(x, definitions) into x
					where x.DefinitionId != 0
					select x).ToArray<Inventory.Recipe.Ingredient>();
				Inventory.Recipe.Ingredient[] ingredients = array.Ingredients;
				for (int i = 0; i < (int)ingredients.Length; i++)
				{
					Inventory.Recipe.Ingredient ingredient = ingredients[i];
					if (ingredient.Definition != null)
					{
						ingredient.Definition.InRecipe(array);
					}
				}
				return array;
			}

			public struct Ingredient
			{
				public int DefinitionId;

				public Inventory.Definition Definition;

				public int Count;

				internal static Inventory.Recipe.Ingredient FromString(string part, Inventory.Definition[] definitions)
				{
					Inventory.Recipe.Ingredient ingredient;
					Inventory.Recipe.Ingredient ingredient1 = new Inventory.Recipe.Ingredient()
					{
						Count = 1
					};
					try
					{
						if (part.Contains<char>('x'))
						{
							int num = part.IndexOf('x');
							int num1 = 0;
							if (int.TryParse(part.Substring(num + 1), out num1))
							{
								ingredient1.Count = num1;
							}
							part = part.Substring(0, num);
						}
						ingredient1.DefinitionId = int.Parse(part);
						ingredient1.Definition = definitions.FirstOrDefault<Inventory.Definition>((Inventory.Definition x) => x.Id == ingredient1.DefinitionId);
						return ingredient1;
					}
					catch (Exception exception)
					{
						ingredient = ingredient1;
					}
					return ingredient;
				}
			}
		}

		public class Result : IDisposable
		{
			internal static Dictionary<int, Inventory.Result> Pending;

			internal Inventory inventory;

			public Action<Inventory.Result> OnResult;

			protected bool _gotResult;

			public Inventory.Item[] Consumed
			{
				get;
				internal set;
			}

			private SteamInventoryResult_t Handle
			{
				get;
				set;
			}

			public bool IsPending
			{
				get
				{
					if (this._gotResult)
					{
						return false;
					}
					if (this.Status() == Facepunch.Steamworks.Callbacks.Result.OK)
					{
						this.Fill();
						return false;
					}
					return this.Status() == Facepunch.Steamworks.Callbacks.Result.Pending;
				}
			}

			internal bool IsSuccess
			{
				get
				{
					if (this.Items != null)
					{
						return true;
					}
					if (this.Handle == -1)
					{
						return false;
					}
					return this.Status() == Facepunch.Steamworks.Callbacks.Result.OK;
				}
			}

			public Inventory.Item[] Items
			{
				get;
				internal set;
			}

			public Inventory.Item[] Removed
			{
				get;
				internal set;
			}

			internal uint Timestamp
			{
				get;
				private set;
			}

			internal Result(Inventory inventory, int Handle, bool pending)
			{
				if (pending)
				{
					Inventory.Result.Pending.Add(Handle, this);
				}
				this.Handle = Handle;
				this.inventory = inventory;
			}

			public void Dispose()
			{
				if (this.Handle != -1 && this.inventory != null)
				{
					this.inventory.inventory.DestroyResult(this.Handle);
					this.Handle = -1;
				}
				this.inventory = null;
			}

			internal void Fill()
			{
				if (this._gotResult)
				{
					return;
				}
				if (this.Items != null)
				{
					return;
				}
				if (this.Status() != Facepunch.Steamworks.Callbacks.Result.OK)
				{
					return;
				}
				this._gotResult = true;
				this.Timestamp = this.inventory.inventory.GetResultTimestamp(this.Handle);
				SteamItemDetails_t[] resultItems = this.inventory.inventory.GetResultItems(this.Handle);
				if (resultItems == null)
				{
					return;
				}
				List<Inventory.Item> items = new List<Inventory.Item>();
				List<Inventory.Item> items1 = new List<Inventory.Item>();
				List<Inventory.Item> items2 = new List<Inventory.Item>();
				for (int i = 0; i < (int)resultItems.Length; i++)
				{
					Inventory.Item item = this.inventory.ItemFrom(this.Handle, resultItems[i], i);
					if (item != null)
					{
						if ((resultItems[i].Flags & 256) != 0)
						{
							items1.Add(item);
						}
						else if ((resultItems[i].Flags & 512) == 0)
						{
							items.Add(item);
						}
						else
						{
							items2.Add(item);
						}
					}
				}
				this.Items = items.ToArray();
				this.Removed = items1.ToArray();
				this.Consumed = items2.ToArray();
				if (this.OnResult != null)
				{
					this.OnResult(this);
				}
			}

			internal void OnSteamResult(SteamInventoryResultReady_t data)
			{
				if (data.Result == SteamNative.Result.OK)
				{
					this.Fill();
				}
			}

			internal unsafe byte[] Serialize()
			{
				// 
				// Current member / type: System.Byte[] Facepunch.Steamworks.Inventory/Result::Serialize()
				// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
				// 
				// Product version: 2019.1.118.0
				// Exception in: System.Byte[] Serialize()
				// 
				// Specified argument was out of the range of valid values.
				// Parameter name: Target of array indexer expression is not an array.
				//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
				//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 109
				//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 95
				//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 143
				//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 73
				//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
				//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
				//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
				//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
				//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com

			}

			internal Facepunch.Steamworks.Callbacks.Result Status()
			{
				if (this.Handle == -1)
				{
					return Facepunch.Steamworks.Callbacks.Result.InvalidParam;
				}
				return (Facepunch.Steamworks.Callbacks.Result)this.inventory.inventory.GetResultStatus(this.Handle);
			}
		}

		public delegate void StartPurchaseSuccess(ulong orderId, ulong transactionId);
	}
}