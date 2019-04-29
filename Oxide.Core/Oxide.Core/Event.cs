using System;
using System.Collections.Generic;

namespace Oxide.Core
{
	public class Event
	{
		public Event.Callback First;

		public Event.Callback Last;

		internal object Lock = new object();

		internal bool Invoking;

		internal Queue<Event.Callback> RemovedQueue = new Queue<Event.Callback>();

		public Event()
		{
		}

		public void Add(Event.Callback callback)
		{
			callback.Handler = this;
			lock (this.Lock)
			{
				Event.Callback last = this.Last;
				if (last != null)
				{
					last.Next = callback;
					callback.Previous = last;
					this.Last = callback;
				}
				else
				{
					this.First = callback;
					this.Last = callback;
				}
			}
		}

		public Event.Callback Add(Action callback)
		{
			Event.Callback callback1 = new Event.Callback(callback);
			this.Add(callback1);
			return callback1;
		}

		public void Invoke()
		{
			Event.Callback i;
			lock (this.Lock)
			{
				this.Invoking = true;
				for (i = this.First; i != null; i = i.Next)
				{
					i.Call();
				}
				this.Invoking = false;
				Queue<Event.Callback> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					i = removedQueue.Dequeue();
					i.Previous = null;
					i.Next = null;
				}
			}
		}

		public static void Remove(ref Event.Callback callback)
		{
			if (callback == null)
			{
				return;
			}
			callback.Remove();
			callback = null;
		}

		public static void Remove<T1>(ref Event.Callback<T1> callback)
		{
			if (callback == null)
			{
				return;
			}
			callback.Remove();
			callback = null;
		}

		public static void Remove<T1, T2>(ref Event.Callback<T1, T2> callback)
		{
			if (callback == null)
			{
				return;
			}
			callback.Remove();
			callback = null;
		}

		public static void Remove<T1, T2, T3>(ref Event.Callback<T1, T2, T3> callback)
		{
			if (callback == null)
			{
				return;
			}
			callback.Remove();
			callback = null;
		}

		public static void Remove<T1, T2, T3, T4>(ref Event.Callback<T1, T2, T3, T4> callback)
		{
			if (callback == null)
			{
				return;
			}
			callback.Remove();
			callback = null;
		}

		public static void Remove<T1, T2, T3, T4, T5>(ref Event.Callback<T1, T2, T3, T4, T5> callback)
		{
			if (callback == null)
			{
				return;
			}
			callback.Remove();
			callback = null;
		}

		public delegate void Action<in T1, in T2, in T3, in T4, in T5>(T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4);

		public class Callback
		{
			public Action Invoke;

			internal Event.Callback Previous;

			internal Event.Callback Next;

			internal Event Handler;

			public Callback(Action callback)
			{
				this.Invoke = callback;
			}

			public void Call()
			{
				Action invoke = this.Invoke;
				if (invoke == null)
				{
					return;
				}
				try
				{
					invoke();
				}
				catch (Exception exception)
				{
					Interface.Oxide.LogException("Exception while invoking event handler", exception);
				}
			}

			public void Remove()
			{
				Event handler = this.Handler;
				Event.Callback next = this.Next;
				Event.Callback previous = this.Previous;
				if (previous != null)
				{
					previous.Next = next;
					if (next == null)
					{
						handler.Last = previous;
					}
				}
				else
				{
					handler.First = next;
				}
				if (next != null)
				{
					next.Previous = previous;
					if (previous == null)
					{
						handler.First = next;
					}
				}
				else
				{
					handler.Last = previous;
				}
				if (!handler.Invoking)
				{
					this.Previous = null;
					this.Next = null;
				}
				else
				{
					handler.RemovedQueue.Enqueue(this);
				}
				this.Invoke = null;
				this.Handler = null;
			}
		}

		public class Callback<T>
		{
			public Action<T> Invoke;

			internal Event.Callback<T> Previous;

			internal Event.Callback<T> Next;

			internal Event<T> Handler;

			public Callback(Action<T> callback)
			{
				this.Invoke = callback;
			}

			public void Call(T arg0)
			{
				Action<T> invoke = this.Invoke;
				if (invoke == null)
				{
					return;
				}
				try
				{
					invoke(arg0);
				}
				catch (Exception exception)
				{
					Interface.Oxide.LogException("Exception while invoking event handler", exception);
				}
			}

			public void Remove()
			{
				Event<T> handler = this.Handler;
				Event.Callback<T> next = this.Next;
				Event.Callback<T> previous = this.Previous;
				if (previous != null)
				{
					previous.Next = next;
					if (next == null)
					{
						handler.Last = previous;
					}
				}
				else
				{
					handler.First = next;
				}
				if (next != null)
				{
					next.Previous = previous;
					if (previous == null)
					{
						handler.First = next;
					}
				}
				else
				{
					handler.Last = previous;
				}
				if (!handler.Invoking)
				{
					this.Previous = null;
					this.Next = null;
				}
				else
				{
					handler.RemovedQueue.Enqueue(this);
				}
				this.Invoke = null;
				this.Handler = null;
			}
		}

		public class Callback<T1, T2>
		{
			public Action<T1, T2> Invoke;

			internal Event.Callback<T1, T2> Previous;

			internal Event.Callback<T1, T2> Next;

			internal Event<T1, T2> Handler;

			public Callback(Action<T1, T2> callback)
			{
				this.Invoke = callback;
			}

			public void Call(T1 arg0, T2 arg1)
			{
				Action<T1, T2> invoke = this.Invoke;
				if (invoke == null)
				{
					return;
				}
				try
				{
					invoke(arg0, arg1);
				}
				catch (Exception exception)
				{
					Interface.Oxide.LogException("Exception while invoking event handler", exception);
				}
			}

			public void Remove()
			{
				Event<T1, T2> handler = this.Handler;
				Event.Callback<T1, T2> next = this.Next;
				Event.Callback<T1, T2> previous = this.Previous;
				if (previous != null)
				{
					previous.Next = next;
					if (next == null)
					{
						handler.Last = previous;
					}
				}
				else
				{
					handler.First = next;
				}
				if (next != null)
				{
					next.Previous = previous;
					if (previous == null)
					{
						handler.First = next;
					}
				}
				else
				{
					handler.Last = previous;
				}
				if (!handler.Invoking)
				{
					this.Previous = null;
					this.Next = null;
				}
				else
				{
					handler.RemovedQueue.Enqueue(this);
				}
				this.Invoke = null;
				this.Handler = null;
			}
		}

		public class Callback<T1, T2, T3>
		{
			public Action<T1, T2, T3> Invoke;

			internal Event.Callback<T1, T2, T3> Previous;

			internal Event.Callback<T1, T2, T3> Next;

			internal Event<T1, T2, T3> Handler;

			public Callback(Action<T1, T2, T3> callback)
			{
				this.Invoke = callback;
			}

			public void Call(T1 arg0, T2 arg1, T3 arg2)
			{
				Action<T1, T2, T3> invoke = this.Invoke;
				if (invoke == null)
				{
					return;
				}
				try
				{
					invoke(arg0, arg1, arg2);
				}
				catch (Exception exception)
				{
					Interface.Oxide.LogException("Exception while invoking event handler", exception);
				}
			}

			public void Remove()
			{
				Event<T1, T2, T3> handler = this.Handler;
				Event.Callback<T1, T2, T3> next = this.Next;
				Event.Callback<T1, T2, T3> previous = this.Previous;
				if (previous != null)
				{
					previous.Next = next;
					if (next == null)
					{
						handler.Last = previous;
					}
				}
				else
				{
					handler.First = next;
				}
				if (next != null)
				{
					next.Previous = previous;
					if (previous == null)
					{
						handler.First = next;
					}
				}
				else
				{
					handler.Last = previous;
				}
				if (!handler.Invoking)
				{
					this.Previous = null;
					this.Next = null;
				}
				else
				{
					handler.RemovedQueue.Enqueue(this);
				}
				this.Invoke = null;
				this.Handler = null;
			}
		}

		public class Callback<T1, T2, T3, T4>
		{
			public Action<T1, T2, T3, T4> Invoke;

			internal Event.Callback<T1, T2, T3, T4> Previous;

			internal Event.Callback<T1, T2, T3, T4> Next;

			internal Event<T1, T2, T3, T4> Handler;

			public Callback(Action<T1, T2, T3, T4> callback)
			{
				this.Invoke = callback;
			}

			public void Call(T1 arg0, T2 arg1, T3 arg2, T4 arg3)
			{
				Action<T1, T2, T3, T4> invoke = this.Invoke;
				if (invoke == null)
				{
					return;
				}
				try
				{
					invoke(arg0, arg1, arg2, arg3);
				}
				catch (Exception exception)
				{
					Interface.Oxide.LogException("Exception while invoking event handler", exception);
				}
			}

			public void Remove()
			{
				Event<T1, T2, T3, T4> handler = this.Handler;
				Event.Callback<T1, T2, T3, T4> next = this.Next;
				Event.Callback<T1, T2, T3, T4> previous = this.Previous;
				if (previous != null)
				{
					previous.Next = next;
					if (next == null)
					{
						handler.Last = previous;
					}
				}
				else
				{
					handler.First = next;
				}
				if (next != null)
				{
					next.Previous = previous;
					if (previous == null)
					{
						handler.First = next;
					}
				}
				else
				{
					handler.Last = previous;
				}
				if (!handler.Invoking)
				{
					this.Previous = null;
					this.Next = null;
				}
				else
				{
					handler.RemovedQueue.Enqueue(this);
				}
				this.Invoke = null;
				this.Handler = null;
			}
		}

		public class Callback<T1, T2, T3, T4, T5>
		{
			public Event.Action<T1, T2, T3, T4, T5> Invoke;

			internal Event.Callback<T1, T2, T3, T4, T5> Previous;

			internal Event.Callback<T1, T2, T3, T4, T5> Next;

			internal Event<T1, T2, T3, T4, T5> Handler;

			public Callback(Event.Action<T1, T2, T3, T4, T5> callback)
			{
				this.Invoke = callback;
			}

			public void Call(T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4)
			{
				Event.Action<T1, T2, T3, T4, T5> invoke = this.Invoke;
				if (invoke == null)
				{
					return;
				}
				try
				{
					invoke(arg0, arg1, arg2, arg3, arg4);
				}
				catch (Exception exception)
				{
					Interface.Oxide.LogException("Exception while invoking event handler", exception);
				}
			}

			public void Remove()
			{
				Event<T1, T2, T3, T4, T5> handler = this.Handler;
				Event.Callback<T1, T2, T3, T4, T5> next = this.Next;
				Event.Callback<T1, T2, T3, T4, T5> previous = this.Previous;
				if (previous != null)
				{
					previous.Next = next;
					if (next == null)
					{
						handler.Last = previous;
					}
				}
				else
				{
					handler.First = next;
				}
				if (next != null)
				{
					next.Previous = previous;
					if (previous == null)
					{
						handler.First = next;
					}
				}
				else
				{
					handler.Last = previous;
				}
				if (!handler.Invoking)
				{
					this.Previous = null;
					this.Next = null;
				}
				else
				{
					handler.RemovedQueue.Enqueue(this);
				}
				this.Invoke = null;
				this.Handler = null;
			}
		}
	}
}