using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class Item : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public uint UID;

		[NonSerialized]
		public int itemid;

		[NonSerialized]
		public int slot;

		[NonSerialized]
		public int amount;

		[NonSerialized]
		public int flags;

		[NonSerialized]
		public float removetime;

		[NonSerialized]
		public float locktime;

		[NonSerialized]
		public uint worldEntity;

		[NonSerialized]
		public Item.InstanceData instanceData;

		[NonSerialized]
		public uint heldEntity;

		[NonSerialized]
		public Item.ConditionData conditionData;

		[NonSerialized]
		public string name;

		[NonSerialized]
		public string text;

		[NonSerialized]
		public ulong skinid;

		[NonSerialized]
		public ItemContainer contents;

		public bool ShouldPool = true;

		private bool _disposed;

		public Item()
		{
		}

		public Item Copy()
		{
			Item item = Pool.Get<Item>();
			this.CopyTo(item);
			return item;
		}

		public void CopyTo(Item instance)
		{
			instance.UID = this.UID;
			instance.itemid = this.itemid;
			instance.slot = this.slot;
			instance.amount = this.amount;
			instance.flags = this.flags;
			instance.removetime = this.removetime;
			instance.locktime = this.locktime;
			instance.worldEntity = this.worldEntity;
			if (this.instanceData == null)
			{
				instance.instanceData = null;
			}
			else if (instance.instanceData != null)
			{
				this.instanceData.CopyTo(instance.instanceData);
			}
			else
			{
				instance.instanceData = this.instanceData.Copy();
			}
			instance.heldEntity = this.heldEntity;
			if (this.conditionData == null)
			{
				instance.conditionData = null;
			}
			else if (instance.conditionData != null)
			{
				this.conditionData.CopyTo(instance.conditionData);
			}
			else
			{
				instance.conditionData = this.conditionData.Copy();
			}
			instance.name = this.name;
			instance.text = this.text;
			instance.skinid = this.skinid;
			if (this.contents == null)
			{
				instance.contents = null;
				return;
			}
			if (instance.contents == null)
			{
				instance.contents = this.contents.Copy();
				return;
			}
			this.contents.CopyTo(instance.contents);
		}

		public static Item Deserialize(Stream stream)
		{
			Item item = Pool.Get<Item>();
			Item.Deserialize(stream, item, false);
			return item;
		}

		public static Item Deserialize(byte[] buffer)
		{
			Item item = Pool.Get<Item>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Item.Deserialize(memoryStream, item, false);
			}
			return item;
		}

		public static Item Deserialize(byte[] buffer, Item instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Item.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static Item Deserialize(Stream stream, Item instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 53)
				{
					if (num <= 24)
					{
						if (num == 8)
						{
							instance.UID = ProtocolParser.ReadUInt32(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.itemid = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 24)
						{
							instance.slot = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 32)
					{
						instance.amount = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 40)
					{
						instance.flags = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 53)
					{
						instance.removetime = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num <= 74)
				{
					if (num == 61)
					{
						instance.locktime = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 64)
					{
						instance.worldEntity = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 74)
					{
						if (instance.instanceData != null)
						{
							Item.InstanceData.DeserializeLengthDelimited(stream, instance.instanceData, isDelta);
							continue;
						}
						else
						{
							instance.instanceData = Item.InstanceData.DeserializeLengthDelimited(stream);
							continue;
						}
					}
				}
				else if (num <= 90)
				{
					if (num == 80)
					{
						instance.heldEntity = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 90)
					{
						if (instance.conditionData != null)
						{
							Item.ConditionData.DeserializeLengthDelimited(stream, instance.conditionData, isDelta);
							continue;
						}
						else
						{
							instance.conditionData = Item.ConditionData.DeserializeLengthDelimited(stream);
							continue;
						}
					}
				}
				else if (num == 114)
				{
					instance.name = ProtocolParser.ReadString(stream);
					continue;
				}
				else if (num == 122)
				{
					instance.text = ProtocolParser.ReadString(stream);
					continue;
				}
				Key key = ProtocolParser.ReadKey((byte)num, stream);
				uint field = key.Field;
				if (field == 0)
				{
					throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
				}
				if (field == 16)
				{
					if (key.WireType == Wire.Varint)
					{
						instance.skinid = ProtocolParser.ReadUInt64(stream);
					}
				}
				else if (field != 100)
				{
					ProtocolParser.SkipKey(stream, key);
				}
				else if (key.WireType == Wire.LengthDelimited)
				{
					if (instance.contents != null)
					{
						ItemContainer.DeserializeLengthDelimited(stream, instance.contents, isDelta);
					}
					else
					{
						instance.contents = ItemContainer.DeserializeLengthDelimited(stream);
					}
				}
			}
			return instance;
		}

		public static Item DeserializeLength(Stream stream, int length)
		{
			Item item = Pool.Get<Item>();
			Item.DeserializeLength(stream, length, item, false);
			return item;
		}

		public static Item DeserializeLength(Stream stream, int length, Item instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num <= 53)
				{
					if (num <= 24)
					{
						if (num == 8)
						{
							instance.UID = ProtocolParser.ReadUInt32(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.itemid = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 24)
						{
							instance.slot = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 32)
					{
						instance.amount = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 40)
					{
						instance.flags = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 53)
					{
						instance.removetime = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num <= 74)
				{
					if (num == 61)
					{
						instance.locktime = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 64)
					{
						instance.worldEntity = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 74)
					{
						if (instance.instanceData != null)
						{
							Item.InstanceData.DeserializeLengthDelimited(stream, instance.instanceData, isDelta);
							continue;
						}
						else
						{
							instance.instanceData = Item.InstanceData.DeserializeLengthDelimited(stream);
							continue;
						}
					}
				}
				else if (num <= 90)
				{
					if (num == 80)
					{
						instance.heldEntity = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 90)
					{
						if (instance.conditionData != null)
						{
							Item.ConditionData.DeserializeLengthDelimited(stream, instance.conditionData, isDelta);
							continue;
						}
						else
						{
							instance.conditionData = Item.ConditionData.DeserializeLengthDelimited(stream);
							continue;
						}
					}
				}
				else if (num == 114)
				{
					instance.name = ProtocolParser.ReadString(stream);
					continue;
				}
				else if (num == 122)
				{
					instance.text = ProtocolParser.ReadString(stream);
					continue;
				}
				Key key = ProtocolParser.ReadKey((byte)num, stream);
				uint field = key.Field;
				if (field == 0)
				{
					throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
				}
				if (field == 16)
				{
					if (key.WireType != Wire.Varint)
					{
						continue;
					}
					instance.skinid = ProtocolParser.ReadUInt64(stream);
				}
				else if (field == 100)
				{
					if (key.WireType != Wire.LengthDelimited)
					{
						continue;
					}
					if (instance.contents != null)
					{
						ItemContainer.DeserializeLengthDelimited(stream, instance.contents, isDelta);
					}
					else
					{
						instance.contents = ItemContainer.DeserializeLengthDelimited(stream);
					}
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

		public static Item DeserializeLengthDelimited(Stream stream)
		{
			Item item = Pool.Get<Item>();
			Item.DeserializeLengthDelimited(stream, item, false);
			return item;
		}

		public static Item DeserializeLengthDelimited(Stream stream, Item instance, bool isDelta)
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
				if (num <= 53)
				{
					if (num <= 24)
					{
						if (num == 8)
						{
							instance.UID = ProtocolParser.ReadUInt32(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.itemid = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 24)
						{
							instance.slot = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 32)
					{
						instance.amount = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 40)
					{
						instance.flags = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 53)
					{
						instance.removetime = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num <= 74)
				{
					if (num == 61)
					{
						instance.locktime = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 64)
					{
						instance.worldEntity = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 74)
					{
						if (instance.instanceData != null)
						{
							Item.InstanceData.DeserializeLengthDelimited(stream, instance.instanceData, isDelta);
							continue;
						}
						else
						{
							instance.instanceData = Item.InstanceData.DeserializeLengthDelimited(stream);
							continue;
						}
					}
				}
				else if (num <= 90)
				{
					if (num == 80)
					{
						instance.heldEntity = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 90)
					{
						if (instance.conditionData != null)
						{
							Item.ConditionData.DeserializeLengthDelimited(stream, instance.conditionData, isDelta);
							continue;
						}
						else
						{
							instance.conditionData = Item.ConditionData.DeserializeLengthDelimited(stream);
							continue;
						}
					}
				}
				else if (num == 114)
				{
					instance.name = ProtocolParser.ReadString(stream);
					continue;
				}
				else if (num == 122)
				{
					instance.text = ProtocolParser.ReadString(stream);
					continue;
				}
				Key key = ProtocolParser.ReadKey((byte)num, stream);
				uint field = key.Field;
				if (field == 0)
				{
					throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
				}
				if (field == 16)
				{
					if (key.WireType != Wire.Varint)
					{
						continue;
					}
					instance.skinid = ProtocolParser.ReadUInt64(stream);
				}
				else if (field == 100)
				{
					if (key.WireType != Wire.LengthDelimited)
					{
						continue;
					}
					if (instance.contents != null)
					{
						ItemContainer.DeserializeLengthDelimited(stream, instance.contents, isDelta);
					}
					else
					{
						instance.contents = ItemContainer.DeserializeLengthDelimited(stream);
					}
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
			Item.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			Item.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(Item instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.UID = 0;
			instance.itemid = 0;
			instance.slot = 0;
			instance.amount = 0;
			instance.flags = 0;
			instance.removetime = 0f;
			instance.locktime = 0f;
			instance.worldEntity = 0;
			if (instance.instanceData != null)
			{
				instance.instanceData.ResetToPool();
				instance.instanceData = null;
			}
			instance.heldEntity = 0;
			if (instance.conditionData != null)
			{
				instance.conditionData.ResetToPool();
				instance.conditionData = null;
			}
			instance.name = string.Empty;
			instance.text = string.Empty;
			instance.skinid = (ulong)0;
			if (instance.contents != null)
			{
				instance.contents.ResetToPool();
				instance.contents = null;
			}
			Pool.Free<Item>(ref instance);
		}

		public void ResetToPool()
		{
			Item.ResetToPool(this);
		}

		public static void Serialize(Stream stream, Item instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.UID);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.itemid);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.slot);
			stream.WriteByte(32);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.amount);
			stream.WriteByte(40);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.flags);
			stream.WriteByte(53);
			ProtocolParser.WriteSingle(stream, instance.removetime);
			stream.WriteByte(61);
			ProtocolParser.WriteSingle(stream, instance.locktime);
			stream.WriteByte(64);
			ProtocolParser.WriteUInt32(stream, instance.worldEntity);
			if (instance.instanceData != null)
			{
				stream.WriteByte(74);
				memoryStream.SetLength((long)0);
				Item.InstanceData.Serialize(memoryStream, instance.instanceData);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			stream.WriteByte(80);
			ProtocolParser.WriteUInt32(stream, instance.heldEntity);
			if (instance.conditionData != null)
			{
				stream.WriteByte(90);
				memoryStream.SetLength((long)0);
				Item.ConditionData.Serialize(memoryStream, instance.conditionData);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			if (instance.name != null)
			{
				stream.WriteByte(114);
				ProtocolParser.WriteString(stream, instance.name);
			}
			if (instance.text != null)
			{
				stream.WriteByte(122);
				ProtocolParser.WriteString(stream, instance.text);
			}
			stream.WriteByte(128);
			stream.WriteByte(1);
			ProtocolParser.WriteUInt64(stream, instance.skinid);
			if (instance.contents != null)
			{
				stream.WriteByte(162);
				stream.WriteByte(6);
				memoryStream.SetLength((long)0);
				ItemContainer.Serialize(memoryStream, instance.contents);
				uint length1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, Item instance, Item previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.UID != previous.UID)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.UID);
			}
			if (instance.itemid != previous.itemid)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.itemid);
			}
			if (instance.slot != previous.slot)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.slot);
			}
			if (instance.amount != previous.amount)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.amount);
			}
			if (instance.flags != previous.flags)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.flags);
			}
			if (instance.removetime != previous.removetime)
			{
				stream.WriteByte(53);
				ProtocolParser.WriteSingle(stream, instance.removetime);
			}
			if (instance.locktime != previous.locktime)
			{
				stream.WriteByte(61);
				ProtocolParser.WriteSingle(stream, instance.locktime);
			}
			if (instance.worldEntity != previous.worldEntity)
			{
				stream.WriteByte(64);
				ProtocolParser.WriteUInt32(stream, instance.worldEntity);
			}
			if (instance.instanceData != null)
			{
				stream.WriteByte(74);
				memoryStream.SetLength((long)0);
				Item.InstanceData.SerializeDelta(memoryStream, instance.instanceData, previous.instanceData);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.heldEntity != previous.heldEntity)
			{
				stream.WriteByte(80);
				ProtocolParser.WriteUInt32(stream, instance.heldEntity);
			}
			if (instance.conditionData != null)
			{
				stream.WriteByte(90);
				memoryStream.SetLength((long)0);
				Item.ConditionData.SerializeDelta(memoryStream, instance.conditionData, previous.conditionData);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			if (instance.name != null && instance.name != previous.name)
			{
				stream.WriteByte(114);
				ProtocolParser.WriteString(stream, instance.name);
			}
			if (instance.text != null && instance.text != previous.text)
			{
				stream.WriteByte(122);
				ProtocolParser.WriteString(stream, instance.text);
			}
			if (instance.skinid != previous.skinid)
			{
				stream.WriteByte(128);
				stream.WriteByte(1);
				ProtocolParser.WriteUInt64(stream, instance.skinid);
			}
			if (instance.contents != null)
			{
				stream.WriteByte(162);
				stream.WriteByte(6);
				memoryStream.SetLength((long)0);
				ItemContainer.SerializeDelta(memoryStream, instance.contents, previous.contents);
				uint length1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, Item instance)
		{
			byte[] bytes = Item.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(Item instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Item.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			Item.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return Item.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			Item.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, Item previous)
		{
			if (previous == null)
			{
				Item.Serialize(stream, this);
				return;
			}
			Item.SerializeDelta(stream, this, previous);
		}

		public class ConditionData : IDisposable, Pool.IPooled, IProto
		{
			[NonSerialized]
			public float condition;

			[NonSerialized]
			public float maxCondition;

			public bool ShouldPool;

			private bool _disposed;

			public ConditionData()
			{
			}

			public Item.ConditionData Copy()
			{
				Item.ConditionData conditionDatum = Pool.Get<Item.ConditionData>();
				this.CopyTo(conditionDatum);
				return conditionDatum;
			}

			public void CopyTo(Item.ConditionData instance)
			{
				instance.condition = this.condition;
				instance.maxCondition = this.maxCondition;
			}

			public static Item.ConditionData Deserialize(Stream stream)
			{
				Item.ConditionData conditionDatum = Pool.Get<Item.ConditionData>();
				Item.ConditionData.Deserialize(stream, conditionDatum, false);
				return conditionDatum;
			}

			public static Item.ConditionData Deserialize(byte[] buffer)
			{
				Item.ConditionData conditionDatum = Pool.Get<Item.ConditionData>();
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					Item.ConditionData.Deserialize(memoryStream, conditionDatum, false);
				}
				return conditionDatum;
			}

			public static Item.ConditionData Deserialize(byte[] buffer, Item.ConditionData instance, bool isDelta = false)
			{
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					Item.ConditionData.Deserialize(memoryStream, instance, isDelta);
				}
				return instance;
			}

			public static Item.ConditionData Deserialize(Stream stream, Item.ConditionData instance, bool isDelta)
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
						instance.condition = ProtocolParser.ReadSingle(stream);
					}
					else if (num == 21)
					{
						instance.maxCondition = ProtocolParser.ReadSingle(stream);
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

			public static Item.ConditionData DeserializeLength(Stream stream, int length)
			{
				Item.ConditionData conditionDatum = Pool.Get<Item.ConditionData>();
				Item.ConditionData.DeserializeLength(stream, length, conditionDatum, false);
				return conditionDatum;
			}

			public static Item.ConditionData DeserializeLength(Stream stream, int length, Item.ConditionData instance, bool isDelta)
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
						instance.condition = ProtocolParser.ReadSingle(stream);
					}
					else if (num == 21)
					{
						instance.maxCondition = ProtocolParser.ReadSingle(stream);
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

			public static Item.ConditionData DeserializeLengthDelimited(Stream stream)
			{
				Item.ConditionData conditionDatum = Pool.Get<Item.ConditionData>();
				Item.ConditionData.DeserializeLengthDelimited(stream, conditionDatum, false);
				return conditionDatum;
			}

			public static Item.ConditionData DeserializeLengthDelimited(Stream stream, Item.ConditionData instance, bool isDelta)
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
						instance.condition = ProtocolParser.ReadSingle(stream);
					}
					else if (num == 21)
					{
						instance.maxCondition = ProtocolParser.ReadSingle(stream);
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
				Item.ConditionData.Deserialize(stream, this, isDelta);
			}

			public virtual void LeavePool()
			{
				this._disposed = false;
			}

			public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
			{
				Item.ConditionData.DeserializeLength(stream, size, this, isDelta);
			}

			public static void ResetToPool(Item.ConditionData instance)
			{
				if (!instance.ShouldPool)
				{
					return;
				}
				instance.condition = 0f;
				instance.maxCondition = 0f;
				Pool.Free<Item.ConditionData>(ref instance);
			}

			public void ResetToPool()
			{
				Item.ConditionData.ResetToPool(this);
			}

			public static void Serialize(Stream stream, Item.ConditionData instance)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				stream.WriteByte(13);
				ProtocolParser.WriteSingle(stream, instance.condition);
				stream.WriteByte(21);
				ProtocolParser.WriteSingle(stream, instance.maxCondition);
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeDelta(Stream stream, Item.ConditionData instance, Item.ConditionData previous)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.condition != previous.condition)
				{
					stream.WriteByte(13);
					ProtocolParser.WriteSingle(stream, instance.condition);
				}
				if (instance.maxCondition != previous.maxCondition)
				{
					stream.WriteByte(21);
					ProtocolParser.WriteSingle(stream, instance.maxCondition);
				}
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeLengthDelimited(Stream stream, Item.ConditionData instance)
			{
				byte[] bytes = Item.ConditionData.SerializeToBytes(instance);
				ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
				stream.Write(bytes, 0, (int)bytes.Length);
			}

			public static byte[] SerializeToBytes(Item.ConditionData instance)
			{
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					Item.ConditionData.Serialize(memoryStream, instance);
					array = memoryStream.ToArray();
				}
				return array;
			}

			public void ToProto(Stream stream)
			{
				Item.ConditionData.Serialize(stream, this);
			}

			public byte[] ToProtoBytes()
			{
				return Item.ConditionData.SerializeToBytes(this);
			}

			public virtual void WriteToStream(Stream stream)
			{
				Item.ConditionData.Serialize(stream, this);
			}

			public virtual void WriteToStreamDelta(Stream stream, Item.ConditionData previous)
			{
				if (previous == null)
				{
					Item.ConditionData.Serialize(stream, this);
					return;
				}
				Item.ConditionData.SerializeDelta(stream, this, previous);
			}
		}

		public class InstanceData : IDisposable, Pool.IPooled, IProto
		{
			[NonSerialized]
			public int dataInt;

			[NonSerialized]
			public int blueprintTarget;

			[NonSerialized]
			public int blueprintAmount;

			[NonSerialized]
			public uint subEntity;

			public bool ShouldPool;

			private bool _disposed;

			public InstanceData()
			{
			}

			public Item.InstanceData Copy()
			{
				Item.InstanceData instanceDatum = Pool.Get<Item.InstanceData>();
				this.CopyTo(instanceDatum);
				return instanceDatum;
			}

			public void CopyTo(Item.InstanceData instance)
			{
				instance.dataInt = this.dataInt;
				instance.blueprintTarget = this.blueprintTarget;
				instance.blueprintAmount = this.blueprintAmount;
				instance.subEntity = this.subEntity;
			}

			public static Item.InstanceData Deserialize(Stream stream)
			{
				Item.InstanceData instanceDatum = Pool.Get<Item.InstanceData>();
				Item.InstanceData.Deserialize(stream, instanceDatum, false);
				return instanceDatum;
			}

			public static Item.InstanceData Deserialize(byte[] buffer)
			{
				Item.InstanceData instanceDatum = Pool.Get<Item.InstanceData>();
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					Item.InstanceData.Deserialize(memoryStream, instanceDatum, false);
				}
				return instanceDatum;
			}

			public static Item.InstanceData Deserialize(byte[] buffer, Item.InstanceData instance, bool isDelta = false)
			{
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					Item.InstanceData.Deserialize(memoryStream, instance, isDelta);
				}
				return instance;
			}

			public static Item.InstanceData Deserialize(Stream stream, Item.InstanceData instance, bool isDelta)
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
							instance.dataInt = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.blueprintTarget = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 24)
					{
						instance.blueprintAmount = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.subEntity = ProtocolParser.ReadUInt32(stream);
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

			public static Item.InstanceData DeserializeLength(Stream stream, int length)
			{
				Item.InstanceData instanceDatum = Pool.Get<Item.InstanceData>();
				Item.InstanceData.DeserializeLength(stream, length, instanceDatum, false);
				return instanceDatum;
			}

			public static Item.InstanceData DeserializeLength(Stream stream, int length, Item.InstanceData instance, bool isDelta)
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
							instance.dataInt = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.blueprintTarget = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 24)
					{
						instance.blueprintAmount = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.subEntity = ProtocolParser.ReadUInt32(stream);
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

			public static Item.InstanceData DeserializeLengthDelimited(Stream stream)
			{
				Item.InstanceData instanceDatum = Pool.Get<Item.InstanceData>();
				Item.InstanceData.DeserializeLengthDelimited(stream, instanceDatum, false);
				return instanceDatum;
			}

			public static Item.InstanceData DeserializeLengthDelimited(Stream stream, Item.InstanceData instance, bool isDelta)
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
							instance.dataInt = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.blueprintTarget = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 24)
					{
						instance.blueprintAmount = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.subEntity = ProtocolParser.ReadUInt32(stream);
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
				Item.InstanceData.Deserialize(stream, this, isDelta);
			}

			public virtual void LeavePool()
			{
				this._disposed = false;
			}

			public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
			{
				Item.InstanceData.DeserializeLength(stream, size, this, isDelta);
			}

			public static void ResetToPool(Item.InstanceData instance)
			{
				if (!instance.ShouldPool)
				{
					return;
				}
				instance.dataInt = 0;
				instance.blueprintTarget = 0;
				instance.blueprintAmount = 0;
				instance.subEntity = 0;
				Pool.Free<Item.InstanceData>(ref instance);
			}

			public void ResetToPool()
			{
				Item.InstanceData.ResetToPool(this);
			}

			public static void Serialize(Stream stream, Item.InstanceData instance)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.dataInt);
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.blueprintTarget);
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.blueprintAmount);
				stream.WriteByte(32);
				ProtocolParser.WriteUInt32(stream, instance.subEntity);
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeDelta(Stream stream, Item.InstanceData instance, Item.InstanceData previous)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.dataInt != previous.dataInt)
				{
					stream.WriteByte(8);
					ProtocolParser.WriteUInt64(stream, (ulong)instance.dataInt);
				}
				if (instance.blueprintTarget != previous.blueprintTarget)
				{
					stream.WriteByte(16);
					ProtocolParser.WriteUInt64(stream, (ulong)instance.blueprintTarget);
				}
				if (instance.blueprintAmount != previous.blueprintAmount)
				{
					stream.WriteByte(24);
					ProtocolParser.WriteUInt64(stream, (ulong)instance.blueprintAmount);
				}
				if (instance.subEntity != previous.subEntity)
				{
					stream.WriteByte(32);
					ProtocolParser.WriteUInt32(stream, instance.subEntity);
				}
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeLengthDelimited(Stream stream, Item.InstanceData instance)
			{
				byte[] bytes = Item.InstanceData.SerializeToBytes(instance);
				ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
				stream.Write(bytes, 0, (int)bytes.Length);
			}

			public static byte[] SerializeToBytes(Item.InstanceData instance)
			{
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					Item.InstanceData.Serialize(memoryStream, instance);
					array = memoryStream.ToArray();
				}
				return array;
			}

			public void ToProto(Stream stream)
			{
				Item.InstanceData.Serialize(stream, this);
			}

			public byte[] ToProtoBytes()
			{
				return Item.InstanceData.SerializeToBytes(this);
			}

			public virtual void WriteToStream(Stream stream)
			{
				Item.InstanceData.Serialize(stream, this);
			}

			public virtual void WriteToStreamDelta(Stream stream, Item.InstanceData previous)
			{
				if (previous == null)
				{
					Item.InstanceData.Serialize(stream, this);
					return;
				}
				Item.InstanceData.SerializeDelta(stream, this, previous);
			}
		}
	}
}