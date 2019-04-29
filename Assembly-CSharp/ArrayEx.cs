using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ArrayEx
{
	public static void BubbleSort<T>(this T[] array)
	where T : IComparable<T>
	{
		for (int i = 1; i < (int)array.Length; i++)
		{
			T t = array[i];
			for (int j = i - 1; j >= 0; j--)
			{
				T t1 = array[j];
				if (t.CompareTo(t1) >= 0)
				{
					break;
				}
				array[j + 1] = t1;
				array[j] = t;
			}
		}
	}

	public static T GetRandom<T>(this T[] array)
	{
		if (array == null || array.Length == 0)
		{
			return default(T);
		}
		return array[UnityEngine.Random.Range(0, (int)array.Length)];
	}

	public static T GetRandom<T>(this T[] array, uint seed)
	{
		if (array == null || array.Length == 0)
		{
			return default(T);
		}
		return array[SeedRandom.Range(ref seed, 0, (int)array.Length)];
	}

	public static T GetRandom<T>(this T[] array, ref uint seed)
	{
		if (array == null || array.Length == 0)
		{
			return default(T);
		}
		return array[SeedRandom.Range(ref seed, 0, (int)array.Length)];
	}

	public static void Shuffle<T>(this T[] array, uint seed)
	{
		array.Shuffle<T>(ref seed);
	}

	public static void Shuffle<T>(this T[] array, ref uint seed)
	{
		for (int i = 0; i < (int)array.Length; i++)
		{
			int num = SeedRandom.Range(ref seed, 0, (int)array.Length);
			int num1 = SeedRandom.Range(ref seed, 0, (int)array.Length);
			T t = array[num];
			array[num] = array[num1];
			array[num1] = t;
		}
	}
}