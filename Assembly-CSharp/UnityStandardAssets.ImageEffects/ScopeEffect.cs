using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[AddComponentMenu("Image Effects/Other/Scope Overlay")]
	[ExecuteInEditMode]
	public class ScopeEffect : PostEffectsBase, IImageEffect
	{
		public Material overlayMaterial;

		public ScopeEffect()
		{
		}

		public override bool CheckResources()
		{
			return true;
		}

		public bool IsActive()
		{
			if (!base.enabled)
			{
				return false;
			}
			return this.CheckResources();
		}

		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			this.overlayMaterial.SetVector("_Screen", new Vector2((float)Screen.width, (float)Screen.height));
			Graphics.Blit(source, destination, this.overlayMaterial);
		}
	}
}