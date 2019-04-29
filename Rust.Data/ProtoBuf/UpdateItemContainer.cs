using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProtoBuf
{
	public class UpdateItemContainer : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public int type;

		[NonSerialized]
		public List<ItemContainer> container;

		public bool ShouldPool = true;

		private bool _disposed;

		public UpdateItemContainer()
		{
		}

		public UpdateItemContainer Copy()
		{
			UpdateItemContainer updateItemContainer = Pool.Get<UpdateItemContainer>();
			this.CopyTo(updateItemContainer);
			return updateItemContainer;
		}

		public void CopyTo(UpdateItemContainer instance)
		{
			instance.type = this.type;
			throw new NotImplementedException();
		}

		public static UpdateItemContainer Deserialize(Stream stream)
		{
			UpdateItemContainer updateItemContainer = Pool.Get<UpdateItemContainer>();
			UpdateItemContainer.Deserialize(stream, updateItemContainer, false);
			return updateItemContainer;
		}

		public static UpdateItemContainer Deserialize(byte[] buffer)
		{
			UpdateItemContainer updateItemContainer = Pool.Get<UpdateItemContainer>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				UpdateItemContainer.Deserialize(memoryStream, updateItemContainer, false);
			}
			return updateItemContainer;
		}

		public static UpdateItemContainer Deserialize(byte[] buffer, UpdateItemContainer instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				UpdateItemContainer.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static UpdateItemContainer Deserialize(Stream stream, UpdateItemContainer instance, bool isDelta)
		{
			if (!isDelta && instance.container == null)
			{
				instance.container = Pool.Get<List<ItemContainer>>();
			}
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num == 8)
				{
					instance.type = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 18)
				{
					instance.container.Add(ItemContainer.DeserializeLengthDelimited(stream));
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

		public static UpdateItemContainer DeserializeLength(Stream stream, int length)
		{
			UpdateItemContainer updateItemContainer = Pool.Get<UpdateItemContainer>();
			UpdateItemContainer.DeserializeLength(stream, length, updateItemContainer, false);
			return updateItemContainer;
		}

		public static UpdateItemContainer DeserializeLength(Stream stream, int length, UpdateItemContainer instance, bool isDelta)
		{
			if (!isDelta && instance.container == null)
			{
				instance.container = Pool.Get<List<ItemContainer>>();
			}
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
					instance.type = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 18)
				{
					instance.container.Add(ItemContainer.DeserializeLengthDelimited(stream));
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

		public static UpdateItemContainer DeserializeLengthDelimited(Stream stream)
		{
			UpdateItemContainer updateItemContainer = Pool.Get<UpdateItemContainer>();
			UpdateItemContainer.DeserializeLengthDelimited(stream, updateItemContainer, false);
			return updateItemContainer;
		}

		public static UpdateItemContainer DeserializeLengthDelimited(Stream stream, UpdateItemContainer instance, bool isDelta)
		{
			if (!isDelta && instance.container == null)
			{
				instance.container = Pool.Get<List<ItemContainer>>();
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
				if (num == 8)
				{
					instance.type = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 18)
				{
					instance.container.Add(ItemContainer.DeserializeLengthDelimited(stream));
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
			UpdateItemContainer.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			UpdateItemContainer.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(UpdateItemContainer instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.type = 0;
			if (instance.container != null)
			{
				for (int i = 0; i < instance.container.Count; i++)
				{
					if (instance.container[i] != null)
					{
						instance.container[i].ResetToPool();
						instance.container[i] = null;
					}
				}
				List<ItemContainer> itemContainers = instance.container;
				Pool.FreeList<ItemContainer>(ref itemContainers);
				instance.container = itemContainers;
			}
			Pool.Free<UpdateItemContainer>(ref instance);
		}

		public void ResetToPool()
		{
			UpdateItemContainer.ResetToPool(this);
		}

		public static void Serialize(Stream stream, UpdateItemContainer instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.type);
			if (instance.container != null)
			{
				for (int i = 0; i < instance.container.Count; i++)
				{
					ItemContainer item = instance.container[i];
					stream.WriteByte(18);
					memoryStream.SetLength((long)0);
					ItemContainer.Serialize(memoryStream, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, UpdateItemContainer instance, UpdateItemContainer previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.type != previous.type)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.type);
			}
			if (instance.container != null)
			{
				for (int i = 0; i < instance.container.Count; i++)
				{
					ItemContainer item = instance.container[i];
					stream.WriteByte(18);
					memoryStream.SetLength((long)0);
					ItemContainer.SerializeDelta(memoryStream, item, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, UpdateItemContainer instance)
		{
			byte[] bytes = UpdateItemContainer.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(UpdateItemContainer instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				UpdateItemContainer.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			UpdateItemContainer.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return UpdateItemContainer.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			UpdateItemContainer.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, UpdateItemContainer previous)
		{
			if (previous == null)
			{
				UpdateItemContainer.Serialize(stream, this);
				return;
			}
			UpdateItemContainer.SerializeDelta(stream, this, previous);
		}
	}
}