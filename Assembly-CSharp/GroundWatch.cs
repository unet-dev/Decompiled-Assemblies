using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GroundWatch : MonoBehaviour, IServerComponent
{
	public Vector3 groundPosition = Vector3.zero;

	public LayerMask layers = 27328512;

	public float radius = 0.1f;

	public GroundWatch()
	{
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(this.groundPosition, this.radius);
	}

	private bool OnGround()
	{
		bool flag;
		BaseEntity component = base.GetComponent<BaseEntity>();
		if (component)
		{
			Construction construction = PrefabAttribute.server.Find<Construction>(component.prefabID);
			if (construction)
			{
				Socket_Base[] socketBaseArray = construction.allSockets;
				for (int i = 0; i < (int)socketBaseArray.Length; i++)
				{
					SocketMod[] socketModArray = socketBaseArray[i].socketMods;
					for (int j = 0; j < (int)socketModArray.Length; j++)
					{
						SocketMod_AreaCheck socketModAreaCheck = socketModArray[j] as SocketMod_AreaCheck;
						if (socketModAreaCheck && socketModAreaCheck.wantsInside && !socketModAreaCheck.DoCheck(component.transform.position, component.transform.rotation))
						{
							return false;
						}
					}
				}
			}
		}
		List<Collider> list = Pool.GetList<Collider>();
		Vis.Colliders<Collider>(base.transform.TransformPoint(this.groundPosition), this.radius, list, this.layers, QueryTriggerInteraction.Collide);
		List<Collider>.Enumerator enumerator = list.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BaseEntity baseEntity = enumerator.Current.gameObject.ToBaseEntity();
				if (baseEntity && (baseEntity == component || baseEntity.IsDestroyed || baseEntity.isClient))
				{
					continue;
				}
				Pool.FreeList<Collider>(ref list);
				flag = true;
				return flag;
			}
			Pool.FreeList<Collider>(ref list);
			return false;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return flag;
	}

	private void OnPhysicsNeighbourChanged()
	{
		if (!this.OnGround())
		{
			base.transform.root.BroadcastMessage("OnGroundMissing", SendMessageOptions.DontRequireReceiver);
		}
	}

	public static void PhysicsChanged(GameObject obj)
	{
		Collider component = obj.GetComponent<Collider>();
		if (!component)
		{
			return;
		}
		Bounds bound = component.bounds;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vector3 vector3 = bound.center;
		Vector3 vector31 = bound.extents;
		Vis.Entities<BaseEntity>(vector3, vector31.magnitude + 1f, list, 2228480, QueryTriggerInteraction.Collide);
		foreach (BaseEntity baseEntity in list)
		{
			if (baseEntity.IsDestroyed || baseEntity.isClient || baseEntity is BuildingBlock)
			{
				continue;
			}
			baseEntity.BroadcastMessage("OnPhysicsNeighbourChanged", SendMessageOptions.DontRequireReceiver);
		}
		Pool.FreeList<BaseEntity>(ref list);
	}
}