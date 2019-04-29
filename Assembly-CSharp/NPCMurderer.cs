using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCMurderer : NPCPlayerApex
{
	public LootContainer.LootSpawnSlot[] LootSpawnSlots;

	public override BaseNpc.AiStatistics.FamilyEnum Family
	{
		get
		{
			return BaseNpc.AiStatistics.FamilyEnum.Murderer;
		}
	}

	public NPCMurderer()
	{
	}

	public override string Categorize()
	{
		return "murderer";
	}

	public override BaseCorpse CreateCorpse()
	{
		int i;
		BaseCorpse baseCorpse;
		using (TimeWarning timeWarning = TimeWarning.New("Create corpse", 0.1f))
		{
			NPCPlayerCorpse nPCPlayerCorpse = base.DropCorpse("assets/prefabs/npc/murderer/murderer_corpse.prefab") as NPCPlayerCorpse;
			if (nPCPlayerCorpse)
			{
				nPCPlayerCorpse.SetLootableIn(2f);
				nPCPlayerCorpse.SetFlag(BaseEntity.Flags.Reserved5, base.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash), false, true);
				nPCPlayerCorpse.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
				int num = 0;
				while (num < this.inventory.containerWear.itemList.Count)
				{
					Item item = this.inventory.containerWear.itemList[num];
					if (item == null || !(item.info.shortname == "gloweyes"))
					{
						num++;
					}
					else
					{
						this.inventory.containerWear.Remove(item);
						break;
					}
				}
				nPCPlayerCorpse.TakeFrom(new ItemContainer[] { this.inventory.containerMain, this.inventory.containerWear, this.inventory.containerBelt });
				nPCPlayerCorpse.playerName = base.displayName;
				nPCPlayerCorpse.playerSteamID = this.userID;
				nPCPlayerCorpse.Spawn();
				nPCPlayerCorpse.TakeChildren(this);
				ItemContainer[] itemContainerArray = nPCPlayerCorpse.containers;
				for (i = 0; i < (int)itemContainerArray.Length; i++)
				{
					itemContainerArray[i].Clear();
				}
				if (this.LootSpawnSlots.Length != 0)
				{
					LootContainer.LootSpawnSlot[] lootSpawnSlots = this.LootSpawnSlots;
					for (i = 0; i < (int)lootSpawnSlots.Length; i++)
					{
						LootContainer.LootSpawnSlot lootSpawnSlot = lootSpawnSlots[i];
						for (int j = 0; j < lootSpawnSlot.numberToSpawn; j++)
						{
							if (UnityEngine.Random.Range(0f, 1f) <= lootSpawnSlot.probability)
							{
								lootSpawnSlot.definition.SpawnIntoContainer(nPCPlayerCorpse.containers[0]);
							}
						}
					}
				}
			}
			baseCorpse = nPCPlayerCorpse;
		}
		return baseCorpse;
	}

	public override float MaxHealth()
	{
		return this._maxHealth;
	}

	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	public override float StartHealth()
	{
		return UnityEngine.Random.Range(100f, 100f);
	}

	public override float StartMaxHealth()
	{
		return 100f;
	}
}