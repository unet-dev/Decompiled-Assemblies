using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class Sign : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public uint imageid;

		public bool ShouldPool = true;

		private bool _disposed;

		public Sign()
		{
		}

		public Sign Copy()
		{
			Sign sign = Pool.Get<Sign>();
			this.CopyTo(sign);
			return sign;
		}

		public void CopyTo(Sign instance)
		{
			instance.imageid = this.imageid;
		}

		public static Sign Deserialize(Stream stream)
		{
			Sign sign = Pool.Get<Sign>();
			Sign.Deserialize(stream, sign, false);
			return sign;
		}

		public static Sign Deserialize(byte[] buffer)
		{
			Sign sign = Pool.Get<Sign>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Sign.Deserialize(memoryStream, sign, false);
			}
			return sign;
		}

		public static Sign Deserialize(byte[] buffer, Sign instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Sign.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static Sign Deserialize(Stream stream, Sign instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num != 24)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else
				{
					instance.imageid = ProtocolParser.ReadUInt32(stream);
				}
			}
			return instance;
		}

		public static Sign DeserializeLength(Stream stream, int length)
		{
			Sign sign = Pool.Get<Sign>();
			Sign.DeserializeLength(stream, length, sign, false);
			return sign;
		}

		public static Sign DeserializeLength(Stream stream, int length, Sign instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num != 24)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else
				{
					instance.imageid = ProtocolParser.ReadUInt32(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static Sign DeserializeLengthDelimited(Stream stream)
		{
			Sign sign = Pool.Get<Sign>();
			Sign.DeserializeLengthDelimited(stream, sign, false);
			return sign;
		}

		public static Sign DeserializeLengthDelimited(Stream stream, Sign instance, bool isDelta)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num != 24)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else
				{
					instance.imageid = ProtocolParser.ReadUInt32(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public virtual void Dispose()
		{
			if (this._disposed)
			{
				return;
			}
			this.ResetToPool();
			this._disposed = true;
		}

		public virtual void EnterPool()
		{
			this._disposed = true;
		}

		public void FromProto(Stream stream, bool isDelta = false)
		{
			Sign.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			Sign.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(Sign instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.imageid = 0;
			Pool.Free<Sign>(ref instance);
		}

		public void ResetToPool()
		{
			Sign.ResetToPool(this);
		}

		public static void Serialize(Stream stream, Sign instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(24);
			ProtocolParser.WriteUInt32(stream, instance.imageid);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, Sign instance, Sign previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.imageid != previous.imageid)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.imageid);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, Sign instance)
		{
			byte[] bytes = Sign.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(Sign instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Sign.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			Sign.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return Sign.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			Sign.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, Sign previous)
		{
			if (previous == null)
			{
				Sign.Serialize(stream, this);
				return;
			}
			Sign.SerializeDelta(stream, this, previous);
		}
	}
}