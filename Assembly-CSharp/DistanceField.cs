using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DistanceField
{
	private readonly static int[] GaussOffsets;

	private readonly static float[] GaussWeights;

	static DistanceField()
	{
		DistanceField.GaussOffsets = new int[] { -6, -4, -2, 0, 2, 4, 6 };
		DistanceField.GaussWeights = new float[] { 0.03125f, 0.109375f, 0.21875f, 0.28125f, 0.21875f, 0.109375f, 0.03125f };
	}

	public DistanceField()
	{
	}

	public static void ApplyGaussianBlur(int size, float[] distanceField, int steps = 1)
	{
		if (steps > 0)
		{
			float[] singleArray = new float[size * size];
			int num = size - 1;
			for (int i = 0; i < steps; i++)
			{
				int num1 = 0;
				int num2 = 0;
				int num3 = 0;
				while (num1 < size)
				{
					int num4 = 0;
					while (num4 < size)
					{
						float single = 0f;
						for (int j = 0; j < 7; j++)
						{
							int gaussOffsets = num4 + DistanceField.GaussOffsets[j];
							gaussOffsets = (gaussOffsets >= 0 ? gaussOffsets : 0);
							gaussOffsets = (gaussOffsets <= num ? gaussOffsets : num);
							single = single + distanceField[num3 + gaussOffsets] * DistanceField.GaussWeights[j];
						}
						singleArray[num2] = single;
						num4++;
						num2++;
					}
					num1++;
					num3 += size;
				}
				int num5 = 0;
				int num6 = 0;
				while (num5 < size)
				{
					int num7 = 0;
					while (num7 < size)
					{
						float gaussWeights = 0f;
						for (int k = 0; k < 7; k++)
						{
							int gaussOffsets1 = num5 + DistanceField.GaussOffsets[k];
							gaussOffsets1 = (gaussOffsets1 >= 0 ? gaussOffsets1 : 0);
							gaussOffsets1 = (gaussOffsets1 <= num ? gaussOffsets1 : num);
							gaussWeights = gaussWeights + singleArray[gaussOffsets1 * size + num7] * DistanceField.GaussWeights[k];
						}
						distanceField[num6] = gaussWeights;
						num7++;
						num6++;
					}
					num5++;
				}
			}
		}
	}

	public static void Generate([In][IsReadOnly] ref int size, [In][IsReadOnly] ref byte threshold, [In][IsReadOnly] ref byte[] image, ref float[] distanceField)
	{
		int num;
		float single;
		int num1 = size + 2;
		int[] numArray = new int[num1 * num1];
		int[] numArray1 = new int[num1 * num1];
		float[] singleArray = new float[num1 * num1];
		int num2 = 0;
		int num3 = 0;
		while (num2 < num1)
		{
			int num4 = 0;
			while (num4 < num1)
			{
				numArray[num3] = -1;
				numArray1[num3] = -1;
				singleArray[num3] = Single.PositiveInfinity;
				num4++;
				num3++;
			}
			num2++;
		}
		int num5 = 1;
		int num6 = num5 * size;
		int num7 = num5 * num1;
		while (num5 < size - 2)
		{
			int num8 = 1;
			int num9 = num6 + num8;
			int num10 = num7 + num8;
			while (num8 < size - 2)
			{
				int num11 = num10 + num1 + 1;
				bool flag = image[num9] > threshold;
				if (flag && (image[num9 - 1] > threshold != flag || image[num9 + 1] > threshold != flag || image[num9 - size] > threshold != flag || image[num9 + size] > threshold != flag))
				{
					numArray[num11] = num8 + 1;
					numArray1[num11] = num5 + 1;
					singleArray[num11] = 0f;
				}
				num8++;
				num9++;
				num10++;
			}
			num5++;
			num6 += size;
			num7 += num1;
		}
		int num12 = 1;
		int num13 = num12 * num1;
		while (num12 < num1 - 1)
		{
			int num14 = 1;
			int num15 = num13 + num14;
			while (num14 < num1 - 1)
			{
				int num16 = num15 - 1;
				int num17 = num15 - num1;
				int num18 = num17 - 1;
				int num19 = num17 + 1;
				float single1 = singleArray[num15];
				if (singleArray[num18] + 1.41421354f < single1)
				{
					int num20 = numArray[num18];
					num = num20;
					numArray[num15] = num20;
					int num21 = num;
					int num22 = numArray1[num18];
					num = num22;
					numArray1[num15] = num22;
					int num23 = num;
					float single2 = Vector2Ex.Length((float)(num14 - num21), (float)(num12 - num23));
					single = single2;
					singleArray[num15] = single2;
					single1 = single;
				}
				if (singleArray[num17] + 1f < single1)
				{
					int num24 = numArray[num17];
					num = num24;
					numArray[num15] = num24;
					int num25 = num;
					int num26 = numArray1[num17];
					num = num26;
					numArray1[num15] = num26;
					int num27 = num;
					float single3 = Vector2Ex.Length((float)(num14 - num25), (float)(num12 - num27));
					single = single3;
					singleArray[num15] = single3;
					single1 = single;
				}
				if (singleArray[num19] + 1.41421354f < single1)
				{
					int num28 = numArray[num19];
					num = num28;
					numArray[num15] = num28;
					int num29 = num;
					int num30 = numArray1[num19];
					num = num30;
					numArray1[num15] = num30;
					int num31 = num;
					float single4 = Vector2Ex.Length((float)(num14 - num29), (float)(num12 - num31));
					single = single4;
					singleArray[num15] = single4;
					single1 = single;
				}
				if (singleArray[num16] + 1f < single1)
				{
					int num32 = numArray[num16];
					num = num32;
					numArray[num15] = num32;
					int num33 = num;
					int num34 = numArray1[num16];
					num = num34;
					numArray1[num15] = num34;
					int num35 = num;
					float single5 = Vector2Ex.Length((float)(num14 - num33), (float)(num12 - num35));
					single = single5;
					singleArray[num15] = single5;
					single1 = single;
				}
				num14++;
				num15++;
			}
			num12++;
			num13 += num1;
		}
		int num36 = num1 - 2;
		int num37 = num36 * num1;
		while (num36 >= 1)
		{
			int num38 = num1 - 2;
			int num39 = num37 + num38;
			while (num38 >= 1)
			{
				int num40 = num39 + 1;
				int num41 = num39 + num1;
				int num42 = num41 - 1;
				int num43 = num41 + 1;
				float single6 = singleArray[num39];
				if (singleArray[num40] + 1f < single6)
				{
					int num44 = numArray[num40];
					num = num44;
					numArray[num39] = num44;
					int num45 = num;
					int num46 = numArray1[num40];
					num = num46;
					numArray1[num39] = num46;
					int num47 = num;
					float single7 = Vector2Ex.Length((float)(num38 - num45), (float)(num36 - num47));
					single = single7;
					singleArray[num39] = single7;
					single6 = single;
				}
				if (singleArray[num42] + 1.41421354f < single6)
				{
					int num48 = numArray[num42];
					num = num48;
					numArray[num39] = num48;
					int num49 = num;
					int num50 = numArray1[num42];
					num = num50;
					numArray1[num39] = num50;
					int num51 = num;
					float single8 = Vector2Ex.Length((float)(num38 - num49), (float)(num36 - num51));
					single = single8;
					singleArray[num39] = single8;
					single6 = single;
				}
				if (singleArray[num41] + 1f < single6)
				{
					int num52 = numArray[num41];
					num = num52;
					numArray[num39] = num52;
					int num53 = num;
					int num54 = numArray1[num41];
					num = num54;
					numArray1[num39] = num54;
					int num55 = num;
					float single9 = Vector2Ex.Length((float)(num38 - num53), (float)(num36 - num55));
					single = single9;
					singleArray[num39] = single9;
					single6 = single;
				}
				if (singleArray[num43] + 1f < single6)
				{
					int num56 = numArray[num43];
					num = num56;
					numArray[num39] = num56;
					int num57 = num;
					int num58 = numArray1[num43];
					num = num58;
					numArray1[num39] = num58;
					int num59 = num;
					float single10 = Vector2Ex.Length((float)(num38 - num57), (float)(num36 - num59));
					single = single10;
					singleArray[num39] = single10;
					single6 = single;
				}
				num38--;
				num39--;
			}
			num36--;
			num37 -= num1;
		}
		int num60 = 0;
		int num61 = 0;
		int num62 = num1;
		while (num60 < size)
		{
			int num63 = 0;
			int num64 = num62 + 1;
			while (num63 < size)
			{
				distanceField[num61] = (image[num61] > threshold ? -singleArray[num64] : singleArray[num64]);
				num63++;
				num61++;
				num64++;
			}
			num60++;
			num62 += num1;
		}
	}

	public static void GenerateVectors([In][IsReadOnly] ref int size, [In][IsReadOnly] ref float[] distanceField, ref Vector2[] vectorField)
	{
		for (int i = 1; i < size - 1; i++)
		{
			for (int j = 1; j < size - 1; j++)
			{
				float single = DistanceField.SampleClamped(distanceField, size, i - 1, j - 1);
				float single1 = DistanceField.SampleClamped(distanceField, size, i - 1, j);
				float single2 = DistanceField.SampleClamped(distanceField, size, i - 1, j + 1);
				float single3 = DistanceField.SampleClamped(distanceField, size, i, j - 1);
				float single4 = DistanceField.SampleClamped(distanceField, size, i, j + 1);
				float single5 = DistanceField.SampleClamped(distanceField, size, i + 1, j - 1);
				float single6 = DistanceField.SampleClamped(distanceField, size, i + 1, j);
				float single7 = DistanceField.SampleClamped(distanceField, size, i + 1, j + 1);
				float single8 = single2 + 2f * single4 + single7 - (single + 2f * single3 + single5);
				Vector2 vector2 = new Vector2(-(single5 + 2f * single6 + single7 - (single + 2f * single1 + single2)), -single8);
				Vector2 vector21 = vector2.normalized;
				vectorField[j * size + i] = new Vector2(vector21.x, vector21.y);
			}
		}
		for (int k = 1; k < size - 1; k++)
		{
			vectorField[k] = DistanceField.SampleClamped(vectorField, size, k, 1);
			vectorField[(size - 1) * size + k] = DistanceField.SampleClamped(vectorField, size, k, size - 2);
		}
		for (int l = 0; l < size; l++)
		{
			vectorField[l * size] = DistanceField.SampleClamped(vectorField, size, 1, l);
			vectorField[l * size + size - 1] = DistanceField.SampleClamped(vectorField, size, size - 2, l);
		}
	}

	private static float SampleClamped(float[] data, int size, int x, int y)
	{
		x = (x < 0 ? 0 : x);
		y = (y < 0 ? 0 : y);
		x = (x >= size ? size - 1 : x);
		y = (y >= size ? size - 1 : y);
		return data[y * size + x];
	}

	private static Vector2 SampleClamped(Vector2[] data, int size, int x, int y)
	{
		x = (x < 0 ? 0 : x);
		y = (y < 0 ? 0 : y);
		x = (x >= size ? size - 1 : x);
		y = (y >= size ? size - 1 : y);
		return data[y * size + x];
	}
}