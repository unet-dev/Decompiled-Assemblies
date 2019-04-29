using System;
using System.Collections.Generic;

namespace Apex.AI
{
	public struct ScoredOptionComparer<T> : IComparer<ScoredOption<T>>
	{
		public bool @descending;

		public ScoredOptionComparer(bool descending = false)
		{
			this.@descending = descending;
		}

		public int Compare(ScoredOption<T> x, ScoredOption<T> y)
		{
			int num = x.score.CompareTo(y.score);
			if (!this.@descending)
			{
				return num;
			}
			return -1 * num;
		}
	}
}