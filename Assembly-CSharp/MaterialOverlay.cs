using System;
using UnityEngine;

[ExecuteInEditMode]
public class MaterialOverlay : MonoBehaviour
{
	public Material material;

	public MaterialOverlay()
	{
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.material)
		{
			Graphics.Blit(source, destination);
			return;
		}
		for (int i = 0; i < this.material.passCount; i++)
		{
			Graphics.Blit(source, destination, this.material, i);
		}
	}
}