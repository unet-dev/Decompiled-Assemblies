using System;

namespace WebSocketSharp.Net
{
	internal class ReadBufferState
	{
		private HttpStreamAsyncResult _asyncResult;

		private byte[] _buffer;

		private int _count;

		private int _initialCount;

		private int _offset;

		public HttpStreamAsyncResult AsyncResult
		{
			get
			{
				return this._asyncResult;
			}
			set
			{
				this._asyncResult = value;
			}
		}

		public byte[] Buffer
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

		public int Count
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

		public int InitialCount
		{
			get
			{
				return this._initialCount;
			}
			set
			{
				this._initialCount = value;
			}
		}

		public int Offset
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

		public ReadBufferState(byte[] buffer, int offset, int count, HttpStreamAsyncResult asyncResult)
		{
			this._buffer = buffer;
			this._offset = offset;
			this._count = count;
			this._initialCount = count;
			this._asyncResult = asyncResult;
		}
	}
}