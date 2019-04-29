using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Loot Spawn")]
public class LootSpawn : ScriptableObject
{
	public ItemAmountRanged[] items;

	public LootSpawn.Entry[] subSpawn;

	public LootSpawn()
	{
	}

	public ItemDefinition GetBlueprintBaseDef()
	{
		return ItemManager.FindItemDefinition("blueprintbase");
	}

	public void SpawnIntoContainer(ItemContainer container)
	{
		if (this.subSpawn != null && this.subSpawn.Length != 0)
		{
			this.SubCategoryIntoContainer(container);
			return;
		}
		if (this.items != null)
		{
			ItemAmountRanged[] itemAmountRangedArray = this.items;
			for (int i = 0; i < (int)itemAmountRangedArray.Length; i++)
			{
				ItemAmountRanged itemAmountRanged = itemAmountRangedArray[i];
				if (itemAmountRanged != null)
				{
					Item item = null;
					if (!itemAmountRanged.itemDef.spawnAsBlueprint)
					{
						item = ItemManager.CreateByItemID(itemAmountRanged.itemid, (int)itemAmountRanged.GetAmount(), (ulong)0);
					}
					else
					{
						ItemDefinition blueprintBaseDef = this.GetBlueprintBaseDef();
						if (blueprintBaseDef == null)
						{
							goto Label0;
						}
						Item item1 = ItemManager.Create(blueprintBaseDef, 1, (ulong)0);
						item1.blueprintTarget = itemAmountRanged.itemDef.itemid;
						item = item1;
					}
					if (item != null)
					{
						item.OnVirginSpawn();
						if (!item.MoveToContainer(container, -1, true))
						{
							if (!container.playerOwner)
							{
								item.Remove(0f);
							}
							else
							{
								item.Drop(container.playerOwner.GetDropPosition(), container.playerOwner.GetDropVelocity(), new Quaternion());
							}
						}
					}
				}
			Label0:
			}
		}
	}

	private void SubCategoryIntoContainer(ItemContainer container)
	{
		int num = ((IEnumerable<LootSpawn.Entry>)this.subSpawn).Sum<LootSpawn.Entry>((LootSpawn.Entry x) => x.weight);
		int num1 = UnityEngine.Random.Range(0, num);
		for (int i = 0; i < (int)this.subSpawn.Length; i++)
		{
			if (this.subSpawn[i].category != null)
			{
				num -= this.subSpawn[i].weight;
				if (num1 >= num)
				{
					this.subSpawn[i].category.SpawnIntoContainer(container);
					return;
				}
			}
		}
		Debug.LogWarning("SubCategoryIntoContainer: This should never happen!", this);
	}

	[Serializable]
	public struct Entry
	{
		[Tooltip("If a subcategory exists we'll choose from there instead of any items specified")]
		public LootSpawn category;

		[Tooltip("The higher this number, the more likely this will be chosen")]
		public int weight;
	}
}