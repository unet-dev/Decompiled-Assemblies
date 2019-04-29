using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProtoBuf
{
	public class EntityList : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public List<Entity> entity;

		public bool ShouldPool = true;

		private bool _disposed;

		public EntityList()
		{
		}

		public EntityList Copy()
		{
			EntityList entityList = Pool.Get<EntityList>();
			this.CopyTo(entityList);
			return entityList;
		}

		public void CopyTo(EntityList instance)
		{
			throw new NotImplementedException();
		}

		public static EntityList Deserialize(Stream stream)
		{
			EntityList entityList = Pool.Get<EntityList>();
			EntityList.Deserialize(stream, entityList, false);
			return entityList;
		}

		public static EntityList Deserialize(byte[] buffer)
		{
			EntityList entityList = Pool.Get<EntityList>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				EntityList.Deserialize(memoryStream, entityList, false);
			}
			return entityList;
		}

		public static EntityList Deserialize(byte[] buffer, EntityList instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				EntityList.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static EntityList Deserialize(Stream stream, EntityList instance, bool isDelta)
		{
			if (!isDelta && instance.entity == null)
			{
				instance.entity = Pool.Get<List<Entity>>();
			}
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num != 10)
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
					instance.entity.Add(Entity.DeserializeLengthDelimited(stream));
				}
			}
			return instance;
		}

		public static EntityList DeserializeLength(Stream stream, int length)
		{
			EntityList entityList = Pool.Get<EntityList>();
			EntityList.DeserializeLength(stream, length, entityList, false);
			return entityList;
		}

		public static EntityList DeserializeLength(Stream stream, int length, EntityList instance, bool isDelta)
		{
			if (!isDelta && instance.entity == null)
			{
				instance.entity = Pool.Get<List<Entity>>();
			}
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num != 10)
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
					instance.entity.Add(Entity.DeserializeLengthDelimited(stream));
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static EntityList DeserializeLengthDelimited(Stream stream)
		{
			EntityList entityList = Pool.Get<EntityList>();
			EntityList.DeserializeLengthDelimited(stream, entityList, false);
			return entityList;
		}

		public static EntityList DeserializeLengthDelimited(Stream stream, EntityList instance, bool isDelta)
		{
			if (!isDelta && instance.entity == null)
			{
				instance.entity = Pool.Get<List<Entity>>();
			}
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num != 10)
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
					instance.entity.Add(Entity.DeserializeLengthDelimited(stream));
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
			EntityList.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			EntityList.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(EntityList instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.entity != null)
			{
				for (int i = 0; i < instance.entity.Count; i++)
				{
					if (instance.entity[i] != null)
					{
						instance.entity[i].ResetToPool();
						instance.entity[i] = null;
					}
				}
				List<Entity> entities = instance.entity;
				Pool.FreeList<Entity>(ref entities);
				instance.entity = entities;
			}
			Pool.Free<EntityList>(ref instance);
		}

		public void ResetToPool()
		{
			EntityList.ResetToPool(this);
		}

		public static void Serialize(Stream stream, EntityList instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.entity != null)
			{
				for (int i = 0; i < instance.entity.Count; i++)
				{
					Entity item = instance.entity[i];
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					Entity.Serialize(memoryStream, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, EntityList instance, EntityList previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.entity != null)
			{
				for (int i = 0; i < instance.entity.Count; i++)
				{
					Entity item = instance.entity[i];
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					Entity.SerializeDelta(memoryStream, item, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, EntityList instance)
		{
			byte[] bytes = EntityList.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(EntityList instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				EntityList.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			EntityList.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return EntityList.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			EntityList.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, EntityList previous)
		{
			if (previous == null)
			{
				EntityList.Serialize(stream, this);
				return;
			}
			EntityList.SerializeDelta(stream, this, previous);
		}
	}
}