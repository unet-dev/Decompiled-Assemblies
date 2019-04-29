using System;
using UnityEngine;

public class FlameExplosive : TimedExplosive
{
	public GameObjectRef createOnExplode;

	public float numToCreate = 10f;

	public float minVelocity = 2f;

	public float maxVelocity = 5f;

	public float spreadAngle = 90f;

	public FlameExplosive()
	{
	}

	public override void Explode()
	{
		this.Explode(-base.transform.forward);
	}

	public void Explode(Vector3 surfaceNormal)
	{
		if (!base.isServer)
		{
			return;
		}
		for (int i = 0; (float)i < this.numToCreate; i++)
		{
			GameManager gameManager = GameManager.server;
			string str = this.createOnExplode.resourcePath;
			Vector3 vector3 = base.transform.position;
			Quaternion quaternion = new Quaternion();
			BaseEntity baseEntity = gameManager.CreateEntity(str, vector3, quaternion, true);
			if (baseEntity)
			{
				baseEntity.transform.position = base.transform.position;
				Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(this.spreadAngle, surfaceNormal, true);
				baseEntity.transform.rotation = Quaternion.LookRotation(modifiedAimConeDirection);
				baseEntity.creatorEntity = (this.creatorEntity == null ? baseEntity : this.creatorEntity);
				baseEntity.Spawn();
				baseEntity.SetVelocity(modifiedAimConeDirection * UnityEngine.Random.Range(this.minVelocity, this.maxVelocity));
			}
		}
		base.Explode();
	}

	public override void ProjectileImpact(RaycastHit info)
	{
		this.Explode(info.normal);
	}
}