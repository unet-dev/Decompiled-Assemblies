using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TerrainWaterMap : TerrainMap<short>
{
	public Texture2D WaterTexture;

	private float normY;

	public TerrainWaterMap()
	{
	}

	public void ApplyTextures()
	{
		this.WaterTexture.Apply(true, true);
	}

	public void GenerateTextures()
	{
		Color32[] color32Array = new Color32[this.res * this.res];
		Parallel.For(0, this.res, (int z) => {
			for (int i = 0; i < this.res; i++)
			{
				color32Array[z * this.res + i] = BitUtility.EncodeShort(this.src[z * this.res + i]);
			}
		});
		this.WaterTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true)
		{
			name = "WaterTexture",
			wrapMode = TextureWrapMode.Clamp
		};
		this.WaterTexture.SetPixels32(color32Array);
	}

	public float GetDepth(Vector3 worldPos)
	{
		return this.GetHeight(worldPos) - TerrainMeta.HeightMap.GetHeight(worldPos);
	}

	public float GetDepth(float normX, float normZ)
	{
		return this.GetHeight(normX, normZ) - TerrainMeta.HeightMap.GetHeight(normX, normZ);
	}

	public float GetHeight(Vector3 worldPos)
	{
		return TerrainMeta.Position.y + this.GetHeight01(worldPos) * TerrainMeta.Size.y;
	}

	public float GetHeight(float normX, float normZ)
	{
		return TerrainMeta.Position.y + this.GetHeight01(normX, normZ) * TerrainMeta.Size.y;
	}

	public float GetHeight(int x, int z)
	{
		return TerrainMeta.Position.y + this.GetHeight01(x, z) * TerrainMeta.Size.y;
	}

	public float GetHeight01(Vector3 worldPos)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		return this.GetHeight01(single, TerrainMeta.NormalizeZ(worldPos.z));
	}

	public float GetHeight01(float normX, float normZ)
	{
		int num = this.res - 1;
		float single = normX * (float)num;
		float single1 = normZ * (float)num;
		int num1 = Mathf.Clamp((int)single, 0, num);
		int num2 = Mathf.Clamp((int)single1, 0, num);
		int num3 = Mathf.Min(num1 + 1, num);
		int num4 = Mathf.Min(num2 + 1, num);
		float single2 = Mathf.Lerp(this.GetHeight01(num1, num2), this.GetHeight01(num3, num2), single - (float)num1);
		float single3 = Mathf.Lerp(this.GetHeight01(num1, num4), this.GetHeight01(num3, num4), single - (float)num1);
		return Mathf.Lerp(single2, single3, single1 - (float)num2);
	}

	public float GetHeight01(int x, int z)
	{
		return BitUtility.Short2Float(this.src[z * this.res + x]);
	}

	public float GetHeightFast(Vector2 uv)
	{
		int num = this.res - 1;
		float single = uv.x * (float)num;
		float single1 = uv.y * (float)num;
		int num1 = (int)single;
		int num2 = (int)single1;
		float single2 = single - (float)num1;
		float single3 = single1 - (float)num2;
		num1 = (num1 >= 0 ? num1 : 0);
		num2 = (num2 >= 0 ? num2 : 0);
		num1 = (num1 <= num ? num1 : num);
		num2 = (num2 <= num ? num2 : num);
		int num3 = (single < (float)num ? 1 : 0);
		int num4 = (single1 < (float)num ? this.res : 0);
		int num5 = num2 * this.res + num1;
		int num6 = num5 + num3;
		int num7 = num5 + num4;
		int num8 = num7 + num3;
		float single4 = (float)this.src[num5] * 3.051944E-05f;
		float single5 = (float)this.src[num6] * 3.051944E-05f;
		float single6 = (float)this.src[num7] * 3.051944E-05f;
		float single7 = (single5 - single4) * single2 + single4;
		float single8 = (((float)this.src[num8] * 3.051944E-05f - single6) * single2 + single6 - single7) * single3 + single7;
		return TerrainMeta.Position.y + single8 * TerrainMeta.Size.y;
	}

	public Vector3 GetNormal(Vector3 worldPos)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		return this.GetNormal(single, TerrainMeta.NormalizeZ(worldPos.z));
	}

	public Vector3 GetNormal(float normX, float normZ)
	{
		int num = this.res - 1;
		float single = normX * (float)num;
		float single1 = normZ * (float)num;
		int num1 = Mathf.Clamp((int)single, 0, num);
		int num2 = Mathf.Clamp((int)single1, 0, num);
		int num3 = Mathf.Min(num1 + 1, num);
		int num4 = Mathf.Min(num2 + 1, num);
		float height01 = this.GetHeight01(num3, num2) - this.GetHeight01(num1, num2);
		float height011 = this.GetHeight01(num1, num4) - this.GetHeight01(num1, num2);
		Vector3 vector3 = new Vector3(-height01, this.normY, -height011);
		return vector3.normalized;
	}

	public Vector3 GetNormal(int x, int z)
	{
		int num = this.res - 1;
		int num1 = Mathf.Clamp(x - 1, 0, num);
		int num2 = Mathf.Clamp(z - 1, 0, num);
		int num3 = Mathf.Clamp(x + 1, 0, num);
		int num4 = Mathf.Clamp(z + 1, 0, num);
		float height01 = (this.GetHeight01(num1, num4) - this.GetHeight01(num1, num2)) * 0.5f;
		Vector3 vector3 = new Vector3(-((this.GetHeight01(num3, num2) - this.GetHeight01(num1, num2)) * 0.5f), this.normY, -height01);
		return vector3.normalized;
	}

	public Vector3 GetNormalFast(Vector2 uv)
	{
		int num = this.res - 1;
		int num1 = (int)(uv.x * (float)num);
		int num2 = (int)(uv.y * (float)num);
		num1 = (num1 >= 0 ? num1 : 0);
		num2 = (num2 >= 0 ? num2 : 0);
		num1 = (num1 <= num ? num1 : num);
		num2 = (num2 <= num ? num2 : num);
		int num3 = (num1 < num ? 1 : 0);
		int num4 = (num2 < num ? this.res : 0);
		int num5 = num2 * this.res + num1;
		int num6 = num5 + num3;
		int num7 = num5 + num4;
		short num8 = this.src[num5];
		T t = this.src[num6];
		short num9 = this.src[num7];
		float single = (float)(num9 - num8) * 3.051944E-05f;
		return new Vector3(-((float)(t - num8) * 3.051944E-05f), this.normY, -single);
	}

	public float GetSlope(Vector3 worldPos)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(worldPos));
	}

	public float GetSlope(float normX, float normZ)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(normX, normZ));
	}

	public float GetSlope(int x, int z)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(x, z));
	}

	public float GetSlope01(Vector3 worldPos)
	{
		return this.GetSlope(worldPos) * 0.0111111114f;
	}

	public float GetSlope01(float normX, float normZ)
	{
		return this.GetSlope(normX, normZ) * 0.0111111114f;
	}

	public float GetSlope01(int x, int z)
	{
		return this.GetSlope(x, z) * 0.0111111114f;
	}

	public void SetHeight(Vector3 worldPos, float height)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		this.SetHeight(single, TerrainMeta.NormalizeZ(worldPos.z), height);
	}

	public void SetHeight(float normX, float normZ, float height)
	{
		int num = base.Index(normX);
		this.SetHeight(num, base.Index(normZ), height);
	}

	public void SetHeight(int x, int z, float height)
	{
		this.dst[z * this.res + x] = BitUtility.Float2Short(height);
	}

	public override void Setup()
	{
		short[] numArray;
		if (this.WaterTexture == null)
		{
			this.res = this.terrain.terrainData.heightmapResolution;
			short[] numArray1 = new short[this.res * this.res];
			numArray = numArray1;
			this.dst = (T[])numArray1;
			this.src = numArray;
		}
		else if (this.WaterTexture.width != this.WaterTexture.height)
		{
			Debug.LogError(string.Concat("Invalid water texture: ", this.WaterTexture.name));
		}
		else
		{
			this.res = this.WaterTexture.width;
			short[] numArray2 = new short[this.res * this.res];
			numArray = numArray2;
			this.dst = (T[])numArray2;
			this.src = numArray;
			Color32[] pixels32 = this.WaterTexture.GetPixels32();
			int num = 0;
			int num1 = 0;
			while (num < this.res)
			{
				int num2 = 0;
				while (num2 < this.res)
				{
					Color32 color32 = pixels32[num1];
					this.dst[num * this.res + num2] = BitUtility.DecodeShort(color32);
					num2++;
					num1++;
				}
				num++;
			}
		}
		this.normY = TerrainMeta.Size.x / TerrainMeta.Size.y / (float)this.res;
	}
}