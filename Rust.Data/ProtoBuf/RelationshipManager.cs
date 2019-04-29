using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProtoBuf
{
	public class RelationshipManager : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public ulong lastTeamIndex;

		[NonSerialized]
		public List<PlayerTeam> teamList;

		[NonSerialized]
		public int maxTeamSize;

		public bool ShouldPool = true;

		private bool _disposed;

		public RelationshipManager()
		{
		}

		public RelationshipManager Copy()
		{
			RelationshipManager relationshipManager = Pool.Get<RelationshipManager>();
			this.CopyTo(relationshipManager);
			return relationshipManager;
		}

		public void CopyTo(RelationshipManager instance)
		{
			instance.lastTeamIndex = this.lastTeamIndex;
			throw new NotImplementedException();
		}

		public static RelationshipManager Deserialize(Stream stream)
		{
			RelationshipManager relationshipManager = Pool.Get<RelationshipManager>();
			RelationshipManager.Deserialize(stream, relationshipManager, false);
			return relationshipManager;
		}

		public static RelationshipManager Deserialize(byte[] buffer)
		{
			RelationshipManager relationshipManager = Pool.Get<RelationshipManager>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				RelationshipManager.Deserialize(memoryStream, relationshipManager, false);
			}
			return relationshipManager;
		}

		public static RelationshipManager Deserialize(byte[] buffer, RelationshipManager instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				RelationshipManager.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static RelationshipManager Deserialize(Stream stream, RelationshipManager instance, bool isDelta)
		{
			if (!isDelta && instance.teamList == null)
			{
				instance.teamList = Pool.Get<List<PlayerTeam>>();
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
					instance.lastTeamIndex = ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 18)
				{
					instance.teamList.Add(PlayerTeam.DeserializeLengthDelimited(stream));
				}
				else if (num == 24)
				{
					instance.maxTeamSize = (int)ProtocolParser.ReadUInt64(stream);
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

		public static RelationshipManager DeserializeLength(Stream stream, int length)
		{
			RelationshipManager relationshipManager = Pool.Get<RelationshipManager>();
			RelationshipManager.DeserializeLength(stream, length, relationshipManager, false);
			return relationshipManager;
		}

		public static RelationshipManager DeserializeLength(Stream stream, int length, RelationshipManager instance, bool isDelta)
		{
			if (!isDelta && instance.teamList == null)
			{
				instance.teamList = Pool.Get<List<PlayerTeam>>();
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
					instance.lastTeamIndex = ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 18)
				{
					instance.teamList.Add(PlayerTeam.DeserializeLengthDelimited(stream));
				}
				else if (num == 24)
				{
					instance.maxTeamSize = (int)ProtocolParser.ReadUInt64(stream);
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

		public static RelationshipManager DeserializeLengthDelimited(Stream stream)
		{
			RelationshipManager relationshipManager = Pool.Get<RelationshipManager>();
			RelationshipManager.DeserializeLengthDelimited(stream, relationshipManager, false);
			return relationshipManager;
		}

		public static RelationshipManager DeserializeLengthDelimited(Stream stream, RelationshipManager instance, bool isDelta)
		{
			if (!isDelta && instance.teamList == null)
			{
				instance.teamList = Pool.Get<List<PlayerTeam>>();
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
					instance.lastTeamIndex = ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 18)
				{
					instance.teamList.Add(PlayerTeam.DeserializeLengthDelimited(stream));
				}
				else if (num == 24)
				{
					instance.maxTeamSize = (int)ProtocolParser.ReadUInt64(stream);
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
			RelationshipManager.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			RelationshipManager.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(RelationshipManager instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.lastTeamIndex = (ulong)0;
			if (instance.teamList != null)
			{
				for (int i = 0; i < instance.teamList.Count; i++)
				{
					if (instance.teamList[i] != null)
					{
						instance.teamList[i].ResetToPool();
						instance.teamList[i] = null;
					}
				}
				List<PlayerTeam> playerTeams = instance.teamList;
				Pool.FreeList<PlayerTeam>(ref playerTeams);
				instance.teamList = playerTeams;
			}
			instance.maxTeamSize = 0;
			Pool.Free<RelationshipManager>(ref instance);
		}

		public void ResetToPool()
		{
			RelationshipManager.ResetToPool(this);
		}

		public static void Serialize(Stream stream, RelationshipManager instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, instance.lastTeamIndex);
			if (instance.teamList != null)
			{
				for (int i = 0; i < instance.teamList.Count; i++)
				{
					PlayerTeam item = instance.teamList[i];
					stream.WriteByte(18);
					memoryStream.SetLength((long)0);
					PlayerTeam.Serialize(memoryStream, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			stream.WriteByte(24);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.maxTeamSize);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, RelationshipManager instance, RelationshipManager previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.lastTeamIndex != previous.lastTeamIndex)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, instance.lastTeamIndex);
			}
			if (instance.teamList != null)
			{
				for (int i = 0; i < instance.teamList.Count; i++)
				{
					PlayerTeam item = instance.teamList[i];
					stream.WriteByte(18);
					memoryStream.SetLength((long)0);
					PlayerTeam.SerializeDelta(memoryStream, item, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			if (instance.maxTeamSize != previous.maxTeamSize)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.maxTeamSize);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, RelationshipManager instance)
		{
			byte[] bytes = RelationshipManager.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(RelationshipManager instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				RelationshipManager.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			RelationshipManager.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return RelationshipManager.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			RelationshipManager.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, RelationshipManager previous)
		{
			if (previous == null)
			{
				RelationshipManager.Serialize(stream, this);
				return;
			}
			RelationshipManager.SerializeDelta(stream, this, previous);
		}
	}
}