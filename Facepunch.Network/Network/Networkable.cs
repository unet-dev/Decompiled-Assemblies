using Facepunch;
using Facepunch.Extend;
using Network.Visibility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Network
{
	public class Networkable : Pool.IPooled
	{
		public uint ID;

		public Group @group;

		public Subscriber subscriber;

		public NetworkHandler handler;

		private bool updateSubscriptions;

		internal Server sv;

		internal Client cl;

		public Connection connection
		{
			get;
			private set;
		}

		public Networkable()
		{
		}

		public void CloseSubscriber()
		{
			if (this.subscriber != null)
			{
				this.sv.visibility.DestroySubscriber(ref this.subscriber);
			}
		}

		public void Destroy()
		{
			this.CloseSubscriber();
			if (this.ID > 0)
			{
				this.SwitchGroup(null);
				if (this.sv != null)
				{
					this.sv.ReturnUID(this.ID);
				}
			}
		}

		public void EnterPool()
		{
			this.ID = 0;
			this.connection = null;
			this.@group = null;
			this.sv = null;
			this.cl = null;
			this.handler = null;
			this.updateSubscriptions = false;
		}

		public void LeavePool()
		{
		}

		public void OnConnected(Connection c)
		{
			this.connection = c;
		}

		public void OnDisconnected()
		{
			this.connection = null;
			this.CloseSubscriber();
		}

		internal void OnGroupTransition(Group oldGroup)
		{
			if (oldGroup == null)
			{
				if (this.@group != null && this.handler != null)
				{
					this.handler.OnNetworkSubscribersEnter(this.@group.subscribers);
				}
				return;
			}
			if (this.@group == null)
			{
				if (oldGroup != null && this.handler != null)
				{
					this.handler.OnNetworkSubscribersLeave(oldGroup.subscribers);
				}
				return;
			}
			List<Connection> list = Pool.GetList<Connection>();
			List<Connection> connections = Pool.GetList<Connection>();
			oldGroup.subscribers.Compare<Connection>(this.@group.subscribers, list, connections, null);
			if (this.handler != null)
			{
				this.handler.OnNetworkSubscribersEnter(list);
			}
			if (this.handler != null)
			{
				this.handler.OnNetworkSubscribersLeave(connections);
			}
			Pool.FreeList<Connection>(ref list);
			Pool.FreeList<Connection>(ref connections);
		}

		internal void OnSubscriptionChange()
		{
			if (this.subscriber == null)
			{
				return;
			}
			if (this.@group != null && !this.subscriber.IsSubscribed(this.@group))
			{
				this.subscriber.Subscribe(this.@group);
				if (this.handler != null)
				{
					this.handler.OnNetworkGroupEnter(this.@group);
				}
			}
			this.updateSubscriptions = true;
		}

		public void StartSubscriber()
		{
			if (this.subscriber != null)
			{
				Debug.Log("BecomeSubscriber called twice!");
				return;
			}
			this.subscriber = this.sv.visibility.CreateSubscriber(this.connection);
			this.OnSubscriptionChange();
		}

		public bool SwitchGroup(Group newGroup)
		{
			if (newGroup == this.@group)
			{
				return false;
			}
			using (TimeWarning timeWarning = TimeWarning.New("SwitchGroup", 0.1f))
			{
				if (this.@group != null)
				{
					using (TimeWarning timeWarning1 = TimeWarning.New("group.Leave", 0.1f))
					{
						this.@group.Leave(this);
					}
				}
				Group group = this.@group;
				this.@group = newGroup;
				if (this.@group != null)
				{
					using (timeWarning1 = TimeWarning.New("group.Join", 0.1f))
					{
						this.@group.Join(this);
					}
				}
				if (this.handler != null && this.@group != null)
				{
					using (timeWarning1 = TimeWarning.New("OnNetworkGroupChange", 0.1f))
					{
						this.handler.OnNetworkGroupChange();
					}
				}
				using (timeWarning1 = TimeWarning.New("OnSubscriptionChange", 0.1f))
				{
					this.OnSubscriptionChange();
				}
				using (timeWarning1 = TimeWarning.New("OnGroupTransition", 0.1f))
				{
					this.OnGroupTransition(group);
				}
			}
			return true;
		}

		public bool UpdateGroups(Vector3 position)
		{
			Debug.Assert(this.sv != null, "SV IS NULL");
			Debug.Assert(this.sv.visibility != null, "sv.visibility IS NULL");
			if (this.sv.visibility.IsInside(this.@group, position))
			{
				return false;
			}
			Group group = this.sv.visibility.GetGroup(position);
			if (group != this.@group || this.@group == null)
			{
				return this.SwitchGroup(group);
			}
			Debug.LogWarning(string.Concat("UpdateGroups: IsInside isn't giving the same results as GetGroup (", position, ")"));
			return false;
		}

		public bool UpdateSubscriptions(int removeLimit, int addLimit)
		{
			if (!this.updateSubscriptions)
			{
				return false;
			}
			if (this.subscriber == null)
			{
				return false;
			}
			using (TimeWarning timeWarning = TimeWarning.New("UpdateSubscriptions", 0.1f))
			{
				this.updateSubscriptions = false;
				List<Group> list = Pool.GetList<Group>();
				List<Group> groups = Pool.GetList<Group>();
				List<Group> list1 = Pool.GetList<Group>();
				this.sv.visibility.GetVisibleFrom(this.@group, list1);
				this.subscriber.subscribed.Compare<Group>(list1, list, groups, null);
				for (int i = 0; i < groups.Count; i++)
				{
					Group item = groups[i];
					if (removeLimit <= 0)
					{
						this.updateSubscriptions = true;
					}
					else
					{
						this.subscriber.Unsubscribe(item);
						if (this.handler != null)
						{
							this.handler.OnNetworkGroupLeave(item);
						}
						removeLimit -= item.networkables.Count;
					}
				}
				for (int j = 0; j < list.Count; j++)
				{
					Group group = list[j];
					if (addLimit <= 0)
					{
						this.updateSubscriptions = true;
					}
					else
					{
						this.subscriber.Subscribe(group);
						if (this.handler != null)
						{
							this.handler.OnNetworkGroupEnter(group);
						}
						addLimit -= group.networkables.Count;
					}
				}
				Pool.FreeList<Group>(ref list);
				Pool.FreeList<Group>(ref groups);
				Pool.FreeList<Group>(ref list1);
			}
			return true;
		}
	}
}