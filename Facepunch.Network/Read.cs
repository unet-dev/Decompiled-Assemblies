using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Network
{
	public abstract class Read : Stream
	{
		private static MemoryStream buffer;

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

		public int length
		{
			get
			{
				return (int)this.Length;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int position
		{
			get
			{
				return (int)this.Position;
			}
		}

		public override long Position
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public int unread
		{
			get
			{
				return (int)(this.Length - this.Position);
			}
		}

		static Read()
		{
			Read.buffer = new MemoryStream();
		}

		protected Read()
		{
		}

		public abstract bool Bit();

		public byte[] BytesWithSize()
		{
			uint num = this.UInt32();
			if (num == 0)
			{
				return null;
			}
			if (num > 10485760)
			{
				return null;
			}
			byte[] numArray = new byte[num];
			if ((long)this.Read(numArray, 0, (Int32)num) != (ulong)num)
			{
				return null;
			}
			return numArray;
		}

		public abstract double Double();

		public uint EntityID()
		{
			return this.UInt32();
		}

		public abstract float Float();

		public override void Flush()
		{
			throw new NotImplementedException();
		}

		public uint GroupID()
		{
			return this.UInt32();
		}

		public abstract short Int16();

		public abstract int Int32();

		public abstract long Int64();

		public abstract sbyte Int8();

		public MemoryStream MemoryStreamWithSize()
		{
			uint num = this.UInt32();
			if (num == 0)
			{
				return null;
			}
			if (num > 10485760)
			{
				return null;
			}
			if ((long)Read.buffer.Capacity < (ulong)num)
			{
				Read.buffer.Capacity = (int)num;
			}
			Read.buffer.Position = (long)0;
			Read.buffer.SetLength((Int64)num);
			if ((long)this.Read(Read.buffer.GetBuffer(), 0, (Int32)num) != (ulong)num)
			{
				return null;
			}
			return Read.buffer;
		}

		public abstract byte PacketID();

		public Quaternion Quaternion()
		{
			return new Quaternion(this.Float(), this.Float(), this.Float(), this.Float());
		}

		public Ray Ray()
		{
			return new Ray(this.Vector3(), this.Vector3());
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public abstract bool Start();

		public abstract bool Start(Connection connection);

		public string String()
		{
			MemoryStream memoryStream = this.MemoryStreamWithSize();
			if (memoryStream == null)
			{
				return string.Empty;
			}
			return Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
		}

		public abstract ushort UInt16();

		public abstract uint UInt32();

		public abstract ulong UInt64();

		public abstract byte UInt8();

		public Vector3 Vector3()
		{
			return new Vector3(this.Float(), this.Float(), this.Float());
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public override void WriteByte(byte value)
		{
			throw new NotImplementedException();
		}
	}
}