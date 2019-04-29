using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProtoBuf
{
	public class LootableCorpse : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public LootableCorpse.Private privateData;

		[NonSerialized]
		public ulong playerID;

		[NonSerialized]
		public string playerName;

		public bool ShouldPool = true;

		private bool _disposed;

		public LootableCorpse()
		{
		}

		public LootableCorpse Copy()
		{
			LootableCorpse lootableCorpse = Pool.Get<LootableCorpse>();
			this.CopyTo(lootableCorpse);
			return lootableCorpse;
		}

		public void CopyTo(LootableCorpse instance)
		{
			if (this.privateData == null)
			{
				instance.privateData = null;
			}
			else if (instance.privateData != null)
			{
				this.privateData.CopyTo(instance.privateData);
			}
			else
			{
				instance.privateData = this.privateData.Copy();
			}
			instance.playerID = this.playerID;
			instance.playerName = this.playerName;
		}

		public static LootableCorpse Deserialize(Stream stream)
		{
			LootableCorpse lootableCorpse = Pool.Get<LootableCorpse>();
			LootableCorpse.Deserialize(stream, lootableCorpse, false);
			return lootableCorpse;
		}

		public static LootableCorpse Deserialize(byte[] buffer)
		{
			LootableCorpse lootableCorpse = Pool.Get<LootableCorpse>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				LootableCorpse.Deserialize(memoryStream, lootableCorpse, false);
			}
			return lootableCorpse;
		}

		public static LootableCorpse Deserialize(byte[] buffer, LootableCorpse instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				LootableCorpse.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static LootableCorpse Deserialize(Stream stream, LootableCorpse instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num == 10)
				{
					if (instance.privateData != null)
					{
						LootableCorpse.Private.DeserializeLengthDelimited(stream, instance.privateData, isDelta);
					}
					else
					{
						instance.privateData = LootableCorpse.Private.DeserializeLengthDelimited(stream);
					}
				}
				else if (num == 16)
				{
					instance.playerID = ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 26)
				{
					instance.playerName = ProtocolParser.ReadString(stream);
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

		public static LootableCorpse DeserializeLength(Stream stream, int length)
		{
			LootableCorpse lootableCorpse = Pool.Get<LootableCorpse>();
			LootableCorpse.DeserializeLength(stream, length, lootableCorpse, false);
			return lootableCorpse;
		}

		public static LootableCorpse DeserializeLength(Stream stream, int length, LootableCorpse instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num == 10)
				{
					if (instance.privateData != null)
					{
						LootableCorpse.Private.DeserializeLengthDelimited(stream, instance.privateData, isDelta);
					}
					else
					{
						instance.privateData = LootableCorpse.Private.DeserializeLengthDelimited(stream);
					}
				}
				else if (num == 16)
				{
					instance.playerID = ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 26)
				{
					instance.playerName = ProtocolParser.ReadString(stream);
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

		public static LootableCorpse DeserializeLengthDelimited(Stream stream)
		{
			LootableCorpse lootableCorpse = Pool.Get<LootableCorpse>();
			LootableCorpse.DeserializeLengthDelimited(stream, lootableCorpse, false);
			return lootableCorpse;
		}

		public static LootableCorpse DeserializeLengthDelimited(Stream stream, LootableCorpse instance, bool isDelta)
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
				if (num == 10)
				{
					if (instance.privateData != null)
					{
						LootableCorpse.Private.DeserializeLengthDelimited(stream, instance.privateData, isDelta);
					}
					else
					{
						instance.privateData = LootableCorpse.Private.DeserializeLengthDelimited(stream);
					}
				}
				else if (num == 16)
				{
					instance.playerID = ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 26)
				{
					instance.playerName = ProtocolParser.ReadString(stream);
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
			LootableCorpse.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			LootableCorpse.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(LootableCorpse instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.privateData != null)
			{
				instance.privateData.ResetToPool();
				instance.privateData = null;
			}
			instance.playerID = (ulong)0;
			instance.playerName = string.Empty;
			Pool.Free<LootableCorpse>(ref instance);
		}

		public void ResetToPool()
		{
			LootableCorpse.ResetToPool(this);
		}

		public static void Serialize(Stream stream, LootableCorpse instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.privateData != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				LootableCorpse.Private.Serialize(memoryStream, instance.privateData);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			stream.WriteByte(16);
			ProtocolParser.WriteUInt64(stream, instance.playerID);
			if (instance.playerName != null)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteString(stream, instance.playerName);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, LootableCorpse instance, LootableCorpse previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.privateData != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				LootableCorpse.Private.SerializeDelta(memoryStream, instance.privateData, previous.privateData);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.playerID != previous.playerID)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, instance.playerID);
			}
			if (instance.playerName != null && instance.playerName != previous.playerName)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteString(stream, instance.playerName);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, LootableCorpse instance)
		{
			byte[] bytes = LootableCorpse.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(LootableCorpse instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				LootableCorpse.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			LootableCorpse.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return LootableCorpse.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			LootableCorpse.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, LootableCorpse previous)
		{
			if (previous == null)
			{
				LootableCorpse.Serialize(stream, this);
				return;
			}
			LootableCorpse.SerializeDelta(stream, this, previous);
		}

		public class Private : IDisposable, Pool.IPooled, IProto
		{
			[NonSerialized]
			public List<ItemContainer> container;

			public bool ShouldPool;

			private bool _disposed;

			public Private()
			{
			}

			public LootableCorpse.Private Copy()
			{
				LootableCorpse.Private @private = Pool.Get<LootableCorpse.Private>();
				this.CopyTo(@private);
				return @private;
			}

			public void CopyTo(LootableCorpse.Private instance)
			{
				throw new NotImplementedException();
			}

			public static LootableCorpse.Private Deserialize(Stream stream)
			{
				LootableCorpse.Private @private = Pool.Get<LootableCorpse.Private>();
				LootableCorpse.Private.Deserialize(stream, @private, false);
				return @private;
			}

			public static LootableCorpse.Private Deserialize(byte[] buffer)
			{
				LootableCorpse.Private @private = Pool.Get<LootableCorpse.Private>();
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					LootableCorpse.Private.Deserialize(memoryStream, @private, false);
				}
				return @private;
			}

			public static LootableCorpse.Private Deserialize(byte[] buffer, LootableCorpse.Private instance, bool isDelta = false)
			{
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					LootableCorpse.Private.Deserialize(memoryStream, instance, isDelta);
				}
				return instance;
			}

			public static LootableCorpse.Private Deserialize(Stream stream, LootableCorpse.Private instance, bool isDelta)
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
						instance.container.Add(ItemContainer.DeserializeLengthDelimited(stream));
					}
				}
				return instance;
			}

			public static LootableCorpse.Private DeserializeLength(Stream stream, int length)
			{
				LootableCorpse.Private @private = Pool.Get<LootableCorpse.Private>();
				LootableCorpse.Private.DeserializeLength(stream, length, @private, false);
				return @private;
			}

			public static LootableCorpse.Private DeserializeLength(Stream stream, int length, LootableCorpse.Private instance, bool isDelta)
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
						instance.container.Add(ItemContainer.DeserializeLengthDelimited(stream));
					}
				}
				if (stream.Position != position)
				{
					throw new ProtocolBufferException("Read past max limit");
				}
				return instance;
			}

			public static LootableCorpse.Private DeserializeLengthDelimited(Stream stream)
			{
				LootableCorpse.Private @private = Pool.Get<LootableCorpse.Private>();
				LootableCorpse.Private.DeserializeLengthDelimited(stream, @private, false);
				return @private;
			}

			public static LootableCorpse.Private DeserializeLengthDelimited(Stream stream, LootableCorpse.Private instance, bool isDelta)
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
						instance.container.Add(ItemContainer.DeserializeLengthDelimited(stream));
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
				LootableCorpse.Private.Deserialize(stream, this, isDelta);
			}

			public virtual void LeavePool()
			{
				this._disposed = false;
			}

			public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
			{
				LootableCorpse.Private.DeserializeLength(stream, size, this, isDelta);
			}

			public static void ResetToPool(LootableCorpse.Private instance)
			{
				if (!instance.ShouldPool)
				{
					return;
				}
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
				Pool.Free<LootableCorpse.Private>(ref instance);
			}

			public void ResetToPool()
			{
				LootableCorpse.Private.ResetToPool(this);
			}

			public static void Serialize(Stream stream, LootableCorpse.Private instance)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.container != null)
				{
					for (int i = 0; i < instance.container.Count; i++)
					{
						ItemContainer item = instance.container[i];
						stream.WriteByte(10);
						memoryStream.SetLength((long)0);
						ItemContainer.Serialize(memoryStream, item);
						uint length = (uint)memoryStream.Length;
						ProtocolParser.WriteUInt32(stream, length);
						stream.Write(memoryStream.GetBuffer(), 0, (int)length);
					}
				}
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeDelta(Stream stream, LootableCorpse.Private instance, LootableCorpse.Private previous)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.container != null)
				{
					for (int i = 0; i < instance.container.Count; i++)
					{
						ItemContainer item = instance.container[i];
						stream.WriteByte(10);
						memoryStream.SetLength((long)0);
						ItemContainer.SerializeDelta(memoryStream, item, item);
						uint length = (uint)memoryStream.Length;
						ProtocolParser.WriteUInt32(stream, length);
						stream.Write(memoryStream.GetBuffer(), 0, (int)length);
					}
				}
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeLengthDelimited(Stream stream, LootableCorpse.Private instance)
			{
				byte[] bytes = LootableCorpse.Private.SerializeToBytes(instance);
				ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
				stream.Write(bytes, 0, (int)bytes.Length);
			}

			public static byte[] SerializeToBytes(LootableCorpse.Private instance)
			{
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					LootableCorpse.Private.Serialize(memoryStream, instance);
					array = memoryStream.ToArray();
				}
				return array;
			}

			public void ToProto(Stream stream)
			{
				LootableCorpse.Private.Serialize(stream, this);
			}

			public byte[] ToProtoBytes()
			{
				return LootableCorpse.Private.SerializeToBytes(this);
			}

			public virtual void WriteToStream(Stream stream)
			{
				LootableCorpse.Private.Serialize(stream, this);
			}

			public virtual void WriteToStreamDelta(Stream stream, LootableCorpse.Private previous)
			{
				if (previous == null)
				{
					LootableCorpse.Private.Serialize(stream, this);
					return;
				}
				LootableCorpse.Private.SerializeDelta(stream, this, previous);
			}
		}
	}
}