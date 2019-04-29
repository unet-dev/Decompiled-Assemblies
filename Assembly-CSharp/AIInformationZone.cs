using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIInformationZone : MonoBehaviour
{
	public static List<AIInformationZone> zones;

	public List<AICoverPoint> coverPoints = new List<AICoverPoint>();

	public List<AIMovePoint> movePoints = new List<AIMovePoint>();

	public List<NavMeshLink> navMeshLinks = new List<NavMeshLink>();

	public Bounds bounds;

	private OBB areaBox;

	static AIInformationZone()
	{
		AIInformationZone.zones = new List<AIInformationZone>();
	}

	public AIInformationZone()
	{
	}

	public AICoverPoint GetBestCoverPoint(Vector3 currentPosition, Vector3 hideFromPosition, float minRange = 0f, float maxRange = 20f, BaseEntity forObject = null)
	{
		AICoverPoint aICoverPoint = null;
		float single = 0f;
		foreach (AICoverPoint coverPoint in this.coverPoints)
		{
			if (coverPoint.InUse() && !coverPoint.IsUsedBy(forObject))
			{
				continue;
			}
			Vector3 vector3 = coverPoint.transform.position;
			Vector3 vector31 = hideFromPosition - vector3;
			Vector3 vector32 = vector31.normalized;
			float single1 = Vector3.Dot(coverPoint.transform.forward, vector32);
			if (single1 < 1f - coverPoint.coverDot)
			{
				continue;
			}
			float single2 = Vector3.Distance(currentPosition, vector3);
			float single3 = 0f;
			if (minRange > 0f)
			{
				single3 = single3 - (1f - Mathf.InverseLerp(0f, minRange, single2)) * 100f;
			}
			float single4 = Mathf.Abs(vector3.y - currentPosition.y);
			single3 = single3 + (1f - Mathf.InverseLerp(1f, 5f, single4)) * 500f;
			single3 = single3 + Mathf.InverseLerp(1f - coverPoint.coverDot, 1f, single1) * 50f;
			single3 = single3 + (1f - Mathf.InverseLerp(2f, maxRange, single2)) * 100f;
			float single5 = 1f - Mathf.InverseLerp(4f, 10f, Vector3.Distance(currentPosition, hideFromPosition));
			vector31 = coverPoint.transform.position - currentPosition;
			float single6 = Vector3.Dot(vector31.normalized, vector32);
			single3 = single3 - Mathf.InverseLerp(-1f, 0.25f, single6) * 50f * single5;
			if (single3 <= single)
			{
				continue;
			}
			aICoverPoint = coverPoint;
			single = single3;
		}
		if (aICoverPoint)
		{
			return aICoverPoint;
		}
		return null;
	}

	public AIMovePoint GetBestMovePointNear(Vector3 targetPosition, Vector3 fromPosition, float minRange, float maxRange, bool checkLOS = false, BaseEntity forObject = null)
	{
		AIMovePoint aIMovePoint = null;
		float single = -1f;
		foreach (AIMovePoint movePoint in this.movePoints)
		{
			float single1 = 0f;
			Vector3 vector3 = movePoint.transform.position;
			float single2 = Vector3.Distance(targetPosition, vector3);
			if (single2 > maxRange || !movePoint.transform.parent.gameObject.activeSelf)
			{
				continue;
			}
			single1 = single1 + (movePoint.CanBeUsedBy(forObject) ? 100f : 0f);
			single1 = single1 + (1f - Mathf.InverseLerp(minRange, maxRange, single2)) * 100f;
			if (single1 < single || checkLOS && Physics.Linecast(targetPosition + (Vector3.up * 1f), vector3 + (Vector3.up * 1f), 1218519297, QueryTriggerInteraction.Ignore) || single1 <= single)
			{
				continue;
			}
			aIMovePoint = movePoint;
			single = single1;
		}
		return aIMovePoint;
	}

	public Vector3 GetBestPositionNear(Vector3 targetPosition, Vector3 fromPosition, float minRange, float maxRange, bool checkLOS = false)
	{
		AIMovePoint aIMovePoint = null;
		float single = -1f;
		foreach (AIMovePoint movePoint in this.movePoints)
		{
			float single1 = 0f;
			Vector3 vector3 = movePoint.transform.position;
			float single2 = Vector3.Distance(targetPosition, vector3);
			if (single2 > maxRange || !movePoint.transform.parent.gameObject.activeSelf)
			{
				continue;
			}
			single1 = single1 + (1f - Mathf.InverseLerp(minRange, maxRange, single2)) * 100f;
			if (checkLOS && Physics.Linecast(targetPosition + (Vector3.up * 1f), vector3 + (Vector3.up * 1f), 1218650369, QueryTriggerInteraction.Ignore) || single1 <= single)
			{
				continue;
			}
			aIMovePoint = movePoint;
			single = single1;
		}
		if (aIMovePoint == null)
		{
			return targetPosition;
		}
		return aIMovePoint.transform.position;
	}

	public NavMeshLink GetClosestNavMeshLink(Vector3 pos)
	{
		NavMeshLink navMeshLink = null;
		float single = Single.PositiveInfinity;
		foreach (NavMeshLink navMeshLink1 in this.navMeshLinks)
		{
			float single1 = Vector3.Distance(navMeshLink1.gameObject.transform.position, pos);
			if (single1 >= single)
			{
				continue;
			}
			navMeshLink = navMeshLink1;
			single = single1;
			if (single1 >= 0.25f)
			{
				continue;
			}
			return navMeshLink;
		}
		return navMeshLink;
	}

	public static AIInformationZone GetForPoint(Vector3 point, BaseEntity from = null)
	{
		AIInformationZone aIInformationZone;
		if (AIInformationZone.zones == null || AIInformationZone.zones.Count == 0)
		{
			return null;
		}
		List<AIInformationZone>.Enumerator enumerator = AIInformationZone.zones.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				AIInformationZone current = enumerator.Current;
				if (!current.areaBox.Contains(point))
				{
					continue;
				}
				aIInformationZone = current;
				return aIInformationZone;
			}
			return AIInformationZone.zones[0];
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return aIInformationZone;
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		Gizmos.DrawCube(base.transform.position + this.bounds.center, this.bounds.size);
	}

	public void OnValidate()
	{
	}

	public void Process()
	{
		AICoverPoint[] componentsInChildren = base.transform.GetComponentsInChildren<AICoverPoint>();
		this.coverPoints.AddRange(componentsInChildren);
		AIMovePoint[] aIMovePointArray = base.transform.GetComponentsInChildren<AIMovePoint>(true);
		this.movePoints.AddRange(aIMovePointArray);
		NavMeshLink[] navMeshLinkArray = base.transform.GetComponentsInChildren<NavMeshLink>(true);
		this.navMeshLinks.AddRange(navMeshLinkArray);
	}

	public void Start()
	{
		this.Process();
		this.areaBox = new OBB(base.transform.position, base.transform.lossyScale, base.transform.rotation, this.bounds);
		AIInformationZone.zones.Add(this);
	}
}