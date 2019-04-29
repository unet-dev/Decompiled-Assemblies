using Oxide.Core;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainMeta : MonoBehaviour
{
	public UnityEngine.Terrain terrain;

	public TerrainConfig config;

	public TerrainMeta.PaintMode paint;

	[HideInInspector]
	public TerrainMeta.PaintMode currentPaintMode;

	public static TerrainAlphaMap AlphaMap
	{
		get;
		private set;
	}

	public static float BiomeAxisAngle
	{
		get;
		private set;
	}

	public static TerrainBiomeMap BiomeMap
	{
		get;
		private set;
	}

	public static TerrainBlendMap BlendMap
	{
		get;
		private set;
	}

	public static Vector3 Center
	{
		get
		{
			return TerrainMeta.Position + (TerrainMeta.Size * 0.5f);
		}
	}

	public static TerrainCollider Collider
	{
		get;
		private set;
	}

	public static TerrainCollision Collision
	{
		get;
		private set;
	}

	public static TerrainColors Colors
	{
		get;
		private set;
	}

	public static TerrainConfig Config
	{
		get;
		private set;
	}

	public static TerrainData Data
	{
		get;
		private set;
	}

	public static TerrainDistanceMap DistanceMap
	{
		get;
		private set;
	}

	public static TerrainHeightMap HeightMap
	{
		get;
		private set;
	}

	public static Vector3 HighestPoint
	{
		get;
		set;
	}

	public static float LootAxisAngle
	{
		get;
		private set;
	}

	public static Vector3 LowestPoint
	{
		get;
		set;
	}

	public static Vector3 OneOverSize
	{
		get;
		private set;
	}

	public static TerrainPath Path
	{
		get;
		private set;
	}

	public static TerrainPhysics Physics
	{
		get;
		private set;
	}

	public static Vector3 Position
	{
		get;
		private set;
	}

	public static TerrainQuality Quality
	{
		get;
		private set;
	}

	public static Vector3 Size
	{
		get;
		private set;
	}

	public static TerrainSplatMap SplatMap
	{
		get;
		private set;
	}

	public static UnityEngine.Terrain Terrain
	{
		get;
		private set;
	}

	public static TerrainTexturing Texturing
	{
		get;
		private set;
	}

	public static TerrainTopologyMap TopologyMap
	{
		get;
		private set;
	}

	public static UnityEngine.Transform Transform
	{
		get;
		private set;
	}

	public static TerrainWaterMap WaterMap
	{
		get;
		private set;
	}

	public TerrainMeta()
	{
	}

	protected void Awake()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		Shader.DisableKeyword("TERRAIN_PAINTING");
	}

	public void BindShaderProperties()
	{
		if (this.config)
		{
			Shader.SetGlobalTexture("Terrain_AlbedoArray", this.config.AlbedoArray);
			Shader.SetGlobalTexture("Terrain_NormalArray", this.config.NormalArray);
			Shader.SetGlobalVector("Terrain_TexelSize", new Vector2(1f / this.config.GetMinSplatTiling(), 1f / this.config.GetMinSplatTiling()));
			Shader.SetGlobalVector("Terrain_TexelSize0", new Vector4(1f / this.config.Splats[0].SplatTiling, 1f / this.config.Splats[1].SplatTiling, 1f / this.config.Splats[2].SplatTiling, 1f / this.config.Splats[3].SplatTiling));
			Shader.SetGlobalVector("Terrain_TexelSize1", new Vector4(1f / this.config.Splats[4].SplatTiling, 1f / this.config.Splats[5].SplatTiling, 1f / this.config.Splats[6].SplatTiling, 1f / this.config.Splats[7].SplatTiling));
			Shader.SetGlobalVector("Splat0_UVMIX", new Vector3(this.config.Splats[0].UVMIXMult, this.config.Splats[0].UVMIXStart, 1f / this.config.Splats[0].UVMIXDist));
			Shader.SetGlobalVector("Splat1_UVMIX", new Vector3(this.config.Splats[1].UVMIXMult, this.config.Splats[1].UVMIXStart, 1f / this.config.Splats[1].UVMIXDist));
			Shader.SetGlobalVector("Splat2_UVMIX", new Vector3(this.config.Splats[2].UVMIXMult, this.config.Splats[2].UVMIXStart, 1f / this.config.Splats[2].UVMIXDist));
			Shader.SetGlobalVector("Splat3_UVMIX", new Vector3(this.config.Splats[3].UVMIXMult, this.config.Splats[3].UVMIXStart, 1f / this.config.Splats[3].UVMIXDist));
			Shader.SetGlobalVector("Splat4_UVMIX", new Vector3(this.config.Splats[4].UVMIXMult, this.config.Splats[4].UVMIXStart, 1f / this.config.Splats[4].UVMIXDist));
			Shader.SetGlobalVector("Splat5_UVMIX", new Vector3(this.config.Splats[5].UVMIXMult, this.config.Splats[5].UVMIXStart, 1f / this.config.Splats[5].UVMIXDist));
			Shader.SetGlobalVector("Splat6_UVMIX", new Vector3(this.config.Splats[6].UVMIXMult, this.config.Splats[6].UVMIXStart, 1f / this.config.Splats[6].UVMIXDist));
			Shader.SetGlobalVector("Splat7_UVMIX", new Vector3(this.config.Splats[7].UVMIXMult, this.config.Splats[7].UVMIXStart, 1f / this.config.Splats[7].UVMIXDist));
		}
		if (TerrainMeta.HeightMap)
		{
			Shader.SetGlobalTexture("Terrain_Normal", TerrainMeta.HeightMap.NormalTexture);
		}
		if (TerrainMeta.AlphaMap)
		{
			Shader.SetGlobalTexture("Terrain_Alpha", TerrainMeta.AlphaMap.AlphaTexture);
		}
		if (TerrainMeta.BiomeMap)
		{
			Shader.SetGlobalTexture("Terrain_Biome", TerrainMeta.BiomeMap.BiomeTexture);
		}
		if (TerrainMeta.SplatMap)
		{
			Shader.SetGlobalTexture("Terrain_Control0", TerrainMeta.SplatMap.SplatTexture0);
			Shader.SetGlobalTexture("Terrain_Control1", TerrainMeta.SplatMap.SplatTexture1);
		}
		bool waterMap = TerrainMeta.WaterMap;
		if (TerrainMeta.DistanceMap)
		{
			Shader.SetGlobalTexture("Terrain_Distance", TerrainMeta.DistanceMap.DistanceTexture);
		}
		if (this.terrain)
		{
			Shader.SetGlobalVector("Terrain_Position", TerrainMeta.Position);
			Shader.SetGlobalVector("Terrain_Size", TerrainMeta.Size);
			Shader.SetGlobalVector("Terrain_RcpSize", TerrainMeta.OneOverSize);
			if (this.terrain.materialTemplate)
			{
				if (this.terrain.materialTemplate.IsKeywordEnabled("_TERRAIN_BLEND_LINEAR"))
				{
					this.terrain.materialTemplate.DisableKeyword("_TERRAIN_BLEND_LINEAR");
				}
				if (this.terrain.materialTemplate.IsKeywordEnabled("_TERRAIN_VERTEX_NORMALS"))
				{
					this.terrain.materialTemplate.DisableKeyword("_TERRAIN_VERTEX_NORMALS");
				}
			}
		}
	}

	public static Vector3 Denormalize(Vector3 normPos)
	{
		float position = TerrainMeta.Position.x + normPos.x * TerrainMeta.Size.x;
		float single = TerrainMeta.Position.y + normPos.y * TerrainMeta.Size.y;
		float position1 = TerrainMeta.Position.z + normPos.z * TerrainMeta.Size.z;
		return new Vector3(position, single, position1);
	}

	public static float DenormalizeX(float normX)
	{
		return TerrainMeta.Position.x + normX * TerrainMeta.Size.x;
	}

	public static float DenormalizeY(float normY)
	{
		return TerrainMeta.Position.y + normY * TerrainMeta.Size.y;
	}

	public static float DenormalizeZ(float normZ)
	{
		return TerrainMeta.Position.z + normZ * TerrainMeta.Size.z;
	}

	public void Init(UnityEngine.Terrain terrainOverride = null, TerrainConfig configOverride = null)
	{
		if (terrainOverride != null)
		{
			this.terrain = terrainOverride;
		}
		if (configOverride != null)
		{
			this.config = configOverride;
		}
		TerrainMeta.Terrain = this.terrain;
		TerrainMeta.Config = this.config;
		TerrainMeta.Transform = this.terrain.transform;
		TerrainMeta.Data = this.terrain.terrainData;
		TerrainMeta.Size = this.terrain.terrainData.size;
		TerrainMeta.OneOverSize = TerrainMeta.Size.Inverse();
		TerrainMeta.Position = this.terrain.GetPosition();
		TerrainMeta.Collider = this.terrain.GetComponent<TerrainCollider>();
		TerrainMeta.Collision = this.terrain.GetComponent<TerrainCollision>();
		TerrainMeta.Physics = this.terrain.GetComponent<TerrainPhysics>();
		TerrainMeta.Colors = this.terrain.GetComponent<TerrainColors>();
		TerrainMeta.Quality = this.terrain.GetComponent<TerrainQuality>();
		TerrainMeta.Path = this.terrain.GetComponent<TerrainPath>();
		TerrainMeta.BiomeMap = this.terrain.GetComponent<TerrainBiomeMap>();
		TerrainMeta.AlphaMap = this.terrain.GetComponent<TerrainAlphaMap>();
		TerrainMeta.BlendMap = this.terrain.GetComponent<TerrainBlendMap>();
		TerrainMeta.HeightMap = this.terrain.GetComponent<TerrainHeightMap>();
		TerrainMeta.SplatMap = this.terrain.GetComponent<TerrainSplatMap>();
		TerrainMeta.TopologyMap = this.terrain.GetComponent<TerrainTopologyMap>();
		TerrainMeta.WaterMap = this.terrain.GetComponent<TerrainWaterMap>();
		TerrainMeta.DistanceMap = this.terrain.GetComponent<TerrainDistanceMap>();
		TerrainMeta.Texturing = this.terrain.GetComponent<TerrainTexturing>();
		TerrainMeta.HighestPoint = new Vector3(TerrainMeta.Position.x, TerrainMeta.Position.y + TerrainMeta.Size.y, TerrainMeta.Position.z);
		TerrainMeta.LowestPoint = new Vector3(TerrainMeta.Position.x, TerrainMeta.Position.y, TerrainMeta.Position.z);
		TerrainExtension[] components = base.GetComponents<TerrainExtension>();
		for (int i = 0; i < (int)components.Length; i++)
		{
			components[i].Init(this.terrain, this.config);
		}
		uint seed = World.Seed;
		int num = SeedRandom.Range(ref seed, 0, 4) * 90;
		int num1 = SeedRandom.Range(ref seed, -45, 46);
		int num2 = SeedRandom.Sign(ref seed);
		TerrainMeta.LootAxisAngle = (float)num;
		TerrainMeta.BiomeAxisAngle = (float)(num + num1 + num2 * 90);
	}

	public static void InitNoTerrain()
	{
		TerrainMeta.Size = new Vector3(4096f, 4096f, 4096f);
	}

	public static Vector3 Normalize(Vector3 worldPos)
	{
		float single = (worldPos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
		float single1 = (worldPos.y - TerrainMeta.Position.y) * TerrainMeta.OneOverSize.y;
		float single2 = (worldPos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
		return new Vector3(single, single1, single2);
	}

	public static float NormalizeX(float x)
	{
		return (x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
	}

	public static float NormalizeY(float y)
	{
		return (y - TerrainMeta.Position.y) * TerrainMeta.OneOverSize.y;
	}

	public static float NormalizeZ(float z)
	{
		return (z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
	}

	public static bool OutOfBounds(Vector3 worldPos)
	{
		if (worldPos.x < TerrainMeta.Position.x)
		{
			return true;
		}
		if (worldPos.z < TerrainMeta.Position.z)
		{
			return true;
		}
		if (worldPos.x > TerrainMeta.Position.x + TerrainMeta.Size.x)
		{
			return true;
		}
		if (worldPos.z > TerrainMeta.Position.z + TerrainMeta.Size.z)
		{
			return true;
		}
		return false;
	}

	public static bool OutOfMargin(Vector3 worldPos)
	{
		if (worldPos.x < TerrainMeta.Position.x - TerrainMeta.Size.x)
		{
			return true;
		}
		if (worldPos.z < TerrainMeta.Position.z - TerrainMeta.Size.z)
		{
			return true;
		}
		if (worldPos.x > TerrainMeta.Position.x + TerrainMeta.Size.x + TerrainMeta.Size.x)
		{
			return true;
		}
		if (worldPos.z > TerrainMeta.Position.z + TerrainMeta.Size.z + TerrainMeta.Size.z)
		{
			return true;
		}
		return false;
	}

	public void PostSetupComponents()
	{
		TerrainExtension[] components = base.GetComponents<TerrainExtension>();
		for (int i = 0; i < (int)components.Length; i++)
		{
			components[i].PostSetup();
		}
		Interface.CallHook("OnTerrainInitialized");
	}

	public void SetupComponents()
	{
		TerrainExtension[] components = base.GetComponents<TerrainExtension>();
		for (int i = 0; i < (int)components.Length; i++)
		{
			TerrainExtension terrainExtension = components[i];
			terrainExtension.Setup();
			terrainExtension.isInitialized = true;
		}
	}

	public enum PaintMode
	{
		None,
		Splats,
		Biomes,
		Alpha,
		Blend,
		Field,
		Cliff,
		Summit,
		Beachside,
		Beach,
		Forest,
		Forestside,
		Ocean,
		Oceanside,
		Decor,
		Monument,
		Road,
		Roadside,
		Bridge,
		River,
		Riverside,
		Lake,
		Lakeside,
		Offshore,
		Powerline,
		Runway,
		Building,
		Cliffside,
		Mountain,
		Clutter,
		Alt,
		Tier0,
		Tier1,
		Tier2,
		Mainland,
		Hilltop
	}
}