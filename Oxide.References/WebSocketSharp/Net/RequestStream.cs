using System;
using System.IO;
using System.Threading;

namespace WebSocketSharp.Net
{
	internal class RequestStream : Stream
	{
		private long _bodyLeft;

		private byte[] _buffer;

		private int _count;

		private bool _disposed;

		private int _offset;

		private Stream _stream;

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		internal RequestStream(Stream stream, byte[] buffer, int offset, int count) : this(stream, buffer, offset, count, (long)-1)
		{
		}

		internal RequestStream(Stream stream, byte[] buffer, int offset, int count, long contentLength)
		{
			this._stream = stream;
			this._buffer = buffer;
			this._offset = offset;
			this._count = count;
			this._bodyLeft = contentLength;
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			IAsyncResult asyncResult;
			if (this._disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			int num = this.fillFromBuffer(buffer, offset, count);
			if ((num > 0 ? false : num != -1))
			{
				if ((this._bodyLeft < (long)0 ? false : (long)count > this._bodyLeft))
				{
					count = (int)this._bodyLeft;
				}
				asyncResult = this._stream.BeginRead(buffer, offset, count, callback, state);
			}
			else
			{
				HttpStreamAsyncResult httpStreamAsyncResult = new HttpStreamAsyncResult(callback, state)
				{
					Buffer = buffer,
					Offset = offset,
					Count = count,
					SyncRead = (num > 0 ? num : 0)
				};
				httpStreamAsyncResult.Complete();
				asyncResult = httpStreamAsyncResult;
			}
			return asyncResult;
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException();
		}

		public override void Close()
		{
			this._disposed = true;
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			int syncRead;
			if (this._disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (!(asyncResult is HttpStreamAsyncResult))
			{
				int num = this._stream.EndRead(asyncResult);
				if ((num <= 0 ? false : this._bodyLeft > (long)0))
				{
					this._bodyLeft -= (long)num;
				}
				syncRead = num;
			}
			else
			{
				HttpStreamAsyncResult httpStreamAsyncResult = (HttpStreamAsyncResult)asyncResult;
				if (!httpStreamAsyncResult.IsCompleted)
				{
					httpStreamAsyncResult.AsyncWaitHandle.WaitOne();
				}
				syncRead = httpStreamAsyncResult.SyncRead;
			}
			return syncRead;
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			throw new NotSupportedException();
		}

		private int fillFromBuffer(byte[] buffer, int offset, int count)
		{
			int num;
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
			if (this._bodyLeft == (long)0)
			{
				num = -1;
			}
			else if ((this._count == 0 ? false : count != 0))
			{
				if (count > this._count)
				{
					count = this._count;
				}
				if ((this._bodyLeft <= (long)0 ? false : (long)count > this._bodyLeft))
				{
					count = (int)this._bodyLeft;
				}
				Buffer.BlockCopy(this._buffer, this._offset, buffer, offset, count);
				this._offset += count;
				this._count -= count;
				if (this._bodyLeft > (long)0)
				{
					this._bodyLeft -= (long)count;
				}
				num = count;
			}
			else
			{
				num = 0;
			}
			return num;
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num;
			if (this._disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			int num1 = this.fillFromBuffer(buffer, offset, count);
			if (num1 == -1)
			{
				num = 0;
			}
			else if (num1 <= 0)
			{
				num1 = this._stream.Read(buffer, offset, count);
				if ((num1 <= 0 ? false : this._bodyLeft > (long)0))
				{
					this._bodyLeft -= (long)num1;
				}
				num = num1;
			}
			else
			{
				num = num1;
			}
			return num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}
	}
}