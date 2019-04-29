using Rust;
using System;
using UnityEngine;

namespace Rust.ImageEffects
{
	[AddComponentMenu("Image Effects/Icon Outline")]
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class IconOutline : MonoBehaviour
	{
		public UnityEngine.Material Material;

		public IconOutline()
		{
		}

		private void OnDisable()
		{
			if (Rust.Application.isQuitting)
			{
				return;
			}
			UnityEngine.Object.DestroyImmediate(this.Material);
			this.Material = null;
		}

		private void OnEnable()
		{
			if (this.Material == null)
			{
				Shader shader = Shader.Find("UI/IconOutline");
				if (shader == null)
				{
					throw new Exception("UI/IconOutline - Missing!");
				}
				this.Material = new UnityEngine.Material(shader)
				{
					hideFlags = HideFlags.DontSave
				};
			}
		}

		[ImageEffectOpaque]
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Graphics.Blit(source, destination, this.Material);
		}
	}
}