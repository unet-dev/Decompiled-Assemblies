using System;
using UnityEngine;

public static class WaterLevel
{
	public static float Factor(Bounds bounds)
	{
		float single;
		using (TimeWarning timeWarning = TimeWarning.New("WaterLevel.Factor", 0.1f))
		{
			if (bounds.size == Vector3.zero)
			{
				bounds.size = new Vector3(0.1f, 0.1f, 0.1f);
			}
			WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(bounds);
			single = (waterInfo.isValid ? Mathf.InverseLerp(bounds.min.y, bounds.max.y, waterInfo.surfaceLevel) : 0f);
		}
		return single;
	}

	public static WaterLevel.WaterInfo GetBuoyancyWaterInfo(Vector3 pos, Vector2 posUV, float terrainHeight, float waterHeight)
	{
		WaterLevel.WaterInfo waterInfo;
		using (TimeWarning timeWarning = TimeWarning.New("WaterLevel.GetWaterInfo", 0.1f))
		{
			WaterLevel.WaterInfo waterInfo1 = new WaterLevel.WaterInfo();
			if (pos.y <= waterHeight)
			{
				bool flag = pos.y < terrainHeight - 1f;
				if (flag)
				{
					waterHeight = 0f;
					if (pos.y > waterHeight)
					{
						waterInfo = waterInfo1;
						return waterInfo;
					}
				}
				int num = (TerrainMeta.TopologyMap ? TerrainMeta.TopologyMap.GetTopologyFast(posUV) : 0);
				if ((flag || (num & 246144) == 0) && WaterSystem.Collision && WaterSystem.Collision.GetIgnore(pos, 0.01f))
				{
					waterInfo = waterInfo1;
				}
				else
				{
					waterInfo1.isValid = true;
					waterInfo1.currentDepth = Mathf.Max(0f, waterHeight - pos.y);
					waterInfo1.overallDepth = Mathf.Max(0f, waterHeight - terrainHeight);
					waterInfo1.surfaceLevel = waterHeight;
					waterInfo = waterInfo1;
				}
			}
			else
			{
				waterInfo = waterInfo1;
			}
		}
		return waterInfo;
	}

	public static float GetOverallWaterDepth(Vector3 pos)
	{
		float waterInfo;
		using (TimeWarning timeWarning = TimeWarning.New("WaterLevel.GetOverallWaterDepth", 0.1f))
		{
			waterInfo = WaterLevel.GetWaterInfo(pos).overallDepth;
		}
		return waterInfo;
	}

	public static float GetWaterDepth(Vector3 pos)
	{
		float waterInfo;
		using (TimeWarning timeWarning = TimeWarning.New("WaterLevel.GetWaterDepth", 0.1f))
		{
			waterInfo = WaterLevel.GetWaterInfo(pos).currentDepth;
		}
		return waterInfo;
	}

	public static WaterLevel.WaterInfo GetWaterInfo(Vector3 pos)
	{
		WaterLevel.WaterInfo waterInfo;
		using (TimeWarning timeWarning = TimeWarning.New("WaterLevel.GetWaterInfo", 0.1f))
		{
			WaterLevel.WaterInfo waterInfo1 = new WaterLevel.WaterInfo();
			float single = (TerrainMeta.WaterMap ? TerrainMeta.WaterMap.GetHeight(pos) : 0f);
			if (pos.y <= single)
			{
				float single1 = (TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetHeight(pos) : 0f);
				if (pos.y < single1 - 1f)
				{
					single = 0f;
					if (pos.y > single)
					{
						waterInfo = waterInfo1;
						return waterInfo;
					}
				}
				if (!WaterSystem.Collision || !WaterSystem.Collision.GetIgnore(pos, 0.01f))
				{
					waterInfo1.isValid = true;
					waterInfo1.currentDepth = Mathf.Max(0f, single - pos.y);
					waterInfo1.overallDepth = Mathf.Max(0f, single - single1);
					waterInfo1.surfaceLevel = single;
					waterInfo = waterInfo1;
				}
				else
				{
					waterInfo = waterInfo1;
				}
			}
			else
			{
				waterInfo = waterInfo1;
			}
		}
		return waterInfo;
	}

	public static WaterLevel.WaterInfo GetWaterInfo(Bounds bounds)
	{
		WaterLevel.WaterInfo waterInfo;
		using (TimeWarning timeWarning = TimeWarning.New("WaterLevel.GetWaterInfo", 0.1f))
		{
			WaterLevel.WaterInfo waterInfo1 = new WaterLevel.WaterInfo();
			float single = (TerrainMeta.WaterMap ? TerrainMeta.WaterMap.GetHeight(bounds.center) : 0f);
			if (bounds.min.y <= single)
			{
				float single1 = (TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetHeight(bounds.center) : 0f);
				if (bounds.max.y < single1 - 1f)
				{
					single = 0f;
					if (bounds.min.y > single)
					{
						waterInfo = waterInfo1;
						return waterInfo;
					}
				}
				if (!WaterSystem.Collision || !WaterSystem.Collision.GetIgnore(bounds))
				{
					waterInfo1.isValid = true;
					waterInfo1.currentDepth = Mathf.Max(0f, single - bounds.min.y);
					waterInfo1.overallDepth = Mathf.Max(0f, single - single1);
					waterInfo1.surfaceLevel = single;
					waterInfo = waterInfo1;
				}
				else
				{
					waterInfo = waterInfo1;
				}
			}
			else
			{
				waterInfo = waterInfo1;
			}
		}
		return waterInfo;
	}

	public static bool Test(Vector3 pos)
	{
		bool waterInfo;
		using (TimeWarning timeWarning = TimeWarning.New("WaterLevel.Test", 0.1f))
		{
			waterInfo = WaterLevel.GetWaterInfo(pos).isValid;
		}
		return waterInfo;
	}

	public struct WaterInfo
	{
		public bool isValid;

		public float currentDepth;

		public float overallDepth;

		public float surfaceLevel;
	}
}