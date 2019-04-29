using Facepunch;
using ProtoBuf;
using Rust;
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ResourceEntity : BaseEntity
{
	[FormerlySerializedAs("health")]
	public float startHealth;

	[FormerlySerializedAs("protection")]
	public ProtectionProperties baseProtection;

	protected float health;

	internal ResourceDispenser resourceDispenser;

	[NonSerialized]
	protected bool isKilled;

	public ResourceEntity()
	{
	}

	public override float BoundsPadding()
	{
		return 1f;
	}

	public override float Health()
	{
		return this.health;
	}

	public override void InitShared()
	{
		base.InitShared();
		if (base.isServer)
		{
			DecorComponent[] decorComponentArray = PrefabAttribute.server.FindAll<DecorComponent>(this.prefabID);
			base.transform.ApplyDecorComponentsScaleOnly(decorComponentArray);
		}
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.resource == null)
		{
			return;
		}
		this.health = info.msg.resource.health;
	}

	public override float MaxHealth()
	{
		return this.startHealth;
	}

	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer && !this.isKilled)
		{
			if (this.resourceDispenser != null)
			{
				this.resourceDispenser.OnAttacked(info);
			}
			if (!info.DidGather)
			{
				if (this.baseProtection)
				{
					this.baseProtection.Scale(info.damageTypes, 1f);
				}
				this.health -= info.damageTypes.Total();
				if (this.health <= 0f)
				{
					this.OnKilled(info);
					return;
				}
				this.OnHealthChanged();
			}
		}
	}

	protected virtual void OnHealthChanged()
	{
	}

	public virtual void OnKilled(HitInfo info)
	{
		this.isKilled = true;
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.resource = Pool.Get<BaseResource>();
			info.msg.resource.health = this.Health();
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this.resourceDispenser = base.GetComponent<ResourceDispenser>();
		if (this.health == 0f)
		{
			this.health = this.startHealth;
		}
	}
}