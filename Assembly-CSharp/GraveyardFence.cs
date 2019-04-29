using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardFence : SimpleBuildingBlock
{
	public BoxCollider[] pillars;

	public GraveyardFence()
	{
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		List<GraveyardFence> list = Pool.GetList<GraveyardFence>();
		Vis.Entities<GraveyardFence>(base.transform.position, 5f, list, 2097152, QueryTriggerInteraction.Collide);
		foreach (GraveyardFence graveyardFence in list)
		{
			graveyardFence.UpdatePillars();
		}
		Pool.FreeList<GraveyardFence>(ref list);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this.UpdatePillars();
	}

	public void UpdatePillars()
	{
		BoxCollider[] boxColliderArray = this.pillars;
		for (int i = 0; i < (int)boxColliderArray.Length; i++)
		{
			BoxCollider boxCollider = boxColliderArray[i];
			boxCollider.gameObject.SetActive(true);
			Collider[] colliderArray = Physics.OverlapBox(boxCollider.transform.TransformPoint(boxCollider.center), boxCollider.size * 0.5f, boxCollider.transform.rotation, 2097152);
			for (int j = 0; j < (int)colliderArray.Length; j++)
			{
				Collider collider = colliderArray[j];
				if (collider.CompareTag("Usable Auxiliary"))
				{
					BaseEntity baseEntity = collider.gameObject.ToBaseEntity();
					if (!(baseEntity == null) && !base.EqualNetID(baseEntity) && collider != boxCollider)
					{
						boxCollider.gameObject.SetActive(false);
					}
				}
			}
		}
	}
}