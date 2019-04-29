using System;
using UnityEngine;

public class DrawSkeleton : MonoBehaviour
{
	public DrawSkeleton()
	{
	}

	private static void DrawTransform(Transform t)
	{
		for (int i = 0; i < t.childCount; i++)
		{
			Gizmos.DrawLine(t.position, t.GetChild(i).position);
			DrawSkeleton.DrawTransform(t.GetChild(i));
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		DrawSkeleton.DrawTransform(base.transform);
	}
}