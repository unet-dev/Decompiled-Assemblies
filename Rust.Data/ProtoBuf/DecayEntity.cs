using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class DecayEntity : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public float decayTimer;

		[NonSerialized]
		public uint buildingID;

		[NonSerialized]
		public float upkeepTimer;

		public bool ShouldPool = true;

		private bool _disposed;

		public DecayEntity()
		{
		}

		public DecayEntity Copy()
		{
			DecayEntity decayEntity = Pool.Get<DecayEntity>();
			this.CopyTo(decayEntity);
			return decayEntity;
		}

		public void CopyTo(DecayEntity instance)
		{
			instance.decayTimer = this.decayTimer;
			instance.buildingID = this.buildingID;
			instance.upkeepTimer = this.upkeepTimer;
		}

		public static DecayEntity Deserialize(Stream stream)
		{
			DecayEntity decayEntity = Pool.Get<DecayEntity>();
			DecayEntity.Deserialize(stream, decayEntity, false);
			return decayEntity;
		}

		public static DecayEntity Deserialize(byte[] buffer)
		{
			DecayEntity decayEntity = Pool.Get<DecayEntity>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				DecayEntity.Deserialize(memoryStream, decayEntity, false);
			}
			return decayEntity;
		}

		public static DecayEntity Deserialize(byte[] buffer, DecayEntity instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				DecayEntity.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static DecayEntity Deserialize(Stream stream, DecayEntity instance, bool isDelta)
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
					instance.decayTimer = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 16)
				{
					instance.buildingID = ProtocolParser.ReadUInt32(stream);
				}
				else if (num == 29)
				{
					instance.upkeepTimer = ProtocolParser.ReadSingle(stream);
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

		public static DecayEntity DeserializeLength(Stream stream, int length)
		{
			DecayEntity decayEntity = Pool.Get<DecayEntity>();
			DecayEntity.DeserializeLength(stream, length, decayEntity, false);
			return decayEntity;
		}

		public static DecayEntity DeserializeLength(Stream stream, int length, DecayEntity instance, bool isDelta)
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
					instance.decayTimer = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 16)
				{
					instance.buildingID = ProtocolParser.ReadUInt32(stream);
				}
				else if (num == 29)
				{
					instance.upkeepTimer = ProtocolParser.ReadSingle(stream);
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

		public static DecayEntity DeserializeLengthDelimited(Stream stream)
		{
			DecayEntity decayEntity = Pool.Get<DecayEntity>();
			DecayEntity.DeserializeLengthDelimited(stream, decayEntity, false);
			return decayEntity;
		}

		public static DecayEntity DeserializeLengthDelimited(Stream stream, DecayEntity instance, bool isDelta)
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
					instance.decayTimer = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 16)
				{
					instance.buildingID = ProtocolParser.ReadUInt32(stream);
				}
				else if (num == 29)
				{
					instance.upkeepTimer = ProtocolParser.ReadSingle(stream);
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
			DecayEntity.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			DecayEntity.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(DecayEntity instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.decayTimer = 0f;
			instance.buildingID = 0;
			instance.upkeepTimer = 0f;
			Pool.Free<DecayEntity>(ref instance);
		}

		public void ResetToPool()
		{
			DecayEntity.ResetToPool(this);
		}

		public static void Serialize(Stream stream, DecayEntity instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(13);
			ProtocolParser.WriteSingle(stream, instance.decayTimer);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt32(stream, instance.buildingID);
			stream.WriteByte(29);
			ProtocolParser.WriteSingle(stream, instance.upkeepTimer);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, DecayEntity instance, DecayEntity previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.decayTimer != previous.decayTimer)
			{
				stream.WriteByte(13);
				ProtocolParser.WriteSingle(stream, instance.decayTimer);
			}
			if (instance.buildingID != previous.buildingID)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.buildingID);
			}
			if (instance.upkeepTimer != previous.upkeepTimer)
			{
				stream.WriteByte(29);
				ProtocolParser.WriteSingle(stream, instance.upkeepTimer);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, DecayEntity instance)
		{
			byte[] bytes = DecayEntity.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(DecayEntity instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				DecayEntity.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			DecayEntity.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return DecayEntity.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			DecayEntity.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, DecayEntity previous)
		{
			if (previous == null)
			{
				DecayEntity.Serialize(stream, this);
				return;
			}
			DecayEntity.SerializeDelta(stream, this, previous);
		}
	}
}