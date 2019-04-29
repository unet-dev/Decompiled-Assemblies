using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProtoBuf
{
	public class PlayerUpdateLoot : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public uint itemID;

		[NonSerialized]
		public uint entityID;

		[NonSerialized]
		public List<ItemContainer> containers;

		public bool ShouldPool = true;

		private bool _disposed;

		public PlayerUpdateLoot()
		{
		}

		public PlayerUpdateLoot Copy()
		{
			PlayerUpdateLoot playerUpdateLoot = Pool.Get<PlayerUpdateLoot>();
			this.CopyTo(playerUpdateLoot);
			return playerUpdateLoot;
		}

		public void CopyTo(PlayerUpdateLoot instance)
		{
			instance.itemID = this.itemID;
			instance.entityID = this.entityID;
			throw new NotImplementedException();
		}

		public static PlayerUpdateLoot Deserialize(Stream stream)
		{
			PlayerUpdateLoot playerUpdateLoot = Pool.Get<PlayerUpdateLoot>();
			PlayerUpdateLoot.Deserialize(stream, playerUpdateLoot, false);
			return playerUpdateLoot;
		}

		public static PlayerUpdateLoot Deserialize(byte[] buffer)
		{
			PlayerUpdateLoot playerUpdateLoot = Pool.Get<PlayerUpdateLoot>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerUpdateLoot.Deserialize(memoryStream, playerUpdateLoot, false);
			}
			return playerUpdateLoot;
		}

		public static PlayerUpdateLoot Deserialize(byte[] buffer, PlayerUpdateLoot instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerUpdateLoot.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static PlayerUpdateLoot Deserialize(Stream stream, PlayerUpdateLoot instance, bool isDelta)
		{
			if (!isDelta && instance.containers == null)
			{
				instance.containers = Pool.Get<List<ItemContainer>>();
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
					instance.itemID = ProtocolParser.ReadUInt32(stream);
				}
				else if (num == 16)
				{
					instance.entityID = ProtocolParser.ReadUInt32(stream);
				}
				else if (num == 26)
				{
					instance.containers.Add(ItemContainer.DeserializeLengthDelimited(stream));
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

		public static PlayerUpdateLoot DeserializeLength(Stream stream, int length)
		{
			PlayerUpdateLoot playerUpdateLoot = Pool.Get<PlayerUpdateLoot>();
			PlayerUpdateLoot.DeserializeLength(stream, length, playerUpdateLoot, false);
			return playerUpdateLoot;
		}

		public static PlayerUpdateLoot DeserializeLength(Stream stream, int length, PlayerUpdateLoot instance, bool isDelta)
		{
			if (!isDelta && instance.containers == null)
			{
				instance.containers = Pool.Get<List<ItemContainer>>();
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
					instance.itemID = ProtocolParser.ReadUInt32(stream);
				}
				else if (num == 16)
				{
					instance.entityID = ProtocolParser.ReadUInt32(stream);
				}
				else if (num == 26)
				{
					instance.containers.Add(ItemContainer.DeserializeLengthDelimited(stream));
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

		public static PlayerUpdateLoot DeserializeLengthDelimited(Stream stream)
		{
			PlayerUpdateLoot playerUpdateLoot = Pool.Get<PlayerUpdateLoot>();
			PlayerUpdateLoot.DeserializeLengthDelimited(stream, playerUpdateLoot, false);
			return playerUpdateLoot;
		}

		public static PlayerUpdateLoot DeserializeLengthDelimited(Stream stream, PlayerUpdateLoot instance, bool isDelta)
		{
			if (!isDelta && instance.containers == null)
			{
				instance.containers = Pool.Get<List<ItemContainer>>();
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
					instance.itemID = ProtocolParser.ReadUInt32(stream);
				}
				else if (num == 16)
				{
					instance.entityID = ProtocolParser.ReadUInt32(stream);
				}
				else if (num == 26)
				{
					instance.containers.Add(ItemContainer.DeserializeLengthDelimited(stream));
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
			PlayerUpdateLoot.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			PlayerUpdateLoot.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(PlayerUpdateLoot instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.itemID = 0;
			instance.entityID = 0;
			if (instance.containers != null)
			{
				for (int i = 0; i < instance.containers.Count; i++)
				{
					if (instance.containers[i] != null)
					{
						instance.containers[i].ResetToPool();
						instance.containers[i] = null;
					}
				}
				List<ItemContainer> itemContainers = instance.containers;
				Pool.FreeList<ItemContainer>(ref itemContainers);
				instance.containers = itemContainers;
			}
			Pool.Free<PlayerUpdateLoot>(ref instance);
		}

		public void ResetToPool()
		{
			PlayerUpdateLoot.ResetToPool(this);
		}

		public static void Serialize(Stream stream, PlayerUpdateLoot instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.itemID);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt32(stream, instance.entityID);
			if (instance.containers != null)
			{
				for (int i = 0; i < instance.containers.Count; i++)
				{
					ItemContainer item = instance.containers[i];
					stream.WriteByte(26);
					memoryStream.SetLength((long)0);
					ItemContainer.Serialize(memoryStream, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, PlayerUpdateLoot instance, PlayerUpdateLoot previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.itemID != previous.itemID)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.itemID);
			}
			if (instance.entityID != previous.entityID)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.entityID);
			}
			if (instance.containers != null)
			{
				for (int i = 0; i < instance.containers.Count; i++)
				{
					ItemContainer item = instance.containers[i];
					stream.WriteByte(26);
					memoryStream.SetLength((long)0);
					ItemContainer.SerializeDelta(memoryStream, item, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, PlayerUpdateLoot instance)
		{
			byte[] bytes = PlayerUpdateLoot.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(PlayerUpdateLoot instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				PlayerUpdateLoot.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			PlayerUpdateLoot.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return PlayerUpdateLoot.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			PlayerUpdateLoot.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, PlayerUpdateLoot previous)
		{
			if (previous == null)
			{
				PlayerUpdateLoot.Serialize(stream, this);
				return;
			}
			PlayerUpdateLoot.SerializeDelta(stream, this, previous);
		}
	}
}