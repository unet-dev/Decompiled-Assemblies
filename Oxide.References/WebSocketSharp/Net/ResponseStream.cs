using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace WebSocketSharp.Net
{
	internal class ResponseStream : Stream
	{
		private MemoryStream _body;

		private readonly static byte[] _crlf;

		private bool _disposed;

		private HttpListenerResponse _response;

		private bool _sendChunked;

		private Stream _stream;

		private Action<byte[], int, int> _write;

		private Action<byte[], int, int> _writeBody;

		private Action<byte[], int, int> _writeChunked;

		public override bool CanRead
		{
			get
			{
				return false;
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
				return !this._disposed;
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

		static ResponseStream()
		{
			ResponseStream._crlf = new byte[] { 13, 10 };
		}

		internal ResponseStream(Stream stream, HttpListenerResponse response, bool ignoreWriteExceptions)
		{
			this._stream = stream;
			this._response = response;
			if (!ignoreWriteExceptions)
			{
				Stream stream1 = stream;
				this._write = new Action<byte[], int, int>(stream1.Write);
				this._writeChunked = new Action<byte[], int, int>(this.writeChunked);
			}
			else
			{
				this._write = new Action<byte[], int, int>(this.writeWithoutThrowingException);
				this._writeChunked = new Action<byte[], int, int>(this.writeChunkedWithoutThrowingException);
			}
			this._body = new MemoryStream();
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException();
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			IAsyncResult asyncResult = this._body.BeginWrite(buffer, offset, count, callback, state);
			return asyncResult;
		}

		internal void Close(bool force)
		{
			if (!this._disposed)
			{
				this._disposed = true;
				if ((force ? true : !this.flush(true)))
				{
					if (this._sendChunked)
					{
						byte[] chunkSizeBytes = ResponseStream.getChunkSizeBytes(0, true);
						this._write(chunkSizeBytes, 0, (int)chunkSizeBytes.Length);
					}
					this._body.Dispose();
					this._body = null;
					this._response.Abort();
				}
				else
				{
					this._response.Close();
				}
				this._response = null;
				this._stream = null;
			}
		}

		public override void Close()
		{
			this.Close(false);
		}

		protected override void Dispose(bool disposing)
		{
			this.Close(!disposing);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			throw new NotSupportedException();
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			this._body.EndWrite(asyncResult);
		}

		private bool flush(bool closing)
		{
			bool flag;
			if (!this._response.HeadersSent)
			{
				if (!this.flushHeaders(closing))
				{
					if (closing)
					{
						this._response.CloseConnection = true;
					}
					flag = false;
					return flag;
				}
				this._sendChunked = this._response.SendChunked;
				this._writeBody = (this._sendChunked ? this._writeChunked : this._write);
			}
			this.flushBody(closing);
			if ((!closing ? false : this._sendChunked))
			{
				byte[] chunkSizeBytes = ResponseStream.getChunkSizeBytes(0, true);
				this._write(chunkSizeBytes, 0, (int)chunkSizeBytes.Length);
			}
			flag = true;
			return flag;
		}

		public override void Flush()
		{
			bool flag;
			if (this._disposed)
			{
				flag = false;
			}
			else
			{
				flag = (this._sendChunked ? true : this._response.SendChunked);
			}
			if (flag)
			{
				this.flush(false);
			}
		}

		private void flushBody(bool closing)
		{
			MemoryStream memoryStream;
			using (this._body)
			{
				long length = this._body.Length;
				if (length > (long)2147483647)
				{
					this._body.Position = (long)0;
					int num = 1024;
					byte[] numArray = new byte[num];
					int num1 = 0;
					while (true)
					{
						int num2 = this._body.Read(numArray, 0, num);
						num1 = num2;
						if (num2 <= 0)
						{
							break;
						}
						this._writeBody(numArray, 0, num1);
					}
				}
				else if (length > (long)0)
				{
					this._writeBody(this._body.GetBuffer(), 0, (int)length);
				}
			}
			if (!closing)
			{
				memoryStream = new MemoryStream();
			}
			else
			{
				memoryStream = null;
			}
			this._body = memoryStream;
		}

		private bool flushHeaders(bool closing)
		{
			bool flag;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				WebHeaderCollection webHeaderCollection = this._response.WriteHeadersTo(memoryStream);
				long position = memoryStream.Position;
				long length = memoryStream.Length - position;
				if (length > (long)32768)
				{
					flag = false;
					return flag;
				}
				else if ((this._response.SendChunked ? true : this._response.ContentLength64 == this._body.Length))
				{
					this._write(memoryStream.GetBuffer(), (int)position, (int)length);
					this._response.CloseConnection = webHeaderCollection["Connection"] == "close";
					this._response.HeadersSent = true;
				}
				else
				{
					flag = false;
					return flag;
				}
			}
			flag = true;
			return flag;
		}

		private static byte[] getChunkSizeBytes(int size, bool final)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(string.Format("{0:x}\r\n{1}", size, (final ? "\r\n" : "")));
			return bytes;
		}

		internal void InternalWrite(byte[] buffer, int offset, int count)
		{
			this._write(buffer, offset, count);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
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
			if (this._disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			this._body.Write(buffer, offset, count);
		}

		private void writeChunked(byte[] buffer, int offset, int count)
		{
			byte[] chunkSizeBytes = ResponseStream.getChunkSizeBytes(count, false);
			this._stream.Write(chunkSizeBytes, 0, (int)chunkSizeBytes.Length);
			this._stream.Write(buffer, offset, count);
			this._stream.Write(ResponseStream._crlf, 0, 2);
		}

		private void writeChunkedWithoutThrowingException(byte[] buffer, int offset, int count)
		{
			try
			{
				this.writeChunked(buffer, offset, count);
			}
			catch
			{
			}
		}

		private void writeWithoutThrowingException(byte[] buffer, int offset, int count)
		{
			try
			{
				this._stream.Write(buffer, offset, count);
			}
			catch
			{
			}
		}
	}
}