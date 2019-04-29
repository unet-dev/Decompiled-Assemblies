using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Facepunch.Extend
{
	public static class List
	{
		public static void Compare<T>(this List<T> a, List<T> b, List<T> added, List<T> removed, List<T> remained)
		{
			if (a == null && b == null)
			{
				return;
			}
			if (a == null)
			{
				if (added != null)
				{
					added.AddRange(b);
				}
				return;
			}
			if (b == null)
			{
				if (removed != null)
				{
					removed.AddRange(a);
				}
				return;
			}
			if (a.Count == 0 && b.Count == 0)
			{
				return;
			}
			if (added != null)
			{
				added.AddRange(b);
			}
			if (removed != null)
			{
				removed.AddRange(a);
			}
			foreach (T t in b)
			{
				if (!a.Contains(t))
				{
					continue;
				}
				if (remained != null)
				{
					remained.Add(t);
				}
				if (added != null)
				{
					while (added.Remove(t))
					{
					}
				}
				if (removed == null)
				{
					continue;
				}
				while (removed.Remove(t))
				{
				}
			}
		}
	}
}