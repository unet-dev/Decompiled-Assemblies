using System;
using UnityEngine;

[ExecuteInEditMode]
public class AdaptMeshToTerrain : MonoBehaviour
{
	public UnityEngine.LayerMask LayerMask = -1;

	public float RayHeight = 10f;

	public float RayMaxDistance = 20f;

	public float MinDisplacement = 0.01f;

	public float MaxDisplacement = 0.33f;

	[Range(8f, 64f)]
	public int PlaneResolution = 24;

	public AdaptMeshToTerrain()
	{
	}
}