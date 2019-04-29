using Apex.Utilities;
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Apex.LoadBalancing
{
	public class LongRunningAction : ILoadBalanced
	{
		private Func<IEnumerator> _action;

		private Action _callback;

		private int _maxMillisecondUsedPerFrame;

		private IEnumerator _iter;

		private Stopwatch _watch;

		public bool repeat
		{
			get
			{
				return JustDecompileGenerated_get_repeat();
			}
			set
			{
				JustDecompileGenerated_set_repeat(value);
			}
		}

		private bool JustDecompileGenerated_repeat_k__BackingField;

		public bool JustDecompileGenerated_get_repeat()
		{
			return this.JustDecompileGenerated_repeat_k__BackingField;
		}

		private void JustDecompileGenerated_set_repeat(bool value)
		{
			this.JustDecompileGenerated_repeat_k__BackingField = value;
		}

		public LongRunningAction(Func<IEnumerator> action, int maxMillisecondUsedPerFrame) : this(action, maxMillisecondUsedPerFrame, null)
		{
		}

		public LongRunningAction(Func<IEnumerator> action, int maxMillisecondUsedPerFrame, Action callback)
		{
			Ensure.ArgumentNotNull(action, "action");
			this._action = action;
			this._maxMillisecondUsedPerFrame = maxMillisecondUsedPerFrame;
			this._callback = callback;
			this._watch = new Stopwatch();
		}

		public LongRunningAction(IEnumerator action, int maxMillisecondUsedPerFrame) : this(action, maxMillisecondUsedPerFrame, null)
		{
		}

		public LongRunningAction(IEnumerator action, int maxMillisecondUsedPerFrame, Action callback)
		{
			Ensure.ArgumentNotNull(action, "action");
			this._iter = action;
			this._maxMillisecondUsedPerFrame = maxMillisecondUsedPerFrame;
			this._callback = callback;
			this._watch = new Stopwatch();
		}

		public float? ExecuteUpdate(float deltaTime, float nextInterval)
		{
			if (this._iter == null)
			{
				this._iter = this._action();
			}
			bool flag = true;
			this._watch.Reset();
			this._watch.Start();
			while (flag && this._watch.ElapsedMilliseconds < (long)this._maxMillisecondUsedPerFrame)
			{
				flag = this._iter.MoveNext();
			}
			this.repeat = flag;
			if (!flag)
			{
				this._iter = null;
				if (this._callback != null)
				{
					this._callback();
				}
			}
			return new float?(0f);
		}
	}
}