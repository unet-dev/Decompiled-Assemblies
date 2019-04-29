using System;
using UnityEngine;

public class AITraversalArea : TriggerBase
{
	public Transform entryPoint1;

	public Transform entryPoint2;

	public AITraversalWaitPoint[] waitPoints;

	public Bounds movementArea;

	public Transform activeEntryPoint;

	public float nextFreeTime;

	public AITraversalArea()
	{
	}

	public bool CanTraverse(BaseEntity ent)
	{
		return Time.time > this.nextFreeTime;
	}

	public bool CanUse(Vector3 dirFrom)
	{
		return Time.time > this.nextFreeTime;
	}

	public bool EntityFilter(BaseEntity ent)
	{
		if (!ent.IsNpc)
		{
			return false;
		}
		return ent.isServer;
	}

	public Transform GetClosestEntry(Vector3 position)
	{
		if (Vector3.Distance(position, this.entryPoint1.position) < Vector3.Distance(position, this.entryPoint2.position))
		{
			return this.entryPoint1;
		}
		return this.entryPoint2;
	}

	public AITraversalWaitPoint GetEntryPointNear(Vector3 pos)
	{
		Vector3 closestEntry = this.GetClosestEntry(pos).position;
		Vector3 farthestEntry = this.GetFarthestEntry(pos).position;
		BaseEntity[] baseEntityArray = new BaseEntity[1];
		AITraversalWaitPoint aITraversalWaitPoint = null;
		float single = 0f;
		AITraversalWaitPoint[] aITraversalWaitPointArray = this.waitPoints;
		for (int i = 0; i < (int)aITraversalWaitPointArray.Length; i++)
		{
			AITraversalWaitPoint aITraversalWaitPoint1 = aITraversalWaitPointArray[i];
			if (!aITraversalWaitPoint1.Occupied())
			{
				Vector3 vector3 = aITraversalWaitPoint1.transform.position;
				float single1 = Vector3.Distance(closestEntry, vector3);
				if (Vector3.Distance(farthestEntry, vector3) >= single1)
				{
					float single2 = Vector3.Distance(vector3, pos);
					float single3 = (1f - Mathf.InverseLerp(0f, 20f, single2)) * 100f;
					if (single3 > single)
					{
						single = single3;
						aITraversalWaitPoint = aITraversalWaitPoint1;
					}
				}
			}
		}
		return aITraversalWaitPoint;
	}

	public Transform GetFarthestEntry(Vector3 position)
	{
		if (Vector3.Distance(position, this.entryPoint1.position) > Vector3.Distance(position, this.entryPoint2.position))
		{
			return this.entryPoint1;
		}
		return this.entryPoint2;
	}

	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		if (!baseEntity.IsNpc)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawCube(this.entryPoint1.position + (Vector3.up * 0.125f), new Vector3(0.5f, 0.25f, 0.5f));
		Gizmos.DrawCube(this.entryPoint2.position + (Vector3.up * 0.125f), new Vector3(0.5f, 0.25f, 0.5f));
		Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.5f);
		Gizmos.DrawCube(this.movementArea.center, this.movementArea.size);
		Gizmos.color = Color.magenta;
		AITraversalWaitPoint[] aITraversalWaitPointArray = this.waitPoints;
		for (int i = 0; i < (int)aITraversalWaitPointArray.Length; i++)
		{
			GizmosUtil.DrawCircleY(aITraversalWaitPointArray[i].transform.position, 0.5f);
		}
	}

	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		ent.GetComponent<HumanNPC>();
		Debug.Log("Enter Traversal Area");
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		ent.GetComponent<HumanNPC>();
		Debug.Log("Leave Traversal Area");
	}

	public void OnValidate()
	{
		this.movementArea.center = base.transform.position;
	}

	public void SetBusyFor(float dur = 1f)
	{
		this.nextFreeTime = Time.time + dur;
	}
}