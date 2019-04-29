using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName="Rust/Terrain Config")]
public class TerrainConfig : ScriptableObject
{
	public bool CastShadows = true;

	public LayerMask GroundMask = 0;

	public LayerMask WaterMask = 0;

	public PhysicMaterial GenericMaterial;

	public UnityEngine.Material Material;

	public Texture[] AlbedoArrays = new Texture[3];

	public Texture[] NormalArrays = new Texture[3];

	public float HeightMapErrorMin = 5f;

	public float HeightMapErrorMax = 100f;

	public float BaseMapDistanceMin = 100f;

	public float BaseMapDistanceMax = 500f;

	public float ShaderLodMin = 100f;

	public float ShaderLodMax = 600f;

	public TerrainConfig.SplatType[] Splats = new TerrainConfig.SplatType[8];

	public Texture AlbedoArray
	{
		get
		{
			return this.AlbedoArrays[Mathf.Clamp(QualitySettings.masterTextureLimit, 0, 2)];
		}
	}

	public Texture NormalArray
	{
		get
		{
			return this.NormalArrays[Mathf.Clamp(QualitySettings.masterTextureLimit, 0, 2)];
		}
	}

	public TerrainConfig()
	{
	}

	public Color[] GetArcticColors()
	{
		Color[] arcticColor = new Color[(int)this.Splats.Length];
		for (int i = 0; i < (int)this.Splats.Length; i++)
		{
			arcticColor[i] = this.Splats[i].ArcticColor;
		}
		return arcticColor;
	}

	public Color[] GetAridColors()
	{
		Color[] aridColor = new Color[(int)this.Splats.Length];
		for (int i = 0; i < (int)this.Splats.Length; i++)
		{
			aridColor[i] = this.Splats[i].AridColor;
		}
		return aridColor;
	}

	public float GetMaxSplatTiling()
	{
		float splatTiling = Single.MinValue;
		for (int i = 0; i < (int)this.Splats.Length; i++)
		{
			if (this.Splats[i].SplatTiling > splatTiling)
			{
				splatTiling = this.Splats[i].SplatTiling;
			}
		}
		return splatTiling;
	}

	public float GetMinSplatTiling()
	{
		float splatTiling = Single.MaxValue;
		for (int i = 0; i < (int)this.Splats.Length; i++)
		{
			if (this.Splats[i].SplatTiling < splatTiling)
			{
				splatTiling = this.Splats[i].SplatTiling;
			}
		}
		return splatTiling;
	}

	public Vector3[] GetPackedUVMIX()
	{
		Vector3[] vector3 = new Vector3[(int)this.Splats.Length];
		for (int i = 0; i < (int)this.Splats.Length; i++)
		{
			vector3[i] = new Vector3(this.Splats[i].UVMIXMult, this.Splats[i].UVMIXStart, this.Splats[i].UVMIXDist);
		}
		return vector3;
	}

	public PhysicMaterial[] GetPhysicMaterials()
	{
		PhysicMaterial[] material = new PhysicMaterial[(int)this.Splats.Length];
		for (int i = 0; i < (int)this.Splats.Length; i++)
		{
			material[i] = this.Splats[i].Material;
		}
		return material;
	}

	public float[] GetSplatTiling()
	{
		float[] splatTiling = new float[(int)this.Splats.Length];
		for (int i = 0; i < (int)this.Splats.Length; i++)
		{
			splatTiling[i] = this.Splats[i].SplatTiling;
		}
		return splatTiling;
	}

	public Color[] GetTemperateColors()
	{
		Color[] temperateColor = new Color[(int)this.Splats.Length];
		for (int i = 0; i < (int)this.Splats.Length; i++)
		{
			temperateColor[i] = this.Splats[i].TemperateColor;
		}
		return temperateColor;
	}

	public Color[] GetTundraColors()
	{
		Color[] tundraColor = new Color[(int)this.Splats.Length];
		for (int i = 0; i < (int)this.Splats.Length; i++)
		{
			tundraColor[i] = this.Splats[i].TundraColor;
		}
		return tundraColor;
	}

	[Serializable]
	public class SplatType
	{
		public string Name;

		[FormerlySerializedAs("WarmColor")]
		public Color AridColor;

		[FormerlySerializedAs("Color")]
		public Color TemperateColor;

		[FormerlySerializedAs("ColdColor")]
		public Color TundraColor;

		[FormerlySerializedAs("ColdColor")]
		public Color ArcticColor;

		public PhysicMaterial Material;

		public float SplatTiling;

		[Range(0f, 1f)]
		public float UVMIXMult;

		public float UVMIXStart;

		public float UVMIXDist;

		public SplatType()
		{
		}
	}
}