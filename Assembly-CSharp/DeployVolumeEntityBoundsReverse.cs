using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DeployVolumeEntityBoundsReverse : DeployVolume
{
	private Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	private int layer;

	public DeployVolumeEntityBoundsReverse()
	{
	}

	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.bounds = rootObj.GetComponent<BaseEntity>().bounds;
		this.layer = rootObj.layer;
	}

	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		bool flag;
		position = position + (rotation * this.bounds.center);
		OBB oBB = new OBB(position, this.bounds.size, rotation);
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(position, oBB.extents.magnitude, list, this.layers & mask, QueryTriggerInteraction.Collide);
		List<BaseEntity>.Enumerator enumerator = list.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BaseEntity current = enumerator.Current;
				DeployVolume[] deployVolumeArray = PrefabAttribute.server.FindAll<DeployVolume>(current.prefabID);
				if (!DeployVolume.Check(current.transform.position, current.transform.rotation, deployVolumeArray, oBB, 1 << (this.layer & 31)))
				{
					continue;
				}
				Pool.FreeList<BaseEntity>(ref list);
				flag = true;
				return flag;
			}
			Pool.FreeList<BaseEntity>(ref list);
			return false;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return flag;
	}

	protected override bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1)
	{
		return false;
	}
}