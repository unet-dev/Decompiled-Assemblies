using System;
using System.Collections.Generic;

namespace Apex.DataStructures
{
	public interface ISortable<T>
	{
		void Sort();

		void Sort(IComparer<T> comparer);

		void Sort(int index, int length);

		void Sort(int index, int length, IComparer<T> comparer);
	}
}