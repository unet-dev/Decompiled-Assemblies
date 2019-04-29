using Newtonsoft.Json.Shims;
using System;
using System.IO;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal class Base64Encoder
	{
		private const int Base64LineSize = 76;

		private const int LineSizeInBytes = 57;

		private readonly char[] _charsLine = new char[76];

		private readonly TextWriter _writer;

		private byte[] _leftOverBytes;

		private int _leftOverBytesCount;

		public Base64Encoder(TextWriter writer)
		{
			ValidationUtils.ArgumentNotNull(writer, "writer");
			this._writer = writer;
		}

		public void Encode(byte[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (count > (int)buffer.Length - index)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (this._leftOverBytesCount > 0)
			{
				int num = this._leftOverBytesCount;
				while (num < 3 && count > 0)
				{
					int num1 = num;
					num = num1 + 1;
					int num2 = index;
					index = num2 + 1;
					this._leftOverBytes[num1] = buffer[num2];
					count--;
				}
				if (count == 0 && num < 3)
				{
					this._leftOverBytesCount = num;
					return;
				}
				int base64CharArray = Convert.ToBase64CharArray(this._leftOverBytes, 0, 3, this._charsLine, 0);
				this.WriteChars(this._charsLine, 0, base64CharArray);
			}
			this._leftOverBytesCount = count % 3;
			if (this._leftOverBytesCount > 0)
			{
				count -= this._leftOverBytesCount;
				if (this._leftOverBytes == null)
				{
					this._leftOverBytes = new byte[3];
				}
				for (int i = 0; i < this._leftOverBytesCount; i++)
				{
					this._leftOverBytes[i] = buffer[index + count + i];
				}
			}
			int num3 = index + count;
			int num4 = 57;
			while (index < num3)
			{
				if (index + num4 > num3)
				{
					num4 = num3 - index;
				}
				int base64CharArray1 = Convert.ToBase64CharArray(buffer, index, num4, this._charsLine, 0);
				this.WriteChars(this._charsLine, 0, base64CharArray1);
				index += num4;
			}
		}

		public void Flush()
		{
			if (this._leftOverBytesCount > 0)
			{
				int base64CharArray = Convert.ToBase64CharArray(this._leftOverBytes, 0, this._leftOverBytesCount, this._charsLine, 0);
				this.WriteChars(this._charsLine, 0, base64CharArray);
				this._leftOverBytesCount = 0;
			}
		}

		private void WriteChars(char[] chars, int index, int count)
		{
			this._writer.Write(chars, index, count);
		}
	}
}