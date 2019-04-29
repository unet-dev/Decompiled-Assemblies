using System;
using UnityEngine;

public class OreHotSpot : BaseCombatEntity, ILOD
{
	public float visualDistance = 20f;

	public GameObjectRef visualEffect;

	public GameObjectRef finishEffect;

	public GameObjectRef damageEffect;

	public OreResourceEntity owner;

	public OreHotSpot()
	{
	}

	public void FireFinishEffect()
	{
		if (this.finishEffect.isValid)
		{
			Effect.server.Run(this.finishEffect.resourcePath, base.transform.position, base.transform.forward, null, false);
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (base.isClient)
		{
			return;
		}
		if (this.owner)
		{
			this.owner.OnAttacked(info);
		}
	}

	public override void OnKilled(HitInfo info)
	{
		this.FireFinishEffect();
		base.OnKilled(info);
	}

	public void OreOwner(OreResourceEntity newOwner)
	{
		this.owner = newOwner;
	}

	public override void ServerInit()
	{
		base.ServerInit();
	}
}