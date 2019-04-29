using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class PlantEntity : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public int state;

		[NonSerialized]
		public float age;

		[NonSerialized]
		public int genetics;

		[NonSerialized]
		public int water;

		[NonSerialized]
		public float healthy;

		[NonSerialized]
		public float totalAge;

		[NonSerialized]
		public float growthAge;

		[NonSerialized]
		public float yieldFraction;

		[NonSerialized]
		public float stageAge;

		public bool ShouldPool = true;

		private bool _disposed;

		public PlantEntity()
		{
		}

		public PlantEntity Copy()
		{
			PlantEntity plantEntity = Pool.Get<PlantEntity>();
			this.CopyTo(plantEntity);
			return plantEntity;
		}

		public void CopyTo(PlantEntity instance)
		{
			instance.state = this.state;
			instance.age = this.age;
			instance.genetics = this.genetics;
			instance.water = this.water;
			instance.healthy = this.healthy;
			instance.totalAge = this.totalAge;
			instance.growthAge = this.growthAge;
			instance.yieldFraction = this.yieldFraction;
			instance.stageAge = this.stageAge;
		}

		public static PlantEntity Deserialize(Stream stream)
		{
			PlantEntity plantEntity = Pool.Get<PlantEntity>();
			PlantEntity.Deserialize(stream, plantEntity, false);
			return plantEntity;
		}

		public static PlantEntity Deserialize(byte[] buffer)
		{
			PlantEntity plantEntity = Pool.Get<PlantEntity>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlantEntity.Deserialize(memoryStream, plantEntity, false);
			}
			return plantEntity;
		}

		public static PlantEntity Deserialize(byte[] buffer, PlantEntity instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlantEntity.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static PlantEntity Deserialize(Stream stream, PlantEntity instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 32)
				{
					if (num <= 21)
					{
						if (num == 8)
						{
							instance.state = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 21)
						{
							instance.age = ProtocolParser.ReadSingle(stream);
							continue;
						}
					}
					else if (num == 24)
					{
						instance.genetics = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.water = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num <= 53)
				{
					if (num == 45)
					{
						instance.healthy = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 53)
					{
						instance.totalAge = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num == 61)
				{
					instance.growthAge = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 69)
				{
					instance.yieldFraction = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 77)
				{
					instance.stageAge = ProtocolParser.ReadSingle(stream);
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

		public static PlantEntity DeserializeLength(Stream stream, int length)
		{
			PlantEntity plantEntity = Pool.Get<PlantEntity>();
			PlantEntity.DeserializeLength(stream, length, plantEntity, false);
			return plantEntity;
		}

		public static PlantEntity DeserializeLength(Stream stream, int length, PlantEntity instance, bool isDelta)
		{
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
					if (num <= 21)
					{
						if (num == 8)
						{
							instance.state = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 21)
						{
							instance.age = ProtocolParser.ReadSingle(stream);
							continue;
						}
					}
					else if (num == 24)
					{
						instance.genetics = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.water = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num <= 53)
				{
					if (num == 45)
					{
						instance.healthy = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 53)
					{
						instance.totalAge = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num == 61)
				{
					instance.growthAge = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 69)
				{
					instance.yieldFraction = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 77)
				{
					instance.stageAge = ProtocolParser.ReadSingle(stream);
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

		public static PlantEntity DeserializeLengthDelimited(Stream stream)
		{
			PlantEntity plantEntity = Pool.Get<PlantEntity>();
			PlantEntity.DeserializeLengthDelimited(stream, plantEntity, false);
			return plantEntity;
		}

		public static PlantEntity DeserializeLengthDelimited(Stream stream, PlantEntity instance, bool isDelta)
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
				if (num <= 32)
				{
					if (num <= 21)
					{
						if (num == 8)
						{
							instance.state = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 21)
						{
							instance.age = ProtocolParser.ReadSingle(stream);
							continue;
						}
					}
					else if (num == 24)
					{
						instance.genetics = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.water = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num <= 53)
				{
					if (num == 45)
					{
						instance.healthy = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 53)
					{
						instance.totalAge = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num == 61)
				{
					instance.growthAge = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 69)
				{
					instance.yieldFraction = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 77)
				{
					instance.stageAge = ProtocolParser.ReadSingle(stream);
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
			PlantEntity.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			PlantEntity.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(PlantEntity instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.state = 0;
			instance.age = 0f;
			instance.genetics = 0;
			instance.water = 0;
			instance.healthy = 0f;
			instance.totalAge = 0f;
			instance.growthAge = 0f;
			instance.yieldFraction = 0f;
			instance.stageAge = 0f;
			Pool.Free<PlantEntity>(ref instance);
		}

		public void ResetToPool()
		{
			PlantEntity.ResetToPool(this);
		}

		public static void Serialize(Stream stream, PlantEntity instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.state);
			stream.WriteByte(21);
			ProtocolParser.WriteSingle(stream, instance.age);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.genetics);
			stream.WriteByte(32);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.water);
			stream.WriteByte(45);
			ProtocolParser.WriteSingle(stream, instance.healthy);
			stream.WriteByte(53);
			ProtocolParser.WriteSingle(stream, instance.totalAge);
			stream.WriteByte(61);
			ProtocolParser.WriteSingle(stream, instance.growthAge);
			stream.WriteByte(69);
			ProtocolParser.WriteSingle(stream, instance.yieldFraction);
			stream.WriteByte(77);
			ProtocolParser.WriteSingle(stream, instance.stageAge);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, PlantEntity instance, PlantEntity previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.state != previous.state)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.state);
			}
			if (instance.age != previous.age)
			{
				stream.WriteByte(21);
				ProtocolParser.WriteSingle(stream, instance.age);
			}
			if (instance.genetics != previous.genetics)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.genetics);
			}
			if (instance.water != previous.water)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.water);
			}
			if (instance.healthy != previous.healthy)
			{
				stream.WriteByte(45);
				ProtocolParser.WriteSingle(stream, instance.healthy);
			}
			if (instance.totalAge != previous.totalAge)
			{
				stream.WriteByte(53);
				ProtocolParser.WriteSingle(stream, instance.totalAge);
			}
			if (instance.growthAge != previous.growthAge)
			{
				stream.WriteByte(61);
				ProtocolParser.WriteSingle(stream, instance.growthAge);
			}
			if (instance.yieldFraction != previous.yieldFraction)
			{
				stream.WriteByte(69);
				ProtocolParser.WriteSingle(stream, instance.yieldFraction);
			}
			if (instance.stageAge != previous.stageAge)
			{
				stream.WriteByte(77);
				ProtocolParser.WriteSingle(stream, instance.stageAge);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, PlantEntity instance)
		{
			byte[] bytes = PlantEntity.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(PlantEntity instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				PlantEntity.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			PlantEntity.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return PlantEntity.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			PlantEntity.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, PlantEntity previous)
		{
			if (previous == null)
			{
				PlantEntity.Serialize(stream, this);
				return;
			}
			PlantEntity.SerializeDelta(stream, this, previous);
		}
	}
}