using Facepunch;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Visibility
{
	public class Subscriber : Pool.IPooled
	{
		internal Manager manager;

		internal Connection connection;

		public List<Group> subscribed = new List<Group>();

		public Subscriber()
		{
		}

		public void Destroy()
		{
			this.UnsubscribeAll();
		}

		public void EnterPool()
		{
			this.connection = null;
			this.manager = null;
		}

		public bool IsSubscribed(Group group)
		{
			return this.subscribed.Contains(group);
		}

		public void LeavePool()
		{
		}

		public Group Subscribe(Group group)
		{
			if (this.subscribed.Contains(group))
			{
				Debug.LogWarning("Subscribe: Network Group already subscribed!");
				return null;
			}
			this.subscribed.Add(group);
			group.AddSubscriber(this.connection);
			return group;
		}

		public Group Subscribe(uint group)
		{
			return this.Subscribe(this.manager.Get(group));
		}

		public void Unsubscribe(Group group)
		{
			this.subscribed.Remove(group);
			group.RemoveSubscriber(this.connection);
		}

		public void UnsubscribeAll()
		{
			foreach (Group group in this.subscribed)
			{
				group.RemoveSubscriber(this.connection);
			}
			this.subscribed.Clear();
		}
	}
}