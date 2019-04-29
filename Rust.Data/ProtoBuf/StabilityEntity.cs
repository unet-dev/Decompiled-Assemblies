using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class StabilityEntity : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public float stability;

		[NonSerialized]
		public int distanceFromGround;

		public bool ShouldPool = true;

		private bool _disposed;

		public StabilityEntity()
		{
		}

		public StabilityEntity Copy()
		{
			StabilityEntity stabilityEntity = Pool.Get<StabilityEntity>();
			this.CopyTo(stabilityEntity);
			return stabilityEntity;
		}

		public void CopyTo(StabilityEntity instance)
		{
			instance.stability = this.stability;
			instance.distanceFromGround = this.distanceFromGround;
		}

		public static StabilityEntity Deserialize(Stream stream)
		{
			StabilityEntity stabilityEntity = Pool.Get<StabilityEntity>();
			StabilityEntity.Deserialize(stream, stabilityEntity, false);
			return stabilityEntity;
		}

		public static StabilityEntity Deserialize(byte[] buffer)
		{
			StabilityEntity stabilityEntity = Pool.Get<StabilityEntity>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				StabilityEntity.Deserialize(memoryStream, stabilityEntity, false);
			}
			return stabilityEntity;
		}

		public static StabilityEntity Deserialize(byte[] buffer, StabilityEntity instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				StabilityEntity.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static StabilityEntity Deserialize(Stream stream, StabilityEntity instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num == 13)
				{
					instance.stability = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 16)
				{
					instance.distanceFromGround = (int)ProtocolParser.ReadUInt64(stream);
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

		public static StabilityEntity DeserializeLength(Stream stream, int length)
		{
			StabilityEntity stabilityEntity = Pool.Get<StabilityEntity>();
			StabilityEntity.DeserializeLength(stream, length, stabilityEntity, false);
			return stabilityEntity;
		}

		public static StabilityEntity DeserializeLength(Stream stream, int length, StabilityEntity instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num == 13)
				{
					instance.stability = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 16)
				{
					instance.distanceFromGround = (int)ProtocolParser.ReadUInt64(stream);
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

		public static StabilityEntity DeserializeLengthDelimited(Stream stream)
		{
			StabilityEntity stabilityEntity = Pool.Get<StabilityEntity>();
			StabilityEntity.DeserializeLengthDelimited(stream, stabilityEntity, false);
			return stabilityEntity;
		}

		public static StabilityEntity DeserializeLengthDelimited(Stream stream, StabilityEntity instance, bool isDelta)
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
				if (num == 13)
				{
					instance.stability = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 16)
				{
					instance.distanceFromGround = (int)ProtocolParser.ReadUInt64(stream);
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
			StabilityEntity.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			StabilityEntity.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(StabilityEntity instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.stability = 0f;
			instance.distanceFromGround = 0;
			Pool.Free<StabilityEntity>(ref instance);
		}

		public void ResetToPool()
		{
			StabilityEntity.ResetToPool(this);
		}

		public static void Serialize(Stream stream, StabilityEntity instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(13);
			ProtocolParser.WriteSingle(stream, instance.stability);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.distanceFromGround);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, StabilityEntity instance, StabilityEntity previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.stability != previous.stability)
			{
				stream.WriteByte(13);
				ProtocolParser.WriteSingle(stream, instance.stability);
			}
			if (instance.distanceFromGround != previous.distanceFromGround)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.distanceFromGround);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, StabilityEntity instance)
		{
			byte[] bytes = StabilityEntity.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(StabilityEntity instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				StabilityEntity.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			StabilityEntity.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return StabilityEntity.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			StabilityEntity.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, StabilityEntity previous)
		{
			if (previous == null)
			{
				StabilityEntity.Serialize(stream, this);
				return;
			}
			StabilityEntity.SerializeDelta(stream, this, previous);
		}
	}
}