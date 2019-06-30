using Facepunch;
using Facepunch.Extend;
using System;
using System.Collections.Generic;

namespace Network
{
	public class Stats
	{
		public bool Enabled;

		public Stats.Node Building = new Stats.Node();

		public Stats.Node Previous = new Stats.Node();

		public Stats()
		{
			this.Building.Add("", (long)0);
			this.Building.Clear();
			this.Previous.Add("", (long)0);
			this.Previous.Clear();
		}

		public void Add(string Category, string Object, long Bytes)
		{
			if (!this.Enabled)
			{
				return;
			}
			this.Building.Bytes += Bytes;
			this.Building.Count += (long)1;
			this.Building.Add(Category, Bytes).Add(Object, Bytes);
		}

		public void Add(string Category, long Bytes)
		{
			if (!this.Enabled)
			{
				return;
			}
			this.Building.Bytes += Bytes;
			this.Building.Count += (long)1;
			this.Building.Add(Category, Bytes);
		}

		public void Flip()
		{
			if (!this.Enabled)
			{
				return;
			}
			Stats.Node building = this.Building;
			this.Building = this.Previous;
			this.Previous = building;
			this.Building.Clear();
		}

		public class Node : Pool.IPooled
		{
			public Dictionary<string, Stats.Node> Children;

			public long Bytes;

			public long Count;

			public Node()
			{
			}

			internal Stats.Node Add(string category, long bytes)
			{
				if (this.Children == null)
				{
					this.Children = Pool.Get<Dictionary<string, Stats.Node>>();
				}
				Stats.Node orCreatePooled = this.Children.GetOrCreatePooled<string, Stats.Node>(category);
				orCreatePooled.Bytes += bytes;
				orCreatePooled.Count += (long)1;
				return orCreatePooled;
			}

			internal void Clear()
			{
				this.Bytes = (long)0;
				this.Count = (long)0;
				if (this.Children == null)
				{
					return;
				}
				foreach (KeyValuePair<string, Stats.Node> child in this.Children)
				{
					Stats.Node value = child.Value;
					Pool.Free<Stats.Node>(ref value);
				}
				this.Children.Clear();
			}

			public void EnterPool()
			{
				this.Clear();
			}

			public void LeavePool()
			{
				this.Clear();
			}
		}
	}
}