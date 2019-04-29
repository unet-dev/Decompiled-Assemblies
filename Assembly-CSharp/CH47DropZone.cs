using System;
using System.Collections.Generic;
using UnityEngine;

public class CH47DropZone : MonoBehaviour
{
	public float lastDropTime;

	private static List<CH47DropZone> dropZones;

	static CH47DropZone()
	{
		CH47DropZone.dropZones = new List<CH47DropZone>();
	}

	public CH47DropZone()
	{
	}

	public void Awake()
	{
		if (!CH47DropZone.dropZones.Contains(this))
		{
			CH47DropZone.dropZones.Add(this);
		}
	}

	public static CH47DropZone GetClosest(Vector3 pos)
	{
		float single = Single.PositiveInfinity;
		CH47DropZone cH47DropZone = null;
		foreach (CH47DropZone dropZone in CH47DropZone.dropZones)
		{
			float single1 = Vector3Ex.Distance2D(pos, dropZone.transform.position);
			if (single1 >= single)
			{
				continue;
			}
			single = single1;
			cH47DropZone = dropZone;
		}
		return cH47DropZone;
	}

	public void OnDestroy()
	{
		if (CH47DropZone.dropZones.Contains(this))
		{
			CH47DropZone.dropZones.Remove(this);
		}
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(base.transform.position, 5f);
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