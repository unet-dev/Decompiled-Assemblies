using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Facepunch.Utility
{
	public static class Texture
	{
		public static void CompressNormals(this Texture2D tex)
		{
			Color color = new Color();
			for (int i = 0; i < tex.width; i++)
			{
				for (int j = 0; j < tex.height; j++)
				{
					Color pixel = tex.GetPixel(i, j);
					color.r = pixel.g;
					color.g = pixel.g;
					color.b = pixel.g;
					color.a = pixel.r;
					tex.SetPixel(i, j, color);
				}
			}
			tex.Apply(true);
		}

		public static Texture2D CreateReadableCopy(Texture2D texture, int width = 0, int height = 0)
		{
			if (width <= 0)
			{
				width = texture.width;
			}
			if (height <= 0)
			{
				height = texture.height;
			}
			RenderTexture renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, texture.mipmapCount > 0)
			{
				name = texture.name
			};
			Graphics.Blit(texture, renderTexture);
			RenderTexture.active = renderTexture;
			texture2D.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0, true);
			RenderTexture.active = null;
			texture2D.Apply(true);
			UnityEngine.Object.DestroyImmediate(renderTexture);
			return texture2D;
		}

		public static void DecompressNormals(this Texture2D tex)
		{
			Color linearSpace = new Color();
			for (int i = 0; i < tex.width; i++)
			{
				for (int j = 0; j < tex.height; j++)
				{
					Color pixel = tex.GetPixel(i, j);
					linearSpace.r = pixel.a;
					linearSpace.g = Mathf.GammaToLinearSpace(pixel.g);
					linearSpace.b = 1f;
					linearSpace.a = 1f;
					tex.SetPixel(i, j, linearSpace);
				}
			}
			tex.Apply(true);
		}

		public static Texture2D LimitSize(Texture2D tex, int w, int h)
		{
			if (tex.width <= w && tex.height <= h)
			{
				return tex;
			}
			int num = tex.width;
			int num1 = tex.height;
			if (num > w)
			{
				double num2 = (double)w / (double)num;
				num = (int)((double)num * num2);
				num1 = (int)((double)num1 * num2);
			}
			if (num1 > h)
			{
				double num3 = (double)h / (double)num1;
				num = (int)((double)num * num3);
				num1 = (int)((double)num1 * num3);
			}
			return Facepunch.Utility.Texture.CreateReadableCopy(tex, num, num1);
		}

		public static Texture2D LimitSize(Texture2D tex, object maxTextureSize1, object maxTextureSize2)
		{
			throw new NotImplementedException();
		}

		public static bool SaveAsPng(this UnityEngine.Texture texture, string fileName)
		{
			Texture2D texture2D = texture as Texture2D;
			if (texture2D == null)
			{
				return false;
			}
			byte[] pNG = null;
			try
			{
				pNG = texture2D.EncodeToPNG();
			}
			catch (UnityException unityException)
			{
			}
			if (pNG == null)
			{
				Texture2D texture2D1 = Facepunch.Utility.Texture.CreateReadableCopy(texture2D, 0, 0);
				pNG = texture2D1.EncodeToPNG();
				UnityEngine.Object.DestroyImmediate(texture2D1);
			}
			if (pNG == null)
			{
				return false;
			}
			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}
			File.WriteAllBytes(fileName, pNG);
			return true;
		}
	}
}