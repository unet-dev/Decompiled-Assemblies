using System;
using UnityEngine;

public abstract class TerrainPlacement : PrefabAttribute
{
	[ReadOnly]
	public Vector3 size = Vector3.zero;

	[ReadOnly]
	public Vector3 extents = Vector3.zero;

	[ReadOnly]
	public Vector3 offset = Vector3.zero;

	public bool HeightMap = true;

	public bool AlphaMap = true;

	public bool WaterMap;

	[InspectorFlags]
	public TerrainSplat.Enum SplatMask;

	[InspectorFlags]
	public TerrainBiome.Enum BiomeMask;

	[InspectorFlags]
	public TerrainTopology.Enum TopologyMask;

	[HideInInspector]
	public Texture2D heightmap;

	[HideInInspector]
	public Texture2D splatmap0;

	[HideInInspector]
	public Texture2D splatmap1;

	[HideInInspector]
	public Texture2D alphamap;

	[HideInInspector]
	public Texture2D biomemap;

	[HideInInspector]
	public Texture2D topologymap;

	[HideInInspector]
	public Texture2D watermap;

	[HideInInspector]
	public Texture2D blendmap;

	protected TerrainPlacement()
	{
	}

	public void Apply(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.ShouldHeight())
		{
			this.ApplyHeight(localToWorld, worldToLocal);
		}
		if (this.ShouldSplat(-1))
		{
			this.ApplySplat(localToWorld, worldToLocal);
		}
		if (this.ShouldAlpha())
		{
			this.ApplyAlpha(localToWorld, worldToLocal);
		}
		if (this.ShouldBiome(-1))
		{
			this.ApplyBiome(localToWorld, worldToLocal);
		}
		if (this.ShouldTopology(-1))
		{
			this.ApplyTopology(localToWorld, worldToLocal);
		}
		if (this.ShouldWater())
		{
			this.ApplyWater(localToWorld, worldToLocal);
		}
	}

	protected abstract void ApplyAlpha(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	protected abstract void ApplyBiome(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	protected abstract void ApplyHeight(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	protected abstract void ApplySplat(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	protected abstract void ApplyTopology(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	protected abstract void ApplyWater(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	protected override Type GetIndexedType()
	{
		return typeof(TerrainPlacement);
	}

	[ContextMenu("Refresh Terrain Data")]
	public void RefreshTerrainData()
	{
		TerrainData terrainDatum = Terrain.activeTerrain.terrainData;
		TerrainHeightMap component = Terrain.activeTerrain.GetComponent<TerrainHeightMap>();
		if (component)
		{
			this.heightmap = component.HeightTexture;
		}
		TerrainSplatMap terrainSplatMap = Terrain.activeTerrain.GetComponent<TerrainSplatMap>();
		if (terrainSplatMap)
		{
			this.splatmap0 = terrainSplatMap.SplatTexture0;
			this.splatmap1 = terrainSplatMap.SplatTexture1;
		}
		TerrainAlphaMap terrainAlphaMap = Terrain.activeTerrain.GetComponent<TerrainAlphaMap>();
		if (terrainAlphaMap)
		{
			this.alphamap = terrainAlphaMap.AlphaTexture;
		}
		TerrainBiomeMap terrainBiomeMap = Terrain.activeTerrain.GetComponent<TerrainBiomeMap>();
		if (terrainBiomeMap)
		{
			this.biomemap = terrainBiomeMap.BiomeTexture;
		}
		TerrainTopologyMap terrainTopologyMap = Terrain.activeTerrain.GetComponent<TerrainTopologyMap>();
		if (terrainTopologyMap)
		{
			this.topologymap = terrainTopologyMap.TopologyTexture;
		}
		TerrainWaterMap terrainWaterMap = Terrain.activeTerrain.GetComponent<TerrainWaterMap>();
		if (terrainWaterMap)
		{
			this.watermap = terrainWaterMap.WaterTexture;
		}
		TerrainBlendMap terrainBlendMap = Terrain.activeTerrain.GetComponent<TerrainBlendMap>();
		if (terrainBlendMap)
		{
			this.blendmap = terrainBlendMap.BlendTexture;
		}
		this.size = terrainDatum.size;
		this.extents = terrainDatum.size * 0.5f;
		this.offset = (Terrain.activeTerrain.GetPosition() + (terrainDatum.size.XZ3D() * 0.5f)) - base.transform.position;
	}

	protected bool ShouldAlpha()
	{
		if (this.alphamap == null)
		{
			return false;
		}
		return this.AlphaMap;
	}

	protected bool ShouldBiome(int id = -1)
	{
		if (this.biomemap == null)
		{
			return false;
		}
		return ((int)this.BiomeMask & id) != 0;
	}

	protected bool ShouldHeight()
	{
		if (this.heightmap == null)
		{
			return false;
		}
		return this.HeightMap;
	}

	protected bool ShouldSplat(int id = -1)
	{
		if (!(this.splatmap0 != null) || !(this.splatmap1 != null))
		{
			return false;
		}
		return ((int)this.SplatMask & id) != 0;
	}

	protected bool ShouldTopology(int id = -1)
	{
		if (this.topologymap == null)
		{
			return false;
		}
		return ((int)this.TopologyMask & id) != 0;
	}

	protected bool ShouldWater()
	{
		if (this.watermap == null)
		{
			return false;
		}
		return this.WaterMap;
	}
}