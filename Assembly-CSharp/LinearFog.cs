using System;
using UnityEngine;

[ExecuteInEditMode]
public class LinearFog : MonoBehaviour
{
	public Material fogMaterial;

	public Color fogColor = Color.white;

	public float fogStart;

	public float fogRange = 1f;

	public float fogDensity = 1f;

	public bool fogSky;

	public LinearFog()
	{
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.fogMaterial)
		{
			Graphics.Blit(source, destination);
			return;
		}
		this.fogMaterial.SetColor("_FogColor", this.fogColor);
		this.fogMaterial.SetFloat("_Start", this.fogStart);
		this.fogMaterial.SetFloat("_Range", this.fogRange);
		this.fogMaterial.SetFloat("_Density", this.fogDensity);
		if (!this.fogSky)
		{
			this.fogMaterial.SetFloat("_CutOff", 1f);
		}
		else
		{
			this.fogMaterial.SetFloat("_CutOff", 2f);
		}
		for (int i = 0; i < this.fogMaterial.passCount; i++)
		{
			Graphics.Blit(source, destination, this.fogMaterial, i);
		}
	}
}