using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class SpanningTree<T>
{
	private List<SpanningTree<T>.Node> nodes;

	private List<SpanningTree<T>.Edge> edges;

	public SpanningTree()
	{
	}

	public void AddEdge(int a_idx, int b_idx, int cost, T value)
	{
		SpanningTree<T>.Node item = this.nodes[a_idx];
		SpanningTree<T>.Node node = this.nodes[b_idx];
		item.edges.Add(new SpanningTree<T>.Edge(item, node, cost, value));
	}

	public int AddNode()
	{
		this.nodes.Add(new SpanningTree<T>.Node());
		return this.nodes.Count - 1;
	}

	public void CalculateMin()
	{
		this.Reset();
		IntrusiveMinHeap<SpanningTree<T>.Edge> intrusiveMinHeap = new IntrusiveMinHeap<SpanningTree<T>.Edge>();
		foreach (SpanningTree<T>.Node node in this.nodes)
		{
			if (node.connected)
			{
				continue;
			}
			foreach (SpanningTree<T>.Edge edge in node.edges)
			{
				if (edge.target.connected)
				{
					continue;
				}
				intrusiveMinHeap.Add(edge);
			}
			node.connected = true;
			while (!intrusiveMinHeap.Empty)
			{
				SpanningTree<T>.Edge edge1 = intrusiveMinHeap.Pop();
				SpanningTree<T>.Node node1 = edge1.target;
				if (node1.connected)
				{
					continue;
				}
				node1.connected = true;
				foreach (SpanningTree<T>.Edge edge2 in node1.edges)
				{
					if (edge2.target == edge1.source)
					{
						edge1 = edge2;
					}
					if (edge2.target.connected)
					{
						continue;
					}
					intrusiveMinHeap.Add(edge2);
				}
				this.edges.Add(edge1);
			}
		}
	}

	public void Clear()
	{
		this.nodes.Clear();
		this.edges.Clear();
	}

	public void ForEach(Action<T> action)
	{
		foreach (SpanningTree<T>.Edge edge in this.edges)
		{
			action(edge.@value);
		}
	}

	public void Reset()
	{
		foreach (SpanningTree<T>.Node node in this.nodes)
		{
			node.connected = false;
		}
		this.edges.Clear();
	}

	private class Edge : IMinHeapNode<SpanningTree<T>.Edge>
	{
		public SpanningTree<T>.Node source;

		public SpanningTree<T>.Node target;

		public T @value;

		public SpanningTree<T>.Edge child
		{
			get;
			set;
		}

		public int order
		{
			get
			{
				return get_order();
			}
			set
			{
				set_order(value);
			}
		}

		private int <order>k__BackingField;

		public int get_order()
		{
			return this.<order>k__BackingField;
		}

		private void set_order(int value)
		{
			this.<order>k__BackingField = value;
		}

		public Edge(SpanningTree<T>.Node source, SpanningTree<T>.Node target, int order, T value)
		{
			this.source = source;
			this.target = target;
			this.order = order;
			this.@value = value;
		}
	}

	private class Node
	{
		public List<SpanningTree<T>.Edge> edges;

		public bool connected;

		public Node()
		{
			this.edges = new List<SpanningTree<T>.Edge>();
			this.connected = false;
		}
	}
}