using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class SphereEntity : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public float radius;

		public bool ShouldPool = true;

		private bool _disposed;

		public SphereEntity()
		{
		}

		public SphereEntity Copy()
		{
			SphereEntity sphereEntity = Pool.Get<SphereEntity>();
			this.CopyTo(sphereEntity);
			return sphereEntity;
		}

		public void CopyTo(SphereEntity instance)
		{
			instance.radius = this.radius;
		}

		public static SphereEntity Deserialize(Stream stream)
		{
			SphereEntity sphereEntity = Pool.Get<SphereEntity>();
			SphereEntity.Deserialize(stream, sphereEntity, false);
			return sphereEntity;
		}

		public static SphereEntity Deserialize(byte[] buffer)
		{
			SphereEntity sphereEntity = Pool.Get<SphereEntity>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				SphereEntity.Deserialize(memoryStream, sphereEntity, false);
			}
			return sphereEntity;
		}

		public static SphereEntity Deserialize(byte[] buffer, SphereEntity instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				SphereEntity.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static SphereEntity Deserialize(Stream stream, SphereEntity instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num != 13)
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
					instance.radius = ProtocolParser.ReadSingle(stream);
				}
			}
			return instance;
		}

		public static SphereEntity DeserializeLength(Stream stream, int length)
		{
			SphereEntity sphereEntity = Pool.Get<SphereEntity>();
			SphereEntity.DeserializeLength(stream, length, sphereEntity, false);
			return sphereEntity;
		}

		public static SphereEntity DeserializeLength(Stream stream, int length, SphereEntity instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num != 13)
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
					instance.radius = ProtocolParser.ReadSingle(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static SphereEntity DeserializeLengthDelimited(Stream stream)
		{
			SphereEntity sphereEntity = Pool.Get<SphereEntity>();
			SphereEntity.DeserializeLengthDelimited(stream, sphereEntity, false);
			return sphereEntity;
		}

		public static SphereEntity DeserializeLengthDelimited(Stream stream, SphereEntity instance, bool isDelta)
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
				if (num != 13)
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
					instance.radius = ProtocolParser.ReadSingle(stream);
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
			SphereEntity.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			SphereEntity.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(SphereEntity instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.radius = 0f;
			Pool.Free<SphereEntity>(ref instance);
		}

		public void ResetToPool()
		{
			SphereEntity.ResetToPool(this);
		}

		public static void Serialize(Stream stream, SphereEntity instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(13);
			ProtocolParser.WriteSingle(stream, instance.radius);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, SphereEntity instance, SphereEntity previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.radius != previous.radius)
			{
				stream.WriteByte(13);
				ProtocolParser.WriteSingle(stream, instance.radius);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, SphereEntity instance)
		{
			byte[] bytes = SphereEntity.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(SphereEntity instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				SphereEntity.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			SphereEntity.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return SphereEntity.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			SphereEntity.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, SphereEntity previous)
		{
			if (previous == null)
			{
				SphereEntity.Serialize(stream, this);
				return;
			}
			SphereEntity.SerializeDelta(stream, this, previous);
		}
	}
}