using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class BaseResource : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public int stage;

		[NonSerialized]
		public float health;

		public bool ShouldPool = true;

		private bool _disposed;

		public BaseResource()
		{
		}

		public BaseResource Copy()
		{
			BaseResource baseResource = Pool.Get<BaseResource>();
			this.CopyTo(baseResource);
			return baseResource;
		}

		public void CopyTo(BaseResource instance)
		{
			instance.stage = this.stage;
			instance.health = this.health;
		}

		public static BaseResource Deserialize(Stream stream)
		{
			BaseResource baseResource = Pool.Get<BaseResource>();
			BaseResource.Deserialize(stream, baseResource, false);
			return baseResource;
		}

		public static BaseResource Deserialize(byte[] buffer)
		{
			BaseResource baseResource = Pool.Get<BaseResource>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BaseResource.Deserialize(memoryStream, baseResource, false);
			}
			return baseResource;
		}

		public static BaseResource Deserialize(byte[] buffer, BaseResource instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BaseResource.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static BaseResource Deserialize(Stream stream, BaseResource instance, bool isDelta)
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
					instance.stage = (int)ProtocolParser.ReadUInt64(stream);
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

		public static BaseResource DeserializeLength(Stream stream, int length)
		{
			BaseResource baseResource = Pool.Get<BaseResource>();
			BaseResource.DeserializeLength(stream, length, baseResource, false);
			return baseResource;
		}

		public static BaseResource DeserializeLength(Stream stream, int length, BaseResource instance, bool isDelta)
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
					instance.stage = (int)ProtocolParser.ReadUInt64(stream);
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

		public static BaseResource DeserializeLengthDelimited(Stream stream)
		{
			BaseResource baseResource = Pool.Get<BaseResource>();
			BaseResource.DeserializeLengthDelimited(stream, baseResource, false);
			return baseResource;
		}

		public static BaseResource DeserializeLengthDelimited(Stream stream, BaseResource instance, bool isDelta)
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
					instance.stage = (int)ProtocolParser.ReadUInt64(stream);
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
			BaseResource.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			BaseResource.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(BaseResource instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.stage = 0;
			instance.health = 0f;
			Pool.Free<BaseResource>(ref instance);
		}

		public void ResetToPool()
		{
			BaseResource.ResetToPool(this);
		}

		public static void Serialize(Stream stream, BaseResource instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.stage);
			stream.WriteByte(21);
			ProtocolParser.WriteSingle(stream, instance.health);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, BaseResource instance, BaseResource previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.stage != previous.stage)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.stage);
			}
			if (instance.health != previous.health)
			{
				stream.WriteByte(21);
				ProtocolParser.WriteSingle(stream, instance.health);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, BaseResource instance)
		{
			byte[] bytes = BaseResource.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(BaseResource instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BaseResource.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			BaseResource.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return BaseResource.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			BaseResource.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, BaseResource previous)
		{
			if (previous == null)
			{
				BaseResource.Serialize(stream, this);
				return;
			}
			BaseResource.SerializeDelta(stream, this, previous);
		}
	}
}