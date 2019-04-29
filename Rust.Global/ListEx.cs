using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ListEx
{
	public static void BubbleSort<T>(this List<T> list)
	where T : IComparable<T>
	{
		for (int i = 1; i < list.Count; i++)
		{
			T item = list[i];
			for (int j = i - 1; j >= 0; j--)
			{
				T t = list[j];
				if (item.CompareTo(t) >= 0)
				{
					break;
				}
				list[j + 1] = t;
				list[j] = item;
			}
		}
	}

	public static T GetRandom<T>(this List<T> list)
	{
		if (list == null || list.Count == 0)
		{
			return default(T);
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public static T GetRandom<T>(this List<T> list, uint seed)
	{
		if (list == null || list.Count == 0)
		{
			return default(T);
		}
		return list[SeedRandom.Range(ref seed, 0, list.Count)];
	}

	public static T GetRandom<T>(this List<T> list, ref uint seed)
	{
		if (list == null || list.Count == 0)
		{
			return default(T);
		}
		return list[SeedRandom.Range(ref seed, 0, list.Count)];
	}

	public static void RemoveUnordered<T>(this List<T> list, int index)
	{
		list[index] = list[list.Count - 1];
		list.RemoveAt(list.Count - 1);
	}

	public static void Shuffle<T>(this List<T> list, uint seed)
	{
		list.Shuffle<T>(ref seed);
	}

	public static void Shuffle<T>(this List<T> list, ref uint seed)
	{
		for (int i = 0; i < list.Count; i++)
		{
			int item = SeedRandom.Range(ref seed, 0, list.Count);
			int num = SeedRandom.Range(ref seed, 0, list.Count);
			T t = list[item];
			list[item] = list[num];
			list[num] = t;
		}
	}

	public static double TruncatedAverage(this List<double> list, float pct)
	{
		int num = (int)Math.Floor((double)((float)list.Count * pct));
		return (
			from x in list
			orderby x
			select x).Skip<double>(num).Take<double>(list.Count - num * 2).Average();
	}
}