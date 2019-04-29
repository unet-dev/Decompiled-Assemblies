using System;
using System.Reflection;

namespace Apex.DataStructures
{
	public interface IIndexable<T>
	{
		int count
		{
			get;
		}

		T this[int idx]
		{
			get;
		}
	}
}