using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TerrainSplatMap : TerrainMap<byte>
{
	public Texture2D SplatTexture0;

	public Texture2D SplatTexture1;

	internal int num;

	public TerrainSplatMap()
	{
	}

	public void AddSplat(Vector3 worldPos, int id, float delta, float radius, float fade = 0f)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddSplat(single, single1, id, delta, radius, fade);
	}

	public void AddSplat(float normX, float normZ, int id, float delta, float radius, float fade = 0f)
	{
		int index = TerrainSplat.TypeToIndex(id);
		Action<int, int, float> action = (int x, int z, float lerp) => {
			if (lerp > 0f)
			{
				float u003cu003e4_this = (float)this.dst[(index * this.res + z) * this.res + x];
				float single = Mathf.Clamp01(u003cu003e4_this + lerp * delta);
				this.SetSplat(x, z, id, u003cu003e4_this, single);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	public void ApplyTextures()
	{
		this.SplatTexture0.Apply(true, true);
		this.SplatTexture1.Apply(true, true);
	}

	public void GenerateTextures()
	{
		this.SplatTexture0 = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true)
		{
			name = "SplatTexture0",
			wrapMode = TextureWrapMode.Clamp
		};
		Color32[] color32 = new Color32[this.res * this.res];
		Parallel.For(0, this.res, (int z) => {
			byte num;
			for (int i = 0; i < this.res; i++)
			{
				if (this.num > 0)
				{
					T[] u003cu003e4_this = this.src;
					int u003cu003e4_this1 = this.res;
					num = u003cu003e4_this[(0 + z) * this.res + i];
				}
				else
				{
					num = 0;
				}
				color32[z * this.res + i] = new Color32(num, (this.num > 1 ? this.src[(this.res + z) * this.res + i] : 0), (this.num > 2 ? this.src[(2 * this.res + z) * this.res + i] : 0), (this.num > 3 ? this.src[(3 * this.res + z) * this.res + i] : 0));
			}
		});
		this.SplatTexture0.SetPixels32(color32);
		this.SplatTexture1 = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true)
		{
			name = "SplatTexture1",
			wrapMode = TextureWrapMode.Clamp
		};
		Color32[] color32Array = new Color32[this.res * this.res];
		Parallel.For(0, this.res, (int z) => {
			for (int i = 0; i < this.res; i++)
			{
				color32Array[z * this.res + i] = new Color32((this.num > 4 ? this.src[(4 * this.res + z) * this.res + i] : 0), (this.num > 5 ? this.src[(5 * this.res + z) * this.res + i] : 0), (this.num > 6 ? this.src[(6 * this.res + z) * this.res + i] : 0), (this.num > 7 ? this.src[(7 * this.res + z) * this.res + i] : 0));
			}
		});
		this.SplatTexture1.SetPixels32(color32Array);
	}

	public float GetSplat(Vector3 worldPos, int mask)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetSplat(single, single1, mask);
	}

	public float GetSplat(float normX, float normZ, int mask)
	{
		int num = this.res - 1;
		float single = normX * (float)num;
		float single1 = normZ * (float)num;
		int num1 = Mathf.Clamp((int)single, 0, num);
		int num2 = Mathf.Clamp((int)single1, 0, num);
		int num3 = Mathf.Min(num1 + 1, num);
		int num4 = Mathf.Min(num2 + 1, num);
		float single2 = Mathf.Lerp(this.GetSplat(num1, num2, mask), this.GetSplat(num3, num2, mask), single - (float)num1);
		float single3 = Mathf.Lerp(this.GetSplat(num1, num4, mask), this.GetSplat(num3, num4, mask), single - (float)num1);
		return Mathf.Lerp(single2, single3, single1 - (float)num2);
	}

	public float GetSplat(int x, int z, int mask)
	{
		if (Mathf.IsPowerOfTwo(mask))
		{
			return BitUtility.Byte2Float(this.src[(TerrainSplat.TypeToIndex(mask) * this.res + z) * this.res + x]);
		}
		int num = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainSplat.IndexToType(i) & mask) != 0)
			{
				num += this.src[(i * this.res + z) * this.res + x];
			}
		}
		return Mathf.Clamp01(BitUtility.Byte2Float(num));
	}

	public float GetSplatMax(Vector3 worldPos, int mask = -1)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetSplatMax(single, single1, mask);
	}

	public float GetSplatMax(float normX, float normZ, int mask = -1)
	{
		int num = base.Index(normX);
		return this.GetSplatMax(num, base.Index(normZ), mask);
	}

	public float GetSplatMax(int x, int z, int mask = -1)
	{
		byte num = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainSplat.IndexToType(i) & mask) != 0)
			{
				byte num1 = this.src[(i * this.res + z) * this.res + x];
				if (num1 >= num)
				{
					num = num1;
				}
			}
		}
		return BitUtility.Byte2Float((int)num);
	}

	public int GetSplatMaxIndex(Vector3 worldPos, int mask = -1)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetSplatMaxIndex(single, single1, mask);
	}

	public int GetSplatMaxIndex(float normX, float normZ, int mask = -1)
	{
		int num = base.Index(normX);
		return this.GetSplatMaxIndex(num, base.Index(normZ), mask);
	}

	public int GetSplatMaxIndex(int x, int z, int mask = -1)
	{
		byte num = 0;
		int num1 = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainSplat.IndexToType(i) & mask) != 0)
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

	public int GetSplatMaxType(Vector3 worldPos, int mask = -1)
	{
		return TerrainSplat.IndexToType(this.GetSplatMaxIndex(worldPos, mask));
	}

	public int GetSplatMaxType(float normX, float normZ, int mask = -1)
	{
		return TerrainSplat.IndexToType(this.GetSplatMaxIndex(normX, normZ, mask));
	}

	public int GetSplatMaxType(int x, int z, int mask = -1)
	{
		return TerrainSplat.IndexToType(this.GetSplatMaxIndex(x, z, mask));
	}

	public void SetSplat(Vector3 worldPos, int id)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		this.SetSplat(single, TerrainMeta.NormalizeZ(worldPos.z), id);
	}

	public void SetSplat(float normX, float normZ, int id)
	{
		int num = base.Index(normX);
		this.SetSplat(num, base.Index(normZ), id);
	}

	public void SetSplat(int x, int z, int id)
	{
		int index = TerrainSplat.TypeToIndex(id);
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

	public void SetSplat(Vector3 worldPos, int id, float v)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetSplat(single, single1, id, v);
	}

	public void SetSplat(float normX, float normZ, int id, float v)
	{
		int num = base.Index(normX);
		this.SetSplat(num, base.Index(normZ), id, v);
	}

	public void SetSplat(int x, int z, int id, float v)
	{
		this.SetSplat(x, z, id, this.GetSplat(x, z, id), v);
	}

	public void SetSplat(Vector3 worldPos, int id, float opacity, float radius, float fade = 0f)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetSplat(single, single1, id, opacity, radius, fade);
	}

	public void SetSplat(float normX, float normZ, int id, float opacity, float radius, float fade = 0f)
	{
		int index = TerrainSplat.TypeToIndex(id);
		Action<int, int, float> action = (int x, int z, float lerp) => {
			if (lerp > 0f)
			{
				float u003cu003e4_this = (float)this.dst[(index * this.res + z) * this.res + x];
				float single = Mathf.Lerp(u003cu003e4_this, 1f, lerp * opacity);
				this.SetSplat(x, z, id, u003cu003e4_this, single);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	private void SetSplat(int x, int z, int id, float old_val, float new_val)
	{
		int index = TerrainSplat.TypeToIndex(id);
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

	public void SetSplatRaw(int x, int z, Vector4 v1, Vector4 v2, float opacity)
	{
		if (opacity == 0f)
		{
			return;
		}
		float single = Mathf.Clamp01(v1.x + v1.y + v1.z + v1.w + v2.x + v2.y + v2.z + v2.w);
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
			tArray[(0 + z) * this.res + x] = BitUtility.Float2Byte(v1.x);
			this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte(v1.y);
			this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v1.z);
			this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v1.w);
			this.dst[(4 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v2.x);
			this.dst[(5 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v2.y);
			this.dst[(6 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v2.z);
			this.dst[(7 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v2.w);
			return;
		}
		T[] tArray1 = this.dst;
		int num1 = this.res;
		T[] tArray2 = this.src;
		int num2 = this.res;
		tArray1[(0 + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float(tArray2[(0 + z) * this.res + x]) * single1 + v1.x * single2);
		this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float(this.src[(this.res + z) * this.res + x]) * single1 + v1.y * single2);
		this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float(this.src[(2 * this.res + z) * this.res + x]) * single1 + v1.z * single2);
		this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float(this.src[(3 * this.res + z) * this.res + x]) * single1 + v1.w * single2);
		this.dst[(4 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float(this.src[(4 * this.res + z) * this.res + x]) * single1 + v2.x * single2);
		this.dst[(5 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float(this.src[(5 * this.res + z) * this.res + x]) * single1 + v2.y * single2);
		this.dst[(6 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float(this.src[(6 * this.res + z) * this.res + x]) * single1 + v2.z * single2);
		this.dst[(7 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float(this.src[(7 * this.res + z) * this.res + x]) * single1 + v2.w * single2);
	}

	public override void Setup()
	{
		this.res = this.terrain.terrainData.alphamapResolution;
		this.num = (int)this.config.Splats.Length;
		byte[] numArray = new byte[this.num * this.res * this.res];
		byte[] numArray1 = numArray;
		this.dst = (T[])numArray;
		this.src = numArray1;
		if (this.SplatTexture0 != null)
		{
			if (this.SplatTexture0.width != this.SplatTexture0.height || this.SplatTexture0.width != this.res)
			{
				Debug.LogError(string.Concat("Invalid splat texture: ", this.SplatTexture0.name), this.SplatTexture0);
			}
			else
			{
				Color32[] pixels32 = this.SplatTexture0.GetPixels32();
				int num = 0;
				int num1 = 0;
				while (num < this.res)
				{
					int num2 = 0;
					while (num2 < this.res)
					{
						Color32 color32 = pixels32[num1];
						if (this.num > 0)
						{
							T[] tArray = this.dst;
							int num3 = this.res;
							tArray[(0 + num) * this.res + num2] = color32.r;
						}
						if (this.num > 1)
						{
							this.dst[(this.res + num) * this.res + num2] = color32.g;
						}
						if (this.num > 2)
						{
							this.dst[(2 * this.res + num) * this.res + num2] = color32.b;
						}
						if (this.num > 3)
						{
							this.dst[(3 * this.res + num) * this.res + num2] = color32.a;
						}
						num2++;
						num1++;
					}
					num++;
				}
			}
		}
		if (this.SplatTexture1 != null)
		{
			if (this.SplatTexture1.width == this.SplatTexture1.height && this.SplatTexture1.width == this.res && this.num > 5)
			{
				Color32[] color32Array = this.SplatTexture1.GetPixels32();
				int num4 = 0;
				int num5 = 0;
				while (num4 < this.res)
				{
					int num6 = 0;
					while (num6 < this.res)
					{
						Color32 color321 = color32Array[num5];
						if (this.num > 4)
						{
							this.dst[(4 * this.res + num4) * this.res + num6] = color321.r;
						}
						if (this.num > 5)
						{
							this.dst[(5 * this.res + num4) * this.res + num6] = color321.g;
						}
						if (this.num > 6)
						{
							this.dst[(6 * this.res + num4) * this.res + num6] = color321.b;
						}
						if (this.num > 7)
						{
							this.dst[(7 * this.res + num4) * this.res + num6] = color321.a;
						}
						num6++;
						num5++;
					}
					num4++;
				}
				return;
			}
			Debug.LogError(string.Concat("Invalid splat texture: ", this.SplatTexture1.name), this.SplatTexture1);
		}
	}
}