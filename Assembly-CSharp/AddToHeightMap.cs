using System;
using UnityEngine;

public class AddToHeightMap : ProceduralObject
{
	public AddToHeightMap()
	{
	}

	public override void Process()
	{
		RaycastHit raycastHit;
		Collider component = base.GetComponent<Collider>();
		Bounds bound = component.bounds;
		int num = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(bound.min.x));
		int num1 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(bound.max.x));
		int num2 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(bound.min.z));
		int num3 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(bound.max.z));
		for (int i = num2; i <= num3; i++)
		{
			float single = TerrainMeta.HeightMap.Coordinate(i);
			for (int j = num; j <= num1; j++)
			{
				float single1 = TerrainMeta.HeightMap.Coordinate(j);
				Vector3 vector3 = new Vector3(TerrainMeta.DenormalizeX(single1), bound.max.y, TerrainMeta.DenormalizeZ(single));
				Ray ray = new Ray(vector3, Vector3.down);
				if (component.Raycast(ray, out raycastHit, bound.size.y))
				{
					float single2 = TerrainMeta.NormalizeY(raycastHit.point.y);
					if (single2 > TerrainMeta.HeightMap.GetHeight01(j, i))
					{
						TerrainMeta.HeightMap.SetHeight(j, i, single2);
					}
				}
			}
		}
		GameManager.Destroy(this, 0f);
	}
}