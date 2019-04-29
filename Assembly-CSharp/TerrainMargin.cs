using System;
using UnityEngine;

public class TerrainMargin
{
	public TerrainMargin()
	{
	}

	public static void Create()
	{
		Material terrain = TerrainMeta.Terrain.materialTemplate;
		Vector3 center = TerrainMeta.Center;
		Vector3 size = TerrainMeta.Size;
		Vector3 vector3 = new Vector3(size.x, 0f, 0f);
		Vector3 vector31 = new Vector3(0f, 0f, size.z);
		center.y = TerrainMeta.HeightMap.GetHeight(0, 0);
		TerrainMargin.Create(center - vector31, size, terrain);
		TerrainMargin.Create((center - vector31) - vector3, size, terrain);
		TerrainMargin.Create((center - vector31) + vector3, size, terrain);
		TerrainMargin.Create(center - vector3, size, terrain);
		TerrainMargin.Create(center + vector3, size, terrain);
		TerrainMargin.Create(center + vector31, size, terrain);
		TerrainMargin.Create((center + vector31) - vector3, size, terrain);
		TerrainMargin.Create((center + vector31) + vector3, size, terrain);
	}

	private static void Create(Vector3 position, Vector3 size, Material material)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
		gameObject.name = "TerrainMargin";
		gameObject.layer = 16;
		gameObject.transform.position = position;
		gameObject.transform.localScale = size * 0.1f;
		UnityEngine.Object.Destroy(gameObject.GetComponent<MeshRenderer>());
		UnityEngine.Object.Destroy(gameObject.GetComponent<MeshFilter>());
	}
}