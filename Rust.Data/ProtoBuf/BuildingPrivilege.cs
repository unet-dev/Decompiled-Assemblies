using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProtoBuf
{
	public class BuildingPrivilege : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public List<PlayerNameID> users;

		[NonSerialized]
		public float upkeepPeriodMinutes;

		[NonSerialized]
		public float costFraction;

		[NonSerialized]
		public float protectedMinutes;

		public bool ShouldPool = true;

		private bool _disposed;

		public BuildingPrivilege()
		{
		}

		public BuildingPrivilege Copy()
		{
			BuildingPrivilege buildingPrivilege = Pool.Get<BuildingPrivilege>();
			this.CopyTo(buildingPrivilege);
			return buildingPrivilege;
		}

		public void CopyTo(BuildingPrivilege instance)
		{
			throw new NotImplementedException();
		}

		public static BuildingPrivilege Deserialize(Stream stream)
		{
			BuildingPrivilege buildingPrivilege = Pool.Get<BuildingPrivilege>();
			BuildingPrivilege.Deserialize(stream, buildingPrivilege, false);
			return buildingPrivilege;
		}

		public static BuildingPrivilege Deserialize(byte[] buffer)
		{
			BuildingPrivilege buildingPrivilege = Pool.Get<BuildingPrivilege>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BuildingPrivilege.Deserialize(memoryStream, buildingPrivilege, false);
			}
			return buildingPrivilege;
		}

		public static BuildingPrivilege Deserialize(byte[] buffer, BuildingPrivilege instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BuildingPrivilege.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static BuildingPrivilege Deserialize(Stream stream, BuildingPrivilege instance, bool isDelta)
		{
			if (!isDelta && instance.users == null)
			{
				instance.users = Pool.Get<List<PlayerNameID>>();
			}
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 21)
				{
					if (num == 10)
					{
						instance.users.Add(PlayerNameID.DeserializeLengthDelimited(stream));
						continue;
					}
					else if (num == 21)
					{
						instance.upkeepPeriodMinutes = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num == 29)
				{
					instance.costFraction = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 37)
				{
					instance.protectedMinutes = ProtocolParser.ReadSingle(stream);
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

		public static BuildingPrivilege DeserializeLength(Stream stream, int length)
		{
			BuildingPrivilege buildingPrivilege = Pool.Get<BuildingPrivilege>();
			BuildingPrivilege.DeserializeLength(stream, length, buildingPrivilege, false);
			return buildingPrivilege;
		}

		public static BuildingPrivilege DeserializeLength(Stream stream, int length, BuildingPrivilege instance, bool isDelta)
		{
			if (!isDelta && instance.users == null)
			{
				instance.users = Pool.Get<List<PlayerNameID>>();
			}
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num <= 21)
				{
					if (num == 10)
					{
						instance.users.Add(PlayerNameID.DeserializeLengthDelimited(stream));
						continue;
					}
					else if (num == 21)
					{
						instance.upkeepPeriodMinutes = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num == 29)
				{
					instance.costFraction = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 37)
				{
					instance.protectedMinutes = ProtocolParser.ReadSingle(stream);
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

		public static BuildingPrivilege DeserializeLengthDelimited(Stream stream)
		{
			BuildingPrivilege buildingPrivilege = Pool.Get<BuildingPrivilege>();
			BuildingPrivilege.DeserializeLengthDelimited(stream, buildingPrivilege, false);
			return buildingPrivilege;
		}

		public static BuildingPrivilege DeserializeLengthDelimited(Stream stream, BuildingPrivilege instance, bool isDelta)
		{
			if (!isDelta && instance.users == null)
			{
				instance.users = Pool.Get<List<PlayerNameID>>();
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
				if (num <= 21)
				{
					if (num == 10)
					{
						instance.users.Add(PlayerNameID.DeserializeLengthDelimited(stream));
						continue;
					}
					else if (num == 21)
					{
						instance.upkeepPeriodMinutes = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num == 29)
				{
					instance.costFraction = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 37)
				{
					instance.protectedMinutes = ProtocolParser.ReadSingle(stream);
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
			BuildingPrivilege.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			BuildingPrivilege.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(BuildingPrivilege instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.users != null)
			{
				for (int i = 0; i < instance.users.Count; i++)
				{
					if (instance.users[i] != null)
					{
						instance.users[i].ResetToPool();
						instance.users[i] = null;
					}
				}
				List<PlayerNameID> playerNameIDs = instance.users;
				Pool.FreeList<PlayerNameID>(ref playerNameIDs);
				instance.users = playerNameIDs;
			}
			instance.upkeepPeriodMinutes = 0f;
			instance.costFraction = 0f;
			instance.protectedMinutes = 0f;
			Pool.Free<BuildingPrivilege>(ref instance);
		}

		public void ResetToPool()
		{
			BuildingPrivilege.ResetToPool(this);
		}

		public static void Serialize(Stream stream, BuildingPrivilege instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.users != null)
			{
				for (int i = 0; i < instance.users.Count; i++)
				{
					PlayerNameID item = instance.users[i];
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					PlayerNameID.Serialize(memoryStream, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			stream.WriteByte(21);
			ProtocolParser.WriteSingle(stream, instance.upkeepPeriodMinutes);
			stream.WriteByte(29);
			ProtocolParser.WriteSingle(stream, instance.costFraction);
			stream.WriteByte(37);
			ProtocolParser.WriteSingle(stream, instance.protectedMinutes);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, BuildingPrivilege instance, BuildingPrivilege previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.users != null)
			{
				for (int i = 0; i < instance.users.Count; i++)
				{
					PlayerNameID item = instance.users[i];
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					PlayerNameID.SerializeDelta(memoryStream, item, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			if (instance.upkeepPeriodMinutes != previous.upkeepPeriodMinutes)
			{
				stream.WriteByte(21);
				ProtocolParser.WriteSingle(stream, instance.upkeepPeriodMinutes);
			}
			if (instance.costFraction != previous.costFraction)
			{
				stream.WriteByte(29);
				ProtocolParser.WriteSingle(stream, instance.costFraction);
			}
			if (instance.protectedMinutes != previous.protectedMinutes)
			{
				stream.WriteByte(37);
				ProtocolParser.WriteSingle(stream, instance.protectedMinutes);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, BuildingPrivilege instance)
		{
			byte[] bytes = BuildingPrivilege.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(BuildingPrivilege instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BuildingPrivilege.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			BuildingPrivilege.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return BuildingPrivilege.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			BuildingPrivilege.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, BuildingPrivilege previous)
		{
			if (previous == null)
			{
				BuildingPrivilege.Serialize(stream, this);
				return;
			}
			BuildingPrivilege.SerializeDelta(stream, this, previous);
		}
	}
}