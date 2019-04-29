using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProtoBuf
{
	public class ItemContainer : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public uint UID;

		[NonSerialized]
		public int slots;

		[NonSerialized]
		public float temperature;

		[NonSerialized]
		public int flags;

		[NonSerialized]
		public int allowedContents;

		[NonSerialized]
		public int maxStackSize;

		[NonSerialized]
		public int allowedItem;

		[NonSerialized]
		public List<int> availableSlots;

		[NonSerialized]
		public List<Item> contents;

		public bool ShouldPool = true;

		private bool _disposed;

		public ItemContainer()
		{
		}

		public ItemContainer Copy()
		{
			ItemContainer itemContainer = Pool.Get<ItemContainer>();
			this.CopyTo(itemContainer);
			return itemContainer;
		}

		public void CopyTo(ItemContainer instance)
		{
			instance.UID = this.UID;
			instance.slots = this.slots;
			instance.temperature = this.temperature;
			instance.flags = this.flags;
			instance.allowedContents = this.allowedContents;
			instance.maxStackSize = this.maxStackSize;
			instance.allowedItem = this.allowedItem;
			throw new NotImplementedException();
		}

		public static ItemContainer Deserialize(Stream stream)
		{
			ItemContainer itemContainer = Pool.Get<ItemContainer>();
			ItemContainer.Deserialize(stream, itemContainer, false);
			return itemContainer;
		}

		public static ItemContainer Deserialize(byte[] buffer)
		{
			ItemContainer itemContainer = Pool.Get<ItemContainer>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ItemContainer.Deserialize(memoryStream, itemContainer, false);
			}
			return itemContainer;
		}

		public static ItemContainer Deserialize(byte[] buffer, ItemContainer instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ItemContainer.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static ItemContainer Deserialize(Stream stream, ItemContainer instance, bool isDelta)
		{
			if (!isDelta)
			{
				if (instance.availableSlots == null)
				{
					instance.availableSlots = Pool.Get<List<int>>();
				}
				if (instance.contents == null)
				{
					instance.contents = Pool.Get<List<Item>>();
				}
			}
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 32)
				{
					if (num <= 16)
					{
						if (num == 8)
						{
							instance.UID = ProtocolParser.ReadUInt32(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.slots = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 29)
					{
						instance.temperature = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.flags = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num <= 48)
				{
					if (num == 40)
					{
						instance.allowedContents = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 48)
					{
						instance.maxStackSize = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num == 56)
				{
					instance.allowedItem = (int)ProtocolParser.ReadUInt64(stream);
					continue;
				}
				else if (num == 64)
				{
					instance.availableSlots.Add((int)ProtocolParser.ReadUInt64(stream));
					continue;
				}
				Key key = ProtocolParser.ReadKey((byte)num, stream);
				uint field = key.Field;
				if (field == 0)
				{
					throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
				}
				if (field != 100)
				{
					ProtocolParser.SkipKey(stream, key);
				}
				else if (key.WireType == Wire.LengthDelimited)
				{
					instance.contents.Add(Item.DeserializeLengthDelimited(stream));
				}
			}
			return instance;
		}

		public static ItemContainer DeserializeLength(Stream stream, int length)
		{
			ItemContainer itemContainer = Pool.Get<ItemContainer>();
			ItemContainer.DeserializeLength(stream, length, itemContainer, false);
			return itemContainer;
		}

		public static ItemContainer DeserializeLength(Stream stream, int length, ItemContainer instance, bool isDelta)
		{
			if (!isDelta)
			{
				if (instance.availableSlots == null)
				{
					instance.availableSlots = Pool.Get<List<int>>();
				}
				if (instance.contents == null)
				{
					instance.contents = Pool.Get<List<Item>>();
				}
			}
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num <= 32)
				{
					if (num <= 16)
					{
						if (num == 8)
						{
							instance.UID = ProtocolParser.ReadUInt32(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.slots = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 29)
					{
						instance.temperature = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.flags = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num <= 48)
				{
					if (num == 40)
					{
						instance.allowedContents = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 48)
					{
						instance.maxStackSize = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num == 56)
				{
					instance.allowedItem = (int)ProtocolParser.ReadUInt64(stream);
					continue;
				}
				else if (num == 64)
				{
					instance.availableSlots.Add((int)ProtocolParser.ReadUInt64(stream));
					continue;
				}
				Key key = ProtocolParser.ReadKey((byte)num, stream);
				uint field = key.Field;
				if (field == 0)
				{
					throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
				}
				if (field == 100)
				{
					if (key.WireType != Wire.LengthDelimited)
					{
						continue;
					}
					instance.contents.Add(Item.DeserializeLengthDelimited(stream));
				}
				else
				{
					ProtocolParser.SkipKey(stream, key);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static ItemContainer DeserializeLengthDelimited(Stream stream)
		{
			ItemContainer itemContainer = Pool.Get<ItemContainer>();
			ItemContainer.DeserializeLengthDelimited(stream, itemContainer, false);
			return itemContainer;
		}

		public static ItemContainer DeserializeLengthDelimited(Stream stream, ItemContainer instance, bool isDelta)
		{
			if (!isDelta)
			{
				if (instance.availableSlots == null)
				{
					instance.availableSlots = Pool.Get<List<int>>();
				}
				if (instance.contents == null)
				{
					instance.contents = Pool.Get<List<Item>>();
				}
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
				if (num <= 32)
				{
					if (num <= 16)
					{
						if (num == 8)
						{
							instance.UID = ProtocolParser.ReadUInt32(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.slots = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 29)
					{
						instance.temperature = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.flags = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num <= 48)
				{
					if (num == 40)
					{
						instance.allowedContents = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 48)
					{
						instance.maxStackSize = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num == 56)
				{
					instance.allowedItem = (int)ProtocolParser.ReadUInt64(stream);
					continue;
				}
				else if (num == 64)
				{
					instance.availableSlots.Add((int)ProtocolParser.ReadUInt64(stream));
					continue;
				}
				Key key = ProtocolParser.ReadKey((byte)num, stream);
				uint field = key.Field;
				if (field == 0)
				{
					throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
				}
				if (field == 100)
				{
					if (key.WireType != Wire.LengthDelimited)
					{
						continue;
					}
					instance.contents.Add(Item.DeserializeLengthDelimited(stream));
				}
				else
				{
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
			ItemContainer.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			ItemContainer.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(ItemContainer instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.UID = 0;
			instance.slots = 0;
			instance.temperature = 0f;
			instance.flags = 0;
			instance.allowedContents = 0;
			instance.maxStackSize = 0;
			instance.allowedItem = 0;
			if (instance.availableSlots != null)
			{
				List<int> nums = instance.availableSlots;
				Pool.FreeList<int>(ref nums);
				instance.availableSlots = nums;
			}
			if (instance.contents != null)
			{
				for (int i = 0; i < instance.contents.Count; i++)
				{
					if (instance.contents[i] != null)
					{
						instance.contents[i].ResetToPool();
						instance.contents[i] = null;
					}
				}
				List<Item> items = instance.contents;
				Pool.FreeList<Item>(ref items);
				instance.contents = items;
			}
			Pool.Free<ItemContainer>(ref instance);
		}

		public void ResetToPool()
		{
			ItemContainer.ResetToPool(this);
		}

		public static void Serialize(Stream stream, ItemContainer instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.UID);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.slots);
			stream.WriteByte(29);
			ProtocolParser.WriteSingle(stream, instance.temperature);
			stream.WriteByte(32);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.flags);
			stream.WriteByte(40);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.allowedContents);
			stream.WriteByte(48);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.maxStackSize);
			stream.WriteByte(56);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.allowedItem);
			if (instance.availableSlots != null)
			{
				for (int i = 0; i < instance.availableSlots.Count; i++)
				{
					int item = instance.availableSlots[i];
					stream.WriteByte(64);
					ProtocolParser.WriteUInt64(stream, (ulong)item);
				}
			}
			if (instance.contents != null)
			{
				for (int j = 0; j < instance.contents.Count; j++)
				{
					Item item1 = instance.contents[j];
					stream.WriteByte(162);
					stream.WriteByte(6);
					memoryStream.SetLength((long)0);
					Item.Serialize(memoryStream, item1);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, ItemContainer instance, ItemContainer previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.UID != previous.UID)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.UID);
			}
			if (instance.slots != previous.slots)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.slots);
			}
			if (instance.temperature != previous.temperature)
			{
				stream.WriteByte(29);
				ProtocolParser.WriteSingle(stream, instance.temperature);
			}
			if (instance.flags != previous.flags)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.flags);
			}
			if (instance.allowedContents != previous.allowedContents)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.allowedContents);
			}
			if (instance.maxStackSize != previous.maxStackSize)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.maxStackSize);
			}
			if (instance.allowedItem != previous.allowedItem)
			{
				stream.WriteByte(56);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.allowedItem);
			}
			if (instance.availableSlots != null)
			{
				for (int i = 0; i < instance.availableSlots.Count; i++)
				{
					int item = instance.availableSlots[i];
					stream.WriteByte(64);
					ProtocolParser.WriteUInt64(stream, (ulong)item);
				}
			}
			if (instance.contents != null)
			{
				for (int j = 0; j < instance.contents.Count; j++)
				{
					Item item1 = instance.contents[j];
					stream.WriteByte(162);
					stream.WriteByte(6);
					memoryStream.SetLength((long)0);
					Item.SerializeDelta(memoryStream, item1, item1);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, ItemContainer instance)
		{
			byte[] bytes = ItemContainer.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(ItemContainer instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ItemContainer.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			ItemContainer.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return ItemContainer.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			ItemContainer.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, ItemContainer previous)
		{
			if (previous == null)
			{
				ItemContainer.Serialize(stream, this);
				return;
			}
			ItemContainer.SerializeDelta(stream, this, previous);
		}
	}
}