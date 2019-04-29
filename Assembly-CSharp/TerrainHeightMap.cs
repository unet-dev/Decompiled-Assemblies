using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TerrainHeightMap : TerrainMap<short>
{
	public Texture2D HeightTexture;

	public Texture2D NormalTexture;

	private float normY;

	public TerrainHeightMap()
	{
	}

	public void AddHeight(Vector3 worldPos, float delta)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		this.AddHeight(single, TerrainMeta.NormalizeZ(worldPos.z), delta);
	}

	public void AddHeight(float normX, float normZ, float delta)
	{
		int num = base.Index(normX);
		this.AddHeight(num, base.Index(normZ), delta);
	}

	public void AddHeight(int x, int z, float delta)
	{
		float single = Mathf.Clamp01(this.GetDstHeight01(x, z) + delta);
		this.SetHeight(x, z, single);
	}

	public void AddHeight(Vector3 worldPos, float delta, float radius, float fade = 0f)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddHeight(single, single1, delta, radius, fade);
	}

	public void AddHeight(float normX, float normZ, float delta, float radius, float fade = 0f)
	{
		Action<int, int, float> action = (int x, int z, float lerp) => {
			if (lerp > 0f)
			{
				this.AddHeight(x, z, lerp * delta);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	public void ApplyTextures()
	{
		this.HeightTexture.Apply(true, false);
		this.NormalTexture.Apply(true, false);
		this.NormalTexture.Compress(false);
		this.HeightTexture.Apply(false, true);
		this.NormalTexture.Apply(false, true);
	}

	public void ApplyToTerrain()
	{
		float[,] heights = this.terrain.terrainData.GetHeights(0, 0, this.res, this.res);
		Parallel.For(0, this.res, (int z) => {
			for (int i = 0; i < this.res; i++)
			{
				heights[z, i] = this.GetHeight01(i, z);
			}
		});
		this.terrain.terrainData.SetHeights(0, 0, heights);
		TerrainCollider component = this.terrain.GetComponent<TerrainCollider>();
		if (component)
		{
			component.enabled = false;
			component.enabled = true;
		}
	}

	public void GenerateTextures(bool heightTexture = true, bool normalTexture = true)
	{
		if (heightTexture)
		{
			Color32[] color32Array = new Color32[this.res * this.res];
			Parallel.For(0, this.res, (int z) => {
				for (int i = 0; i < this.res; i++)
				{
					color32Array[z * this.res + i] = BitUtility.EncodeShort(this.src[z * this.res + i]);
				}
			});
			this.HeightTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true)
			{
				name = "HeightTexture",
				wrapMode = TextureWrapMode.Clamp
			};
			this.HeightTexture.SetPixels32(color32Array);
		}
		if (normalTexture)
		{
			int num = this.res - 1;
			Color32[] color32Array1 = new Color32[num * num];
			Parallel.For(0, num, (int z) => {
				float single = ((float)z + 0.5f) / (float)num;
				for (int i = 0; i < num; i++)
				{
					float single1 = ((float)i + 0.5f) / (float)num;
					Vector3 normal = this.GetNormal(single1, single);
					color32Array1[z * num + i] = BitUtility.EncodeNormal(normal);
				}
			});
			this.NormalTexture = new Texture2D(num, num, TextureFormat.RGBA32, true, true)
			{
				name = "NormalTexture",
				wrapMode = TextureWrapMode.Clamp
			};
			this.NormalTexture.SetPixels32(color32Array1);
		}
	}

	private float GetDstHeight01(int x, int z)
	{
		return BitUtility.Short2Float(this.dst[z * this.res + x]);
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
		float height01 = this.GetHeight01(num1, num2);
		float height011 = this.GetHeight01(num3, num2);
		float height012 = this.GetHeight01(num1, num4);
		float single2 = this.GetHeight01(num3, num4);
		float single3 = single - (float)num1;
		float single4 = single1 - (float)num2;
		float single5 = Mathf.Lerp(height01, height011, single3);
		float single6 = Mathf.Lerp(height012, single2, single3);
		return Mathf.Lerp(single5, single6, single4);
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
		Vector3 normal = this.GetNormal(num1, num2);
		Vector3 vector3 = this.GetNormal(num3, num2);
		Vector3 normal1 = this.GetNormal(num1, num4);
		Vector3 vector31 = this.GetNormal(num3, num4);
		float single2 = single - (float)num1;
		float single3 = single1 - (float)num2;
		Vector3 vector32 = Vector3.Lerp(normal, vector3, single2);
		Vector3 vector33 = Vector3.Lerp(normal1, vector31, single2);
		return Vector3.Lerp(vector32, vector33, single3).normalized;
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

	private Vector3 GetNormalSobel(int x, int z)
	{
		int num = this.res - 1;
		Vector3 vector3 = new Vector3(TerrainMeta.Size.x / (float)num, TerrainMeta.Size.y, TerrainMeta.Size.z / (float)num);
		int num1 = x;
		int num2 = z;
		int num3 = Mathf.Clamp(x - 1, 0, num);
		int num4 = Mathf.Clamp(z - 1, 0, num);
		int num5 = Mathf.Clamp(x + 1, 0, num);
		int num6 = Mathf.Clamp(z + 1, 0, num);
		float height01 = this.GetHeight01(num3, num4) * -1f;
		height01 = height01 + this.GetHeight01(num3, num2) * -2f;
		height01 = height01 + this.GetHeight01(num3, num6) * -1f;
		height01 = height01 + this.GetHeight01(num5, num4) * 1f;
		height01 = height01 + this.GetHeight01(num5, num2) * 2f;
		height01 = height01 + this.GetHeight01(num5, num6) * 1f;
		height01 = height01 * vector3.y / vector3.x;
		float single = this.GetHeight01(num3, num4) * -1f;
		single = single + this.GetHeight01(num1, num4) * -2f;
		single = single + this.GetHeight01(num5, num4) * -1f;
		single = single + this.GetHeight01(num3, num6) * 1f;
		single = single + this.GetHeight01(num1, num6) * 2f;
		single = single + this.GetHeight01(num5, num6) * 1f;
		single = single * vector3.y / vector3.z;
		return (new Vector3(-height01, 8f, -single)).normalized;
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

	private float GetSrcHeight01(int x, int z)
	{
		return BitUtility.Short2Float(this.src[z * this.res + x]);
	}

	public void LowerHeight(Vector3 worldPos, float height, float opacity)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		this.LowerHeight(single, single1, height, opacity);
	}

	public void LowerHeight(float normX, float normZ, float height, float opacity)
	{
		int num = base.Index(normX);
		this.LowerHeight(num, base.Index(normZ), height, opacity);
	}

	public void LowerHeight(int x, int z, float height, float opacity)
	{
		float single = Mathf.Min(this.GetDstHeight01(x, z), Mathf.SmoothStep(this.GetSrcHeight01(x, z), height, opacity));
		this.SetHeight(x, z, single);
	}

	public void LowerHeight(Vector3 worldPos, float opacity, float radius, float fade = 0f)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		float single2 = TerrainMeta.NormalizeY(worldPos.y);
		this.LowerHeight(single, single1, single2, opacity, radius, fade);
	}

	public void LowerHeight(float normX, float normZ, float height, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = (int x, int z, float lerp) => {
			if (lerp > 0f)
			{
				this.LowerHeight(x, z, height, lerp * opacity);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	public void RaiseHeight(Vector3 worldPos, float height, float opacity)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		this.RaiseHeight(single, single1, height, opacity);
	}

	public void RaiseHeight(float normX, float normZ, float height, float opacity)
	{
		int num = base.Index(normX);
		this.RaiseHeight(num, base.Index(normZ), height, opacity);
	}

	public void RaiseHeight(int x, int z, float height, float opacity)
	{
		float single = Mathf.Max(this.GetDstHeight01(x, z), Mathf.SmoothStep(this.GetSrcHeight01(x, z), height, opacity));
		this.SetHeight(x, z, single);
	}

	public void RaiseHeight(Vector3 worldPos, float opacity, float radius, float fade = 0f)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		float single2 = TerrainMeta.NormalizeY(worldPos.y);
		this.RaiseHeight(single, single1, single2, opacity, radius, fade);
	}

	public void RaiseHeight(float normX, float normZ, float height, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = (int x, int z, float lerp) => {
			if (lerp > 0f)
			{
				this.RaiseHeight(x, z, height, lerp * opacity);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
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

	public void SetHeight(Vector3 worldPos, float height, float opacity)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetHeight(single, single1, height, opacity);
	}

	public void SetHeight(float normX, float normZ, float height, float opacity)
	{
		int num = base.Index(normX);
		this.SetHeight(num, base.Index(normZ), height, opacity);
	}

	public void SetHeight(int x, int z, float height, float opacity)
	{
		float single = Mathf.SmoothStep(this.GetDstHeight01(x, z), height, opacity);
		this.SetHeight(x, z, single);
	}

	public void SetHeight(Vector3 worldPos, float opacity, float radius, float fade = 0f)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		float single2 = TerrainMeta.NormalizeY(worldPos.y);
		this.SetHeight(single, single1, single2, opacity, radius, fade);
	}

	public void SetHeight(float normX, float normZ, float height, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = (int x, int z, float lerp) => {
			if (lerp > 0f)
			{
				this.SetHeight(x, z, height, lerp * opacity);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	public override void Setup()
	{
		short[] numArray;
		if (this.HeightTexture == null)
		{
			this.res = this.terrain.terrainData.heightmapResolution;
			short[] numArray1 = new short[this.res * this.res];
			numArray = numArray1;
			this.dst = (T[])numArray1;
			this.src = numArray;
		}
		else if (this.HeightTexture.width != this.HeightTexture.height)
		{
			Debug.LogError(string.Concat("Invalid height texture: ", this.HeightTexture.name));
		}
		else
		{
			this.res = this.HeightTexture.width;
			short[] numArray2 = new short[this.res * this.res];
			numArray = numArray2;
			this.dst = (T[])numArray2;
			this.src = numArray;
			Color32[] pixels32 = this.HeightTexture.GetPixels32();
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