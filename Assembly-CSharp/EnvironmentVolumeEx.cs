using Facepunch;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class EnvironmentVolumeEx
{
	public static bool CheckEnvironmentVolumes(this Transform transform, Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type)
	{
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		transform.GetComponentsInChildren<EnvironmentVolume>(true, list);
		for (int i = 0; i < list.Count; i++)
		{
			EnvironmentVolume item = list[i];
			OBB oBB = new OBB(item.transform, new Bounds(item.Center, item.Size));
			oBB.Transform(pos, scale, rot);
			if (EnvironmentManager.Check(oBB, type))
			{
				Pool.FreeList<EnvironmentVolume>(ref list);
				return true;
			}
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return false;
	}

	public static bool CheckEnvironmentVolumes(this Transform transform, EnvironmentType type)
	{
		return transform.CheckEnvironmentVolumes(transform.position, transform.rotation, transform.lossyScale, type);
	}
}