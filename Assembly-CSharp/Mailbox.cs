using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Mailbox : StorageContainer
{
	public string ownerPanel;

	public GameObjectRef mailDropSound;

	public bool autoSubmitWhenClosed;

	public bool shouldMarkAsFull;

	public int mailInputSlot
	{
		get
		{
			return this.inventorySlots - 1;
		}
	}

	public Mailbox()
	{
	}

	public override bool CanMoveFrom(BasePlayer player, Item item)
	{
		bool flag = this.PlayerIsOwner(player);
		if (!flag)
		{
			flag = item == this.inventory.GetSlot(this.mailInputSlot);
		}
		if (!flag)
		{
			return false;
		}
		return base.CanMoveFrom(player, item);
	}

	public override bool CanOpenLootPanel(BasePlayer player, string panelName = "")
	{
		if (panelName != this.ownerPanel)
		{
			if (this.HasFreeSpace())
			{
				return true;
			}
			return !this.shouldMarkAsFull;
		}
		if (!this.PlayerIsOwner(player))
		{
			return false;
		}
		return base.CanOpenLootPanel(player, panelName);
	}

	private int GetFreeSlot()
	{
		for (int i = 0; i < this.mailInputSlot; i++)
		{
			if (this.inventory.GetSlot(i) == null)
			{
				return i;
			}
		}
		return -1;
	}

	private bool HasFreeSpace()
	{
		return this.GetFreeSlot() != -1;
	}

	public bool IsFull()
	{
		if (!this.shouldMarkAsFull)
		{
			return false;
		}
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	public void MarkFull(bool full)
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, this.shouldMarkAsFull & full, false, true);
	}

	public virtual bool MoveItemToStorage(Item item)
	{
		item.RemoveFromContainer();
		if (!item.MoveToContainer(this.inventory, -1, true))
		{
			return false;
		}
		return true;
	}

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		this.MarkFull(!this.HasFreeSpace());
		base.OnItemAddedOrRemoved(item, added);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("Mailbox.OnRpcMessage", 0.1f))
		{
			if (rpc != 131727457 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_Submit "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_Submit", 0.1f))
				{
					try
					{
						using (TimeWarning timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Submit(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_Submit");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public virtual bool PlayerIsOwner(BasePlayer player)
	{
		object obj = Interface.CallHook("CanUseMailbox", player, this);
		if (obj as bool)
		{
			return (bool)obj;
		}
		return player.CanBuild();
	}

	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen)
	{
		return base.PlayerOpenLoot(player, (this.PlayerIsOwner(player) ? this.ownerPanel : panelToOpen));
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		if (this.autoSubmitWhenClosed)
		{
			this.SubmitInputItems(player);
		}
		if (this.IsFull())
		{
			Item slot = this.inventory.GetSlot(this.mailInputSlot);
			if (slot != null)
			{
				slot.Drop(this.GetDropPosition(), this.GetDropVelocity(), new Quaternion());
			}
		}
		base.PlayerStoppedLooting(player);
		if (this.PlayerIsOwner(player))
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
		}
	}

	[RPC_Server]
	public void RPC_Submit(BaseEntity.RPCMessage msg)
	{
		if (this.IsFull())
		{
			return;
		}
		this.SubmitInputItems(msg.player);
	}

	public void SubmitInputItems(BasePlayer fromPlayer)
	{
		Item slot = this.inventory.GetSlot(this.mailInputSlot);
		if (this.IsFull())
		{
			return;
		}
		if (slot != null)
		{
			if (!this.MoveItemToStorage(slot))
			{
				slot.Drop(this.GetDropPosition(), this.GetDropVelocity(), new Quaternion());
			}
			else if (slot.position != this.mailInputSlot)
			{
				Effect.server.Run(this.mailDropSound.resourcePath, this.GetDropPosition(), new Vector3(), null, false);
				if (fromPlayer != null && !this.PlayerIsOwner(fromPlayer))
				{
					base.SetFlag(BaseEntity.Flags.On, true, false, true);
					return;
				}
			}
		}
	}
}