using System;
using System.Collections.Generic;

namespace Oxide.Core
{
	public class Event<T1, T2>
	{
		public Event.Callback<T1, T2> First;

		public Event.Callback<T1, T2> Last;

		internal object Lock;

		internal bool Invoking;

		internal Queue<Event.Callback<T1, T2>> RemovedQueue;

		public Event()
		{
		}

		public void Add(Event.Callback<T1, T2> callback)
		{
			callback.Handler = this;
			lock (this.Lock)
			{
				Event.Callback<T1, T2> last = this.Last;
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

		public Event.Callback<T1, T2> Add(Action<T1, T2> callback)
		{
			Event.Callback<T1, T2> callback1 = new Event.Callback<T1, T2>(callback);
			this.Add(callback1);
			return callback1;
		}

		public void Invoke()
		{
			Event.Callback<T1, T2> i;
			lock (this.Lock)
			{
				this.Invoking = true;
				for (i = this.First; i != null; i = i.Next)
				{
					i.Call(default(T1), default(T2));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2>> removedQueue = this.RemovedQueue;
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
			Event.Callback<T1, T2> i;
			lock (this.Lock)
			{
				this.Invoking = true;
				for (i = this.First; i != null; i = i.Next)
				{
					i.Call(arg0, default(T2));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2>> removedQueue = this.RemovedQueue;
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
			Event.Callback<T1, T2> i;
			lock (this.Lock)
			{
				this.Invoking = true;
				for (i = this.First; i != null; i = i.Next)
				{
					i.Call(arg0, arg1);
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2>> removedQueue = this.RemovedQueue;
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