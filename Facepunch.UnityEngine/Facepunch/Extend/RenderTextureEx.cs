using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Facepunch.Extend
{
	public static class RenderTextureEx
	{
		private static Material _alphaBlending;

		public static Material AlphaBlending
		{
			get
			{
				if (!RenderTextureEx._alphaBlending)
				{
					RenderTextureEx._alphaBlending = new Material(Shader.Find("Hidden/BlitAlphaBlend"));
				}
				return RenderTextureEx._alphaBlending;
			}
		}

		public static void Blit(this RenderTexture t, Texture tex)
		{
			Graphics.Blit(tex, t);
		}

		public static void BlitWithAlphaBlending(this RenderTexture t, Texture tex)
		{
			Graphics.Blit(tex, t, RenderTextureEx.AlphaBlending, 0);
		}

		public static void ToTexture(this RenderTexture t, Texture texture)
		{
			if (texture.width != t.width)
			{
				throw new Exception("width should match!");
			}
			if (texture.height != t.height)
			{
				throw new Exception("height should match!");
			}
			Graphics.SetRenderTarget(t);
			(texture as Texture2D).ReadPixels(new Rect(0f, 0f, (float)texture.width, (float)texture.height), 0, 0);
			Graphics.SetRenderTarget(null);
		}
	}
}