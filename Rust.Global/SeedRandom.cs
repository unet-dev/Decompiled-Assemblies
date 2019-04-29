using System;
using UnityEngine;

public class SeedRandom
{
	public uint Seed;

	public SeedRandom(uint seed = 0)
	{
		if (seed != 0)
		{
			this.Seed = seed;
			return;
		}
		this.Seed = (uint)UnityEngine.Random.Range(1, 2147483647);
	}

	public int Range(int min, int max)
	{
		return SeedRandom.Range(ref this.Seed, min, max);
	}

	public static int Range(uint seed, int min, int max)
	{
		return SeedRandom.Range(ref seed, min, max);
	}

	public static int Range(ref uint seed, int min, int max)
	{
		uint num = (uint)(max - min);
		return (int)(min + SeedRandom.Xorshift(ref seed) % num);
	}

	public float Range(float min, float max)
	{
		return SeedRandom.Range(ref this.Seed, min, max);
	}

	public static float Range(uint seed, float min, float max)
	{
		return SeedRandom.Range(ref seed, min, max);
	}

	public static float Range(ref uint seed, float min, float max)
	{
		return min + SeedRandom.Xorshift01(ref seed) * (max - min);
	}

	public int Sign()
	{
		if (SeedRandom.Xorshift(ref this.Seed) % 2 != 0)
		{
			return -1;
		}
		return 1;
	}

	public static int Sign(uint seed)
	{
		if (SeedRandom.Xorshift(ref seed) % 2 != 0)
		{
			return -1;
		}
		return 1;
	}

	public static int Sign(ref uint seed)
	{
		if (SeedRandom.Xorshift(ref seed) % 2 != 0)
		{
			return -1;
		}
		return 1;
	}

	public float Value()
	{
		return SeedRandom.Xorshift01(ref this.Seed);
	}

	public static float Value(uint seed)
	{
		return SeedRandom.Xorshift01(ref seed);
	}

	public static float Value(ref uint seed)
	{
		return SeedRandom.Xorshift01(ref seed);
	}

	public Vector2 Value2D()
	{
		return SeedRandom.Value2D(ref this.Seed);
	}

	public static Vector2 Value2D(uint seed)
	{
		return SeedRandom.Value2D(ref seed);
	}

	public static Vector2 Value2D(ref uint seed)
	{
		float single = SeedRandom.Value(ref seed) * 3.14159274f * 2f;
		return new Vector2(Mathf.Cos(single), Mathf.Sin(single));
	}

	public static uint Wanghash(ref uint x)
	{
		x = x ^ 61 ^ x >> 16;
		x *= 9;
		x = x ^ x >> 4;
		x *= 668265261;
		x = x ^ x >> 15;
		return x;
	}

	public static float Wanghash01(ref uint x)
	{
		return (float)((float)SeedRandom.Wanghash(ref x)) * 2.32830644E-10f;
	}

	public static uint Xorshift(ref uint x)
	{
		x = x ^ x << 13;
		x = x ^ x >> 17;
		x = x ^ x << 5;
		return x;
	}

	public static float Xorshift01(ref uint x)
	{
		return (float)((float)SeedRandom.Xorshift(ref x)) * 2.32830644E-10f;
	}
}