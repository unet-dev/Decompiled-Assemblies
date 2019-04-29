using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WaterBall : BaseEntity
{
	public ItemDefinition liquidType;

	public int waterAmount;

	public GameObjectRef waterExplosion;

	public Rigidbody myRigidBody;

	public WaterBall()
	{
	}

	public void DoSplash()
	{
		float single = 2.5f;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(base.transform.position + new Vector3(0f, single * 0.75f, 0f), single, list, 1219701521, QueryTriggerInteraction.Collide);
		for (int i = 0; this.waterAmount > 0 && i < 3; i++)
		{
			List<ISplashable> splashables = Pool.GetList<ISplashable>();
			foreach (BaseEntity baseEntity in list)
			{
				if (baseEntity.isClient)
				{
					continue;
				}
				ISplashable splashable = baseEntity as ISplashable;
				if (splashable == null || splashables.Contains(splashable) || !splashable.wantsSplash(this.liquidType, this.waterAmount))
				{
					continue;
				}
				splashables.Add(splashable);
			}
			if (splashables.Count == 0)
			{
				break;
			}
			int num = Mathf.CeilToInt((float)(this.waterAmount / splashables.Count));
			foreach (ISplashable splashable1 in splashables)
			{
				int num1 = splashable1.DoSplash(this.liquidType, Mathf.Min(this.waterAmount, num));
				this.waterAmount -= num1;
				if (this.waterAmount > 0)
				{
					continue;
				}
				goto Label0;
			}
		Label0:
			Pool.FreeList<ISplashable>(ref splashables);
		}
		Pool.FreeList<BaseEntity>(ref list);
	}

	public void Extinguish()
	{
		base.CancelInvoke(new Action(this.Extinguish));
		if (!base.IsDestroyed)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	public void FixedUpdate()
	{
		if (base.isServer)
		{
			base.GetComponent<Rigidbody>().AddForce(Physics.gravity, ForceMode.Acceleration);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (base.isClient)
		{
			return;
		}
		if (this.myRigidBody.isKinematic)
		{
			return;
		}
		this.DoSplash();
		Effect.server.Run(this.waterExplosion.resourcePath, base.transform.position + new Vector3(0f, 0f, 0f), Vector3.up, null, false);
		this.myRigidBody.isKinematic = true;
		base.Invoke(new Action(this.Extinguish), 2f);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.Extinguish), 10f);
	}
}