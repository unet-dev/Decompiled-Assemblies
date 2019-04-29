using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ObjectStream.Threading
{
	internal class Worker
	{
		public Worker()
		{
		}

		private void Callback(Action action)
		{
			(new Thread(new ThreadStart(action.Invoke))
			{
				IsBackground = true
			}).Start();
		}

		public void DoWork(Action action)
		{
			(new Thread(new ParameterizedThreadStart(this.DoWorkImpl))
			{
				IsBackground = true
			}).Start(action);
		}

		private void DoWorkImpl(object oAction)
		{
			Action action = (Action)oAction;
			try
			{
				action();
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.Callback(() => this.Fail(exception));
			}
		}

		private void Fail(Exception exception)
		{
			if (this.Error != null)
			{
				this.Error(exception);
			}
		}

		public event WorkerExceptionEventHandler Error;
	}
}