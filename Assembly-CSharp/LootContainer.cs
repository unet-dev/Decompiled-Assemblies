using Oxide.Core;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LootContainer : StorageContainer
{
	public bool destroyOnEmpty = true;

	public LootSpawn lootDefinition;

	public int maxDefinitionsToSpawn;

	public float minSecondsBetweenRefresh = 3600f;

	public float maxSecondsBetweenRefresh = 7200f;

	public bool initialLootSpawn = true;

	public float xpLootedScale = 1f;

	public float xpDestroyedScale = 1f;

	public bool BlockPlayerItemInput;

	public int scrapAmount;

	public string deathStat = "";

	public LootContainer.spawnType SpawnType;

	private static ItemDefinition scrapDef;

	public LootContainer.LootSpawnSlot[] LootSpawnSlots;

	public bool shouldRefreshContents
	{
		get
		{
			if (this.minSecondsBetweenRefresh <= 0f)
			{
				return false;
			}
			return this.maxSecondsBetweenRefresh > 0f;
		}
	}

	static LootContainer()
	{
	}

	public LootContainer()
	{
	}

	public void GenerateScrap()
	{
		if (this.scrapAmount <= 0)
		{
			return;
		}
		if (LootContainer.scrapDef == null)
		{
			LootContainer.scrapDef = ItemManager.FindItemDefinition("scrap");
		}
		int num = this.scrapAmount;
		if (num > 0)
		{
			Item item = ItemManager.Create(LootContainer.scrapDef, num, (ulong)0);
			if (!item.MoveToContainer(this.inventory, -1, true))
			{
				item.Drop(base.transform.position, this.GetInheritedDropVelocity(), new Quaternion());
			}
		}
	}

	public override void InitShared()
	{
		base.InitShared();
	}

	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		if (this.destroyOnEmpty && (this.inventory == null || this.inventory.itemList == null || this.inventory.itemList.Count == 0))
		{
			base.Kill(BaseNetworkable.DestroyMode.Gib);
		}
	}

	public virtual void PopulateLoot()
	{
		if (this.LootSpawnSlots.Length != 0)
		{
			LootContainer.LootSpawnSlot[] lootSpawnSlots = this.LootSpawnSlots;
			for (int i = 0; i < (int)lootSpawnSlots.Length; i++)
			{
				LootContainer.LootSpawnSlot lootSpawnSlot = lootSpawnSlots[i];
				for (int j = 0; j < lootSpawnSlot.numberToSpawn; j++)
				{
					if (UnityEngine.Random.Range(0f, 1f) <= lootSpawnSlot.probability)
					{
						lootSpawnSlot.definition.SpawnIntoContainer(this.inventory);
					}
				}
			}
		}
		else if (this.lootDefinition != null)
		{
			for (int k = 0; k < this.maxDefinitionsToSpawn; k++)
			{
				this.lootDefinition.SpawnIntoContainer(this.inventory);
			}
		}
		if (this.SpawnType == LootContainer.spawnType.ROADSIDE || this.SpawnType == LootContainer.spawnType.TOWN)
		{
			foreach (Item item in this.inventory.itemList)
			{
				if (!item.hasCondition)
				{
					continue;
				}
				item.condition = UnityEngine.Random.Range(item.info.condition.foundCondition.fractionMin, item.info.condition.foundCondition.fractionMax) * item.info.condition.max;
			}
		}
		this.GenerateScrap();
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.BlockPlayerItemInput && this.inventory != null)
		{
			this.inventory.SetFlag(ItemContainer.Flag.NoItemInput, true);
		}
	}

	public void RemoveMe()
	{
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	public int ScoreForRarity(Rarity rarity)
	{
		switch (rarity)
		{
			case Rarity.Common:
			{
				return 1;
			}
			case Rarity.Uncommon:
			{
				return 2;
			}
			case Rarity.Rare:
			{
				return 3;
			}
			case Rarity.VeryRare:
			{
				return 4;
			}
		}
		return 5000;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (this.initialLootSpawn)
		{
			this.SpawnLoot();
		}
		if (this.BlockPlayerItemInput && !Rust.Application.isLoadingSave && this.inventory != null)
		{
			this.inventory.SetFlag(ItemContainer.Flag.NoItemInput, true);
		}
		base.SetFlag(BaseEntity.Flags.Reserved6, PlayerInventory.IsBirthday(), false, true);
	}

	public override bool ShouldDropItemsIndividually()
	{
		return true;
	}

	public virtual void SpawnLoot()
	{
		if (this.inventory == null)
		{
			Debug.Log("CONTACT DEVELOPERS! LootContainer::PopulateLoot has null inventory!!!");
			return;
		}
		this.inventory.Clear();
		ItemManager.DoRemoves();
		if (Interface.CallHook("OnLootSpawn", this) != null)
		{
			return;
		}
		this.PopulateLoot();
		if (this.shouldRefreshContents)
		{
			LootContainer lootContainer = this;
			base.Invoke(new Action(lootContainer.SpawnLoot), UnityEngine.Random.Range(this.minSecondsBetweenRefresh, this.maxSecondsBetweenRefresh));
		}
	}

	[Serializable]
	public struct LootSpawnSlot
	{
		public LootSpawn definition;

		public int numberToSpawn;

		public float probability;
	}

	public enum spawnType
	{
		GENERIC,
		PLAYER,
		TOWN,
		AIRDROP,
		CRASHSITE,
		ROADSIDE
	}
}