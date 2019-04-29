using System;
using System.IO;
using System.Threading;

namespace WebSocketSharp.Net
{
	internal class ChunkedRequestStream : RequestStream
	{
		private const int _bufferLength = 8192;

		private HttpListenerContext _context;

		private ChunkStream _decoder;

		private bool _disposed;

		private bool _noMoreData;

		internal ChunkStream Decoder
		{
			get
			{
				return this._decoder;
			}
			set
			{
				this._decoder = value;
			}
		}

		internal ChunkedRequestStream(Stream stream, byte[] buffer, int offset, int count, HttpListenerContext context) : base(stream, buffer, offset, count)
		{
			this._context = context;
			this._decoder = new ChunkStream((WebHeaderCollection)context.Request.Headers);
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			IAsyncResult asyncResult;
			if (this._disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "A negative value.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "A negative value.");
			}
			if (offset + count > (int)buffer.Length)
			{
				throw new ArgumentException("The sum of 'offset' and 'count' is greater than 'buffer' length.");
			}
			HttpStreamAsyncResult httpStreamAsyncResult = new HttpStreamAsyncResult(callback, state);
			if (!this._noMoreData)
			{
				int num = this._decoder.Read(buffer, offset, count);
				offset += num;
				count -= num;
				if (count == 0)
				{
					httpStreamAsyncResult.Count = num;
					httpStreamAsyncResult.Complete();
					asyncResult = httpStreamAsyncResult;
				}
				else if (this._decoder.WantMore)
				{
					httpStreamAsyncResult.Buffer = new byte[8192];
					httpStreamAsyncResult.Offset = 0;
					httpStreamAsyncResult.Count = 8192;
					ReadBufferState readBufferState = new ReadBufferState(buffer, offset, count, httpStreamAsyncResult);
					ReadBufferState initialCount = readBufferState;
					initialCount.InitialCount = initialCount.InitialCount + num;
					base.BeginRead(httpStreamAsyncResult.Buffer, httpStreamAsyncResult.Offset, httpStreamAsyncResult.Count, new AsyncCallback(this.onRead), readBufferState);
					asyncResult = httpStreamAsyncResult;
				}
				else
				{
					this._noMoreData = num == 0;
					httpStreamAsyncResult.Count = num;
					httpStreamAsyncResult.Complete();
					asyncResult = httpStreamAsyncResult;
				}
			}
			else
			{
				httpStreamAsyncResult.Complete();
				asyncResult = httpStreamAsyncResult;
			}
			return asyncResult;
		}

		public override void Close()
		{
			if (!this._disposed)
			{
				this._disposed = true;
				base.Close();
			}
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			HttpStreamAsyncResult httpStreamAsyncResult = asyncResult as HttpStreamAsyncResult;
			if (httpStreamAsyncResult == null)
			{
				throw new ArgumentException("A wrong IAsyncResult.", "asyncResult");
			}
			if (!httpStreamAsyncResult.IsCompleted)
			{
				httpStreamAsyncResult.AsyncWaitHandle.WaitOne();
			}
			if (httpStreamAsyncResult.HasException)
			{
				throw new HttpListenerException(400, "I/O operation aborted.");
			}
			return httpStreamAsyncResult.Count;
		}

		private void onRead(IAsyncResult asyncResult)
		{
			ReadBufferState asyncState = (ReadBufferState)asyncResult.AsyncState;
			HttpStreamAsyncResult initialCount = asyncState.AsyncResult;
			try
			{
				int num = base.EndRead(asyncResult);
				this._decoder.Write(initialCount.Buffer, initialCount.Offset, num);
				num = this._decoder.Read(asyncState.Buffer, asyncState.Offset, asyncState.Count);
				ReadBufferState offset = asyncState;
				offset.Offset = offset.Offset + num;
				ReadBufferState count = asyncState;
				count.Count = count.Count - num;
				if ((asyncState.Count == 0 || !this._decoder.WantMore ? false : num != 0))
				{
					initialCount.Offset = 0;
					initialCount.Count = Math.Min(8192, this._decoder.ChunkLeft + 6);
					base.BeginRead(initialCount.Buffer, initialCount.Offset, initialCount.Count, new AsyncCallback(this.onRead), asyncState);
				}
				else
				{
					this._noMoreData = (this._decoder.WantMore ? false : num == 0);
					initialCount.Count = asyncState.InitialCount - asyncState.Count;
					initialCount.Complete();
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this._context.Connection.SendError(exception.Message, 400);
				initialCount.Complete(exception);
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			IAsyncResult asyncResult = this.BeginRead(buffer, offset, count, null, null);
			return this.EndRead(asyncResult);
		}
	}
}