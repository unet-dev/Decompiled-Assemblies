using ConVar;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LiquidVessel : HeldEntity
{
	public LiquidVessel()
	{
	}

	public void AddLiquid(ItemDefinition liquidType, int amount)
	{
		if (amount <= 0)
		{
			return;
		}
		Item item = this.GetItem();
		Item slot = item.contents.GetSlot(0);
		ItemModContainer component = item.info.GetComponent<ItemModContainer>();
		if (slot != null)
		{
			int num = Mathf.Clamp(slot.amount + amount, 0, component.maxStackSize);
			ItemDefinition itemDefinition = WaterResource.Merge(slot.info, liquidType);
			if (itemDefinition == slot.info)
			{
				slot.amount = num;
			}
			else
			{
				slot.Remove(0f);
				slot = ItemManager.Create(itemDefinition, num, (ulong)0);
				slot.MoveToContainer(item.contents, -1, true);
			}
			slot.MarkDirty();
			base.SendNetworkUpdateImmediate(false);
		}
		else
		{
			Item item1 = ItemManager.Create(liquidType, amount, (ulong)0);
			if (item1 != null)
			{
				item1.MoveToContainer(item.contents, -1, true);
				return;
			}
		}
	}

	public int AmountHeld()
	{
		Item slot = this.GetItem().contents.GetSlot(0);
		if (slot == null)
		{
			return 0;
		}
		return slot.amount;
	}

	public bool CanDrink()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return false;
		}
		if (!ownerPlayer.metabolism.CanConsume())
		{
			return false;
		}
		Item item = this.GetItem();
		if (item == null)
		{
			return false;
		}
		if (item.contents == null)
		{
			return false;
		}
		if (item.contents.itemList == null)
		{
			return false;
		}
		if (item.contents.itemList.Count == 0)
		{
			return false;
		}
		return true;
	}

	public bool CanFillHere(Vector3 pos)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return false;
		}
		if ((double)ownerPlayer.WaterFactor() > 0.05)
		{
			return true;
		}
		return false;
	}

	[IsActiveItem]
	[RPC_Server]
	private void DoEmpty(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		Item item = this.GetItem();
		if (item == null)
		{
			return;
		}
		if (item.contents == null)
		{
			return;
		}
		if (!msg.player.metabolism.CanConsume())
		{
			return;
		}
		List<Item>.Enumerator enumerator = item.contents.itemList.GetEnumerator();
		try
		{
			if (enumerator.MoveNext())
			{
				enumerator.Current.UseItem(50);
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	public float HeldFraction()
	{
		return (float)this.AmountHeld() / (float)this.MaxHoldable();
	}

	public bool IsFull()
	{
		return this.HeldFraction() >= 1f;
	}

	public int MaxHoldable()
	{
		return this.GetItem().info.GetComponent<ItemModContainer>().maxStackSize;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("LiquidVessel.OnRpcMessage", 0.1f))
		{
			if (rpc != -260241759 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - DoEmpty "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("DoEmpty", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("DoEmpty", this, player))
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
							this.DoEmpty(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in DoEmpty");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}
}