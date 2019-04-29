using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class HeldEntity : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public uint itemUID;

		public bool ShouldPool = true;

		private bool _disposed;

		public HeldEntity()
		{
		}

		public HeldEntity Copy()
		{
			HeldEntity heldEntity = Pool.Get<HeldEntity>();
			this.CopyTo(heldEntity);
			return heldEntity;
		}

		public void CopyTo(HeldEntity instance)
		{
			instance.itemUID = this.itemUID;
		}

		public static HeldEntity Deserialize(Stream stream)
		{
			HeldEntity heldEntity = Pool.Get<HeldEntity>();
			HeldEntity.Deserialize(stream, heldEntity, false);
			return heldEntity;
		}

		public static HeldEntity Deserialize(byte[] buffer)
		{
			HeldEntity heldEntity = Pool.Get<HeldEntity>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				HeldEntity.Deserialize(memoryStream, heldEntity, false);
			}
			return heldEntity;
		}

		public static HeldEntity Deserialize(byte[] buffer, HeldEntity instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				HeldEntity.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static HeldEntity Deserialize(Stream stream, HeldEntity instance, bool isDelta)
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
					instance.itemUID = ProtocolParser.ReadUInt32(stream);
				}
			}
			return instance;
		}

		public static HeldEntity DeserializeLength(Stream stream, int length)
		{
			HeldEntity heldEntity = Pool.Get<HeldEntity>();
			HeldEntity.DeserializeLength(stream, length, heldEntity, false);
			return heldEntity;
		}

		public static HeldEntity DeserializeLength(Stream stream, int length, HeldEntity instance, bool isDelta)
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
					instance.itemUID = ProtocolParser.ReadUInt32(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static HeldEntity DeserializeLengthDelimited(Stream stream)
		{
			HeldEntity heldEntity = Pool.Get<HeldEntity>();
			HeldEntity.DeserializeLengthDelimited(stream, heldEntity, false);
			return heldEntity;
		}

		public static HeldEntity DeserializeLengthDelimited(Stream stream, HeldEntity instance, bool isDelta)
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
					instance.itemUID = ProtocolParser.ReadUInt32(stream);
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
			HeldEntity.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			HeldEntity.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(HeldEntity instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.itemUID = 0;
			Pool.Free<HeldEntity>(ref instance);
		}

		public void ResetToPool()
		{
			HeldEntity.ResetToPool(this);
		}

		public static void Serialize(Stream stream, HeldEntity instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.itemUID);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, HeldEntity instance, HeldEntity previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.itemUID != previous.itemUID)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.itemUID);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, HeldEntity instance)
		{
			byte[] bytes = HeldEntity.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(HeldEntity instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				HeldEntity.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			HeldEntity.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return HeldEntity.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			HeldEntity.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, HeldEntity previous)
		{
			if (previous == null)
			{
				HeldEntity.Serialize(stream, this);
				return;
			}
			HeldEntity.SerializeDelta(stream, this, previous);
		}
	}
}