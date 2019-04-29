using System;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Material Config")]
public class MaterialConfig : ScriptableObject
{
	[Horizontal(4, 0)]
	public MaterialConfig.ShaderParametersFloat[] Floats;

	[Horizontal(4, 0)]
	public MaterialConfig.ShaderParametersColor[] Colors;

	[Horizontal(4, 0)]
	public MaterialConfig.ShaderParametersTexture[] Textures;

	public string[] ScaleUV;

	private MaterialPropertyBlock properties;

	public MaterialConfig()
	{
	}

	public MaterialPropertyBlock GetMaterialPropertyBlock(Material mat, Vector3 pos, Vector3 scale)
	{
		float single;
		float single1;
		Color color;
		Color color1;
		if (this.properties == null)
		{
			this.properties = new MaterialPropertyBlock();
		}
		this.properties.Clear();
		for (int i = 0; i < (int)this.Floats.Length; i++)
		{
			MaterialConfig.ShaderParametersFloat floats = this.Floats[i];
			float single2 = floats.FindBlendParameters(pos, out single, out single1);
			this.properties.SetFloat(floats.Name, Mathf.Lerp(single, single1, single2));
		}
		for (int j = 0; j < (int)this.Colors.Length; j++)
		{
			MaterialConfig.ShaderParametersColor colors = this.Colors[j];
			float single3 = colors.FindBlendParameters(pos, out color, out color1);
			this.properties.SetColor(colors.Name, Color.Lerp(color, color1, single3));
		}
		for (int k = 0; k < (int)this.Textures.Length; k++)
		{
			MaterialConfig.ShaderParametersTexture textures = this.Textures[k];
			Texture texture = textures.FindBlendParameters(pos);
			if (texture)
			{
				this.properties.SetTexture(textures.Name, texture);
			}
		}
		for (int l = 0; l < (int)this.ScaleUV.Length; l++)
		{
			Vector4 vector = mat.GetVector(this.ScaleUV[l]);
			vector = new Vector4(vector.x * scale.y, vector.y * scale.y, vector.z, vector.w);
			this.properties.SetVector(this.ScaleUV[l], vector);
		}
		return this.properties;
	}

	public class ShaderParameters<T>
	{
		public string Name;

		public T Arid;

		public T Temperate;

		public T Tundra;

		public T Arctic;

		private T[] climates;

		public ShaderParameters()
		{
		}

		public float FindBlendParameters(Vector3 pos, out T src, out T dst)
		{
			if (TerrainMeta.BiomeMap == null)
			{
				src = this.Temperate;
				dst = this.Tundra;
				return 0f;
			}
			if (this.climates == null || this.climates.Length == 0)
			{
				this.climates = new T[] { this.Arid, this.Temperate, this.Tundra, this.Arctic };
			}
			int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, -1);
			int num = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, ~biomeMaxType);
			src = this.climates[TerrainBiome.TypeToIndex(biomeMaxType)];
			dst = this.climates[TerrainBiome.TypeToIndex(num)];
			return TerrainMeta.BiomeMap.GetBiome(pos, num);
		}

		public T FindBlendParameters(Vector3 pos)
		{
			if (TerrainMeta.BiomeMap == null)
			{
				return this.Temperate;
			}
			if (this.climates == null || this.climates.Length == 0)
			{
				this.climates = new T[] { this.Arid, this.Temperate, this.Tundra, this.Arctic };
			}
			int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, -1);
			return this.climates[TerrainBiome.TypeToIndex(biomeMaxType)];
		}
	}

	[Serializable]
	public class ShaderParametersColor : MaterialConfig.ShaderParameters<Color>
	{
		public ShaderParametersColor()
		{
		}
	}

	[Serializable]
	public class ShaderParametersFloat : MaterialConfig.ShaderParameters<float>
	{
		public ShaderParametersFloat()
		{
		}
	}

	[Serializable]
	public class ShaderParametersTexture : MaterialConfig.ShaderParameters<Texture>
	{
		public ShaderParametersTexture()
		{
		}
	}
}