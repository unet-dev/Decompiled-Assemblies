using System;
using UnityEngine;

public class ItemModProjectileSpawn : ItemModProjectile
{
	public float createOnImpactChance;

	public GameObjectRef createOnImpact = new GameObjectRef();

	public float spreadAngle = 30f;

	public float spreadVelocityMin = 1f;

	public float spreadVelocityMax = 3f;

	public int numToCreateChances = 1;

	public ItemModProjectileSpawn()
	{
	}

	public override void ServerProjectileHit(HitInfo info)
	{
		for (int i = 0; i < this.numToCreateChances; i++)
		{
			if (this.createOnImpact.isValid && UnityEngine.Random.Range(0f, 1f) < this.createOnImpactChance)
			{
				GameManager gameManager = GameManager.server;
				string str = this.createOnImpact.resourcePath;
				Vector3 vector3 = new Vector3();
				Quaternion quaternion = new Quaternion();
				BaseEntity hitPositionWorld = gameManager.CreateEntity(str, vector3, quaternion, true);
				if (hitPositionWorld)
				{
					hitPositionWorld.transform.position = info.HitPositionWorld + (info.HitNormalWorld * 0.1f);
					hitPositionWorld.transform.rotation = Quaternion.LookRotation(info.HitNormalWorld);
					if (!GamePhysics.LineOfSight(info.HitPositionWorld, hitPositionWorld.transform.position, 2162688, 0f))
					{
						hitPositionWorld.transform.position = info.HitPositionWorld;
					}
					hitPositionWorld.Spawn();
					if (this.spreadAngle > 0f)
					{
						Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(this.spreadAngle, info.HitNormalWorld, true);
						hitPositionWorld.SetVelocity(modifiedAimConeDirection * UnityEngine.Random.Range(1f, 3f));
					}
				}
			}
		}
		base.ServerProjectileHit(info);
	}
}