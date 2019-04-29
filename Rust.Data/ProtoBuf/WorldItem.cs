using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class WorldItem : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public Item item;

		public bool ShouldPool = true;

		private bool _disposed;

		public WorldItem()
		{
		}

		public WorldItem Copy()
		{
			WorldItem worldItem = Pool.Get<WorldItem>();
			this.CopyTo(worldItem);
			return worldItem;
		}

		public void CopyTo(WorldItem instance)
		{
			if (this.item == null)
			{
				instance.item = null;
				return;
			}
			if (instance.item == null)
			{
				instance.item = this.item.Copy();
				return;
			}
			this.item.CopyTo(instance.item);
		}

		public static WorldItem Deserialize(Stream stream)
		{
			WorldItem worldItem = Pool.Get<WorldItem>();
			WorldItem.Deserialize(stream, worldItem, false);
			return worldItem;
		}

		public static WorldItem Deserialize(byte[] buffer)
		{
			WorldItem worldItem = Pool.Get<WorldItem>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				WorldItem.Deserialize(memoryStream, worldItem, false);
			}
			return worldItem;
		}

		public static WorldItem Deserialize(byte[] buffer, WorldItem instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				WorldItem.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static WorldItem Deserialize(Stream stream, WorldItem instance, bool isDelta)
		{
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
				else if (instance.item != null)
				{
					Item.DeserializeLengthDelimited(stream, instance.item, isDelta);
				}
				else
				{
					instance.item = Item.DeserializeLengthDelimited(stream);
				}
			}
			return instance;
		}

		public static WorldItem DeserializeLength(Stream stream, int length)
		{
			WorldItem worldItem = Pool.Get<WorldItem>();
			WorldItem.DeserializeLength(stream, length, worldItem, false);
			return worldItem;
		}

		public static WorldItem DeserializeLength(Stream stream, int length, WorldItem instance, bool isDelta)
		{
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
				else if (instance.item != null)
				{
					Item.DeserializeLengthDelimited(stream, instance.item, isDelta);
				}
				else
				{
					instance.item = Item.DeserializeLengthDelimited(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static WorldItem DeserializeLengthDelimited(Stream stream)
		{
			WorldItem worldItem = Pool.Get<WorldItem>();
			WorldItem.DeserializeLengthDelimited(stream, worldItem, false);
			return worldItem;
		}

		public static WorldItem DeserializeLengthDelimited(Stream stream, WorldItem instance, bool isDelta)
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
				if (num != 10)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else if (instance.item != null)
				{
					Item.DeserializeLengthDelimited(stream, instance.item, isDelta);
				}
				else
				{
					instance.item = Item.DeserializeLengthDelimited(stream);
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
			WorldItem.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			WorldItem.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(WorldItem instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.item != null)
			{
				instance.item.ResetToPool();
				instance.item = null;
			}
			Pool.Free<WorldItem>(ref instance);
		}

		public void ResetToPool()
		{
			WorldItem.ResetToPool(this);
		}

		public static void Serialize(Stream stream, WorldItem instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.item != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				Item.Serialize(memoryStream, instance.item);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, WorldItem instance, WorldItem previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.item != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				Item.SerializeDelta(memoryStream, instance.item, previous.item);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, WorldItem instance)
		{
			byte[] bytes = WorldItem.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(WorldItem instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				WorldItem.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			WorldItem.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return WorldItem.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			WorldItem.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, WorldItem previous)
		{
			if (previous == null)
			{
				WorldItem.Serialize(stream, this);
				return;
			}
			WorldItem.SerializeDelta(stream, this, previous);
		}
	}
}