using ConVar;
using Facepunch;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RepairBench : StorageContainer
{
	public float maxConditionLostOnRepair = 0.2f;

	public GameObjectRef skinchangeEffect;

	private float nextSkinChangeTime;

	public RepairBench()
	{
	}

	[RPC_Server]
	public void ChangeSkin(BaseEntity.RPCMessage msg)
	{
		if (UnityEngine.Time.realtimeSinceStartup < this.nextSkinChangeTime)
		{
			return;
		}
		BasePlayer basePlayer = msg.player;
		int num = msg.read.Int32();
		Item slot = this.inventory.GetSlot(0);
		if (slot == null)
		{
			return;
		}
		if (num != 0 && !basePlayer.blueprints.steamInventory.HasItem(num))
		{
			this.debugprint(string.Concat("RepairBench.ChangeSkin player does not have item :", num, ":"));
			return;
		}
		ulong num1 = ItemDefinition.FindSkin(slot.info.itemid, num);
		if (num1 == slot.skin)
		{
			this.debugprint(string.Concat(new object[] { "RepairBench.ChangeSkin cannot apply same skin twice : ", num1, ": ", slot.skin }));
			return;
		}
		this.nextSkinChangeTime = UnityEngine.Time.realtimeSinceStartup + 0.75f;
		slot.skin = num1;
		slot.MarkDirty();
		BaseEntity heldEntity = slot.GetHeldEntity();
		if (heldEntity != null)
		{
			heldEntity.skinID = num1;
			heldEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
		if (this.skinchangeEffect.isValid)
		{
			Effect.server.Run(this.skinchangeEffect.resourcePath, this, 0, new Vector3(0f, 1.5f, 0f), Vector3.zero, null, false);
		}
	}

	public void debugprint(string toPrint)
	{
		if (Global.developer > 0)
		{
			Debug.LogWarning(toPrint);
		}
	}

	public void GetRepairCostList(ItemBlueprint bp, List<ItemAmount> allIngredients)
	{
		foreach (ItemAmount ingredient in bp.ingredients)
		{
			allIngredients.Add(new ItemAmount(ingredient.itemDef, ingredient.amount));
		}
		foreach (ItemAmount itemAmount in bp.ingredients)
		{
			if (itemAmount.itemDef.category != ItemCategory.Component || !(itemAmount.itemDef.Blueprint != null))
			{
				continue;
			}
			bool flag = false;
			ItemAmount item = itemAmount.itemDef.Blueprint.ingredients[0];
			foreach (ItemAmount allIngredient in allIngredients)
			{
				if (allIngredient.itemDef != item.itemDef)
				{
					continue;
				}
				ItemAmount itemAmount1 = allIngredient;
				itemAmount1.amount = itemAmount1.amount + item.amount * itemAmount.amount;
				flag = true;
				goto Label0;
			}
		Label0:
			if (flag)
			{
				continue;
			}
			allIngredients.Add(new ItemAmount(item.itemDef, item.amount * itemAmount.amount));
		}
	}

	public float GetRepairFraction(Item itemToRepair)
	{
		return 1f - itemToRepair.condition / itemToRepair.maxCondition;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		BaseEntity.RPCMessage rPCMessage;
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("RepairBench.OnRpcMessage", 0.1f))
		{
			if (rpc == 1942825351 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - ChangeSkin "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("ChangeSkin", 0.1f))
				{
					try
					{
						using (TimeWarning timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ChangeSkin(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in ChangeSkin");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != 1178348163 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RepairItem "));
				}
				using (timeWarning1 = TimeWarning.New("RepairItem", 0.1f))
				{
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RepairItem(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in RepairItem");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public float RepairCostFraction(Item itemToRepair)
	{
		return this.GetRepairFraction(itemToRepair) * 0.2f;
	}

	[RPC_Server]
	public void RepairItem(BaseEntity.RPCMessage msg)
	{
		bool flag;
		Item slot = this.inventory.GetSlot(0);
		if (slot == null)
		{
			return;
		}
		ItemDefinition itemDefinition = slot.info;
		ItemBlueprint component = itemDefinition.GetComponent<ItemBlueprint>();
		if (!component)
		{
			return;
		}
		if (!itemDefinition.condition.repairable)
		{
			return;
		}
		if (slot.condition == slot.maxCondition)
		{
			return;
		}
		BasePlayer basePlayer = msg.player;
		if (basePlayer.blueprints.HasUnlocked(itemDefinition))
		{
			flag = true;
		}
		else
		{
			flag = (itemDefinition.Blueprint == null ? false : !itemDefinition.Blueprint.isResearchable);
		}
		if (!flag)
		{
			return;
		}
		float single = this.RepairCostFraction(slot);
		bool flag1 = false;
		List<ItemAmount> list = Facepunch.Pool.GetList<ItemAmount>();
		this.GetRepairCostList(component, list);
		foreach (ItemAmount itemAmount in list)
		{
			if (itemAmount.itemDef.category == ItemCategory.Component)
			{
				continue;
			}
			int amount = basePlayer.inventory.GetAmount(itemAmount.itemDef.itemid);
			if (Mathf.CeilToInt(itemAmount.amount * single) <= amount)
			{
				continue;
			}
			flag1 = true;
			goto Label0;
		}
	Label0:
		if (flag1)
		{
			Facepunch.Pool.FreeList<ItemAmount>(ref list);
			return;
		}
		foreach (ItemAmount itemAmount1 in list)
		{
			if (itemAmount1.itemDef.category == ItemCategory.Component)
			{
				continue;
			}
			int num = Mathf.CeilToInt(itemAmount1.amount * single);
			basePlayer.inventory.Take(null, itemAmount1.itemid, num);
		}
		Facepunch.Pool.FreeList<ItemAmount>(ref list);
		slot.DoRepair(this.maxConditionLostOnRepair);
		if (Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[] { "Item repaired! condition : ", slot.condition, "/", slot.maxCondition }));
		}
		Effect.server.Run("assets/bundled/prefabs/fx/repairbench/itemrepair.prefab", this, 0, Vector3.zero, Vector3.zero, null, false);
	}
}