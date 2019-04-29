using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PathInterestNode : MonoBehaviour
{
	public float NextVisitTime
	{
		get;
		set;
	}

	public PathInterestNode()
	{
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
		Gizmos.DrawSphere(base.transform.position, 0.5f);
	}
}