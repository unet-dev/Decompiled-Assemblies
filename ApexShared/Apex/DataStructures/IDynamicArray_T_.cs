using System;
using System.Collections;
using System.Collections.Generic;

namespace Apex.DataStructures
{
	public interface IDynamicArray<T> : IIterable<T>, IEnumerable<T>, IEnumerable, IIndexable<T>
	{
		void Add(T item);

		void Clear();

		void EnsureCapacity(int capacity);

		bool Remove(T item);

		void RemoveAt(int index);
	}
}