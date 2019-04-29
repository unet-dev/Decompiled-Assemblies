using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class TransformUtil
{
	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hit, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out hit, 100f, -1, ignoreTransform);
	}

	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hit, float range, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out hit, range, -1, ignoreTransform);
	}

	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hitOut, float range, LayerMask mask, Transform ignoreTransform = null)
	{
		RaycastHit raycastHit;
		startPos.y += 0.25f;
		range += 0.25f;
		hitOut = new RaycastHit();
		if (!Physics.Raycast(new Ray(startPos, Vector3.down), out raycastHit, range, mask))
		{
			return false;
		}
		if (!(ignoreTransform != null) || !(raycastHit.collider.transform == ignoreTransform) && !raycastHit.collider.transform.IsChildOf(ignoreTransform))
		{
			hitOut = raycastHit;
			return true;
		}
		return TransformUtil.GetGroundInfo(startPos - new Vector3(0f, 0.01f, 0f), out hitOut, range, mask, ignoreTransform);
	}

	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out pos, out normal, 100f, -1, ignoreTransform);
	}

	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out pos, out normal, range, -1, ignoreTransform);
	}

	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, LayerMask mask, Transform ignoreTransform = null)
	{
		bool flag;
		startPos.y += 0.25f;
		range += 0.25f;
		using (IEnumerator<RaycastHit> enumerator = (
			from h in (IEnumerable<RaycastHit>)Physics.RaycastAll(new Ray(startPos, Vector3.down), range, mask)
			orderby h.distance
			select h).GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				RaycastHit current = enumerator.Current;
				if (ignoreTransform != null && (current.collider.transform == ignoreTransform || current.collider.transform.IsChildOf(ignoreTransform)))
				{
					continue;
				}
				pos = current.point;
				normal = current.normal;
				flag = true;
				return flag;
			}
			pos = startPos;
			normal = Vector3.up;
			return false;
		}
		return flag;
	}

	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal)
	{
		return TransformUtil.GetGroundInfoTerrainOnly(startPos, out pos, out normal, 100f, -1);
	}

	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range)
	{
		return TransformUtil.GetGroundInfoTerrainOnly(startPos, out pos, out normal, range, -1);
	}

	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, LayerMask mask)
	{
		RaycastHit raycastHit;
		startPos.y += 0.25f;
		range += 0.25f;
		if (!Physics.Raycast(new Ray(startPos, Vector3.down), out raycastHit, range, mask) || !(raycastHit.collider is TerrainCollider))
		{
			pos = startPos;
			normal = Vector3.up;
			return false;
		}
		pos = raycastHit.point;
		normal = raycastHit.normal;
		return true;
	}

	public static Transform[] GetRootObjects()
	{
		return (
			from x in (IEnumerable<Transform>)UnityEngine.Object.FindObjectsOfType<Transform>()
			where x.transform == x.transform.root
			select x).ToArray<Transform>();
	}
}