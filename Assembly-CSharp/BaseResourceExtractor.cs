using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseResourceExtractor : BaseCombatEntity
{
	public bool canExtractLiquid;

	public bool canExtractSolid = true;

	public BaseResourceExtractor()
	{
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isClient)
		{
			return;
		}
		List<SurveyCrater> list = Pool.GetList<SurveyCrater>();
		Vis.Entities<SurveyCrater>(base.transform.position, 3f, list, 1, QueryTriggerInteraction.Collide);
		foreach (SurveyCrater surveyCrater in list)
		{
			if (!surveyCrater.isServer)
			{
				continue;
			}
			surveyCrater.Kill(BaseNetworkable.DestroyMode.None);
		}
		Pool.FreeList<SurveyCrater>(ref list);
	}
}