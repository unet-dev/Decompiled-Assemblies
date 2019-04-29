using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class DeployVolume : PrefabAttribute
{
	public LayerMask layers = 537001984;

	[InspectorFlags]
	public ColliderInfo.Flags ignore;

	protected DeployVolume()
	{
	}

	protected abstract bool Check(Vector3 position, Quaternion rotation, int mask = -1);

	protected abstract bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1);

	public static bool Check(Vector3 position, Quaternion rotation, DeployVolume[] volumes, int mask = -1)
	{
		for (int i = 0; i < (int)volumes.Length; i++)
		{
			if (volumes[i].Check(position, rotation, mask))
			{
				return true;
			}
		}
		return false;
	}

	public static bool Check(Vector3 position, Quaternion rotation, DeployVolume[] volumes, OBB test, int mask = -1)
	{
		for (int i = 0; i < (int)volumes.Length; i++)
		{
			if (volumes[i].Check(position, rotation, test, mask))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckBounds(Bounds bounds, int layerMask, ColliderInfo.Flags ignore)
	{
		if ((int)ignore == 0)
		{
			return GamePhysics.CheckBounds(bounds, layerMask, QueryTriggerInteraction.UseGlobal);
		}
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapBounds(bounds, list, layerMask, QueryTriggerInteraction.Ignore);
		bool flag = DeployVolume.CheckFlags(list, ignore);
		Pool.FreeList<Collider>(ref list);
		return flag;
	}

	public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layerMask, ColliderInfo.Flags ignore)
	{
		if ((int)ignore == 0)
		{
			return GamePhysics.CheckCapsule(start, end, radius, layerMask, QueryTriggerInteraction.UseGlobal);
		}
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapCapsule(start, end, radius, list, layerMask, QueryTriggerInteraction.Ignore);
		bool flag = DeployVolume.CheckFlags(list, ignore);
		Pool.FreeList<Collider>(ref list);
		return flag;
	}

	private static bool CheckFlags(List<Collider> list, ColliderInfo.Flags ignore)
	{
		for (int i = 0; i < list.Count; i++)
		{
			ColliderInfo component = list[i].gameObject.GetComponent<ColliderInfo>();
			if (component == null)
			{
				return true;
			}
			if (!component.HasFlag(ignore))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckOBB(OBB obb, int layerMask, ColliderInfo.Flags ignore)
	{
		if ((int)ignore == 0)
		{
			return GamePhysics.CheckOBB(obb, layerMask, QueryTriggerInteraction.UseGlobal);
		}
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapOBB(obb, list, layerMask, QueryTriggerInteraction.Ignore);
		bool flag = DeployVolume.CheckFlags(list, ignore);
		Pool.FreeList<Collider>(ref list);
		return flag;
	}

	public static bool CheckSphere(Vector3 pos, float radius, int layerMask, ColliderInfo.Flags ignore)
	{
		if ((int)ignore == 0)
		{
			return GamePhysics.CheckSphere(pos, radius, layerMask, QueryTriggerInteraction.UseGlobal);
		}
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, radius, list, layerMask, QueryTriggerInteraction.Ignore);
		bool flag = DeployVolume.CheckFlags(list, ignore);
		Pool.FreeList<Collider>(ref list);
		return flag;
	}

	protected override Type GetIndexedType()
	{
		return typeof(DeployVolume);
	}
}