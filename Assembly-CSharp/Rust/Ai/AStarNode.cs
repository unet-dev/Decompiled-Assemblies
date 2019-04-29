using System;

namespace Rust.AI
{
	public class AStarNode
	{
		public AStarNode Parent;

		public float G;

		public float H;

		public BasePathNode Node;

		public float F
		{
			get
			{
				return this.G + this.H;
			}
		}

		public AStarNode(float g, float h, AStarNode parent, BasePathNode node)
		{
			this.G = g;
			this.H = h;
			this.Parent = parent;
			this.Node = node;
		}

		public static bool operator >(AStarNode lhs, AStarNode rhs)
		{
			return lhs.F > rhs.F;
		}

		public static bool operator <(AStarNode lhs, AStarNode rhs)
		{
			return lhs.F < rhs.F;
		}

		public bool Satisfies(BasePathNode node)
		{
			return this.Node == node;
		}

		public void Update(float g, float h, AStarNode parent, BasePathNode node)
		{
			this.G = g;
			this.H = h;
			this.Parent = parent;
			this.Node = node;
		}
	}
}