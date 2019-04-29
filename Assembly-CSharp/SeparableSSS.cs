using System;
using UnityEngine;

public class SeparableSSS
{
	public SeparableSSS()
	{
	}

	public static void CalculateKernel(Color[] target, int targetStart, int targetSize, Color subsurfaceColor, Color falloffColor)
	{
		float single;
		int num = targetSize;
		int num1 = num * 2 - 1;
		float single1 = (num1 > 20 ? 3f : 2f);
		float single2 = 2f;
		Color[] colorArray = new Color[num1];
		float single3 = 2f * single1 / (float)(num1 - 1);
		for (int i = 0; i < num1; i++)
		{
			float single4 = -single1 + (float)i * single3;
			float single5 = (single4 < 0f ? -1f : 1f);
			colorArray[i].a = single1 * single5 * Mathf.Abs(Mathf.Pow(single4, single2)) / Mathf.Pow(single1, single2);
		}
		for (int j = 0; j < num1; j++)
		{
			single = (j > 0 ? Mathf.Abs(colorArray[j].a - colorArray[j - 1].a) : 0f);
			float single6 = (j < num1 - 1 ? Mathf.Abs(colorArray[j].a - colorArray[j + 1].a) : 0f);
			Vector3 vector3 = (single + single6) / 2f * SeparableSSS.Profile(colorArray[j].a, falloffColor);
			colorArray[j].r = vector3.x;
			colorArray[j].g = vector3.y;
			colorArray[j].b = vector3.z;
		}
		Color color = colorArray[num1 / 2];
		for (int k = num1 / 2; k > 0; k--)
		{
			colorArray[k] = colorArray[k - 1];
		}
		colorArray[0] = color;
		Vector3 vector31 = Vector3.zero;
		for (int l = 0; l < num1; l++)
		{
			vector31.x += colorArray[l].r;
			vector31.y += colorArray[l].g;
			vector31.z += colorArray[l].b;
		}
		for (int m = 0; m < num1; m++)
		{
			colorArray[m].r /= vector31.x;
			colorArray[m].g /= vector31.y;
			colorArray[m].b /= vector31.z;
		}
		target[targetStart] = colorArray[0];
		for (uint n = 0; (ulong)n < (long)(num - 1); n++)
		{
			target[checked((IntPtr)((long)targetStart + (ulong)n + (long)1))] = colorArray[checked((IntPtr)((long)num + (ulong)n))];
		}
	}

	private static Vector3 Gaussian(float variance, float r, Color falloffColor)
	{
		Vector3 vector3 = Vector3.zero;
		for (int i = 0; i < 3; i++)
		{
			float single = r / (0.001f + falloffColor[i]);
			vector3[i] = Mathf.Exp(-(single * single) / (2f * variance)) / (6.28f * variance);
		}
		return vector3;
	}

	private static Vector3 Profile(float r, Color falloffColor)
	{
		return ((((0.1f * SeparableSSS.Gaussian(0.0484f, r, falloffColor)) + (0.118f * SeparableSSS.Gaussian(0.187f, r, falloffColor))) + (0.113f * SeparableSSS.Gaussian(0.567f, r, falloffColor))) + (0.358f * SeparableSSS.Gaussian(1.99f, r, falloffColor))) + (0.078f * SeparableSSS.Gaussian(7.41f, r, falloffColor));
	}
}