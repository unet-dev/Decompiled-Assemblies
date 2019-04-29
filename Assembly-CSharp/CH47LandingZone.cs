using System;
using System.Collections.Generic;
using UnityEngine;

public class CH47LandingZone : MonoBehaviour
{
	public float lastDropTime;

	private static List<CH47LandingZone> landingZones;

	public float dropoffScale = 1f;

	static CH47LandingZone()
	{
		CH47LandingZone.landingZones = new List<CH47LandingZone>();
	}

	public CH47LandingZone()
	{
	}

	public void Awake()
	{
		if (!CH47LandingZone.landingZones.Contains(this))
		{
			CH47LandingZone.landingZones.Add(this);
		}
	}

	public static CH47LandingZone GetClosest(Vector3 pos)
	{
		float single = Single.PositiveInfinity;
		CH47LandingZone cH47LandingZone = null;
		foreach (CH47LandingZone landingZone in CH47LandingZone.landingZones)
		{
			float single1 = Vector3Ex.Distance2D(pos, landingZone.transform.position);
			if (single1 >= single)
			{
				continue;
			}
			single = single1;
			cH47LandingZone = landingZone;
		}
		return cH47LandingZone;
	}

	public void OnDestroy()
	{
		if (CH47LandingZone.landingZones.Contains(this))
		{
			CH47LandingZone.landingZones.Remove(this);
		}
	}

	public void OnDrawGizmos()
	{
		Color color = Color.magenta;
		color.a = 0.25f;
		Gizmos.color = color;
		GizmosUtil.DrawCircleY(base.transform.position, 6f);
		color.a = 1f;
		Gizmos.color = color;
		GizmosUtil.DrawWireCircleY(base.transform.position, 6f);
	}

	public float TimeSinceLastDrop()
	{
		return Time.time - this.lastDropTime;
	}

	public void Used()
	{
		this.lastDropTime = Time.time;
	}
}