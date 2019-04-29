using System;
using System.Collections.Generic;
using System.Threading;

namespace Oxide.Core.Logging
{
	public abstract class ThreadedLogger : Logger
	{
		private AutoResetEvent waitevent;

		private bool exit;

		private object syncroot;

		private Thread workerthread;

		public ThreadedLogger() : base(false)
		{
			this.waitevent = new AutoResetEvent(false);
			this.exit = false;
			this.syncroot = new object();
			this.workerthread = new Thread(new ThreadStart(this.Worker))
			{
				IsBackground = true
			};
			this.workerthread.Start();
		}

		protected abstract void BeginBatchProcess();

		protected override void Finalize()
		{
			try
			{
				this.OnRemoved();
			}
			finally
			{
				base.Finalize();
			}
		}

		protected abstract void FinishBatchProcess();

		public override void OnRemoved()
		{
			if (this.exit)
			{
				return;
			}
			this.exit = true;
			this.waitevent.Set();
			this.workerthread.Join();
		}

		private void Worker()
		{
			while (!this.exit)
			{
				this.waitevent.WaitOne();
				lock (this.syncroot)
				{
					if (this.MessageQueue.Count > 0)
					{
						this.BeginBatchProcess();
						try
						{
							while (this.MessageQueue.Count > 0)
							{
								this.ProcessMessage(this.MessageQueue.Dequeue());
							}
						}
						finally
						{
							this.FinishBatchProcess();
						}
					}
				}
			}
		}

		internal override void Write(Logger.LogMessage msg)
		{
			lock (this.syncroot)
			{
				base.Write(msg);
			}
			this.waitevent.Set();
		}
	}
}