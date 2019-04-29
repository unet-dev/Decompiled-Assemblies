using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TerrainBiomeMap : TerrainMap<byte>
{
	public Texture2D BiomeTexture;

	internal int num;

	public TerrainBiomeMap()
	{
	}

	public void ApplyTextures()
	{
		this.BiomeTexture.Apply(true, false);
		this.BiomeTexture.Compress(false);
		this.BiomeTexture.Apply(false, true);
	}

	public void GenerateTextures()
	{
		this.BiomeTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true)
		{
			name = "BiomeTexture",
			wrapMode = TextureWrapMode.Clamp
		};
		Color32[] color32 = new Color32[this.res * this.res];
		Parallel.For(0, this.res, (int z) => {
			for (int i = 0; i < this.res; i++)
			{
				T[] u003cu003e4_this = this.src;
				int num = this.res;
				byte num1 = u003cu003e4_this[(0 + z) * this.res + i];
				byte u003cu003e4_this1 = this.src[(this.res + z) * this.res + i];
				byte u003cu003e4_this2 = this.src[(2 * this.res + z) * this.res + i];
				byte num2 = this.src[(3 * this.res + z) * this.res + i];
				color32[z * this.res + i] = new Color32(num1, u003cu003e4_this1, u003cu003e4_this2, num2);
			}
		});
		this.BiomeTexture.SetPixels32(color32);
	}

	public float GetBiome(Vector3 worldPos, int mask)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBiome(single, single1, mask);
	}

	public float GetBiome(float normX, float normZ, int mask)
	{
		int num = base.Index(normX);
		return this.GetBiome(num, base.Index(normZ), mask);
	}

	public float GetBiome(int x, int z, int mask)
	{
		if (Mathf.IsPowerOfTwo(mask))
		{
			return BitUtility.Byte2Float(this.src[(TerrainBiome.TypeToIndex(mask) * this.res + z) * this.res + x]);
		}
		int num = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainBiome.IndexToType(i) & mask) != 0)
			{
				num += this.src[(i * this.res + z) * this.res + x];
			}
		}
		return Mathf.Clamp01(BitUtility.Byte2Float(num));
	}

	public float GetBiomeMax(Vector3 worldPos, int mask = -1)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBiomeMax(single, single1, mask);
	}

	public float GetBiomeMax(float normX, float normZ, int mask = -1)
	{
		int num = base.Index(normX);
		return this.GetBiomeMax(num, base.Index(normZ), mask);
	}

	public float GetBiomeMax(int x, int z, int mask = -1)
	{
		byte num = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainBiome.IndexToType(i) & mask) != 0)
			{
				byte num1 = this.src[(i * this.res + z) * this.res + x];
				if (num1 >= num)
				{
					num = num1;
				}
			}
		}
		return (float)num;
	}

	public int GetBiomeMaxIndex(Vector3 worldPos, int mask = -1)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBiomeMaxIndex(single, single1, mask);
	}

	public int GetBiomeMaxIndex(float normX, float normZ, int mask = -1)
	{
		int num = base.Index(normX);
		return this.GetBiomeMaxIndex(num, base.Index(normZ), mask);
	}

	public int GetBiomeMaxIndex(int x, int z, int mask = -1)
	{
		byte num = 0;
		int num1 = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainBiome.IndexToType(i) & mask) != 0)
			{
				byte num2 = this.src[(i * this.res + z) * this.res + x];
				if (num2 >= num)
				{
					num = num2;
					num1 = i;
				}
			}
		}
		return num1;
	}

	public int GetBiomeMaxType(Vector3 worldPos, int mask = -1)
	{
		return TerrainBiome.IndexToType(this.GetBiomeMaxIndex(worldPos, mask));
	}

	public int GetBiomeMaxType(float normX, float normZ, int mask = -1)
	{
		return TerrainBiome.IndexToType(this.GetBiomeMaxIndex(normX, normZ, mask));
	}

	public int GetBiomeMaxType(int x, int z, int mask = -1)
	{
		return TerrainBiome.IndexToType(this.GetBiomeMaxIndex(x, z, mask));
	}

	public void SetBiome(Vector3 worldPos, int id)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		this.SetBiome(single, TerrainMeta.NormalizeZ(worldPos.z), id);
	}

	public void SetBiome(float normX, float normZ, int id)
	{
		int num = base.Index(normX);
		this.SetBiome(num, base.Index(normZ), id);
	}

	public void SetBiome(int x, int z, int id)
	{
		int index = TerrainBiome.TypeToIndex(id);
		for (int i = 0; i < this.num; i++)
		{
			if (i != index)
			{
				this.dst[(i * this.res + z) * this.res + x] = 0;
			}
			else
			{
				this.dst[(i * this.res + z) * this.res + x] = 255;
			}
		}
	}

	public void SetBiome(Vector3 worldPos, int id, float v)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetBiome(single, single1, id, v);
	}

	public void SetBiome(float normX, float normZ, int id, float v)
	{
		int num = base.Index(normX);
		this.SetBiome(num, base.Index(normZ), id, v);
	}

	public void SetBiome(int x, int z, int id, float v)
	{
		this.SetBiome(x, z, id, this.GetBiome(x, z, id), v);
	}

	private void SetBiome(int x, int z, int id, float old_val, float new_val)
	{
		int index = TerrainBiome.TypeToIndex(id);
		if (old_val >= 1f)
		{
			return;
		}
		float newVal = (1f - new_val) / (1f - old_val);
		for (int i = 0; i < this.num; i++)
		{
			if (i != index)
			{
				this.dst[(i * this.res + z) * this.res + x] = BitUtility.Float2Byte(newVal * BitUtility.Byte2Float(this.dst[(i * this.res + z) * this.res + x]));
			}
			else
			{
				this.dst[(i * this.res + z) * this.res + x] = BitUtility.Float2Byte(new_val);
			}
		}
	}

	public void SetBiomeRaw(int x, int z, Vector4 v, float opacity)
	{
		if (opacity == 0f)
		{
			return;
		}
		float single = Mathf.Clamp01(v.x + v.y + v.z + v.w);
		if (single == 0f)
		{
			return;
		}
		float single1 = 1f - opacity * single;
		float single2 = opacity;
		if (single1 == 0f && single2 == 1f)
		{
			T[] tArray = this.dst;
			int num = this.res;
			tArray[(0 + z) * this.res + x] = BitUtility.Float2Byte(v.x);
			this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte(v.y);
			this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v.z);
			this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v.w);
			return;
		}
		T[] tArray1 = this.dst;
		int num1 = this.res;
		T[] tArray2 = this.src;
		int num2 = this.res;
		tArray1[(0 + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float(tArray2[(0 + z) * this.res + x]) * single1 + v.x * single2);
		this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float(this.src[(this.res + z) * this.res + x]) * single1 + v.y * single2);
		this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float(this.src[(2 * this.res + z) * this.res + x]) * single1 + v.z * single2);
		this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float(this.src[(3 * this.res + z) * this.res + x]) * single1 + v.w * single2);
	}

	public override void Setup()
	{
		byte[] numArray;
		if (this.BiomeTexture == null)
		{
			this.res = this.terrain.terrainData.alphamapResolution;
			this.num = 4;
			byte[] numArray1 = new byte[this.num * this.res * this.res];
			numArray = numArray1;
			this.dst = (T[])numArray1;
			this.src = numArray;
			return;
		}
		if (this.BiomeTexture.width != this.BiomeTexture.height)
		{
			Debug.LogError(string.Concat("Invalid biome texture: ", this.BiomeTexture.name));
			return;
		}
		this.res = this.BiomeTexture.width;
		this.num = 4;
		byte[] numArray2 = new byte[this.num * this.res * this.res];
		numArray = numArray2;
		this.dst = (T[])numArray2;
		this.src = numArray;
		Color32[] pixels32 = this.BiomeTexture.GetPixels32();
		int num = 0;
		int num1 = 0;
		while (num < this.res)
		{
			int num2 = 0;
			while (num2 < this.res)
			{
				Color32 color32 = pixels32[num1];
				T[] tArray = this.dst;
				int num3 = this.res;
				tArray[(0 + num) * this.res + num2] = color32.r;
				this.dst[(this.res + num) * this.res + num2] = color32.g;
				this.dst[(2 * this.res + num) * this.res + num2] = color32.b;
				this.dst[(3 * this.res + num) * this.res + num2] = color32.a;
				num2++;
				num1++;
			}
			num++;
		}
	}
}