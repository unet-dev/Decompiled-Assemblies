using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TerrainTopologyMap : TerrainMap<int>
{
	public Texture2D TopologyTexture;

	public TerrainTopologyMap()
	{
	}

	public void AddTopology(Vector3 worldPos, int mask)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		this.AddTopology(single, TerrainMeta.NormalizeZ(worldPos.z), mask);
	}

	public void AddTopology(float normX, float normZ, int mask)
	{
		int num = base.Index(normX);
		this.AddTopology(num, base.Index(normZ), mask);
	}

	public void AddTopology(int x, int z, int mask)
	{
		ref T tPointer = ref this.dst[z * this.res + x];
		tPointer = (int)tPointer | mask;
	}

	public void AddTopology(Vector3 worldPos, int mask, float radius, float fade = 0f)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddTopology(single, single1, mask, radius, fade);
	}

	public void AddTopology(float normX, float normZ, int mask, float radius, float fade = 0f)
	{
		Action<int, int, float> action = (int x, int z, float lerp) => {
			if ((double)lerp > 0.5)
			{
				ref T u003cu003e4_this = ref this.dst[z * this.res + x];
				u003cu003e4_this = (int)u003cu003e4_this | mask;
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	public void ApplyTextures()
	{
		this.TopologyTexture.Apply(false, true);
	}

	public void GenerateTextures()
	{
		this.TopologyTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, false, true)
		{
			name = "TopologyTexture",
			wrapMode = TextureWrapMode.Clamp
		};
		Color32[] color32Array = new Color32[this.res * this.res];
		Parallel.For(0, this.res, (int z) => {
			for (int i = 0; i < this.res; i++)
			{
				color32Array[z * this.res + i] = BitUtility.EncodeInt(this.src[z * this.res + i]);
			}
		});
		this.TopologyTexture.SetPixels32(color32Array);
	}

	public bool GetTopology(Vector3 worldPos, int mask)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetTopology(single, single1, mask);
	}

	public bool GetTopology(float normX, float normZ, int mask)
	{
		int num = base.Index(normX);
		return this.GetTopology(num, base.Index(normZ), mask);
	}

	public bool GetTopology(int x, int z, int mask)
	{
		return (this.src[z * this.res + x] & mask) != 0;
	}

	public int GetTopology(Vector3 worldPos)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		return this.GetTopology(single, TerrainMeta.NormalizeZ(worldPos.z));
	}

	public int GetTopology(float normX, float normZ)
	{
		int num = base.Index(normX);
		return this.GetTopology(num, base.Index(normZ));
	}

	public int GetTopology(int x, int z)
	{
		return this.src[z * this.res + x];
	}

	public int GetTopology(Vector3 worldPos, float radius)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetTopology(single, single1, radius);
	}

	public int GetTopology(float normX, float normZ, float radius)
	{
		int num = 0;
		float oneOverSize = TerrainMeta.OneOverSize.x * radius;
		int num1 = base.Index(normX - oneOverSize);
		int num2 = base.Index(normX + oneOverSize);
		int num3 = base.Index(normZ - oneOverSize);
		int num4 = base.Index(normZ + oneOverSize);
		for (int i = num3; i <= num4; i++)
		{
			for (int j = num1; j <= num2; j++)
			{
				num |= this.src[i * this.res + j];
			}
		}
		return num;
	}

	public int GetTopologyFast(Vector2 uv)
	{
		int num = this.res - 1;
		int num1 = (int)(uv.x * (float)this.res);
		int num2 = (int)(uv.y * (float)this.res);
		num1 = (num1 >= 0 ? num1 : 0);
		num2 = (num2 >= 0 ? num2 : 0);
		num1 = (num1 <= num ? num1 : num);
		num2 = (num2 <= num ? num2 : num);
		return this.src[num2 * this.res + num1];
	}

	public void RemoveTopology(Vector3 worldPos, int mask)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		this.RemoveTopology(single, TerrainMeta.NormalizeZ(worldPos.z), mask);
	}

	public void RemoveTopology(float normX, float normZ, int mask)
	{
		int num = base.Index(normX);
		this.RemoveTopology(num, base.Index(normZ), mask);
	}

	public void RemoveTopology(int x, int z, int mask)
	{
		ref T tPointer = ref this.dst[z * this.res + x];
		tPointer = (int)tPointer & ~mask;
	}

	public void SetTopology(Vector3 worldPos, int mask)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		this.SetTopology(single, TerrainMeta.NormalizeZ(worldPos.z), mask);
	}

	public void SetTopology(float normX, float normZ, int mask)
	{
		int num = base.Index(normX);
		this.SetTopology(num, base.Index(normZ), mask);
	}

	public void SetTopology(int x, int z, int mask)
	{
		this.dst[z * this.res + x] = mask;
	}

	public void SetTopology(Vector3 worldPos, int mask, float radius, float fade = 0f)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetTopology(single, single1, mask, radius, fade);
	}

	public void SetTopology(float normX, float normZ, int mask, float radius, float fade = 0f)
	{
		Action<int, int, float> action = (int x, int z, float lerp) => {
			if ((double)lerp > 0.5)
			{
				this.dst[z * this.res + x] = mask;
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	public override void Setup()
	{
		int[] numArray;
		if (this.TopologyTexture == null)
		{
			this.res = this.terrain.terrainData.alphamapResolution;
			int[] numArray1 = new int[this.res * this.res];
			numArray = numArray1;
			this.dst = (T[])numArray1;
			this.src = numArray;
			return;
		}
		if (this.TopologyTexture.width != this.TopologyTexture.height)
		{
			Debug.LogError(string.Concat("Invalid topology texture: ", this.TopologyTexture.name));
			return;
		}
		this.res = this.TopologyTexture.width;
		int[] numArray2 = new int[this.res * this.res];
		numArray = numArray2;
		this.dst = (T[])numArray2;
		this.src = numArray;
		Color32[] pixels32 = this.TopologyTexture.GetPixels32();
		int num = 0;
		int num1 = 0;
		while (num < this.res)
		{
			int num2 = 0;
			while (num2 < this.res)
			{
				this.dst[num * this.res + num2] = BitUtility.DecodeInt(pixels32[num1]);
				num2++;
				num1++;
			}
			num++;
		}
	}
}