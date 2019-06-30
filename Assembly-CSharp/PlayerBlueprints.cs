using Oxide.Core;
using ProtoBuf;
using Steamworks;
using System;
using System.Collections.Generic;

public class PlayerBlueprints : EntityComponent<BasePlayer>
{
	public SteamInventory steamInventory;

	public PlayerBlueprints()
	{
	}

	public bool CanCraft(int itemid, int skinItemId)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemid);
		if (itemDefinition == null)
		{
			return false;
		}
		object obj = Interface.CallHook("CanCraft", this, itemDefinition, skinItemId);
		if (obj as bool)
		{
			return (bool)obj;
		}
		if (skinItemId != 0 && !this.steamInventory.HasItem(skinItemId))
		{
			return false;
		}
		if (base.baseEntity.currentCraftLevel < (float)itemDefinition.Blueprint.workbenchLevelRequired)
		{
			return false;
		}
		if (this.HasUnlocked(itemDefinition))
		{
			return true;
		}
		return false;
	}

	public bool HasUnlocked(ItemDefinition targetItem)
	{
		int i;
		if (!targetItem.Blueprint || !targetItem.Blueprint.NeedsSteamItem)
		{
			int[] numArray = ItemManager.defaultBlueprints;
			for (i = 0; i < (int)numArray.Length; i++)
			{
				if (numArray[i] == targetItem.itemid)
				{
					return true;
				}
			}
			if (!base.baseEntity.isServer)
			{
				return false;
			}
			return this.IsUnlocked(targetItem);
		}
		if (targetItem.steamItem != null && !this.steamInventory.HasItem(targetItem.steamItem.id))
		{
			return false;
		}
		if (targetItem.steamItem == null)
		{
			bool flag = false;
			ItemSkinDirectory.Skin[] skinArray = targetItem.skins;
			i = 0;
			while (i < (int)skinArray.Length)
			{
				ItemSkinDirectory.Skin skin = skinArray[i];
				if (!this.steamInventory.HasItem(skin.id))
				{
					i++;
				}
				else
				{
					flag = true;
					break;
				}
			}
			if (!flag && targetItem.skins2 != null)
			{
				InventoryDef[] inventoryDefArray = targetItem.skins2;
				i = 0;
				while (i < (int)inventoryDefArray.Length)
				{
					InventoryDef inventoryDef = inventoryDefArray[i];
					if (!this.steamInventory.HasItem(inventoryDef.Id))
					{
						i++;
					}
					else
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsUnlocked(ItemDefinition itemDef)
	{
		PersistantPlayer playerInfo = SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerInfo(base.baseEntity.userID);
		if (playerInfo.unlockedItems == null)
		{
			return false;
		}
		return playerInfo.unlockedItems.Contains(itemDef.itemid);
	}

	public void Reset()
	{
		PersistantPlayer playerInfo = SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerInfo(base.baseEntity.userID);
		playerInfo.unlockedItems = new List<int>();
		SingletonComponent<ServerMgr>.Instance.persistance.SetPlayerInfo(base.baseEntity.userID, playerInfo);
		base.baseEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public void Unlock(ItemDefinition itemDef)
	{
		PersistantPlayer playerInfo = SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerInfo(base.baseEntity.userID);
		if (!playerInfo.unlockedItems.Contains(itemDef.itemid))
		{
			playerInfo.unlockedItems.Add(itemDef.itemid);
			SingletonComponent<ServerMgr>.Instance.persistance.SetPlayerInfo(base.baseEntity.userID, playerInfo);
			base.baseEntity.SendNetworkUpdateImmediate(false);
			base.baseEntity.ClientRPCPlayer<int>(null, base.baseEntity, "UnlockedBlueprint", itemDef.itemid);
			base.baseEntity.stats.Add("blueprint_studied", 1, Stats.Steam);
		}
	}

	public void UnlockAll()
	{
		foreach (ItemBlueprint itemBlueprint in ItemManager.bpList)
		{
			if (!itemBlueprint.userCraftable || itemBlueprint.defaultBlueprint)
			{
				continue;
			}
			PersistantPlayer playerInfo = SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerInfo(base.baseEntity.userID);
			if (playerInfo.unlockedItems.Contains(itemBlueprint.targetItem.itemid))
			{
				continue;
			}
			playerInfo.unlockedItems.Add(itemBlueprint.targetItem.itemid);
			SingletonComponent<ServerMgr>.Instance.persistance.SetPlayerInfo(base.baseEntity.userID, playerInfo);
		}
		base.baseEntity.SendNetworkUpdateImmediate(false);
		base.baseEntity.ClientRPCPlayer<int>(null, base.baseEntity, "UnlockedBlueprint", 0);
	}
}