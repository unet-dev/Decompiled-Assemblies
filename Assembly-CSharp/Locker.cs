using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Locker : StorageContainer
{
	public GameObjectRef equipSound;

	private int rowSize = 7;

	private int beltSize = 6;

	private int columnSize = 2;

	private Item[] clothingBuffer = new Item[7];

	public bool equippingActive;

	public Locker()
	{
	}

	public void ClearEquipping()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
	}

	public bool IsEquipping()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	public bool LockerItemFilter(Item item, int targetSlot)
	{
		return this.equippingActive;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("Locker.OnRpcMessage", 0.1f))
		{
			if (rpc != 1799659668 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_Equip "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_Equip", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_Equip", this, player, 3f))
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
							this.RPC_Equip(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_Equip");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RPC_Equip(BaseEntity.RPCMessage msg)
	{
		Quaternion quaternion;
		int num = msg.read.Int32();
		if (num < 0 || num > 2)
		{
			return;
		}
		if (this.IsEquipping())
		{
			return;
		}
		BasePlayer basePlayer = msg.player;
		int num1 = num * this.rowSize * this.columnSize;
		this.equippingActive = true;
		bool flag = false;
		for (int i = 0; i < basePlayer.inventory.containerWear.capacity; i++)
		{
			Item slot = basePlayer.inventory.containerWear.GetSlot(i);
			if (slot != null)
			{
				slot.RemoveFromContainer();
				this.clothingBuffer[i] = slot;
			}
		}
		for (int j = 0; j < this.rowSize; j++)
		{
			int num2 = num1 + j;
			int num3 = j;
			Item item = this.inventory.GetSlot(num2);
			Item item1 = this.clothingBuffer[j];
			if (item != null)
			{
				flag = true;
				if (item.info.category != ItemCategory.Attire || !item.MoveToContainer(basePlayer.inventory.containerWear, num3, true))
				{
					Vector3 dropPosition = this.GetDropPosition();
					Vector3 dropVelocity = this.GetDropVelocity();
					quaternion = new Quaternion();
					item.Drop(dropPosition, dropVelocity, quaternion);
				}
			}
			if (item1 != null)
			{
				flag = true;
				if (item1.info.category != ItemCategory.Attire || !item1.MoveToContainer(this.inventory, num2, true))
				{
					Vector3 vector3 = this.GetDropPosition();
					Vector3 dropVelocity1 = this.GetDropVelocity();
					quaternion = new Quaternion();
					item1.Drop(vector3, dropVelocity1, quaternion);
				}
			}
			this.clothingBuffer[j] = null;
		}
		for (int k = 0; k < this.beltSize; k++)
		{
			int num4 = num1 + k + this.rowSize;
			int num5 = k;
			Item slot1 = this.inventory.GetSlot(num4);
			Item slot2 = basePlayer.inventory.containerBelt.GetSlot(k);
			if (slot2 != null)
			{
				slot2.RemoveFromContainer();
			}
			if (slot1 != null)
			{
				flag = true;
				if (!slot1.MoveToContainer(basePlayer.inventory.containerBelt, num5, true))
				{
					Vector3 dropPosition1 = this.GetDropPosition();
					Vector3 vector31 = this.GetDropVelocity();
					quaternion = new Quaternion();
					slot1.Drop(dropPosition1, vector31, quaternion);
				}
			}
			if (slot2 != null)
			{
				flag = true;
				if (!slot2.MoveToContainer(this.inventory, num4, true))
				{
					Vector3 dropPosition2 = this.GetDropPosition();
					Vector3 dropVelocity2 = this.GetDropVelocity();
					quaternion = new Quaternion();
					slot2.Drop(dropPosition2, dropVelocity2, quaternion);
				}
			}
		}
		this.equippingActive = false;
		if (flag)
		{
			Effect.server.Run(this.equipSound.resourcePath, basePlayer, StringPool.Get("spine3"), Vector3.zero, Vector3.zero, null, false);
			base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
			base.Invoke(new Action(this.ClearEquipping), 1.5f);
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this.inventory.canAcceptItem += new Func<Item, int, bool>(this.LockerItemFilter);
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
	}

	public static class LockerFlags
	{
		public const BaseEntity.Flags IsEquipping = BaseEntity.Flags.Reserved1;
	}
}