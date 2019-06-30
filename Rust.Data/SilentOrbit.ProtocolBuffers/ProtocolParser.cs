using Facepunch.Extend;
using System;
using System.IO;
using System.Text;

namespace SilentOrbit.ProtocolBuffers
{
	public static class ProtocolParser
	{
		private static byte[] staticBuffer;

		static ProtocolParser()
		{
			ProtocolParser.staticBuffer = new byte[131072];
		}

		public static bool ReadBool(Stream stream)
		{
			int num = stream.ReadByte();
			if (num < 0)
			{
				throw new IOException("Stream ended too early");
			}
			if (num == 1)
			{
				return true;
			}
			if (num != 0)
			{
				throw new ProtocolBufferException("Invalid boolean value");
			}
			return false;
		}

		public static byte[] ReadBytes(Stream stream)
		{
			int num = 0;
			int num1 = (int)ProtocolParser.ReadUInt32(stream);
			byte[] numArray = new byte[num1];
			for (int i = 0; i < num1; i += num)
			{
				num = stream.Read(numArray, i, num1 - i);
				if (num == 0)
				{
					throw new ProtocolBufferException(string.Concat(new object[] { "Expected ", num1 - i, " got ", i }));
				}
			}
			return numArray;
		}

		[Obsolete("Only for reference")]
		public static double ReadDouble(BinaryReader reader)
		{
			return reader.ReadDouble();
		}

		[Obsolete("Only for reference")]
		public static uint ReadFixed32(BinaryReader reader)
		{
			return reader.ReadUInt32();
		}

		[Obsolete("Only for reference")]
		public static ulong ReadFixed64(BinaryReader reader)
		{
			return reader.ReadUInt64();
		}

		[Obsolete("Only for reference")]
		public static float ReadFloat(BinaryReader reader)
		{
			return reader.ReadSingle();
		}

		[Obsolete("Use (int)ReadUInt64(stream); //yes 64")]
		public static int ReadInt32(Stream stream)
		{
			return (int)ProtocolParser.ReadUInt64(stream);
		}

		[Obsolete("Use (long)ReadUInt64(stream); instead")]
		public static int ReadInt64(Stream stream)
		{
			return (int)ProtocolParser.ReadUInt64(stream);
		}

		public static Key ReadKey(Stream stream)
		{
			uint num = ProtocolParser.ReadUInt32(stream);
			return new Key(num >> 3, (Wire)(num & 7));
		}

		public static Key ReadKey(byte firstByte, Stream stream)
		{
			if (firstByte < 128)
			{
				return new Key((uint)(firstByte >> 3), (Wire)(firstByte & 7));
			}
			return new Key(ProtocolParser.ReadUInt32(stream) << 4 | firstByte >> 3 & 15, (Wire)(firstByte & 7));
		}

		[Obsolete("Only for reference")]
		public static int ReadSFixed32(BinaryReader reader)
		{
			return reader.ReadInt32();
		}

		[Obsolete("Only for reference")]
		public static long ReadSFixed64(BinaryReader reader)
		{
			return reader.ReadInt64();
		}

		public static float ReadSingle(Stream stream)
		{
			stream.Read(ProtocolParser.staticBuffer, 0, 4);
			return ProtocolParser.staticBuffer.ReadUnsafe<float>(0);
		}

		public static void ReadSkipVarInt(Stream stream)
		{
			int num;
			do
			{
				num = stream.ReadByte();
				if (num >= 0)
				{
					continue;
				}
				throw new IOException("Stream ended too early");
			}
			while ((num & 128) != 0);
		}

		public static string ReadString(Stream stream)
		{
			int num = (int)ProtocolParser.ReadUInt32(stream);
			if (num <= 0)
			{
				return string.Empty;
			}
			string empty = string.Empty;
			if (num < (int)ProtocolParser.staticBuffer.Length)
			{
				stream.Read(ProtocolParser.staticBuffer, 0, num);
				empty = Encoding.UTF8.GetString(ProtocolParser.staticBuffer, 0, num);
			}
			else
			{
				byte[] numArray = new byte[num];
				stream.Read(numArray, 0, num);
				empty = Encoding.UTF8.GetString(numArray, 0, num);
			}
			return empty;
		}

		public static uint ReadUInt32(Stream stream)
		{
			uint num = 0;
			for (int i = 0; i < 5; i++)
			{
				int num1 = stream.ReadByte();
				if (num1 < 0)
				{
					throw new IOException("Stream ended too early");
				}
				if (i == 4 && (num1 & 240) != 0)
				{
					throw new ProtocolBufferException("Got larger VarInt than 32bit unsigned");
				}
				if ((num1 & 128) == 0)
				{
					return num | num1 << (7 * i & 31);
				}
				num = num | (num1 & 127) << (7 * i & 31);
			}
			throw new ProtocolBufferException("Got larger VarInt than 32bit unsigned");
		}

		public static ulong ReadUInt64(Stream stream)
		{
			ulong num = (ulong)0;
			for (int i = 0; i < 10; i++)
			{
				int num1 = stream.ReadByte();
				if (num1 < 0)
				{
					throw new IOException("Stream ended too early");
				}
				if (i == 9 && (num1 & 254) != 0)
				{
					throw new ProtocolBufferException("Got larger VarInt than 64 bit unsigned");
				}
				if ((num1 & 128) == 0)
				{
					return num | (long)num1 << (7 * i & 63);
				}
				num = num | (long)(num1 & 127) << (7 * i & 63);
			}
			throw new ProtocolBufferException("Got larger VarInt than 64 bit unsigned");
		}

		public static byte[] ReadValueBytes(Stream stream, Key key)
		{
			throw new NotSupportedException("ReadValueBytes");
		}

		public static byte[] ReadVarIntBytes(Stream stream)
		{
			byte[] numArray = new byte[10];
			int num = 0;
			do
			{
				int num1 = stream.ReadByte();
				if (num1 < 0)
				{
					throw new IOException("Stream ended too early");
				}
				numArray[num] = (byte)num1;
				num++;
				if ((num1 & 128) != 0)
				{
					continue;
				}
				byte[] numArray1 = new byte[num];
				Array.Copy(numArray, numArray1, (int)numArray1.Length);
				return numArray1;
			}
			while (num < (int)numArray.Length);
			throw new ProtocolBufferException("VarInt too long, more than 10 bytes");
		}

		public static int ReadZInt32(Stream stream)
		{
			uint num = ProtocolParser.ReadUInt32(stream);
			return (int)(num >> 1 ^ num << 31 >> 31);
		}

		public static long ReadZInt64(Stream stream)
		{
			ulong num = ProtocolParser.ReadUInt64(stream);
			return (long)(num >> 1 ^ num << 63 >> 63);
		}

		public static void SkipBytes(Stream stream)
		{
			int num = (int)ProtocolParser.ReadUInt32(stream);
			if (!stream.CanSeek)
			{
				ProtocolParser.ReadBytes(stream);
				return;
			}
			stream.Seek((long)num, SeekOrigin.Current);
		}

		public static void SkipKey(Stream stream, Key key)
		{
			switch (key.WireType)
			{
				case Wire.Varint:
				{
					ProtocolParser.ReadSkipVarInt(stream);
					return;
				}
				case Wire.Fixed64:
				{
					stream.Seek((long)8, SeekOrigin.Current);
					return;
				}
				case Wire.LengthDelimited:
				{
					stream.Seek((long)ProtocolParser.ReadUInt32(stream), SeekOrigin.Current);
					return;
				}
				case Wire.Fixed64 | Wire.LengthDelimited:
				case 4:
				{
					throw new NotImplementedException(string.Concat("Unknown wire type: ", key.WireType));
				}
				case Wire.Fixed32:
				{
					stream.Seek((long)4, SeekOrigin.Current);
					return;
				}
				default:
				{
					throw new NotImplementedException(string.Concat("Unknown wire type: ", key.WireType));
				}
			}
		}

		public static void WriteBool(Stream stream, bool val)
		{
			stream.WriteByte((byte)((val ? 1 : 0)));
		}

		public static void WriteBytes(Stream stream, byte[] val)
		{
			ProtocolParser.WriteUInt32(stream, (uint)val.Length);
			stream.Write(val, 0, (int)val.Length);
		}

		[Obsolete("Only for reference")]
		public static void WriteDouble(BinaryWriter writer, double val)
		{
			writer.Write(val);
		}

		[Obsolete("Only for reference")]
		public static void WriteFixed32(BinaryWriter writer, uint val)
		{
			writer.Write(val);
		}

		[Obsolete("Only for reference")]
		public static void WriteFixed64(BinaryWriter writer, ulong val)
		{
			writer.Write(val);
		}

		[Obsolete("Only for reference")]
		public static void WriteFloat(BinaryWriter writer, float val)
		{
			writer.Write(val);
		}

		[Obsolete("Use WriteUInt64(stream, (ulong)val); //yes 64, negative numbers are encoded that way")]
		public static void WriteInt32(Stream stream, int val)
		{
			ProtocolParser.WriteUInt64(stream, (ulong)val);
		}

		[Obsolete("Use WriteUInt64 (stream, (ulong)val); instead")]
		public static void WriteInt64(Stream stream, int val)
		{
			ProtocolParser.WriteUInt64(stream, (ulong)val);
		}

		public static void WriteKey(Stream stream, Key key)
		{
			uint field = key.Field << 3 | (UInt32)key.WireType;
			ProtocolParser.WriteUInt32(stream, field);
		}

		[Obsolete("Only for reference")]
		public static void WriteSFixed32(BinaryWriter writer, int val)
		{
			writer.Write(val);
		}

		[Obsolete("Only for reference")]
		public static void WriteSFixed64(BinaryWriter writer, long val)
		{
			writer.Write(val);
		}

		public static void WriteSingle(Stream stream, float f)
		{
			ProtocolParser.staticBuffer.WriteUnsafe<float>(f, 0);
			stream.Write(ProtocolParser.staticBuffer, 0, 4);
		}

		public static void WriteString(Stream stream, string val)
		{
			int bytes = Encoding.UTF8.GetBytes(val, 0, val.Length, ProtocolParser.staticBuffer, 0);
			ProtocolParser.WriteUInt32(stream, (uint)bytes);
			stream.Write(ProtocolParser.staticBuffer, 0, bytes);
		}

		public static void WriteUInt32(Stream stream, uint val)
		{
			byte num;
			while (true)
			{
				num = (byte)(val & 127);
				val >>= 7;
				if (val == 0)
				{
					break;
				}
				num = (byte)(num | 128);
				stream.WriteByte(num);
			}
			stream.WriteByte(num);
		}

		public static void WriteUInt64(Stream stream, ulong val)
		{
			byte num;
			while (true)
			{
				num = (byte)(val & (long)127);
				val >>= 7;
				if (val == 0)
				{
					break;
				}
				num = (byte)(num | 128);
				stream.WriteByte(num);
			}
			stream.WriteByte(num);
		}

		public static void WriteZInt32(Stream stream, int val)
		{
			ProtocolParser.WriteUInt32(stream, (uint)(val << 1 ^ val >> 31));
		}

		public static void WriteZInt64(Stream stream, long val)
		{
			ProtocolParser.WriteUInt64(stream, (ulong)(val << 1 ^ val >> 63));
		}
	}
}