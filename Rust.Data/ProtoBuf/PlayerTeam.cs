using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ProtoBuf
{
	public class PlayerTeam : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public ulong teamID;

		[NonSerialized]
		public string teamName;

		[NonSerialized]
		public ulong teamLeader;

		[NonSerialized]
		public List<PlayerTeam.TeamMember> members;

		public bool ShouldPool = true;

		private bool _disposed;

		public PlayerTeam()
		{
		}

		public PlayerTeam Copy()
		{
			PlayerTeam playerTeam = Pool.Get<PlayerTeam>();
			this.CopyTo(playerTeam);
			return playerTeam;
		}

		public void CopyTo(PlayerTeam instance)
		{
			instance.teamID = this.teamID;
			instance.teamName = this.teamName;
			instance.teamLeader = this.teamLeader;
			throw new NotImplementedException();
		}

		public static PlayerTeam Deserialize(Stream stream)
		{
			PlayerTeam playerTeam = Pool.Get<PlayerTeam>();
			PlayerTeam.Deserialize(stream, playerTeam, false);
			return playerTeam;
		}

		public static PlayerTeam Deserialize(byte[] buffer)
		{
			PlayerTeam playerTeam = Pool.Get<PlayerTeam>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerTeam.Deserialize(memoryStream, playerTeam, false);
			}
			return playerTeam;
		}

		public static PlayerTeam Deserialize(byte[] buffer, PlayerTeam instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerTeam.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static PlayerTeam Deserialize(Stream stream, PlayerTeam instance, bool isDelta)
		{
			if (!isDelta && instance.members == null)
			{
				instance.members = Pool.Get<List<PlayerTeam.TeamMember>>();
			}
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 18)
				{
					if (num == 8)
					{
						instance.teamID = ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 18)
					{
						instance.teamName = ProtocolParser.ReadString(stream);
						continue;
					}
				}
				else if (num == 24)
				{
					instance.teamLeader = ProtocolParser.ReadUInt64(stream);
					continue;
				}
				else if (num == 34)
				{
					instance.members.Add(PlayerTeam.TeamMember.DeserializeLengthDelimited(stream));
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

		public static PlayerTeam DeserializeLength(Stream stream, int length)
		{
			PlayerTeam playerTeam = Pool.Get<PlayerTeam>();
			PlayerTeam.DeserializeLength(stream, length, playerTeam, false);
			return playerTeam;
		}

		public static PlayerTeam DeserializeLength(Stream stream, int length, PlayerTeam instance, bool isDelta)
		{
			if (!isDelta && instance.members == null)
			{
				instance.members = Pool.Get<List<PlayerTeam.TeamMember>>();
			}
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num <= 18)
				{
					if (num == 8)
					{
						instance.teamID = ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 18)
					{
						instance.teamName = ProtocolParser.ReadString(stream);
						continue;
					}
				}
				else if (num == 24)
				{
					instance.teamLeader = ProtocolParser.ReadUInt64(stream);
					continue;
				}
				else if (num == 34)
				{
					instance.members.Add(PlayerTeam.TeamMember.DeserializeLengthDelimited(stream));
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

		public static PlayerTeam DeserializeLengthDelimited(Stream stream)
		{
			PlayerTeam playerTeam = Pool.Get<PlayerTeam>();
			PlayerTeam.DeserializeLengthDelimited(stream, playerTeam, false);
			return playerTeam;
		}

		public static PlayerTeam DeserializeLengthDelimited(Stream stream, PlayerTeam instance, bool isDelta)
		{
			if (!isDelta && instance.members == null)
			{
				instance.members = Pool.Get<List<PlayerTeam.TeamMember>>();
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
				if (num <= 18)
				{
					if (num == 8)
					{
						instance.teamID = ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 18)
					{
						instance.teamName = ProtocolParser.ReadString(stream);
						continue;
					}
				}
				else if (num == 24)
				{
					instance.teamLeader = ProtocolParser.ReadUInt64(stream);
					continue;
				}
				else if (num == 34)
				{
					instance.members.Add(PlayerTeam.TeamMember.DeserializeLengthDelimited(stream));
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
			PlayerTeam.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			PlayerTeam.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(PlayerTeam instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.teamID = (ulong)0;
			instance.teamName = string.Empty;
			instance.teamLeader = (ulong)0;
			if (instance.members != null)
			{
				for (int i = 0; i < instance.members.Count; i++)
				{
					if (instance.members[i] != null)
					{
						instance.members[i].ResetToPool();
						instance.members[i] = null;
					}
				}
				List<PlayerTeam.TeamMember> teamMembers = instance.members;
				Pool.FreeList<PlayerTeam.TeamMember>(ref teamMembers);
				instance.members = teamMembers;
			}
			Pool.Free<PlayerTeam>(ref instance);
		}

		public void ResetToPool()
		{
			PlayerTeam.ResetToPool(this);
		}

		public static void Serialize(Stream stream, PlayerTeam instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, instance.teamID);
			if (instance.teamName != null)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteString(stream, instance.teamName);
			}
			stream.WriteByte(24);
			ProtocolParser.WriteUInt64(stream, instance.teamLeader);
			if (instance.members != null)
			{
				for (int i = 0; i < instance.members.Count; i++)
				{
					PlayerTeam.TeamMember item = instance.members[i];
					stream.WriteByte(34);
					memoryStream.SetLength((long)0);
					PlayerTeam.TeamMember.Serialize(memoryStream, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, PlayerTeam instance, PlayerTeam previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.teamID != previous.teamID)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, instance.teamID);
			}
			if (instance.teamName != null && instance.teamName != previous.teamName)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteString(stream, instance.teamName);
			}
			if (instance.teamLeader != previous.teamLeader)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, instance.teamLeader);
			}
			if (instance.members != null)
			{
				for (int i = 0; i < instance.members.Count; i++)
				{
					PlayerTeam.TeamMember item = instance.members[i];
					stream.WriteByte(34);
					memoryStream.SetLength((long)0);
					PlayerTeam.TeamMember.SerializeDelta(memoryStream, item, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, PlayerTeam instance)
		{
			byte[] bytes = PlayerTeam.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(PlayerTeam instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				PlayerTeam.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			PlayerTeam.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return PlayerTeam.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			PlayerTeam.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, PlayerTeam previous)
		{
			if (previous == null)
			{
				PlayerTeam.Serialize(stream, this);
				return;
			}
			PlayerTeam.SerializeDelta(stream, this, previous);
		}

		public class TeamMember : IDisposable, Pool.IPooled, IProto
		{
			[NonSerialized]
			public string displayName;

			[NonSerialized]
			public ulong userID;

			[NonSerialized]
			public float healthFraction;

			[NonSerialized]
			public Vector3 position;

			[NonSerialized]
			public bool online;

			public bool ShouldPool;

			private bool _disposed;

			public TeamMember()
			{
			}

			public PlayerTeam.TeamMember Copy()
			{
				PlayerTeam.TeamMember teamMember = Pool.Get<PlayerTeam.TeamMember>();
				this.CopyTo(teamMember);
				return teamMember;
			}

			public void CopyTo(PlayerTeam.TeamMember instance)
			{
				instance.displayName = this.displayName;
				instance.userID = this.userID;
				instance.healthFraction = this.healthFraction;
				instance.position = this.position;
				instance.online = this.online;
			}

			public static PlayerTeam.TeamMember Deserialize(Stream stream)
			{
				PlayerTeam.TeamMember teamMember = Pool.Get<PlayerTeam.TeamMember>();
				PlayerTeam.TeamMember.Deserialize(stream, teamMember, false);
				return teamMember;
			}

			public static PlayerTeam.TeamMember Deserialize(byte[] buffer)
			{
				PlayerTeam.TeamMember teamMember = Pool.Get<PlayerTeam.TeamMember>();
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					PlayerTeam.TeamMember.Deserialize(memoryStream, teamMember, false);
				}
				return teamMember;
			}

			public static PlayerTeam.TeamMember Deserialize(byte[] buffer, PlayerTeam.TeamMember instance, bool isDelta = false)
			{
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					PlayerTeam.TeamMember.Deserialize(memoryStream, instance, isDelta);
				}
				return instance;
			}

			public static PlayerTeam.TeamMember Deserialize(Stream stream, PlayerTeam.TeamMember instance, bool isDelta)
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
							instance.displayName = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.userID = ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 29)
					{
						instance.healthFraction = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 34)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.position, isDelta);
						continue;
					}
					else if (num == 40)
					{
						instance.online = ProtocolParser.ReadBool(stream);
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

			public static PlayerTeam.TeamMember DeserializeLength(Stream stream, int length)
			{
				PlayerTeam.TeamMember teamMember = Pool.Get<PlayerTeam.TeamMember>();
				PlayerTeam.TeamMember.DeserializeLength(stream, length, teamMember, false);
				return teamMember;
			}

			public static PlayerTeam.TeamMember DeserializeLength(Stream stream, int length, PlayerTeam.TeamMember instance, bool isDelta)
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
							instance.displayName = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.userID = ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 29)
					{
						instance.healthFraction = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 34)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.position, isDelta);
						continue;
					}
					else if (num == 40)
					{
						instance.online = ProtocolParser.ReadBool(stream);
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

			public static PlayerTeam.TeamMember DeserializeLengthDelimited(Stream stream)
			{
				PlayerTeam.TeamMember teamMember = Pool.Get<PlayerTeam.TeamMember>();
				PlayerTeam.TeamMember.DeserializeLengthDelimited(stream, teamMember, false);
				return teamMember;
			}

			public static PlayerTeam.TeamMember DeserializeLengthDelimited(Stream stream, PlayerTeam.TeamMember instance, bool isDelta)
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
							instance.displayName = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.userID = ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 29)
					{
						instance.healthFraction = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 34)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.position, isDelta);
						continue;
					}
					else if (num == 40)
					{
						instance.online = ProtocolParser.ReadBool(stream);
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
				PlayerTeam.TeamMember.Deserialize(stream, this, isDelta);
			}

			public virtual void LeavePool()
			{
				this._disposed = false;
			}

			public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
			{
				PlayerTeam.TeamMember.DeserializeLength(stream, size, this, isDelta);
			}

			public static void ResetToPool(PlayerTeam.TeamMember instance)
			{
				if (!instance.ShouldPool)
				{
					return;
				}
				instance.displayName = string.Empty;
				instance.userID = (ulong)0;
				instance.healthFraction = 0f;
				instance.position = new Vector3();
				instance.online = false;
				Pool.Free<PlayerTeam.TeamMember>(ref instance);
			}

			public void ResetToPool()
			{
				PlayerTeam.TeamMember.ResetToPool(this);
			}

			public static void Serialize(Stream stream, PlayerTeam.TeamMember instance)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.displayName != null)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteString(stream, instance.displayName);
				}
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, instance.userID);
				stream.WriteByte(29);
				ProtocolParser.WriteSingle(stream, instance.healthFraction);
				stream.WriteByte(34);
				memoryStream.SetLength((long)0);
				Vector3Serialized.Serialize(memoryStream, instance.position);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				stream.WriteByte(40);
				ProtocolParser.WriteBool(stream, instance.online);
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeDelta(Stream stream, PlayerTeam.TeamMember instance, PlayerTeam.TeamMember previous)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.displayName != null && instance.displayName != previous.displayName)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteString(stream, instance.displayName);
				}
				if (instance.userID != previous.userID)
				{
					stream.WriteByte(16);
					ProtocolParser.WriteUInt64(stream, instance.userID);
				}
				if (instance.healthFraction != previous.healthFraction)
				{
					stream.WriteByte(29);
					ProtocolParser.WriteSingle(stream, instance.healthFraction);
				}
				if (instance.position != previous.position)
				{
					stream.WriteByte(34);
					memoryStream.SetLength((long)0);
					Vector3Serialized.SerializeDelta(memoryStream, instance.position, previous.position);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
				stream.WriteByte(40);
				ProtocolParser.WriteBool(stream, instance.online);
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeLengthDelimited(Stream stream, PlayerTeam.TeamMember instance)
			{
				byte[] bytes = PlayerTeam.TeamMember.SerializeToBytes(instance);
				ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
				stream.Write(bytes, 0, (int)bytes.Length);
			}

			public static byte[] SerializeToBytes(PlayerTeam.TeamMember instance)
			{
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					PlayerTeam.TeamMember.Serialize(memoryStream, instance);
					array = memoryStream.ToArray();
				}
				return array;
			}

			public void ToProto(Stream stream)
			{
				PlayerTeam.TeamMember.Serialize(stream, this);
			}

			public byte[] ToProtoBytes()
			{
				return PlayerTeam.TeamMember.SerializeToBytes(this);
			}

			public virtual void WriteToStream(Stream stream)
			{
				PlayerTeam.TeamMember.Serialize(stream, this);
			}

			public virtual void WriteToStreamDelta(Stream stream, PlayerTeam.TeamMember previous)
			{
				if (previous == null)
				{
					PlayerTeam.TeamMember.Serialize(stream, this);
					return;
				}
				PlayerTeam.TeamMember.SerializeDelta(stream, this, previous);
			}
		}
	}
}