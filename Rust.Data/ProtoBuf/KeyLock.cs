using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class KeyLock : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public int code;

		public bool ShouldPool = true;

		private bool _disposed;

		public KeyLock()
		{
		}

		public KeyLock Copy()
		{
			KeyLock keyLock = Pool.Get<KeyLock>();
			this.CopyTo(keyLock);
			return keyLock;
		}

		public void CopyTo(KeyLock instance)
		{
			instance.code = this.code;
		}

		public static KeyLock Deserialize(Stream stream)
		{
			KeyLock keyLock = Pool.Get<KeyLock>();
			KeyLock.Deserialize(stream, keyLock, false);
			return keyLock;
		}

		public static KeyLock Deserialize(byte[] buffer)
		{
			KeyLock keyLock = Pool.Get<KeyLock>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				KeyLock.Deserialize(memoryStream, keyLock, false);
			}
			return keyLock;
		}

		public static KeyLock Deserialize(byte[] buffer, KeyLock instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				KeyLock.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static KeyLock Deserialize(Stream stream, KeyLock instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num != 8)
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
					instance.code = (int)ProtocolParser.ReadUInt64(stream);
				}
			}
			return instance;
		}

		public static KeyLock DeserializeLength(Stream stream, int length)
		{
			KeyLock keyLock = Pool.Get<KeyLock>();
			KeyLock.DeserializeLength(stream, length, keyLock, false);
			return keyLock;
		}

		public static KeyLock DeserializeLength(Stream stream, int length, KeyLock instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num != 8)
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
					instance.code = (int)ProtocolParser.ReadUInt64(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static KeyLock DeserializeLengthDelimited(Stream stream)
		{
			KeyLock keyLock = Pool.Get<KeyLock>();
			KeyLock.DeserializeLengthDelimited(stream, keyLock, false);
			return keyLock;
		}

		public static KeyLock DeserializeLengthDelimited(Stream stream, KeyLock instance, bool isDelta)
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
				if (num != 8)
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
					instance.code = (int)ProtocolParser.ReadUInt64(stream);
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
			KeyLock.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			KeyLock.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(KeyLock instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.code = 0;
			Pool.Free<KeyLock>(ref instance);
		}

		public void ResetToPool()
		{
			KeyLock.ResetToPool(this);
		}

		public static void Serialize(Stream stream, KeyLock instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.code);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, KeyLock instance, KeyLock previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.code != previous.code)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.code);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, KeyLock instance)
		{
			byte[] bytes = KeyLock.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(KeyLock instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				KeyLock.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			KeyLock.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return KeyLock.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			KeyLock.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, KeyLock previous)
		{
			if (previous == null)
			{
				KeyLock.Serialize(stream, this);
				return;
			}
			KeyLock.SerializeDelta(stream, this, previous);
		}
	}
}