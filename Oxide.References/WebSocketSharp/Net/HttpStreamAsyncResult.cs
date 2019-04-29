using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace WebSocketSharp.Net
{
	internal class HttpStreamAsyncResult : IAsyncResult
	{
		private byte[] _buffer;

		private AsyncCallback _callback;

		private bool _completed;

		private int _count;

		private System.Exception _exception;

		private int _offset;

		private object _state;

		private object _sync;

		private int _syncRead;

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

		internal byte[] Buffer
		{
			get
			{
				return this._buffer;
			}
			set
			{
				this._buffer = value;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return this._syncRead == this._count;
			}
		}

		internal int Count
		{
			get
			{
				return this._count;
			}
			set
			{
				this._count = value;
			}
		}

		internal System.Exception Exception
		{
			get
			{
				return this._exception;
			}
		}

		internal bool HasException
		{
			get
			{
				return this._exception != null;
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

		internal int Offset
		{
			get
			{
				return this._offset;
			}
			set
			{
				this._offset = value;
			}
		}

		internal int SyncRead
		{
			get
			{
				return this._syncRead;
			}
			set
			{
				this._syncRead = value;
			}
		}

		internal HttpStreamAsyncResult(AsyncCallback callback, object state)
		{
			this._callback = callback;
			this._state = state;
			this._sync = new object();
		}

		internal void Complete()
		{
			object obj = this._sync;
			Monitor.Enter(obj);
			try
			{
				if (!this._completed)
				{
					this._completed = true;
					if (this._waitHandle != null)
					{
						this._waitHandle.Set();
					}
					if (this._callback != null)
					{
						this._callback.BeginInvoke(this, (IAsyncResult ar) => this._callback.EndInvoke(ar), null);
					}
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}

		internal void Complete(System.Exception exception)
		{
			this._exception = exception;
			this.Complete();
		}
	}
}