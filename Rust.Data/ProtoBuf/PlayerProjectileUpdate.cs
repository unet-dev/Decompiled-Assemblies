using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;
using UnityEngine;

namespace ProtoBuf
{
	public class PlayerProjectileUpdate : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public int projectileID;

		[NonSerialized]
		public Vector3 curPosition;

		[NonSerialized]
		public Vector3 curVelocity;

		[NonSerialized]
		public float travelTime;

		public bool ShouldPool = true;

		private bool _disposed;

		public PlayerProjectileUpdate()
		{
		}

		public PlayerProjectileUpdate Copy()
		{
			PlayerProjectileUpdate playerProjectileUpdate = Pool.Get<PlayerProjectileUpdate>();
			this.CopyTo(playerProjectileUpdate);
			return playerProjectileUpdate;
		}

		public void CopyTo(PlayerProjectileUpdate instance)
		{
			instance.projectileID = this.projectileID;
			instance.curPosition = this.curPosition;
			instance.curVelocity = this.curVelocity;
			instance.travelTime = this.travelTime;
		}

		public static PlayerProjectileUpdate Deserialize(Stream stream)
		{
			PlayerProjectileUpdate playerProjectileUpdate = Pool.Get<PlayerProjectileUpdate>();
			PlayerProjectileUpdate.Deserialize(stream, playerProjectileUpdate, false);
			return playerProjectileUpdate;
		}

		public static PlayerProjectileUpdate Deserialize(byte[] buffer)
		{
			PlayerProjectileUpdate playerProjectileUpdate = Pool.Get<PlayerProjectileUpdate>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerProjectileUpdate.Deserialize(memoryStream, playerProjectileUpdate, false);
			}
			return playerProjectileUpdate;
		}

		public static PlayerProjectileUpdate Deserialize(byte[] buffer, PlayerProjectileUpdate instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerProjectileUpdate.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static PlayerProjectileUpdate Deserialize(Stream stream, PlayerProjectileUpdate instance, bool isDelta)
		{
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
						instance.projectileID = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 18)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.curPosition, isDelta);
						continue;
					}
				}
				else if (num == 26)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.curVelocity, isDelta);
					continue;
				}
				else if (num == 37)
				{
					instance.travelTime = ProtocolParser.ReadSingle(stream);
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

		public static PlayerProjectileUpdate DeserializeLength(Stream stream, int length)
		{
			PlayerProjectileUpdate playerProjectileUpdate = Pool.Get<PlayerProjectileUpdate>();
			PlayerProjectileUpdate.DeserializeLength(stream, length, playerProjectileUpdate, false);
			return playerProjectileUpdate;
		}

		public static PlayerProjectileUpdate DeserializeLength(Stream stream, int length, PlayerProjectileUpdate instance, bool isDelta)
		{
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
						instance.projectileID = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 18)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.curPosition, isDelta);
						continue;
					}
				}
				else if (num == 26)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.curVelocity, isDelta);
					continue;
				}
				else if (num == 37)
				{
					instance.travelTime = ProtocolParser.ReadSingle(stream);
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

		public static PlayerProjectileUpdate DeserializeLengthDelimited(Stream stream)
		{
			PlayerProjectileUpdate playerProjectileUpdate = Pool.Get<PlayerProjectileUpdate>();
			PlayerProjectileUpdate.DeserializeLengthDelimited(stream, playerProjectileUpdate, false);
			return playerProjectileUpdate;
		}

		public static PlayerProjectileUpdate DeserializeLengthDelimited(Stream stream, PlayerProjectileUpdate instance, bool isDelta)
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
				if (num <= 18)
				{
					if (num == 8)
					{
						instance.projectileID = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 18)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.curPosition, isDelta);
						continue;
					}
				}
				else if (num == 26)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.curVelocity, isDelta);
					continue;
				}
				else if (num == 37)
				{
					instance.travelTime = ProtocolParser.ReadSingle(stream);
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
			PlayerProjectileUpdate.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			PlayerProjectileUpdate.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(PlayerProjectileUpdate instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.projectileID = 0;
			instance.curPosition = new Vector3();
			instance.curVelocity = new Vector3();
			instance.travelTime = 0f;
			Pool.Free<PlayerProjectileUpdate>(ref instance);
		}

		public void ResetToPool()
		{
			PlayerProjectileUpdate.ResetToPool(this);
		}

		public static void Serialize(Stream stream, PlayerProjectileUpdate instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.projectileID);
			stream.WriteByte(18);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.curPosition);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			stream.WriteByte(26);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.curVelocity);
			uint num = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			stream.WriteByte(37);
			ProtocolParser.WriteSingle(stream, instance.travelTime);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, PlayerProjectileUpdate instance, PlayerProjectileUpdate previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.projectileID != previous.projectileID)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.projectileID);
			}
			if (instance.curPosition != previous.curPosition)
			{
				stream.WriteByte(18);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.curPosition, previous.curPosition);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.curVelocity != previous.curVelocity)
			{
				stream.WriteByte(26);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.curVelocity, previous.curVelocity);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			if (instance.travelTime != previous.travelTime)
			{
				stream.WriteByte(37);
				ProtocolParser.WriteSingle(stream, instance.travelTime);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, PlayerProjectileUpdate instance)
		{
			byte[] bytes = PlayerProjectileUpdate.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(PlayerProjectileUpdate instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				PlayerProjectileUpdate.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			PlayerProjectileUpdate.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return PlayerProjectileUpdate.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			PlayerProjectileUpdate.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, PlayerProjectileUpdate previous)
		{
			if (previous == null)
			{
				PlayerProjectileUpdate.Serialize(stream, this);
				return;
			}
			PlayerProjectileUpdate.SerializeDelta(stream, this, previous);
		}
	}
}