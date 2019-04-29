using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.AI
{
	public class AStarNodeList : List<AStarNode>
	{
		private readonly AStarNodeList.AStarNodeComparer comparer = new AStarNodeList.AStarNodeComparer();

		public AStarNodeList()
		{
		}

		public void AStarNodeSort()
		{
			base.Sort(this.comparer);
		}

		public bool Contains(BasePathNode n)
		{
			for (int i = 0; i < base.Count; i++)
			{
				AStarNode item = base[i];
				if (item != null && item.Node.Equals(n))
				{
					return true;
				}
			}
			return false;
		}

		public AStarNode GetAStarNodeOf(BasePathNode n)
		{
			for (int i = 0; i < base.Count; i++)
			{
				AStarNode item = base[i];
				if (item != null && item.Node.Equals(n))
				{
					return item;
				}
			}
			return null;
		}

		private class AStarNodeComparer : IComparer<AStarNode>
		{
			public AStarNodeComparer()
			{
			}

			int System.Collections.Generic.IComparer<Rust.AI.AStarNode>.Compare(AStarNode lhs, AStarNode rhs)
			{
				if (lhs < rhs)
				{
					return -1;
				}
				if (lhs > rhs)
				{
					return 1;
				}
				return 0;
			}
		}
	}
}