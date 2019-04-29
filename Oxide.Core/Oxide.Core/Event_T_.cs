using System;
using System.Collections.Generic;

namespace Oxide.Core
{
	public class Event<T>
	{
		public Event.Callback<T> First;

		public Event.Callback<T> Last;

		internal object Lock;

		internal bool Invoking;

		internal Queue<Event.Callback<T>> RemovedQueue;

		public Event()
		{
		}

		public void Add(Event.Callback<T> callback)
		{
			callback.Handler = this;
			lock (this.Lock)
			{
				Event.Callback<T> last = this.Last;
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

		public Event.Callback<T> Add(Action<T> callback)
		{
			Event.Callback<T> callback1 = new Event.Callback<T>(callback);
			this.Add(callback1);
			return callback1;
		}

		public void Invoke(T arg0)
		{
			Event.Callback<T> i;
			lock (this.Lock)
			{
				this.Invoking = true;
				for (i = this.First; i != null; i = i.Next)
				{
					i.Call(arg0);
				}
				this.Invoking = false;
				Queue<Event.Callback<T>> removedQueue = this.RemovedQueue;
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