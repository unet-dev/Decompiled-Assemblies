using System;
using UnityEngine;

public class BlurTexture : ProcessedTexture
{
	public BlurTexture(int width, int height, bool linear = true)
	{
		this.material = base.CreateMaterial("Hidden/Rust/SeparableBlur");
		this.result = base.CreateRenderTexture("Blur Texture", width, height, linear);
	}

	public void Blur(float radius)
	{
		this.Blur(this.result, radius);
	}

	public void Blur(Texture source, float radius)
	{
		RenderTexture renderTexture = base.CreateTemporary();
		this.material.SetVector("offsets", new Vector4(radius / (float)Screen.width, 0f, 0f, 0f));
		Graphics.Blit(source, renderTexture, this.material, 0);
		this.material.SetVector("offsets", new Vector4(0f, radius / (float)Screen.height, 0f, 0f));
		Graphics.Blit(renderTexture, this.result, this.material, 0);
		base.ReleaseTemporary(renderTexture);
	}
}