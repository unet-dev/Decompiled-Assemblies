using Facepunch;
using ProtoBuf;
using System;
using UnityEngine;

public class PlanterBox : BaseCombatEntity, ISplashable
{
	public int soilSaturation;

	public int soilSaturationMax = 8000;

	public MeshRenderer soilRenderer;

	public float soilSaturationFraction
	{
		get
		{
			return (float)this.soilSaturation / (float)this.soilSaturationMax;
		}
	}

	public PlanterBox()
	{
	}

	public int DoSplash(ItemDefinition splashType, int amount)
	{
		if (splashType.shortname == "water.salt")
		{
			this.soilSaturation = 0;
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			return amount;
		}
		int num = Mathf.Min(this.soilSaturationMax - this.soilSaturation, amount);
		this.soilSaturation += num;
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		return num;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.resource != null)
		{
			this.soilSaturation = info.msg.resource.stage;
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.resource = Pool.Get<BaseResource>();
		info.msg.resource.stage = this.soilSaturation;
	}

	public int UseWater(int amount)
	{
		int num = Mathf.Min(amount, this.soilSaturation);
		this.soilSaturation -= num;
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		return num;
	}

	public bool wantsSplash(ItemDefinition splashType, int amount)
	{
		if (splashType.shortname == "water.salt")
		{
			return true;
		}
		return this.soilSaturation < this.soilSaturationMax;
	}
}