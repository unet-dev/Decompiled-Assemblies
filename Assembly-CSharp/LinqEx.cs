using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class LinqEx
{
	public static int MaxIndex<T>(this IEnumerable<T> sequence)
	where T : IComparable<T>
	{
		int num = -1;
		T t = default(T);
		int num1 = 0;
		foreach (T t1 in sequence)
		{
			if (t1.CompareTo(t) > 0 || num == -1)
			{
				num = num1;
				t = t1;
			}
			num1++;
		}
		return num;
	}
}