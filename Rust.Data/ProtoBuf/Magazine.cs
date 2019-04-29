using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class Magazine : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public int capacity;

		[NonSerialized]
		public int contents;

		[NonSerialized]
		public int ammoType;

		public bool ShouldPool = true;

		private bool _disposed;

		public Magazine()
		{
		}

		public Magazine Copy()
		{
			Magazine magazine = Pool.Get<Magazine>();
			this.CopyTo(magazine);
			return magazine;
		}

		public void CopyTo(Magazine instance)
		{
			instance.capacity = this.capacity;
			instance.contents = this.contents;
			instance.ammoType = this.ammoType;
		}

		public static Magazine Deserialize(Stream stream)
		{
			Magazine magazine = Pool.Get<Magazine>();
			Magazine.Deserialize(stream, magazine, false);
			return magazine;
		}

		public static Magazine Deserialize(byte[] buffer)
		{
			Magazine magazine = Pool.Get<Magazine>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Magazine.Deserialize(memoryStream, magazine, false);
			}
			return magazine;
		}

		public static Magazine Deserialize(byte[] buffer, Magazine instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Magazine.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static Magazine Deserialize(Stream stream, Magazine instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num == 8)
				{
					instance.capacity = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 16)
				{
					instance.contents = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 24)
				{
					instance.ammoType = (int)ProtocolParser.ReadUInt64(stream);
				}
				else
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
			}
			return instance;
		}

		public static Magazine DeserializeLength(Stream stream, int length)
		{
			Magazine magazine = Pool.Get<Magazine>();
			Magazine.DeserializeLength(stream, length, magazine, false);
			return magazine;
		}

		public static Magazine DeserializeLength(Stream stream, int length, Magazine instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num == 8)
				{
					instance.capacity = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 16)
				{
					instance.contents = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 24)
				{
					instance.ammoType = (int)ProtocolParser.ReadUInt64(stream);
				}
				else
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static Magazine DeserializeLengthDelimited(Stream stream)
		{
			Magazine magazine = Pool.Get<Magazine>();
			Magazine.DeserializeLengthDelimited(stream, magazine, false);
			return magazine;
		}

		public static Magazine DeserializeLengthDelimited(Stream stream, Magazine instance, bool isDelta)
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
				if (num == 8)
				{
					instance.capacity = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 16)
				{
					instance.contents = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 24)
				{
					instance.ammoType = (int)ProtocolParser.ReadUInt64(stream);
				}
				else
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
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
			Magazine.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			Magazine.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(Magazine instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.capacity = 0;
			instance.contents = 0;
			instance.ammoType = 0;
			Pool.Free<Magazine>(ref instance);
		}

		public void ResetToPool()
		{
			Magazine.ResetToPool(this);
		}

		public static void Serialize(Stream stream, Magazine instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.capacity);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.contents);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.ammoType);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, Magazine instance, Magazine previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.capacity != previous.capacity)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.capacity);
			}
			if (instance.contents != previous.contents)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.contents);
			}
			if (instance.ammoType != previous.ammoType)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.ammoType);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, Magazine instance)
		{
			byte[] bytes = Magazine.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(Magazine instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Magazine.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			Magazine.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return Magazine.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			Magazine.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, Magazine previous)
		{
			if (previous == null)
			{
				Magazine.Serialize(stream, this);
				return;
			}
			Magazine.SerializeDelta(stream, this, previous);
		}
	}
}