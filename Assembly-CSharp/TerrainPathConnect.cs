using System;
using UnityEngine;

public class TerrainPathConnect : MonoBehaviour
{
	public InfrastructureType Type;

	public TerrainPathConnect()
	{
	}

	public PathFinder.Point GetPoint(int res)
	{
		Vector3 vector3 = base.transform.position;
		float single = TerrainMeta.NormalizeX(vector3.x);
		float single1 = TerrainMeta.NormalizeZ(vector3.z);
		PathFinder.Point point = new PathFinder.Point()
		{
			x = Mathf.Clamp((int)(single * (float)res), 0, res - 1),
			y = Mathf.Clamp((int)(single1 * (float)res), 0, res - 1)
		};
		return point;
	}
}