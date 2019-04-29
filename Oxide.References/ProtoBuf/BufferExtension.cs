using System;
using System.IO;

namespace ProtoBuf
{
	public sealed class BufferExtension : IExtension
	{
		private byte[] buffer;

		public BufferExtension()
		{
		}

		Stream ProtoBuf.IExtension.BeginAppend()
		{
			return new MemoryStream();
		}

		Stream ProtoBuf.IExtension.BeginQuery()
		{
			if (this.buffer == null)
			{
				return Stream.Null;
			}
			return new MemoryStream(this.buffer);
		}

		void ProtoBuf.IExtension.EndAppend(Stream stream, bool commit)
		{
			using (stream)
			{
				if (commit)
				{
					int length = (int)stream.Length;
					int num = length;
					if (length > 0)
					{
						MemoryStream memoryStream = (MemoryStream)stream;
						if (this.buffer != null)
						{
							int length1 = (int)this.buffer.Length;
							byte[] numArray = new byte[length1 + num];
							Helpers.BlockCopy(this.buffer, 0, numArray, 0, length1);
							Helpers.BlockCopy(memoryStream.GetBuffer(), 0, numArray, length1, num);
							this.buffer = numArray;
						}
						else
						{
							this.buffer = memoryStream.ToArray();
						}
					}
				}
			}
		}

		void ProtoBuf.IExtension.EndQuery(Stream stream)
		{
			using (stream)
			{
			}
		}

		int ProtoBuf.IExtension.GetLength()
		{
			if (this.buffer == null)
			{
				return 0;
			}
			return (int)this.buffer.Length;
		}
	}
}