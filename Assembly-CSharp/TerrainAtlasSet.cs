using System;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Terrain Atlas Set")]
public class TerrainAtlasSet : ScriptableObject
{
	public const int SplatCount = 8;

	public const int SplatSize = 2048;

	public const int MaxSplatSize = 2047;

	public const int SplatPadding = 256;

	public const int AtlasSize = 8192;

	public const int RegionSize = 2560;

	public const int SplatsPerLine = 3;

	public const int SourceTypeCount = 4;

	public const int AtlasMipCount = 10;

	public static string[] sourceTypeNames;

	public static string[] sourceTypeNamesExt;

	public static string[] sourceTypePostfix;

	public string[] splatNames;

	public bool[] albedoHighpass;

	public string[] albedoPaths;

	public Color[] defaultValues;

	public TerrainAtlasSet.SourceMapSet[] sourceMaps;

	public bool highQualityCompression = true;

	public bool generateTextureAtlases = true;

	public bool generateTextureArrays;

	public string splatSearchPrefix = "terrain_";

	public string splatSearchFolder = "Assets/Content/Nature/Terrain";

	public string albedoAtlasSavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_albedo_atlas";

	public string normalAtlasSavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_normal_atlas";

	public string albedoArraySavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_albedo_array";

	public string normalArraySavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_normal_array";

	static TerrainAtlasSet()
	{
		TerrainAtlasSet.sourceTypeNames = new string[] { "Albedo", "Normal", "Specular", "Height" };
		TerrainAtlasSet.sourceTypeNamesExt = new string[] { "Albedo (rgb)", "Normal (rgb)", "Specular (rgba)", "Height (gray)" };
		TerrainAtlasSet.sourceTypePostfix = new string[] { "_albedo", "_normal", "_specular", "_height" };
	}

	public TerrainAtlasSet()
	{
	}

	public void CheckReset()
	{
		if (this.splatNames == null)
		{
			this.splatNames = new string[] { "Dirt", "Snow", "Sand", "Rock", "Grass", "Forest", "Stones", "Gravel" };
		}
		else if ((int)this.splatNames.Length != 8)
		{
			Array.Resize<string>(ref this.splatNames, 8);
		}
		if (this.albedoHighpass == null)
		{
			this.albedoHighpass = new bool[8];
		}
		else if ((int)this.albedoHighpass.Length != 8)
		{
			Array.Resize<bool>(ref this.albedoHighpass, 8);
		}
		if (this.albedoPaths == null)
		{
			this.albedoPaths = new string[8];
		}
		else if ((int)this.albedoPaths.Length != 8)
		{
			Array.Resize<string>(ref this.albedoPaths, 8);
		}
		if (this.defaultValues == null)
		{
			this.defaultValues = new Color[] { new Color(1f, 1f, 1f, 0.5f), new Color(0.5f, 0.5f, 1f, 0f), new Color(0.5f, 0.5f, 0.5f, 0.5f), Color.black };
		}
		else if ((int)this.defaultValues.Length != 4)
		{
			Array.Resize<Color>(ref this.defaultValues, 4);
		}
		if (this.sourceMaps == null)
		{
			this.sourceMaps = new TerrainAtlasSet.SourceMapSet[4];
		}
		else if ((int)this.sourceMaps.Length != 4)
		{
			Array.Resize<TerrainAtlasSet.SourceMapSet>(ref this.sourceMaps, 4);
		}
		for (int i = 0; i < 4; i++)
		{
			this.sourceMaps[i] = (this.sourceMaps[i] != null ? this.sourceMaps[i] : new TerrainAtlasSet.SourceMapSet());
			this.sourceMaps[i].CheckReset();
		}
	}

	[Serializable]
	public class SourceMapSet
	{
		public Texture2D[] maps;

		public SourceMapSet()
		{
		}

		internal void CheckReset()
		{
			if (this.maps == null)
			{
				this.maps = new Texture2D[8];
				return;
			}
			if ((int)this.maps.Length != 8)
			{
				Array.Resize<Texture2D>(ref this.maps, 8);
			}
		}
	}

	public enum SourceType
	{
		ALBEDO,
		NORMAL,
		SPECULAR,
		HEIGHT,
		COUNT
	}
}