using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Steamworks
{
	public static class SteamInventory
	{
		private static ISteamInventory _internal;

		private static Dictionary<int, InventoryDef> _defMap;

		public static string Currency
		{
			get;
			internal set;
		}

		public static InventoryDef[] Definitions
		{
			get;
			internal set;
		}

		internal static ISteamInventory Internal
		{
			get
			{
				if (SteamInventory._internal == null)
				{
					SteamInventory._internal = new ISteamInventory();
					SteamInventory._internal.Init();
				}
				return SteamInventory._internal;
			}
		}

		public static InventoryItem[] Items
		{
			get;
			internal set;
		}

		public static async Task<InventoryResult?> AddPromoItemAsync(InventoryDefId id)
		{
			InventoryResult? async;
			SteamInventoryResult_t steamInventoryResultT = new SteamInventoryResult_t();
			if (SteamInventory.Internal.AddPromoItem(ref steamInventoryResultT, id))
			{
				async = await InventoryResult.GetAsync(steamInventoryResultT);
			}
			else
			{
				async = null;
			}
			return async;
		}

		public static async Task<InventoryResult?> CraftItemAsync(InventoryItem[] list, InventoryDef target)
		{
			InventoryResult? async;
			SteamInventoryResult_t steamInventoryResultT = new SteamInventoryResult_t();
			InventoryDefId[] id = new InventoryDefId[] { target.Id };
			uint[] numArray = new UInt32[] { 1 };
			InventoryItem[] inventoryItemArray = list;
			InventoryItemId[] array = (
				from x in (IEnumerable<InventoryItem>)inventoryItemArray
				select x.Id).ToArray<InventoryItemId>();
			InventoryItem[] inventoryItemArray1 = list;
			uint[] array1 = (
				from x in (IEnumerable<InventoryItem>)inventoryItemArray1
				select (uint)1).ToArray<uint>();
			if (SteamInventory.Internal.ExchangeItems(ref steamInventoryResultT, id, numArray, 1, array, array1, (uint)array.Length))
			{
				async = await InventoryResult.GetAsync(steamInventoryResultT);
			}
			else
			{
				async = null;
			}
			return async;
		}

		public static async Task<InventoryResult?> CraftItemAsync(InventoryItem.Amount[] list, InventoryDef target)
		{
			InventoryResult? async;
			SteamInventoryResult_t steamInventoryResultT = new SteamInventoryResult_t();
			InventoryDefId[] id = new InventoryDefId[] { target.Id };
			uint[] numArray = new UInt32[] { 1 };
			InventoryItem.Amount[] amountArray = list;
			InventoryItemId[] array = (
				from x in (IEnumerable<InventoryItem.Amount>)amountArray
				select x.Item.Id).ToArray<InventoryItemId>();
			InventoryItem.Amount[] amountArray1 = list;
			uint[] array1 = (
				from x in (IEnumerable<InventoryItem.Amount>)amountArray1
				select (uint)x.Quantity).ToArray<uint>();
			if (SteamInventory.Internal.ExchangeItems(ref steamInventoryResultT, id, numArray, 1, array, array1, (uint)array.Length))
			{
				async = await InventoryResult.GetAsync(steamInventoryResultT);
			}
			else
			{
				async = null;
			}
			return async;
		}

		public static async Task<InventoryResult?> DeserializeAsync(byte[] data, int dataLength = -1)
		{
			SteamInventory.<DeserializeAsync>d__36 variable = null;
			AsyncTaskMethodBuilder<InventoryResult?> asyncTaskMethodBuilder = AsyncTaskMethodBuilder<InventoryResult?>.Create();
			asyncTaskMethodBuilder.Start<SteamInventory.<DeserializeAsync>d__36>(ref variable);
			return asyncTaskMethodBuilder.Task;
		}

		public static InventoryDef FindDefinition(InventoryDefId defId)
		{
			InventoryDef inventoryDef;
			InventoryDef inventoryDef1;
			if (SteamInventory._defMap == null)
			{
				inventoryDef1 = null;
			}
			else if (!SteamInventory._defMap.TryGetValue(defId, out inventoryDef))
			{
				inventoryDef1 = null;
			}
			else
			{
				inventoryDef1 = inventoryDef;
			}
			return inventoryDef1;
		}

		public static async Task<InventoryResult?> GenerateItemAsync(InventoryDef target, int amount)
		{
			InventoryResult? async;
			SteamInventoryResult_t steamInventoryResultT = new SteamInventoryResult_t();
			InventoryDefId[] id = new InventoryDefId[] { target.Id };
			uint[] numArray = new UInt32[] { amount };
			if (SteamInventory.Internal.GenerateItems(ref steamInventoryResultT, id, numArray, 1))
			{
				async = await InventoryResult.GetAsync(steamInventoryResultT);
			}
			else
			{
				async = null;
			}
			return async;
		}

		public static bool GetAllItems()
		{
			SteamInventoryResult_t steamInventoryResultT = new SteamInventoryResult_t();
			return SteamInventory.Internal.GetAllItems(ref steamInventoryResultT);
		}

		public static async Task<InventoryResult?> GetAllItemsAsync()
		{
			InventoryResult? async;
			SteamInventoryResult_t steamInventoryResultT = new SteamInventoryResult_t();
			if (SteamInventory.Internal.GetAllItems(ref steamInventoryResultT))
			{
				async = await InventoryResult.GetAsync(steamInventoryResultT);
			}
			else
			{
				async = null;
			}
			return async;
		}

		internal static InventoryDef[] GetDefinitions()
		{
			InventoryDef[] array;
			uint num = 0;
			if (SteamInventory.Internal.GetItemDefinitionIDs(null, ref num))
			{
				InventoryDefId[] inventoryDefIdArray = new InventoryDefId[num];
				if (SteamInventory.Internal.GetItemDefinitionIDs(inventoryDefIdArray, ref num))
				{
					array = (
						from x in (IEnumerable<InventoryDefId>)inventoryDefIdArray
						select new InventoryDef(x)).ToArray<InventoryDef>();
				}
				else
				{
					array = null;
				}
			}
			else
			{
				array = null;
			}
			return array;
		}

		public static async Task<InventoryDef[]> GetDefinitionsWithPricesAsync()
		{
			InventoryDef[] array;
			bool flag;
			string currency;
			SteamInventoryRequestPricesResult_t? nullable = await SteamInventory.Internal.RequestPrices();
			SteamInventoryRequestPricesResult_t? nullable1 = nullable;
			nullable = null;
			flag = (!nullable1.HasValue ? true : nullable1.Value.Result != Result.OK);
			if (!flag)
			{
				ref Nullable nullablePointer = ref nullable1;
				if (nullablePointer.HasValue)
				{
					currency = nullablePointer.GetValueOrDefault().Currency;
				}
				else
				{
					currency = null;
				}
				SteamInventory.Currency = currency;
				uint numItemsWithPrices = SteamInventory.Internal.GetNumItemsWithPrices();
				if (numItemsWithPrices != 0)
				{
					InventoryDefId[] inventoryDefIdArray = new InventoryDefId[numItemsWithPrices];
					ulong[] numArray = new UInt64[numItemsWithPrices];
					ulong[] numArray1 = new UInt64[numItemsWithPrices];
					if (SteamInventory.Internal.GetItemsWithPrices(inventoryDefIdArray, numArray, numArray1, numItemsWithPrices))
					{
						InventoryDefId[] inventoryDefIdArray1 = inventoryDefIdArray;
						array = (
							from x in (IEnumerable<InventoryDefId>)inventoryDefIdArray1
							select new InventoryDef(x)).ToArray<InventoryDef>();
					}
					else
					{
						array = null;
					}
				}
				else
				{
					array = null;
				}
			}
			else
			{
				array = null;
			}
			return array;
		}

		public static async Task<InventoryResult?> GrantPromoItemsAsync()
		{
			InventoryResult? async;
			SteamInventoryResult_t steamInventoryResultT = new SteamInventoryResult_t();
			if (SteamInventory.Internal.GrantPromoItems(ref steamInventoryResultT))
			{
				async = await InventoryResult.GetAsync(steamInventoryResultT);
			}
			else
			{
				async = null;
			}
			return async;
		}

		internal static void InstallEvents()
		{
			SteamInventoryFullUpdate_t.Install((SteamInventoryFullUpdate_t x) => SteamInventory.InventoryUpdated(x), false);
			SteamInventoryDefinitionUpdate_t.Install((SteamInventoryDefinitionUpdate_t x) => SteamInventory.LoadDefinitions(), false);
			SteamInventoryDefinitionUpdate_t.Install((SteamInventoryDefinitionUpdate_t x) => SteamInventory.LoadDefinitions(), true);
		}

		private static void InventoryUpdated(SteamInventoryFullUpdate_t x)
		{
			InventoryResult inventoryResult = new InventoryResult(x.Handle, false);
			SteamInventory.Items = inventoryResult.GetItems(false);
			Action<InventoryResult> action = SteamInventory.OnInventoryUpdated;
			if (action != null)
			{
				action(inventoryResult);
			}
			else
			{
			}
		}

		private static void LoadDefinitions()
		{
			SteamInventory.Definitions = SteamInventory.GetDefinitions();
			if (SteamInventory.Definitions != null)
			{
				SteamInventory._defMap = new Dictionary<int, InventoryDef>();
				InventoryDef[] definitions = SteamInventory.Definitions;
				for (int i = 0; i < (int)definitions.Length; i++)
				{
					InventoryDef inventoryDef = definitions[i];
					SteamInventory._defMap[inventoryDef.Id] = inventoryDef;
				}
				Action action = SteamInventory.OnDefinitionsUpdated;
				if (action != null)
				{
					action();
				}
				else
				{
				}
			}
		}

		public static void LoadItemDefinitions()
		{
			if (SteamInventory.Definitions == null)
			{
				SteamInventory.LoadDefinitions();
			}
			SteamInventory.Internal.LoadItemDefinitions();
		}

		internal static void Shutdown()
		{
			SteamInventory._internal = null;
		}

		public static async Task<InventoryPurchaseResult?> StartPurchaseAsync(InventoryDef[] items)
		{
			InventoryPurchaseResult? nullable;
			InventoryDef[] inventoryDefArray = items;
			InventoryDefId[] array = (
				from x in (IEnumerable<InventoryDef>)inventoryDefArray
				select x._id).ToArray<InventoryDefId>();
			InventoryDef[] inventoryDefArray1 = items;
			uint[] numArray = (
				from x in (IEnumerable<InventoryDef>)inventoryDefArray1
				select (uint)1).ToArray<uint>();
			SteamInventoryStartPurchaseResult_t? nullable1 = await SteamInventory.Internal.StartPurchase(array, numArray, (uint)array.Length);
			SteamInventoryStartPurchaseResult_t? nullable2 = nullable1;
			nullable1 = null;
			if (nullable2.HasValue)
			{
				InventoryPurchaseResult inventoryPurchaseResult = new InventoryPurchaseResult()
				{
					Result = nullable2.Value.Result,
					OrderID = nullable2.Value.OrderID,
					TransID = nullable2.Value.TransID
				};
				nullable = new InventoryPurchaseResult?(inventoryPurchaseResult);
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static async Task<InventoryResult?> TriggerItemDropAsync(InventoryDefId id)
		{
			InventoryResult? async;
			SteamInventoryResult_t steamInventoryResultT = new SteamInventoryResult_t();
			if (SteamInventory.Internal.TriggerItemDrop(ref steamInventoryResultT, id))
			{
				async = await InventoryResult.GetAsync(steamInventoryResultT);
			}
			else
			{
				async = null;
			}
			return async;
		}

		public static async Task<bool> WaitForDefinitions(float timeoutSeconds = 30f)
		{
			bool flag;
			if (SteamInventory.Definitions == null)
			{
				SteamInventory.LoadDefinitions();
				SteamInventory.LoadItemDefinitions();
				if (SteamInventory.Definitions == null)
				{
					Stopwatch stopwatch = Stopwatch.StartNew();
					while (SteamInventory.Definitions == null)
					{
						if (stopwatch.Elapsed.TotalSeconds <= (double)timeoutSeconds)
						{
							await Task.Delay(10);
						}
						else
						{
							flag = false;
							return flag;
						}
					}
					flag = true;
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		public static event Action OnDefinitionsUpdated;

		public static event Action<InventoryResult> OnInventoryUpdated;
	}
}