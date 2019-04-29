using System;
using System.Collections.Generic;

namespace Oxide.Core
{
	public class Event<T1, T2, T3, T4, T5>
	{
		public Event.Callback<T1, T2, T3, T4, T5> First;

		public Event.Callback<T1, T2, T3, T4, T5> Last;

		internal object Lock;

		internal bool Invoking;

		internal Queue<Event.Callback<T1, T2, T3, T4, T5>> RemovedQueue;

		public Event()
		{
		}

		public void Add(Event.Callback<T1, T2, T3, T4, T5> callback)
		{
			callback.Handler = this;
			lock (this.Lock)
			{
				Event.Callback<T1, T2, T3, T4, T5> last = this.Last;
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

		public Event.Callback<T1, T2, T3, T4, T5> Add(Event.Action<T1, T2, T3, T4, T5> callback)
		{
			Event.Callback<T1, T2, T3, T4, T5> callback1 = new Event.Callback<T1, T2, T3, T4, T5>(callback);
			this.Add(callback1);
			return callback1;
		}

		public void Invoke()
		{
			Event.Callback<T1, T2, T3, T4, T5> i;
			lock (this.Lock)
			{
				this.Invoking = true;
				for (i = this.First; i != null; i = i.Next)
				{
					i.Call(default(T1), default(T2), default(T3), default(T4), default(T5));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4, T5>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					i = removedQueue.Dequeue();
					i.Previous = null;
					i.Next = null;
				}
			}
		}

		public void Invoke(T1 arg0)
		{
			Event.Callback<T1, T2, T3, T4, T5> i;
			lock (this.Lock)
			{
				this.Invoking = true;
				for (i = this.First; i != null; i = i.Next)
				{
					i.Call(arg0, default(T2), default(T3), default(T4), default(T5));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4, T5>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					i = removedQueue.Dequeue();
					i.Previous = null;
					i.Next = null;
				}
			}
		}

		public void Invoke(T1 arg0, T2 arg1)
		{
			Event.Callback<T1, T2, T3, T4, T5> i;
			lock (this.Lock)
			{
				this.Invoking = true;
				for (i = this.First; i != null; i = i.Next)
				{
					i.Call(arg0, arg1, default(T3), default(T4), default(T5));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4, T5>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					i = removedQueue.Dequeue();
					i.Previous = null;
					i.Next = null;
				}
			}
		}

		public void Invoke(T1 arg0, T2 arg1, T3 arg2)
		{
			Event.Callback<T1, T2, T3, T4, T5> i;
			lock (this.Lock)
			{
				this.Invoking = true;
				for (i = this.First; i != null; i = i.Next)
				{
					i.Call(arg0, arg1, arg2, default(T4), default(T5));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4, T5>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					i = removedQueue.Dequeue();
					i.Previous = null;
					i.Next = null;
				}
			}
		}

		public void Invoke(T1 arg0, T2 arg1, T3 arg2, T4 arg3)
		{
			Event.Callback<T1, T2, T3, T4, T5> i;
			lock (this.Lock)
			{
				this.Invoking = true;
				for (i = this.First; i != null; i = i.Next)
				{
					i.Call(arg0, arg1, arg2, arg3, default(T5));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4, T5>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					i = removedQueue.Dequeue();
					i.Previous = null;
					i.Next = null;
				}
			}
		}

		public void Invoke(T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4)
		{
			Event.Callback<T1, T2, T3, T4, T5> i;
			lock (this.Lock)
			{
				this.Invoking = true;
				for (i = this.First; i != null; i = i.Next)
				{
					i.Call(arg0, arg1, arg2, arg3, arg4);
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4, T5>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					i = removedQueue.Dequeue();
					i.Previous = null;
					i.Next = null;
				}
			}
		}
	}
}