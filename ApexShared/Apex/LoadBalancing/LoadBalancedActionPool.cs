using Apex.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Apex.LoadBalancing
{
	public static class LoadBalancedActionPool
	{
		private static Queue<LoadBalancedActionPool.RecycledOneTimeAction> _oneTimeActions;

		private static Queue<LoadBalancedActionPool.RecycledLongRunningAction> _longActions;

		private static Queue<LoadBalancedActionPool.RecycledAction> _actions;

		public static ILoadBalancedHandle Execute(this ILoadBalancer lb, Func<float, bool> action, bool delayFirstUpdate = false)
		{
			return lb.Add(LoadBalancedActionPool.GetAction(action), delayFirstUpdate);
		}

		public static ILoadBalancedHandle Execute(this ILoadBalancer lb, Func<float, bool> action, float interval, bool delayFirstUpdate = false)
		{
			return lb.Add(LoadBalancedActionPool.GetAction(action), interval, delayFirstUpdate);
		}

		public static ILoadBalancedHandle Execute(this ILoadBalancer lb, Func<float, bool> action, float interval, float delayFirstUpdateBy)
		{
			return lb.Add(LoadBalancedActionPool.GetAction(action), interval, delayFirstUpdateBy);
		}

		public static ILoadBalancedHandle Execute(this ILoadBalancer lb, IEnumerator longRunningAction, int maxMillisecondsUsedPerFrame)
		{
			LoadBalancedActionPool.RecycledLongRunningAction recycledLongRunningAction;
			Ensure.ArgumentNotNull(longRunningAction, "longRunningAction");
			if (LoadBalancedActionPool._longActions == null)
			{
				LoadBalancedActionPool._longActions = new Queue<LoadBalancedActionPool.RecycledLongRunningAction>(1);
			}
			if (LoadBalancedActionPool._longActions.Count <= 0)
			{
				recycledLongRunningAction = new LoadBalancedActionPool.RecycledLongRunningAction()
				{
					iter = longRunningAction,
					maxMillisecondsUsedPerFrame = maxMillisecondsUsedPerFrame
				};
			}
			else
			{
				recycledLongRunningAction = LoadBalancedActionPool._longActions.Dequeue();
				recycledLongRunningAction.iter = longRunningAction;
				recycledLongRunningAction.maxMillisecondsUsedPerFrame = maxMillisecondsUsedPerFrame;
			}
			return lb.Add(recycledLongRunningAction);
		}

		public static void ExecuteOnce(this ILoadBalancer lb, Action action, float delay = 0f)
		{
			LoadBalancedActionPool.RecycledOneTimeAction recycledOneTimeAction;
			Ensure.ArgumentNotNull(action, "action");
			if (LoadBalancedActionPool._oneTimeActions == null)
			{
				LoadBalancedActionPool._oneTimeActions = new Queue<LoadBalancedActionPool.RecycledOneTimeAction>(1);
			}
			if (LoadBalancedActionPool._oneTimeActions.Count <= 0)
			{
				recycledOneTimeAction = new LoadBalancedActionPool.RecycledOneTimeAction()
				{
					action = action
				};
			}
			else
			{
				recycledOneTimeAction = LoadBalancedActionPool._oneTimeActions.Dequeue();
				recycledOneTimeAction.action = action;
			}
			if (delay <= 0f)
			{
				lb.Add(recycledOneTimeAction);
				return;
			}
			lb.Add(recycledOneTimeAction, delay, true);
		}

		private static LoadBalancedActionPool.RecycledAction GetAction(Func<float, bool> action)
		{
			LoadBalancedActionPool.RecycledAction recycledAction;
			Ensure.ArgumentNotNull(action, "action");
			if (LoadBalancedActionPool._actions == null)
			{
				LoadBalancedActionPool._actions = new Queue<LoadBalancedActionPool.RecycledAction>(1);
			}
			if (LoadBalancedActionPool._actions.Count <= 0)
			{
				recycledAction = new LoadBalancedActionPool.RecycledAction()
				{
					action = action
				};
			}
			else
			{
				recycledAction = LoadBalancedActionPool._actions.Dequeue();
				recycledAction.action = action;
			}
			return recycledAction;
		}

		private static void Return(LoadBalancedActionPool.RecycledOneTimeAction action)
		{
			action.action = null;
			LoadBalancedActionPool._oneTimeActions.Enqueue(action);
		}

		private static void Return(LoadBalancedActionPool.RecycledAction action)
		{
			action.action = null;
			LoadBalancedActionPool._actions.Enqueue(action);
		}

		private static void Return(LoadBalancedActionPool.RecycledLongRunningAction action)
		{
			action.iter = null;
			LoadBalancedActionPool._longActions.Enqueue(action);
		}

		private class RecycledAction : ILoadBalanced
		{
			private Func<float, bool> _action;

			internal Func<float, bool> action
			{
				get
				{
					return this._action;
				}
				set
				{
					this._action = value;
				}
			}

			public bool repeat
			{
				get
				{
					return get_repeat();
				}
				set
				{
					set_repeat(value);
				}
			}

			private bool <repeat>k__BackingField;

			public bool get_repeat()
			{
				return this.<repeat>k__BackingField;
			}

			private void set_repeat(bool value)
			{
				this.<repeat>k__BackingField = value;
			}

			internal RecycledAction()
			{
				this.repeat = true;
			}

			float? Apex.LoadBalancing.ILoadBalanced.ExecuteUpdate(float deltaTime, float nextInterval)
			{
				this.repeat = this._action(deltaTime);
				if (!this.repeat)
				{
					LoadBalancedActionPool.Return(this);
				}
				return null;
			}
		}

		private class RecycledLongRunningAction : ILoadBalanced
		{
			private readonly Stopwatch _watch;

			private IEnumerator _iter;

			private int _maxMillisecondsUsedPerFrame;

			internal IEnumerator iter
			{
				get
				{
					return this._iter;
				}
				set
				{
					this._iter = value;
				}
			}

			internal int maxMillisecondsUsedPerFrame
			{
				get
				{
					return this._maxMillisecondsUsedPerFrame;
				}
				set
				{
					this._maxMillisecondsUsedPerFrame = value;
				}
			}

			public bool repeat
			{
				get
				{
					return get_repeat();
				}
				set
				{
					set_repeat(value);
				}
			}

			private bool <repeat>k__BackingField;

			public bool get_repeat()
			{
				return this.<repeat>k__BackingField;
			}

			private void set_repeat(bool value)
			{
				this.<repeat>k__BackingField = value;
			}

			internal RecycledLongRunningAction()
			{
				this.repeat = true;
			}

			float? Apex.LoadBalancing.ILoadBalanced.ExecuteUpdate(float deltaTime, float nextInterval)
			{
				bool flag = true;
				this._watch.Reset();
				this._watch.Start();
				while (flag && this._watch.ElapsedMilliseconds < (long)this._maxMillisecondsUsedPerFrame)
				{
					flag = this._iter.MoveNext();
				}
				this.repeat = flag;
				if (!flag)
				{
					LoadBalancedActionPool.Return(this);
				}
				return new float?(0f);
			}
		}

		private class RecycledOneTimeAction : ILoadBalanced
		{
			private Action _action;

			internal Action action
			{
				get
				{
					return this._action;
				}
				set
				{
					this._action = value;
				}
			}

			bool Apex.LoadBalancing.ILoadBalanced.repeat
			{
				get
				{
					return false;
				}
			}

			public RecycledOneTimeAction()
			{
			}

			float? Apex.LoadBalancing.ILoadBalanced.ExecuteUpdate(float deltaTime, float nextInterval)
			{
				this._action();
				LoadBalancedActionPool.Return(this);
				return null;
			}
		}
	}
}