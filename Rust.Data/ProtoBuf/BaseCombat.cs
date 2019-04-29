using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class BaseCombat : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public int state;

		[NonSerialized]
		public float health;

		public bool ShouldPool = true;

		private bool _disposed;

		public BaseCombat()
		{
		}

		public BaseCombat Copy()
		{
			BaseCombat baseCombat = Pool.Get<BaseCombat>();
			this.CopyTo(baseCombat);
			return baseCombat;
		}

		public void CopyTo(BaseCombat instance)
		{
			instance.state = this.state;
			instance.health = this.health;
		}

		public static BaseCombat Deserialize(Stream stream)
		{
			BaseCombat baseCombat = Pool.Get<BaseCombat>();
			BaseCombat.Deserialize(stream, baseCombat, false);
			return baseCombat;
		}

		public static BaseCombat Deserialize(byte[] buffer)
		{
			BaseCombat baseCombat = Pool.Get<BaseCombat>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BaseCombat.Deserialize(memoryStream, baseCombat, false);
			}
			return baseCombat;
		}

		public static BaseCombat Deserialize(byte[] buffer, BaseCombat instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BaseCombat.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static BaseCombat Deserialize(Stream stream, BaseCombat instance, bool isDelta)
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
					instance.state = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 21)
				{
					instance.health = ProtocolParser.ReadSingle(stream);
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

		public static BaseCombat DeserializeLength(Stream stream, int length)
		{
			BaseCombat baseCombat = Pool.Get<BaseCombat>();
			BaseCombat.DeserializeLength(stream, length, baseCombat, false);
			return baseCombat;
		}

		public static BaseCombat DeserializeLength(Stream stream, int length, BaseCombat instance, bool isDelta)
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
					instance.state = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 21)
				{
					instance.health = ProtocolParser.ReadSingle(stream);
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

		public static BaseCombat DeserializeLengthDelimited(Stream stream)
		{
			BaseCombat baseCombat = Pool.Get<BaseCombat>();
			BaseCombat.DeserializeLengthDelimited(stream, baseCombat, false);
			return baseCombat;
		}

		public static BaseCombat DeserializeLengthDelimited(Stream stream, BaseCombat instance, bool isDelta)
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
					instance.state = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 21)
				{
					instance.health = ProtocolParser.ReadSingle(stream);
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
			BaseCombat.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			BaseCombat.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(BaseCombat instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.state = 0;
			instance.health = 0f;
			Pool.Free<BaseCombat>(ref instance);
		}

		public void ResetToPool()
		{
			BaseCombat.ResetToPool(this);
		}

		public static void Serialize(Stream stream, BaseCombat instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.state);
			stream.WriteByte(21);
			ProtocolParser.WriteSingle(stream, instance.health);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, BaseCombat instance, BaseCombat previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.state != previous.state)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.state);
			}
			if (instance.health != previous.health)
			{
				stream.WriteByte(21);
				ProtocolParser.WriteSingle(stream, instance.health);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, BaseCombat instance)
		{
			byte[] bytes = BaseCombat.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(BaseCombat instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BaseCombat.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			BaseCombat.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return BaseCombat.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			BaseCombat.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, BaseCombat previous)
		{
			if (previous == null)
			{
				BaseCombat.Serialize(stream, this);
				return;
			}
			BaseCombat.SerializeDelta(stream, this, previous);
		}
	}
}