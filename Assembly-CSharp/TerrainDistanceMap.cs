using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TerrainDistanceMap : TerrainMap<byte>
{
	public Texture2D DistanceTexture;

	public TerrainDistanceMap()
	{
	}

	public void ApplyTextures()
	{
		this.DistanceTexture.Apply(true, true);
	}

	public void GenerateTextures()
	{
		this.DistanceTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true)
		{
			name = "DistanceTexture",
			wrapMode = TextureWrapMode.Clamp
		};
		Color32[] color32Array = new Color32[this.res * this.res];
		Parallel.For(0, this.res, (int z) => {
			for (int i = 0; i < this.res; i++)
			{
				color32Array[z * this.res + i] = BitUtility.EncodeVector2i(this.GetDistance(i, z));
			}
		});
		this.DistanceTexture.SetPixels32(color32Array);
	}

	public Vector2i GetDistance(Vector3 worldPos)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		return this.GetDistance(single, TerrainMeta.NormalizeZ(worldPos.z));
	}

	public Vector2i GetDistance(float normX, float normZ)
	{
		int num = this.res - 1;
		int num1 = Mathf.Clamp(Mathf.RoundToInt(normX * (float)num), 0, num);
		int num2 = Mathf.Clamp(Mathf.RoundToInt(normZ * (float)num), 0, num);
		return this.GetDistance(num1, num2);
	}

	public Vector2i GetDistance(int x, int z)
	{
		T[] tArray = this.src;
		int num = this.res;
		byte num1 = tArray[(0 + z) * this.res + x];
		byte num2 = this.src[(this.res + z) * this.res + x];
		byte num3 = this.src[(2 * this.res + z) * this.res + x];
		byte num4 = this.src[(3 * this.res + z) * this.res + x];
		if (num1 == 255 && num2 == 255 && num3 == 255 && num4 == 255)
		{
			return new Vector2i(256, 256);
		}
		return new Vector2i((int)(num1 - num2), (int)(num3 - num4));
	}

	public void SetDistance(int x, int z, Vector2i v)
	{
		T[] tArray = this.dst;
		int num = this.res;
		tArray[(0 + z) * this.res + x] = (byte)Mathf.Clamp(v.x, 0, 255);
		this.dst[(this.res + z) * this.res + x] = (byte)Mathf.Clamp(-v.x, 0, 255);
		this.dst[(2 * this.res + z) * this.res + x] = (byte)Mathf.Clamp(v.y, 0, 255);
		this.dst[(3 * this.res + z) * this.res + x] = (byte)Mathf.Clamp(-v.y, 0, 255);
	}

	public override void Setup()
	{
		this.res = this.terrain.terrainData.heightmapResolution;
		byte[] numArray = new byte[4 * this.res * this.res];
		byte[] numArray1 = numArray;
		this.dst = (T[])numArray;
		this.src = numArray1;
		if (this.DistanceTexture != null)
		{
			if (this.DistanceTexture.width == this.DistanceTexture.height && this.DistanceTexture.width == this.res)
			{
				Color32[] pixels32 = this.DistanceTexture.GetPixels32();
				int num = 0;
				int num1 = 0;
				while (num < this.res)
				{
					int num2 = 0;
					while (num2 < this.res)
					{
						this.SetDistance(num2, num, BitUtility.DecodeVector2i(pixels32[num1]));
						num2++;
						num1++;
					}
					num++;
				}
				return;
			}
			Debug.LogError(string.Concat("Invalid distance texture: ", this.DistanceTexture.name), this.DistanceTexture);
		}
	}
}