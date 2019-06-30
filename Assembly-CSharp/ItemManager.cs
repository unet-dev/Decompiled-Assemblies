using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

public class ItemManager
{
	public static List<ItemDefinition> itemList;

	public static Dictionary<int, ItemDefinition> itemDictionary;

	public static Dictionary<string, ItemDefinition> itemDictionaryByName;

	public static List<ItemBlueprint> bpList;

	public static int[] defaultBlueprints;

	private static List<ItemManager.ItemRemove> ItemRemoves;

	static ItemManager()
	{
		ItemManager.ItemRemoves = new List<ItemManager.ItemRemove>();
	}

	public ItemManager()
	{
	}

	public static Item Create(ItemDefinition template, int iAmount = 1, ulong skin = 0L)
	{
		if (template == null)
		{
			UnityEngine.Debug.LogWarning("Creating invalid/missing item!");
			return null;
		}
		Item item = new Item()
		{
			isServer = true
		};
		if (iAmount <= 0)
		{
			UnityEngine.Debug.LogError(string.Concat("Creating item with less than 1 amount! (", template.displayName.english, ")"));
			return null;
		}
		item.info = template;
		item.amount = iAmount;
		item.skin = skin;
		item.Initialize(template);
		return item;
	}

	public static Item CreateByItemID(int itemID, int iAmount = 1, ulong skin = 0L)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemID);
		if (itemDefinition == null)
		{
			return null;
		}
		return ItemManager.Create(itemDefinition, iAmount, skin);
	}

	public static Item CreateByName(string strName, int iAmount = 1, ulong skin = 0L)
	{
		ItemDefinition itemDefinition = ItemManager.itemList.Find((ItemDefinition x) => x.shortname == strName);
		if (itemDefinition == null)
		{
			return null;
		}
		return ItemManager.CreateByItemID(itemDefinition.itemid, iAmount, skin);
	}

	public static Item CreateByPartialName(string strName, int iAmount = 1)
	{
		ItemDefinition itemDefinition = ItemManager.itemList.Find((ItemDefinition x) => x.shortname == strName);
		if (itemDefinition == null)
		{
			itemDefinition = ItemManager.itemList.Find((ItemDefinition x) => x.shortname.Contains(strName, CompareOptions.IgnoreCase));
		}
		if (itemDefinition == null)
		{
			return null;
		}
		return ItemManager.CreateByItemID(itemDefinition.itemid, iAmount, (ulong)0);
	}

	public static void DoRemoves()
	{
		using (TimeWarning timeWarning = TimeWarning.New("DoRemoves", 0.1f))
		{
			for (int i = 0; i < ItemManager.ItemRemoves.Count; i++)
			{
				if (ItemManager.ItemRemoves[i].time <= Time.time)
				{
					Item item = ItemManager.ItemRemoves[i].item;
					int num = i;
					i = num - 1;
					ItemManager.ItemRemoves.RemoveAt(num);
					item.DoRemove();
				}
			}
		}
	}

	public static ItemBlueprint FindBlueprint(ItemDefinition item)
	{
		return item.GetComponent<ItemBlueprint>();
	}

	public static ItemDefinition FindItemDefinition(int itemID)
	{
		ItemDefinition itemDefinition;
		ItemManager.Initialize();
		if (ItemManager.itemDictionary.TryGetValue(itemID, out itemDefinition))
		{
			return itemDefinition;
		}
		return null;
	}

	public static ItemDefinition FindItemDefinition(string shortName)
	{
		ItemDefinition itemDefinition;
		ItemManager.Initialize();
		if (ItemManager.itemDictionaryByName.TryGetValue(shortName, out itemDefinition))
		{
			return itemDefinition;
		}
		return null;
	}

	public static List<ItemBlueprint> GetBlueprints()
	{
		ItemManager.Initialize();
		return ItemManager.bpList;
	}

	public static List<ItemDefinition> GetItemDefinitions()
	{
		ItemManager.Initialize();
		return ItemManager.itemList;
	}

	public static void Heartbeat()
	{
		ItemManager.DoRemoves();
	}

	public static void Initialize()
	{
		TimeSpan elapsed;
		double totalMilliseconds;
		if (ItemManager.itemList != null)
		{
			return;
		}
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		IEnumerable<GameObject> gameObjects = FileSystem.Load<ObjectList>("Assets/items.asset", true).objects.Cast<GameObject>();
		if (stopwatch.Elapsed.TotalSeconds > 1)
		{
			elapsed = stopwatch.Elapsed;
			totalMilliseconds = elapsed.TotalMilliseconds / 1000;
			UnityEngine.Debug.Log(string.Concat("Loading Items Took: ", totalMilliseconds.ToString(), " seconds"));
		}
		List<ItemDefinition> list = (
			from x in gameObjects
			select x.GetComponent<ItemDefinition>() into x
			where x != null
			select x).ToList<ItemDefinition>();
		List<ItemBlueprint> itemBlueprints = (
			from x in gameObjects
			select x.GetComponent<ItemBlueprint>()).Where<ItemBlueprint>((ItemBlueprint x) => {
			if (x == null)
			{
				return false;
			}
			return x.userCraftable;
		}).ToList<ItemBlueprint>();
		Dictionary<int, ItemDefinition> nums = new Dictionary<int, ItemDefinition>();
		Dictionary<string, ItemDefinition> strs = new Dictionary<string, ItemDefinition>(StringComparer.OrdinalIgnoreCase);
		foreach (ItemDefinition itemDefinition in list)
		{
			itemDefinition.Initialize(list);
			if (!nums.ContainsKey(itemDefinition.itemid))
			{
				nums.Add(itemDefinition.itemid, itemDefinition);
				strs.Add(itemDefinition.shortname, itemDefinition);
			}
			else
			{
				ItemDefinition item = nums[itemDefinition.itemid];
				UnityEngine.Debug.LogWarning(string.Concat(new object[] { "Item ID duplicate ", itemDefinition.itemid, " (", itemDefinition.name, ") - have you given your items unique shortnames?" }), itemDefinition.gameObject);
				UnityEngine.Debug.LogWarning(string.Concat("Other item is ", item.name), item);
			}
		}
		stopwatch.Stop();
		if (stopwatch.Elapsed.TotalSeconds > 1)
		{
			string[] str = new string[] { "Building Items Took: ", null, null, null, null, null };
			elapsed = stopwatch.Elapsed;
			totalMilliseconds = elapsed.TotalMilliseconds / 1000;
			str[1] = totalMilliseconds.ToString();
			str[2] = " seconds / Items: ";
			int count = list.Count;
			str[3] = count.ToString();
			str[4] = " / Blueprints: ";
			count = itemBlueprints.Count;
			str[5] = count.ToString();
			UnityEngine.Debug.Log(string.Concat(str));
		}
		ItemManager.defaultBlueprints = itemBlueprints.Where<ItemBlueprint>((ItemBlueprint x) => {
			if (x.NeedsSteamItem)
			{
				return false;
			}
			return x.defaultBlueprint;
		}).Select<ItemBlueprint, int>((ItemBlueprint x) => x.targetItem.itemid).ToArray<int>();
		ItemManager.itemList = list;
		ItemManager.bpList = itemBlueprints;
		ItemManager.itemDictionary = nums;
		ItemManager.itemDictionaryByName = strs;
	}

	public static void InvalidateWorkshopSkinCache()
	{
		if (ItemManager.itemList == null)
		{
			return;
		}
		foreach (ItemDefinition itemDefinition in ItemManager.itemList)
		{
			itemDefinition.InvalidateWorkshopSkinCache();
		}
	}

	public static Item Load(ProtoBuf.Item load, Item created, bool isServer)
	{
		if (created == null)
		{
			created = new Item();
		}
		created.isServer = isServer;
		created.Load(load);
		if (created.info != null)
		{
			return created;
		}
		UnityEngine.Debug.LogWarning("Item loading failed - item is invalid");
		return null;
	}

	public static void RemoveItem(Item item, float fTime = 0f)
	{
		UnityEngine.Assertions.Assert.IsTrue(item.isServer, "RemoveItem: Removing a client item!");
		ItemManager.ItemRemove itemRemove = new ItemManager.ItemRemove()
		{
			item = item,
			time = Time.time + fTime
		};
		ItemManager.ItemRemoves.Add(itemRemove);
	}

	private struct ItemRemove
	{
		public Item item;

		public float time;
	}
}