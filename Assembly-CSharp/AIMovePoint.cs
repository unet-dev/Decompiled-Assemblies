using System;
using UnityEngine;

public class AIMovePoint : MonoBehaviour
{
	public float radius = 1f;

	public float nextAvailableRoamTime;

	public float nextAvailableEngagementTime;

	public BaseEntity lastUser;

	public AIMovePoint()
	{
	}

	public bool CanBeUsedBy(BaseEntity user)
	{
		if (user != null && this.lastUser == user)
		{
			return true;
		}
		return this.IsUsed();
	}

	public bool IsUsed()
	{
		if (this.IsUsedForRoaming())
		{
			return true;
		}
		return this.IsUsedForEngagement();
	}

	public bool IsUsedForEngagement()
	{
		return Time.time < this.nextAvailableEngagementTime;
	}

	public bool IsUsedForRoaming()
	{
		return Time.time < this.nextAvailableRoamTime;
	}

	public void MarkUsedForEngagement(float dur = 5f, BaseEntity user = null)
	{
		this.nextAvailableEngagementTime = Time.time + dur;
		this.lastUser = user;
	}

	public void MarkUsedForRoam(float dur = 10f, BaseEntity user = null)
	{
		this.nextAvailableRoamTime = Time.time + dur;
		this.lastUser = user;
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		GizmosUtil.DrawWireCircleY(base.transform.position, this.radius);
	}
}