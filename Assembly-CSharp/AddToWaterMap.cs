using System;
using UnityEngine;

public class AddToWaterMap : ProceduralObject
{
	public AddToWaterMap()
	{
	}

	public override void Process()
	{
		RaycastHit raycastHit;
		Collider component = base.GetComponent<Collider>();
		Bounds bound = component.bounds;
		int num = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeX(bound.min.x));
		int num1 = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeZ(bound.max.x));
		int num2 = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeX(bound.min.z));
		int num3 = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeZ(bound.max.z));
		if (!(component is BoxCollider) || !(base.transform.rotation == Quaternion.identity))
		{
			for (int i = num2; i <= num3; i++)
			{
				float single = TerrainMeta.WaterMap.Coordinate(i);
				for (int j = num; j <= num1; j++)
				{
					float single1 = TerrainMeta.WaterMap.Coordinate(j);
					Vector3 vector3 = new Vector3(TerrainMeta.DenormalizeX(single1), bound.max.y + 1f, TerrainMeta.DenormalizeZ(single));
					Ray ray = new Ray(vector3, Vector3.down);
					if (component.Raycast(ray, out raycastHit, bound.size.y + 1f + 1f))
					{
						float single2 = TerrainMeta.NormalizeY(raycastHit.point.y);
						if (single2 > TerrainMeta.WaterMap.GetHeight01(j, i))
						{
							TerrainMeta.WaterMap.SetHeight(j, i, single2);
						}
					}
				}
			}
		}
		else
		{
			float single3 = TerrainMeta.NormalizeY(bound.max.y);
			for (int k = num2; k <= num3; k++)
			{
				for (int l = num; l <= num1; l++)
				{
					if (single3 > TerrainMeta.WaterMap.GetHeight01(l, k))
					{
						TerrainMeta.WaterMap.SetHeight(l, k, single3);
					}
				}
			}
		}
		GameManager.Destroy(this, 0f);
	}
}