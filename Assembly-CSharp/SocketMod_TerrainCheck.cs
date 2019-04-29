using System;
using UnityEngine;

public class SocketMod_TerrainCheck : SocketMod
{
	public bool wantsInTerrain = true;

	public SocketMod_TerrainCheck()
	{
	}

	public override bool DoCheck(Construction.Placement place)
	{
		if (SocketMod_TerrainCheck.IsInTerrain(place.position + (place.rotation * this.worldPosition)) == this.wantsInTerrain)
		{
			return true;
		}
		Construction.lastPlacementError = string.Concat(this.fullName, ": not in terrain");
		return false;
	}

	public static bool IsInTerrain(Vector3 vPoint)
	{
		if (TerrainMeta.OutOfBounds(vPoint))
		{
			return false;
		}
		if (!TerrainMeta.Collision || !TerrainMeta.Collision.GetIgnore(vPoint, 0.01f))
		{
			Terrain[] terrainArray = Terrain.activeTerrains;
			for (int i = 0; i < (int)terrainArray.Length; i++)
			{
				Terrain terrain = terrainArray[i];
				if (terrain.SampleHeight(vPoint) + terrain.transform.position.y > vPoint.y)
				{
					return true;
				}
			}
		}
		if (Physics.Raycast(new Ray(vPoint + (Vector3.up * 3f), Vector3.down), 3f, 65536))
		{
			return true;
		}
		return false;
	}

	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		bool flag = SocketMod_TerrainCheck.IsInTerrain(base.transform.position);
		if (!this.wantsInTerrain)
		{
			flag = !flag;
		}
		Gizmos.color = (flag ? Color.green : Color.red);
		Gizmos.DrawSphere(Vector3.zero, 0.1f);
	}
}