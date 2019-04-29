using System;
using UnityEngine;

public class ProcessedTexture
{
	protected RenderTexture result;

	protected Material material;

	public ProcessedTexture()
	{
	}

	protected Material CreateMaterial(string shader)
	{
		return this.CreateMaterial(Shader.Find(shader));
	}

	protected Material CreateMaterial(Shader shader)
	{
		return new Material(shader)
		{
			hideFlags = HideFlags.DontSave
		};
	}

	protected RenderTexture CreateRenderTexture(string name, int width, int height, bool linear)
	{
		RenderTexture renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, (linear ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB))
		{
			hideFlags = HideFlags.DontSave,
			name = name,
			filterMode = FilterMode.Bilinear,
			anisoLevel = 0
		};
		renderTexture.Create();
		return renderTexture;
	}

	protected RenderTexture CreateTemporary()
	{
		return RenderTexture.GetTemporary(this.result.width, this.result.height, this.result.depth, this.result.format, (this.result.sRGB ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear));
	}

	protected void DestroyMaterial(ref Material mat)
	{
		if (mat == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(mat);
		mat = null;
	}

	protected void DestroyRenderTexture(ref RenderTexture rt)
	{
		if (rt == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(rt);
		rt = null;
	}

	public void Dispose()
	{
		this.DestroyRenderTexture(ref this.result);
		this.DestroyMaterial(ref this.material);
	}

	public static implicit operator Texture(ProcessedTexture t)
	{
		return t.result;
	}

	protected void ReleaseTemporary(RenderTexture rt)
	{
		RenderTexture.ReleaseTemporary(rt);
	}
}