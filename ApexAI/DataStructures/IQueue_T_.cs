using System;

namespace Apex.DataStructures
{
	public interface IQueue<T>
	{
		T Dequeue();

		void Enqueue(T obj);
	}
}