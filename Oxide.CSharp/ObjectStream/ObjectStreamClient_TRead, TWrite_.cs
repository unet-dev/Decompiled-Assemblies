using ObjectStream.Threading;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ObjectStream
{
	public class ObjectStreamClient<TRead, TWrite>
	where TRead : class
	where TWrite : class
	{
		private readonly Stream _inStream;

		private readonly Stream _outStream;

		private ObjectStreamConnection<TRead, TWrite> _connection;

		public ObjectStreamClient(Stream inStream, Stream outStream)
		{
			this._inStream = inStream;
			this._outStream = outStream;
		}

		private void ConnectionOnError(ObjectStreamConnection<TRead, TWrite> connection, Exception exception)
		{
			this.OnError(exception);
		}

		private void ListenSync()
		{
			this._connection = ConnectionFactory.CreateConnection<TRead, TWrite>(this._inStream, this._outStream);
			this._connection.ReceiveMessage += new ConnectionMessageEventHandler<TRead, TWrite>(this.OnReceiveMessage);
			this._connection.Error += new ConnectionExceptionEventHandler<TRead, TWrite>(this.ConnectionOnError);
			this._connection.Open();
		}

		private void OnError(Exception exception)
		{
			if (this.Error != null)
			{
				this.Error(exception);
			}
		}

		private void OnReceiveMessage(ObjectStreamConnection<TRead, TWrite> connection, TRead message)
		{
			if (this.Message != null)
			{
				this.Message(connection, message);
			}
		}

		public void PushMessage(TWrite message)
		{
			if (this._connection != null)
			{
				this._connection.PushMessage(message);
			}
		}

		public void Start()
		{
			Worker worker = new Worker();
			worker.Error += new WorkerExceptionEventHandler(this.OnError);
			worker.DoWork(new Action(this.ListenSync));
		}

		public void Stop()
		{
			if (this._connection != null)
			{
				this._connection.Close();
			}
		}

		public event StreamExceptionEventHandler Error;

		public event ConnectionMessageEventHandler<TRead, TWrite> Message;
	}
}