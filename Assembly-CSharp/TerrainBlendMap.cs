using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TerrainBlendMap : TerrainMap<byte>
{
	public Texture2D BlendTexture;

	public TerrainBlendMap()
	{
	}

	public void ApplyTextures()
	{
		this.BlendTexture.Apply(true, false);
		this.BlendTexture.Compress(false);
		this.BlendTexture.Apply(false, true);
	}

	public void GenerateTextures()
	{
		this.BlendTexture = new Texture2D(this.res, this.res, TextureFormat.Alpha8, true, true)
		{
			name = "BlendTexture",
			wrapMode = TextureWrapMode.Clamp
		};
		Color32[] color32 = new Color32[this.res * this.res];
		Parallel.For(0, this.res, (int z) => {
			for (int i = 0; i < this.res; i++)
			{
				byte u003cu003e4_this = this.src[z * this.res + i];
				color32[z * this.res + i] = new Color32(u003cu003e4_this, u003cu003e4_this, u003cu003e4_this, u003cu003e4_this);
			}
		});
		this.BlendTexture.SetPixels32(color32);
	}

	public float GetAlpha(Vector3 worldPos)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		return this.GetAlpha(single, TerrainMeta.NormalizeZ(worldPos.z));
	}

	public float GetAlpha(float normX, float normZ)
	{
		int num = this.res - 1;
		float single = normX * (float)num;
		float single1 = normZ * (float)num;
		int num1 = Mathf.Clamp((int)single, 0, num);
		int num2 = Mathf.Clamp((int)single1, 0, num);
		int num3 = Mathf.Min(num1 + 1, num);
		int num4 = Mathf.Min(num2 + 1, num);
		float single2 = Mathf.Lerp(this.GetAlpha(num1, num2), this.GetAlpha(num3, num2), single - (float)num1);
		float single3 = Mathf.Lerp(this.GetAlpha(num1, num4), this.GetAlpha(num3, num4), single - (float)num1);
		return Mathf.Lerp(single2, single3, single1 - (float)num2);
	}

	public float GetAlpha(int x, int z)
	{
		return BitUtility.Byte2Float(this.src[z * this.res + x]);
	}

	public void SetAlpha(Vector3 worldPos, float a)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		this.SetAlpha(single, TerrainMeta.NormalizeZ(worldPos.z), a);
	}

	public void SetAlpha(float normX, float normZ, float a)
	{
		int num = base.Index(normX);
		this.SetAlpha(num, base.Index(normZ), a);
	}

	public void SetAlpha(int x, int z, float a)
	{
		this.dst[z * this.res + x] = BitUtility.Float2Byte(a);
	}

	public void SetAlpha(int x, int z, float a, float opacity)
	{
		this.SetAlpha(x, z, Mathf.Lerp(this.GetAlpha(x, z), a, opacity));
	}

	public void SetAlpha(Vector3 worldPos, float a, float opacity, float radius, float fade = 0f)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetAlpha(single, single1, a, opacity, radius, fade);
	}

	public void SetAlpha(float normX, float normZ, float a, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = (int x, int z, float lerp) => {
			lerp *= opacity;
			if (lerp > 0f)
			{
				this.SetAlpha(x, z, a, lerp);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	public override void Setup()
	{
		byte[] numArray;
		if (this.BlendTexture == null)
		{
			this.res = this.terrain.terrainData.alphamapResolution;
			byte[] numArray1 = new byte[this.res * this.res];
			numArray = numArray1;
			this.dst = (T[])numArray1;
			this.src = numArray;
			for (int i = 0; i < this.res; i++)
			{
				for (int j = 0; j < this.res; j++)
				{
					this.dst[i * this.res + j] = 0;
				}
			}
			return;
		}
		if (this.BlendTexture.width != this.BlendTexture.height)
		{
			Debug.LogError(string.Concat("Invalid alpha texture: ", this.BlendTexture.name));
			return;
		}
		this.res = this.BlendTexture.width;
		byte[] numArray2 = new byte[this.res * this.res];
		numArray = numArray2;
		this.dst = (T[])numArray2;
		this.src = numArray;
		Color32[] pixels32 = this.BlendTexture.GetPixels32();
		int num = 0;
		int num1 = 0;
		while (num < this.res)
		{
			int num2 = 0;
			while (num2 < this.res)
			{
				this.dst[num * this.res + num2] = pixels32[num1].a;
				num2++;
				num1++;
			}
			num++;
		}
	}
}