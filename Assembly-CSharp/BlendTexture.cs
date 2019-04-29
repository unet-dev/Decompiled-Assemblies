using System;
using UnityEngine;

public class BlendTexture : ProcessedTexture
{
	public BlendTexture(int width, int height, bool linear = true)
	{
		this.material = base.CreateMaterial("Hidden/BlitCopyAlpha");
		this.result = base.CreateRenderTexture("Blend Texture", width, height, linear);
	}

	public void Blend(Texture source, Texture target, float alpha)
	{
		this.material.SetTexture("_BlendTex", target);
		this.material.SetFloat("_Alpha", Mathf.Clamp01(alpha));
		Graphics.Blit(source, this.result, this.material);
	}

	public void CopyTo(BlendTexture target)
	{
		Graphics.Blit(this.result, target.result);
	}
}