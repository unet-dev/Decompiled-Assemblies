using System;
using UnityEngine;

namespace Facepunch.Extend
{
	public class TextureSampler
	{
		private Color[] _data;

		private int _height;

		private int _width;

		public TextureSampler(Texture2D source)
		{
			this._data = source.GetPixels();
			this._width = source.width;
			this._height = source.height;
		}

		public Color GetPixel(float x, float y)
		{
			int num = (int)this.WrapBetween(x, 0f, (float)this._width);
			int num1 = (int)this.WrapBetween(y, 0f, (float)this._height);
			return this._data[num1 * this._width + num];
		}

		public Color GetPixelBilinear(float u, float v)
		{
			u *= (float)this._width;
			v *= (float)this._height;
			int num = Mathf.FloorToInt(u);
			int num1 = Mathf.FloorToInt(v);
			float single = u - (float)num;
			float single1 = v - (float)num1;
			float single2 = 1f - single;
			float single3 = 1f - single1;
			return (((this.GetPixel((float)num, (float)num1) * single2) + (this.GetPixel((float)(num + 1), (float)num1) * single)) * single3) + (((this.GetPixel((float)num, (float)(num1 + 1)) * single2) + (this.GetPixel((float)(num + 1), (float)(num1 + 1)) * single)) * single1);
		}

		private float Mod(float x, float y)
		{
			if (0f == y)
			{
				return x;
			}
			return x - y * Mathf.Floor(x / y);
		}

		private float WrapBetween(float value, float min, float max)
		{
			return this.Mod(value - min, max - min) + min;
		}
	}
}