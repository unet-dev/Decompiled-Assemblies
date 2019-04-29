using Mono;
using System;

namespace Mono.Cecil.PE
{
	internal class ByteBuffer
	{
		internal byte[] buffer;

		internal int length;

		internal int position;

		public ByteBuffer()
		{
			this.buffer = Empty<byte>.Array;
		}

		public ByteBuffer(int length)
		{
			this.buffer = new byte[length];
		}

		public ByteBuffer(byte[] buffer)
		{
			Object array = buffer;
			if (array == null)
			{
				array = Empty<byte>.Array;
			}
			this.buffer = (byte[])array;
			this.length = (int)this.buffer.Length;
		}

		public void Advance(int length)
		{
			this.position += length;
		}

		private void Grow(int desired)
		{
			byte[] numArray = this.buffer;
			int length = (int)numArray.Length;
			byte[] numArray1 = new byte[System.Math.Max(length + desired, length * 2)];
			Buffer.BlockCopy((Array)numArray, 0, numArray1, 0, length);
			this.buffer = numArray1;
		}

		public byte ReadByte()
		{
			byte[] numArray = this.buffer;
			int num = this.position;
			this.position = num + 1;
			return numArray[num];
		}

		public byte[] ReadBytes(int length)
		{
			byte[] numArray = new byte[length];
			Buffer.BlockCopy(this.buffer, this.position, numArray, 0, length);
			this.position += length;
			return numArray;
		}

		public int ReadCompressedInt32()
		{
			int num = (int)(this.ReadCompressedUInt32() >> 1);
			if ((num & 1) == 0)
			{
				return num;
			}
			if (num < 64)
			{
				return num - 64;
			}
			if (num < 8192)
			{
				return num - 8192;
			}
			if (num < 268435456)
			{
				return num - 268435456;
			}
			return num - 536870912;
		}

		public uint ReadCompressedUInt32()
		{
			byte num = this.ReadByte();
			if ((num & 128) == 0)
			{
				return num;
			}
			if ((num & 64) == 0)
			{
				return (uint)((num & -129) << 8 | this.ReadByte());
			}
			return (uint)((num & -193) << 24 | this.ReadByte() << 16 | this.ReadByte() << 8 | this.ReadByte());
		}

		public double ReadDouble()
		{
			if (!BitConverter.IsLittleEndian)
			{
				byte[] numArray = this.ReadBytes(8);
				Array.Reverse((Array)numArray);
				return BitConverter.ToDouble(numArray, 0);
			}
			double num = BitConverter.ToDouble(this.buffer, this.position);
			this.position += 8;
			return num;
		}

		public short ReadInt16()
		{
			return (short)this.ReadUInt16();
		}

		public int ReadInt32()
		{
			return (int)this.ReadUInt32();
		}

		public long ReadInt64()
		{
			return (long)this.ReadUInt64();
		}

		public sbyte ReadSByte()
		{
			return (sbyte)this.ReadByte();
		}

		public float ReadSingle()
		{
			if (!BitConverter.IsLittleEndian)
			{
				byte[] numArray = this.ReadBytes(4);
				Array.Reverse((Array)numArray);
				return BitConverter.ToSingle(numArray, 0);
			}
			float single = BitConverter.ToSingle(this.buffer, this.position);
			this.position += 4;
			return single;
		}

		public ushort ReadUInt16()
		{
			ushort num = (ushort)(this.buffer[this.position] | this.buffer[this.position + 1] << 8);
			this.position += 2;
			return num;
		}

		public uint ReadUInt32()
		{
			Int32 num = this.buffer[this.position] | this.buffer[this.position + 1] << 8 | this.buffer[this.position + 2] << 16 | this.buffer[this.position + 3] << 24;
			this.position += 4;
			return (uint)num;
		}

		public ulong ReadUInt64()
		{
			uint num = this.ReadUInt32();
			return (ulong)this.ReadUInt32() << 32 | (ulong)num;
		}

		public void Reset(byte[] buffer)
		{
			Object array = buffer;
			if (array == null)
			{
				array = Empty<byte>.Array;
			}
			this.buffer = (byte[])array;
			this.length = (int)this.buffer.Length;
		}

		public void WriteByte(byte value)
		{
			if (this.position == (int)this.buffer.Length)
			{
				this.Grow(1);
			}
			byte[] numArray = this.buffer;
			int num = this.position;
			this.position = num + 1;
			numArray[num] = value;
			if (this.position > this.length)
			{
				this.length = this.position;
			}
		}

		public void WriteBytes(byte[] bytes)
		{
			int length = (int)bytes.Length;
			if (this.position + length > (int)this.buffer.Length)
			{
				this.Grow(length);
			}
			Buffer.BlockCopy(bytes, 0, this.buffer, this.position, length);
			this.position += length;
			if (this.position > this.length)
			{
				this.length = this.position;
			}
		}

		public void WriteBytes(int length)
		{
			if (this.position + length > (int)this.buffer.Length)
			{
				this.Grow(length);
			}
			this.position += length;
			if (this.position > this.length)
			{
				this.length = this.position;
			}
		}

		public void WriteBytes(ByteBuffer buffer)
		{
			if (this.position + buffer.length > (int)this.buffer.Length)
			{
				this.Grow(buffer.length);
			}
			Buffer.BlockCopy(buffer.buffer, 0, this.buffer, this.position, buffer.length);
			this.position += buffer.length;
			if (this.position > this.length)
			{
				this.length = this.position;
			}
		}

		public void WriteCompressedInt32(int value)
		{
			if (value >= 0)
			{
				this.WriteCompressedUInt32((uint)(value << 1));
				return;
			}
			if (value > -64)
			{
				value = 64 + value;
			}
			else if (value >= -8192)
			{
				value = 8192 + value;
			}
			else if (value >= -536870912)
			{
				value = 536870912 + value;
			}
			this.WriteCompressedUInt32((uint)(value << 1 | 1));
		}

		public void WriteCompressedUInt32(uint value)
		{
			if (value < 128)
			{
				this.WriteByte((byte)value);
				return;
			}
			if (value < 16384)
			{
				this.WriteByte((byte)(128 | value >> 8));
				this.WriteByte((byte)(value & 255));
				return;
			}
			this.WriteByte((byte)(value >> 24 | 192));
			this.WriteByte((byte)(value >> 16 & 255));
			this.WriteByte((byte)(value >> 8 & 255));
			this.WriteByte((byte)(value & 255));
		}

		public void WriteDouble(double value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
			}
			this.WriteBytes(bytes);
		}

		public void WriteInt16(short value)
		{
			this.WriteUInt16((ushort)value);
		}

		public void WriteInt32(int value)
		{
			this.WriteUInt32((uint)value);
		}

		public void WriteInt64(long value)
		{
			this.WriteUInt64((ulong)value);
		}

		public void WriteSByte(sbyte value)
		{
			this.WriteByte((byte)value);
		}

		public void WriteSingle(float value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
			}
			this.WriteBytes(bytes);
		}

		public void WriteUInt16(ushort value)
		{
			if (this.position + 2 > (int)this.buffer.Length)
			{
				this.Grow(2);
			}
			byte[] numArray = this.buffer;
			int num = this.position;
			this.position = num + 1;
			numArray[num] = (byte)value;
			byte[] numArray1 = this.buffer;
			num = this.position;
			this.position = num + 1;
			numArray1[num] = (byte)(value >> 8);
			if (this.position > this.length)
			{
				this.length = this.position;
			}
		}

		public void WriteUInt32(uint value)
		{
			if (this.position + 4 > (int)this.buffer.Length)
			{
				this.Grow(4);
			}
			byte[] numArray = this.buffer;
			int num = this.position;
			this.position = num + 1;
			numArray[num] = (byte)value;
			byte[] numArray1 = this.buffer;
			num = this.position;
			this.position = num + 1;
			numArray1[num] = (byte)(value >> 8);
			byte[] numArray2 = this.buffer;
			num = this.position;
			this.position = num + 1;
			numArray2[num] = (byte)(value >> 16);
			byte[] numArray3 = this.buffer;
			num = this.position;
			this.position = num + 1;
			numArray3[num] = (byte)(value >> 24);
			if (this.position > this.length)
			{
				this.length = this.position;
			}
		}

		public void WriteUInt64(ulong value)
		{
			if (this.position + 8 > (int)this.buffer.Length)
			{
				this.Grow(8);
			}
			byte[] numArray = this.buffer;
			int num = this.position;
			this.position = num + 1;
			numArray[num] = (byte)value;
			byte[] numArray1 = this.buffer;
			num = this.position;
			this.position = num + 1;
			numArray1[num] = (byte)(value >> 8);
			byte[] numArray2 = this.buffer;
			num = this.position;
			this.position = num + 1;
			numArray2[num] = (byte)(value >> 16);
			byte[] numArray3 = this.buffer;
			num = this.position;
			this.position = num + 1;
			numArray3[num] = (byte)(value >> 24);
			byte[] numArray4 = this.buffer;
			num = this.position;
			this.position = num + 1;
			numArray4[num] = (byte)(value >> 32);
			byte[] numArray5 = this.buffer;
			num = this.position;
			this.position = num + 1;
			numArray5[num] = (byte)(value >> 40);
			byte[] numArray6 = this.buffer;
			num = this.position;
			this.position = num + 1;
			numArray6[num] = (byte)(value >> 48);
			byte[] numArray7 = this.buffer;
			num = this.position;
			this.position = num + 1;
			numArray7[num] = (byte)(value >> 56);
			if (this.position > this.length)
			{
				this.length = this.position;
			}
		}
	}
}