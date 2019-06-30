using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public static class TextureEx
	{
		private static Color32[] buffer;

		static TextureEx()
		{
			TextureEx.buffer = new Color32[8192];
		}

		public static void Clear(this Texture2D tex, Color32 color)
		{
			if (tex.width > (int)TextureEx.buffer.Length)
			{
				Debug.LogError(string.Concat("Trying to clear texture that is too big: ", tex.width));
				return;
			}
			for (int i = 0; i < tex.width; i++)
			{
				TextureEx.buffer[i] = color;
			}
			for (int j = 0; j < tex.height; j++)
			{
				tex.SetPixels32(0, j, tex.width, 1, TextureEx.buffer);
			}
			tex.Apply();
		}

		public static int GetBitsPerPixel(TextureFormat format)
		{
			switch (format)
			{
				case TextureFormat.Alpha8:
				{
					return 8;
				}
				case TextureFormat.ARGB4444:
				{
					return 16;
				}
				case TextureFormat.RGB24:
				{
					return 24;
				}
				case TextureFormat.RGBA32:
				{
					return 32;
				}
				case TextureFormat.ARGB32:
				{
					return 32;
				}
				case TextureFormat.ARGB4444 | TextureFormat.RGBA32:
				case 8:
				case TextureFormat.R16:
				case TextureFormat.Alpha8 | TextureFormat.ARGB4444 | TextureFormat.RGB24 | TextureFormat.R16 | TextureFormat.DXT1:
				{
					return 0;
				}
				case TextureFormat.RGB565:
				{
					return 16;
				}
				case TextureFormat.DXT1:
				{
					return 4;
				}
				case TextureFormat.DXT5:
				{
					return 8;
				}
				case TextureFormat.RGBA4444:
				{
					return 16;
				}
				case TextureFormat.BGRA32:
				{
					return 32;
				}
				default:
				{
					switch (format)
					{
						case TextureFormat.BC7:
						{
							return 8;
						}
						case TextureFormat.BC4:
						case TextureFormat.BC5:
						case TextureFormat.DXT1Crunched:
						case TextureFormat.DXT5Crunched:
						{
							return 0;
						}
						case TextureFormat.PVRTC_RGB2:
						{
							return 2;
						}
						case TextureFormat.PVRTC_RGBA2:
						{
							return 2;
						}
						case TextureFormat.PVRTC_RGB4:
						{
							return 4;
						}
						case TextureFormat.PVRTC_RGBA4:
						{
							return 4;
						}
						case TextureFormat.ETC_RGB4:
						{
							return 4;
						}
						default:
						{
							if (format == TextureFormat.ETC2_RGBA8)
							{
								return 8;
							}
							return 0;
						}
					}
					break;
				}
			}
		}

		public static int GetSizeInBytes(this Texture texture)
		{
			int num = texture.width;
			int num1 = texture.height;
			if (texture is Texture2D)
			{
				Texture2D texture2D = texture as Texture2D;
				int bitsPerPixel = TextureEx.GetBitsPerPixel(texture2D.format);
				int num2 = texture2D.mipmapCount;
				int num3 = 1;
				int num4 = 0;
				while (num3 <= num2)
				{
					num4 = num4 + num * num1 * bitsPerPixel / 8;
					num /= 2;
					num1 /= 2;
					num3++;
				}
				return num4;
			}
			if (!(texture is Texture2DArray))
			{
				if (!(texture is Cubemap))
				{
					return 0;
				}
				int bitsPerPixel1 = TextureEx.GetBitsPerPixel((texture as Cubemap).format);
				return num * num1 * bitsPerPixel1 / 8 * 6;
			}
			Texture2DArray texture2DArray = texture as Texture2DArray;
			int bitsPerPixel2 = TextureEx.GetBitsPerPixel(texture2DArray.format);
			int num5 = 10;
			int num6 = 1;
			int num7 = 0;
			int num8 = texture2DArray.depth;
			while (num6 <= num5)
			{
				num7 = num7 + num * num1 * bitsPerPixel2 / 8;
				num /= 2;
				num1 /= 2;
				num6++;
			}
			return num7 * num8;
		}
	}
}