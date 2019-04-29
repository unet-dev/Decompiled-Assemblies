using ObjectStream.IO;
using ObjectStream.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ObjectStream
{
	public class ObjectStreamConnection<TRead, TWrite>
	where TRead : class
	where TWrite : class
	{
		private readonly ObjectStreamWrapper<TRead, TWrite> _streamWrapper;

		private readonly Queue<TWrite> _writeQueue;

		private readonly AutoResetEvent _writeSignal;

		internal ObjectStreamConnection(Stream inStream, Stream outStream)
		{
			this._streamWrapper = new ObjectStreamWrapper<TRead, TWrite>(inStream, outStream);
		}

		public void Close()
		{
			this.CloseImpl();
		}

		private void CloseImpl()
		{
			this.Error = null;
			this._streamWrapper.Close();
			this._writeSignal.Set();
		}

		private void OnError(Exception exception)
		{
			if (this.Error != null)
			{
				this.Error(this, exception);
			}
		}

		public void Open()
		{
			Worker worker = new Worker();
			worker.Error += new WorkerExceptionEventHandler(this.OnError);
			worker.DoWork(new Action(this.ReadStream));
			Worker worker1 = new Worker();
			worker1.Error += new WorkerExceptionEventHandler(this.OnError);
			worker1.DoWork(new Action(this.WriteStream));
		}

		public void PushMessage(TWrite message)
		{
			this._writeQueue.Enqueue(message);
			this._writeSignal.Set();
		}

		private void ReadStream()
		{
			while (this._streamWrapper.CanRead)
			{
				TRead tRead = this._streamWrapper.ReadObject();
				ConnectionMessageEventHandler<TRead, TWrite> connectionMessageEventHandler = this.ReceiveMessage;
				if (connectionMessageEventHandler != null)
				{
					connectionMessageEventHandler(this, tRead);
				}
				else
				{
				}
				if (tRead != null)
				{
					continue;
				}
				this.CloseImpl();
				return;
			}
		}

		private void WriteStream()
		{
			while (this._streamWrapper.CanWrite)
			{
				this._writeSignal.WaitOne();
				while (this._writeQueue.Count > 0)
				{
					this._streamWrapper.WriteObject(this._writeQueue.Dequeue());
				}
			}
		}

		public event ConnectionExceptionEventHandler<TRead, TWrite> Error;

		public event ConnectionMessageEventHandler<TRead, TWrite> ReceiveMessage;
	}
}