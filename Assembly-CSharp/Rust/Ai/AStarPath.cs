using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.AI
{
	public static class AStarPath
	{
		public static bool FindPath(BasePathNode start, BasePathNode goal, out Stack<BasePathNode> path, out float pathCost)
		{
			path = null;
			pathCost = -1f;
			bool flag = false;
			if (start == goal)
			{
				return false;
			}
			AStarNodeList aStarNodeList = new AStarNodeList();
			HashSet<BasePathNode> basePathNodes = new HashSet<BasePathNode>();
			AStarNode aStarNode = new AStarNode(0f, AStarPath.Heuristic(start, goal), null, start);
			aStarNodeList.Add(aStarNode);
			while (aStarNodeList.Count > 0)
			{
				AStarNode item = aStarNodeList[0];
				aStarNodeList.RemoveAt(0);
				basePathNodes.Add(item.Node);
				if (!item.Satisfies(goal))
				{
					foreach (BasePathNode node in item.Node.linked)
					{
						if (basePathNodes.Contains(node))
						{
							continue;
						}
						float g = item.G + AStarPath.Heuristic(item.Node, node);
						AStarNode aStarNodeOf = aStarNodeList.GetAStarNodeOf(node);
						if (aStarNodeOf != null)
						{
							if (g >= aStarNodeOf.G)
							{
								continue;
							}
							aStarNodeOf.Update(g, aStarNodeOf.H, item, node);
							aStarNodeList.AStarNodeSort();
						}
						else
						{
							aStarNodeOf = new AStarNode(g, AStarPath.Heuristic(node, goal), item, node);
							aStarNodeList.Add(aStarNodeOf);
							aStarNodeList.AStarNodeSort();
						}
					}
				}
				else
				{
					path = new Stack<BasePathNode>();
					pathCost = 0f;
					while (item.Parent != null)
					{
						pathCost += item.F;
						path.Push(item.Node);
						item = item.Parent;
					}
					if (item != null)
					{
						path.Push(item.Node);
					}
					flag = true;
					break;
				}
			}
			return flag;
		}

		private static float Heuristic(BasePathNode from, BasePathNode to)
		{
			return Vector3.Distance(from.transform.position, to.transform.position);
		}
	}
}