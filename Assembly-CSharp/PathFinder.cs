using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PathFinder
{
	private int[,] costmap;

	private bool[,] visited;

	private PathFinder.Point[] neighbors;

	private static PathFinder.Point[] mooreNeighbors;

	private static PathFinder.Point[] neumannNeighbors;

	static PathFinder()
	{
		PathFinder.mooreNeighbors = new PathFinder.Point[] { new PathFinder.Point(0, 1), new PathFinder.Point(-1, 0), new PathFinder.Point(1, 0), new PathFinder.Point(0, -1), new PathFinder.Point(-1, 1), new PathFinder.Point(1, 1), new PathFinder.Point(-1, -1), new PathFinder.Point(1, -1) };
		PathFinder.neumannNeighbors = new PathFinder.Point[] { new PathFinder.Point(0, 1), new PathFinder.Point(-1, 0), new PathFinder.Point(1, 0), new PathFinder.Point(0, -1) };
	}

	public PathFinder(int[,] costmap, bool diagonals = true)
	{
		this.costmap = costmap;
		this.neighbors = (diagonals ? PathFinder.mooreNeighbors : PathFinder.neumannNeighbors);
	}

	public PathFinder.Node FindClosestWalkable(PathFinder.Point start, int depth = 2147483647)
	{
		if (this.visited != null)
		{
			Array.Clear(this.visited, 0, this.visited.Length);
		}
		else
		{
			this.visited = new bool[this.costmap.GetLength(0), this.costmap.GetLength(1)];
		}
		int num = 0;
		int length = this.costmap.GetLength(0) - 1;
		int num1 = 0;
		int length1 = this.costmap.GetLength(1) - 1;
		IntrusiveMinHeap<PathFinder.Node> intrusiveMinHeap = new IntrusiveMinHeap<PathFinder.Node>();
		int num2 = 1;
		int num3 = this.Heuristic(start);
		intrusiveMinHeap.Add(new PathFinder.Node(start, num2, num3, null));
		this.visited[start.y, start.x] = true;
		while (!intrusiveMinHeap.Empty)
		{
			int num4 = depth;
			depth = num4 - 1;
			if (num4 <= 0)
			{
				break;
			}
			PathFinder.Node node = intrusiveMinHeap.Pop();
			if (node.heuristic == 0)
			{
				return node;
			}
			for (int i = 0; i < (int)this.neighbors.Length; i++)
			{
				PathFinder.Point point = node.point + this.neighbors[i];
				if (point.x >= num && point.x <= length && point.y >= num1 && point.y <= length1 && !this.visited[point.y, point.x])
				{
					this.visited[point.y, point.x] = true;
					int num5 = node.cost + 1;
					int num6 = this.Heuristic(point);
					intrusiveMinHeap.Add(new PathFinder.Node(point, num5, num6, node));
				}
			}
		}
		return null;
	}

	public PathFinder.Node FindEnd(PathFinder.Node start)
	{
		for (PathFinder.Node i = start; i != null; i = i.next)
		{
			if (i.next == null)
			{
				return i;
			}
		}
		return start;
	}

	public PathFinder.Node FindPath(PathFinder.Point start, PathFinder.Point end, int depth = 2147483647)
	{
		return this.FindPathReversed(end, start, depth);
	}

	public PathFinder.Node FindPathDirected(List<PathFinder.Point> startList, List<PathFinder.Point> endList, int depth = 2147483647)
	{
		return this.FindPathReversed(endList, startList, depth);
	}

	private PathFinder.Node FindPathReversed(PathFinder.Point start, PathFinder.Point end, int depth = 2147483647)
	{
		if (this.visited != null)
		{
			Array.Clear(this.visited, 0, this.visited.Length);
		}
		else
		{
			this.visited = new bool[this.costmap.GetLength(0), this.costmap.GetLength(1)];
		}
		int num = 0;
		int length = this.costmap.GetLength(0) - 1;
		int num1 = 0;
		int length1 = this.costmap.GetLength(1) - 1;
		IntrusiveMinHeap<PathFinder.Node> intrusiveMinHeap = new IntrusiveMinHeap<PathFinder.Node>();
		int num2 = this.costmap[start.y, start.x];
		int num3 = this.Heuristic(start, end);
		intrusiveMinHeap.Add(new PathFinder.Node(start, num2, num3, null));
		this.visited[start.y, start.x] = true;
		while (!intrusiveMinHeap.Empty)
		{
			int num4 = depth;
			depth = num4 - 1;
			if (num4 <= 0)
			{
				break;
			}
			PathFinder.Node node = intrusiveMinHeap.Pop();
			if (node.heuristic == 0)
			{
				return node;
			}
			for (int i = 0; i < (int)this.neighbors.Length; i++)
			{
				PathFinder.Point point = node.point + this.neighbors[i];
				if (point.x >= num && point.x <= length && point.y >= num1 && point.y <= length1 && !this.visited[point.y, point.x])
				{
					this.visited[point.y, point.x] = true;
					int num5 = this.costmap[point.y, point.x];
					if (num5 != 2147483647)
					{
						int num6 = node.cost + num5;
						int num7 = this.Heuristic(point, end);
						intrusiveMinHeap.Add(new PathFinder.Node(point, num6, num7, node));
					}
				}
			}
		}
		return null;
	}

	private PathFinder.Node FindPathReversed(List<PathFinder.Point> startList, List<PathFinder.Point> endList, int depth = 2147483647)
	{
		if (this.visited != null)
		{
			Array.Clear(this.visited, 0, this.visited.Length);
		}
		else
		{
			this.visited = new bool[this.costmap.GetLength(0), this.costmap.GetLength(1)];
		}
		int num = 0;
		int length = this.costmap.GetLength(0) - 1;
		int num1 = 0;
		int length1 = this.costmap.GetLength(1) - 1;
		IntrusiveMinHeap<PathFinder.Node> intrusiveMinHeap = new IntrusiveMinHeap<PathFinder.Node>();
		foreach (PathFinder.Point point in startList)
		{
			int num2 = this.costmap[point.y, point.x];
			int num3 = this.Heuristic(point, endList);
			intrusiveMinHeap.Add(new PathFinder.Node(point, num2, num3, null));
			this.visited[point.y, point.x] = true;
		}
		while (!intrusiveMinHeap.Empty)
		{
			int num4 = depth;
			depth = num4 - 1;
			if (num4 <= 0)
			{
				break;
			}
			PathFinder.Node node = intrusiveMinHeap.Pop();
			if (node.heuristic == 0)
			{
				return node;
			}
			for (int i = 0; i < (int)this.neighbors.Length; i++)
			{
				PathFinder.Point point1 = node.point + this.neighbors[i];
				if (point1.x >= num && point1.x <= length && point1.y >= num1 && point1.y <= length1 && !this.visited[point1.y, point1.x])
				{
					this.visited[point1.y, point1.x] = true;
					int num5 = this.costmap[point1.y, point1.x];
					if (num5 != 2147483647)
					{
						int num6 = node.cost + num5;
						int num7 = this.Heuristic(point1, endList);
						intrusiveMinHeap.Add(new PathFinder.Node(point1, num6, num7, node));
					}
				}
			}
		}
		return null;
	}

	public PathFinder.Node FindPathUndirected(List<PathFinder.Point> startList, List<PathFinder.Point> endList, int depth = 2147483647)
	{
		if (startList.Count > endList.Count)
		{
			return this.FindPathReversed(endList, startList, depth);
		}
		return this.FindPathReversed(startList, endList, depth);
	}

	public int Heuristic(PathFinder.Point a)
	{
		if (this.costmap[a.y, a.x] != 2147483647)
		{
			return 0;
		}
		return 1;
	}

	public int Heuristic(PathFinder.Point a, PathFinder.Point b)
	{
		int num = a.x - b.x;
		int num1 = a.y - b.y;
		return num * num + num1 * num1;
	}

	public int Heuristic(PathFinder.Point a, List<PathFinder.Point> b)
	{
		int num = 2147483647;
		for (int i = 0; i < b.Count; i++)
		{
			num = Mathf.Min(num, this.Heuristic(a, b[i]));
		}
		return num;
	}

	public PathFinder.Node Reverse(PathFinder.Node start)
	{
		PathFinder.Node node = null;
		PathFinder.Node node1 = null;
		for (PathFinder.Node i = start; i != null; i = i.next)
		{
			if (node != null)
			{
				node.next = node1;
			}
			node1 = node;
			node = i;
		}
		if (node != null)
		{
			node.next = node1;
		}
		return node;
	}

	public class Node : IMinHeapNode<PathFinder.Node>, ILinkedListNode<PathFinder.Node>
	{
		public PathFinder.Point point;

		public int cost;

		public int heuristic;

		public PathFinder.Node child
		{
			get;
			set;
		}

		public PathFinder.Node next
		{
			get;
			set;
		}

		public int order
		{
			get
			{
				return this.cost + this.heuristic;
			}
		}

		public Node(PathFinder.Point point, int cost, int heuristic, PathFinder.Node next = null)
		{
			this.point = point;
			this.cost = cost;
			this.heuristic = heuristic;
			this.next = next;
		}
	}

	public struct Point : IEquatable<PathFinder.Point>
	{
		public int x;

		public int y;

		public Point(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public override bool Equals(object other)
		{
			if (!(other is PathFinder.Point))
			{
				return false;
			}
			return this.Equals((PathFinder.Point)other);
		}

		public bool Equals(PathFinder.Point other)
		{
			if (this.x != other.x)
			{
				return false;
			}
			return this.y == other.y;
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode();
		}

		public static PathFinder.Point operator +(PathFinder.Point a, PathFinder.Point b)
		{
			return new PathFinder.Point(a.x + b.x, a.y + b.y);
		}

		public static PathFinder.Point operator /(PathFinder.Point p, int i)
		{
			return new PathFinder.Point(p.x / i, p.y / i);
		}

		public static bool operator ==(PathFinder.Point a, PathFinder.Point b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(PathFinder.Point a, PathFinder.Point b)
		{
			return !a.Equals(b);
		}

		public static PathFinder.Point operator *(PathFinder.Point p, int i)
		{
			return new PathFinder.Point(p.x * i, p.y * i);
		}

		public static PathFinder.Point operator -(PathFinder.Point a, PathFinder.Point b)
		{
			return new PathFinder.Point(a.x - b.x, a.y - b.y);
		}
	}
}