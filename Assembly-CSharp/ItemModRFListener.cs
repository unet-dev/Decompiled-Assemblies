using Network;
using ProtoBuf;
using System;
using UnityEngine;

public class ItemModRFListener : ItemMod
{
	public GameObjectRef frequencyPanelPrefab;

	public GameObjectRef entityPrefab;

	public ItemModRFListener()
	{
	}

	public BaseEntity GetEntityForParenting(Item item = null)
	{
		BaseEntity baseEntity;
		if (item == null)
		{
			return null;
		}
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		if (ownerPlayer)
		{
			return ownerPlayer;
		}
		if (item.parent == null)
		{
			baseEntity = null;
		}
		else
		{
			baseEntity = item.parent.entityOwner;
		}
		BaseEntity baseEntity1 = baseEntity;
		if (baseEntity1 != null)
		{
			return baseEntity1;
		}
		BaseEntity worldEntity = item.GetWorldEntity();
		if (worldEntity)
		{
			return worldEntity;
		}
		return null;
	}

	public float GetMaxRange()
	{
		return Single.PositiveInfinity;
	}

	public PagerEntity GetPagerEnt(Item item, bool isServer = true)
	{
		BaseNetworkable baseNetworkable = null;
		if (item.instanceData == null)
		{
			return null;
		}
		if (isServer)
		{
			baseNetworkable = BaseNetworkable.serverEntities.Find(item.instanceData.subEntity);
		}
		if (!baseNetworkable)
		{
			return null;
		}
		return baseNetworkable.GetComponent<PagerEntity>();
	}

	public override void OnChanged(Item item)
	{
		base.OnChanged(item);
	}

	public override void OnItemCreated(Item item)
	{
		base.OnItemCreated(item);
		if (item.instanceData == null)
		{
			GameManager gameManager = GameManager.server;
			string str = this.entityPrefab.resourcePath;
			Vector3 vector3 = Vector3.zero;
			Quaternion quaternion = new Quaternion();
			BaseEntity baseEntity = gameManager.CreateEntity(str, vector3, quaternion, true);
			baseEntity.Spawn();
			item.instanceData = new ProtoBuf.Item.InstanceData()
			{
				ShouldPool = false,
				subEntity = baseEntity.net.ID
			};
			item.MarkDirty();
		}
	}

	public override void OnMovedToWorld(Item item)
	{
		this.UpdateParent(item);
		base.OnMovedToWorld(item);
	}

	public override void OnParentChanged(Item item)
	{
		base.OnParentChanged(item);
		this.UpdateParent(item);
	}

	public override void OnRemove(Item item)
	{
		base.OnRemove(item);
		PagerEntity pagerEnt = this.GetPagerEnt(item, true);
		if (pagerEnt)
		{
			pagerEnt.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	public override void OnRemovedFromWorld(Item item)
	{
		this.UpdateParent(item);
		base.OnRemovedFromWorld(item);
	}

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		base.ServerCommand(item, command, player);
		PagerEntity pagerEnt = this.GetPagerEnt(item, true);
		if (command == "stop")
		{
			pagerEnt.SetOff();
			return;
		}
		if (command == "silenton")
		{
			pagerEnt.SetSilentMode(true);
			return;
		}
		if (command == "silentoff")
		{
			pagerEnt.SetSilentMode(false);
		}
	}

	public void UpdateParent(Item item)
	{
		BaseEntity entityForParenting = this.GetEntityForParenting(item);
		if (entityForParenting == null)
		{
			return;
		}
		if (!entityForParenting.isServer)
		{
			return;
		}
		if (!entityForParenting.IsFullySpawned())
		{
			return;
		}
		PagerEntity pagerEnt = this.GetPagerEnt(item, true);
		if (pagerEnt)
		{
			pagerEnt.SetParent(entityForParenting, false, true);
		}
	}
}