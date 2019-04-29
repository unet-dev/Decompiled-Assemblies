using System;
using UnityEngine;

[AddComponentMenu("Image Effects/FXAA")]
public class FXAA : FXAAPostEffectsBase, IImageEffect
{
	public Shader shader;

	private Material mat;

	public FXAA()
	{
	}

	private void CreateMaterials()
	{
		if (this.mat == null)
		{
			this.mat = base.CheckShaderAndCreateMaterial(this.shader, this.mat);
		}
	}

	public bool IsActive()
	{
		return base.enabled;
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.CreateMaterials();
		float single = 1f / (float)Screen.width;
		float single1 = 1f / (float)Screen.height;
		this.mat.SetVector("_rcpFrame", new Vector4(single, single1, 0f, 0f));
		this.mat.SetVector("_rcpFrameOpt", new Vector4(single * 2f, single1 * 2f, single * 0.5f, single1 * 0.5f));
		Graphics.Blit(source, destination, this.mat);
	}

	private void Start()
	{
		this.CreateMaterials();
		base.CheckSupport(false);
	}
}