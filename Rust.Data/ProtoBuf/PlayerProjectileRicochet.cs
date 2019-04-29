using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;
using UnityEngine;

namespace ProtoBuf
{
	public class PlayerProjectileRicochet : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public int projectileID;

		[NonSerialized]
		public Vector3 hitPosition;

		[NonSerialized]
		public Vector3 inVelocity;

		[NonSerialized]
		public Vector3 outVelocity;

		[NonSerialized]
		public Vector3 hitNormal;

		[NonSerialized]
		public float travelTime;

		public bool ShouldPool = true;

		private bool _disposed;

		public PlayerProjectileRicochet()
		{
		}

		public PlayerProjectileRicochet Copy()
		{
			PlayerProjectileRicochet playerProjectileRicochet = Pool.Get<PlayerProjectileRicochet>();
			this.CopyTo(playerProjectileRicochet);
			return playerProjectileRicochet;
		}

		public void CopyTo(PlayerProjectileRicochet instance)
		{
			instance.projectileID = this.projectileID;
			instance.hitPosition = this.hitPosition;
			instance.inVelocity = this.inVelocity;
			instance.outVelocity = this.outVelocity;
			instance.hitNormal = this.hitNormal;
			instance.travelTime = this.travelTime;
		}

		public static PlayerProjectileRicochet Deserialize(Stream stream)
		{
			PlayerProjectileRicochet playerProjectileRicochet = Pool.Get<PlayerProjectileRicochet>();
			PlayerProjectileRicochet.Deserialize(stream, playerProjectileRicochet, false);
			return playerProjectileRicochet;
		}

		public static PlayerProjectileRicochet Deserialize(byte[] buffer)
		{
			PlayerProjectileRicochet playerProjectileRicochet = Pool.Get<PlayerProjectileRicochet>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerProjectileRicochet.Deserialize(memoryStream, playerProjectileRicochet, false);
			}
			return playerProjectileRicochet;
		}

		public static PlayerProjectileRicochet Deserialize(byte[] buffer, PlayerProjectileRicochet instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerProjectileRicochet.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static PlayerProjectileRicochet Deserialize(Stream stream, PlayerProjectileRicochet instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 26)
				{
					if (num == 8)
					{
						instance.projectileID = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 18)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitPosition, isDelta);
						continue;
					}
					else if (num == 26)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.inVelocity, isDelta);
						continue;
					}
				}
				else if (num == 34)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.outVelocity, isDelta);
					continue;
				}
				else if (num == 42)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitNormal, isDelta);
					continue;
				}
				else if (num == 53)
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

		public static PlayerProjectileRicochet DeserializeLength(Stream stream, int length)
		{
			PlayerProjectileRicochet playerProjectileRicochet = Pool.Get<PlayerProjectileRicochet>();
			PlayerProjectileRicochet.DeserializeLength(stream, length, playerProjectileRicochet, false);
			return playerProjectileRicochet;
		}

		public static PlayerProjectileRicochet DeserializeLength(Stream stream, int length, PlayerProjectileRicochet instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num <= 26)
				{
					if (num == 8)
					{
						instance.projectileID = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 18)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitPosition, isDelta);
						continue;
					}
					else if (num == 26)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.inVelocity, isDelta);
						continue;
					}
				}
				else if (num == 34)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.outVelocity, isDelta);
					continue;
				}
				else if (num == 42)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitNormal, isDelta);
					continue;
				}
				else if (num == 53)
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

		public static PlayerProjectileRicochet DeserializeLengthDelimited(Stream stream)
		{
			PlayerProjectileRicochet playerProjectileRicochet = Pool.Get<PlayerProjectileRicochet>();
			PlayerProjectileRicochet.DeserializeLengthDelimited(stream, playerProjectileRicochet, false);
			return playerProjectileRicochet;
		}

		public static PlayerProjectileRicochet DeserializeLengthDelimited(Stream stream, PlayerProjectileRicochet instance, bool isDelta)
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
				if (num <= 26)
				{
					if (num == 8)
					{
						instance.projectileID = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 18)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitPosition, isDelta);
						continue;
					}
					else if (num == 26)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.inVelocity, isDelta);
						continue;
					}
				}
				else if (num == 34)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.outVelocity, isDelta);
					continue;
				}
				else if (num == 42)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitNormal, isDelta);
					continue;
				}
				else if (num == 53)
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
			PlayerProjectileRicochet.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			PlayerProjectileRicochet.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(PlayerProjectileRicochet instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.projectileID = 0;
			instance.hitPosition = new Vector3();
			instance.inVelocity = new Vector3();
			instance.outVelocity = new Vector3();
			instance.hitNormal = new Vector3();
			instance.travelTime = 0f;
			Pool.Free<PlayerProjectileRicochet>(ref instance);
		}

		public void ResetToPool()
		{
			PlayerProjectileRicochet.ResetToPool(this);
		}

		public static void Serialize(Stream stream, PlayerProjectileRicochet instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.projectileID);
			stream.WriteByte(18);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.hitPosition);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			stream.WriteByte(26);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.inVelocity);
			uint num = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			stream.WriteByte(34);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.outVelocity);
			uint length1 = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length1);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
			stream.WriteByte(42);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.hitNormal);
			uint num1 = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num1);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
			stream.WriteByte(53);
			ProtocolParser.WriteSingle(stream, instance.travelTime);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, PlayerProjectileRicochet instance, PlayerProjectileRicochet previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.projectileID != previous.projectileID)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.projectileID);
			}
			if (instance.hitPosition != previous.hitPosition)
			{
				stream.WriteByte(18);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.hitPosition, previous.hitPosition);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.inVelocity != previous.inVelocity)
			{
				stream.WriteByte(26);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.inVelocity, previous.inVelocity);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			if (instance.outVelocity != previous.outVelocity)
			{
				stream.WriteByte(34);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.outVelocity, previous.outVelocity);
				uint length1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
			}
			if (instance.hitNormal != previous.hitNormal)
			{
				stream.WriteByte(42);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.hitNormal, previous.hitNormal);
				uint num1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
			}
			if (instance.travelTime != previous.travelTime)
			{
				stream.WriteByte(53);
				ProtocolParser.WriteSingle(stream, instance.travelTime);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, PlayerProjectileRicochet instance)
		{
			byte[] bytes = PlayerProjectileRicochet.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(PlayerProjectileRicochet instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				PlayerProjectileRicochet.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			PlayerProjectileRicochet.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return PlayerProjectileRicochet.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			PlayerProjectileRicochet.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, PlayerProjectileRicochet previous)
		{
			if (previous == null)
			{
				PlayerProjectileRicochet.Serialize(stream, this);
				return;
			}
			PlayerProjectileRicochet.SerializeDelta(stream, this, previous);
		}
	}
}