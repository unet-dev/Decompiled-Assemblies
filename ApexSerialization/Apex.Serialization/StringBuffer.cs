using Apex.Utilities;
using System;

namespace Apex.Serialization
{
	internal sealed class StringBuffer
	{
		private char[] _buffer;

		private int _position;

		internal int position
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

		internal StringBuffer()
		{
			this._buffer = Empty<char>.array;
		}

		internal StringBuffer(int initalCapacity)
		{
			this._buffer = new char[initalCapacity];
		}

		internal void Append(char value)
		{
			if (this._position == (int)this._buffer.Length)
			{
				this.Resize(1);
			}
			char[] chrArray = this._buffer;
			int num = this._position;
			this._position = num + 1;
			chrArray[num] = value;
		}

		internal void Append(string value)
		{
			int length = value.Length;
			if (this._position + length >= (int)this._buffer.Length)
			{
				this.Resize(length);
			}
			value.CopyTo(0, this._buffer, this._position, length);
			this._position += length;
		}

		internal void Append(string value, int startIndex, int count)
		{
			if (this._position + count >= (int)this._buffer.Length)
			{
				this.Resize(count);
			}
			value.CopyTo(startIndex, this._buffer, this._position, count);
			this._position += count;
		}

		internal void Append(char[] value)
		{
			int length = (int)value.Length;
			if (this._position + length >= (int)this._buffer.Length)
			{
				this.Resize(length);
			}
			Array.Copy(value, 0, this._buffer, this._position, length);
			this._position += length;
		}

		internal void Append(char[] value, int startIndex, int count)
		{
			if (this._position + count >= (int)this._buffer.Length)
			{
				this.Resize(count);
			}
			Array.Copy(value, startIndex, this._buffer, this._position, count);
			this._position += count;
		}

		internal void Clear()
		{
			this._buffer = Empty<char>.array;
			this._position = 0;
		}

		internal void EnsureCapacity(int minimumSpace)
		{
			if (this._position + minimumSpace >= (int)this._buffer.Length)
			{
				this.Resize(minimumSpace);
			}
		}

		internal string Flush()
		{
			string str = new string(this._buffer, 0, this._position);
			this._position = 0;
			return str;
		}

		private void Resize(int appendLength)
		{
			char[] chrArray = new char[(this._position + appendLength) * 2];
			Array.Copy(this._buffer, chrArray, this._position);
			this._buffer = chrArray;
		}

		public override string ToString()
		{
			return new string(this._buffer, 0, this._position);
		}
	}
}