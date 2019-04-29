using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class PlayerMetabolism : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public float health;

		[NonSerialized]
		public float calories;

		[NonSerialized]
		public float hydration;

		[NonSerialized]
		public float heartrate;

		[NonSerialized]
		public float temperature;

		[NonSerialized]
		public float poison;

		[NonSerialized]
		public float radiation_level;

		[NonSerialized]
		public float wetness;

		[NonSerialized]
		public float dirtyness;

		[NonSerialized]
		public float oxygen;

		[NonSerialized]
		public float bleeding;

		[NonSerialized]
		public float radiation_poisoning;

		[NonSerialized]
		public float comfort;

		[NonSerialized]
		public float pending_health;

		public bool ShouldPool = true;

		private bool _disposed;

		public PlayerMetabolism()
		{
		}

		public PlayerMetabolism Copy()
		{
			PlayerMetabolism playerMetabolism = Pool.Get<PlayerMetabolism>();
			this.CopyTo(playerMetabolism);
			return playerMetabolism;
		}

		public void CopyTo(PlayerMetabolism instance)
		{
			instance.health = this.health;
			instance.calories = this.calories;
			instance.hydration = this.hydration;
			instance.heartrate = this.heartrate;
			instance.temperature = this.temperature;
			instance.poison = this.poison;
			instance.radiation_level = this.radiation_level;
			instance.wetness = this.wetness;
			instance.dirtyness = this.dirtyness;
			instance.oxygen = this.oxygen;
			instance.bleeding = this.bleeding;
			instance.radiation_poisoning = this.radiation_poisoning;
			instance.comfort = this.comfort;
			instance.pending_health = this.pending_health;
		}

		public static PlayerMetabolism Deserialize(Stream stream)
		{
			PlayerMetabolism playerMetabolism = Pool.Get<PlayerMetabolism>();
			PlayerMetabolism.Deserialize(stream, playerMetabolism, false);
			return playerMetabolism;
		}

		public static PlayerMetabolism Deserialize(byte[] buffer)
		{
			PlayerMetabolism playerMetabolism = Pool.Get<PlayerMetabolism>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerMetabolism.Deserialize(memoryStream, playerMetabolism, false);
			}
			return playerMetabolism;
		}

		public static PlayerMetabolism Deserialize(byte[] buffer, PlayerMetabolism instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerMetabolism.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static PlayerMetabolism Deserialize(Stream stream, PlayerMetabolism instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 61)
				{
					if (num <= 29)
					{
						if (num == 13)
						{
							instance.health = ProtocolParser.ReadSingle(stream);
							continue;
						}
						else if (num == 21)
						{
							instance.calories = ProtocolParser.ReadSingle(stream);
							continue;
						}
						else if (num == 29)
						{
							instance.hydration = ProtocolParser.ReadSingle(stream);
							continue;
						}
					}
					else if (num <= 45)
					{
						if (num == 37)
						{
							instance.heartrate = ProtocolParser.ReadSingle(stream);
							continue;
						}
						else if (num == 45)
						{
							instance.temperature = ProtocolParser.ReadSingle(stream);
							continue;
						}
					}
					else if (num == 53)
					{
						instance.poison = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 61)
					{
						instance.radiation_level = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num <= 85)
				{
					if (num == 69)
					{
						instance.wetness = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 77)
					{
						instance.dirtyness = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 85)
					{
						instance.oxygen = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num <= 101)
				{
					if (num == 93)
					{
						instance.bleeding = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 101)
					{
						instance.radiation_poisoning = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num == 109)
				{
					instance.comfort = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 117)
				{
					instance.pending_health = ProtocolParser.ReadSingle(stream);
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

		public static PlayerMetabolism DeserializeLength(Stream stream, int length)
		{
			PlayerMetabolism playerMetabolism = Pool.Get<PlayerMetabolism>();
			PlayerMetabolism.DeserializeLength(stream, length, playerMetabolism, false);
			return playerMetabolism;
		}

		public static PlayerMetabolism DeserializeLength(Stream stream, int length, PlayerMetabolism instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num <= 61)
				{
					if (num <= 29)
					{
						if (num == 13)
						{
							instance.health = ProtocolParser.ReadSingle(stream);
							continue;
						}
						else if (num == 21)
						{
							instance.calories = ProtocolParser.ReadSingle(stream);
							continue;
						}
						else if (num == 29)
						{
							instance.hydration = ProtocolParser.ReadSingle(stream);
							continue;
						}
					}
					else if (num <= 45)
					{
						if (num == 37)
						{
							instance.heartrate = ProtocolParser.ReadSingle(stream);
							continue;
						}
						else if (num == 45)
						{
							instance.temperature = ProtocolParser.ReadSingle(stream);
							continue;
						}
					}
					else if (num == 53)
					{
						instance.poison = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 61)
					{
						instance.radiation_level = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num <= 85)
				{
					if (num == 69)
					{
						instance.wetness = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 77)
					{
						instance.dirtyness = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 85)
					{
						instance.oxygen = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num <= 101)
				{
					if (num == 93)
					{
						instance.bleeding = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 101)
					{
						instance.radiation_poisoning = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num == 109)
				{
					instance.comfort = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 117)
				{
					instance.pending_health = ProtocolParser.ReadSingle(stream);
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

		public static PlayerMetabolism DeserializeLengthDelimited(Stream stream)
		{
			PlayerMetabolism playerMetabolism = Pool.Get<PlayerMetabolism>();
			PlayerMetabolism.DeserializeLengthDelimited(stream, playerMetabolism, false);
			return playerMetabolism;
		}

		public static PlayerMetabolism DeserializeLengthDelimited(Stream stream, PlayerMetabolism instance, bool isDelta)
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
				if (num <= 61)
				{
					if (num <= 29)
					{
						if (num == 13)
						{
							instance.health = ProtocolParser.ReadSingle(stream);
							continue;
						}
						else if (num == 21)
						{
							instance.calories = ProtocolParser.ReadSingle(stream);
							continue;
						}
						else if (num == 29)
						{
							instance.hydration = ProtocolParser.ReadSingle(stream);
							continue;
						}
					}
					else if (num <= 45)
					{
						if (num == 37)
						{
							instance.heartrate = ProtocolParser.ReadSingle(stream);
							continue;
						}
						else if (num == 45)
						{
							instance.temperature = ProtocolParser.ReadSingle(stream);
							continue;
						}
					}
					else if (num == 53)
					{
						instance.poison = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 61)
					{
						instance.radiation_level = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num <= 85)
				{
					if (num == 69)
					{
						instance.wetness = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 77)
					{
						instance.dirtyness = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 85)
					{
						instance.oxygen = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num <= 101)
				{
					if (num == 93)
					{
						instance.bleeding = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 101)
					{
						instance.radiation_poisoning = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num == 109)
				{
					instance.comfort = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 117)
				{
					instance.pending_health = ProtocolParser.ReadSingle(stream);
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
			PlayerMetabolism.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			PlayerMetabolism.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(PlayerMetabolism instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.health = 0f;
			instance.calories = 0f;
			instance.hydration = 0f;
			instance.heartrate = 0f;
			instance.temperature = 0f;
			instance.poison = 0f;
			instance.radiation_level = 0f;
			instance.wetness = 0f;
			instance.dirtyness = 0f;
			instance.oxygen = 0f;
			instance.bleeding = 0f;
			instance.radiation_poisoning = 0f;
			instance.comfort = 0f;
			instance.pending_health = 0f;
			Pool.Free<PlayerMetabolism>(ref instance);
		}

		public void ResetToPool()
		{
			PlayerMetabolism.ResetToPool(this);
		}

		public static void Serialize(Stream stream, PlayerMetabolism instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(13);
			ProtocolParser.WriteSingle(stream, instance.health);
			stream.WriteByte(21);
			ProtocolParser.WriteSingle(stream, instance.calories);
			stream.WriteByte(29);
			ProtocolParser.WriteSingle(stream, instance.hydration);
			stream.WriteByte(37);
			ProtocolParser.WriteSingle(stream, instance.heartrate);
			stream.WriteByte(45);
			ProtocolParser.WriteSingle(stream, instance.temperature);
			stream.WriteByte(53);
			ProtocolParser.WriteSingle(stream, instance.poison);
			stream.WriteByte(61);
			ProtocolParser.WriteSingle(stream, instance.radiation_level);
			stream.WriteByte(69);
			ProtocolParser.WriteSingle(stream, instance.wetness);
			stream.WriteByte(77);
			ProtocolParser.WriteSingle(stream, instance.dirtyness);
			stream.WriteByte(85);
			ProtocolParser.WriteSingle(stream, instance.oxygen);
			stream.WriteByte(93);
			ProtocolParser.WriteSingle(stream, instance.bleeding);
			stream.WriteByte(101);
			ProtocolParser.WriteSingle(stream, instance.radiation_poisoning);
			stream.WriteByte(109);
			ProtocolParser.WriteSingle(stream, instance.comfort);
			stream.WriteByte(117);
			ProtocolParser.WriteSingle(stream, instance.pending_health);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, PlayerMetabolism instance, PlayerMetabolism previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.health != previous.health)
			{
				stream.WriteByte(13);
				ProtocolParser.WriteSingle(stream, instance.health);
			}
			if (instance.calories != previous.calories)
			{
				stream.WriteByte(21);
				ProtocolParser.WriteSingle(stream, instance.calories);
			}
			if (instance.hydration != previous.hydration)
			{
				stream.WriteByte(29);
				ProtocolParser.WriteSingle(stream, instance.hydration);
			}
			if (instance.heartrate != previous.heartrate)
			{
				stream.WriteByte(37);
				ProtocolParser.WriteSingle(stream, instance.heartrate);
			}
			if (instance.temperature != previous.temperature)
			{
				stream.WriteByte(45);
				ProtocolParser.WriteSingle(stream, instance.temperature);
			}
			if (instance.poison != previous.poison)
			{
				stream.WriteByte(53);
				ProtocolParser.WriteSingle(stream, instance.poison);
			}
			if (instance.radiation_level != previous.radiation_level)
			{
				stream.WriteByte(61);
				ProtocolParser.WriteSingle(stream, instance.radiation_level);
			}
			if (instance.wetness != previous.wetness)
			{
				stream.WriteByte(69);
				ProtocolParser.WriteSingle(stream, instance.wetness);
			}
			if (instance.dirtyness != previous.dirtyness)
			{
				stream.WriteByte(77);
				ProtocolParser.WriteSingle(stream, instance.dirtyness);
			}
			if (instance.oxygen != previous.oxygen)
			{
				stream.WriteByte(85);
				ProtocolParser.WriteSingle(stream, instance.oxygen);
			}
			if (instance.bleeding != previous.bleeding)
			{
				stream.WriteByte(93);
				ProtocolParser.WriteSingle(stream, instance.bleeding);
			}
			if (instance.radiation_poisoning != previous.radiation_poisoning)
			{
				stream.WriteByte(101);
				ProtocolParser.WriteSingle(stream, instance.radiation_poisoning);
			}
			if (instance.comfort != previous.comfort)
			{
				stream.WriteByte(109);
				ProtocolParser.WriteSingle(stream, instance.comfort);
			}
			if (instance.pending_health != previous.pending_health)
			{
				stream.WriteByte(117);
				ProtocolParser.WriteSingle(stream, instance.pending_health);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, PlayerMetabolism instance)
		{
			byte[] bytes = PlayerMetabolism.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(PlayerMetabolism instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				PlayerMetabolism.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			PlayerMetabolism.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return PlayerMetabolism.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			PlayerMetabolism.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, PlayerMetabolism previous)
		{
			if (previous == null)
			{
				PlayerMetabolism.Serialize(stream, this);
				return;
			}
			PlayerMetabolism.SerializeDelta(stream, this, previous);
		}
	}
}