using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProtoBuf
{
	public class PersistantPlayer : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public List<int> unlockedItems;

		[NonSerialized]
		public int protocolVersion;

		public bool ShouldPool = true;

		private bool _disposed;

		public PersistantPlayer()
		{
		}

		public PersistantPlayer Copy()
		{
			PersistantPlayer persistantPlayer = Pool.Get<PersistantPlayer>();
			this.CopyTo(persistantPlayer);
			return persistantPlayer;
		}

		public void CopyTo(PersistantPlayer instance)
		{
			throw new NotImplementedException();
		}

		public static PersistantPlayer Deserialize(Stream stream)
		{
			PersistantPlayer persistantPlayer = Pool.Get<PersistantPlayer>();
			PersistantPlayer.Deserialize(stream, persistantPlayer, false);
			return persistantPlayer;
		}

		public static PersistantPlayer Deserialize(byte[] buffer)
		{
			PersistantPlayer persistantPlayer = Pool.Get<PersistantPlayer>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PersistantPlayer.Deserialize(memoryStream, persistantPlayer, false);
			}
			return persistantPlayer;
		}

		public static PersistantPlayer Deserialize(byte[] buffer, PersistantPlayer instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PersistantPlayer.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static PersistantPlayer Deserialize(Stream stream, PersistantPlayer instance, bool isDelta)
		{
			if (!isDelta && instance.unlockedItems == null)
			{
				instance.unlockedItems = Pool.Get<List<int>>();
			}
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num != 24)
				{
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
					else if (key.WireType == Wire.Varint)
					{
						instance.protocolVersion = (int)ProtocolParser.ReadUInt64(stream);
					}
				}
				else
				{
					instance.unlockedItems.Add((int)ProtocolParser.ReadUInt64(stream));
				}
			}
			return instance;
		}

		public static PersistantPlayer DeserializeLength(Stream stream, int length)
		{
			PersistantPlayer persistantPlayer = Pool.Get<PersistantPlayer>();
			PersistantPlayer.DeserializeLength(stream, length, persistantPlayer, false);
			return persistantPlayer;
		}

		public static PersistantPlayer DeserializeLength(Stream stream, int length, PersistantPlayer instance, bool isDelta)
		{
			if (!isDelta && instance.unlockedItems == null)
			{
				instance.unlockedItems = Pool.Get<List<int>>();
			}
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num != 24)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					uint field = key.Field;
					if (field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					if (field == 100)
					{
						if (key.WireType != Wire.Varint)
						{
							continue;
						}
						instance.protocolVersion = (int)ProtocolParser.ReadUInt64(stream);
					}
					else
					{
						ProtocolParser.SkipKey(stream, key);
					}
				}
				else
				{
					instance.unlockedItems.Add((int)ProtocolParser.ReadUInt64(stream));
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static PersistantPlayer DeserializeLengthDelimited(Stream stream)
		{
			PersistantPlayer persistantPlayer = Pool.Get<PersistantPlayer>();
			PersistantPlayer.DeserializeLengthDelimited(stream, persistantPlayer, false);
			return persistantPlayer;
		}

		public static PersistantPlayer DeserializeLengthDelimited(Stream stream, PersistantPlayer instance, bool isDelta)
		{
			if (!isDelta && instance.unlockedItems == null)
			{
				instance.unlockedItems = Pool.Get<List<int>>();
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
				if (num != 24)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					uint field = key.Field;
					if (field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					if (field == 100)
					{
						if (key.WireType != Wire.Varint)
						{
							continue;
						}
						instance.protocolVersion = (int)ProtocolParser.ReadUInt64(stream);
					}
					else
					{
						ProtocolParser.SkipKey(stream, key);
					}
				}
				else
				{
					instance.unlockedItems.Add((int)ProtocolParser.ReadUInt64(stream));
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
			PersistantPlayer.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			PersistantPlayer.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(PersistantPlayer instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.unlockedItems != null)
			{
				List<int> nums = instance.unlockedItems;
				Pool.FreeList<int>(ref nums);
				instance.unlockedItems = nums;
			}
			instance.protocolVersion = 0;
			Pool.Free<PersistantPlayer>(ref instance);
		}

		public void ResetToPool()
		{
			PersistantPlayer.ResetToPool(this);
		}

		public static void Serialize(Stream stream, PersistantPlayer instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.unlockedItems != null)
			{
				for (int i = 0; i < instance.unlockedItems.Count; i++)
				{
					int item = instance.unlockedItems[i];
					stream.WriteByte(24);
					ProtocolParser.WriteUInt64(stream, (ulong)item);
				}
			}
			stream.WriteByte(160);
			stream.WriteByte(6);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.protocolVersion);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, PersistantPlayer instance, PersistantPlayer previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.unlockedItems != null)
			{
				for (int i = 0; i < instance.unlockedItems.Count; i++)
				{
					int item = instance.unlockedItems[i];
					stream.WriteByte(24);
					ProtocolParser.WriteUInt64(stream, (ulong)item);
				}
			}
			if (instance.protocolVersion != previous.protocolVersion)
			{
				stream.WriteByte(160);
				stream.WriteByte(6);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.protocolVersion);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, PersistantPlayer instance)
		{
			byte[] bytes = PersistantPlayer.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(PersistantPlayer instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				PersistantPlayer.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			PersistantPlayer.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return PersistantPlayer.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			PersistantPlayer.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, PersistantPlayer previous)
		{
			if (previous == null)
			{
				PersistantPlayer.Serialize(stream, this);
				return;
			}
			PersistantPlayer.SerializeDelta(stream, this, previous);
		}
	}
}