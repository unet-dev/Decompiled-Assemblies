using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class UpdateItem : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public Item item;

		public bool ShouldPool = true;

		private bool _disposed;

		public UpdateItem()
		{
		}

		public UpdateItem Copy()
		{
			UpdateItem updateItem = Pool.Get<UpdateItem>();
			this.CopyTo(updateItem);
			return updateItem;
		}

		public void CopyTo(UpdateItem instance)
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

		public static UpdateItem Deserialize(Stream stream)
		{
			UpdateItem updateItem = Pool.Get<UpdateItem>();
			UpdateItem.Deserialize(stream, updateItem, false);
			return updateItem;
		}

		public static UpdateItem Deserialize(byte[] buffer)
		{
			UpdateItem updateItem = Pool.Get<UpdateItem>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				UpdateItem.Deserialize(memoryStream, updateItem, false);
			}
			return updateItem;
		}

		public static UpdateItem Deserialize(byte[] buffer, UpdateItem instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				UpdateItem.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static UpdateItem Deserialize(Stream stream, UpdateItem instance, bool isDelta)
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

		public static UpdateItem DeserializeLength(Stream stream, int length)
		{
			UpdateItem updateItem = Pool.Get<UpdateItem>();
			UpdateItem.DeserializeLength(stream, length, updateItem, false);
			return updateItem;
		}

		public static UpdateItem DeserializeLength(Stream stream, int length, UpdateItem instance, bool isDelta)
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

		public static UpdateItem DeserializeLengthDelimited(Stream stream)
		{
			UpdateItem updateItem = Pool.Get<UpdateItem>();
			UpdateItem.DeserializeLengthDelimited(stream, updateItem, false);
			return updateItem;
		}

		public static UpdateItem DeserializeLengthDelimited(Stream stream, UpdateItem instance, bool isDelta)
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
			UpdateItem.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			UpdateItem.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(UpdateItem instance)
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
			Pool.Free<UpdateItem>(ref instance);
		}

		public void ResetToPool()
		{
			UpdateItem.ResetToPool(this);
		}

		public static void Serialize(Stream stream, UpdateItem instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.item == null)
			{
				throw new ArgumentNullException("item", "Required by proto specification.");
			}
			stream.WriteByte(10);
			memoryStream.SetLength((long)0);
			Item.Serialize(memoryStream, instance.item);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, UpdateItem instance, UpdateItem previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.item == null)
			{
				throw new ArgumentNullException("item", "Required by proto specification.");
			}
			stream.WriteByte(10);
			memoryStream.SetLength((long)0);
			Item.SerializeDelta(memoryStream, instance.item, previous.item);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, UpdateItem instance)
		{
			byte[] bytes = UpdateItem.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(UpdateItem instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				UpdateItem.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			UpdateItem.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return UpdateItem.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			UpdateItem.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, UpdateItem previous)
		{
			if (previous == null)
			{
				UpdateItem.Serialize(stream, this);
				return;
			}
			UpdateItem.SerializeDelta(stream, this, previous);
		}
	}
}