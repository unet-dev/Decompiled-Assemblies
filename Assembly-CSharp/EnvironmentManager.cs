using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : SingletonComponent<EnvironmentManager>
{
	public EnvironmentManager()
	{
	}

	public static bool Check(OBB obb, EnvironmentType type)
	{
		return (int)(EnvironmentManager.Get(obb) & type) != 0;
	}

	public static bool Check(Vector3 pos, EnvironmentType type)
	{
		return (int)(EnvironmentManager.Get(pos) & type) != 0;
	}

	public static EnvironmentType Get(OBB obb)
	{
		EnvironmentType type = (EnvironmentType)0;
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		GamePhysics.OverlapOBB<EnvironmentVolume>(obb, list, 262144, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			type |= list[i].Type;
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return type;
	}

	public static EnvironmentType Get(Vector3 pos, ref List<EnvironmentVolume> list)
	{
		EnvironmentType type = (EnvironmentType)0;
		GamePhysics.OverlapSphere<EnvironmentVolume>(pos, 0.01f, list, 262144, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			type |= list[i].Type;
		}
		return type;
	}

	public static EnvironmentType Get(Vector3 pos)
	{
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		EnvironmentType environmentType = EnvironmentManager.Get(pos, ref list);
		Pool.FreeList<EnvironmentVolume>(ref list);
		return environmentType;
	}
}