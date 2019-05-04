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
				BaseEntity baseEntity = gameManager.CreateEntity(str, vector3, quaternion, true);
				if (baseEntity)
				{
					Vector3 hitPositionWorld = info.HitPositionWorld;
					Vector3 hitNormalWorld = info.HitNormalWorld.normalized;
					Vector3 vector31 = hitPositionWorld + (hitNormalWorld * 0.1f);
					if (!GamePhysics.LineOfSight(hitPositionWorld, vector31, 2162688, 0f))
					{
						baseEntity.transform.position = hitPositionWorld;
					}
					else
					{
						baseEntity.transform.position = vector31;
					}
					baseEntity.transform.rotation = Quaternion.LookRotation(hitNormalWorld);
					baseEntity.Spawn();
					if (this.spreadAngle > 0f)
					{
						Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(this.spreadAngle, hitNormalWorld, true);
						baseEntity.SetVelocity(modifiedAimConeDirection * UnityEngine.Random.Range(1f, 3f));
					}
				}
			}
		}
		base.ServerProjectileHit(info);
	}
}