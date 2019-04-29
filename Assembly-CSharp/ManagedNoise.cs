using System;

public static class ManagedNoise
{
	private readonly static int[] hash;

	private const int hashMask = 255;

	private const double sqrt2 = 1.4142135623731;

	private const double rsqrt2 = 0.707106781186548;

	private const double squaresToTriangles = 0.211324865405187;

	private const double trianglesToSquares = 0.366025403784439;

	private const double simplexScale1D = 2.40740740740741;

	private const double simplexScale2D = 32.9907739830396;

	private const double gradientScale2D = 4;

	private static double[] gradients1D;

	private const int gradientsMask1D = 1;

	private static double[] gradients2Dx;

	private static double[] gradients2Dy;

	private const int gradientsMask2D = 7;

	static ManagedNoise()
	{
		ManagedNoise.hash = new int[] { 151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180, 151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180 };
		ManagedNoise.gradients1D = new double[] { 1, -1 };
		ManagedNoise.gradients2Dx = new double[] { 1, -1, 0, 0, 0.707106781186548, -0.707106781186548, 0.707106781186548, -0.707106781186548 };
		ManagedNoise.gradients2Dy = new double[] { 0, 0, 1, -1, 0.707106781186548, 0.707106781186548, -0.707106781186548, -0.707106781186548 };
	}

	private static double Abs(double x)
	{
		if (x >= 0)
		{
			return x;
		}
		return -x;
	}

	public static double Billow(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain)
	{
		x *= frequency;
		y *= frequency;
		double num = 0;
		double num1 = 1;
		double num2 = 1;
		for (int i = 0; i < octaves; i++)
		{
			double num3 = ManagedNoise.Simplex2D(x * num1, y * num1);
			num = num + num2 * ManagedNoise.Abs(num3);
			num1 *= lacunarity;
			num2 *= gain;
		}
		return num * amplitude;
	}

	public static double BillowIQ(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain)
	{
		double num;
		double num1;
		x *= frequency;
		y *= frequency;
		double num2 = 0;
		double num3 = 1;
		double num4 = 1;
		double num5 = 0;
		double num6 = 0;
		for (int i = 0; i < octaves; i++)
		{
			double num7 = ManagedNoise.Simplex2D(x * num3, y * num3, out num, out num1);
			num5 += num;
			num6 += num1;
			num2 = num2 + num4 * ManagedNoise.Abs(num7) / (1 + (num5 * num5 + num6 * num6));
			num3 *= lacunarity;
			num4 *= gain;
		}
		return num2 * amplitude;
	}

	public static double BillowWarp(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain, double warp)
	{
		double num;
		double num1;
		x *= frequency;
		y *= frequency;
		double num2 = 0;
		double num3 = 1;
		double num4 = 1;
		double num5 = 0;
		double num6 = 0;
		for (int i = 0; i < octaves; i++)
		{
			double num7 = ManagedNoise.Simplex2D((x + warp * num5) * num3, (y + warp * num6) * num3, out num, out num1);
			num2 = num2 + num4 * ManagedNoise.Abs(num7);
			num5 = num5 + num4 * num * -num7;
			num6 = num6 + num4 * num1 * -num7;
			num3 *= lacunarity;
			num4 = num4 * (gain * ManagedNoise.Saturate(num2));
		}
		return num2 * amplitude;
	}

	private static int Floor(double x)
	{
		if (x >= 0)
		{
			return (int)x;
		}
		return (int)x - 1;
	}

	public static double Jordan(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain, double warp, double damp, double damp_scale)
	{
		double num;
		double num1;
		x *= frequency;
		y *= frequency;
		double num2 = 0;
		double num3 = 1;
		double num4 = 1;
		double num5 = 0;
		double num6 = 0;
		double num7 = 0;
		double num8 = 0;
		double dampScale = num3 * gain;
		for (int i = 0; i < octaves; i++)
		{
			double num9 = ManagedNoise.Simplex2D(x * num4 + num5, y * num4 + num6, out num, out num1);
			double num10 = num9 * num9;
			double num11 = num * num9;
			double num12 = num1 * num9;
			num2 = num2 + dampScale * num10;
			num5 = num5 + warp * num11;
			num6 = num6 + warp * num12;
			num7 = num7 + damp * num11;
			num8 = num8 + damp * num12;
			num4 *= lacunarity;
			num3 *= gain;
			dampScale = num3 * (1 - damp_scale / (1 + (num7 * num7 + num8 * num8)));
		}
		return num2 * amplitude;
	}

	public static double Ridge(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain)
	{
		x *= frequency;
		y *= frequency;
		double num = 0;
		double num1 = 1;
		double num2 = 1;
		for (int i = 0; i < octaves; i++)
		{
			double num3 = ManagedNoise.Simplex2D(x * num1, y * num1);
			num = num + num2 * (1 - ManagedNoise.Abs(num3));
			num1 *= lacunarity;
			num2 *= gain;
		}
		return num * amplitude;
	}

	public static double RidgeIQ(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain)
	{
		double num;
		double num1;
		x *= frequency;
		y *= frequency;
		double num2 = 0;
		double num3 = 1;
		double num4 = 1;
		double num5 = 0;
		double num6 = 0;
		for (int i = 0; i < octaves; i++)
		{
			double num7 = ManagedNoise.Simplex2D(x * num3, y * num3, out num, out num1);
			num5 += num;
			num6 += num1;
			num2 = num2 + num4 * (1 - ManagedNoise.Abs(num7)) / (1 + (num5 * num5 + num6 * num6));
			num3 *= lacunarity;
			num4 *= gain;
		}
		return num2 * amplitude;
	}

	public static double RidgeWarp(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain, double warp)
	{
		double num;
		double num1;
		x *= frequency;
		y *= frequency;
		double num2 = 0;
		double num3 = 1;
		double num4 = 1;
		double num5 = 0;
		double num6 = 0;
		for (int i = 0; i < octaves; i++)
		{
			double num7 = ManagedNoise.Simplex2D((x + warp * num5) * num3, (y + warp * num6) * num3, out num, out num1);
			num2 = num2 + num4 * (1 - ManagedNoise.Abs(num7));
			num5 = num5 + num4 * num * -num7;
			num6 = num6 + num4 * num1 * -num7;
			num3 *= lacunarity;
			num4 = num4 * (gain * ManagedNoise.Saturate(num2));
		}
		return num2 * amplitude;
	}

	private static double Saturate(double x)
	{
		if (x > 1)
		{
			return 1;
		}
		if (x >= 0)
		{
			return x;
		}
		return 0;
	}

	public static double Sharp(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain)
	{
		x *= frequency;
		y *= frequency;
		double num = 0;
		double num1 = 1;
		double num2 = 1;
		for (int i = 0; i < octaves; i++)
		{
			double num3 = ManagedNoise.Simplex2D(x * num1, y * num1);
			num = num + num2 * (num3 * num3);
			num1 *= lacunarity;
			num2 *= gain;
		}
		return num * amplitude;
	}

	public static double SharpIQ(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain)
	{
		double num;
		double num1;
		x *= frequency;
		y *= frequency;
		double num2 = 0;
		double num3 = 1;
		double num4 = 1;
		double num5 = 0;
		double num6 = 0;
		for (int i = 0; i < octaves; i++)
		{
			double num7 = ManagedNoise.Simplex2D(x * num3, y * num3, out num, out num1);
			num5 += num;
			num6 += num1;
			num2 = num2 + num4 * (num7 * num7) / (1 + (num5 * num5 + num6 * num6));
			num3 *= lacunarity;
			num4 *= gain;
		}
		return num2 * amplitude;
	}

	public static double SharpWarp(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain, double warp)
	{
		double num;
		double num1;
		x *= frequency;
		y *= frequency;
		double num2 = 0;
		double num3 = 1;
		double num4 = 1;
		double num5 = 0;
		double num6 = 0;
		for (int i = 0; i < octaves; i++)
		{
			double num7 = ManagedNoise.Simplex2D((x + warp * num5) * num3, (y + warp * num6) * num3, out num, out num1);
			num2 = num2 + num4 * (num7 * num7);
			num5 = num5 + num4 * num * -num7;
			num6 = num6 + num4 * num1 * -num7;
			num3 *= lacunarity;
			num4 = num4 * (gain * ManagedNoise.Saturate(num2));
		}
		return num2 * amplitude;
	}

	public static double Simplex1D(double x)
	{
		double num = 0;
		int num1 = ManagedNoise.Floor(x);
		int num2 = num1;
		double num3 = x - (double)num2;
		double num4 = 1 - num3 * num3;
		if (num4 > 0)
		{
			double num5 = num4 * (num4 * num4);
			int num6 = ManagedNoise.hash[num2 & 255] & 1;
			double num7 = ManagedNoise.gradients1D[num6] * num3;
			num = num + num7 * num5;
		}
		int num8 = num1 + 1;
		double num9 = x - (double)num8;
		double num10 = 1 - num9 * num9;
		if (num10 > 0)
		{
			double num11 = num10 * (num10 * num10);
			int num12 = ManagedNoise.hash[num8 & 255] & 1;
			double num13 = ManagedNoise.gradients1D[num12] * num9;
			num = num + num13 * num11;
		}
		return num * 2.40740740740741;
	}

	public static double Simplex1D(double x, out double dx)
	{
		double num = 0;
		dx = 0;
		int num1 = ManagedNoise.Floor(x);
		int num2 = num1;
		double num3 = x - (double)num2;
		double num4 = 1 - num3 * num3;
		if (num4 > 0)
		{
			double num5 = num4 * num4;
			double num6 = num4 * num5;
			int num7 = ManagedNoise.hash[num2 & 255] & 1;
			double num8 = ManagedNoise.gradients1D[num7];
			double num9 = num8 * num3;
			double num10 = num9 * 6 * num5;
			dx = dx + (num8 * num6 - num10 * num3);
			num = num + num9 * num6;
		}
		int num11 = num1 + 1;
		double num12 = x - (double)num11;
		double num13 = 1 - num12 * num12;
		if (num13 > 0)
		{
			double num14 = num13 * num13;
			double num15 = num13 * num14;
			int num16 = ManagedNoise.hash[num11 & 255] & 1;
			double num17 = ManagedNoise.gradients1D[num16];
			double num18 = num17 * num12;
			double num19 = num18 * 6 * num14;
			dx = dx + (num17 * num15 - num19 * num12);
			num = num + num18 * num15;
		}
		return num * 2.40740740740741;
	}

	public static double Simplex2D(double x, double y)
	{
		double num = 0;
		double num1 = (x + y) * 0.366025403784439;
		double num2 = x + num1;
		double num3 = y + num1;
		int num4 = ManagedNoise.Floor(num2);
		int num5 = ManagedNoise.Floor(num3);
		int num6 = num4;
		int num7 = num5;
		double num8 = (double)(num6 + num7) * 0.211324865405187;
		double num9 = x - (double)num6 + num8;
		double num10 = y - (double)num7 + num8;
		double num11 = 0.5 - num9 * num9 - num10 * num10;
		if (num11 > 0)
		{
			double num12 = num11 * (num11 * num11);
			int num13 = ManagedNoise.hash[ManagedNoise.hash[num6 & 255] + num7 & 255] & 7;
			double num14 = ManagedNoise.gradients2Dx[num13];
			double num15 = ManagedNoise.gradients2Dy[num13];
			double num16 = num14 * num9 + num15 * num10;
			num = num + num16 * num12;
		}
		int num17 = num4 + 1;
		int num18 = num5 + 1;
		double num19 = (double)(num17 + num18) * 0.211324865405187;
		double num20 = x - (double)num17 + num19;
		double num21 = y - (double)num18 + num19;
		double num22 = 0.5 - num20 * num20 - num21 * num21;
		if (num22 > 0)
		{
			double num23 = num22 * (num22 * num22);
			int num24 = ManagedNoise.hash[ManagedNoise.hash[num17 & 255] + num18 & 255] & 7;
			double num25 = ManagedNoise.gradients2Dx[num24];
			double num26 = ManagedNoise.gradients2Dy[num24];
			double num27 = num25 * num20 + num26 * num21;
			num = num + num27 * num23;
		}
		if (num2 - (double)num4 < num3 - (double)num5)
		{
			int num28 = num4;
			int num29 = num5 + 1;
			double num30 = (double)(num28 + num29) * 0.211324865405187;
			double num31 = x - (double)num28 + num30;
			double num32 = y - (double)num29 + num30;
			double num33 = 0.5 - num31 * num31 - num32 * num32;
			if (num33 > 0)
			{
				double num34 = num33 * (num33 * num33);
				int num35 = ManagedNoise.hash[ManagedNoise.hash[num28 & 255] + num29 & 255] & 7;
				double num36 = ManagedNoise.gradients2Dx[num35];
				double num37 = ManagedNoise.gradients2Dy[num35];
				double num38 = num36 * num31 + num37 * num32;
				num = num + num38 * num34;
			}
		}
		else
		{
			int num39 = num4 + 1;
			int num40 = num5;
			double num41 = (double)(num39 + num40) * 0.211324865405187;
			double num42 = x - (double)num39 + num41;
			double num43 = y - (double)num40 + num41;
			double num44 = 0.5 - num42 * num42 - num43 * num43;
			if (num44 > 0)
			{
				double num45 = num44 * (num44 * num44);
				int num46 = ManagedNoise.hash[ManagedNoise.hash[num39 & 255] + num40 & 255] & 7;
				double num47 = ManagedNoise.gradients2Dx[num46];
				double num48 = ManagedNoise.gradients2Dy[num46];
				double num49 = num47 * num42 + num48 * num43;
				num = num + num49 * num45;
			}
		}
		return num * 32.9907739830396;
	}

	public static double Simplex2D(double x, double y, out double dx, out double dy)
	{
		double num = 0;
		dx = 0;
		dy = 0;
		double num1 = (x + y) * 0.366025403784439;
		double num2 = x + num1;
		double num3 = y + num1;
		int num4 = ManagedNoise.Floor(num2);
		int num5 = ManagedNoise.Floor(num3);
		int num6 = num4;
		int num7 = num5;
		double num8 = (double)(num6 + num7) * 0.211324865405187;
		double num9 = x - (double)num6 + num8;
		double num10 = y - (double)num7 + num8;
		double num11 = 0.5 - num9 * num9 - num10 * num10;
		if (num11 > 0)
		{
			double num12 = num11 * num11;
			double num13 = num11 * num12;
			int num14 = ManagedNoise.hash[ManagedNoise.hash[num6 & 255] + num7 & 255] & 7;
			double num15 = ManagedNoise.gradients2Dx[num14];
			double num16 = ManagedNoise.gradients2Dy[num14];
			double num17 = num15 * num9 + num16 * num10;
			double num18 = num17 * 6 * num12;
			dx = dx + (num15 * num13 - num18 * num9);
			dy = dy + (num16 * num13 - num18 * num10);
			num = num + num17 * num13;
		}
		int num19 = num4 + 1;
		int num20 = num5 + 1;
		double num21 = (double)(num19 + num20) * 0.211324865405187;
		double num22 = x - (double)num19 + num21;
		double num23 = y - (double)num20 + num21;
		double num24 = 0.5 - num22 * num22 - num23 * num23;
		if (num24 > 0)
		{
			double num25 = num24 * num24;
			double num26 = num24 * num25;
			int num27 = ManagedNoise.hash[ManagedNoise.hash[num19 & 255] + num20 & 255] & 7;
			double num28 = ManagedNoise.gradients2Dx[num27];
			double num29 = ManagedNoise.gradients2Dy[num27];
			double num30 = num28 * num22 + num29 * num23;
			double num31 = num30 * 6 * num25;
			dx = dx + (num28 * num26 - num31 * num22);
			dy = dy + (num29 * num26 - num31 * num23);
			num = num + num30 * num26;
		}
		if (num2 - (double)num4 < num3 - (double)num5)
		{
			int num32 = num4;
			int num33 = num5 + 1;
			double num34 = (double)(num32 + num33) * 0.211324865405187;
			double num35 = x - (double)num32 + num34;
			double num36 = y - (double)num33 + num34;
			double num37 = 0.5 - num35 * num35 - num36 * num36;
			if (num37 > 0)
			{
				double num38 = num37 * num37;
				double num39 = num37 * num38;
				int num40 = ManagedNoise.hash[ManagedNoise.hash[num32 & 255] + num33 & 255] & 7;
				double num41 = ManagedNoise.gradients2Dx[num40];
				double num42 = ManagedNoise.gradients2Dy[num40];
				double num43 = num41 * num35 + num42 * num36;
				double num44 = num43 * 6 * num38;
				dx = dx + (num41 * num39 - num44 * num35);
				dy = dy + (num42 * num39 - num44 * num36);
				num = num + num43 * num39;
			}
		}
		else
		{
			int num45 = num4 + 1;
			int num46 = num5;
			double num47 = (double)(num45 + num46) * 0.211324865405187;
			double num48 = x - (double)num45 + num47;
			double num49 = y - (double)num46 + num47;
			double num50 = 0.5 - num48 * num48 - num49 * num49;
			if (num50 > 0)
			{
				double num51 = num50 * num50;
				double num52 = num50 * num51;
				int num53 = ManagedNoise.hash[ManagedNoise.hash[num45 & 255] + num46 & 255] & 7;
				double num54 = ManagedNoise.gradients2Dx[num53];
				double num55 = ManagedNoise.gradients2Dy[num53];
				double num56 = num54 * num48 + num55 * num49;
				double num57 = num56 * 6 * num51;
				dx = dx + (num54 * num52 - num57 * num48);
				dy = dy + (num55 * num52 - num57 * num49);
				num = num + num56 * num52;
			}
		}
		dx *= 4;
		dy *= 4;
		return num * 32.9907739830396;
	}

	public static double Turbulence(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain)
	{
		x *= frequency;
		y *= frequency;
		double num = 0;
		double num1 = 1;
		double num2 = 1;
		for (int i = 0; i < octaves; i++)
		{
			double num3 = ManagedNoise.Simplex2D(x * num1, y * num1);
			num = num + num2 * num3;
			num1 *= lacunarity;
			num2 *= gain;
		}
		return num * amplitude;
	}

	public static double TurbulenceIQ(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain)
	{
		double num;
		double num1;
		x *= frequency;
		y *= frequency;
		double num2 = 0;
		double num3 = 1;
		double num4 = 1;
		double num5 = 0;
		double num6 = 0;
		for (int i = 0; i < octaves; i++)
		{
			double num7 = ManagedNoise.Simplex2D(x * num3, y * num3, out num, out num1);
			num5 += num;
			num6 += num1;
			num2 = num2 + num4 * num7 / (1 + (num5 * num5 + num6 * num6));
			num3 *= lacunarity;
			num4 *= gain;
		}
		return num2 * amplitude;
	}

	public static double TurbulenceWarp(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain, double warp)
	{
		double num;
		double num1;
		x *= frequency;
		y *= frequency;
		double num2 = 0;
		double num3 = 1;
		double num4 = 1;
		double num5 = 0;
		double num6 = 0;
		for (int i = 0; i < octaves; i++)
		{
			double num7 = ManagedNoise.Simplex2D((x + warp * num5) * num3, (y + warp * num6) * num3, out num, out num1);
			num2 = num2 + num4 * num7;
			num5 = num5 + num4 * num * -num7;
			num6 = num6 + num4 * num1 * -num7;
			num3 *= lacunarity;
			num4 = num4 * (gain * ManagedNoise.Saturate(num2));
		}
		return num2 * amplitude;
	}
}