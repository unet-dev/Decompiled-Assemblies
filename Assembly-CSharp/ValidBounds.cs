using System;
using UnityEngine;

public class ValidBounds : SingletonComponent<ValidBounds>
{
	public Bounds worldBounds;

	public ValidBounds()
	{
	}

	internal bool IsInside(Vector3 vPos)
	{
		if (vPos.IsNaNOrInfinity())
		{
			return false;
		}
		if (!this.worldBounds.Contains(vPos))
		{
			return false;
		}
		if (TerrainMeta.Terrain != null)
		{
			if (vPos.y < TerrainMeta.Position.y)
			{
				return false;
			}
			if (TerrainMeta.OutOfMargin(vPos))
			{
				return false;
			}
		}
		return true;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(this.worldBounds.center, this.worldBounds.size);
	}

	public static bool Test(Vector3 vPos)
	{
		if (!SingletonComponent<ValidBounds>.Instance)
		{
			return true;
		}
		return SingletonComponent<ValidBounds>.Instance.IsInside(vPos);
	}
}