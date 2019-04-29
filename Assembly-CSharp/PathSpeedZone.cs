using System;
using UnityEngine;

public class PathSpeedZone : MonoBehaviour
{
	public Bounds bounds;

	public OBB obbBounds;

	public float maxVelocityPerSec = 5f;

	public PathSpeedZone()
	{
	}

	public float GetMaxSpeed()
	{
		return this.maxVelocityPerSec;
	}

	public virtual void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
		Gizmos.DrawCube(this.bounds.center, this.bounds.size);
		Gizmos.color = new Color(1f, 0.7f, 0f, 1f);
		Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
	}

	public OBB WorldSpaceBounds()
	{
		return new OBB(base.transform.position, base.transform.lossyScale, base.transform.rotation, this.bounds);
	}
}