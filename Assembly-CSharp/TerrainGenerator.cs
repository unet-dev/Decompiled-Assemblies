using System;
using UnityEngine;

public class TerrainGenerator : SingletonComponent<TerrainGenerator>
{
	public TerrainConfig config;

	private const float HeightMapRes = 0.5f;

	private const float SplatMapRes = 0.5f;

	private const float BaseMapRes = 0.01f;

	public TerrainGenerator()
	{
	}

	public GameObject CreateTerrain()
	{
		Terrain component = Terrain.CreateTerrainGameObject(new TerrainData()
		{
			baseMapResolution = Mathf.NextPowerOfTwo((int)((float)((float)World.Size) * 0.01f)),
			heightmapResolution = Mathf.NextPowerOfTwo((int)((float)((float)World.Size) * 0.5f)) + 1,
			alphamapResolution = Mathf.NextPowerOfTwo((int)((float)((float)World.Size) * 0.5f)),
			size = new Vector3((float)((float)World.Size), 1000f, (float)((float)World.Size))
		}).GetComponent<Terrain>();
		component.transform.position = base.transform.position + new Vector3((float)(-(ulong)World.Size) * 0.5f, 0f, (float)(-(ulong)World.Size) * 0.5f);
		component.castShadows = this.config.CastShadows;
		component.materialType = Terrain.MaterialType.Custom;
		component.materialTemplate = this.config.Material;
		component.gameObject.tag = base.gameObject.tag;
		component.gameObject.layer = base.gameObject.layer;
		component.gameObject.GetComponent<TerrainCollider>().sharedMaterial = this.config.GenericMaterial;
		TerrainMeta terrainMetum = component.gameObject.AddComponent<TerrainMeta>();
		component.gameObject.AddComponent<TerrainPhysics>();
		component.gameObject.AddComponent<TerrainColors>();
		component.gameObject.AddComponent<TerrainCollision>();
		component.gameObject.AddComponent<TerrainBiomeMap>();
		component.gameObject.AddComponent<TerrainAlphaMap>();
		component.gameObject.AddComponent<TerrainHeightMap>();
		component.gameObject.AddComponent<TerrainSplatMap>();
		component.gameObject.AddComponent<TerrainTopologyMap>();
		component.gameObject.AddComponent<TerrainWaterMap>();
		component.gameObject.AddComponent<TerrainPath>();
		component.gameObject.AddComponent<TerrainTexturing>();
		terrainMetum.terrain = component;
		terrainMetum.config = this.config;
		UnityEngine.Object.DestroyImmediate(base.gameObject);
		return component.gameObject;
	}
}