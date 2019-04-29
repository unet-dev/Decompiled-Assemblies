using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class WorldItem : BaseEntity
{
	[Header("WorldItem")]
	public bool allowPickup = true;

	[NonSerialized]
	public Item item;

	protected float eatSeconds = 10f;

	protected float caloriesPerSecond = 1f;

	private bool _isInvokingSendItemUpdate;

	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			if (this.item == null)
			{
				return base.Traits;
			}
			return this.item.Traits;
		}
	}

	public WorldItem()
	{
	}

	public void DestroyItem()
	{
		if (this.item == null)
		{
			return;
		}
		WorldItem worldItem = this;
		this.item.OnDirty -= new Action<Item>(worldItem.OnItemDirty);
		this.item.Remove(0f);
		this.item = null;
	}

	private void DoItemNetworking()
	{
		if (this._isInvokingSendItemUpdate)
		{
			return;
		}
		this._isInvokingSendItemUpdate = true;
		base.Invoke(new Action(this.SendItemUpdate), 0.1f);
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.RemoveItem();
	}

	public override void Eat(BaseNpc baseNpc, float timeSpent)
	{
		if (this.eatSeconds <= 0f)
		{
			return;
		}
		this.eatSeconds -= timeSpent;
		baseNpc.AddCalories(this.caloriesPerSecond * timeSpent);
		if (this.eatSeconds < 0f)
		{
			this.DestroyItem();
			base.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	public override Item GetItem()
	{
		return this.item;
	}

	public void InitializeItem(Item in_item)
	{
		if (this.item != null)
		{
			this.RemoveItem();
		}
		this.item = in_item;
		if (this.item == null)
		{
			return;
		}
		WorldItem worldItem = this;
		this.item.OnDirty += new Action<Item>(worldItem.OnItemDirty);
		base.name = string.Concat(this.item.info.shortname, " (world)");
		this.item.SetWorldEntity(this);
		this.OnItemDirty(this.item);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.worldItem == null)
		{
			return;
		}
		if (info.msg.worldItem.item == null)
		{
			return;
		}
		Item item = ItemManager.Load(info.msg.worldItem.item, this.item, base.isServer);
		if (item != null)
		{
			this.InitializeItem(item);
		}
	}

	public override void OnInvalidPosition()
	{
		this.DestroyItem();
		base.OnInvalidPosition();
	}

	protected virtual void OnItemDirty(Item in_item)
	{
		Assert.IsTrue(this.item == in_item, "WorldItem:OnItemDirty - dirty item isn't ours!");
		if (this.item != null)
		{
			base.BroadcastMessage("OnItemChanged", this.item, SendMessageOptions.DontRequireReceiver);
		}
		this.DoItemNetworking();
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("WorldItem.OnRpcMessage", 0.1f))
		{
			if (rpc != -1516891826 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - Pickup "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("Pickup", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("Pickup", this, player, 3f))
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
							this.Pickup(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in Pickup");
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
	public void Pickup(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (this.item == null)
		{
			return;
		}
		if (!this.allowPickup)
		{
			return;
		}
		if (Interface.CallHook("OnItemPickup", this.item, msg.player) != null)
		{
			return;
		}
		base.ClientRPC(null, "PickupSound");
		msg.player.GiveItem(this.item, BaseEntity.GiveItemReason.PickedUp);
		msg.player.SignalBroadcast(BaseEntity.Signal.Gesture, "pickup_item", null);
	}

	public void RemoveItem()
	{
		if (this.item == null)
		{
			return;
		}
		WorldItem worldItem = this;
		this.item.OnDirty -= new Action<Item>(worldItem.OnItemDirty);
		this.item = null;
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (this.item == null)
		{
			return;
		}
		bool flag = info.forDisk;
		info.msg.worldItem = Facepunch.Pool.Get<ProtoBuf.WorldItem>();
		info.msg.worldItem.item = this.item.Save(flag, false);
	}

	private void SendItemUpdate()
	{
		this._isInvokingSendItemUpdate = false;
		if (this.item == null)
		{
			return;
		}
		using (UpdateItem updateItem = Facepunch.Pool.Get<UpdateItem>())
		{
			updateItem.item = this.item.Save(false, false);
			base.ClientRPC<UpdateItem>(null, "UpdateItem", updateItem);
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (this.item != null)
		{
			base.BroadcastMessage("OnItemChanged", this.item, SendMessageOptions.DontRequireReceiver);
		}
	}

	public override void SwitchParent(BaseEntity ent)
	{
		base.SetParent(ent, this.parentBone, false, false);
	}

	public override string ToString()
	{
		uint d;
		if (this._name == null)
		{
			if (!base.isServer)
			{
				this._name = base.ShortPrefabName;
			}
			else
			{
				if (this.net != null)
				{
					d = this.net.ID;
				}
				else
				{
					d = 0;
				}
				this._name = string.Format("{1}[{0}] {2}", d, base.ShortPrefabName, base.name);
			}
		}
		return this._name;
	}
}