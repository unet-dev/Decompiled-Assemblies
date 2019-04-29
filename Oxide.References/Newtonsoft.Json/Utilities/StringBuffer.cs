using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal struct StringBuffer
	{
		private char[] _buffer;

		private int _position;

		public char[] InternalBuffer
		{
			get
			{
				return this._buffer;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this._buffer == null;
			}
		}

		public int Position
		{
			get
			{
				return this._position;
			}
			set
			{
				this._position = value;
			}
		}

		public StringBuffer(IArrayPool<char> bufferPool, int initalSize) : this(BufferUtils.RentBuffer(bufferPool, initalSize))
		{
		}

		private StringBuffer(char[] buffer)
		{
			this._buffer = buffer;
			this._position = 0;
		}

		public void Append(IArrayPool<char> bufferPool, char value)
		{
			if (this._position == (int)this._buffer.Length)
			{
				this.EnsureSize(bufferPool, 1);
			}
			char[] chrArray = this._buffer;
			int num = this._position;
			this._position = num + 1;
			chrArray[num] = value;
		}

		public void Append(IArrayPool<char> bufferPool, char[] buffer, int startIndex, int count)
		{
			if (this._position + count >= (int)this._buffer.Length)
			{
				this.EnsureSize(bufferPool, count);
			}
			Array.Copy(buffer, startIndex, this._buffer, this._position, count);
			this._position += count;
		}

		public void Clear(IArrayPool<char> bufferPool)
		{
			if (this._buffer != null)
			{
				BufferUtils.ReturnBuffer(bufferPool, this._buffer);
				this._buffer = null;
			}
			this._position = 0;
		}

		private void EnsureSize(IArrayPool<char> bufferPool, int appendLength)
		{
			char[] chrArray = BufferUtils.RentBuffer(bufferPool, (this._position + appendLength) * 2);
			if (this._buffer != null)
			{
				Array.Copy(this._buffer, chrArray, this._position);
				BufferUtils.ReturnBuffer(bufferPool, this._buffer);
			}
			this._buffer = chrArray;
		}

		public override string ToString()
		{
			return this.ToString(0, this._position);
		}

		public string ToString(int start, int length)
		{
			return new string(this._buffer, start, length);
		}
	}
}