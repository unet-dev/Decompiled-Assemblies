using Apex.Utilities;
using System;
using System.Collections.Generic;

namespace Apex.DataStructures
{
	public class FunctionComparer<T> : IComparer<T>
	{
		private Comparison<T> _comparer;

		public FunctionComparer(Comparison<T> comparer)
		{
			Ensure.ArgumentNotNull(comparer, "comparer");
			this._comparer = comparer;
		}

		public int Compare(T x, T y)
		{
			return this._comparer(x, y);
		}
	}
}