using System;
using UnityEngine;

[Serializable]
public class SpawnFilter
{
	[InspectorFlags]
	public TerrainSplat.Enum SplatType = TerrainSplat.Enum.Dirt | TerrainSplat.Enum.Snow | TerrainSplat.Enum.Sand | TerrainSplat.Enum.Rock | TerrainSplat.Enum.Grass | TerrainSplat.Enum.Forest | TerrainSplat.Enum.Stones | TerrainSplat.Enum.Gravel;

	[InspectorFlags]
	public TerrainBiome.Enum BiomeType = TerrainBiome.Enum.Arid | TerrainBiome.Enum.Temperate | TerrainBiome.Enum.Tundra | TerrainBiome.Enum.Arctic;

	[InspectorFlags]
	public TerrainTopology.Enum TopologyAny = TerrainTopology.Enum.Field | TerrainTopology.Enum.Cliff | TerrainTopology.Enum.Summit | TerrainTopology.Enum.Beachside | TerrainTopology.Enum.Beach | TerrainTopology.Enum.Forest | TerrainTopology.Enum.Forestside | TerrainTopology.Enum.Ocean | TerrainTopology.Enum.Oceanside | TerrainTopology.Enum.Decor | TerrainTopology.Enum.Monument | TerrainTopology.Enum.Road | TerrainTopology.Enum.Roadside | TerrainTopology.Enum.Swamp | TerrainTopology.Enum.River | TerrainTopology.Enum.Riverside | TerrainTopology.Enum.Lake | TerrainTopology.Enum.Lakeside | TerrainTopology.Enum.Offshore | TerrainTopology.Enum.Powerline | TerrainTopology.Enum.Runway | TerrainTopology.Enum.Building | TerrainTopology.Enum.Cliffside | TerrainTopology.Enum.Mountain | TerrainTopology.Enum.Clutter | TerrainTopology.Enum.Alt | TerrainTopology.Enum.Tier0 | TerrainTopology.Enum.Tier1 | TerrainTopology.Enum.Tier2 | TerrainTopology.Enum.Mainland | TerrainTopology.Enum.Hilltop;

	[InspectorFlags]
	public TerrainTopology.Enum TopologyAll;

	[InspectorFlags]
	public TerrainTopology.Enum TopologyNot;

	public SpawnFilter()
	{
	}

	public float GetFactor(Vector3 worldPos)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		return this.GetFactor(single, TerrainMeta.NormalizeZ(worldPos.z));
	}

	public float GetFactor(float normX, float normZ)
	{
		if (TerrainMeta.TopologyMap == null)
		{
			return 0f;
		}
		int splatType = (int)this.SplatType;
		int biomeType = (int)this.BiomeType;
		int topologyAny = (int)this.TopologyAny;
		int topologyAll = (int)this.TopologyAll;
		int topologyNot = (int)this.TopologyNot;
		if (topologyAny == 0)
		{
			Debug.LogError("Empty topology filter is invalid.");
		}
		else if (topologyAny != -1 || topologyAll != 0 || topologyNot != 0)
		{
			int topology = TerrainMeta.TopologyMap.GetTopology(normX, normZ);
			if (topologyAny != -1 && (topology & topologyAny) == 0)
			{
				return 0f;
			}
			if (topologyNot != 0 && (topology & topologyNot) != 0)
			{
				return 0f;
			}
			if (topologyAll != 0 && (topology & topologyAll) != topologyAll)
			{
				return 0f;
			}
		}
		if (biomeType == 0)
		{
			Debug.LogError("Empty biome filter is invalid.");
		}
		else if (biomeType != -1 && (TerrainMeta.BiomeMap.GetBiomeMaxType(normX, normZ, -1) & biomeType) == 0)
		{
			return 0f;
		}
		if (splatType == 0)
		{
			Debug.LogError("Empty splat filter is invalid.");
		}
		else if (splatType != -1)
		{
			return TerrainMeta.SplatMap.GetSplat(normX, normZ, splatType);
		}
		return 1f;
	}

	public bool Test(Vector3 worldPos)
	{
		return this.GetFactor(worldPos) > 0.5f;
	}

	public bool Test(float normX, float normZ)
	{
		return this.GetFactor(normX, normZ) > 0.5f;
	}
}