using Apex.DataStructures;
using System;
using System.Diagnostics;

namespace Apex.LoadBalancing
{
	public sealed class Marshaller : IMarshaller
	{
		private readonly Stopwatch _watch;

		private readonly SimpleQueue<Action> _queue;

		private readonly int _maxMillisecondsPerFrame;

		internal Marshaller(int maxMillisecondsPerFrame)
		{
			this._maxMillisecondsPerFrame = maxMillisecondsPerFrame;
			this._queue = new SimpleQueue<Action>(10);
			this._watch = new Stopwatch();
		}

		public void ExecuteOnMainThread(Action a)
		{
			lock (this._queue)
			{
				this._queue.Enqueue(a);
			}
		}

		internal void ProcessPending()
		{
			Action action;
			if (this._queue.count == 0)
			{
				return;
			}
			this._watch.Start();
			do
			{
				lock (this._queue)
				{
					action = this._queue.Dequeue();
				}
				action();
			}
			while (this._queue.count > 0 && this._watch.ElapsedMilliseconds < (long)this._maxMillisecondsPerFrame);
			this._watch.Reset();
		}
	}
}