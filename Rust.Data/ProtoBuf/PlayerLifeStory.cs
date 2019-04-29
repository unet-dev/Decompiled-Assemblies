using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class PlayerLifeStory : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public float secondsAlive;

		[NonSerialized]
		public float metersWalked;

		[NonSerialized]
		public float metersRun;

		[NonSerialized]
		public float secondsSleeping;

		[NonSerialized]
		public uint timeBorn;

		[NonSerialized]
		public uint timeDied;

		[NonSerialized]
		public PlayerLifeStory.DeathInfo deathInfo;

		public bool ShouldPool = true;

		private bool _disposed;

		public PlayerLifeStory()
		{
		}

		public PlayerLifeStory Copy()
		{
			PlayerLifeStory playerLifeStory = Pool.Get<PlayerLifeStory>();
			this.CopyTo(playerLifeStory);
			return playerLifeStory;
		}

		public void CopyTo(PlayerLifeStory instance)
		{
			instance.secondsAlive = this.secondsAlive;
			instance.metersWalked = this.metersWalked;
			instance.metersRun = this.metersRun;
			instance.secondsSleeping = this.secondsSleeping;
			instance.timeBorn = this.timeBorn;
			instance.timeDied = this.timeDied;
			if (this.deathInfo == null)
			{
				instance.deathInfo = null;
				return;
			}
			if (instance.deathInfo == null)
			{
				instance.deathInfo = this.deathInfo.Copy();
				return;
			}
			this.deathInfo.CopyTo(instance.deathInfo);
		}

		public static PlayerLifeStory Deserialize(Stream stream)
		{
			PlayerLifeStory playerLifeStory = Pool.Get<PlayerLifeStory>();
			PlayerLifeStory.Deserialize(stream, playerLifeStory, false);
			return playerLifeStory;
		}

		public static PlayerLifeStory Deserialize(byte[] buffer)
		{
			PlayerLifeStory playerLifeStory = Pool.Get<PlayerLifeStory>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerLifeStory.Deserialize(memoryStream, playerLifeStory, false);
			}
			return playerLifeStory;
		}

		public static PlayerLifeStory Deserialize(byte[] buffer, PlayerLifeStory instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerLifeStory.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static PlayerLifeStory Deserialize(Stream stream, PlayerLifeStory instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				Key key = ProtocolParser.ReadKey((byte)num, stream);
				uint field = key.Field;
				if (field == 0)
				{
					throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
				}
				switch (field)
				{
					case 100:
					{
						if (key.WireType != Wire.Fixed32)
						{
							continue;
						}
						instance.secondsAlive = ProtocolParser.ReadSingle(stream);
						continue;
					}
					case 101:
					{
						if (key.WireType != Wire.Fixed32)
						{
							continue;
						}
						instance.metersWalked = ProtocolParser.ReadSingle(stream);
						continue;
					}
					case 102:
					{
						if (key.WireType != Wire.Fixed32)
						{
							continue;
						}
						instance.metersRun = ProtocolParser.ReadSingle(stream);
						continue;
					}
					case 103:
					{
						if (key.WireType != Wire.Fixed32)
						{
							continue;
						}
						instance.secondsSleeping = ProtocolParser.ReadSingle(stream);
						continue;
					}
					case 104:
					{
						if (key.WireType != Wire.Varint)
						{
							continue;
						}
						instance.timeBorn = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					case 105:
					{
						if (key.WireType != Wire.Varint)
						{
							continue;
						}
						instance.timeDied = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					default:
					{
						if (field == 200)
						{
							if (key.WireType != Wire.LengthDelimited)
							{
								continue;
							}
							if (instance.deathInfo != null)
							{
								PlayerLifeStory.DeathInfo.DeserializeLengthDelimited(stream, instance.deathInfo, isDelta);
								continue;
							}
							else
							{
								instance.deathInfo = PlayerLifeStory.DeathInfo.DeserializeLengthDelimited(stream);
								continue;
							}
						}
						else
						{
							ProtocolParser.SkipKey(stream, key);
							continue;
						}
					}
				}
			}
			return instance;
		}

		public static PlayerLifeStory DeserializeLength(Stream stream, int length)
		{
			PlayerLifeStory playerLifeStory = Pool.Get<PlayerLifeStory>();
			PlayerLifeStory.DeserializeLength(stream, length, playerLifeStory, false);
			return playerLifeStory;
		}

		public static PlayerLifeStory DeserializeLength(Stream stream, int length, PlayerLifeStory instance, bool isDelta)
		{
			Key key;
			long position = stream.Position + (long)length;
		Label1:
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				key = ProtocolParser.ReadKey((byte)num, stream);
				uint field = key.Field;
				if (field == 0)
				{
					throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
				}
				switch (field)
				{
					case 100:
					{
						if (key.WireType != Wire.Fixed32)
						{
							continue;
						}
						instance.secondsAlive = ProtocolParser.ReadSingle(stream);
						continue;
					}
					case 101:
					{
						if (key.WireType != Wire.Fixed32)
						{
							continue;
						}
						instance.metersWalked = ProtocolParser.ReadSingle(stream);
						continue;
					}
					case 102:
					{
						if (key.WireType != Wire.Fixed32)
						{
							continue;
						}
						instance.metersRun = ProtocolParser.ReadSingle(stream);
						continue;
					}
					case 103:
					{
						if (key.WireType != Wire.Fixed32)
						{
							continue;
						}
						instance.secondsSleeping = ProtocolParser.ReadSingle(stream);
						continue;
					}
					case 104:
					{
						if (key.WireType != Wire.Varint)
						{
							continue;
						}
						instance.timeBorn = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					case 105:
					{
						if (key.WireType != Wire.Varint)
						{
							continue;
						}
						instance.timeDied = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					default:
					{
						if (field == 200)
						{
							break;
						}
						else
						{
							goto Label0;
						}
					}
				}
				if (key.WireType != Wire.LengthDelimited)
				{
					continue;
				}
				if (instance.deathInfo != null)
				{
					PlayerLifeStory.DeathInfo.DeserializeLengthDelimited(stream, instance.deathInfo, isDelta);
				}
				else
				{
					instance.deathInfo = PlayerLifeStory.DeathInfo.DeserializeLengthDelimited(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		Label0:
			ProtocolParser.SkipKey(stream, key);
			goto Label1;
		}

		public static PlayerLifeStory DeserializeLengthDelimited(Stream stream)
		{
			PlayerLifeStory playerLifeStory = Pool.Get<PlayerLifeStory>();
			PlayerLifeStory.DeserializeLengthDelimited(stream, playerLifeStory, false);
			return playerLifeStory;
		}

		public static PlayerLifeStory DeserializeLengthDelimited(Stream stream, PlayerLifeStory instance, bool isDelta)
		{
			Key key;
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
		Label1:
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				key = ProtocolParser.ReadKey((byte)num, stream);
				uint field = key.Field;
				if (field == 0)
				{
					throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
				}
				switch (field)
				{
					case 100:
					{
						if (key.WireType != Wire.Fixed32)
						{
							continue;
						}
						instance.secondsAlive = ProtocolParser.ReadSingle(stream);
						continue;
					}
					case 101:
					{
						if (key.WireType != Wire.Fixed32)
						{
							continue;
						}
						instance.metersWalked = ProtocolParser.ReadSingle(stream);
						continue;
					}
					case 102:
					{
						if (key.WireType != Wire.Fixed32)
						{
							continue;
						}
						instance.metersRun = ProtocolParser.ReadSingle(stream);
						continue;
					}
					case 103:
					{
						if (key.WireType != Wire.Fixed32)
						{
							continue;
						}
						instance.secondsSleeping = ProtocolParser.ReadSingle(stream);
						continue;
					}
					case 104:
					{
						if (key.WireType != Wire.Varint)
						{
							continue;
						}
						instance.timeBorn = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					case 105:
					{
						if (key.WireType != Wire.Varint)
						{
							continue;
						}
						instance.timeDied = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					default:
					{
						if (field == 200)
						{
							break;
						}
						else
						{
							goto Label0;
						}
					}
				}
				if (key.WireType != Wire.LengthDelimited)
				{
					continue;
				}
				if (instance.deathInfo != null)
				{
					PlayerLifeStory.DeathInfo.DeserializeLengthDelimited(stream, instance.deathInfo, isDelta);
				}
				else
				{
					instance.deathInfo = PlayerLifeStory.DeathInfo.DeserializeLengthDelimited(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		Label0:
			ProtocolParser.SkipKey(stream, key);
			goto Label1;
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
			PlayerLifeStory.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			PlayerLifeStory.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(PlayerLifeStory instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.secondsAlive = 0f;
			instance.metersWalked = 0f;
			instance.metersRun = 0f;
			instance.secondsSleeping = 0f;
			instance.timeBorn = 0;
			instance.timeDied = 0;
			if (instance.deathInfo != null)
			{
				instance.deathInfo.ResetToPool();
				instance.deathInfo = null;
			}
			Pool.Free<PlayerLifeStory>(ref instance);
		}

		public void ResetToPool()
		{
			PlayerLifeStory.ResetToPool(this);
		}

		public static void Serialize(Stream stream, PlayerLifeStory instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(165);
			stream.WriteByte(6);
			ProtocolParser.WriteSingle(stream, instance.secondsAlive);
			stream.WriteByte(173);
			stream.WriteByte(6);
			ProtocolParser.WriteSingle(stream, instance.metersWalked);
			stream.WriteByte(181);
			stream.WriteByte(6);
			ProtocolParser.WriteSingle(stream, instance.metersRun);
			stream.WriteByte(189);
			stream.WriteByte(6);
			ProtocolParser.WriteSingle(stream, instance.secondsSleeping);
			stream.WriteByte(192);
			stream.WriteByte(6);
			ProtocolParser.WriteUInt32(stream, instance.timeBorn);
			stream.WriteByte(200);
			stream.WriteByte(6);
			ProtocolParser.WriteUInt32(stream, instance.timeDied);
			if (instance.deathInfo != null)
			{
				stream.WriteByte(194);
				stream.WriteByte(12);
				memoryStream.SetLength((long)0);
				PlayerLifeStory.DeathInfo.Serialize(memoryStream, instance.deathInfo);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, PlayerLifeStory instance, PlayerLifeStory previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.secondsAlive != previous.secondsAlive)
			{
				stream.WriteByte(165);
				stream.WriteByte(6);
				ProtocolParser.WriteSingle(stream, instance.secondsAlive);
			}
			if (instance.metersWalked != previous.metersWalked)
			{
				stream.WriteByte(173);
				stream.WriteByte(6);
				ProtocolParser.WriteSingle(stream, instance.metersWalked);
			}
			if (instance.metersRun != previous.metersRun)
			{
				stream.WriteByte(181);
				stream.WriteByte(6);
				ProtocolParser.WriteSingle(stream, instance.metersRun);
			}
			if (instance.secondsSleeping != previous.secondsSleeping)
			{
				stream.WriteByte(189);
				stream.WriteByte(6);
				ProtocolParser.WriteSingle(stream, instance.secondsSleeping);
			}
			if (instance.timeBorn != previous.timeBorn)
			{
				stream.WriteByte(192);
				stream.WriteByte(6);
				ProtocolParser.WriteUInt32(stream, instance.timeBorn);
			}
			if (instance.timeDied != previous.timeDied)
			{
				stream.WriteByte(200);
				stream.WriteByte(6);
				ProtocolParser.WriteUInt32(stream, instance.timeDied);
			}
			if (instance.deathInfo != null)
			{
				stream.WriteByte(194);
				stream.WriteByte(12);
				memoryStream.SetLength((long)0);
				PlayerLifeStory.DeathInfo.SerializeDelta(memoryStream, instance.deathInfo, previous.deathInfo);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, PlayerLifeStory instance)
		{
			byte[] bytes = PlayerLifeStory.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(PlayerLifeStory instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				PlayerLifeStory.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			PlayerLifeStory.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return PlayerLifeStory.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			PlayerLifeStory.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, PlayerLifeStory previous)
		{
			if (previous == null)
			{
				PlayerLifeStory.Serialize(stream, this);
				return;
			}
			PlayerLifeStory.SerializeDelta(stream, this, previous);
		}

		public class DeathInfo : IDisposable, Pool.IPooled, IProto
		{
			[NonSerialized]
			public string attackerName;

			[NonSerialized]
			public ulong attackerSteamID;

			[NonSerialized]
			public string hitBone;

			[NonSerialized]
			public string inflictorName;

			[NonSerialized]
			public int lastDamageType;

			public bool ShouldPool;

			private bool _disposed;

			public DeathInfo()
			{
			}

			public PlayerLifeStory.DeathInfo Copy()
			{
				PlayerLifeStory.DeathInfo deathInfo = Pool.Get<PlayerLifeStory.DeathInfo>();
				this.CopyTo(deathInfo);
				return deathInfo;
			}

			public void CopyTo(PlayerLifeStory.DeathInfo instance)
			{
				instance.attackerName = this.attackerName;
				instance.attackerSteamID = this.attackerSteamID;
				instance.hitBone = this.hitBone;
				instance.inflictorName = this.inflictorName;
				instance.lastDamageType = this.lastDamageType;
			}

			public static PlayerLifeStory.DeathInfo Deserialize(Stream stream)
			{
				PlayerLifeStory.DeathInfo deathInfo = Pool.Get<PlayerLifeStory.DeathInfo>();
				PlayerLifeStory.DeathInfo.Deserialize(stream, deathInfo, false);
				return deathInfo;
			}

			public static PlayerLifeStory.DeathInfo Deserialize(byte[] buffer)
			{
				PlayerLifeStory.DeathInfo deathInfo = Pool.Get<PlayerLifeStory.DeathInfo>();
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					PlayerLifeStory.DeathInfo.Deserialize(memoryStream, deathInfo, false);
				}
				return deathInfo;
			}

			public static PlayerLifeStory.DeathInfo Deserialize(byte[] buffer, PlayerLifeStory.DeathInfo instance, bool isDelta = false)
			{
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					PlayerLifeStory.DeathInfo.Deserialize(memoryStream, instance, isDelta);
				}
				return instance;
			}

			public static PlayerLifeStory.DeathInfo Deserialize(Stream stream, PlayerLifeStory.DeathInfo instance, bool isDelta)
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
						if (num == 10)
						{
							instance.attackerName = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.attackerSteamID = ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 26)
					{
						instance.hitBone = ProtocolParser.ReadString(stream);
						continue;
					}
					else if (num == 34)
					{
						instance.inflictorName = ProtocolParser.ReadString(stream);
						continue;
					}
					else if (num == 40)
					{
						instance.lastDamageType = (int)ProtocolParser.ReadUInt64(stream);
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

			public static PlayerLifeStory.DeathInfo DeserializeLength(Stream stream, int length)
			{
				PlayerLifeStory.DeathInfo deathInfo = Pool.Get<PlayerLifeStory.DeathInfo>();
				PlayerLifeStory.DeathInfo.DeserializeLength(stream, length, deathInfo, false);
				return deathInfo;
			}

			public static PlayerLifeStory.DeathInfo DeserializeLength(Stream stream, int length, PlayerLifeStory.DeathInfo instance, bool isDelta)
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
						if (num == 10)
						{
							instance.attackerName = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.attackerSteamID = ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 26)
					{
						instance.hitBone = ProtocolParser.ReadString(stream);
						continue;
					}
					else if (num == 34)
					{
						instance.inflictorName = ProtocolParser.ReadString(stream);
						continue;
					}
					else if (num == 40)
					{
						instance.lastDamageType = (int)ProtocolParser.ReadUInt64(stream);
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

			public static PlayerLifeStory.DeathInfo DeserializeLengthDelimited(Stream stream)
			{
				PlayerLifeStory.DeathInfo deathInfo = Pool.Get<PlayerLifeStory.DeathInfo>();
				PlayerLifeStory.DeathInfo.DeserializeLengthDelimited(stream, deathInfo, false);
				return deathInfo;
			}

			public static PlayerLifeStory.DeathInfo DeserializeLengthDelimited(Stream stream, PlayerLifeStory.DeathInfo instance, bool isDelta)
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
						if (num == 10)
						{
							instance.attackerName = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.attackerSteamID = ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 26)
					{
						instance.hitBone = ProtocolParser.ReadString(stream);
						continue;
					}
					else if (num == 34)
					{
						instance.inflictorName = ProtocolParser.ReadString(stream);
						continue;
					}
					else if (num == 40)
					{
						instance.lastDamageType = (int)ProtocolParser.ReadUInt64(stream);
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
				PlayerLifeStory.DeathInfo.Deserialize(stream, this, isDelta);
			}

			public virtual void LeavePool()
			{
				this._disposed = false;
			}

			public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
			{
				PlayerLifeStory.DeathInfo.DeserializeLength(stream, size, this, isDelta);
			}

			public static void ResetToPool(PlayerLifeStory.DeathInfo instance)
			{
				if (!instance.ShouldPool)
				{
					return;
				}
				instance.attackerName = string.Empty;
				instance.attackerSteamID = (ulong)0;
				instance.hitBone = string.Empty;
				instance.inflictorName = string.Empty;
				instance.lastDamageType = 0;
				Pool.Free<PlayerLifeStory.DeathInfo>(ref instance);
			}

			public void ResetToPool()
			{
				PlayerLifeStory.DeathInfo.ResetToPool(this);
			}

			public static void Serialize(Stream stream, PlayerLifeStory.DeathInfo instance)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.attackerName != null)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteString(stream, instance.attackerName);
				}
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, instance.attackerSteamID);
				if (instance.hitBone != null)
				{
					stream.WriteByte(26);
					ProtocolParser.WriteString(stream, instance.hitBone);
				}
				if (instance.inflictorName != null)
				{
					stream.WriteByte(34);
					ProtocolParser.WriteString(stream, instance.inflictorName);
				}
				stream.WriteByte(40);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.lastDamageType);
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeDelta(Stream stream, PlayerLifeStory.DeathInfo instance, PlayerLifeStory.DeathInfo previous)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.attackerName != null && instance.attackerName != previous.attackerName)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteString(stream, instance.attackerName);
				}
				if (instance.attackerSteamID != previous.attackerSteamID)
				{
					stream.WriteByte(16);
					ProtocolParser.WriteUInt64(stream, instance.attackerSteamID);
				}
				if (instance.hitBone != null && instance.hitBone != previous.hitBone)
				{
					stream.WriteByte(26);
					ProtocolParser.WriteString(stream, instance.hitBone);
				}
				if (instance.inflictorName != null && instance.inflictorName != previous.inflictorName)
				{
					stream.WriteByte(34);
					ProtocolParser.WriteString(stream, instance.inflictorName);
				}
				if (instance.lastDamageType != previous.lastDamageType)
				{
					stream.WriteByte(40);
					ProtocolParser.WriteUInt64(stream, (ulong)instance.lastDamageType);
				}
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeLengthDelimited(Stream stream, PlayerLifeStory.DeathInfo instance)
			{
				byte[] bytes = PlayerLifeStory.DeathInfo.SerializeToBytes(instance);
				ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
				stream.Write(bytes, 0, (int)bytes.Length);
			}

			public static byte[] SerializeToBytes(PlayerLifeStory.DeathInfo instance)
			{
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					PlayerLifeStory.DeathInfo.Serialize(memoryStream, instance);
					array = memoryStream.ToArray();
				}
				return array;
			}

			public void ToProto(Stream stream)
			{
				PlayerLifeStory.DeathInfo.Serialize(stream, this);
			}

			public byte[] ToProtoBytes()
			{
				return PlayerLifeStory.DeathInfo.SerializeToBytes(this);
			}

			public virtual void WriteToStream(Stream stream)
			{
				PlayerLifeStory.DeathInfo.Serialize(stream, this);
			}

			public virtual void WriteToStreamDelta(Stream stream, PlayerLifeStory.DeathInfo previous)
			{
				if (previous == null)
				{
					PlayerLifeStory.DeathInfo.Serialize(stream, this);
					return;
				}
				PlayerLifeStory.DeathInfo.SerializeDelta(stream, this, previous);
			}
		}
	}
}