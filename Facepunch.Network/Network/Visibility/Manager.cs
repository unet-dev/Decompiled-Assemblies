using Facepunch;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Visibility
{
	public class Manager : IDisposable
	{
		private Dictionary<uint, Group> groups = new Dictionary<uint, Group>();

		internal Provider provider;

		public Manager(Provider p)
		{
			if (this.groups.Count > 0 && p != null)
			{
				Debug.LogWarning(string.Concat("SetProvider should be called before anything else! ", this.groups.Count, " groups have already been registered!"));
			}
			this.provider = p;
		}

		public Subscriber CreateSubscriber(Connection connection)
		{
			Subscriber subscriber = Pool.Get<Subscriber>();
			subscriber.manager = this;
			subscriber.connection = connection;
			return subscriber;
		}

		public void DestroySubscriber(ref Subscriber subscriber)
		{
			subscriber.Destroy();
			Pool.Free<Subscriber>(ref subscriber);
		}

		public virtual void Dispose()
		{
			foreach (KeyValuePair<uint, Group> group in this.groups)
			{
				group.Value.Dispose();
			}
			this.groups.Clear();
			this.provider = null;
		}

		public Group Get(uint ID)
		{
			Group group;
			if (this.groups.TryGetValue(ID, out group))
			{
				return group;
			}
			group = new Group(this, ID);
			this.groups.Add(ID, group);
			if (this.provider != null)
			{
				this.provider.OnGroupAdded(group);
			}
			return group;
		}

		public Group GetGroup(Vector3 vPos)
		{
			if (this.provider == null)
			{
				return this.Get(0);
			}
			return this.provider.GetGroup(vPos);
		}

		public void GetVisibleFrom(Group center, List<Group> groups)
		{
			if (this.provider == null)
			{
				return;
			}
			if (center == null)
			{
				return;
			}
			this.provider.GetVisibleFrom(center, groups);
		}

		public bool IsInside(Group group, Vector3 vPos)
		{
			if (this.provider == null)
			{
				return false;
			}
			if (group == null)
			{
				return false;
			}
			return this.provider.IsInside(group, vPos);
		}

		public Group TryGet(uint ID)
		{
			Group group;
			if (this.groups.TryGetValue(ID, out group))
			{
				return group;
			}
			return null;
		}
	}
}