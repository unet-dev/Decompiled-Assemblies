using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerLoot : EntityComponent<BasePlayer>
{
	public BaseEntity entitySource;

	public Item itemSource;

	public List<ItemContainer> containers = new List<ItemContainer>();

	public bool PositionChecks = true;

	private bool isInvokingSendUpdate;

	public PlayerLoot()
	{
	}

	public void AddContainer(ItemContainer container)
	{
		if (container == null)
		{
			return;
		}
		this.containers.Add(container);
		container.onDirty += new Action(this.MarkDirty);
	}

	public void Check()
	{
		if (!this.IsLooting())
		{
			return;
		}
		if (!base.baseEntity.isServer)
		{
			return;
		}
		if (this.entitySource == null)
		{
			base.baseEntity.ChatMessage("Stopping Looting because lootable doesn't exist!");
			this.Clear();
			return;
		}
		if (!this.entitySource.CanBeLooted(base.baseEntity))
		{
			this.Clear();
			return;
		}
		if (!this.PositionChecks || this.entitySource.Distance(base.baseEntity.eyes.position) <= 2.1f)
		{
			return;
		}
		this.Clear();
	}

	public void Clear()
	{
		if (!this.IsLooting())
		{
			return;
		}
		Interface.CallHook("OnPlayerLootEnd", this);
		this.MarkDirty();
		if (this.entitySource)
		{
			this.entitySource.SendMessage("PlayerStoppedLooting", base.baseEntity, SendMessageOptions.DontRequireReceiver);
		}
		foreach (ItemContainer container in this.containers)
		{
			if (container == null)
			{
				continue;
			}
			container.onDirty -= new Action(this.MarkDirty);
		}
		this.containers.Clear();
		this.entitySource = null;
		this.itemSource = null;
	}

	public ItemContainer FindContainer(uint id)
	{
		ItemContainer itemContainer;
		this.Check();
		if (!this.IsLooting())
		{
			return null;
		}
		List<ItemContainer>.Enumerator enumerator = this.containers.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ItemContainer itemContainer1 = enumerator.Current.FindContainer(id);
				if (itemContainer1 == null)
				{
					continue;
				}
				itemContainer = itemContainer1;
				return itemContainer;
			}
			return null;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return itemContainer;
	}

	public Item FindItem(uint id)
	{
		Item item;
		this.Check();
		if (!this.IsLooting())
		{
			return null;
		}
		List<ItemContainer>.Enumerator enumerator = this.containers.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Item item1 = enumerator.Current.FindItemByUID(id);
				if (item1 == null || !item1.IsValid())
				{
					continue;
				}
				item = item1;
				return item;
			}
			return null;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return item;
	}

	public bool IsLooting()
	{
		return this.containers.Count > 0;
	}

	public void MarkDirty()
	{
		if (!this.isInvokingSendUpdate)
		{
			this.isInvokingSendUpdate = true;
			base.Invoke(new Action(this.SendUpdate), 0.1f);
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("PlayerLoot.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public void SendImmediate()
	{
		if (this.isInvokingSendUpdate)
		{
			this.isInvokingSendUpdate = false;
			base.CancelInvoke(new Action(this.SendUpdate));
		}
		this.SendUpdate();
	}

	private void SendUpdate()
	{
		this.isInvokingSendUpdate = false;
		if (!base.baseEntity.IsValid())
		{
			return;
		}
		using (PlayerUpdateLoot d = Pool.Get<PlayerUpdateLoot>())
		{
			if (this.entitySource && this.entitySource.net != null)
			{
				d.entityID = this.entitySource.net.ID;
			}
			if (this.itemSource != null)
			{
				d.itemID = this.itemSource.uid;
			}
			if (this.containers.Count > 0)
			{
				d.containers = Pool.Get<List<ProtoBuf.ItemContainer>>();
				foreach (ItemContainer container in this.containers)
				{
					d.containers.Add(container.Save());
				}
			}
			base.baseEntity.ClientRPCPlayer<PlayerUpdateLoot>(null, base.baseEntity, "UpdateLoot", d);
		}
	}

	public bool StartLootingEntity(BaseEntity targetEntity, bool doPositionChecks = true)
	{
		this.Clear();
		if (!targetEntity)
		{
			return false;
		}
		if (!targetEntity.OnStartBeingLooted(base.baseEntity))
		{
			return false;
		}
		Assert.IsTrue(targetEntity.isServer, "Assure is server");
		this.PositionChecks = doPositionChecks;
		this.entitySource = targetEntity;
		this.itemSource = null;
		Interface.CallHook("OnLootEntity", this.GetComponent<BasePlayer>(), targetEntity);
		this.MarkDirty();
		return true;
	}

	public void StartLootingItem(Item item)
	{
		this.Clear();
		if (item == null)
		{
			return;
		}
		if (item.contents == null)
		{
			return;
		}
		this.PositionChecks = true;
		this.containers.Add(item.contents);
		item.contents.onDirty += new Action(this.MarkDirty);
		this.itemSource = item;
		this.entitySource = item.GetWorldEntity();
		Interface.CallHook("OnLootItem", this.GetComponent<BasePlayer>(), item);
		this.MarkDirty();
	}
}