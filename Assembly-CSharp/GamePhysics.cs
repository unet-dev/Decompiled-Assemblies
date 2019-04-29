using ConVar;
using Facepunch;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class GamePhysics
{
	public const int BufferLength = 8192;

	private static RaycastHit[] hitBuffer;

	private static Collider[] colBuffer;

	static GamePhysics()
	{
		GamePhysics.hitBuffer = new RaycastHit[8192];
		GamePhysics.colBuffer = new Collider[8192];
	}

	private static void BufferToList(int count, List<Collider> list)
	{
		if (count >= (int)GamePhysics.colBuffer.Length)
		{
			Debug.LogWarning("Physics query is exceeding collider buffer length.");
		}
		for (int i = 0; i < count; i++)
		{
			list.Add(GamePhysics.colBuffer[i]);
			GamePhysics.colBuffer[i] = null;
		}
	}

	private static void BufferToList<T>(int count, List<T> list)
	where T : Component
	{
		if (count >= (int)GamePhysics.colBuffer.Length)
		{
			Debug.LogWarning("Physics query is exceeding collider buffer length.");
		}
		for (int i = 0; i < count; i++)
		{
			T component = GamePhysics.colBuffer[i].gameObject.GetComponent<T>();
			if (component)
			{
				list.Add(component);
			}
			GamePhysics.colBuffer[i] = null;
		}
	}

	public static bool CheckBounds(Bounds bounds, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		layerMask = GamePhysics.HandleTerrainCollision(bounds.center, layerMask);
		return UnityEngine.Physics.CheckBox(bounds.center, bounds.extents, Quaternion.identity, layerMask, triggerInteraction);
	}

	public static bool CheckBounds<T>(Bounds bounds, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	where T : Component
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapBounds(bounds, list, layerMask, triggerInteraction);
		bool flag = GamePhysics.CheckComponent<T>(list);
		Facepunch.Pool.FreeList<Collider>(ref list);
		return flag;
	}

	public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		layerMask = GamePhysics.HandleTerrainCollision((start + end) * 0.5f, layerMask);
		return UnityEngine.Physics.CheckCapsule(start, end, radius, layerMask, triggerInteraction);
	}

	public static bool CheckCapsule<T>(Vector3 start, Vector3 end, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	where T : Component
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapCapsule(start, end, radius, list, layerMask, triggerInteraction);
		bool flag = GamePhysics.CheckComponent<T>(list);
		Facepunch.Pool.FreeList<Collider>(ref list);
		return flag;
	}

	private static bool CheckComponent<T>(List<Collider> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].gameObject.GetComponent<T>() != null)
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckOBB(OBB obb, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		layerMask = GamePhysics.HandleTerrainCollision(obb.position, layerMask);
		return UnityEngine.Physics.CheckBox(obb.position, obb.extents, obb.rotation, layerMask, triggerInteraction);
	}

	public static bool CheckOBB<T>(OBB obb, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	where T : Component
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapOBB(obb, list, layerMask, triggerInteraction);
		bool flag = GamePhysics.CheckComponent<T>(list);
		Facepunch.Pool.FreeList<Collider>(ref list);
		return flag;
	}

	public static bool CheckSphere(Vector3 position, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
		return UnityEngine.Physics.CheckSphere(position, radius, layerMask, triggerInteraction);
	}

	public static bool CheckSphere<T>(Vector3 pos, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	where T : Component
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, radius, list, layerMask, triggerInteraction);
		bool flag = GamePhysics.CheckComponent<T>(list);
		Facepunch.Pool.FreeList<Collider>(ref list);
		return flag;
	}

	public static int HandleTerrainCollision(Vector3 position, int layerMask)
	{
		int num = 8388608;
		if ((layerMask & num) != 0 && TerrainMeta.Collision && TerrainMeta.Collision.GetIgnore(position, 0.01f))
		{
			layerMask &= ~num;
		}
		return layerMask;
	}

	public static bool LineOfSight(Vector3 p0, Vector3 p1, int layerMask, float padding = 0f)
	{
		return GamePhysics.LineOfSightInternal(p0, p1, layerMask, padding, padding);
	}

	public static bool LineOfSight(Vector3 p0, Vector3 p1, Vector3 p2, int layerMask, float padding = 0f)
	{
		if (!GamePhysics.LineOfSightInternal(p0, p1, layerMask, padding, 0f))
		{
			return false;
		}
		return GamePhysics.LineOfSightInternal(p1, p2, layerMask, 0f, padding);
	}

	public static bool LineOfSight(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int layerMask, float padding = 0f)
	{
		if (!GamePhysics.LineOfSightInternal(p0, p1, layerMask, padding, 0f) || !GamePhysics.LineOfSightInternal(p1, p2, layerMask, 0f, 0f))
		{
			return false;
		}
		return GamePhysics.LineOfSightInternal(p2, p3, layerMask, 0f, padding);
	}

	public static bool LineOfSight(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, int layerMask, float padding = 0f)
	{
		if (!GamePhysics.LineOfSightInternal(p0, p1, layerMask, padding, 0f) || !GamePhysics.LineOfSightInternal(p1, p2, layerMask, 0f, 0f) || !GamePhysics.LineOfSightInternal(p2, p3, layerMask, 0f, 0f))
		{
			return false;
		}
		return GamePhysics.LineOfSightInternal(p3, p4, layerMask, 0f, padding);
	}

	private static bool LineOfSightInternal(Vector3 p0, Vector3 p1, int layerMask, float padding0, float padding1)
	{
		RaycastHit raycastHit;
		Vector3 vector3 = p1 - p0;
		float single = vector3.magnitude;
		if (single <= padding0 + padding1)
		{
			return true;
		}
		Vector3 vector31 = vector3 / single;
		Vector3 vector32 = vector31 * padding0;
		Vector3 vector33 = vector31 * padding1;
		if (!UnityEngine.Physics.Linecast(p0 + vector32, p1 - vector33, out raycastHit, layerMask, QueryTriggerInteraction.Ignore))
		{
			if (ConVar.Vis.lineofsight)
			{
				ConsoleNetwork.BroadcastToAllClients("ddraw.line", new object[] { 60f, Color.green, p0, p1 });
			}
			return true;
		}
		if (ConVar.Vis.lineofsight)
		{
			ConsoleNetwork.BroadcastToAllClients("ddraw.line", new object[] { 60f, Color.red, p0, p1 });
			ConsoleNetwork.BroadcastToAllClients("ddraw.text", new object[] { 60f, Color.white, raycastHit.point, raycastHit.collider.name });
		}
		return false;
	}

	public static void OverlapBounds(Bounds bounds, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	{
		layerMask = GamePhysics.HandleTerrainCollision(bounds.center, layerMask);
		GamePhysics.BufferToList(UnityEngine.Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, GamePhysics.colBuffer, Quaternion.identity, layerMask, triggerInteraction), list);
	}

	public static void OverlapBounds<T>(Bounds bounds, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	where T : Component
	{
		layerMask = GamePhysics.HandleTerrainCollision(bounds.center, layerMask);
		GamePhysics.BufferToList<T>(UnityEngine.Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, GamePhysics.colBuffer, Quaternion.identity, layerMask, triggerInteraction), list);
	}

	public static void OverlapCapsule(Vector3 point0, Vector3 point1, float radius, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	{
		layerMask = GamePhysics.HandleTerrainCollision(point0, layerMask);
		layerMask = GamePhysics.HandleTerrainCollision(point1, layerMask);
		GamePhysics.BufferToList(UnityEngine.Physics.OverlapCapsuleNonAlloc(point0, point1, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	public static void OverlapCapsule<T>(Vector3 point0, Vector3 point1, float radius, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	where T : Component
	{
		layerMask = GamePhysics.HandleTerrainCollision(point0, layerMask);
		layerMask = GamePhysics.HandleTerrainCollision(point1, layerMask);
		GamePhysics.BufferToList<T>(UnityEngine.Physics.OverlapCapsuleNonAlloc(point0, point1, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	public static void OverlapOBB(OBB obb, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	{
		layerMask = GamePhysics.HandleTerrainCollision(obb.position, layerMask);
		GamePhysics.BufferToList(UnityEngine.Physics.OverlapBoxNonAlloc(obb.position, obb.extents, GamePhysics.colBuffer, obb.rotation, layerMask, triggerInteraction), list);
	}

	public static void OverlapOBB<T>(OBB obb, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	where T : Component
	{
		layerMask = GamePhysics.HandleTerrainCollision(obb.position, layerMask);
		GamePhysics.BufferToList<T>(UnityEngine.Physics.OverlapBoxNonAlloc(obb.position, obb.extents, GamePhysics.colBuffer, obb.rotation, layerMask, triggerInteraction), list);
	}

	public static void OverlapSphere(Vector3 position, float radius, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	{
		layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
		GamePhysics.BufferToList(UnityEngine.Physics.OverlapSphereNonAlloc(position, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	public static void OverlapSphere<T>(Vector3 position, float radius, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	where T : Component
	{
		layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
		GamePhysics.BufferToList<T>(UnityEngine.Physics.OverlapSphereNonAlloc(position, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	public static void Sort(List<RaycastHit> hits)
	{
		hits.Sort((RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance));
	}

	public static void Sort(RaycastHit[] hits)
	{
		Array.Sort<RaycastHit>(hits, (RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance));
	}

	public static bool Trace(Ray ray, float radius, out RaycastHit hitInfo, float maxDistance = Single.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
		GamePhysics.TraceAllUnordered(ray, radius, list, maxDistance, layerMask, triggerInteraction);
		if (list.Count == 0)
		{
			hitInfo = new RaycastHit();
			Facepunch.Pool.FreeList<RaycastHit>(ref list);
			return false;
		}
		GamePhysics.Sort(list);
		hitInfo = list[0];
		Facepunch.Pool.FreeList<RaycastHit>(ref list);
		return true;
	}

	public static void TraceAll(Ray ray, float radius, List<RaycastHit> hits, float maxDistance = Single.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		GamePhysics.TraceAllUnordered(ray, radius, hits, maxDistance, layerMask, triggerInteraction);
		GamePhysics.Sort(hits);
	}

	public static void TraceAllUnordered(Ray ray, float radius, List<RaycastHit> hits, float maxDistance = Single.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		int num = 0;
		num = (radius != 0f ? UnityEngine.Physics.SphereCastNonAlloc(ray, radius, GamePhysics.hitBuffer, maxDistance, layerMask, triggerInteraction) : UnityEngine.Physics.RaycastNonAlloc(ray, GamePhysics.hitBuffer, maxDistance, layerMask, triggerInteraction));
		if (num == 0)
		{
			return;
		}
		if (num >= (int)GamePhysics.hitBuffer.Length)
		{
			Debug.LogWarning("Physics query is exceeding hit buffer length.");
		}
		for (int i = 0; i < num; i++)
		{
			RaycastHit raycastHit = GamePhysics.hitBuffer[i];
			if (GamePhysics.Verify(raycastHit))
			{
				hits.Add(raycastHit);
			}
		}
	}

	public static bool Verify(RaycastHit hitInfo)
	{
		return GamePhysics.Verify(hitInfo.collider, hitInfo.point);
	}

	public static bool Verify(Collider collider, Vector3 point)
	{
		if (collider is TerrainCollider && TerrainMeta.Collision && TerrainMeta.Collision.GetIgnore(point, 0.01f))
		{
			return false;
		}
		return collider.enabled;
	}
}