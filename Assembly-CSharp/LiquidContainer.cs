using ConVar;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LiquidContainer : StorageContainer, ISplashable
{
	public ItemDefinition defaultLiquid;

	public int startingAmount;

	public LiquidContainer()
	{
	}

	public int DoSplash(ItemDefinition splashType, int amount)
	{
		int num = 0;
		if (!this.HasLiquidItem())
		{
			num = Mathf.Min(amount, this.maxStackSize);
			Item item = ItemManager.Create(splashType, num, (ulong)0);
			if (item != null && !item.MoveToContainer(this.inventory, -1, true))
			{
				item.Remove(0f);
			}
		}
		else
		{
			Item liquidItem = this.GetLiquidItem();
			int num1 = liquidItem.amount;
			ItemDefinition itemDefinition = WaterResource.Merge(splashType, liquidItem.info);
			if (liquidItem.info != itemDefinition)
			{
				liquidItem.Remove(0f);
				liquidItem = ItemManager.Create(itemDefinition, num1, (ulong)0);
				if (!liquidItem.MoveToContainer(this.inventory, -1, true))
				{
					liquidItem.Remove(0f);
					return 0;
				}
			}
			num = Mathf.Min(this.maxStackSize - num1, amount);
			liquidItem.amount += num;
		}
		return num;
	}

	public Item GetLiquidItem()
	{
		if (this.inventory.itemList.Count == 0)
		{
			return null;
		}
		return this.inventory.itemList[0];
	}

	public bool HasLiquidItem()
	{
		return this.GetLiquidItem() != null;
	}

	protected override void OnInventoryDirty()
	{
		this.UpdateOnFlag();
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("LiquidContainer.OnRpcMessage", 0.1f))
		{
			if (rpc != 2002733690 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SVDrink "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("SVDrink", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("SVDrink", this, player, 3f))
						{
							flag = true;
							return flag;
						}
					}
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SVDrink(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in SVDrink");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public virtual void OpenTap(float duration)
	{
		if (base.HasFlag(BaseEntity.Flags.Reserved5))
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved5, true, false, true);
		LiquidContainer liquidContainer = this;
		base.Invoke(new Action(liquidContainer.ShutTap), duration);
		base.SendNetworkUpdateImmediate(false);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (this.startingAmount > 0)
		{
			this.inventory.AddItem(this.defaultLiquid, this.startingAmount);
		}
	}

	public virtual void ShutTap()
	{
		base.SetFlag(BaseEntity.Flags.Reserved5, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	[MaxDistance(3f)]
	[RPC_Server]
	public void SVDrink(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.metabolism.CanConsume())
		{
			return;
		}
		foreach (Item item in this.inventory.itemList)
		{
			ItemModConsume component = item.info.GetComponent<ItemModConsume>();
			if (component == null || !component.CanDoAction(item, rpc.player))
			{
				continue;
			}
			component.DoAction(item, rpc.player);
			return;
		}
	}

	protected void UpdateOnFlag()
	{
		base.SetFlag(BaseEntity.Flags.On, (this.inventory.itemList.Count <= 0 ? false : this.inventory.itemList[0].amount > 0), false, true);
	}

	public bool wantsSplash(ItemDefinition splashType, int amount)
	{
		if (!this.HasLiquidItem())
		{
			return true;
		}
		if (this.GetLiquidItem().info != splashType)
		{
			return false;
		}
		return this.GetLiquidItem().amount < this.maxStackSize;
	}
}