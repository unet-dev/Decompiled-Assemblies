using System;
using UnityEngine;

public class ItemModEntity : ItemMod
{
	public GameObjectRef entityPrefab = new GameObjectRef();

	public string defaultBone;

	public ItemModEntity()
	{
	}

	public override void CollectedForCrafting(Item item, BasePlayer crafter)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		HeldEntity heldEntity1 = heldEntity as HeldEntity;
		if (heldEntity1 == null)
		{
			return;
		}
		heldEntity1.CollectedForCrafting(item, crafter);
	}

	public override void OnItemCreated(Item item)
	{
		if (item.GetHeldEntity() == null)
		{
			GameManager gameManager = GameManager.server;
			string str = this.entityPrefab.resourcePath;
			Vector3 vector3 = new Vector3();
			Quaternion quaternion = new Quaternion();
			BaseEntity baseEntity = gameManager.CreateEntity(str, vector3, quaternion, true);
			if (baseEntity == null)
			{
				Debug.LogWarning(string.Concat(new string[] { "Couldn't create item entity ", item.info.displayName.english, " (", this.entityPrefab.resourcePath, ")" }));
				return;
			}
			baseEntity.skinID = item.skin;
			baseEntity.Spawn();
			item.SetHeldEntity(baseEntity);
		}
	}

	public override void OnParentChanged(Item item)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		if (this.ParentToParent(item, heldEntity))
		{
			return;
		}
		if (this.ParentToPlayer(item, heldEntity))
		{
			return;
		}
		heldEntity.SetParent(null, false, false);
		heldEntity.limitNetworking = true;
		heldEntity.SetFlag(BaseEntity.Flags.Disabled, true, false, true);
	}

	public override void OnRemove(Item item)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		heldEntity.Kill(BaseNetworkable.DestroyMode.None);
		item.SetHeldEntity(null);
	}

	private bool ParentToParent(Item item, BaseEntity ourEntity)
	{
		if (item.parentItem == null)
		{
			return false;
		}
		BaseEntity worldEntity = item.parentItem.GetWorldEntity();
		if (worldEntity == null)
		{
			worldEntity = item.parentItem.GetHeldEntity();
		}
		ourEntity.SetFlag(BaseEntity.Flags.Disabled, false, false, true);
		ourEntity.limitNetworking = false;
		ourEntity.SetParent(worldEntity, this.defaultBone, false, false);
		return true;
	}

	private bool ParentToPlayer(Item item, BaseEntity ourEntity)
	{
		HeldEntity heldEntity = ourEntity as HeldEntity;
		if (heldEntity == null)
		{
			return false;
		}
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			heldEntity.ClearOwnerPlayer();
			return true;
		}
		heldEntity.SetOwnerPlayer(ownerPlayer);
		return true;
	}

	public override void ReturnedFromCancelledCraft(Item item, BasePlayer crafter)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		HeldEntity heldEntity1 = heldEntity as HeldEntity;
		if (heldEntity1 == null)
		{
			return;
		}
		heldEntity1.ReturnedFromCancelledCraft(item, crafter);
	}
}