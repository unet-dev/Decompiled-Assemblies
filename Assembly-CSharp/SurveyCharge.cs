using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SurveyCharge : TimedExplosive
{
	public GameObjectRef craterPrefab;

	public GameObjectRef craterPrefab_Oil;

	public SurveyCharge()
	{
	}

	public override void Explode()
	{
		RaycastHit raycastHit;
		base.Explode();
		if (WaterLevel.Test(base.transform.position))
		{
			return;
		}
		ResourceDepositManager.ResourceDeposit orCreate = ResourceDepositManager.GetOrCreate(base.transform.position);
		if (orCreate == null)
		{
			return;
		}
		if (Time.realtimeSinceStartup - orCreate.lastSurveyTime < 10f)
		{
			return;
		}
		orCreate.lastSurveyTime = Time.realtimeSinceStartup;
		if (!TransformUtil.GetGroundInfo(base.transform.position, out raycastHit, 0.3f, 8388608, null))
		{
			return;
		}
		Vector3 vector3 = raycastHit.point;
		Vector3 vector31 = raycastHit.normal;
		List<SurveyCrater> list = Pool.GetList<SurveyCrater>();
		Vis.Entities<SurveyCrater>(base.transform.position, 10f, list, 1, QueryTriggerInteraction.Collide);
		bool count = list.Count > 0;
		Pool.FreeList<SurveyCrater>(ref list);
		if (count)
		{
			return;
		}
		bool flag = false;
		bool flag1 = false;
		foreach (ResourceDepositManager.ResourceDeposit.ResourceDepositEntry _resource in orCreate._resources)
		{
			if (_resource.spawnType != ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM || _resource.isLiquid || _resource.amount < 1000)
			{
				continue;
			}
			int num = Mathf.Clamp(Mathf.CeilToInt(2.5f / _resource.workNeeded * 10f), 0, 5);
			int num1 = 1;
			flag = true;
			if (_resource.isLiquid)
			{
				flag1 = true;
			}
			for (int i = 0; i < num; i++)
			{
				Item item = ItemManager.Create(_resource.type, num1, (ulong)0);
				Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(20f, Vector3.up, true);
				item.Drop(base.transform.position + (Vector3.up * 1f), this.GetInheritedDropVelocity() + (modifiedAimConeDirection * UnityEngine.Random.Range(5f, 10f)), UnityEngine.Random.rotation).SetAngularVelocity(UnityEngine.Random.rotation.eulerAngles * 5f);
			}
		}
		if (flag)
		{
			string str = (flag1 ? this.craterPrefab_Oil.resourcePath : this.craterPrefab.resourcePath);
			BaseEntity baseEntity = GameManager.server.CreateEntity(str, vector3, Quaternion.identity, true);
			if (baseEntity)
			{
				baseEntity.Spawn();
			}
		}
	}
}