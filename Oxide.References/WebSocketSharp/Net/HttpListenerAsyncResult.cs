using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace WebSocketSharp.Net
{
	internal class HttpListenerAsyncResult : IAsyncResult
	{
		private AsyncCallback _callback;

		private bool _completed;

		private HttpListenerContext _context;

		private bool _endCalled;

		private Exception _exception;

		private bool _inGet;

		private object _state;

		private object _sync;

		private bool _syncCompleted;

		private ManualResetEvent _waitHandle;

		public object AsyncState
		{
			get
			{
				return this._state;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				WaitHandle waitHandle;
				object obj = this._sync;
				Monitor.Enter(obj);
				try
				{
					ManualResetEvent manualResetEvent = this._waitHandle;
					if (manualResetEvent == null)
					{
						ManualResetEvent manualResetEvent1 = new ManualResetEvent(this._completed);
						ManualResetEvent manualResetEvent2 = manualResetEvent1;
						this._waitHandle = manualResetEvent1;
						manualResetEvent = manualResetEvent2;
					}
					waitHandle = manualResetEvent;
				}
				finally
				{
					Monitor.Exit(obj);
				}
				return waitHandle;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return this._syncCompleted;
			}
		}

		internal bool EndCalled
		{
			get
			{
				return this._endCalled;
			}
			set
			{
				this._endCalled = value;
			}
		}

		internal bool InGet
		{
			get
			{
				return this._inGet;
			}
			set
			{
				this._inGet = value;
			}
		}

		public bool IsCompleted
		{
			get
			{
				bool flag;
				object obj = this._sync;
				Monitor.Enter(obj);
				try
				{
					flag = this._completed;
				}
				finally
				{
					Monitor.Exit(obj);
				}
				return flag;
			}
		}

		internal HttpListenerAsyncResult(AsyncCallback callback, object state)
		{
			this._callback = callback;
			this._state = state;
			this._sync = new object();
		}

		private static void complete(HttpListenerAsyncResult asyncResult)
		{
			object obj1 = asyncResult._sync;
			Monitor.Enter(obj1);
			try
			{
				asyncResult._completed = true;
				ManualResetEvent manualResetEvent = asyncResult._waitHandle;
				if (manualResetEvent != null)
				{
					manualResetEvent.Set();
				}
			}
			finally
			{
				Monitor.Exit(obj1);
			}
			AsyncCallback asyncCallback = asyncResult._callback;
			if (asyncCallback != null)
			{
				ThreadPool.QueueUserWorkItem((object state) => {
					try
					{
						asyncCallback(asyncResult);
					}
					catch
					{
					}
				}, null);
			}
		}

		internal void Complete(Exception exception)
		{
			Exception httpListenerException;
			if (!this._inGet || !(exception is ObjectDisposedException))
			{
				httpListenerException = exception;
			}
			else
			{
				httpListenerException = new HttpListenerException(995, "The listener is closed.");
			}
			this._exception = httpListenerException;
			HttpListenerAsyncResult.complete(this);
		}

		internal void Complete(HttpListenerContext context)
		{
			this.Complete(context, false);
		}

		internal void Complete(HttpListenerContext context, bool syncCompleted)
		{
			this._context = context;
			this._syncCompleted = syncCompleted;
			HttpListenerAsyncResult.complete(this);
		}

		internal HttpListenerContext GetContext()
		{
			if (this._exception != null)
			{
				throw this._exception;
			}
			return this._context;
		}
	}
}