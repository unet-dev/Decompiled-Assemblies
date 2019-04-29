using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;
using UnityEngine;

namespace ProtoBuf
{
	public class PlayerProjectileAttack : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public PlayerAttack playerAttack;

		[NonSerialized]
		public Vector3 hitVelocity;

		[NonSerialized]
		public float hitDistance;

		[NonSerialized]
		public float travelTime;

		public bool ShouldPool = true;

		private bool _disposed;

		public PlayerProjectileAttack()
		{
		}

		public PlayerProjectileAttack Copy()
		{
			PlayerProjectileAttack playerProjectileAttack = Pool.Get<PlayerProjectileAttack>();
			this.CopyTo(playerProjectileAttack);
			return playerProjectileAttack;
		}

		public void CopyTo(PlayerProjectileAttack instance)
		{
			if (this.playerAttack == null)
			{
				instance.playerAttack = null;
			}
			else if (instance.playerAttack != null)
			{
				this.playerAttack.CopyTo(instance.playerAttack);
			}
			else
			{
				instance.playerAttack = this.playerAttack.Copy();
			}
			instance.hitVelocity = this.hitVelocity;
			instance.hitDistance = this.hitDistance;
			instance.travelTime = this.travelTime;
		}

		public static PlayerProjectileAttack Deserialize(Stream stream)
		{
			PlayerProjectileAttack playerProjectileAttack = Pool.Get<PlayerProjectileAttack>();
			PlayerProjectileAttack.Deserialize(stream, playerProjectileAttack, false);
			return playerProjectileAttack;
		}

		public static PlayerProjectileAttack Deserialize(byte[] buffer)
		{
			PlayerProjectileAttack playerProjectileAttack = Pool.Get<PlayerProjectileAttack>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerProjectileAttack.Deserialize(memoryStream, playerProjectileAttack, false);
			}
			return playerProjectileAttack;
		}

		public static PlayerProjectileAttack Deserialize(byte[] buffer, PlayerProjectileAttack instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerProjectileAttack.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static PlayerProjectileAttack Deserialize(Stream stream, PlayerProjectileAttack instance, bool isDelta)
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
					if (num != 10)
					{
						if (num == 18)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitVelocity, isDelta);
							continue;
						}
					}
					else if (instance.playerAttack != null)
					{
						PlayerAttack.DeserializeLengthDelimited(stream, instance.playerAttack, isDelta);
						continue;
					}
					else
					{
						instance.playerAttack = PlayerAttack.DeserializeLengthDelimited(stream);
						continue;
					}
				}
				else if (num == 29)
				{
					instance.hitDistance = ProtocolParser.ReadSingle(stream);
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

		public static PlayerProjectileAttack DeserializeLength(Stream stream, int length)
		{
			PlayerProjectileAttack playerProjectileAttack = Pool.Get<PlayerProjectileAttack>();
			PlayerProjectileAttack.DeserializeLength(stream, length, playerProjectileAttack, false);
			return playerProjectileAttack;
		}

		public static PlayerProjectileAttack DeserializeLength(Stream stream, int length, PlayerProjectileAttack instance, bool isDelta)
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
					if (num != 10)
					{
						if (num == 18)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitVelocity, isDelta);
							continue;
						}
					}
					else if (instance.playerAttack != null)
					{
						PlayerAttack.DeserializeLengthDelimited(stream, instance.playerAttack, isDelta);
						continue;
					}
					else
					{
						instance.playerAttack = PlayerAttack.DeserializeLengthDelimited(stream);
						continue;
					}
				}
				else if (num == 29)
				{
					instance.hitDistance = ProtocolParser.ReadSingle(stream);
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

		public static PlayerProjectileAttack DeserializeLengthDelimited(Stream stream)
		{
			PlayerProjectileAttack playerProjectileAttack = Pool.Get<PlayerProjectileAttack>();
			PlayerProjectileAttack.DeserializeLengthDelimited(stream, playerProjectileAttack, false);
			return playerProjectileAttack;
		}

		public static PlayerProjectileAttack DeserializeLengthDelimited(Stream stream, PlayerProjectileAttack instance, bool isDelta)
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
					if (num != 10)
					{
						if (num == 18)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitVelocity, isDelta);
							continue;
						}
					}
					else if (instance.playerAttack != null)
					{
						PlayerAttack.DeserializeLengthDelimited(stream, instance.playerAttack, isDelta);
						continue;
					}
					else
					{
						instance.playerAttack = PlayerAttack.DeserializeLengthDelimited(stream);
						continue;
					}
				}
				else if (num == 29)
				{
					instance.hitDistance = ProtocolParser.ReadSingle(stream);
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
			PlayerProjectileAttack.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			PlayerProjectileAttack.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(PlayerProjectileAttack instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.playerAttack != null)
			{
				instance.playerAttack.ResetToPool();
				instance.playerAttack = null;
			}
			instance.hitVelocity = new Vector3();
			instance.hitDistance = 0f;
			instance.travelTime = 0f;
			Pool.Free<PlayerProjectileAttack>(ref instance);
		}

		public void ResetToPool()
		{
			PlayerProjectileAttack.ResetToPool(this);
		}

		public static void Serialize(Stream stream, PlayerProjectileAttack instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.playerAttack == null)
			{
				throw new ArgumentNullException("playerAttack", "Required by proto specification.");
			}
			stream.WriteByte(10);
			memoryStream.SetLength((long)0);
			PlayerAttack.Serialize(memoryStream, instance.playerAttack);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			stream.WriteByte(18);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.hitVelocity);
			uint num = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			stream.WriteByte(29);
			ProtocolParser.WriteSingle(stream, instance.hitDistance);
			stream.WriteByte(37);
			ProtocolParser.WriteSingle(stream, instance.travelTime);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, PlayerProjectileAttack instance, PlayerProjectileAttack previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.playerAttack == null)
			{
				throw new ArgumentNullException("playerAttack", "Required by proto specification.");
			}
			stream.WriteByte(10);
			memoryStream.SetLength((long)0);
			PlayerAttack.SerializeDelta(memoryStream, instance.playerAttack, previous.playerAttack);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			if (instance.hitVelocity != previous.hitVelocity)
			{
				stream.WriteByte(18);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.hitVelocity, previous.hitVelocity);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			if (instance.hitDistance != previous.hitDistance)
			{
				stream.WriteByte(29);
				ProtocolParser.WriteSingle(stream, instance.hitDistance);
			}
			if (instance.travelTime != previous.travelTime)
			{
				stream.WriteByte(37);
				ProtocolParser.WriteSingle(stream, instance.travelTime);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, PlayerProjectileAttack instance)
		{
			byte[] bytes = PlayerProjectileAttack.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(PlayerProjectileAttack instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				PlayerProjectileAttack.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			PlayerProjectileAttack.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return PlayerProjectileAttack.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			PlayerProjectileAttack.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, PlayerProjectileAttack previous)
		{
			if (previous == null)
			{
				PlayerProjectileAttack.Serialize(stream, this);
				return;
			}
			PlayerProjectileAttack.SerializeDelta(stream, this, previous);
		}
	}
}