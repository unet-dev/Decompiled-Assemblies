using ConVar;
using Oxide.Core;
using System;
using UnityEngine;

public class DroppedItem : WorldItem
{
	[Header("DroppedItem")]
	public GameObject itemModel;

	public DroppedItem()
	{
	}

	public virtual float GetDespawnDuration()
	{
		if (this.item != null && this.item.info.quickDespawn)
		{
			return 30f;
		}
		return Server.itemdespawn * (float)((this.item != null ? this.item.despawnMultiplier : 1));
	}

	public void IdleDestroy()
	{
		base.DestroyItem();
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		if (this.item == null)
		{
			return;
		}
		DroppedItem droppedItem = hitEntity as DroppedItem;
		if (droppedItem == null)
		{
			return;
		}
		if (droppedItem.item == null)
		{
			return;
		}
		if (droppedItem.item.info != this.item.info)
		{
			return;
		}
		droppedItem.OnDroppedOn(this);
	}

	public void OnDroppedOn(DroppedItem di)
	{
		if (this.item == null)
		{
			return;
		}
		if (di.item == null)
		{
			return;
		}
		if (Interface.CallHook("CanCombineDroppedItem", this, di) != null)
		{
			return;
		}
		if (this.item.info.stackable <= 1)
		{
			return;
		}
		if (di.item.info != this.item.info)
		{
			return;
		}
		if (di.item.IsBlueprint() && di.item.blueprintTarget != this.item.blueprintTarget)
		{
			return;
		}
		int num = di.item.amount + this.item.amount;
		if (num > this.item.info.stackable)
		{
			return;
		}
		if (num == 0)
		{
			return;
		}
		di.DestroyItem();
		di.Kill(BaseNetworkable.DestroyMode.None);
		this.item.amount = num;
		this.item.MarkDirty();
		if (this.GetDespawnDuration() < Single.PositiveInfinity)
		{
			base.Invoke(new Action(this.IdleDestroy), this.GetDespawnDuration());
		}
		Effect.server.Run("assets/bundled/prefabs/fx/notice/stack.world.fx.prefab", this, 0, Vector3.zero, Vector3.zero, null, false);
	}

	internal override void OnParentRemoved()
	{
		RaycastHit raycastHit;
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component == null)
		{
			base.OnParentRemoved();
			return;
		}
		Vector3 vector3 = base.transform.position;
		Quaternion quaternion = base.transform.rotation;
		base.SetParent(null, false, false);
		if (UnityEngine.Physics.Raycast(vector3 + (Vector3.up * 2f), Vector3.down, out raycastHit, 2f, 27328512) && vector3.y < raycastHit.point.y)
		{
			vector3 = vector3 + (Vector3.up * 1.5f);
		}
		base.transform.position = vector3;
		base.transform.rotation = quaternion;
		ConVar.Physics.ApplyDropped(component);
		component.isKinematic = false;
		component.useGravity = true;
		component.WakeUp();
		if (this.GetDespawnDuration() < Single.PositiveInfinity)
		{
			base.Invoke(new Action(this.IdleDestroy), this.GetDespawnDuration());
		}
	}

	public override bool PhysicsDriven()
	{
		return true;
	}

	public override void PostInitShared()
	{
		base.PostInitShared();
		GameObject gameObject = null;
		gameObject = (this.item == null || !this.item.info.worldModelPrefab.isValid ? UnityEngine.Object.Instantiate<GameObject>(this.itemModel) : this.item.info.worldModelPrefab.Instantiate(null));
		gameObject.transform.SetParent(base.transform, false);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.SetLayerRecursive(base.gameObject.layer);
		Collider component = gameObject.GetComponent<Collider>();
		if (component)
		{
			component.enabled = false;
			component.enabled = true;
		}
		if (base.isServer)
		{
			WorldModel worldModel = gameObject.GetComponent<WorldModel>();
			float single = (worldModel ? worldModel.mass : 1f);
			float single1 = 0.1f;
			float single2 = 0.1f;
			Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
			rigidbody.mass = single;
			rigidbody.drag = single1;
			rigidbody.angularDrag = single2;
			rigidbody.interpolation = RigidbodyInterpolation.None;
			ConVar.Physics.ApplyDropped(rigidbody);
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}
		if (this.item != null)
		{
			PhysicsEffects physicsEffect = base.gameObject.GetComponent<PhysicsEffects>();
			if (physicsEffect != null)
			{
				physicsEffect.entity = this;
				if (this.item.info.physImpactSoundDef != null)
				{
					physicsEffect.physImpactSoundDef = this.item.info.physImpactSoundDef;
				}
			}
		}
		gameObject.SetActive(true);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (this.GetDespawnDuration() < Single.PositiveInfinity)
		{
			base.Invoke(new Action(this.IdleDestroy), this.GetDespawnDuration());
		}
		base.ReceiveCollisionMessages(true);
	}
}