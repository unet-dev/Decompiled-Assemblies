using Facepunch;
using Network;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CeilingLight : IOEntity
{
	public float pushScale = 2f;

	public CeilingLight()
	{
	}

	public override int ConsumptionAmount()
	{
		if (base.IsOn())
		{
			return 2;
		}
		return base.ConsumptionAmount();
	}

	public override void Hurt(HitInfo info)
	{
		if (base.isServer)
		{
			if (info.damageTypes.Has(DamageType.Explosion))
			{
				base.ClientRPC<int, Vector3, Vector3>(null, "ClientPhysPush", 0, (info.attackNormal * 3f) * (info.damageTypes.Total() / 50f), info.HitPositionWorld);
			}
			base.Hurt(info);
		}
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		bool flag = base.IsOn();
		base.SetFlag(BaseEntity.Flags.On, this.IsPowered(), false, true);
		if (flag != base.IsOn())
		{
			if (base.IsOn())
			{
				this.LightsOn();
				return;
			}
			this.LightsOff();
		}
	}

	public void LightsOff()
	{
		this.RefreshPlants();
	}

	public void LightsOn()
	{
		this.RefreshPlants();
	}

	public override void OnAttacked(HitInfo info)
	{
		uint d;
		float single = 3f * (info.damageTypes.Total() / 50f);
		if (!(info.Initiator != null) || !(info.Initiator is BasePlayer) || info.IsPredicting)
		{
			d = 0;
		}
		else
		{
			d = info.Initiator.net.ID;
		}
		base.ClientRPC<uint, Vector3, Vector3>(null, "ClientPhysPush", d, info.attackNormal * single, info.HitPositionWorld);
		base.OnAttacked(info);
	}

	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		this.RefreshPlants();
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("CeilingLight.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public void RefreshPlants()
	{
		List<PlantEntity> list = Pool.GetList<PlantEntity>();
		Vis.Entities<PlantEntity>(base.transform.position + new Vector3(0f, -2f, 0f), 5f, list, 512, QueryTriggerInteraction.Collide);
		foreach (PlantEntity plantEntity in list)
		{
			plantEntity.RefreshLightExposure();
		}
		Pool.FreeList<PlantEntity>(ref list);
	}
}