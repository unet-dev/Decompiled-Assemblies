using Oxide.Core;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Libraries
{
	public class Timer : Library
	{
		internal readonly static object Lock;

		internal readonly static OxideMod Oxide;

		public const int TimeSlots = 512;

		public const int LastTimeSlot = 511;

		public const float TickDuration = 0.01f;

		private readonly Timer.TimeSlot[] timeSlots = new Timer.TimeSlot[512];

		private readonly Queue<Timer.TimerInstance> expiredInstanceQueue = new Queue<Timer.TimerInstance>();

		private int currentSlot;

		private double nextSlotAt = 0.00999999977648258;

		public static int Count
		{
			get;
			private set;
		}

		public override bool IsGlobal
		{
			get
			{
				return false;
			}
		}

		static Timer()
		{
			Timer.Lock = new object();
			Timer.Oxide = Interface.Oxide;
		}

		public Timer()
		{
			for (int i = 0; i < 512; i++)
			{
				this.timeSlots[i] = new Timer.TimeSlot();
			}
		}

		internal Timer.TimerInstance AddTimer(int repetitions, float delay, Action callback, Plugin owner = null)
		{
			Timer.TimerInstance timerInstance;
			Timer.TimerInstance timerInstance1;
			lock (Timer.Lock)
			{
				Queue<Timer.TimerInstance> pool = Timer.TimerInstance.Pool;
				if (pool.Count <= 0)
				{
					timerInstance = new Timer.TimerInstance(this, repetitions, delay, callback, owner);
				}
				else
				{
					timerInstance = pool.Dequeue();
					timerInstance.Load(this, repetitions, delay, callback, owner);
				}
				this.InsertTimer(timerInstance, timerInstance.ExpiresAt < Timer.Oxide.Now);
				timerInstance1 = timerInstance;
			}
			return timerInstance1;
		}

		private void InsertTimer(Timer.TimerInstance timer, bool in_past = false)
		{
			this.timeSlots[(in_past ? this.currentSlot : (int)(timer.ExpiresAt / 0.01f) & 511)].InsertTimer(timer);
		}

		[LibraryFunction("NextFrame")]
		public Timer.TimerInstance NextFrame(Action callback)
		{
			return this.AddTimer(1, 0f, callback, null);
		}

		[LibraryFunction("Once")]
		public Timer.TimerInstance Once(float delay, Action callback, Plugin owner = null)
		{
			return this.AddTimer(1, delay, callback, owner);
		}

		[LibraryFunction("Repeat")]
		public Timer.TimerInstance Repeat(float delay, int reps, Action callback, Plugin owner = null)
		{
			return this.AddTimer(reps, delay, callback, owner);
		}

		public void Update(float delta)
		{
			float now = Timer.Oxide.Now;
			Timer.TimeSlot[] timeSlotArray = this.timeSlots;
			Queue<Timer.TimerInstance> timerInstances = this.expiredInstanceQueue;
			int num = 0;
			lock (Timer.Lock)
			{
				int num1 = this.currentSlot;
				double num2 = this.nextSlotAt;
				while (true)
				{
					timeSlotArray[num1].GetExpired((num2 > (double)now ? (double)now : num2), timerInstances);
					if ((double)now <= num2)
					{
						break;
					}
					num++;
					num1 = (num1 < 511 ? num1 + 1 : 0);
					num2 += 0.00999999977648258;
				}
				if (num > 0)
				{
					this.currentSlot = num1;
					this.nextSlotAt = num2;
				}
				int count = timerInstances.Count;
				for (int i = 0; i < count; i++)
				{
					Timer.TimerInstance timerInstance = timerInstances.Dequeue();
					if (!timerInstance.Destroyed)
					{
						timerInstance.Invoke(now);
					}
				}
			}
		}

		public class TimerInstance
		{
			public const int MaxPooled = 5000;

			internal static Queue<Timer.TimerInstance> Pool;

			internal float ExpiresAt;

			internal Timer.TimeSlot TimeSlot;

			internal Timer.TimerInstance NextInstance;

			internal Timer.TimerInstance PreviousInstance;

			private Event.Callback<Plugin, PluginManager> removedFromManager;

			private Timer timer;

			public Action Callback
			{
				get;
				private set;
			}

			public float Delay
			{
				get;
				private set;
			}

			public bool Destroyed
			{
				get;
				private set;
			}

			public Plugin Owner
			{
				get;
				private set;
			}

			public int Repetitions
			{
				get;
				private set;
			}

			static TimerInstance()
			{
				Timer.TimerInstance.Pool = new Queue<Timer.TimerInstance>();
			}

			internal TimerInstance(Timer timer, int repetitions, float delay, Action callback, Plugin owner)
			{
				this.Load(timer, repetitions, delay, callback, owner);
			}

			internal void Added(Timer.TimeSlot time_slot)
			{
				time_slot.Count++;
				Timer.Count = Timer.Count + 1;
				this.TimeSlot = time_slot;
			}

			public bool Destroy()
			{
				bool flag;
				lock (Timer.Lock)
				{
					if (!this.Destroyed)
					{
						this.Destroyed = true;
						this.Remove();
						Event.Remove<Plugin, PluginManager>(ref this.removedFromManager);
						return true;
					}
					else
					{
						flag = false;
					}
				}
				return flag;
			}

			public bool DestroyToPool()
			{
				bool flag;
				lock (Timer.Lock)
				{
					if (!this.Destroyed)
					{
						this.Destroyed = true;
						this.Callback = null;
						this.Remove();
						Event.Remove<Plugin, PluginManager>(ref this.removedFromManager);
						Queue<Timer.TimerInstance> pool = Timer.TimerInstance.Pool;
						if (pool.Count < 5000)
						{
							pool.Enqueue(this);
						}
						return true;
					}
					else
					{
						flag = false;
					}
				}
				return flag;
			}

			private void FireCallback()
			{
				Plugin owner = this.Owner;
				if (owner != null)
				{
					owner.TrackStart();
				}
				else
				{
				}
				try
				{
					try
					{
						this.Callback();
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						this.Destroy();
						string str = string.Format("Failed to run a {0:0.00} timer", this.Delay);
						if (this.Owner && this.Owner != null)
						{
							str = string.Concat(str, string.Format(" in '{0} v{1}'", this.Owner.Name, this.Owner.Version));
						}
						Interface.Oxide.LogException(str, exception);
					}
				}
				finally
				{
					Plugin plugin = this.Owner;
					if (plugin != null)
					{
						plugin.TrackEnd();
					}
					else
					{
					}
				}
			}

			internal void Invoke(float now)
			{
				if (this.Repetitions > 0)
				{
					int repetitions = this.Repetitions - 1;
					this.Repetitions = repetitions;
					if (repetitions == 0)
					{
						this.Destroy();
						this.FireCallback();
						return;
					}
				}
				this.Remove();
				float expiresAt = this.ExpiresAt + this.Delay;
				this.ExpiresAt = expiresAt;
				this.timer.InsertTimer(this, expiresAt < now);
				this.FireCallback();
			}

			internal void Load(Timer timer, int repetitions, float delay, Action callback, Plugin owner)
			{
				this.timer = timer;
				this.Repetitions = repetitions;
				this.Delay = delay;
				this.Callback = callback;
				this.ExpiresAt = Timer.Oxide.Now + delay;
				this.Owner = owner;
				this.Destroyed = false;
				if (owner != null)
				{
					this.removedFromManager = owner.OnRemovedFromManager.Add(new Action<Plugin, PluginManager>(this.OnRemovedFromManager));
				}
			}

			private void OnRemovedFromManager(Plugin sender, PluginManager manager)
			{
				this.Destroy();
			}

			internal void Remove()
			{
				Timer.TimeSlot timeSlot = this.TimeSlot;
				if (timeSlot == null)
				{
					return;
				}
				timeSlot.Count--;
				Timer.Count = Timer.Count - 1;
				Timer.TimerInstance previousInstance = this.PreviousInstance;
				Timer.TimerInstance nextInstance = this.NextInstance;
				if (nextInstance != null)
				{
					nextInstance.PreviousInstance = previousInstance;
				}
				else
				{
					timeSlot.LastInstance = previousInstance;
				}
				if (previousInstance != null)
				{
					previousInstance.NextInstance = nextInstance;
				}
				else
				{
					timeSlot.FirstInstance = nextInstance;
				}
				this.TimeSlot = null;
				this.PreviousInstance = null;
				this.NextInstance = null;
			}

			public void Reset(float delay = -1f, int repetitions = 1)
			{
				lock (Timer.Lock)
				{
					if (delay >= 0f)
					{
						this.Delay = delay;
					}
					else
					{
						delay = this.Delay;
					}
					this.Repetitions = repetitions;
					this.ExpiresAt = Timer.Oxide.Now + delay;
					if (!this.Destroyed)
					{
						this.Remove();
					}
					else
					{
						this.Destroyed = false;
						Plugin owner = this.Owner;
						if (owner != null)
						{
							this.removedFromManager = owner.OnRemovedFromManager.Add(new Action<Plugin, PluginManager>(this.OnRemovedFromManager));
						}
					}
					this.timer.InsertTimer(this, false);
				}
			}
		}

		public class TimeSlot
		{
			public int Count;

			public Timer.TimerInstance FirstInstance;

			public Timer.TimerInstance LastInstance;

			public TimeSlot()
			{
			}

			public void GetExpired(double now, Queue<Timer.TimerInstance> queue)
			{
				for (Timer.TimerInstance i = this.FirstInstance; i != null && (double)i.ExpiresAt <= now; i = i.NextInstance)
				{
					queue.Enqueue(i);
				}
			}

			public void InsertTimer(Timer.TimerInstance timer)
			{
				float expiresAt = timer.ExpiresAt;
				Timer.TimerInstance firstInstance = this.FirstInstance;
				Timer.TimerInstance lastInstance = this.LastInstance;
				Timer.TimerInstance nextInstance = firstInstance;
				if (firstInstance != null)
				{
					float single = firstInstance.ExpiresAt;
					float expiresAt1 = lastInstance.ExpiresAt;
					if (expiresAt <= single)
					{
						nextInstance = firstInstance;
					}
					else if (expiresAt >= expiresAt1)
					{
						nextInstance = null;
					}
					else if (expiresAt1 - expiresAt >= expiresAt - single)
					{
						while (nextInstance != null && nextInstance.ExpiresAt <= expiresAt)
						{
							nextInstance = nextInstance.NextInstance;
						}
					}
					else
					{
						nextInstance = lastInstance;
						for (Timer.TimerInstance i = nextInstance; i != null; i = i.PreviousInstance)
						{
							if (i.ExpiresAt <= expiresAt)
							{
								goto Label0;
							}
							nextInstance = i;
						}
					}
				}
			Label0:
				if (nextInstance != null)
				{
					Timer.TimerInstance previousInstance = nextInstance.PreviousInstance;
					if (previousInstance != null)
					{
						previousInstance.NextInstance = timer;
					}
					else
					{
						this.FirstInstance = timer;
					}
					nextInstance.PreviousInstance = timer;
					timer.PreviousInstance = previousInstance;
					timer.NextInstance = nextInstance;
				}
				else
				{
					timer.NextInstance = null;
					if (lastInstance != null)
					{
						lastInstance.NextInstance = timer;
						timer.PreviousInstance = lastInstance;
						this.LastInstance = timer;
					}
					else
					{
						this.FirstInstance = timer;
						this.LastInstance = timer;
					}
				}
				timer.Added(this);
			}
		}
	}
}