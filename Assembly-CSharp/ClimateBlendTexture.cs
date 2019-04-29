using System;
using UnityEngine;

public class ClimateBlendTexture : ProcessedTexture
{
	public ClimateBlendTexture(int width, int height, bool linear = true)
	{
		this.material = base.CreateMaterial("Hidden/ClimateBlendLUTs");
		this.result = base.CreateRenderTexture("Climate Blend Texture", width, height, linear);
		this.result.wrapMode = TextureWrapMode.Clamp;
	}

	public void Blend(Texture srcLut1, Texture dstLut1, float lerpLut1, Texture srcLut2, Texture dstLut2, float lerpLut2, float lerp, ClimateBlendTexture prevLut, float time)
	{
		this.material.SetTexture("_srcLut1", srcLut1);
		this.material.SetTexture("_dstLut1", dstLut1);
		this.material.SetTexture("_srcLut2", srcLut2);
		this.material.SetTexture("_dstLut2", dstLut2);
		this.material.SetTexture("_prevLut", prevLut);
		this.material.SetFloat("_lerpLut1", lerpLut1);
		this.material.SetFloat("_lerpLut2", lerpLut2);
		this.material.SetFloat("_lerp", lerp);
		this.material.SetFloat("_time", time);
		Graphics.Blit(null, this.result, this.material);
	}

	public bool CheckLostData()
	{
		if (this.result.IsCreated())
		{
			return false;
		}
		this.result.Create();
		return true;
	}

	public static void Swap(ref ClimateBlendTexture a, ref ClimateBlendTexture b)
	{
		ClimateBlendTexture climateBlendTexture = a;
		a = b;
		b = climateBlendTexture;
	}
}