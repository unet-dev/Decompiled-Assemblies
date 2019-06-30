using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseCorpse : BaseCombatEntity
{
	public GameObjectRef prefabRagdoll;

	public BaseEntity parentEnt;

	[NonSerialized]
	internal ResourceDispenser resourceDispenser;

	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return base.Traits | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	public BaseCorpse()
	{
	}

	public virtual bool CanRemove()
	{
		return true;
	}

	public override string Categorize()
	{
		return "corpse";
	}

	public override void Eat(BaseNpc baseNpc, float timeSpent)
	{
		this.ResetRemovalTime();
		this.Hurt(timeSpent * 5f);
		baseNpc.AddCalories(timeSpent * 2f);
	}

	public virtual float GetRemovalTime()
	{
		return ConVar.Server.corpsedespawn;
	}

	public virtual void InitCorpse(BaseEntity pr)
	{
		this.parentEnt = pr;
		base.transform.position = this.parentEnt.CenterPoint();
		base.transform.rotation = this.parentEnt.transform.rotation;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.corpse != null)
		{
			this.Load(info.msg.corpse);
		}
	}

	private void Load(Corpse corpse)
	{
		if (base.isServer)
		{
			this.parentEnt = BaseNetworkable.serverEntities.Find(corpse.parentID) as BaseEntity;
		}
		bool flag = base.isClient;
	}

	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer)
		{
			this.ResetRemovalTime();
			if (this.resourceDispenser)
			{
				this.resourceDispenser.OnAttacked(info);
			}
			if (!info.DidGather)
			{
				base.OnAttacked(info);
			}
		}
	}

	public void RemoveCorpse()
	{
		if (!this.CanRemove())
		{
			this.ResetRemovalTime();
			return;
		}
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public void ResetRemovalTime(float dur)
	{
		using (TimeWarning timeWarning = TimeWarning.New("ResetRemovalTime", 0.1f))
		{
			if (base.IsInvoking(new Action(this.RemoveCorpse)))
			{
				base.CancelInvoke(new Action(this.RemoveCorpse));
			}
			base.Invoke(new Action(this.RemoveCorpse), dur);
		}
	}

	public void ResetRemovalTime()
	{
		this.ResetRemovalTime(this.GetRemovalTime());
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.corpse = Facepunch.Pool.Get<Corpse>();
		if (this.parentEnt.IsValid())
		{
			info.msg.corpse.parentID = this.parentEnt.net.ID;
		}
	}

	public override void ServerInit()
	{
		this.SetupRigidBody();
		this.ResetRemovalTime();
		this.resourceDispenser = base.GetComponent<ResourceDispenser>();
		base.ServerInit();
	}

	private Rigidbody SetupRigidBody()
	{
		if (base.isServer)
		{
			GameObject gameObject = base.gameManager.FindPrefab(this.prefabRagdoll.resourcePath);
			if (gameObject == null)
			{
				return null;
			}
			Ragdoll component = gameObject.GetComponent<Ragdoll>();
			if (component == null)
			{
				return null;
			}
			if (component.primaryBody == null)
			{
				Debug.LogError(string.Concat("[BaseCorpse] ragdoll.primaryBody isn't set!", component.gameObject.name));
				return null;
			}
			BoxCollider boxCollider = component.primaryBody.GetComponent<BoxCollider>();
			if (boxCollider == null)
			{
				Debug.LogError("Ragdoll has unsupported primary collider (make it supported) ", component);
				return null;
			}
			BoxCollider boxCollider1 = base.gameObject.AddComponent<BoxCollider>();
			boxCollider1.size = boxCollider.size * 2f;
			boxCollider1.center = boxCollider.center;
			boxCollider1.sharedMaterial = boxCollider.sharedMaterial;
		}
		Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
		if (rigidbody == null)
		{
			Debug.LogError(string.Concat("[BaseCorpse] already has a RigidBody defined - and it shouldn't!", base.gameObject.name));
			return null;
		}
		rigidbody.mass = 10f;
		rigidbody.useGravity = true;
		rigidbody.drag = 0.5f;
		rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
		if (base.isServer)
		{
			Buoyancy buoyancy = base.GetComponent<Buoyancy>();
			if (buoyancy != null)
			{
				buoyancy.rigidBody = rigidbody;
			}
			ConVar.Physics.ApplyDropped(rigidbody);
			Vector3 vector3 = Vector3Ex.Range(-1f, 1f);
			vector3.y += 1f;
			rigidbody.velocity = vector3;
			rigidbody.angularVelocity = Vector3Ex.Range(-10f, 10f);
		}
		return rigidbody;
	}

	public override bool ShouldInheritNetworkGroup()
	{
		return false;
	}

	public void TakeChildren(BaseEntity takeChildrenFrom)
	{
		if (takeChildrenFrom.children == null)
		{
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("Corpse.TakeChildren", 0.1f))
		{
			BaseEntity[] array = takeChildrenFrom.children.ToArray();
			for (int i = 0; i < (int)array.Length; i++)
			{
				array[i].SwitchParent(this);
			}
		}
	}
}