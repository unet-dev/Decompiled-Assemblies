using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterDebris : ServerGib
{
	public ItemDefinition metalFragments;

	public ItemDefinition hqMetal;

	public ItemDefinition charcoal;

	private ResourceDispenser resourceDispenser;

	private float tooHotUntil;

	public HelicopterDebris()
	{
	}

	public bool IsTooHot()
	{
		return this.tooHotUntil > Time.realtimeSinceStartup;
	}

	public override void OnAttacked(HitInfo info)
	{
		if (!this.IsTooHot() || !(info.WeaponPrefab is BaseMelee))
		{
			if (this.resourceDispenser)
			{
				this.resourceDispenser.OnAttacked(info);
			}
			base.OnAttacked(info);
		}
		else if (info.Initiator is BasePlayer)
		{
			HitInfo hitInfo = new HitInfo();
			hitInfo.damageTypes.Add(DamageType.Heat, 5f);
			hitInfo.DoHitEffects = true;
			hitInfo.DidHit = true;
			hitInfo.HitBone = 0;
			hitInfo.Initiator = this;
			hitInfo.PointStart = base.transform.position;
			Effect.server.Run("assets/bundled/prefabs/fx/impacts/additive/fire.prefab", info.Initiator, 0, new Vector3(0f, 1f, 0f), Vector3.up, null, false);
			return;
		}
	}

	public override void PhysicsInit(Mesh mesh)
	{
		base.PhysicsInit(mesh);
		if (base.isServer)
		{
			this.resourceDispenser = base.GetComponent<ResourceDispenser>();
			float single = Mathf.Clamp01(base.GetComponent<Rigidbody>().mass / 5f);
			this.resourceDispenser.containedItems = new List<ItemAmount>();
			if (single > 0.75f)
			{
				this.resourceDispenser.containedItems.Add(new ItemAmount(this.hqMetal, 7f * single));
			}
			if (single > 0f)
			{
				this.resourceDispenser.containedItems.Add(new ItemAmount(this.metalFragments, 150f * single));
				this.resourceDispenser.containedItems.Add(new ItemAmount(this.charcoal, 80f * single));
			}
			this.resourceDispenser.Initialize();
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this.tooHotUntil = Time.realtimeSinceStartup + 480f;
	}
}