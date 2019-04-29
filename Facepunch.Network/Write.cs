using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Network
{
	public abstract class Write : Stream
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

		public override long Length
		{
			get
			{
				throw new NotImplementedException();
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

		static Write()
		{
			Write.buffer = new MemoryStream();
		}

		protected Write()
		{
		}

		public abstract void Bool(bool val);

		public abstract void Bytes(byte[] val);

		public void BytesWithSize(MemoryStream val)
		{
			if (val == null || val.Length == 0)
			{
				this.UInt32(0);
				return;
			}
			this.BytesWithSize(val.GetBuffer(), (int)val.Length);
		}

		public void BytesWithSize(byte[] b)
		{
			this.BytesWithSize(b, (int)b.Length);
		}

		public void BytesWithSize(byte[] b, int length)
		{
			if (b == null || b.Length == 0 || length == 0)
			{
				this.UInt32(0);
				return;
			}
			if (length > 10485760)
			{
				this.UInt32(0);
				Debug.LogError(string.Concat("BytesWithSize: Too big ", length));
				return;
			}
			this.UInt32((uint)length);
			this.Write(b, 0, length);
		}

		public abstract void Double(double val);

		public void EntityID(uint id)
		{
			this.UInt32(id);
		}

		public abstract void Float(float val);

		public override void Flush()
		{
			throw new NotImplementedException();
		}

		public void GroupID(uint id)
		{
			this.UInt32(id);
		}

		public abstract void Int16(short val);

		public abstract void Int32(int val);

		public abstract void Int64(long val);

		public abstract void Int8(sbyte val);

		public abstract void PacketID(Message.Type val);

		public void Quaternion(Quaternion obj)
		{
			this.Float(obj.x);
			this.Float(obj.y);
			this.Float(obj.z);
			this.Float(obj.w);
		}

		public void Ray(Ray obj)
		{
			this.Vector3(obj.origin);
			this.Vector3(obj.direction);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public override int ReadByte()
		{
			throw new NotImplementedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public abstract void Send(SendInfo info);

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public abstract bool Start();

		public void String(string val)
		{
			if (string.IsNullOrEmpty(val))
			{
				this.BytesWithSize(null);
				return;
			}
			if (Write.buffer.Capacity < val.Length * 8)
			{
				Write.buffer.Capacity = val.Length * 8;
			}
			Write.buffer.Position = (long)0;
			Write.buffer.SetLength((long)Write.buffer.Capacity);
			int bytes = Encoding.UTF8.GetBytes(val, 0, val.Length, Write.buffer.GetBuffer(), 0);
			Write.buffer.SetLength((long)bytes);
			this.BytesWithSize(Write.buffer);
		}

		public abstract void UInt16(ushort val);

		public abstract void UInt32(uint val);

		public abstract void UInt64(ulong val);

		public abstract void UInt8(byte val);

		public void Vector3(Vector3 obj)
		{
			this.Float(obj.x);
			this.Float(obj.y);
			this.Float(obj.z);
		}
	}
}