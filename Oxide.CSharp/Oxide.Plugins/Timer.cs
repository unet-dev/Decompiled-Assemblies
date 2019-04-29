using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using System;

namespace Oxide.Plugins
{
	public class Timer
	{
		private Oxide.Core.Libraries.Timer.TimerInstance instance;

		public Action Callback
		{
			get
			{
				return this.instance.Callback;
			}
		}

		public float Delay
		{
			get
			{
				return this.instance.Delay;
			}
		}

		public bool Destroyed
		{
			get
			{
				return this.instance.Destroyed;
			}
		}

		public Plugin Owner
		{
			get
			{
				return this.instance.Owner;
			}
		}

		public int Repetitions
		{
			get
			{
				return this.instance.Repetitions;
			}
		}

		public Timer(Oxide.Core.Libraries.Timer.TimerInstance instance)
		{
			this.instance = instance;
		}

		public void Destroy()
		{
			this.instance.Destroy();
		}

		public void DestroyToPool()
		{
			this.instance.DestroyToPool();
		}

		public void Reset(float delay = -1f, int repetitions = 1)
		{
			this.instance.Reset(delay, repetitions);
		}
	}
}