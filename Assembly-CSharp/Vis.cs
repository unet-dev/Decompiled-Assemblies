using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class Vis
{
	public static Collider[] colBuffer;

	static Vis()
	{
		Vis.colBuffer = new Collider[8192];
	}

	public static void Colliders<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2)
	where T : Collider
	{
		layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
		int num = Physics.OverlapSphereNonAlloc(position, radius, Vis.colBuffer, layerMask, triggerInteraction);
		if (num >= (int)Vis.colBuffer.Length)
		{
			Debug.LogWarning("Vis query is exceeding collider buffer length.");
		}
		for (int i = 0; i < num; i++)
		{
			T t = (T)(Vis.colBuffer[i] as T);
			Vis.colBuffer[i] = null;
			if (!(t == null) && t.enabled)
			{
				if (!t.transform.CompareTag("MeshColliderBatch"))
				{
					list.Add(t);
				}
				else
				{
					t.transform.GetComponent<MeshColliderBatch>().LookupColliders<T>(position, radius, list);
				}
			}
		}
	}

	public static void Colliders<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2)
	where T : Collider
	{
		layerMask = GamePhysics.HandleTerrainCollision(bounds.position, layerMask);
		int num = Physics.OverlapBoxNonAlloc(bounds.position, bounds.extents, Vis.colBuffer, bounds.rotation, layerMask, triggerInteraction);
		if (num >= (int)Vis.colBuffer.Length)
		{
			Debug.LogWarning("Vis query is exceeding collider buffer length.");
		}
		for (int i = 0; i < num; i++)
		{
			T t = (T)(Vis.colBuffer[i] as T);
			Vis.colBuffer[i] = null;
			if (!(t == null) && t.enabled)
			{
				if (!t.transform.CompareTag("MeshColliderBatch"))
				{
					list.Add(t);
				}
				else
				{
					t.transform.GetComponent<MeshColliderBatch>().LookupColliders<T>(bounds, list);
				}
			}
		}
	}

	public static void Components<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2)
	where T : Component
	{
		List<Collider> colliders = Pool.GetList<Collider>();
		Vis.Colliders<Collider>(position, radius, colliders, layerMask, triggerInteraction);
		for (int i = 0; i < colliders.Count; i++)
		{
			T component = colliders[i].gameObject.GetComponent<T>();
			if (component != null)
			{
				list.Add(component);
			}
		}
		Pool.FreeList<Collider>(ref colliders);
	}

	public static void Components<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2)
	where T : Component
	{
		List<Collider> colliders = Pool.GetList<Collider>();
		Vis.Colliders<Collider>(bounds, colliders, layerMask, triggerInteraction);
		for (int i = 0; i < colliders.Count; i++)
		{
			T component = colliders[i].gameObject.GetComponent<T>();
			if (component != null)
			{
				list.Add(component);
			}
		}
		Pool.FreeList<Collider>(ref colliders);
	}

	public static void Entities<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2)
	where T : BaseEntity
	{
		List<Collider> colliders = Pool.GetList<Collider>();
		Vis.Colliders<Collider>(position, radius, colliders, layerMask, triggerInteraction);
		for (int i = 0; i < colliders.Count; i++)
		{
			T baseEntity = (T)(colliders[i].gameObject.ToBaseEntity() as T);
			if (baseEntity != null)
			{
				list.Add(baseEntity);
			}
		}
		Pool.FreeList<Collider>(ref colliders);
	}

	public static void Entities<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2)
	where T : BaseEntity
	{
		List<Collider> colliders = Pool.GetList<Collider>();
		Vis.Colliders<Collider>(bounds, colliders, layerMask, triggerInteraction);
		for (int i = 0; i < colliders.Count; i++)
		{
			T baseEntity = (T)(colliders[i].gameObject.ToBaseEntity() as T);
			if (baseEntity != null)
			{
				list.Add(baseEntity);
			}
		}
		Pool.FreeList<Collider>(ref colliders);
	}

	public static void EntityComponents<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2)
	where T : EntityComponentBase
	{
		List<Collider> colliders = Pool.GetList<Collider>();
		Vis.Colliders<Collider>(position, radius, colliders, layerMask, triggerInteraction);
		for (int i = 0; i < colliders.Count; i++)
		{
			BaseEntity baseEntity = colliders[i].gameObject.ToBaseEntity();
			if (baseEntity != null)
			{
				T component = baseEntity.gameObject.GetComponent<T>();
				if (component != null)
				{
					list.Add(component);
				}
			}
		}
		Pool.FreeList<Collider>(ref colliders);
	}

	public static void EntityComponents<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2)
	where T : EntityComponentBase
	{
		List<Collider> colliders = Pool.GetList<Collider>();
		Vis.Colliders<Collider>(bounds, colliders, layerMask, triggerInteraction);
		for (int i = 0; i < colliders.Count; i++)
		{
			BaseEntity baseEntity = colliders[i].gameObject.ToBaseEntity();
			if (baseEntity != null)
			{
				T component = baseEntity.gameObject.GetComponent<T>();
				if (component != null)
				{
					list.Add(component);
				}
			}
		}
		Pool.FreeList<Collider>(ref colliders);
	}
}