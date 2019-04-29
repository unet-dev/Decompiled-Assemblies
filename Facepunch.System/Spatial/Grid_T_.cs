using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Spatial
{
	public class Grid<T>
	{
		private float CenterX;

		private float CenterY;

		private Grid<T>.Node[,] Nodes;

		private Dictionary<T, Grid<T>.Node> Lookup;

		public int CellCount
		{
			get;
			private set;
		}

		public int CellSize
		{
			get;
			private set;
		}

		public Grid(int CellSize, float WorldSize)
		{
			this.CellSize = CellSize;
			this.CellCount = (int)(WorldSize / (float)CellSize + 0.5f);
			this.CenterX = WorldSize * 0.5f;
			this.CenterY = WorldSize * 0.5f;
			this.Nodes = new Grid<T>.Node[this.CellCount, this.CellCount];
			this.Lookup = new Dictionary<T, Grid<T>.Node>(512);
		}

		public void Add(T obj, float x, float y)
		{
			Grid<T>.Node node = this.GetNode(x, y, true);
			node.Add(obj);
			this.Lookup.Add(obj, node);
		}

		private int Clamp(float input)
		{
			int num = (int)input;
			if (num < 0)
			{
				return 0;
			}
			if (num <= this.CellCount - 1)
			{
				return num;
			}
			return this.CellCount - 1;
		}

		private Grid<T>.Node GetNode(float x, float y, bool create = true)
		{
			x += this.CenterX;
			y += this.CenterY;
			int num = this.Clamp(x / (float)this.CellSize);
			int num1 = this.Clamp(y / (float)this.CellSize);
			Grid<T>.Node nodes = this.Nodes[num, num1];
			if (nodes == null & create)
			{
				nodes = new Grid<T>.Node();
				this.Nodes[num, num1] = nodes;
			}
			return nodes;
		}

		public void Move(T obj, float x, float y)
		{
			Grid<T>.Node node = this.GetNode(x, y, true);
			Grid<T>.Node node1 = null;
			if (this.Lookup.TryGetValue(obj, out node1))
			{
				if (node == node1)
				{
					return;
				}
				node1.Remove(obj);
				node.Add(obj);
				this.Lookup[obj] = node;
			}
		}

		public int Query(float x, float y, float radius, T[] result, Func<T, bool> filter = null)
		{
			int num;
			int num1 = this.Clamp((x + this.CenterX - radius) / (float)this.CellSize);
			int num2 = this.Clamp((x + this.CenterX + radius) / (float)this.CellSize);
			int num3 = this.Clamp((y + this.CenterY - radius) / (float)this.CellSize);
			int num4 = this.Clamp((y + this.CenterY + radius) / (float)this.CellSize);
			int num5 = 0;
			for (int i = num1; i <= num2; i++)
			{
				for (int j = num3; j <= num4; j++)
				{
					if (this.Nodes[i, j] != null)
					{
						HashSet<T>.Enumerator enumerator = this.Nodes[i, j].Contents.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								T current = enumerator.Current;
								if (filter != null && !filter(current))
								{
									continue;
								}
								result[num5] = current;
								num5++;
								if (num5 < (int)result.Length)
								{
									continue;
								}
								num = num5;
								return num;
							}
							goto Label0;
						}
						finally
						{
							((IDisposable)enumerator).Dispose();
						}
						return num;
					}
				Label0:
				}
			}
			return num5;
		}

		public bool Remove(T obj)
		{
			Grid<T>.Node node = null;
			if (!this.Lookup.TryGetValue(obj, out node))
			{
				return false;
			}
			node.Remove(obj);
			this.Lookup.Remove(obj);
			return true;
		}

		internal class Node
		{
			public HashSet<T> Contents;

			public Node()
			{
			}

			public void Add(T obj)
			{
				this.Contents.Add(obj);
			}

			public bool Remove(T obj)
			{
				return this.Contents.Remove(obj);
			}
		}
	}
}