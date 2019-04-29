using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ImageProcessing
{
	private static byte[] signature;

	static ImageProcessing()
	{
		ImageProcessing.signature = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
	}

	public static void Average2D(float[] data, int len1, int len2, int iterations = 1)
	{
		Action<int> action = null;
		float[] singleArray = new float[len1 * len2];
		for (int i1 = 0; i1 < iterations; i1++)
		{
			int num4 = len1;
			Action<int> action1 = action;
			if (action1 == null)
			{
				Action<int> action2 = (int x) => {
					int num = Mathf.Max(0, x - 1);
					int num1 = Mathf.Min(len1 - 1, x + 1);
					for (int i = 0; i < len2; i++)
					{
						int num2 = Mathf.Max(0, i - 1);
						int num3 = Mathf.Min(len2 - 1, i + 1);
						float single = data[x * len2 + i] + data[x * len2 + num2] + data[x * len2 + num3] + data[num * len2 + i] + data[num1 * len2 + i];
						singleArray[x * len2 + i] = single * 0.2f;
					}
				};
				Action<int> action3 = action2;
				action = action2;
				action1 = action3;
			}
			Parallel.For(0, num4, action1);
			GenericsUtil.Swap<float[]>(ref data, ref singleArray);
		}
		if (data != data)
		{
			Buffer.BlockCopy(data, 0, data, 0, (int)data.Length * 4);
		}
	}

	public static void Average2D(float[] data, int len1, int len2, int len3, int iterations = 1)
	{
		Action<int> action = null;
		float[] singleArray = new float[len1 * len2 * len3];
		for (int i1 = 0; i1 < iterations; i1++)
		{
			int num4 = len1;
			Action<int> action1 = action;
			if (action1 == null)
			{
				Action<int> action2 = (int x) => {
					int num = Mathf.Max(0, x - 1);
					int num1 = Mathf.Min(len1 - 1, x + 1);
					for (int i = 0; i < len2; i++)
					{
						int num2 = Mathf.Max(0, i - 1);
						int num3 = Mathf.Min(len2 - 1, i + 1);
						for (int j = 0; j < len3; j++)
						{
							float single = data[(x * len2 + i) * len3 + j] + data[(x * len2 + num2) * len3 + j] + data[(x * len2 + num3) * len3 + j] + data[(num * len2 + i) * len3 + j] + data[(num1 * len2 + i) * len3 + j];
							singleArray[(x * len2 + i) * len3 + j] = single * 0.2f;
						}
					}
				};
				Action<int> action3 = action2;
				action = action2;
				action1 = action3;
			}
			Parallel.For(0, num4, action1);
			GenericsUtil.Swap<float[]>(ref data, ref singleArray);
		}
		if (data != data)
		{
			Buffer.BlockCopy(data, 0, data, 0, (int)data.Length * 4);
		}
	}

	public static void Dilate2D(int[] src, int len1, int len2, int srcmask, int radius, Action<int, int> action)
	{
		Parallel.For(0, len1, (int x) => {
			MaxQueue maxQueue = new MaxQueue(radius * 2 + 1);
			for (int i = 0; i < radius; i++)
			{
				maxQueue.Push(src[x * len2 + i] & srcmask);
			}
			for (int j = 0; j < len2; j++)
			{
				if (j > radius)
				{
					maxQueue.Pop();
				}
				if (j < len2 - radius)
				{
					maxQueue.Push(src[x * len2 + j + radius] & srcmask);
				}
				if (maxQueue.Max != 0)
				{
					action(x, j);
				}
			}
		});
		Parallel.For(0, len2, (int y) => {
			MaxQueue maxQueue = new MaxQueue(radius * 2 + 1);
			for (int i = 0; i < radius; i++)
			{
				maxQueue.Push(src[i * len2 + y] & srcmask);
			}
			for (int j = 0; j < len1; j++)
			{
				if (j > radius)
				{
					maxQueue.Pop();
				}
				if (j < len1 - radius)
				{
					maxQueue.Push(src[(j + radius) * len2 + y] & srcmask);
				}
				if (maxQueue.Max != 0)
				{
					action(j, y);
				}
			}
		});
	}

	public static void FloodFill2D(int x, int y, int[] data, int len1, int len2, int mask_any, int mask_not, Func<int, int> action)
	{
		int i;
		Stack<KeyValuePair<int, int>> keyValuePairs = new Stack<KeyValuePair<int, int>>();
		keyValuePairs.Push(new KeyValuePair<int, int>(x, y));
	Label0:
		while (keyValuePairs.Count > 0)
		{
			KeyValuePair<int, int> keyValuePair = keyValuePairs.Pop();
			x = keyValuePair.Key;
			y = keyValuePair.Value;
			for (i = y; i >= 0; i--)
			{
				int num = data[x * len2 + i];
				if (((num & mask_any) == 0 ? true : (num & mask_not) != 0))
				{
					break;
				}
			}
			i++;
			int num1 = 0;
			bool flag = (bool)num1;
			bool flag1 = (bool)num1;
			while (i < len2)
			{
				int num2 = data[x * len2 + i];
				if (((num2 & mask_any) == 0 ? true : (num2 & mask_not) != 0))
				{
					goto Label0;
				}
				data[x * len2 + i] = action(num2);
				if (x > 0)
				{
					int num3 = data[(x - 1) * len2 + i];
					bool flag2 = ((num3 & mask_any) == 0 ? false : (num3 & mask_not) == 0);
					if (!flag1 & flag2)
					{
						keyValuePairs.Push(new KeyValuePair<int, int>(x - 1, i));
						flag1 = true;
					}
					else if (flag1 && !flag2)
					{
						flag1 = false;
					}
				}
				if (x < len1 - 1)
				{
					int num4 = data[(x + 1) * len2 + i];
					bool flag3 = ((num4 & mask_any) == 0 ? false : (num4 & mask_not) == 0);
					if (!flag & flag3)
					{
						keyValuePairs.Push(new KeyValuePair<int, int>(x + 1, i));
						flag = true;
					}
					else if (flag && !flag3)
					{
						flag = false;
					}
				}
				i++;
			}
		}
	}

	public static void GaussianBlur2D(float[] data, int len1, int len2, int iterations = 1)
	{
		float[] singleArray = data;
		float[] singleArray1 = new float[len1 * len2];
		for (int i = 0; i < iterations; i++)
		{
			for (int j = 0; j < len1; j++)
			{
				int num = Mathf.Max(0, j - 1);
				int num1 = Mathf.Min(len1 - 1, j + 1);
				for (int k = 0; k < len2; k++)
				{
					int num2 = Mathf.Max(0, k - 1);
					int num3 = Mathf.Min(len2 - 1, k + 1);
					float single = singleArray[j * len2 + k] * 4f + singleArray[j * len2 + num2] + singleArray[j * len2 + num3] + singleArray[num * len2 + k] + singleArray[num1 * len2 + k];
					singleArray1[j * len2 + k] = single * 0.125f;
				}
			}
			GenericsUtil.Swap<float[]>(ref singleArray, ref singleArray1);
		}
		if (singleArray != data)
		{
			Buffer.BlockCopy(singleArray, 0, data, 0, (int)data.Length * 4);
		}
	}

	public static void GaussianBlur2D(float[] data, int len1, int len2, int len3, int iterations = 1)
	{
		Action<int> action = null;
		float[] singleArray = new float[len1 * len2 * len3];
		for (int i1 = 0; i1 < iterations; i1++)
		{
			int num4 = len1;
			Action<int> action1 = action;
			if (action1 == null)
			{
				Action<int> action2 = (int x) => {
					int num = Mathf.Max(0, x - 1);
					int num1 = Mathf.Min(len1 - 1, x + 1);
					for (int i = 0; i < len2; i++)
					{
						int num2 = Mathf.Max(0, i - 1);
						int num3 = Mathf.Min(len2 - 1, i + 1);
						for (int j = 0; j < len3; j++)
						{
							float single = data[(x * len2 + i) * len3 + j] * 4f + data[(x * len2 + num2) * len3 + j] + data[(x * len2 + num3) * len3 + j] + data[(num * len2 + i) * len3 + j] + data[(num1 * len2 + i) * len3 + j];
							singleArray[(x * len2 + i) * len3 + j] = single * 0.125f;
						}
					}
				};
				Action<int> action3 = action2;
				action = action2;
				action1 = action3;
			}
			Parallel.For(0, num4, action1);
			GenericsUtil.Swap<float[]>(ref data, ref singleArray);
		}
		if (data != data)
		{
			Buffer.BlockCopy(data, 0, data, 0, (int)data.Length * 4);
		}
	}

	public static bool IsValidPNG(byte[] data, int maxWidth, int maxHeight)
	{
		if ((int)data.Length < 24)
		{
			return false;
		}
		if ((int)data.Length > 24 + maxWidth * maxHeight * 4)
		{
			return false;
		}
		for (int i = 0; i < 8; i++)
		{
			if (data[i] != ImageProcessing.signature[i])
			{
				return false;
			}
		}
		Union32 union32 = new Union32()
		{
			b4 = data[16],
			b3 = data[17],
			b2 = data[18],
			b1 = data[19]
		};
		if (union32.i < 1 || union32.i > maxWidth)
		{
			return false;
		}
		Union32 union321 = new Union32()
		{
			b4 = data[20],
			b3 = data[21],
			b2 = data[22],
			b1 = data[23]
		};
		if (union321.i >= 1 && union321.i <= maxHeight)
		{
			return true;
		}
		return false;
	}

	public static void Upsample2D(float[] src, int srclen1, int srclen2, float[] dst, int dstlen1, int dstlen2)
	{
		if (2 * srclen1 != dstlen1 || 2 * srclen2 != dstlen2)
		{
			return;
		}
		Parallel.For(0, srclen1, (int x) => {
			int num = Mathf.Max(0, x - 1);
			int num1 = Mathf.Min(srclen1 - 1, x + 1);
			for (int i = 0; i < srclen2; i++)
			{
				int num2 = Mathf.Max(0, i - 1);
				int num3 = Mathf.Min(srclen2 - 1, i + 1);
				float single = src[x * srclen2 + i] * 6f;
				float single1 = single + src[num * srclen2 + i] + src[x * srclen2 + num2];
				dst[2 * x * dstlen2 + 2 * i] = single1 * 0.125f;
				float single2 = single + src[num1 * srclen2 + i] + src[x * srclen2 + num2];
				dst[(2 * x + 1) * dstlen2 + 2 * i] = single2 * 0.125f;
				float single3 = single + src[num * srclen2 + i] + src[x * srclen2 + num3];
				dst[2 * x * dstlen2 + 2 * i + 1] = single3 * 0.125f;
				float single4 = single + src[num1 * srclen2 + i] + src[x * srclen2 + num3];
				dst[(2 * x + 1) * dstlen2 + 2 * i + 1] = single4 * 0.125f;
			}
		});
	}

	public static void Upsample2D(float[] src, int srclen1, int srclen2, int srclen3, float[] dst, int dstlen1, int dstlen2, int dstlen3)
	{
		if (2 * srclen1 != dstlen1 || 2 * srclen2 != dstlen2 || srclen3 != dstlen3)
		{
			return;
		}
		Parallel.For(0, srclen1, (int x) => {
			int num = Mathf.Max(0, x - 1);
			int num1 = Mathf.Min(srclen1 - 1, x + 1);
			for (int i = 0; i < srclen2; i++)
			{
				int num2 = Mathf.Max(0, i - 1);
				int num3 = Mathf.Min(srclen2 - 1, i + 1);
				for (int j = 0; j < srclen3; j++)
				{
					float single = src[(x * srclen2 + i) * srclen3 + j] * 6f;
					float single1 = single + src[(num * srclen2 + i) * srclen3 + j] + src[(x * srclen2 + num2) * srclen3 + j];
					dst[(2 * x * dstlen2 + 2 * i) * dstlen3 + j] = single1 * 0.125f;
					float single2 = single + src[(num1 * srclen2 + i) * srclen3 + j] + src[(x * srclen2 + num2) * srclen3 + j];
					dst[((2 * x + 1) * dstlen2 + 2 * i) * dstlen3 + j] = single2 * 0.125f;
					float single3 = single + src[(num * srclen2 + i) * srclen3 + j] + src[(x * srclen2 + num3) * srclen3 + j];
					dst[(2 * x * dstlen2 + 2 * i + 1) * dstlen3 + j] = single3 * 0.125f;
					float single4 = single + src[(num1 * srclen2 + i) * srclen3 + j] + src[(x * srclen2 + num3) * srclen3 + j];
					dst[((2 * x + 1) * dstlen2 + 2 * i + 1) * dstlen3 + j] = single4 * 0.125f;
				}
			}
		});
	}
}