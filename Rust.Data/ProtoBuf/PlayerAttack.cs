using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class PlayerAttack : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public Attack attack;

		[NonSerialized]
		public int projectileID;

		public bool ShouldPool = true;

		private bool _disposed;

		public PlayerAttack()
		{
		}

		public PlayerAttack Copy()
		{
			PlayerAttack playerAttack = Pool.Get<PlayerAttack>();
			this.CopyTo(playerAttack);
			return playerAttack;
		}

		public void CopyTo(PlayerAttack instance)
		{
			if (this.attack == null)
			{
				instance.attack = null;
			}
			else if (instance.attack != null)
			{
				this.attack.CopyTo(instance.attack);
			}
			else
			{
				instance.attack = this.attack.Copy();
			}
			instance.projectileID = this.projectileID;
		}

		public static PlayerAttack Deserialize(Stream stream)
		{
			PlayerAttack playerAttack = Pool.Get<PlayerAttack>();
			PlayerAttack.Deserialize(stream, playerAttack, false);
			return playerAttack;
		}

		public static PlayerAttack Deserialize(byte[] buffer)
		{
			PlayerAttack playerAttack = Pool.Get<PlayerAttack>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerAttack.Deserialize(memoryStream, playerAttack, false);
			}
			return playerAttack;
		}

		public static PlayerAttack Deserialize(byte[] buffer, PlayerAttack instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerAttack.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static PlayerAttack Deserialize(Stream stream, PlayerAttack instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num != 10)
				{
					if (num == 16)
					{
						instance.projectileID = (int)ProtocolParser.ReadUInt64(stream);
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
				else if (instance.attack != null)
				{
					Attack.DeserializeLengthDelimited(stream, instance.attack, isDelta);
				}
				else
				{
					instance.attack = Attack.DeserializeLengthDelimited(stream);
				}
			}
			return instance;
		}

		public static PlayerAttack DeserializeLength(Stream stream, int length)
		{
			PlayerAttack playerAttack = Pool.Get<PlayerAttack>();
			PlayerAttack.DeserializeLength(stream, length, playerAttack, false);
			return playerAttack;
		}

		public static PlayerAttack DeserializeLength(Stream stream, int length, PlayerAttack instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num != 10)
				{
					if (num == 16)
					{
						instance.projectileID = (int)ProtocolParser.ReadUInt64(stream);
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
				else if (instance.attack != null)
				{
					Attack.DeserializeLengthDelimited(stream, instance.attack, isDelta);
				}
				else
				{
					instance.attack = Attack.DeserializeLengthDelimited(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static PlayerAttack DeserializeLengthDelimited(Stream stream)
		{
			PlayerAttack playerAttack = Pool.Get<PlayerAttack>();
			PlayerAttack.DeserializeLengthDelimited(stream, playerAttack, false);
			return playerAttack;
		}

		public static PlayerAttack DeserializeLengthDelimited(Stream stream, PlayerAttack instance, bool isDelta)
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
				if (num != 10)
				{
					if (num == 16)
					{
						instance.projectileID = (int)ProtocolParser.ReadUInt64(stream);
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
				else if (instance.attack != null)
				{
					Attack.DeserializeLengthDelimited(stream, instance.attack, isDelta);
				}
				else
				{
					instance.attack = Attack.DeserializeLengthDelimited(stream);
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
			PlayerAttack.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			PlayerAttack.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(PlayerAttack instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.attack != null)
			{
				instance.attack.ResetToPool();
				instance.attack = null;
			}
			instance.projectileID = 0;
			Pool.Free<PlayerAttack>(ref instance);
		}

		public void ResetToPool()
		{
			PlayerAttack.ResetToPool(this);
		}

		public static void Serialize(Stream stream, PlayerAttack instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.attack == null)
			{
				throw new ArgumentNullException("attack", "Required by proto specification.");
			}
			stream.WriteByte(10);
			memoryStream.SetLength((long)0);
			Attack.Serialize(memoryStream, instance.attack);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.projectileID);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, PlayerAttack instance, PlayerAttack previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.attack == null)
			{
				throw new ArgumentNullException("attack", "Required by proto specification.");
			}
			stream.WriteByte(10);
			memoryStream.SetLength((long)0);
			Attack.SerializeDelta(memoryStream, instance.attack, previous.attack);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			if (instance.projectileID != previous.projectileID)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.projectileID);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, PlayerAttack instance)
		{
			byte[] bytes = PlayerAttack.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(PlayerAttack instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				PlayerAttack.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			PlayerAttack.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return PlayerAttack.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			PlayerAttack.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, PlayerAttack previous)
		{
			if (previous == null)
			{
				PlayerAttack.Serialize(stream, this);
				return;
			}
			PlayerAttack.SerializeDelta(stream, this, previous);
		}
	}
}