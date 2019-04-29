using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class EntitySlots : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public uint slotLock;

		[NonSerialized]
		public uint slotFireMod;

		[NonSerialized]
		public uint slotUpperModification;

		[NonSerialized]
		public uint centerDecoration;

		[NonSerialized]
		public uint lowerCenterDecoration;

		public bool ShouldPool = true;

		private bool _disposed;

		public EntitySlots()
		{
		}

		public EntitySlots Copy()
		{
			EntitySlots entitySlot = Pool.Get<EntitySlots>();
			this.CopyTo(entitySlot);
			return entitySlot;
		}

		public void CopyTo(EntitySlots instance)
		{
			instance.slotLock = this.slotLock;
			instance.slotFireMod = this.slotFireMod;
			instance.slotUpperModification = this.slotUpperModification;
			instance.centerDecoration = this.centerDecoration;
			instance.lowerCenterDecoration = this.lowerCenterDecoration;
		}

		public static EntitySlots Deserialize(Stream stream)
		{
			EntitySlots entitySlot = Pool.Get<EntitySlots>();
			EntitySlots.Deserialize(stream, entitySlot, false);
			return entitySlot;
		}

		public static EntitySlots Deserialize(byte[] buffer)
		{
			EntitySlots entitySlot = Pool.Get<EntitySlots>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				EntitySlots.Deserialize(memoryStream, entitySlot, false);
			}
			return entitySlot;
		}

		public static EntitySlots Deserialize(byte[] buffer, EntitySlots instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				EntitySlots.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static EntitySlots Deserialize(Stream stream, EntitySlots instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 16)
				{
					if (num == 8)
					{
						instance.slotLock = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 16)
					{
						instance.slotFireMod = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num == 24)
				{
					instance.slotUpperModification = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 32)
				{
					instance.centerDecoration = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 40)
				{
					instance.lowerCenterDecoration = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				Key key = ProtocolParser.ReadKey((byte)num, stream);
				if (key.Field == 0)
				{
					throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
				}
				ProtocolParser.SkipKey(stream, key);
			}
			return instance;
		}

		public static EntitySlots DeserializeLength(Stream stream, int length)
		{
			EntitySlots entitySlot = Pool.Get<EntitySlots>();
			EntitySlots.DeserializeLength(stream, length, entitySlot, false);
			return entitySlot;
		}

		public static EntitySlots DeserializeLength(Stream stream, int length, EntitySlots instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num <= 16)
				{
					if (num == 8)
					{
						instance.slotLock = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 16)
					{
						instance.slotFireMod = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num == 24)
				{
					instance.slotUpperModification = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 32)
				{
					instance.centerDecoration = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 40)
				{
					instance.lowerCenterDecoration = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				Key key = ProtocolParser.ReadKey((byte)num, stream);
				if (key.Field == 0)
				{
					throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
				}
				ProtocolParser.SkipKey(stream, key);
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static EntitySlots DeserializeLengthDelimited(Stream stream)
		{
			EntitySlots entitySlot = Pool.Get<EntitySlots>();
			EntitySlots.DeserializeLengthDelimited(stream, entitySlot, false);
			return entitySlot;
		}

		public static EntitySlots DeserializeLengthDelimited(Stream stream, EntitySlots instance, bool isDelta)
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
				if (num <= 16)
				{
					if (num == 8)
					{
						instance.slotLock = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 16)
					{
						instance.slotFireMod = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num == 24)
				{
					instance.slotUpperModification = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 32)
				{
					instance.centerDecoration = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 40)
				{
					instance.lowerCenterDecoration = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				Key key = ProtocolParser.ReadKey((byte)num, stream);
				if (key.Field == 0)
				{
					throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
				}
				ProtocolParser.SkipKey(stream, key);
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
			EntitySlots.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			EntitySlots.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(EntitySlots instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.slotLock = 0;
			instance.slotFireMod = 0;
			instance.slotUpperModification = 0;
			instance.centerDecoration = 0;
			instance.lowerCenterDecoration = 0;
			Pool.Free<EntitySlots>(ref instance);
		}

		public void ResetToPool()
		{
			EntitySlots.ResetToPool(this);
		}

		public static void Serialize(Stream stream, EntitySlots instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.slotLock);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt32(stream, instance.slotFireMod);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt32(stream, instance.slotUpperModification);
			stream.WriteByte(32);
			ProtocolParser.WriteUInt32(stream, instance.centerDecoration);
			stream.WriteByte(40);
			ProtocolParser.WriteUInt32(stream, instance.lowerCenterDecoration);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, EntitySlots instance, EntitySlots previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.slotLock != previous.slotLock)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.slotLock);
			}
			if (instance.slotFireMod != previous.slotFireMod)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.slotFireMod);
			}
			if (instance.slotUpperModification != previous.slotUpperModification)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.slotUpperModification);
			}
			if (instance.centerDecoration != previous.centerDecoration)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt32(stream, instance.centerDecoration);
			}
			if (instance.lowerCenterDecoration != previous.lowerCenterDecoration)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteUInt32(stream, instance.lowerCenterDecoration);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, EntitySlots instance)
		{
			byte[] bytes = EntitySlots.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(EntitySlots instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				EntitySlots.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			EntitySlots.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return EntitySlots.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			EntitySlots.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, EntitySlots previous)
		{
			if (previous == null)
			{
				EntitySlots.Serialize(stream, this);
				return;
			}
			EntitySlots.SerializeDelta(stream, this, previous);
		}
	}
}