using Network;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Visibility
{
	public class Group : IDisposable
	{
		protected Manager manager;

		public uint ID;

		public Bounds bounds;

		public ListHashSet<Networkable> networkables = new ListHashSet<Networkable>(8);

		public List<Connection> subscribers = new List<Connection>();

		public bool isGlobal
		{
			get
			{
				return this.ID == 0;
			}
		}

		public Group(Manager m, uint id)
		{
			this.manager = m;
			this.ID = id;
		}

		public void AddSubscriber(Connection cn)
		{
			this.subscribers.Add(cn);
		}

		public virtual void Dispose()
		{
			this.networkables = null;
			this.subscribers = null;
			this.manager = null;
			this.ID = 0;
		}

		public bool HasSubscribers()
		{
			return this.subscribers.Count > 0;
		}

		public void Join(Networkable nw)
		{
			if (this.networkables == null)
			{
				return;
			}
			if (this.networkables.Contains(nw))
			{
				Debug.LogWarning("Insert: Network Group already contains networkable!");
				return;
			}
			this.networkables.Add(nw);
		}

		public void Leave(Networkable nw)
		{
			if (this.networkables == null)
			{
				return;
			}
			if (!this.networkables.Contains(nw))
			{
				Debug.LogWarning("Leave: Network Group doesn't contain networkable!");
				return;
			}
			this.networkables.Remove(nw);
		}

		public void RemoveSubscriber(Connection cn)
		{
			if (this.subscribers == null)
			{
				return;
			}
			this.subscribers.Remove(cn);
		}

		public override string ToString()
		{
			return string.Concat("NWGroup", this.ID);
		}
	}
}